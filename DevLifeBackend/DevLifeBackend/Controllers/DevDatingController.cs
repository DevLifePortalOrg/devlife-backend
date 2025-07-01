using DevLife.Api.Hubs;
using DevLife.Api.Models;
using DevLife.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DevLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DevDatingController : ControllerBase
    {
        private readonly DevDatingService _devDatingService;
        private readonly IHubContext<GameHub> _hubContext; 
        public DevDatingController(DevDatingService devDatingService, IHubContext<GameHub> hubContext)
        {
            _devDatingService = devDatingService;
            _hubContext = hubContext;
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

        [HttpPost("profile")]
        [ProducesResponseType(typeof(DatingProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrUpdateProfile([FromBody] CreateUpdateDatingProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            var profile = await _devDatingService.CreateOrUpdateDatingProfile(userId, profileDto);
            return Ok(profile);
        }

        [HttpGet("profile")]
        [ProducesResponseType(typeof(DatingProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserIdFromClaims();
            var profile = await _devDatingService.GetDatingProfile(userId);
            if (profile == null)
            {
                return NotFound("Dating profile not found.");
            }
            return Ok(profile);
        }

        [HttpGet("matches")]
        [ProducesResponseType(typeof(IEnumerable<DatingMatchDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FindMatches()
        {
            var userId = GetUserIdFromClaims();
            var matches = await _devDatingService.FindMatches(userId);
            return Ok(matches);
        }

        [HttpPost("{likedUserId}/like")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LikeProfile(Guid likedUserId)
        {
            var currentUserId = GetUserIdFromClaims();
            if (currentUserId == likedUserId)
            {
                return BadRequest("Cannot like your own profile.");
            }

            var isMatch = await _devDatingService.RecordLike(currentUserId, likedUserId);

            if (isMatch)
            {
                await _hubContext.Clients.User(currentUserId.ToString()).SendAsync("ReceiveNotification", "New Match!", $"You matched with {likedUserId}!");
                await _hubContext.Clients.User(likedUserId.ToString()).SendAsync("ReceiveNotification", "New Match!", $"You matched with {currentUserId}!");
            }

            return Ok(isMatch);
        }
    }
}