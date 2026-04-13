using Domain.Enums;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public int CustomerId { get; set; }
        public DateTime DueDate { get; set; }
        public Money TotalAmount { get; private set; }
        public Money PaidAmount { get; private set; }
        public List<InvoiceItem> Items { get; private set; }
        public InvoiceStatusId Status { get; private set; }

    }
}
