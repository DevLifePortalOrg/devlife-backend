using DevLife.Api.Models;
using DevLife.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BugChaseController : ControllerBase
    {
        private readonly BugChaseService _bugChaseService;

        public BugChaseController(BugChaseService bugChaseService)
        {
            _bugChaseService = bugChaseService;
        }

        private Guid GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found in claims.");
        }

        [HttpGet("challenge/random")]
        [ProducesResponseType(typeof(CodeChallenge), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRandomChallenge([FromQuery] string difficulty)
        {
            var challenge = await _bugChaseService.GetRandomBugChallenge(difficulty);
            if (challenge == null)
            {
                return NotFound("No challenges available for the specified difficulty.");
            }
            return Ok(challenge);
        }

        [HttpPost("submit-fix")]
        [ProducesResponseType(typeof(BugFixResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitBugFix([FromBody] SubmitBugFixDto submitDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            var result = await _bugChaseService.SubmitBugFix(userId, submitDto);
            return Ok(result);
        }
    }
}