using System.Globalization;
using System.Text;
using ExchangeRate.Infrastructure.Configuration.Database;
using ExchangeRate.Infrastructure.Interface;
using ExchangeRate.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRate.Infrastructure.Service.Repositories;

/// <summary>
/// Added general entity repository to future proof this.
/// </summary>
public class ExchangeRateRepository : EntityRepository<ExchangeRateEntity, DatabaseContext>, IExchangeRateRepository
{
    private readonly DatabaseContext _context;
    private readonly DbSet<ExchangeRateEntity> _dbSet;

    public ExchangeRateRepository(DatabaseContext context) : base(context)
    {
        _context = context;
        _dbSet = _context.Set<ExchangeRateEntity>();
    }

    public async Task UpsertExchangeRatesAsync(List<ExchangeRateEntity> rates)
    {
        var sql = GenerateMergeSql(rates);
        await _context.Database.ExecuteSqlRawAsync(sql);
    }
    
    public async Task<decimal?> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            return 1m;

        var latestDate = await _dbSet
            .MaxAsync(x => (DateTime?)x.Date);

        if (!latestDate.HasValue)
            return null;

        decimal? fromRate = fromCurrency.ToUpper() == "EUR" 
            ? 1m 
            : await _dbSet
                .Where(x => x.Date == latestDate && x.Currency == fromCurrency)
                .Select(x => (decimal?)x.Rate)
                .FirstOrDefaultAsync();

        decimal? toRate = toCurrency.ToUpper() == "EUR"
            ? 1m
            : await _dbSet
                .Where(x => x.Date == latestDate && x.Currency == toCurrency)
                .Select(x => (decimal?)x.Rate)
                .FirstOrDefaultAsync();

        if (fromRate == null || fromRate == 0 || toRate == null || toRate == 0)
            return null;

        // Calculate the conversion: toRate / fromRate
        return toRate.Value / fromRate.Value;
    }

    private string GenerateMergeSql(List<ExchangeRateEntity> rates)
    {
        var sb = new StringBuilder();

        sb.AppendLine("MERGE INTO public.\"ExchangeRates\" AS Target");
        sb.AppendLine("USING (VALUES");

        var valueLines = rates.Select(r =>
            $"('{EscapeSqlLiteral(r.Currency)}', '{r.Date:yyyy-MM-dd}'::timestamptz, {r.Rate.ToString(CultureInfo.InvariantCulture)})"
        );

        sb.AppendLine(string.Join(",\n", valueLines));

        sb.AppendLine(") AS Source(\"Currency\", \"Date\", \"Rate\")");
        sb.AppendLine("ON Target.\"Currency\" = Source.\"Currency\" AND Target.\"Date\" = Source.\"Date\"");
        sb.AppendLine("WHEN MATCHED THEN");
        sb.AppendLine("    UPDATE SET \"Rate\" = Source.\"Rate\"");
        sb.AppendLine("WHEN NOT MATCHED THEN");
        sb.AppendLine("    INSERT (\"Currency\", \"Date\", \"Rate\", \"CreatedAt\")");
        sb.AppendLine("    VALUES (Source.\"Currency\", Source.\"Date\", Source.\"Rate\", NOW());");

        return sb.ToString();
    }

    private string EscapeSqlLiteral(string input)
    {
        return input.Replace("'", "''");
    }
}