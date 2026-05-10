namespace TAF.Core.Configuration;

public static class CredentialsExtensions
{
    /// <summary>
    /// Gets credentials by role name
    /// </summary>
    public static UserCredentials GetUserByRole(this CredentialsConfiguration credentials, string role)
    {
        return role.ToLower() switch
        {
            "default" => credentials.DefaultUser,
            "admin" => credentials.AdminUser,
            _ => throw new ArgumentException($"Unknown user role: {role}")
        };
    }
}
