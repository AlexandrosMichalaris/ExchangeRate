namespace ExchangeRate.Infrastructure.Interface;

public interface IEntityRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(long id);
    
    Task<IEnumerable<TEntity>> GetAllAsync();
    
    Task<TEntity> AddAsync(TEntity entity);
    
    Task UpdateAsync(TEntity entity);
}