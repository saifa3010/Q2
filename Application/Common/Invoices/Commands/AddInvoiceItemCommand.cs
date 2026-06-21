using Application.Common.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Invoices.Commands
{
    public sealed record AddInvoiceItemCommand(
        Guid InvoiceId,
        string Name,
        decimal Price,
        int Quantity) : IRequest<InvoiceDto>;

    public sealed class AddInvoiceItemCommandHandler
        : IRequestHandler<AddInvoiceItemCommand, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddInvoiceItemCommandHandler(
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceDto> Handle(
            AddInvoiceItemCommand request,
            CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken)
                ?? throw new KeyNotFoundException($"Invoice {request.InvoiceId} not found.");

            invoice.AddItem(request.Name, request.Price, request.Quantity);

            _invoiceRepository.Update(invoice);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return invoice.ToDto();
        }
    }
}