using Domain.Events;

namespace Domain.Outbox
{
    public sealed class OutboxMessage
    {
        private OutboxMessage()
        {
        }

        public Guid Id { get; private set; }
        public string Type { get; private set; } = string.Empty;
        public string Payload { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; private set; }

        public DateTime? ProcessedAt { get; private set; }

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
