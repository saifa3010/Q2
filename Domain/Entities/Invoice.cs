using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Invoice : BaseEntity
    {
        private readonly List<InvoiceItem> _items = new();

        private Invoice()
        {
            // EF Core
        }

        public long CustomerId { get; private set; }

        public DateTime DueDate { get; private set; }

        public Money TotalAmount { get; private set; } = new Money(0);

        public Money PaidAmount { get; private set; } = new Money(0);

        public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

        public InvoiceStatusId StatusId { get; private set; }

        public static Invoice Create(
            int customerId,
            DateTime dueDate)
        {
            if (customerId <= 0)
                throw new ArgumentException("Invalid customer.");

            if (dueDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Due date cannot be in the past.");

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                DueDate = dueDate,
                StatusId = InvoiceStatusId.Pending,
                TotalAmount = new Money(0),
                PaidAmount = new Money(0),
                CreatedAt = DateTime.UtcNow
            };

            invoice.AddDomainEvent(new InvoiceCreatedEvent(
                invoice.Id,
                invoice.CustomerId,
                invoice.DueDate));

            return invoice;
        }

        public void AddItem(string name, decimal price, int quantity)
        {
            if (StatusId == InvoiceStatusId.Paid)
                throw new InvalidOperationException("Cannot modify a paid invoice.");

            var item = new InvoiceItem(
                name,
                new Money(price),
                quantity);

            _items.Add(item);

            RecalculateTotal();

            AddDomainEvent(new InvoiceItemAddedEvent(
                Id,
                name,
                price,
                quantity,
                TotalAmount.Amount));
        }

        public void RemoveItem(string name)
        {
            if (StatusId == InvoiceStatusId.Paid)
                throw new InvalidOperationException("Cannot modify a paid invoice.");

            var item = _items.FirstOrDefault(x => x.Name == name);

            if (item == null)
                throw new InvalidOperationException("Item not found.");

            _items.Remove(item);

            RecalculateTotal();

            AddDomainEvent(new InvoiceItemRemovedEvent(
                Id,
                name,
                TotalAmount.Amount));
        }

        public void RegisterPayment(decimal amount)
        {
            if (StatusId == InvoiceStatusId.Cancelled)
                throw new InvalidOperationException("Cancelled invoice cannot receive payments.");

            if (amount <= 0)
                throw new InvalidOperationException("Payment amount must be greater than zero.");

            PaidAmount = PaidAmount.Add(new Money(amount));

            UpdateStatusAfterPayment();

            AddDomainEvent(new PaymentRegisteredEvent(
                Id,
                amount,
                PaidAmount.Amount,
                StatusId));
        }

        public void Cancel()
        {
            if (StatusId == InvoiceStatusId.Paid)
                throw new InvalidOperationException("Paid invoice cannot be cancelled.");

            StatusId = InvoiceStatusId.Cancelled;

            AddDomainEvent(new InvoiceCancelledEvent(Id, CustomerId));
        }

        public void MarkAsOverdue()
        {
            if (StatusId == InvoiceStatusId.Pending &&
                DueDate < DateTime.UtcNow)
            {
                StatusId = InvoiceStatusId.Overdue;

                AddDomainEvent(new InvoiceOverdueEvent(Id, CustomerId, DueDate));
            }
        }

        private void RecalculateTotal()
        {
            decimal total = _items.Sum(x => x.Total.Amount);
            TotalAmount = new Money(total);
            UpdateStatusAfterPayment();
        }

        private void UpdateStatusAfterPayment()
        {
            if (PaidAmount.Amount == 0)
            {
                StatusId = InvoiceStatusId.Pending;
                return;
            }

            if (PaidAmount.Amount < TotalAmount.Amount)
            {
                StatusId = InvoiceStatusId.PartiallyPaid;
                return;
            }

            if (PaidAmount.Amount == TotalAmount.Amount)
            {
                StatusId = InvoiceStatusId.Paid;
                return;
            }

            throw new InvalidOperationException(
                "Paid amount cannot exceed invoice total.");
        }
    }
}
