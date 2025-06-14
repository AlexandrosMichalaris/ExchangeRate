using ExchangeRate.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Infrastructure.Service.Repositories;

public class EntityRepository<TEntity, TContext> : IEntityRepository<TEntity> 
    where TEntity : class
    where TContext : DbContext
{
    private readonly TContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;
    
    public EntityRepository(TContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }
    
    public async Task<TEntity?> GetByIdAsync(long id) => 
        await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<long>(e, "Id") == id);

    public async Task<IEnumerable<TEntity>> GetAllAsync() => 
        await _dbSet.AsNoTracking().ToListAsync();

    /// <summary>
    /// Add entry
    /// </summary>
    /// <param name="entity">Entity must be DTO</param>
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var dbEntity = _dbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
        return dbEntity.Entity;
    }
    
    /// <summary>
    /// Update Async. Load the tracked entity from the DB and apply changes to it
    /// </summary>
    /// <param name="entity"></param>
    public async Task UpdateAsync(TEntity entity)
    {
        //  Get keys of entity
        var keyValues = _dbContext.Entry(entity).Metadata.FindPrimaryKey()
            ?.Properties.Select(p => p.PropertyInfo.GetValue(entity)).ToArray();

        var existingEntity = await _dbSet.FindAsync(keyValues);

        if (existingEntity == null)
        {
            throw new InvalidOperationException($"{nameof(EntityRepository<TEntity, TContext>)} - UpdateAsync - Entity not found in the database.");
        }

        // Apply changes from the passed-in entity to the tracked one
        _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);

        await _dbContext.SaveChangesAsync();
    }
}