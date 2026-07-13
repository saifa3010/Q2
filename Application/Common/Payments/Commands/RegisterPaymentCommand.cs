using Application.Common.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Payments.Commands
{
    public sealed record RegisterPaymentCommand(Guid InvoiceId, decimal Amount, string ReferenceNumber) : IRequest<PaymentDto>;

    public sealed class RegisterPaymentCommandHandler : IRequestHandler<RegisterPaymentCommand, PaymentDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterPaymentCommandHandler(IInvoiceRepository invoiceRepository, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentDto> Handle(RegisterPaymentCommand request,CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken)
                ?? throw new KeyNotFoundException($"Invoice {request.InvoiceId} not found.");

            invoice.RegisterPayment(request.Amount);

            var payment = Payment.Create(
                invoiceId: invoice.Id,
                amount: request.Amount,
                referenceNumber: $"PMT-{Guid.NewGuid():N}".ToUpperInvariant());

            await _paymentRepository.AddAsync(payment, cancellationToken);
            _invoiceRepository.Update(invoice);

            // Single SaveChanges — payment row + updated invoice + outbox messages all committed atomically.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return payment.ToDto();
        }
    }
}