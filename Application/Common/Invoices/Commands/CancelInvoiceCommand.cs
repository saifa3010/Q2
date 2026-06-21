using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Invoices.Commands
{
    public sealed record CancelInvoiceCommand(Guid InvoiceId) : IRequest<InvoiceDto>;

    public sealed class CancelInvoiceCommandHandler
        : IRequestHandler<CancelInvoiceCommand, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelInvoiceCommandHandler(
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceDto> Handle(
            CancelInvoiceCommand request,
            CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken)
                ?? throw new KeyNotFoundException($"Invoice {request.InvoiceId} not found.");

            invoice.Cancel();

            _invoiceRepository.Update(invoice);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return invoice.ToDto();
        }
    }
}