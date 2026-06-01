using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum InvoiceStatusId
    {
        Pending = 1,
        Paid = 2,
        PartiallyPaid = 3,
        Overdue = 4,
        Cancelled = 5

    }
    public enum PaymentStatusId
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Reversed = 4
    }
}
