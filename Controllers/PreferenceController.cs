
using Linnked.Core.Application.Interfaces.Services;
using Linnked.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Linnked.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreferenceController : ControllerBase
    {
        private readonly IPreferenceService _preferenceService;

        public PreferenceController(IPreferenceService preferenceService)
        {
            _preferenceService = preferenceService;
        }

        [HttpGet("{scrambledGuid}")]
        public async Task<IActionResult> GetPreference(string scrambledGuid)
        {
            var preference = await _preferenceService.GetPreference(scrambledGuid);
            if (preference == null)
            {
                return NotFound("Preference not found.");
            }
            return Ok(preference);
        }

        [HttpPost("{scrambledGuid}")]
        public async Task<IActionResult> SavePreference(string scrambledGuid, [FromBody] PreferenceRequest request)
        {
            if (request == null) return BadRequest("Invalid preference data.");

            var result = await _preferenceService.SavePreference(scrambledGuid, request);
            return result ? Ok("Preference saved successfully.") : StatusCode(500, "Failed to save preference.");
        }

        [HttpPut("{scrambledGuid}")]
        public async Task<IActionResult> UpdatePreference(string scrambledGuid, [FromBody] PreferenceRequest request)
        {
            if (request == null) return BadRequest("Invalid preference data.");

            var result = await _preferenceService.UpdatePreference(scrambledGuid, request);
            return result ? Ok("Preference updated successfully.") : NotFound("Preference not found.");
        }

        [HttpGet("can-set/{userId}")]
        public async Task<IActionResult> CanUserSetPreference(string userId)
        {
            var result = await _preferenceService.CanUserSetPreference(userId);
            return Ok(new { CanSetPreference = result });
        }

        [HttpPost("set/{userId}")]
        public async Task<IActionResult> SetPreference(string userId, [FromBody] PreferenceRequest request)
        {
            if (request == null) return BadRequest("Invalid preference data.");

            var result = await _preferenceService.SetPreference(userId, request);
            return result ? Ok("Preference set successfully.") : BadRequest("User has exceeded the free trial limit or has not paid.");
        }
    }
}
