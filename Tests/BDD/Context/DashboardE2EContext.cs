using Business.Services;

namespace Tests.BDD.Context;

public sealed class DashboardE2EContext
{
    public DashboardApiService ApiService { get; set; } = null!;
    public DashboardUiService UiService { get; set; } = null!;
    public AuthenticationService AuthService { get; set; } = null!;

    public List<int> CreatedDashboardIds { get; } = new();

    public void TrackDashboard(int id) => CreatedDashboardIds.Add(id);

    public string? LastCreatedDashboardName { get; set; }
}
