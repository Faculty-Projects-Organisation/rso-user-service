using System.Linq.Expressions;

namespace UserServiceRSO.Repository;

/// <summary>
/// Handles the basic CRUD operations.
/// </summary>
/// <typeparam name="T">Type of database generated object by the EF Core Power tools.</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Gets the type by its ID property in the database.
    /// </summary>
    /// <param name="id">The ID of the type.</param>
    /// <returns>The database object type.</returns>
    public Task<T> GetByIdAsync(int id);

    /// <summary>
    /// Gets all the enumerators for a database type.
    /// </summary>
    /// <returns>All enumerators for a database type.</returns>
    public Task<List<T>> GetAllAsync();

    /// <summary>
    /// Gets many enumerators for a database type based on a condition.
    /// </summary>
    /// <param name="filter">The expression filter predicate.</param>
    /// <param name="orderBy">The order by predicate.</param>
    /// <param name="top">The number of enteties to take.</param>
    /// <param name="skip">The number of enteties to skip.</param>
    /// <param name="includeProperties">Additional properties from other entities that user the primary-foreign key relation.</param>
    /// <returns>An IEnumerable of type <see cref="T"/> based on the provided filters.</returns>
    public Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>>? filter = null,
                              Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                              int? top = null,
                              int? skip = null,
                              params string[] includeProperties);

    /// <summary>
    /// Inserts a new entity of a database type.
    /// </summary>
    /// <param name="entity">The entity to be inserted to the database.</param>
    /// <returns>The insterted entity with the ID.</returns>
    public Task<T> InsertAsync(T entity);

    /// <summary>
    /// Inserts a List of new entities of a database type.
    /// </summary>
    /// <param name="entities">The List of new entities to be inserted to the database.</param>
    public Task InsertAsync(List<T> entities);

    /// <summary>
    /// Updates a new entity of a database type.
    /// </summary>
    /// <param name="entity">The entity to be updated to the database.</param>
    public Task UpdateAsync(T entity);
}
