using Microsoft.EntityFrameworkCore;
using RSO.Core.UserModels;
using UserServiceRSO.Repository;

namespace RSO.Core.Repository;

/// <summary>
/// Implementation of <see cref="IUserRepository"/>.
/// </summary>
public class UserRepository(UserServicesRSOContext context) : GenericRepository<User>(context), IUserRepository
{
    ///<inheritdoc/>
    public bool TryGetUserByUserNameOrEmailOrPassword(string emailOrUsername, string password, out User? user)
    {
        user = _context.User.FirstOrDefault(u => (u.UserName.Equals(emailOrUsername) || u.UserEmail.Equals(emailOrUsername)) && u.UserPassword.Equals(password));
        return user is not null;
    }
    ///<inheritdoc/>
    public async Task<bool> UsernameOrEmailAlreadyTakenAsync(string userName, string email) => await _context.User.AnyAsync(u => u.UserName.Equals(userName) || u.UserEmail.Equals(email));

    ///<inheritdoc/>
    public async Task DeleteUserByIdAsync(int userId) => await _context.User.Where(User => User.UserId == userId).ExecuteDeleteAsync();

    ///<inheritdoc/>
    public async Task UpdateUsersNameAsync(User user) => await _context.User.Where(User => User.UserId == user.UserId).ExecuteUpdateAsync(users => users.SetProperty(u => u.UserName, user.UserName));

    ///<inheritdoc/>
    public async Task UpdateUserDataAsync(User userData) => await _context.User.Where(User => User.UserId == userData.UserId).ExecuteUpdateAsync(users => users.SetProperty(u => u.UserEmail, userData.UserEmail)
                                                                                                                                                                    .SetProperty(u => u.UserAddress, userData.UserAddress));
    ///<inheritdoc/>
    public async Task<int> GetEmailOccurrenceAsync(string userEmail) => await _context.User.Where(u => u.UserEmail.Equals(userEmail)).CountAsync();

    ///<inheritdoc/>
    public async Task<int> GetUserNameOcurrenceAsync(string userName) => await _context.User.Where(u => u.UserName.Equals(userName)).CountAsync();
}
