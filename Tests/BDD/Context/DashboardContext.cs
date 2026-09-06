using Business.Models;
using Business.Services;

namespace Tests.BDD.Context;

public sealed class DashboardContext
{
    public DashboardApiService ApiService { get; set; } = null!;

    public List<int> CreatedDashboardIds { get; } = new();

    public DashboardCreateRequest? PendingCreateRequest { get; set; }

    public Dashboard? CurrentDashboard { get; set; }

    public Dashboard? UpdatedDashboard { get; set; }

    public string? OriginalDashboardName { get; set; }

    public List<Widget> AddedWidgets { get; } = new();

    public List<string> PendingDashboardNames { get; set; } = new();

    public List<Dashboard> CreatedDashboardList { get; } = new();

    public void TrackDashboard(int id) => CreatedDashboardIds.Add(id);
}
