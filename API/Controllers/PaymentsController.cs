using Application.Common.DTOs;
using Application.Payments.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/invoices/{invoiceId:guid}/payments")]
    [Authorize]
    public sealed class PaymentsController : ControllerBase
    {
        private readonly ISender _sender;

        public PaymentsController(ISender sender)
            => _sender = sender;

        // ── POST /api/invoices/{invoiceId}/payments ───────────────────────
        [HttpPost]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterPayment(
            Guid invoiceId,
            [FromBody] RegisterPaymentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new RegisterPaymentCommand(invoiceId, request.Amount, request.ReferenceNumber),
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result);
        }
    }

    public sealed record RegisterPaymentRequest(decimal Amount, string ReferenceNumber);
}
