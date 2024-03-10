using RSO.Core.AdModels;
using RSO.Core.UserModels;

namespace RSO.Core.BL;

/// <summary>
/// Handles the userId logic.
/// </summary>
public interface IUserLogic
{
    /// <summary>
    /// Gets the city name from the userId zip code.
    /// </summary>
    /// <param name="userZipCode">The userId provided zip code.</param>
    /// <returns>The name of the city.</returns>
    public Task<string?> GetCityFromZipCodeAsync(string userZipCode);

    /// <summary>
    /// Deletes the userId.
    /// </summary>
    /// <param name="userId">The userId we would like to delete.</param>
    /// <returns>Throw error if it fails.</returns>
    public Task<bool> DeleteUserAsync(int userId);

    /// <summary>
    /// Gets the JWT.
    /// </summary>
    /// <param name="existingUser">The userId that hopefully exist in our database.</param>
    /// <returns>JWT token if result is true.</returns>
    public string? GetJwtToken(User existingUser);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>All of the users in the database.</returns>
    public Task<List<User>> GetAllUsersAsync();

    /// <summary>
    /// Gets userId by its Id.
    /// </summary>
    /// <param name="id">The userId of the userId.</param>
    /// <returns><see cref="User"/> instance, specified by the userId of the userId.</returns>
    public Task<User> GetUserByIdAsync(int id);

    /// <summary>
    /// Gets the userId based on the provided username or email or password.
    /// </summary>
    /// <param name="emailOrUsername">User's email or username.</param>
    /// <param name="password">User's password.</param>
    /// <returns>The userId specified by the email or username and password.</returns>
    public User GetUserByUsernameOrEmailAndPassword(string emailOrUsername, string password);

    /// <summary>
    /// Get ads from a certain userId.
    /// </summary>
    /// <param name="userId">The id of the userId for which the adds are going to be queried.</param>
    /// <returns>A list of userId's advertisements based on the userId's id.</returns>
    public Task<List<Ad>?> GetUsersAdsAsync(int userId);

    /// <summary>
    /// Checks if the email is unique.
    /// </summary>
    /// <param name="userEmail"><see cref="User.UserEmail"/></param>
    /// <returns>True, if only one instance of the email is present.</returns>
    public Task<bool> IsEmailUniqueAsync(string userEmail);

    /// <summary>
    /// Checks if the username is unique.
    /// </summary>
    /// <param name="userName"><see cref="User.UserName"/></param>
    /// <returns>True, if only one instance of the username is present.</returns>
    public Task<bool> IsUserNameUniqueAsync(string userName);

    /// <summary>
    /// Implements userId registration and insert the userId.
    /// </summary>
    /// <param name="newUser"></param>
    /// <returns>The same <see cref="User"/> object with and userId.</returns>
    public Task<User> RegisterUserAsync(User newUser);

    /// <summary>
    /// Updates the userId.
    /// </summary>
    /// <param name="user"><see cref="User"/> instance.</param>
    /// <returns>True, if the username was successfully updated.</returns>
    public Task<bool> UpdateUserAsync(User user);

    /// <summary>
    /// Updates the userId data.
    /// </summary>
    /// <param name="userData">The new userId data.</param>
    /// <returns>True, if the update was successfull.</returns>
    public Task<bool> UpdateUserDataAsync(User userData);

    /// <summary>
    /// Checks if the userId exists by username or email or password.
    /// </summary>
    /// <param name="userName">Username.</param>
    /// <param name="email">The email of the userId.</param>
    /// <returns>True, if the username is already taken.</returns>
    public Task<bool> UsernameOrEmailAlreadyTakenAsync(string userName, string email);

    /// <summary>
    /// Gets all the add of a certain userId.
    /// </summary>
    /// <param name="userId">User's ID.</param>
    /// <returns>All of the users ads.</returns>
    public List<Ad> GetUsersAdsByRPC(int userId);
}
