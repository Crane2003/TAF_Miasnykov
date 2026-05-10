namespace TAF.Core.Utilities;

public static class TestHelper
{
    public static string GenerateRandomEmail()
    {
        return $"testuser_{Guid.NewGuid():N}@example.com";
    }

    public static string GenerateRandomUsername()
    {
        return $"user_{Guid.NewGuid():N}";
    }

    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static DateTime GetCurrentDateTime()
    {
        return DateTime.UtcNow;
    }

    public static string GetTimestamp()
    {
        return DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
    }
}
