using Application.Common.Interfaces;
using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.Interceptors;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
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

            // ── Repositories ──────────────────────────────────────────────
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // ── Unit of Work ──────────────────────────────────────────────
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ── Hangfire ──────────────────────────────────────────────────
            services.AddHangfire(config => config
             .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
             .UseSimpleAssemblyNameTypeSerializer()
             .UseRecommendedSerializerSettings()
             .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
             {
                 CommandBatchMaxTimeout = null,   // <-- disables SqlCommandSet batching, avoids the reflection bug
                 SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                 QueuePollInterval = TimeSpan.Zero,
                 UseRecommendedIsolationLevel = true,
                 DisableGlobalLocks = true
             }));

            services.AddHangfireServer();

            services.AddScoped<OutboxProcessorJob>();

            return services;
        }
    }
}