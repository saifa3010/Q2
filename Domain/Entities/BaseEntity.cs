using Domain.Events;

namespace Domain.Entities
{
    public abstract class BaseEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public Guid Id { get; protected set; }
        public DateTime? CreatedAt { get; protected set; }
        public int? CreatedBy { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public int? UpdatedBy { get; protected set; }
        public bool IsDeleted { get; protected set; } = false;

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents()
            => _domainEvents.Clear();
    }
}
