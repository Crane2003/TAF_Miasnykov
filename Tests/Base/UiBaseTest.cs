using System.Reflection;
using Core.Configuration;
using Core.Driver;
using Core.Jira;

namespace Tests.Base;

public class UiBaseTest : BaseTest
{
    private static readonly Lazy<JiraClient> LazyJiraClient = new(() =>
        new JiraClient(ConfigurationLoader.Instance.GetJiraConfiguration()));

    [SetUp]
    public void InitDriver()
    {
        UiTestSupport.InitDriver(Configuration);
    }

    [TearDown]
    public override void TearDown()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        var testFailed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;

        UiTestSupport.QuitWithScreenshot(
            DriverManager.CurrentDriver,
            testName,
            testFailed,
            Configuration.TakeScreenshotOnFailure,
            Logger);

        ReportTestResultToJira(testFailed);

        base.TearDown();
    }

    private void ReportTestResultToJira(bool testFailed)
    {
        var jiraAttribute = GetCurrentTestJiraAttribute();
        if (jiraAttribute == null)
            return;

        var outcome = testFailed ? JiraTestOutcome.Failed : JiraTestOutcome.Passed;

        try
        {
            LazyJiraClient.Value.UpdateTestCaseStatusAsync(jiraAttribute.IssueKey, outcome)
                .GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Failed to report test result to Jira for issue {IssueKey}", jiraAttribute.IssueKey);
        }
    }

    private static JiraTestCaseAttribute? GetCurrentTestJiraAttribute()
    {
        var test = TestContext.CurrentContext.Test;
        var methodName = test.MethodName;
        if (string.IsNullOrEmpty(methodName))
            return null;

        var classType = Type.GetType(test.ClassName ?? string.Empty)
            ?? AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(test.ClassName ?? string.Empty))
                .FirstOrDefault(t => t != null);

        var method = classType?.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        return method?.GetCustomAttribute<JiraTestCaseAttribute>();
    }
}
