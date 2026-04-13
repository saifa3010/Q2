using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime? CreatedAt { get; protected set; }
        public int? CreatedBy { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public int? UpdatedBy { get; protected set; }
        public bool IsDeleted { get; protected set; } = false;
    }
}