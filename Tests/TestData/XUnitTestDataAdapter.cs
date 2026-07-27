using Business.Models;
using Xunit;

namespace Tests.TestData;

public static class XUnitTestDataAdapter
{
    public static TheoryData<DashboardCreateRequest> DashboardCreateCases
    {
        get
        {
            var data = new TheoryData<DashboardCreateRequest>();
            foreach (var r in TestDataProvider.DashboardCreateCases)
                data.Add(r);
            return data;
        }
    }

    public static TheoryData<DashboardCreateRequest, DashboardUpdateRequest> DashboardUpdateCases
    {
        get
        {
            var data = new TheoryData<DashboardCreateRequest, DashboardUpdateRequest>();
            foreach (var (create, update) in TestDataProvider.DashboardUpdateCases)
                data.Add(create, update);
            return data;
        }
    }

    public static TheoryData<Widget> WidgetCases
    {
        get
        {
            var data = new TheoryData<Widget>();
            foreach (var w in TestDataProvider.WidgetCases)
                data.Add(w);
            return data;
        }
    }
}
