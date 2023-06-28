namespace Core.Shared;

public static class JwtSettingsConstants
{
    private static string JWT_SETTINGS_PREFIX = "JwtSettings";

    public static string JWT_SECRET = $"{JWT_SETTINGS_PREFIX}:Secret";
    public static string JWT_ISSUER = $"{JWT_SETTINGS_PREFIX}:Issuer";
    public static string JWT_AUDIENCE = $"{JWT_SETTINGS_PREFIX}:Audience";
}
