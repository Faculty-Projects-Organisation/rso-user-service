using RSO.Core.AdModels;
using RSO.Core.Repository;

namespace UserServiceRSO.Repository;

/// <summary>
/// Implements the <see cref="IUnitOfWork"/> interface.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AdServicesRSOContext _adServicesRSOContext;
    private bool disposed;

    /// <summary>
    /// Constructor for the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="adServicesRSOContext ">The <see cref="userServicesRSOContext "/> context for the database access.</param>
    /// <param name="userRepository">IUserRepository instance.</param>
    public UnitOfWork(AdServicesRSOContext adServicesRSOContext, IAdRepository adrepository)
    {
        _adServicesRSOContext = adServicesRSOContext;
        AddRepository = adrepository;
    }

    ///<inheritdoc/>
    public IAdRepository AddRepository { get; }

    ///<inheritdoc/>
    public async Task<int> SaveChangesAsync() => await _adServicesRSOContext.SaveChangesAsync();

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
                _adServicesRSOContext.Dispose();
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
