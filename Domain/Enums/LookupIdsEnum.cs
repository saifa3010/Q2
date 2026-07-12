using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Domain.Enums
{
    public enum InvoiceStatusId
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Partially Paid")]
        PartiallyPaid = 2,

        [Description("Paid")]
        Paid = 3,

        [Description("Overdue")]
        Overdue = 4,

        [Description("Cancelled")]
        Cancelled = 5
    }

    public static class InvoiceStatusIdExtensions
    {
        public static string GetDisplayName(this InvoiceStatusId status)
        {
            var field = status.GetType().GetField(status.ToString());
            var description = field?.GetCustomAttribute<DescriptionAttribute>();
            return description?.Description ?? status.ToString();
        }
    }
    public enum PaymentStatusId
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Completed")]
        Completed = 2,
        [Description("Failed")]
        Failed = 3,
        [Description("Reversed")]
        Reversed = 4
    }
    public static class PaymentStatusIdExtensions
    {
        public static string GetDisplayName(this PaymentStatusId status)
        {
            var field = status.GetType().GetField(status.ToString());
            var description = field?.GetCustomAttribute<DescriptionAttribute>();
            return description?.Description ?? status.ToString();
        }
    }
}
