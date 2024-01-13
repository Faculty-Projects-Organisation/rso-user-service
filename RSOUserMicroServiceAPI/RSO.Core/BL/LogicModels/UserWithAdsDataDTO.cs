using RSO.Core.AdModels;
using RSO.Core.UserModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace RSO.Core.BL.LogicModels;

/// <summary>
/// A custom DTO for returning the needed user data.
/// </summary>
public class UserWithAdsDataDTO
{
    /// <summary>
    /// Get the neccessary data from the user.
    /// </summary>
    /// <param name="user"><see cref=User""/> instance.</param>
    public UserWithAdsDataDTO(User user, List<Ad> ads)
    {
        Id = user.UserId;
        UserName = user.UserName;
        UserEmail = user.UserEmail;
        UserLocation = $"{user.UserZipCode}  {user.UserCity}";
        RegisteredOn = $"Registriran od {user.RegisteredOn.Value:dd.MM.yyyy at H:mm}";
        Ads = ads;
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

    /// <summary>
    /// The ads of the user.
    /// </summary>
    public List<Ad> Ads { get; }
}
