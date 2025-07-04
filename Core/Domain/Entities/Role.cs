﻿using Linnked.Core.Domain.Entities;
using System.Text.Json.Serialization;

namespace Linnked.Core.Domain.Entities
{
    public class Role : Auditables
    {
        [JsonInclude]
        public string Name { get; set; } = default!;
        [JsonInclude]
        public string? Description { get; set; }
        public ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
