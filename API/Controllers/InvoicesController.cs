using Application.Common.DTOs;
using Application.Invoices.Commands;
using Application.Invoices.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    [Authorize]
    public sealed class InvoicesController : ControllerBase
    {
        private readonly ISender _sender;

        public InvoicesController(ISender sender)
            => _sender = sender;

        // ── GET /api/invoices ─────────────────────────────────────────────
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] long? customerId,
            [FromQuery] InvoiceStatusId? statusId,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetInvoicesQuery(customerId, statusId),
                cancellationToken);

            return Ok(result);
        }

        // ── GET /api/invoices/{id} ────────────────────────────────────────
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetInvoiceByIdQuery(id),
                cancellationToken);

            return Ok(result);
        }

        // ── POST /api/invoices ────────────────────────────────────────────
        [HttpPost]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateInvoiceRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateInvoiceCommand(request.CustomerId, request.DueDate),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // ── POST /api/invoices/{id}/items ─────────────────────────────────
        [HttpPost("{id:guid}/items")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItem(
            Guid id,
            [FromBody] AddInvoiceItemRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new AddInvoiceItemCommand(id, request.Name, request.Price, request.Quantity),
                cancellationToken);

            return Ok(result);
        }

        // ── DELETE /api/invoices/{id}/items/{name} ────────────────────────
        [HttpDelete("{id:guid}/items/{name}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItem(
            Guid id,
            string name,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new RemoveInvoiceItemCommand(id, name),
                cancellationToken);

            return Ok(result);
        }

        // ── POST /api/invoices/{id}/cancel ────────────────────────────────
        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CancelInvoiceCommand(id),
                cancellationToken);

            return Ok(result);
        }
    }

    // ── Request models ────────────────────────────────────────────────────

    public sealed record CreateInvoiceRequest(int CustomerId, DateTime DueDate);

    public sealed record AddInvoiceItemRequest(string Name, decimal Price, int Quantity);
}
