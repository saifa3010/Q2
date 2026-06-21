using Application.Common.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Thin wrapper over AppDbContext.SaveChangesAsync.
    /// Keeps Application layer handlers decoupled from EF Core.
    /// The OutboxInterceptor fires automatically inside this call,
    /// so domain events are persisted atomically with aggregate changes.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
            => _context = context;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);
    }
}
