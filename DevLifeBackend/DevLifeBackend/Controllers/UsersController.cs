// DevLife.Api/Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using DevLife.Api.Models;
using DevLife.Api.Services;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // To get user ID from claims

namespace DevLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All actions in this controller require authentication by default
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        private Guid GetUserIdFromClaims()
        {
            // This assumes your JWT setup includes a ClaimTypes.NameIdentifier
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found in claims.");
        }

        /// <summary>
        /// Gets the current authenticated user's profile.
        /// </summary>
        /// <returns>UserProfile DTO.</returns>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = GetUserIdFromClaims();
            var profile = await _userService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                return NotFound("User profile not found.");
            }
            return Ok(profile);
        }

        /// <summary>
        /// Updates the current authenticated user's profile.
        /// </summary>
        /// <param name="updateDto">Fields to update in the user profile.</param>
        /// <returns>No content if successful, or bad request/not found.</returns>
        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            var success = await _userService.UpdateUserProfileAsync(userId, updateDto);
            if (!success)
            {
                return NotFound("User profile not found or update failed.");
            }
            return NoContent(); // 204 No Content typically for successful PUT/DELETE
        }
    }
}