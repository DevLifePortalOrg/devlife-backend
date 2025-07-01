using DevLife.Api.Models; 
using DevLife.Api.Services; 
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;

namespace DevLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(AuthResponseDto.Fail("Invalid input data."));
            }

            var response = await _authService.RegisterAsync(request);
            if (!response.IsSuccess) 
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(AuthResponseDto.Fail("Invalid input data."));
            }

            var response = await _authService.LoginAsync(request);
            if (!response.IsSuccess)
            {
                return Unauthorized(response);
            }
            return Ok(response);
        }

        [Authorize] 
        [HttpGet("verify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult VerifyAuth()
        {
            return Ok(new { Message = "Authenticated successfully.", UserId = User.Identity.Name });
        }
    }
}