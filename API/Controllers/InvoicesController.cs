using Application.Common.DTOs;
using Application.Invoices.Commands;
using Application.Invoices.Queries;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    [Tags("Invoices")]
    //[Authorize]
    public sealed class InvoicesController : ControllerBase
    {
        private readonly ISender _sender;

        public InvoicesController(ISender sender)
        {
            _sender = sender;
        }

        // GET: api/invoices
        [HttpGet(Name = "GetInvoices")]
        [SwaggerOperation(
            Summary = "Get all invoices",
            Description = "Returns all invoices. Optionally filter by customer ID and invoice status.")]
        [ProducesResponseType(typeof(IReadOnlyList<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInvoices(
            [FromQuery] long? customerId,
            [FromQuery] InvoiceStatusId? statusId,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetInvoicesQuery(customerId, statusId),
                cancellationToken);

            return Ok(result);
        }

        // GET: api/invoices/{id}
        [HttpGet("{id:guid}", Name = "GetInvoiceById")]
        [SwaggerOperation(
            Summary = "Get invoice by ID",
            Description = "Returns the invoice identified by the specified ID.")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvoiceById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetInvoiceByIdQuery(id),
                cancellationToken);

            return Ok(result);
        }

        // POST: api/invoices
        [HttpPost(Name = "CreateInvoice")]
        [SwaggerOperation(
            Summary = "Create a new invoice",
            Description = "Creates a new invoice for the specified customer.")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInvoice(
            [FromBody] CreateInvoiceRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateInvoiceCommand(request.CustomerId, request.DueDate),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetInvoiceById),
                new { id = result.Id },
                result);
        }

        // POST: api/invoices/{id}/items
        [HttpPost("{id:guid}/items", Name = "AddInvoiceItem")]
        [SwaggerOperation(
            Summary = "Add item to invoice",
            Description = "Adds a new item to the specified invoice.")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddInvoiceItem(
            Guid id,
            [FromBody] AddInvoiceItemRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new AddInvoiceItemCommand(
                    id,
                    request.Name,
                    request.Price,
                    request.Quantity),
                cancellationToken);

            return Ok(result);
        }

        // DELETE: api/invoices/{id}/items/{name}
        [HttpDelete("{id:guid}/items/{name}", Name = "RemoveInvoiceItem")]
        [SwaggerOperation(
            Summary = "Remove item from invoice",
            Description = "Removes an item from the specified invoice by item name.")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveInvoiceItem(
            Guid id,
            string name,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new RemoveInvoiceItemCommand(id, name),
                cancellationToken);

            return Ok(result);
        }

        // POST: api/invoices/{id}/cancel
        [HttpPost("{id:guid}/cancel", Name = "CancelInvoice")]
        [SwaggerOperation(
            Summary = "Cancel invoice",
            Description = "Cancels the specified invoice.")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelInvoice(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CancelInvoiceCommand(id),
                cancellationToken);

            return Ok(result);
        }
    }

    // Request Models

    public sealed record CreateInvoiceRequest(
        int CustomerId,
        DateTime DueDate);

    public sealed record AddInvoiceItemRequest(
        string Name,
        decimal Price,
        int Quantity);
}