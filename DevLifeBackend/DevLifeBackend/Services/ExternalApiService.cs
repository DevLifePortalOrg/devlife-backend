// DevLife.Api/Services/ExternalApiService.cs
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace DevLife.Api.Services
{
    public class ExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Add default headers for external APIs if needed, e.g., User-Agent for GitHub
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DevLifeApp/1.0 (devlife@example.com)");
            // Add Authorization if common for all external calls, or handle per service
        }

        public async Task<string> GetAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws HttpRequestException for 4xx or 5xx responses
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error during GET request to {url}: {ex.Message}");
                throw; // Re-throw or wrap in custom exception
            }
        }

        public async Task<HttpResponseMessage> GetWithHeadersAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error during GET request to {url}: {ex.Message}");
                throw;
            }
        }

        // Add PostAsync, PutAsync, DeleteAsync methods as needed for other API interactions
        public async Task<string> PostAsync<T>(string url, T data)
        {
            try
            {
                var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, jsonContent);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error during POST request to {url}: {ex.Message}");
                throw;
            }
        }
    }
}