namespace TAF.Core.Configuration;

public class UserCredentials
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CredentialsConfiguration
{
    public UserCredentials DefaultUser { get; set; } = new();
    public UserCredentials AdminUser { get; set; } = new();
}
