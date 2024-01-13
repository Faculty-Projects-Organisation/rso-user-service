using RSO.Core.UserModels;

namespace RSO.Core.BL.LogicModels;
public class UserDataWithoutAdsDTO : UserDataDTO
{
    /// <summary>
    /// The ad information.
    /// </summary>
    public string AdInformation { get; set; }

    public UserDataWithoutAdsDTO(User user) : base(user)
    {
        AdInformation = "No ads";
    }
}