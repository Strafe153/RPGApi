namespace Core.Constants;

public static class JwtSettingsConstants
{
    private const string JwtSettingsPrefix = "Jwt";

    public const string JwtSecret = $"{JwtSettingsPrefix}:Secret";
    public const string JwtIssuer = $"{JwtSettingsPrefix}:Issuer";
    public const string JwtAudience = $"{JwtSettingsPrefix}:Audience";
}
