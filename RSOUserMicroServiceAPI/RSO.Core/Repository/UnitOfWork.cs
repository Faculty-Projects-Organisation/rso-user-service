using RSO.Core.UserModels;
using RSO.Core.Repository;

namespace UserServiceRSO.Repository;

/// <summary>
/// Implements the <see cref="IUnitOfWork"/> interface.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly UserServicesRSOContext _userServicesRSOContext;
    private bool disposed;

    /// <summary>
    /// Constructor for the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="userServicesRSOContext ">The <see cref="userServicesRSOContext "/> context for the database access.</param>
    /// <param name="userRepository">IUserRepository instance.</param>
    public UnitOfWork(UserServicesRSOContext userServicesRSOContext, IUserRepository userRepository)
    {
        _userServicesRSOContext = userServicesRSOContext;
        UserRepository = userRepository;
    }

    ///<inheritdoc/>
    public IUserRepository UserRepository { get; }

    ///<inheritdoc/>
    public async Task<int> SaveChangesAsync() => await _userServicesRSOContext.SaveChangesAsync();

    /// <summary>
    /// Implements the <see cref="IDisposable"/> interface. Called when we'd like to the dispose the <see cref="UnitOfWork"/> object.
    /// </summary>
    /// <param name="disposing">The </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _userServicesRSOContext.Dispose();
            }
        }
        disposed = true;
    }

    /// <summary>
    /// Disposes the <see cref="UnitOfWork"/> object and acts as a wrapper for <see cref="Dispose(bool)"/> method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
