using ExchangeRate.Infrastructure.Configuration.Database;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Infrastructure.Service.Repositories.WalletRepository;

public class WalletRepository : EntityRepository<WalletEntity, DatabaseContext>, IWalletRepository
{
    public WalletRepository(DatabaseContext dbContext) : base(dbContext)
    {
    }
}