using RSO.Core.UserModels;
using UserServiceRSO.Repository;

namespace RSO.Core.Repository;

public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Checks if user exists by username or email or password.
    /// </summary>
    /// <param name="emailOrUsername">The username or email of the user we are looking for.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>True, if the user exists. Otherwise false.</returns>
    public Task<bool> UserExistsByUserNameOrEmailOrPasswordAsync(string emailOrUsername, string password);

    /// <summary>
    /// Checks if the user name or the password is already taken.
    /// </summary>
    /// <param name="userName">The user name we'd like to query for.</param>
    /// <param name="email">The email we'd like to query for.</param>
    /// <returns>True, if  the user with the user name or the email already exists.</returns>
    public Task<bool> UsernameOrEmailAlreadyTakenAsync(string userName, string email);

    /// <summary>
    /// Gets the user based on his credentials.
    /// </summary>
    /// <param name="emailOrUsername">The username or email of the user we are looking for.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns> The corresponding user if one exists in the database.</returns>
    public Task<User> GetByUsernameOrEmailAndPasswordAsync(string emailOrUsername, string password);

    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="user">User.</param>
    /// <returns></returns>
    public Task DeleteUserAsync(User user);

    /// <summary>
    /// Updates the username of the user.
    /// </summary>
    /// <param name="user">User.</param>
    /// <returns></returns>
    public Task UpdateUsersNameAsync(User user);
}