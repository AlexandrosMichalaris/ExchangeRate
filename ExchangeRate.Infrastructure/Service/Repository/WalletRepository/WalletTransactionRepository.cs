using ExchangeRate.Infrastructure.Configuration.Database;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Infrastructure.Service.Repositories.WalletRepository;

public class WalletTransactionRepository : EntityRepository<WalletTransactionEntity, DatabaseContext>, IWalletTransactionRepository
{
    public WalletTransactionRepository(DatabaseContext dbContext) : base(dbContext)
    {
    }
}