using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Infrastructure.Service.Repositories.WalletRepository;

public class WalletTransactionRepository : EntityRepository<WalletTransactionEntity, DbContext>, IWalletTransaction
{
    public WalletTransactionRepository(DbContext dbContext) : base(dbContext)
    {
    }
}