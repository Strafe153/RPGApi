namespace Core.Shared;

public static class JwtSettingsConstants
{
    private const string JwtSettingsPrefix = "JwtSettings";

    public const string JwtSecret = $"{JwtSettingsPrefix}:Secret";
    public const string JwtIssuer = $"{JwtSettingsPrefix}:Issuer";
    public const string JwtAudience = $"{JwtSettingsPrefix}:Audience";
}
