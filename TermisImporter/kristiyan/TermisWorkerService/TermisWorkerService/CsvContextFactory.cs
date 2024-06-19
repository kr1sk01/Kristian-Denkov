using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermisWorkerService;
public class CsvContextFactory : IDbContextFactory<CsvContext>
{
    private readonly DbContextOptions _options;

    public CsvContextFactory(DbContextOptions options)
    {
        _options = options;
    }

    public CsvContextFactory CreateDbContext()
    {
        return new CsvContextFactory(_options);
    }

    CsvContext IDbContextFactory<CsvContext>.CreateDbContext()
    {
        throw new NotImplementedException();
    }
}
