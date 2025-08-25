
namespace Linnked.Core.Domain.Entities
{

    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
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

    public class WaitList
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime DateCreated { get; set; }
    }

    public class WaitListDTO
    {
        public string FirstName { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public string CallbackUrl { get; set; }
    }

    public class PaystackSettings
    {
        public string SecretKey { get; set; }
        public string PublicKey { get; set; }
        public string BaseUrl { get; set; }
    }

    public class MessageDTO
    {
        public string SenderFirstName { get; set; } = default!;
        public string SenderEmail { get; set; } = default!;
        public string RecipientFirstName { get; set; } = default!;
        public string? CustomMessage { get; set; }
        public bool IsAiGenerated { get; set; }
        public string? PersonalityDescription { get; set; }
        public string MessageTitle { get; set; } = default!;
        public bool MultiPage { get; set; }
    }


    public class ScrambledMessageMapping
    {
        public int Id { get; set; }
        public string ScrambledGuid { get; set; } = default!;
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }


    public class Preference
    {
        public int Id { get; set; }
        public string ScrambledGuid { get; set; } = default!;
        public string FormatType { get; set; } = default!;
        public string Color { get; set; } = default!;
        public string Background { get; set; } = default!;
        public string Font { get; set; } = default!;
        public DateTime DateCreated { get; set; } = default!;
        public bool HasPaid { get; set; } = false;
        public int TrialCount { get; set; } = 0; 
    }


    public class PreferenceRequest
    {
        public string FormatType { get; set; } = default!;
        public string Color { get; set; } = default!;
        public string Background { get; set; } = default!;
        public string Font { get; set; } = default!;
    }


    
}
