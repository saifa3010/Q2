using Domain.Events;

namespace Domain.Outbox
{
    /// <summary>
    /// Represents a serialized domain event waiting to be dispatched.
    /// Stored atomically alongside the aggregate change that produced it.
    /// </summary>
    public sealed class OutboxMessage
    {
        private OutboxMessage()
        {
            // EF Core
        }

        public Guid Id { get; private set; }

        /// <summary>
        /// Fully-qualified type name of the domain event (used for deserialization).
        /// </summary>
        public string Type { get; private set; } = string.Empty;

        /// <summary>
        /// JSON-serialized domain event payload.
        /// </summary>
        public string Payload { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Set when the message has been successfully processed.
        /// Null means it is pending or previously failed.
        /// </summary>
        public DateTime? ProcessedAt { get; private set; }

        /// <summary>
        /// Stores the exception message on processing failure.
        /// Null means no failure has occurred yet.
        /// </summary>
        public string? Error { get; private set; }

        public static OutboxMessage Create(string type, string payload)
            => new()
            {
                Id = Guid.NewGuid(),
                Type = type,
                Payload = payload,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = null,
                Error = null
            };

        public void MarkProcessed()
        {
            ProcessedAt = DateTime.UtcNow;
            Error = null;
        }

        public void MarkFailed(string error)
        {
            Error = error;
        }
    }
}
