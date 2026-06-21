using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
            => _context = context;

        public async Task AddAsync(
            Payment payment,
            CancellationToken cancellationToken = default)
            => await _context.Payments.AddAsync(payment, cancellationToken);
    }
}
