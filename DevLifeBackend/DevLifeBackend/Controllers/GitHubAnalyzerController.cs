// DevLife.Api/Controllers/GitHubAnalyzerController.cs
using DevLife.Api.Models;
using DevLife.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevLife.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Example: Analyze requires authentication
    public class GitHubAnalyzerController : ControllerBase
    {
        private readonly GitHubAnalyzerService _gitHubAnalyzerService;

        public GitHubAnalyzerController(GitHubAnalyzerService gitHubAnalyzerService)
        {
            _gitHubAnalyzerService = gitHubAnalyzerService;
        }

        /// <summary>
        /// Analyzes a public GitHub repository.
        /// </summary>
        /// <param name="request">Username and repository name.</param>
        /// <returns>GitHubRepoData DTO with analysis results.</returns>
        [HttpGet("analyze")]
        [ProducesResponseType(typeof(GitHubRepoData), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AnalyzeRepository([FromQuery] AnalyzeGitHubRepoRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.RepositoryName))
            {
                return BadRequest("Username and RepositoryName are required.");
            }

            try
            {
                var repoData = await _gitHubAnalyzerService.GetRepositoryAnalysis(request.Username, request.RepositoryName);
                if (repoData == null)
                {
                    return NotFound($"Repository '{request.RepositoryName}' by '{request.Username}' not found or could not be analyzed.");
                }
                return Ok(repoData);
            }
            catch (ApplicationException ex) // Catch specific service-level exceptions
            {
                return BadRequest(ex.Message); // Return a meaningful error
            }
        }
    }
}