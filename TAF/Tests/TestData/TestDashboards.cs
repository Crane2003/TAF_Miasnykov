using TAF.Business.Models;

namespace TAF.Tests.TestData;

public static class TestDashboards
{
    public static Dashboard SimpleDashboard => new()
    {
        Name = $"Simple Dashboard {DateTime.Now:yyyyMMddHHmmss}",
        Description = "A simple test dashboard with no widgets",
        Owner = "testuser"
    };

    public static Dashboard DashboardWithChart => new()
    {
        Name = $"Chart Dashboard {DateTime.Now:yyyyMMddHHmmss}",
        Description = "Dashboard with chart widgets",
        Owner = "testuser",
        Widgets = new List<Widget>
        {
            Widget.CreateChartWidget("Sales Chart", 0),
            Widget.CreateChartWidget("Revenue Chart", 1)
        }
    };

    public static Dashboard DashboardWithMixedWidgets => new()
    {
        Name = $"Mixed Dashboard {DateTime.Now:yyyyMMddHHmmss}",
        Description = "Dashboard with different widget types",
        Owner = "testuser",
        Widgets = new List<Widget>
        {
            Widget.CreateChartWidget("Performance Chart", 0),
            Widget.CreateTableWidget("Data Table", 1),
            Widget.CreateChartWidget("Trends Chart", 2)
        }
    };

    public static Dashboard EmptyDashboard => new()
    {
        Name = $"Empty Dashboard {DateTime.Now:yyyyMMddHHmmss}",
        Description = "Dashboard for testing widget additions",
        Owner = "testuser"
    };

    public static Dashboard CreateDashboardWithWidgets(int widgetCount)
    {
        var dashboard = new Dashboard
        {
            Name = $"Dashboard with {widgetCount} widgets {DateTime.Now:yyyyMMddHHmmss}",
            Description = $"Test dashboard containing {widgetCount} widgets",
            Owner = "testuser"
        };

        for (int i = 0; i < widgetCount; i++)
        {
            var widget = i % 2 == 0
                ? Widget.CreateChartWidget($"Chart Widget {i + 1}", i)
                : Widget.CreateTableWidget($"Table Widget {i + 1}", i);

            dashboard.AddWidget(widget);
        }

        return dashboard;
    }

    public static List<Dashboard> GetMultipleDashboards(int count)
    {
        var dashboards = new List<Dashboard>();

        for (int i = 0; i < count; i++)
        {
            dashboards.Add(new Dashboard
            {
                Name = $"Test Dashboard {i + 1} {DateTime.Now:yyyyMMddHHmmss}",
                Description = $"Test dashboard number {i + 1}",
                Owner = "testuser"
            });
        }

        return dashboards;
    }
}
