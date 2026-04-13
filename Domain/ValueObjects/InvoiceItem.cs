using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ValueObjects
{
    public class InvoiceItem
    {
        public string Name { get; }
        public Money Price { get; }
        public int Quantity { get; }

        public Money Total => new Money(Price.Amount * Quantity);
    }
}
