namespace Domain.Events
{
    public sealed class InvoiceItemAddedEvent : BaseDomainEvent
    {
        public Guid InvoiceId { get; }
        public string ItemName { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public decimal NewTotal { get; }

        public InvoiceItemAddedEvent(Guid invoiceId, string itemName, decimal price, int quantity, decimal newTotal)
        {
            InvoiceId = invoiceId;
            ItemName = itemName;
            Price = price;
            Quantity = quantity;
            NewTotal = newTotal;
        }
    }
}
