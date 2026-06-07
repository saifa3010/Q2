namespace Domain.Events
{
    public sealed class InvoiceOverdueEvent : BaseDomainEvent
    {
        public Guid InvoiceId { get; }
        public long CustomerId { get; }
        public DateTime DueDate { get; }

        public InvoiceOverdueEvent(Guid invoiceId, long customerId, DateTime dueDate)
        {
            InvoiceId = invoiceId;
            CustomerId = customerId;
            DueDate = dueDate;
        }
    }
}
