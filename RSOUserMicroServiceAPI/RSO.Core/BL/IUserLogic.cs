﻿using RSO.Core.AdModels;
using RSO.Core.UserModels;

namespace RSO.Core.BL;

/// <summary>
/// Handles the user logic.
/// </summary>
public interface IUserLogic
{
    /// <summary>
    /// Deletes the user.
    /// </summary>
    /// <param name="user">The user we would like to delete.</param>
    /// <returns>Throw error if it fails.</returns>
    public Task DeleteUserAsync(User user);

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
    /// Get ads from a certain user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<List<Ad>> GetUsersAddsAsync(int userId);

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
    /// Implements user registration and insert the user.
    /// </summary>
    /// <param name="newUser"></param>
    /// <returns>The same <see cref="User"/> object with and id.</returns>
    public Task<User> RegisterUserAsync(User newUser);

    /// <summary>
    /// Updates the user.
    /// </summary>
    /// <param name="user"><see cref="User"/> instance.</param>
    /// <returns>True, if the username was successfully updated.</returns>
    public Task<bool> UpdateUserAsync(User user);

    /// <summary>
    /// Updates the user data.
    /// </summary>
    /// <param name="userData">The new user data.</param>
    /// <returns>True, if the update was successfull.</returns>
    public Task<bool> UpdateUserDataAsync(User userData);

    /// <summary>
    /// Checks if the user exists by username or email or password.
    /// </summary>
    /// <param name="userName">Username.</param>
    /// <param name="email">The email of the user.</param>
    /// <returns>True, if the username is already taken.</returns>
    public Task<bool> UsernameOrEmailAlreadyTakenAsync(string userName, string email);
}
