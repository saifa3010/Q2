using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Payment : BaseEntity
    {
        private Payment()
        {
            // EF Core
        }

        public Money Amount { get; private set; }

        public DateTime PaymentDate { get; private set; }

        public string ReferenceNumber { get; private set; }

        public string Notes { get; private set; }

        public PaymentStatusId StatusId { get; private set; }

        public Guid InvoiceId { get; private set; }

        public static Payment Create(
            Guid invoiceId,
            decimal amount,
            string referenceNumber,
            string notes = null)
        {
            if (invoiceId == Guid.Empty)
                throw new ArgumentException("Invalid invoice.");

            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(referenceNumber))
                throw new ArgumentException("Reference number is required.");

            return new Payment
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoiceId,
                Amount = new Money(amount),
                PaymentDate = DateTime.UtcNow,
                ReferenceNumber = referenceNumber,
                Notes = notes,
                StatusId = PaymentStatusId.Completed
            };
        }
    }
}