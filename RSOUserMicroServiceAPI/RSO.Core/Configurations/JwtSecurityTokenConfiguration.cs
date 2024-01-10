namespace RSO.Core.Configurations;

/// <summary>
/// Handles the JWT security token configuration variables.
/// </summary>
public class JwtSecurityTokenConfiguration
{
    /// <summary>
    /// JWT issuer.
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// JWT audience.
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Key for signing the JWT.
    /// </summary>
    public string SecretKey { get; set; }
}
