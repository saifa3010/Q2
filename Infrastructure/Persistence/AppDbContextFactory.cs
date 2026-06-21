using Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Used exclusively by EF Core tooling (add-migration, update-database, script-migration).
    /// PMC commands run outside the app's DI container and ASP.NET host, so they cannot resolve
    /// AppDbContext's constructor dependencies (like OutboxInterceptor) the normal way.
    /// This factory builds the same connection configuration the running app uses, read directly
    /// from appsettings.json, so migrations always target the correct database with the correct
    /// connection options (including TrustServerCertificate).
    /// </summary>
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Locate appsettings.json relative to the Api project (design-time working
            // directory is normally the startup project's output/bin folder or its root).
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found for design-time migration.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Design-time context never actually saves changes, so a real OutboxInterceptor
            // instance isn't required for schema generation — it only matters at runtime.
            return new AppDbContext(optionsBuilder.Options, new OutboxInterceptor());
        }
    }
}