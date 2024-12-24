using RSO.Core.AdModels;
using RSO.Core.UserModels;

namespace RSO.Core.BL.LogicModels.DTO;

/// <summary>
/// A custom DTO for returning the needed user data.
/// </summary>
/// <remarks>
/// Get the neccessary data from the user.
/// </remarks>
/// <param name="user"><see cref=User""/> instance.</param>
/// <param name="adsFromTheUser">Advertisements from the user.</param>
public class UserDataDTO(User user, List<Ad>? adsFromTheUser)
{
    /// <summary>
    /// The date the user was registered on.
    /// </summary>
    public string RegisteredOn { get; } = $"Registriran od {user.RegisteredOn.Value:dd.MM.yyyy at H:mm}";

    /// <summary>
    /// The user id.
    /// </summary>
    public int Id { get; } = user.UserId;

    /// <summary>
    /// The user name.
    /// </summary>
    public string UserName { get; } = user.UserName;

    /// <summary>
    /// The email of the user.
    /// </summary>
    public string UserEmail { get; } = user.UserEmail;

    /// <summary>
    /// The user location based on the zip code and the city.
    /// </summary>
    public string UserLocation { get; } = $"{user.UserZipCode}  {user.UserCity}";

    /// <summary>
    /// The ads that the user has.
    /// </summary>
    public List<Ad> Ads { get; } = adsFromTheUser ?? [];
}
