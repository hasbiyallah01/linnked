using System.Text.Json.Serialization;

namespace Linnked.Core.Domain.Entities
{
    public class User : Auditables
    {
        [JsonInclude]
        public string FirstName { get; set; }
        [JsonInclude]
        public string LastName { get; set; }
        [JsonInclude]
        public string Email { get; set; }
        [JsonInclude]
        public string? Password { get; set; }
        public int RoleId { get; set; }
        [JsonInclude]
        public Role Role { get; set; }
    }
}
