using Microsoft.EntityFrameworkCore;
using RSO.Core.UserModels;
using UserServiceRSO.Repository;

namespace RSO.Core.Repository;

/// <summary>
/// Implementation of <see cref="IUserRepository"/>.    
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(UserServicesRSOContext context) : base(context) { }

    ///<inheritdoc/>
    public async Task<bool> UserExistsByUserNameOrEmailOrPasswordAsync(string emailOrUsername, string password) => await _context.User.AnyAsync(u => (u.UserName.Equals(emailOrUsername) || u.UserEmail.Equals(emailOrUsername)) && u.UserPassword.Equals(password));

    ///<inheritdoc/>
    public async Task<bool> UsernameOrEmailAlreadyTakenAsync(string userName, string email) => await _context.User.AnyAsync(u => u.UserName.Equals(userName) || u.UserEmail.Equals(email));

    ///<inheritdoc/>
    public async Task<User> GetByUsernameOrEmailAndPasswordAsync(string emailOrUsername, string password) => await _context.User.Where(u => (u.UserName.Equals(emailOrUsername) || u.UserEmail.Equals(emailOrUsername)) && u.UserPassword.Equals(password)).FirstOrDefaultAsync();

    ///<inheritdoc/>
    public async Task DeleteUserAsync(User user) => await _context.User.Where(User => User.UserId == user.UserId).ExecuteDeleteAsync();

    ///<inheritdoc/>
    public async Task UpdateUsersNameAsync(User user) => await _context.User.Where(User => User.UserId == user.UserId).ExecuteUpdateAsync(users => users.SetProperty(u => u.UserName, user.UserName));

    ///<inheritdoc/>
    public async Task UpdateUserDataAsync(User userData) => await _context.User.Where(User => User.UserId == userData.UserId).ExecuteUpdateAsync(users => users.SetProperty(u => u.UserEmail, userData.UserEmail)
                                                                                                                                                                    .SetProperty(u => u.UserAddress, userData.UserAddress)
                                                                                                                                                                    .SetProperty(u => u.UserName, userData.UserName)
                                                                                                                                                                    .SetProperty(u => u.UserPassword, userData.UserPassword)
                                                                                                                                                                    .SetProperty(u => u.UserZipCode, userData.UserZipCode)
                                                                                                                                                                    .SetProperty(u => u.UserCity, userData.UserCity)
                                                                                                           );
    ///<inheritdoc/>
    public async Task<int> GetEmailOccurrenceAsync(string userEmail) => await _context.User.Where(u => u.UserEmail.Equals(userEmail)).CountAsync();

    ///<inheritdoc/>
    public async Task<int> GetUserNameOcurrenceAsync(string userName) => await _context.User.Where(u => u.UserName.Equals(userName)).CountAsync();

}
