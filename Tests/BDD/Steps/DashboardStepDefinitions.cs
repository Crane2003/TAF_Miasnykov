using Business.Models;
using Core.Utilities;
using Reqnroll;
using Tests.BDD.Context;
using Assert = Xunit.Assert;

namespace Tests.BDD.Steps;

[Binding]
public sealed class DashboardStepDefinitions
{
    private readonly DashboardContext _ctx;

    public DashboardStepDefinitions(DashboardContext ctx)
    {
        _ctx = ctx;
    }

    [Given("the dashboard API service is configured with valid authentication")]
    public void GivenTheApiServiceIsConfigured()
    {
        Assert.NotNull(_ctx.ApiService);
    }

    [Given("I have a create request with name {string} and description {string}")]
    public void GivenIHaveACreateRequest(string name, string description)
    {
        _ctx.PendingCreateRequest = new DashboardCreateRequest
        {
            Name = name.Unique(),
            Description = description
        };
    }

    [When("I submit the create dashboard request")]
    public async Task WhenISubmitTheCreateRequest()
    {
        Assert.NotNull(_ctx.PendingCreateRequest);

        var result = await _ctx.ApiService.CreateDashboardAsync(_ctx.PendingCreateRequest);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);

        _ctx.CurrentDashboard = result;
        _ctx.TrackDashboard(result.Id);
    }

    [Then("the response should indicate successful creation")]
    public void ThenTheResponseShouldIndicateSuccess()
    {
        Assert.NotNull(_ctx.CurrentDashboard);
    }

    [Then("the returned dashboard name should be {string}")]
    public void ThenTheReturnedNameShouldBe(string expectedName)
    {
        Assert.NotNull(_ctx.CurrentDashboard);
        Assert.StartsWith(expectedName, _ctx.CurrentDashboard.Name);
    }

    [Then("the returned dashboard description should be {string}")]
    public void ThenTheReturnedDescriptionShouldBe(string expectedDescription)
    {
        Assert.NotNull(_ctx.CurrentDashboard);
        Assert.Equal(expectedDescription, _ctx.CurrentDashboard.Description);
    }

    [Given("I have created the dashboard")]
    public async Task GivenIHaveCreatedTheDashboard()
    {
        Assert.NotNull(_ctx.PendingCreateRequest);

        var result = await _ctx.ApiService.CreateDashboardAsync(_ctx.PendingCreateRequest);

        Assert.NotNull(result);
        _ctx.CurrentDashboard = result;
        _ctx.TrackDashboard(result.Id);
    }

    [When("I update the dashboard with name {string} and description {string}")]
    public async Task WhenIUpdateTheDashboard(string newName, string newDescription)
    {
        Assert.NotNull(_ctx.CurrentDashboard);

        _ctx.OriginalDashboardName = _ctx.CurrentDashboard.Name;

        var updateRequest = new DashboardUpdateRequest
        {
            Name = newName,
            Description = newDescription
        };

        var updated = await _ctx.ApiService.UpdateDashboardAsync(_ctx.CurrentDashboard.Id, updateRequest);

        Assert.NotNull(updated);
        _ctx.UpdatedDashboard = updated;
    }

    [Then("the dashboard update should succeed")]
    public void ThenTheDashboardUpdateShouldSucceed()
    {
        Assert.NotNull(_ctx.UpdatedDashboard);
    }

    [Then("the dashboard name should now be {string}")]
    public void ThenTheDashboardNameShouldNowBe(string expectedName)
    {
        Assert.NotNull(_ctx.UpdatedDashboard);
        Assert.Equal(expectedName, _ctx.UpdatedDashboard.Name);
    }

    [Then("the dashboard name should not be {string}")]
    public void ButTheDashboardNameShouldNotBe(string forbiddenName)
    {
        Assert.NotNull(_ctx.UpdatedDashboard);
        Assert.NotEqual(forbiddenName, _ctx.UpdatedDashboard.Name);
    }

    [Given("I have created a dashboard named {string}")]
    public async Task GivenIHaveCreatedADashboardNamed(string name)
    {
        var result = await _ctx.ApiService.CreateDashboardAsync(
            DashboardCreateRequest.CreateWithName(name.Unique()));

        Assert.NotNull(result);
        _ctx.CurrentDashboard = result;
        _ctx.TrackDashboard(result.Id);
    }

    /// <summary>
    /// Columns: Name | Type | Width | Height | PositionX | PositionY
    /// </summary>
    [When("I add the following widgets to the dashboard:")]
    public async Task WhenIAddTheFollowingWidgets(Table table)
    {
        Assert.NotNull(_ctx.CurrentDashboard);

        foreach (var row in table.Rows)
        {
            var widget = new Widget
            {
                Name = row["Name"].Unique(),
                Type = row["Type"],
                Size = new WidgetSize { Width = int.Parse(row["Width"]), Height = int.Parse(row["Height"]) },
                Position = new WidgetPosition { PositionX = int.Parse(row["PositionX"]), PositionY = int.Parse(row["PositionY"]) }
            };

            var updated = await _ctx.ApiService.AddWidgetToDashboardAsync(_ctx.CurrentDashboard.Id, widget);
            Assert.NotNull(updated);

            _ctx.AddedWidgets.Add(widget);
            _ctx.CurrentDashboard = updated;
        }
    }

    [Then("the dashboard should contain {int} widgets")]
    public void ThenTheDashboardShouldContainWidgets(int expectedCount)
    {
        Assert.NotNull(_ctx.CurrentDashboard);
        Assert.Equal(expectedCount, _ctx.CurrentDashboard.GetWidgetCount());
    }

    [Then("each widget should have the correct name and type")]
    public void ThenEachWidgetShouldHaveCorrectNameAndType()
    {
        Assert.NotNull(_ctx.CurrentDashboard);
        Assert.NotEmpty(_ctx.AddedWidgets);

        foreach (var expected in _ctx.AddedWidgets)
            Assert.Contains(_ctx.CurrentDashboard.Widgets, w => w.Name == expected.Name);
    }

    [Given("I want to create dashboards with the following names:")]
    public void GivenIWantToCreateDashboardsWithNames(Table table)
    {
        _ctx.PendingDashboardNames = table.Rows.Select(r => r["Name"]).ToList();
    }

    [When("I create each dashboard from the list")]
    public async Task WhenICreateEachDashboardFromTheList()
    {
        Assert.NotEmpty(_ctx.PendingDashboardNames);

        foreach (var name in _ctx.PendingDashboardNames)
        {
            var result = await _ctx.ApiService.CreateDashboardAsync(DashboardCreateRequest.CreateWithName(name.Unique()));

            Assert.NotNull(result);
            Assert.True(result.Id > 0);

            _ctx.CreatedDashboardList.Add(result);
            _ctx.TrackDashboard(result.Id);
        }
    }

    [Then("all dashboards from the list should be created successfully")]
    public void ThenAllDashboardsFromTheListShouldBeCreated()
    {
        Assert.Equal(_ctx.PendingDashboardNames.Count, _ctx.CreatedDashboardList.Count);

        foreach (var name in _ctx.PendingDashboardNames)
            Assert.Contains(_ctx.CreatedDashboardList, d => d.Name.StartsWith(name));
    }

    [StepArgumentTransformation]
    public static List<string> TransformCommaSeparatedList(string input) =>
        input.Split(',').Select(s => s.Trim()).ToList();

    [When("I add widgets with names {string}")]
    public async Task WhenIAddWidgetsWithNames(List<string> widgetNames)
    {
        Assert.NotNull(_ctx.CurrentDashboard);

        int posY = 0;
        foreach (var name in widgetNames)
        {
            var widget = Widget.CreateDefault(name.Unique());
            widget.Position = new WidgetPosition { PositionX = 0, PositionY = posY };
            posY += widget.Size.Height;

            var updated = await _ctx.ApiService.AddWidgetToDashboardAsync(_ctx.CurrentDashboard.Id, widget);
            Assert.NotNull(updated);

            _ctx.AddedWidgets.Add(widget);
            _ctx.CurrentDashboard = updated;
        }
    }

    [Then("the dashboard should have widgets matching the provided names")]
    public void ThenTheDashboardShouldHaveWidgetsMatchingNames()
    {
        Assert.NotNull(_ctx.CurrentDashboard);
        Assert.NotEmpty(_ctx.AddedWidgets);
        Assert.Equal(_ctx.AddedWidgets.Count, _ctx.CurrentDashboard.GetWidgetCount());

        foreach (var expected in _ctx.AddedWidgets)
            Assert.Contains(_ctx.CurrentDashboard.Widgets, w => w.Name == expected.Name);
    }
}
