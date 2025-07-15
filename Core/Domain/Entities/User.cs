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


    public class Message
    {
        public int Id { get; set; }
        public string SenderFirstName { get; set; } = default!;
        public string SenderEmail { get; set; } = default!;
        public string RecipientFirstName { get; set; } = default!;
        public string? CustomMessage { get; set; } = default!;
        public DateTime DateCreated { get; set; }
        public bool IsAiGenerated { get; set; }
        public string? PersonalityDescription { get; set; }
        public string MessageTitle { get; set; } = default!;
        public bool MultiPage { get; set; }
        public bool Accepted { get; set; }
    }
}
