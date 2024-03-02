namespace RSO.Core.BL.LogicModels.DTO;

/// <summary>
/// DTO for the login credentials.
/// </summary>
public class LoginCredentialsDTO
{
    /// <summary>
    /// The login credential of the user - email or username.
    /// </summary>
    public string? EmailOrUsername { get; set; }

    /// <summary>
    /// User's password.
    /// </summary>
    public string? Password { get; set; }
}
