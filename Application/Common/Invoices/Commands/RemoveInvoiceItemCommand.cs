using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Invoices.Commands
{
    public sealed record RemoveInvoiceItemCommand(Guid InvoiceId, string ItemName) : IRequest<InvoiceDto>;

    public sealed class RemoveInvoiceItemCommandHandler : IRequestHandler<RemoveInvoiceItemCommand, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveInvoiceItemCommandHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceDto> Handle(RemoveInvoiceItemCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken)
                ?? throw new KeyNotFoundException($"Invoice {request.InvoiceId} not found.");

            invoice.RemoveItem(request.ItemName);

            _invoiceRepository.Update(invoice);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return invoice.ToDto();
        }
    }
}