using RSO.Core.UserModels;

namespace RSO.Core.BL.LogicModels;

/// <summary>
/// A custom DTO for returning the needed user data.
/// </summary>
public class UserDataDTO
{
    /// <summary>
    /// Get the neccessary data from the user.
    /// </summary>
    /// <param name="user"><see cref=User""/> instance.</param>
    public UserDataDTO(User user)
    {
        Id = user.UserId;
        UserName = user.UserName;
        UserEmail = user.UserEmail;
        UserLocation = $"{user.UserZipCode}  {user.UserCity}";
        RegisteredOn = $"Registriran od {user.RegisteredOn.Value:dd.MM.yyyy at H:mm}";
    }

    /// <summary>
    /// The date the user was registered on.
    /// </summary>
    public string RegisteredOn { get; }

    /// <summary>
    /// The user id.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The user name.
    /// </summary>
    public string UserName { get; }

    /// <summary>
    /// The email of the user.
    /// </summary>
    public string UserEmail { get; }

    /// <summary>
    /// The user location based on the zip code and the city.
    /// </summary>
    public string UserLocation { get; }
}
