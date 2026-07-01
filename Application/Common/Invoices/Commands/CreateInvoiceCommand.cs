using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Invoices.Commands
{
    public sealed record CreateInvoiceCommand(int CustomerId, DateTime DueDate) : IRequest<InvoiceDto>;

    public sealed class CreateInvoiceCommandHandler: IRequestHandler<CreateInvoiceCommand, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = Invoice.Create(request.CustomerId, request.DueDate);

            await _invoiceRepository.AddAsync(invoice, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return invoice.ToDto();
        }
    }
}