namespace Core.Jira;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class JiraTestCaseAttribute : Attribute
{
    public string IssueKey { get; }

    public JiraTestCaseAttribute(string issueKey)
    {
        if (string.IsNullOrWhiteSpace(issueKey))
            throw new ArgumentException("Jira issue key must not be empty.", nameof(issueKey));

        IssueKey = issueKey;
    }
}
