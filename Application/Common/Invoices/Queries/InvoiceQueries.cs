using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Invoices.Queries
{
    public sealed record GetInvoiceByIdQuery(Guid InvoiceId) : IRequest<InvoiceDto>;

    public sealed class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
            => _invoiceRepository = invoiceRepository;

        public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken)
                ?? throw new KeyNotFoundException($"Invoice {request.InvoiceId} not found.");

            return invoice.ToDto();
        }
    }

    public sealed record GetInvoicesQuery(long? CustomerId = null, InvoiceStatusId? StatusId = null) : IRequest<IReadOnlyList<InvoiceDto>>;

    public sealed class GetInvoicesQueryHandler: IRequestHandler<GetInvoicesQuery, IReadOnlyList<InvoiceDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoicesQueryHandler(IInvoiceRepository invoiceRepository)
            => _invoiceRepository = invoiceRepository;

        public async Task<IReadOnlyList<InvoiceDto>> Handle(GetInvoicesQuery request,CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);

            return invoices
                .Where(i => request.CustomerId == null || i.CustomerId == request.CustomerId)
                .Where(i => request.StatusId == null || i.StatusId == request.StatusId)
                .Select(i => i.ToDto())
                .ToList();
        }
    }
}