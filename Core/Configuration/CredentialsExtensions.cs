namespace Core.Configuration;

public static class CredentialsExtensions
{
    public static UserCredentials GetUserByRole(this CredentialsConfiguration credentials, UserRole role)
    {
        return role switch
        {
            UserRole.Default => credentials.DefaultUser,
            UserRole.Admin => credentials.AdminUser,
            _ => throw new ArgumentException($"Unknown user role: {role}")
        };
    }
}
