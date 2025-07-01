// DevLife.Api/Services/GitHubAnalyzerService.cs
using DevLife.Api.Models;
using System.Text.Json;
using System.Text.Json.Serialization; // For JsonPropertyName

namespace DevLife.Api.Services
{
    public class GitHubAnalyzerService
    {
        private readonly ExternalApiService _externalApiService;

        public GitHubAnalyzerService(ExternalApiService externalApiService)
        {
            _externalApiService = externalApiService;
        }

        public async Task<GitHubRepoData> GetRepositoryAnalysis(string username, string repoName)
        {
            string repoUrl = $"https://api.github.com/repos/{username}/{repoName}";
            string commitsUrl = $"https://api.github.com/repos/{username}/{repoName}/commits?per_page=1"; // Get only the latest to count total commits

            try
            {
                // Fetch repository details
                var repoDataJson = await _externalApiService.GetAsync(repoUrl);
                var repoResponse = JsonSerializer.Deserialize<GitHubRepoApiResponse>(repoDataJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Fetch commit count (GitHub API includes 'Link' header with total pages for pagination)
                int commitCount = 0;
                try
                {
                    var commitResponse = await _externalApiService.GetWithHeadersAsync(commitsUrl); // Custom method to get headers
                    if (commitResponse.Headers.TryGetValues("Link", out var linkHeaders))
                    {
                        // Parse 'Link' header to find the last page and thus total commits
                        // Example Link: <https://api.github.com/repositories/1296269/commits?page=2>; rel="next", <https://api.github.com/repositories/1296269/commits?page=34>; rel="last"
                        foreach (var linkHeader in linkHeaders)
                        {
                            if (linkHeader.Contains("rel=\"last\""))
                            {
                                // Extract the page number from the last link
                                int lastPage = ExtractPageNumberFromLink(linkHeader, "last");
                                commitCount = lastPage * 1; // Assuming per_page=1, if per_page was 100, then lastPage * 100
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Fallback if Link header is not present (e.g., very few commits)
                        var commitsJson = await commitResponse.Content.ReadAsStringAsync();
                        var commitsArray = JsonSerializer.Deserialize<JsonElement[]>(commitsJson);
                        commitCount = commitsArray.Length; // For very small repos, this might be accurate
                    }
                }
                catch (HttpRequestException commitEx)
                {
                    // Handle specific error for commits (e.g., no commits)
                    Console.WriteLine($"Could not fetch commit count: {commitEx.Message}");
                    commitCount = 0;
                }


                return new GitHubRepoData
                {
                    RepoName = repoResponse.Name,
                    Owner = repoResponse.Owner?.Login,
                    Description = repoResponse.Description,
                    Stars = repoResponse.StargazersCount,
                    MainLanguage = repoResponse.Language,
                    CommitCount = commitCount,
                    HtmlUrl = repoResponse.HtmlUrl
                };
            }
            catch (HttpRequestException ex)
            {
                throw new ApplicationException($"Error fetching GitHub data for {username}/{repoName}: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new ApplicationException($"Error parsing GitHub API response: {ex.Message}");
            }
        }

        // Helper for parsing GitHub API Link header
        private int ExtractPageNumberFromLink(string linkHeader, string rel)
        {
            var links = linkHeader.Split(',');
            foreach (var link in links)
            {
                if (link.Contains($"rel=\"{rel}\""))
                {
                    var parts = link.Split(';');
                    if (parts.Length > 0)
                    {
                        var urlPart = parts[0].Trim().Trim('<', '>');
                        var uri = new Uri(urlPart);
                        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                        if (int.TryParse(query["page"], out int pageNumber))
                        {
                            return pageNumber;
                        }
                    }
                }
            }
            return 0; // Or throw
        }

        // Internal DTO for GitHub API response parsing
        private class GitHubRepoApiResponse
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("owner")]
            public OwnerApiResponse Owner { get; set; }
            [JsonPropertyName("description")]
            public string Description { get; set; }
            [JsonPropertyName("stargazers_count")]
            public int StargazersCount { get; set; }
            [JsonPropertyName("language")]
            public string Language { get; set; }
            [JsonPropertyName("html_url")]
            public string HtmlUrl { get; set; }
        }

        private class OwnerApiResponse
        {
            [JsonPropertyName("login")]
            public string Login { get; set; }
        }
    }
}