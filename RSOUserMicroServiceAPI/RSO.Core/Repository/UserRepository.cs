using Microsoft.EntityFrameworkCore;
using RSO.Core.UserModels;
using UserServiceRSO.Repository;

namespace RSO.Core.Repository;

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
    public async Task UpdateUsersNameAsync(User user) => await _context.User.Where(User => User.UserId == user.UserId).ExecuteUpdateAsync(users => users.SetProperty(u => u.UserName,user.UserName));
}
