using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Infrastructure.Service.Repositories.WalletRepository;

public class WalletRepositoryRepository : EntityRepository<WalletEntity, DbContext>, IWalletRepository
{
    public WalletRepositoryRepository(DbContext dbContext) : base(dbContext)
    {
    }
}