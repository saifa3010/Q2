namespace Domain.Events
{
    public sealed class InvoiceCreatedEvent : BaseDomainEvent
    {
        public Guid InvoiceId { get; }
        public long CustomerId { get; }
        public DateTime DueDate { get; }

        public InvoiceCreatedEvent(Guid invoiceId, long customerId, DateTime dueDate)
        {
            InvoiceId = invoiceId;
            CustomerId = customerId;
            DueDate = dueDate;
        }
    }
}
