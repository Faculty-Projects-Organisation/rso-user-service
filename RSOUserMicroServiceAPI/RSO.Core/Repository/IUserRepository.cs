using RSO.Core.UserModels;
using UserServiceRSO.Repository;

namespace RSO.Core.Repository;

/// <summary>
/// Handles the communication with the User database.
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Checks if user exists by username or email or password. IF true, it returns the user as an out parameter.
    /// </summary>
    /// <param name="emailOrUsername">The username or email of the user we are looking for.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>True, if the user exists. Otherwise false.</returns>
    public bool TryGetUserByUserNameOrEmailOrPassword(string emailOrUsername, string password, out User? user);

    /// <summary>
    /// Checks if the user name or the password is already taken.
    /// </summary>
    /// <param name="userName">The user name we'd like to query for.</param>
    /// <param name="email">The email we'd like to query for.</param>
    /// <returns>True, if  the user with the user name or the email already exists.</returns>
    public Task<bool> UsernameOrEmailAlreadyTakenAsync(string userName, string email);

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="userId"></param>
    public Task DeleteUserByIdAsync(int userId);

    /// <summary>
    /// Updates the username of the user.
    /// </summary>
    /// <param name="user">User.</param>
    public Task UpdateUsersNameAsync(User user);

    /// <summary>
    /// Updates the user data.
    /// </summary>
    /// <param name="userData">The new user data.</param>
    /// <returns></returns>
    public Task UpdateUserDataAsync(User userData);

    /// <summary>
    /// How many times the username occurs in the database.
    /// </summary>
    /// <param name="userEmail">The user's email.</param>
    /// <returns>The number of times the email is present in the database.</returns>
    public Task<int> GetEmailOccurrenceAsync(string userEmail);

    /// <summary>
    /// How many times the username occurs in the database.
    /// </summary>
    /// <param name="userName">The username.</param>
    /// <returns>The number of times the username is present in the database. </returns>
    public Task<int> GetUserNameOcurrenceAsync(string userName);
}