using Application.Common.DTOs;
using Application.Payments.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/invoices/{invoiceId:guid}/payments")]
    [Tags("Payments")]
    //[Authorize]
    public sealed class PaymentsController : ControllerBase
    {
        private readonly ISender _sender;

        public PaymentsController(ISender sender)
            => _sender = sender;

        // POST: api/invoices/{invoiceId}/payments
        [HttpPost(Name = "CreateInvoicePayment")]
        [SwaggerOperation(
            Summary = "Create payment for invoice",
            Description = "Registers a new payment against the specified invoice.")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreatePayment(
            Guid invoiceId,
            [FromBody] RegisterPaymentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new RegisterPaymentCommand(
                    invoiceId,
                    request.Amount,
                    request.ReferenceNumber),
                cancellationToken);

            return CreatedAtAction(
                nameof(CreatePayment),
                new { invoiceId },
                result);
        }
    }

    public sealed record RegisterPaymentRequest(
        decimal Amount,
        string ReferenceNumber);
}