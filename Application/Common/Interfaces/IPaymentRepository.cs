using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    }
}