using RSO.Core.UserModels;

namespace RSO.Core.BL;

/// <summary>
/// Handles the user logic.
/// </summary>
public interface IUserLogic
{
    /// <summary>
    /// Gets the city name from the user zip code.
    /// </summary>
    /// <param name="userZipCode">The user provided zip code.</param>
    /// <returns>The name of the city.</returns>
    public Task<string> GetCityFromZipCodeAsync(string userZipCode);

    /// <summary>
    /// Gets the JWT.
    /// </summary>
    /// <param name="existingUser">The user that hopefully exist in our database.</param>
    /// <returns>JWT token if result is true.</returns>
    public string GetJwtToken(User existingUser);

    /// <summary>
    /// Gets user by its Id.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns><see cref="User"/> instance, specified by the id of the user.</returns>
    public Task<User> GetUserByIdAsync(int id);

    /// <summary>
    /// Gets the user based on the provided username or email or password.
    /// </summary>
    /// <param name="emailOrUsername"></param>
    /// <param name="password"></param>
    /// <returns>The user specified by the email or username and password.</returns>
    public Task<User> GetUserByUsernameOrEmailAndPasswordAsync(string emailOrUsername, string password);

    /// <summary>
    /// Implements user registration and insert the user.
    /// </summary>
    /// <param name="newUser"></param>
    /// <returns>The same <see cref="User"/> object with and id.</returns>
    public Task<User> RegisterUserAsync(User newUser);
}
