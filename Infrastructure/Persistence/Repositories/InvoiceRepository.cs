using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
            => _context = context;

        public async Task<Invoice?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
            => await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<Invoice>> GetAllAsync(
            CancellationToken cancellationToken = default)
            => await _context.Invoices
                .Include(i => i.Items)
                .Where(i => !i.IsDeleted)
                .ToListAsync(cancellationToken);

        public async Task AddAsync(
            Invoice invoice,
            CancellationToken cancellationToken = default)
            => await _context.Invoices.AddAsync(invoice, cancellationToken);

        public void Update(Invoice invoice)
            => _context.Invoices.Update(invoice);
    }
}
