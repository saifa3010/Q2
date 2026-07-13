using System.Text.Json;
using Domain.Events;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Outbox
{
    public sealed class OutboxProcessorJob
    {
        private readonly AppDbContext _dbContext;
        private readonly IPublisher _publisher;
        private readonly ILogger<OutboxProcessorJob> _logger;

        private const int _batchSize = 20;

        public OutboxProcessorJob(AppDbContext dbContext, IPublisher publisher, ILogger<OutboxProcessorJob> logger)
        {
            _dbContext = dbContext;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task ProcessAsync()
        {
            var messages = await _dbContext.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.CreatedAt)
                .Take(_batchSize)
                .ToListAsync();

            if (messages.Count == 0)
                return;

            _logger.LogInformation("Processing {Count} outbox message(s).", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    var eventType = ResolveType(message.Type);

                    if (eventType is null)
                    {
                        _logger.LogWarning(
                            "Cannot resolve type '{Type}' for outbox message {Id}. Skipping.",
                            message.Type, message.Id);

                        message.MarkFailed($"Type '{message.Type}' could not be resolved.");
                        continue;
                    }

                    var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType)
                        as IDomainEvent;

                    if (domainEvent is null)
                    {
                        message.MarkFailed("Deserialization returned null.");
                        continue;
                    }

                    // MediatR Publish dispatches to all INotificationHandler<T> subscribers.
                    await _publisher.Publish(domainEvent);

                    message.MarkProcessed();

                    _logger.LogInformation(
                        "Outbox message {Id} ({Type}) processed successfully.",
                        message.Id, eventType.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox message {Id}.", message.Id);

                    // Record the error but leave ProcessedAt null — Hangfire will retry
                    // on the next scheduled trigger.
                    message.MarkFailed(ex.Message);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Resolves the concrete CLR type from the stored AssemblyQualifiedName.
        /// Falls back to scanning the Domain assembly for a matching short name.
        /// </summary>
        private static Type? ResolveType(string typeName)
        {
            var resolved = Type.GetType(typeName);
            if (resolved is not null)
                return resolved;

            return typeof(IDomainEvent).Assembly
                .GetTypes()
                .FirstOrDefault(t =>
                    t.FullName == typeName ||
                    t.Name == typeName);
        }
    }
}
