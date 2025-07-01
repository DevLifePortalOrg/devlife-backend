// DevLife.Api/Controllers/MeetingEscapeController.cs
using Microsoft.AspNetCore.Mvc;
using DevLife.Api.Models;
using DevLife.Api.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DevLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Example: maybe only authenticated users can generate excuses
    public class MeetingEscapeController : ControllerBase
    {
        private readonly MeetingEscapeService _meetingEscapeService;

        public MeetingEscapeController(MeetingEscapeService meetingEscapeService)
        {
            _meetingEscapeService = meetingEscapeService;
        }

        /// <summary>
        /// Generates a random, humorous excuse to escape a meeting.
        /// </summary>
        /// <returns>An Excuse DTO with the generated text.</returns>
        [HttpGet("generate")]
        [ProducesResponseType(typeof(Excuse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateExcuse()
        {
            var excuse = await _meetingEscapeService.GenerateRandomExcuse();
            return Ok(excuse);
        }
    }
}