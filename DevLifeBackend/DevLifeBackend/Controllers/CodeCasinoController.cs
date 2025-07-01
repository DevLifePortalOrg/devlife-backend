using Microsoft.AspNetCore.Mvc;
using DevLife.Api.Models;
using DevLife.Api.Services;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DevLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CodeCasinoController : ControllerBase
    {
        private readonly CodeCasinoService _codeCasinoService;

        public CodeCasinoController(CodeCasinoService codeCasinoService)
        {
            _codeCasinoService = codeCasinoService;
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

        [HttpPost("play")]
        [ProducesResponseType(typeof(GameResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GameResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PlayCasino([FromBody] CodeCasinoPlayRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserIdFromClaims();
            var result = await _codeCasinoService.PlayCodeCasino(userId, request);

            if (result.Outcome == "Fail") 
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}