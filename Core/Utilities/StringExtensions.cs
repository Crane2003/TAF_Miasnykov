namespace Core.Utilities;

public static class StringExtensions
{
    public static string Unique(this string value) => $"{value} {UniqueSuffix()}";

    public static string UniqueSuffix() => Guid.NewGuid().ToString("N");
}
