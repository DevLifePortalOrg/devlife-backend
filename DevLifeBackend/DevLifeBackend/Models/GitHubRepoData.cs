// DevLife.Api/Models/GitHubRepoData.cs
namespace DevLife.Api.Models
{
    public class GitHubRepoData
    {
        public string RepoName { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
        public int Stars { get; set; }
        public string MainLanguage { get; set; }
        public int CommitCount { get; set; }
        public string HtmlUrl { get; set; } // Link to the repo
        // Add more metrics as needed
    }

    public class AnalyzeGitHubRepoRequestDto
    {
        public string Username { get; set; }
        public string RepositoryName { get; set; }
    }
}