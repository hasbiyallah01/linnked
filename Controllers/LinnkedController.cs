using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Chat;
using OpenAI_API;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Linnked.Infrastructure;
using Linnked.Models;
using Linnked.Core.Domain.Entities;


[ApiController]
[Route("api/linnked")]
public class LinnkedController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public LinnkedController(AppDbContext dbContext, HttpClient httpClient, IConfiguration configuration, IEmailService emailService)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _configuration = configuration;
        _emailService = emailService;
    }


    [HttpGet("{scrambledId}")]
    public async Task<IActionResult> GetMessage(string scrambledId)
    {
        var mapping = await _dbContext.ScrambledMessageMappings
                                      .FirstOrDefaultAsync(m => m.ScrambledGuid == scrambledId);

        if (mapping == null)
            return NotFound("Message not found");

        var message = await _dbContext.Messages.FindAsync(mapping.MessageId);

        return Ok(new
        {
            Title = message.MessageTitle,
            SenderFirstName = message.SenderFirstName,
            ReceiverFirstName = message.RecipientFirstName,
            CustomMessage = message.CustomMessage,
            MultiPage = message.MultiPage,
            Accepted = message.Accepted
        });
    }

    [HttpPost("{scrambledId}/respond")]
    public async Task<IActionResult> RespondToMessage(string scrambledId, [FromQuery] bool accept)
    {
        var mapping = await _dbContext.ScrambledMessageMappings
                                      .FirstOrDefaultAsync(m => m.ScrambledGuid == scrambledId);

        if (mapping == null)
            return NotFound("Message not found");

        var message = await _dbContext.Messages.FindAsync(mapping.MessageId);
        if (message == null)
            return NotFound("Message not found");

        message.Accepted = accept;

        BaseResponse emailResponse;
        if (accept)
        {
            emailResponse = await _emailService.SendAcceptanceEmailAsync(message);
            if (!emailResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Failed to send acceptance email.");
            }
        }
        else
        {
            emailResponse = await _emailService.SendRejectionEmailAsync(message);
            if (!emailResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  "Failed to send rejection email.");
            }
        }
        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = accept ? "You accepted the Valentine! ❤️" : "You rejected the Valentine 💔" });
    }

    private async Task<string> GenerateAiMessage(string personalityDescription)
    {
        string apiKey = _configuration["OpenAI:APIKey"];
        var openai = new OpenAIAPI(apiKey);

        var chatRequest = new ChatRequest
        {
            Model = "gpt-3.5-turbo",
            Messages = new[]
            {
                new ChatMessage(ChatMessageRole.System, "You are a romantic AI that writes heartfelt Valentine's messages in exactly 5 paragraphs with romantic emojis. Each paragraph should be between 20 and 25 words."),
                new ChatMessage(ChatMessageRole.User, $"Write a 5-paragraph Valentine's message for someone with this personality: {personalityDescription}. The message should include romantic emojis.")
            }
        };

        var result = await openai.Chat.CreateChatCompletionAsync(chatRequest);

        if (result == null || result.Choices == null || !result.Choices.Any())
            return "Sorry, AI message generation failed.";

        return result.Choices[0].Message.Content ?? "AI message error.";
    }

    [HttpPut("update/{scrambledId}/ai-response")]
    public async Task<IActionResult> UpdateAiResponse(string scrambledId, [FromBody] string newPersonalityDescription)
    {
        var mapping = await _dbContext.ScrambledMessageMappings
                                      .FirstOrDefaultAsync(m => m.ScrambledGuid == scrambledId);

        if (mapping == null)
            return NotFound("Message not found");

        var message = await _dbContext.Messages.FindAsync(mapping.MessageId);
        if (message == null)
            return NotFound("Message not found");

        message.CustomMessage = await GenerateAiMessage(newPersonalityDescription);
        message.PersonalityDescription = newPersonalityDescription;
        message.IsAiGenerated = true;

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            Title = message.MessageTitle,
            Message = "AI response updated successfully.",
            CustomMessage = message.CustomMessage,
            MultiPage = message.MultiPage
        });
    }

    [HttpPut("update/{scrambledId}/edit-ai-message")]
    public async Task<IActionResult> UpdateAiMessageWithoutRegeneration(string scrambledId, [FromBody] string newCustomMessage)
    {
        var mapping = await _dbContext.ScrambledMessageMappings
                                      .FirstOrDefaultAsync(m => m.ScrambledGuid == scrambledId);

        if (mapping == null)
            return NotFound("Message not found");

        var message = await _dbContext.Messages.FindAsync(mapping.MessageId);
        if (message == null)
            return NotFound("Message not found");

        message.CustomMessage = newCustomMessage;

        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            Title = message.MessageTitle,
            Message = "AI message updated successfully without regeneration.",
            CustomMessage = message.CustomMessage,
            MultiPage = message.MultiPage,
        });
    }

    [HttpPut("regenerate/{scrambledId}")]
    public async Task<IActionResult> RegenerateAiResponse(string scrambledId)
    {
        var mapping = await _dbContext.ScrambledMessageMappings
                                      .FirstOrDefaultAsync(m => m.ScrambledGuid == scrambledId);

        if (mapping == null)
            return NotFound("Message not found");

        var message = await _dbContext.Messages.FindAsync(mapping.MessageId);
        if (message == null)
            return NotFound("Message not found");

        if (string.IsNullOrWhiteSpace(message.PersonalityDescription))
            return BadRequest("Cannot regenerate AI message because no personality description exists.");

        string formerMessage = message.CustomMessage; // Store the previous message before regenerating
        string newMessage = await GenerateImprovedAiMessage(message.PersonalityDescription, formerMessage);

        message.CustomMessage = newMessage;
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            Title = message.MessageTitle,
            FormerMessage = formerMessage,
            NewMessage = newMessage,
            MultiPage = message.MultiPage,
            Message = "AI response regenerated with improvements."
        });
    }

    private async Task<string> GenerateImprovedAiMessage(string personalityDescription, string previousMessage)
    {
        string apiKey = _configuration["OpenAI:APIKey"];
        var openai = new OpenAIAPI(apiKey);

        var chatRequest = new ChatRequest
        {
            Model = "gpt-3.5-turbo",
            Messages = new[]
            {
                new ChatMessage(ChatMessageRole.System, "You are a romantic AI that writes heartfelt Valentine's messages in exactly 5 paragraphs with romantic emojis. Each paragraph should be between 20 and 25 words."),
                new ChatMessage(ChatMessageRole.User, $"The previous message was not good enough. Improve it while keeping the romantic tone. Make it more engaging, heartfelt, and unique. Previous message: \"{previousMessage}\". Personality: {personalityDescription}.")
            }
        };

        var result = await openai.Chat.CreateChatCompletionAsync(chatRequest);

        if (result == null || result.Choices == null || !result.Choices.Any())
            return "Sorry, AI message regeneration failed.";

        return result.Choices[0].Message.Content ?? "AI message error.";
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendValentine([FromBody] MessageDTO message)
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}/api/linnked";
        string link = string.Empty;

        if (message.IsAiGenerated)
        {
            if (string.IsNullOrWhiteSpace(message.PersonalityDescription))
                return BadRequest("A personality description is required for AI-generated messages.");

            message.CustomMessage = await GenerateAiMessage(message.PersonalityDescription);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(message.CustomMessage))
                return BadRequest("A custom message is required if AI generation is not selected.");
        }
        var existingMessage = await _dbContext.Messages
            .FirstOrDefaultAsync(m => m.SenderEmail == message.SenderEmail);

        if (existingMessage == null)
        {
            var newMessage = new Message()
            {
                IsAiGenerated = message.IsAiGenerated,
                PersonalityDescription = message.PersonalityDescription ?? null,
                CustomMessage = message.CustomMessage,
                SenderFirstName = message.SenderFirstName,
                RecipientFirstName = message.RecipientFirstName,
                DateCreated = DateTime.UtcNow,
                Accepted = false,
                SenderEmail = message.SenderEmail,
                MessageTitle = message.MessageTitle,
                MultiPage = message.MultiPage
            };

            var emailResponse = await _emailService.SendWelcomeEmailAsync(newMessage);
            if (!emailResponse.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send welcome email.");
            }

            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            var scrambledGuid = GenerateUniqueScrambledGuid(message.SenderFirstName);

            var scrambledMessageMapping = new ScrambledMessageMapping
            {
                ScrambledGuid = scrambledGuid,
                MessageId = newMessage.Id
            };

            _dbContext.ScrambledMessageMappings.Add(scrambledMessageMapping);
            await _dbContext.SaveChangesAsync();

            link = $"{baseUrl}?scrambledId={scrambledGuid}";
        }
        else
        {
            var newMessage = new Message()
            {
                IsAiGenerated = message.IsAiGenerated,
                PersonalityDescription = message.PersonalityDescription ?? null,
                CustomMessage = message.CustomMessage,
                SenderFirstName = message.SenderFirstName,
                RecipientFirstName = message.RecipientFirstName,
                DateCreated = DateTime.UtcNow,
                Accepted = false,
                SenderEmail = message.SenderEmail,
                MessageTitle = message.MessageTitle,
                MultiPage = message.MultiPage
            };

            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            var scrambledGuid = GenerateUniqueScrambledGuid(message.SenderFirstName);

            var scrambledMessageMapping = new ScrambledMessageMapping
            {
                ScrambledGuid = scrambledGuid,
                MessageId = newMessage.Id
            };

            _dbContext.ScrambledMessageMappings.Add(scrambledMessageMapping);
            await _dbContext.SaveChangesAsync();

            link = $"{baseUrl}?scrambledId={scrambledGuid}";
        }

        return Ok(new
        {
            Title = message.MessageTitle,
            SenderFirstName = message.SenderFirstName,
            ReceiverFirstName = message.RecipientFirstName,
            MessageBody = message.CustomMessage,
            MultiPage = message.MultiPage,
            Link = link
        });
    }



    private string GenerateUniqueScrambledGuid(string senderFirstName)
    {
        string scrambledGuid;
        bool isUnique;

        do
        {
            scrambledGuid = senderFirstName.ToLower() + "-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            isUnique = !_dbContext.ScrambledMessageMappings
                .Any(s => s.ScrambledGuid == scrambledGuid);

        } while (!isUnique);

        return scrambledGuid;
    }

    


    [HttpGet]
    public async Task<IActionResult> GetValentineMessage([FromQuery] string scrambledId)
    {
        var mapping = await _dbContext.ScrambledMessageMappings
                                      .FirstOrDefaultAsync(m => m.ScrambledGuid == scrambledId);

        if (mapping == null)
            return NotFound("Message not found");

        var message = await _dbContext.Messages.FindAsync(mapping.MessageId);
        if (message == null)
            return NotFound("Message not found");

        string baseUrl = $"{Request.Scheme}://{Request.Host}/api/linnked";
        string link = $"{baseUrl}?scrambledId={scrambledId}";
        return Ok(new
        {
            Title = message.MessageTitle,
            SenderFirstName = message.SenderFirstName,
            RecipientFirstName = message.RecipientFirstName,
            MessageBody = message.CustomMessage,
            MultiPage = message.MultiPage,
            Link = link
        });
    }


    [HttpPut("edit/multipage")]
    public async Task<IActionResult> UpdateMultiPage([FromQuery] string scrambledId, [FromBody] bool multiPage)
    {
        var scrambledMapping = await _dbContext.ScrambledMessageMappings
            .FirstOrDefaultAsync(s => s.ScrambledGuid == scrambledId);

        if (scrambledMapping == null)
        {
            return NotFound("Invalid scrambled ID.");
        }

        var existingMessage = await _dbContext.Messages
            .FirstOrDefaultAsync(m => m.Id == scrambledMapping.MessageId);

        if (existingMessage == null)
        {
            return NotFound("Message not found.");
        }

        existingMessage.MultiPage = multiPage;

        _dbContext.Messages.Update(existingMessage);
        await _dbContext.SaveChangesAsync();

        string baseUrl = $"{Request.Scheme}://{Request.Host}/api/linnked";
        string link = $"{baseUrl}?scrambledId={scrambledId}";

        return Ok(new
        {
            Title = existingMessage.MessageTitle,
            SenderFirstName = existingMessage.SenderFirstName,
            ReceiverFirstName = existingMessage.RecipientFirstName,
            MessageBody = existingMessage.CustomMessage,
            MultiPage = existingMessage.MultiPage,
            Link = link
        });
    }


    [HttpPut("edit/title")]
    public async Task<IActionResult> UpdateMessageTitle([FromQuery] string scrambledId, [FromBody] string messageTitle)
    {
        var scrambledMapping = await _dbContext.ScrambledMessageMappings
            .FirstOrDefaultAsync(s => s.ScrambledGuid == scrambledId);

        if (scrambledMapping == null)
        {
            return NotFound("Invalid scrambled ID.");
        }

        var existingMessage = await _dbContext.Messages
            .FirstOrDefaultAsync(m => m.Id == scrambledMapping.MessageId);

        if (existingMessage == null)
        {
            return NotFound("Message not found.");
        }

        existingMessage.MessageTitle = messageTitle;

        _dbContext.Messages.Update(existingMessage);
        await _dbContext.SaveChangesAsync();

        string baseUrl = $"{Request.Scheme}://{Request.Host}/api/linnked";
        string link = $"{baseUrl}?scrambledId={scrambledId}";

        return Ok(new
        {
            Title = existingMessage.MessageTitle,
            SenderFirstName = existingMessage.SenderFirstName,
            ReceiverFirstName = existingMessage.RecipientFirstName,
            MessageBody = existingMessage.CustomMessage,
            MultiPage = existingMessage.MultiPage,
            Link = link
        });
    }

    [HttpGet("messages-by-email/{email}")]
    public async Task<IActionResult> GetMessagesByEmail(string email)
    {
        var messages = await _dbContext.Messages
            .Where(m => m.SenderEmail == email)
            .OrderBy(m => m.Id)
            .ToListAsync();

        if (!messages.Any())
        {
            return NotFound(new { Message = "No messages found for this email" });
        }

        var numberedMessages = messages
            .Select((m, index) => new
            {
                Number = index + 1,
                m.RecipientFirstName,
                m.CustomMessage,
                m.DateCreated,
                m.IsAiGenerated,
                m.PersonalityDescription,
                m.MessageTitle,
                m.Accepted,
            })
            .ToList();

        return Ok(numberedMessages);
    }
}