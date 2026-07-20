using Microsoft.Extensions.Configuration;

namespace Core.Configuration;

public class ConfigurationLoader
{
    private static readonly Lazy<ConfigurationLoader> _instance = new(() => new ConfigurationLoader());
    private readonly IConfiguration _configuration;

    private ConfigurationLoader()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static ConfigurationLoader Instance => _instance.Value;

    public TestConfiguration GetTestConfiguration()
    {
        var config = new TestConfiguration();
        _configuration.GetSection("TestConfiguration").Bind(config);
        return config;
    }

    public CredentialsConfiguration GetCredentials()
    {
        var credentials = new CredentialsConfiguration();
        _configuration.GetSection("Credentials").Bind(credentials);
        return credentials;
    }

    public ApiConfiguration GetApiConfiguration()
    {
        var apiConfig = new ApiConfiguration();
        _configuration.GetSection("ApiConfiguration").Bind(apiConfig);
        return apiConfig;
    }

    public T GetSection<T>(string sectionName) where T : new()
    {
        var section = new T();
        _configuration.GetSection(sectionName).Bind(section);
        return section;
    }
}
