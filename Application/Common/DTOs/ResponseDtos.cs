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
        string StatusName,
        IReadOnlyList<InvoiceItemDto> Items,
        DateTime? CreatedAt);
    public sealed record InvoiceStatusDto(int Id, string Name);

    public sealed record PaymentDto(
        Guid Id,
        Guid InvoiceId,
        decimal Amount,
        string ReferenceNumber,
        PaymentStatusId StatusId,
        string StatusName,
        DateTime PaymentDate);
    public sealed record PaymentStatusDto(int Id, string Name);
    public sealed record UserDto(
        Guid Id,
        string KeycloakUserId,
        string FullName,
        string Email,
        string PhoneNumber);
}