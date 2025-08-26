using Valentine.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Linnked.Infrastructure;
using Linnked.Models.UserModel;

namespace Linnked.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly AppDbContext _dbContext;

        public UserController(IUserService userService, ILogger<UserController> logger,AppDbContext dbContext)
        {
            _userService = userService;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await _userService.GetUser(id);
            if (user == null || !user.IsSuccessful)
            {
                _logger.LogError("User not found: {UserId}", id);
                return NotFound(new { Message = user?.Message ?? "User not found" });
            }

            return Ok(user.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UserRequest request)
        {
            var userResponse = await _userService.UpdateUser(id, request);
            if (userResponse.IsSuccessful)
            {
                _logger.LogInformation("User updated successfully: {UserId}", id);
                return Ok(new { userResponse.Message });
            }

            _logger.LogError("User update failed: {UserMessage}", userResponse.Message);
            return BadRequest(new { userResponse.Message });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var userResponse = await _userService.RemoveUser(id);
            if (userResponse.IsSuccessful)
            {
                _logger.LogInformation("User deleted successfully: {UserId}", id);
                return Ok(new { userResponse.Message });
            }

            _logger.LogError("User deletion failed: {UserMessage}", userResponse.Message);
            return BadRequest(new { userResponse.Message });
        }

        [HttpGet("all-grouped-by-email")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllMessagesGroupedByEmail()
        {
            var groupedMessages = await _dbContext.Messages
                .GroupBy(m => m.SenderEmail)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    SenderEmail = g.Key,
                    Messages = g.ToList()
                })
                .ToListAsync();

            return Ok(groupedMessages);
        }

        [HttpGet("all-grouped-by-name")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllMessagesGroupedByName()
        {
            var groupedMessages = await _dbContext.Messages
                .GroupBy(m => m.SenderFirstName)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    SenderFirstName = g.Key,
                    Messages = g.ToList()
                })
                .ToListAsync();

            return Ok(groupedMessages);
        }

        [HttpGet("message/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMessageById(int id)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound(new { Message = "Message not found" });
            }

            return Ok(message);
        }

        [HttpGet("all-ordered-by-id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllMessagesOrderedById()
        {
            var messages = await _dbContext.Messages
                .OrderBy(m => m.Id)
                .ToListAsync();

            return Ok(messages);
        }
    }
}
