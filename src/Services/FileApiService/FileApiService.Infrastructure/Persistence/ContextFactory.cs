using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using FileApiService.Infrastructure.Persistence;

namespace IdentityService.Infrastructure.Persistence;

public class ContextFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {

        var optionsBuilder = new DbContextOptionsBuilder<Context>();
        
        var connectionString = "Host=localhost;Port=5432;Database=fileApiServiceDb;Username=postgres;Password=admin";
        
        optionsBuilder.UseNpgsql(connectionString);
        return new Context(optionsBuilder.Options);
    }
}