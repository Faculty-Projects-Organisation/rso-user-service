namespace RSO.Core.Configurations;

/// <summary>
/// Handles the JWT security token configuration variables.
/// </summary>
public class JwtSecurityTokenConfiguration
{
    /// <summary>
    /// JWT issuer.
    /// </summary>
    public string Issuer { get; init; }

    /// <summary>
    /// JWT audience.
    /// </summary>
    public string Audience { get; init; }

    /// <summary>
    /// Key for signing the JWT.
    /// </summary>
    public string SecretKey { get; init; }
}
