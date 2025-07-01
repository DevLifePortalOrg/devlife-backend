using Microsoft.AspNetCore.Mvc;
using DevLife.Api.Models;
using DevLife.Api.Services;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace DevLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CodeRoastController : ControllerBase
    {
        private readonly CodeRoastService _codeRoastService;

        public CodeRoastController(CodeRoastService codeRoastService)
        {
            _codeRoastService = codeRoastService;
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

        [HttpPost]
        [ProducesResponseType(typeof(CodeSnippet), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitCodeForRoast([FromBody] SubmitCodeRoastDto submitDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            var codeSnippet = await _codeRoastService.SubmitCodeForRoast(userId, submitDto);
            return CreatedAtAction(nameof(GetRoastById), new { id = codeSnippet.Id }, codeSnippet);
        }

        [HttpPost("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddRoastComment(Guid id, [FromBody] AddRoastCommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            var success = await _codeRoastService.AddRoastComment(id, userId, commentDto);
            if (!success)
            {
                return NotFound("Code roast not found.");
            }
            return NoContent();
        }

        [HttpPut("{id}/react")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReactToRoast(Guid id, [FromQuery] bool isLike)
        {
            var success = await _codeRoastService.RecordCodeRoastInteraction(id, isLike);
            if (!success)
            {
                return NotFound("Code roast not found.");
            }
            return NoContent();
        }

        [HttpGet]
        [AllowAnonymous] 
        [ProducesResponseType(typeof(IEnumerable<CodeSnippet>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllRoasts()
        {
            var roasts = await _codeRoastService.GetAllRoastsAsync();
            return Ok(roasts);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] 
        [ProducesResponseType(typeof(CodeSnippet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoastById(Guid id)
        {
            var roast = await _codeRoastService.GetRoastByIdAsync(id);
            if (roast == null)
            {
                return NotFound("Code roast not found.");
            }
            return Ok(roast);
        }
    }
}