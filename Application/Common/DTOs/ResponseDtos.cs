using Domain.Enums;

namespace Application.Common.DTOs
{
    public sealed record InvoiceItemDto(
        string Name,
        decimal Price,
        int Quantity,
        decimal Total);

    public sealed record InvoiceDto(
        Guid Id,
        long CustomerId,
        DateTime DueDate,
        decimal TotalAmount,
        decimal PaidAmount,
        InvoiceStatusId StatusId,
        IReadOnlyList<InvoiceItemDto> Items,
        DateTime? CreatedAt);

    public sealed record PaymentDto(
        Guid Id,
        Guid InvoiceId,
        decimal Amount,
        PaymentStatusId StatusId,
        DateTime? CreatedAt);

    public sealed record UserDto(
        Guid Id,
        string KeycloakUserId,
        string Email,
        string Username);
}