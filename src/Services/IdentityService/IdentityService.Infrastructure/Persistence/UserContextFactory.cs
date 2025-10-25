using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace IdentityService.Infrastructure.Persistence;

public class UserContextFactory : IDesignTimeDbContextFactory<UserContext>
{
    public UserContext CreateDbContext(string[] args)
    {

        var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
        
        var connectionString = "Host=localhost;Port=5432;Database=identityServiceDb;Username=postgres;Password=admin";
        
        optionsBuilder.UseNpgsql(connectionString);

        return new UserContext(optionsBuilder.Options);
    }
}