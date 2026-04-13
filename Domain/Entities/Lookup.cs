using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public class Lookup : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
    }
}
