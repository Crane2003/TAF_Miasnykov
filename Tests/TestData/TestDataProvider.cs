using Business.Models;
using Core.Utilities;

namespace Tests.TestData;

public static class TestDataProvider
{
    public static IEnumerable<DashboardCreateRequest> DashboardCreateCases =>
    [
        new DashboardCreateRequest
        {
            Name = "Short Dashboard".Unique(),
            Description = "Short desc"
        },
        new DashboardCreateRequest
        {
            Name = "Performance Metrics Dashboard — Q3 2025 Extended View".Unique(),
            Description = "Comprehensive overview of all performance KPIs tracked during the third quarter of 2025, " +
                          "including throughput, error rate, and latency percentiles across all microservices."
        },
        new DashboardCreateRequest
        {
            Name = "Dashboard 007 v2".Unique(),
            Description = "Dashboard with numeric identifiers and version suffix in its name"
        },
        new DashboardCreateRequest
        {
            Name = "CI/CD Pipeline & Build Status".Unique(),
            Description = "Tracks build success/failure rates, deployment frequency, and mean-time-to-recovery"
        },
        new DashboardCreateRequest
        {
            Name = "Dashboard Unicode — ünïcödé テスト".Unique(),
            Description = "Dashboard with Unicode characters in both name and description — 日本語テスト content"
        }
    ];

    public static IEnumerable<(DashboardCreateRequest Create, DashboardUpdateRequest Update)> DashboardUpdateCases =>
    [
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Name Only".Unique(),
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "Renamed Dashboard".Unique(),
                Description = "Automated test dashboard"
            }
        ),
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Desc Only".Unique(),
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "Update Test — Desc Only".Unique(),
                Description = "Description was changed while name stayed the same"
            }
        ),
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Short Values".Unique(),
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "X".Unique(),
                Description = "Y"
            }
        ),
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Long Values".Unique(),
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "Very Long Updated Dashboard Name That Exceeds Typical Title Length Boundaries — Extended".Unique(),
                Description = "An extensively updated description intended to verify that the API handles " +
                              "large text payloads gracefully without truncation or validation errors."
            }
        ),
        (
            new DashboardCreateRequest
            {
                Name = "Update Test — Special Chars".Unique(),
                Description = "Initial description — will be overwritten"
            },
            new DashboardUpdateRequest
            {
                Name = "Updated: CI/CD & Monitoring (v3.0) [PROD]".Unique(),
                Description = "Special chars: <html> & \"quotes\" 'apostrophes' \\backslashes\\ — em-dash"
            }
        )
    ];

    // Types must be the widget types ReportPortal actually stores.
    // Names are kept stable here so test case names stay
    // readable; tests make them unique at runtime because names are unique per project.
    public static IEnumerable<Widget> WidgetCases =>
    [
        new Widget { Name = "Overall Statistics Panel", Type = "overallStatistics", Size = new WidgetSize { Width = 6,  Height = 7 }, Position = new WidgetPosition { PositionX = 0, PositionY = 0  } },
        new Widget { Name = "Launch Statistics Chart",  Type = "statisticTrend", Size = new WidgetSize { Width = 6,  Height = 4 }, Position = new WidgetPosition { PositionX = 6, PositionY = 0  } },
        new Widget { Name = "Failed Tests Table",       Type = "launchesTable", Size = new WidgetSize { Width = 12, Height = 6 }, Position = new WidgetPosition { PositionX = 0, PositionY = 7  } },
        new Widget { Name = "Passing Rate Trend",       Type = "statisticTrend", Size = new WidgetSize { Width = 6,  Height = 4 }, Position = new WidgetPosition { PositionX = 0, PositionY = 11 } },
        new Widget { Name = "Top Failing Tests",        Type = "launchesTable", Size = new WidgetSize { Width = 12, Height = 6 }, Position = new WidgetPosition { PositionX = 0, PositionY = 15 } }
    ];
}
