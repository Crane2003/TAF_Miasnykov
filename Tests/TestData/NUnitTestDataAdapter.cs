namespace Tests.TestData;

public static class NUnitTestDataAdapter
{
    public static IEnumerable<TestCaseData> DashboardCreateCases =>
        TestDataProvider.DashboardCreateCases.Select(r =>
            new TestCaseData(r).SetName($"CreateDashboard — \"{r.Name}\""));

    public static IEnumerable<TestCaseData> DashboardUpdateCases =>
        TestDataProvider.DashboardUpdateCases.Select(p =>
            new TestCaseData(p.Create, p.Update).SetName($"UpdateDashboard — \"{p.Create.Name}\" → \"{p.Update.Name}\""));

    public static IEnumerable<TestCaseData> WidgetCases =>
        TestDataProvider.WidgetCases.Select(w =>
            new TestCaseData(w).SetName($"AddWidget — \"{w.Name}\" ({w.Type}) at ({w.Position.PositionX},{w.Position.PositionY})"));
}
