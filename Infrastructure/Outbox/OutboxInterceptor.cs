using System.Text.Json;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Interceptors
{
    /// <summary>
    /// Intercepts SaveChanges to harvest domain events from all tracked aggregates,
    /// serialize them as OutboxMessage rows, and insert them in the same transaction.
    /// After saving, clears the in-memory event collections.
    /// </summary>
    public sealed class OutboxInterceptor : SaveChangesInterceptor
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            WriteIndented = false
        };

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null)
                return base.SavingChangesAsync(eventData, result, cancellationToken);

            ConvertDomainEventsToOutboxMessages(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context is null)
                return base.SavingChanges(eventData, result);

            ConvertDomainEventsToOutboxMessages(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
                ClearAllDomainEvents(eventData.Context);

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public override int SavedChanges(
            SaveChangesCompletedEventData eventData,
            int result)
        {
            if (eventData.Context is not null)
                ClearAllDomainEvents(eventData.Context);

            return base.SavedChanges(eventData, result);
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private static void ConvertDomainEventsToOutboxMessages(DbContext context)
        {
            var outboxMessages = context.ChangeTracker
                .Entries<BaseEntity>()
                .Select(entry => entry.Entity)
                .SelectMany(entity => entity.DomainEvents)
                .Select(ToOutboxMessage)
                .ToList();

            if (outboxMessages.Count > 0)
                context.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        private static OutboxMessage ToOutboxMessage(IDomainEvent domainEvent)
        {
            // Store the fully-qualified type name so the processor can
            // deserialize back to the concrete event type at runtime.
            var type = domainEvent.GetType().AssemblyQualifiedName
                       ?? domainEvent.GetType().FullName
                       ?? domainEvent.GetType().Name;

            var payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), _serializerOptions);

            return OutboxMessage.Create(type, payload);
        }

        private static void ClearAllDomainEvents(DbContext context)
        {
            var entities = context.ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Count > 0)
                .ToList();

            foreach (var entity in entities)
                entity.ClearDomainEvents();
        }
    }
}
