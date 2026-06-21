using Application.Common.DTOs;
using Domain.Entities;

namespace Application.Common.DTOs
{
    public static class MappingExtensions
    {
        public static InvoiceDto ToDto(this Invoice invoice) =>
            new(
                invoice.Id,
                invoice.CustomerId,
                invoice.DueDate,
                invoice.TotalAmount.Amount,
                invoice.PaidAmount.Amount,
                invoice.StatusId,
                invoice.Items.Select(i => i.ToDto()).ToList(),
                invoice.CreatedAt);

        public static InvoiceItemDto ToDto(this InvoiceItem item) =>
            new(
                item.Name,
                item.Price.Amount,
                item.Quantity,
                item.Total.Amount);

        public static PaymentDto ToDto(this Payment payment) =>
            new(
                payment.Id,
                payment.InvoiceId,
                payment.Amount.Amount,
                payment.StatusId,
                payment.CreatedAt);

        public static UserDto ToDto(this AppUser user) =>
            new(
                user.Id,
                user.KeycloakUserId,
                user.Email,
                user.FullName);
    }
}