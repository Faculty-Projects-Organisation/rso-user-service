using Microsoft.EntityFrameworkCore;
using RSO.Core.UserModels;
using System.Linq.Expressions;

namespace UserServiceRSO.Repository;

/// <summary>
/// Implements the <see cref="IGenericRepository{T}"/> interface.
/// </summary>
/// <typeparam name="T">Type of database generated object by the EF Core Power tools.</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly UserServicesRSOContext _context;

    /// <summary>
    /// Constructor for the <see cref="GenericRepository{T}"/> class.
    /// </summary>
    /// <param name="context">The <see cref="UserServicesRSOContext"/> context from the database access.</param>
    protected GenericRepository(UserServicesRSOContext context) { _context = context; }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

    /// <inheritdoc/>
    public async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

    /// <inheritdoc/>
    public async Task<T> InsertAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? top = null, int? skip = null, params string[] includeProperties)
    {
        IQueryable<T> query = _context.Set<T>();

        if (filter is not null)
            query = query.Where(filter);

        if (includeProperties.Length > 0)
            query = includeProperties.Aggregate(query, (theQuery, theInclude) => theQuery.Include(theInclude));

        if (orderBy is not null)
            query = orderBy(query);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (top.HasValue)
            query = query.Take(top.Value);

        return await query.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task InsertAsync(List<T> entities) => await _context.Set<T>().AddRangeAsync(entities);
}
