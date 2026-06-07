namespace Domain.Events
{
    public sealed class InvoiceItemRemovedEvent : BaseDomainEvent
    {
        public Guid InvoiceId { get; }
        public string ItemName { get; }
        public decimal NewTotal { get; }

        public InvoiceItemRemovedEvent(Guid invoiceId, string itemName, decimal newTotal)
        {
            InvoiceId = invoiceId;
            ItemName = itemName;
            NewTotal = newTotal;
        }
    }
}
