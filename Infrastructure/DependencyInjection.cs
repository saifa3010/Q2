using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.Interceptors;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")!;

            // ── Interceptors ──────────────────────────────────────────────
            services.AddSingleton<OutboxInterceptor>();

            // ── DbContext ─────────────────────────────────────────────────
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

                options.AddInterceptors(sp.GetRequiredService<OutboxInterceptor>());
            });

            // ── Hangfire ──────────────────────────────────────────────────
            // Store Hangfire jobs in the same SQL Server database.
            services.AddHangfire(hangfire => hangfire
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            // Hangfire server processes enqueued/recurring jobs in-process.
            services.AddHangfireServer();

            // OutboxProcessorJob is resolved from DI by Hangfire on each trigger.
            services.AddScoped<OutboxProcessorJob>();

            return services;
        }
    }
}
