using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
        void Update(Invoice invoice);
    }
}
