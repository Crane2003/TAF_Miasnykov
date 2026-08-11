using Business.Models;

namespace Tests.TestData;

public static class TestDataProvider
{
    public static IEnumerable<DashboardCreateRequest> DashboardCreateCases { get; } =
    [
        new DashboardCreateRequest
        {
            Name = "Short Dashboard",
            Description = "Short desc"
        },
        new DashboardCreateRequest
        {
            Name = "Performance Metrics Dashboard — Q3 2025 Extended View",
            Description = "Comprehensive overview of all performance KPIs tracked during the third quarter of 2025, " +
                          "including throughput, error rate, and latency percentiles across all microservices."
        },
        new DashboardCreateRequest
        {
            Name = "Dashboard #007 v2.0",
            Description = "Dashboard with numeric identifiers and version suffix in its name"
        },
        new DashboardCreateRequest
        {
            Name = "CI/CD Pipeline & Build Status",
            Description = "Tracks build success/failure rates, deployment frequency, and mean-time-to-recovery"
        },
        new DashboardCreateRequest
        {
            Name = "Dashboard Unicode — ünïcödé テスト",
            Description = "Dashboard with Unicode characters in both name and description — 日本語テスト content"
        }
    ];

    public static IEnumerable<(DashboardCreateRequest Create, DashboardUpdateRequest Update)> DashboardUpdateCases { get; } =
    [
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Name Only",
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "Renamed Dashboard",
                Description = "Automated test dashboard"
            }
        ),
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Desc Only",
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "Update Test — Desc Only",
                Description = "Description was changed while name stayed the same"
            }
        ),
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Short Values",
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "X",
                Description = "Y"
            }
        ),
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Long Values",
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "Very Long Updated Dashboard Name That Exceeds Typical Title Length Boundaries — Extended",
                Description = "An extensively updated description intended to verify that the API handles " +
                              "large text payloads gracefully without truncation or validation errors."
            }
        ),
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Special Chars",
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "Updated: CI/CD & Monitoring (v3.0) [PROD]",
                Description = "Special chars: <html> & \"quotes\" 'apostrophes' \\backslashes\\ — em-dash"
            }
        )
    ];

    public static IEnumerable<Widget> WidgetCases { get; } =
    [
        new Widget { Name = "Overall Statistics Panel", Type = "overallStatistics", Size = new WidgetSize { Width = 6,  Height = 7 }, Position = new WidgetPosition { PositionX = 0, PositionY = 0  } },
        new Widget { Name = "Launch Statistics Chart",  Type = "chart", Size = new WidgetSize { Width = 6,  Height = 4 }, Position = new WidgetPosition { PositionX = 6, PositionY = 0  } },
        new Widget { Name = "Failed Tests Table",       Type = "table", Size = new WidgetSize { Width = 12, Height = 6 }, Position = new WidgetPosition { PositionX = 0, PositionY = 7  } },
        new Widget { Name = "Passing Rate Trend",       Type = "chart", Size = new WidgetSize { Width = 6,  Height = 4 }, Position = new WidgetPosition { PositionX = 0, PositionY = 11 } },
        new Widget { Name = "Top Failing Tests",        Type = "table", Size = new WidgetSize { Width = 12, Height = 5 }, Position = new WidgetPosition { PositionX = 0, PositionY = 15 } }
    ];
}
