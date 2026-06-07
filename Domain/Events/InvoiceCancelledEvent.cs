namespace Domain.Events
{
    public sealed class InvoiceCancelledEvent : BaseDomainEvent
    {
        public Guid InvoiceId { get; }
        public long CustomerId { get; }

        public InvoiceCancelledEvent(Guid invoiceId, long customerId)
        {
            InvoiceId = invoiceId;
            CustomerId = customerId;
        }
    }
}
