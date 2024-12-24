using RSO.Core.Repository;
using RSO.Core.UserModels;

namespace UserServiceRSO.Repository;

/// <summary>
/// Implements the <see cref="IUnitOfWork"/> interface.
/// </summary>
/// <remarks>
/// Constructor for the <see cref="UnitOfWork"/> class.
/// </remarks>
/// <param name="adServicesRSOContext ">The <see cref="userServicesRSOContext "/> context for the database access.</param>
/// <param name="userRepository">IUserRepository instance.</param>
public class UnitOfWork(UserServicesRSOContext adServicesRSOContext, IUserRepository userRepository) : IUnitOfWork
{
    private readonly UserServicesRSOContext _userServicesContext = adServicesRSOContext;
    private bool disposed;

    ///<inheritdoc/>
    public IUserRepository UserRepository { get; } = userRepository;

    ///<inheritdoc/>
    public async Task<int> SaveChangesAsync() => await _userServicesContext.SaveChangesAsync();

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
                _userServicesContext.Dispose();
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
