// DevLife.Api/Services/RedisService.cs
using Microsoft.Extensions.Logging; // For logging
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DevLife.Api.Services
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ILogger<RedisService> _logger;

        public RedisService(string connectionString, ILogger<RedisService> logger)
        {
            _logger = logger;
            try
            {
                _redis = ConnectionMultiplexer.Connect(connectionString);
                _redis.ConnectionFailed += (_, e) => _logger.LogError($"Redis connection failed: {e.Exception?.Message}");
                _redis.ConnectionRestored += (_, e) => _logger.LogInformation("Redis connection restored.");
                _db = _redis.GetDatabase();
                _logger.LogInformation("Connected to Redis successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to connect to Redis: {ex.Message}");
                throw; // Re-throw to indicate a critical startup failure
            }
        }

        public async Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                return await _db.StringSetAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting Redis key '{key}': {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetStringAsync(string key)
        {
            try
            {
                return await _db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Redis key '{key}': {ex.Message}");
                return null;
            }
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking Redis key existence for '{key}': {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteKeyAsync(string key)
        {
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting Redis key '{key}': {ex.Message}");
                return false;
            }
        }

        // Example: Store complex objects using JSON serialization
        public async Task<bool> SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(value);
            return await SetStringAsync(key, json, expiry);
        }

        public async Task<T> GetObjectAsync<T>(string key)
        {
            var json = await GetStringAsync(key);
            if (string.IsNullOrEmpty(json)) return default(T);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
}