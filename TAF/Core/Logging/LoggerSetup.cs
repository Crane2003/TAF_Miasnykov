using Serilog.Events;

namespace TAF.Core.Logging;

public static class LoggerSetup
{
    private static bool _isInitialized;

    public static void Initialize()
    {
        if (_isInitialized)
            return;

        var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        Directory.CreateDirectory(logDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "TAF")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: Path.Combine(logDirectory, "test-.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 7)
            .CreateLogger();

        _isInitialized = true;
        Log.Information("Logger initialized successfully");
    }

    public static void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }
}
