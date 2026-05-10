using TAF.Business.Models;

namespace TAF.Tests.TestData;

public static class TestWidgets
{
    public static Widget ChartWidget => Widget.CreateChartWidget("Default Chart", 0);

    public static Widget TableWidget => Widget.CreateTableWidget("Default Table", 0);

    public static Widget CustomWidget(string name, string type, int position) => new()
    {
        Name = name,
        Type = type,
        Position = position,
        Width = 6,
        Height = 4
    };

    public static List<Widget> GetMultipleChartWidgets(int count)
    {
        var widgets = new List<Widget>();
        for (int i = 0; i < count; i++)
        {
            widgets.Add(Widget.CreateChartWidget($"Chart Widget {i + 1}", i));
        }
        return widgets;
    }

    public static List<Widget> GetMixedWidgets(int count)
    {
        var widgets = new List<Widget>();
        for (int i = 0; i < count; i++)
        {
            var widget = i % 2 == 0
                ? Widget.CreateChartWidget($"Chart {i + 1}", i)
                : Widget.CreateTableWidget($"Table {i + 1}", i);
            widgets.Add(widget);
        }
        return widgets;
    }
}
