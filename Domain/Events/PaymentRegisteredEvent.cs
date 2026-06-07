using Domain.Enums;

namespace Domain.Events
{
    public sealed class PaymentRegisteredEvent : BaseDomainEvent
    {
        public Guid InvoiceId { get; }
        public decimal AmountPaid { get; }
        public decimal TotalPaidSoFar { get; }
        public InvoiceStatusId NewStatus { get; }

        public PaymentRegisteredEvent(Guid invoiceId, decimal amountPaid, decimal totalPaidSoFar, InvoiceStatusId newStatus)
        {
            InvoiceId = invoiceId;
            AmountPaid = amountPaid;
            TotalPaidSoFar = totalPaidSoFar;
            NewStatus = newStatus;
        }
    }
}
