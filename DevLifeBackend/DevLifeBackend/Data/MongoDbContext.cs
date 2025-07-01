// DevLife.Api/Data/MongoDbContext.cs
using MongoDB.Driver;
using DevLife.Domain.Entities;
using Microsoft.Extensions.Configuration; // To read from appsettings
using System;

namespace DevLife.Api.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("MongoDB connection string is not configured.");
            }

            var client = new MongoClient(connectionString);
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException("MongoDB connection string must specify a database name.");
            }
            _database = client.GetDatabase(databaseName);
        }

        // MongoDB Collections
        public IMongoCollection<DatingProfileEntity> DatingProfiles =>
            _database.GetCollection<DatingProfileEntity>("DatingProfiles");

        public IMongoCollection<GameSessionEntity> GameSessions =>
            _database.GetCollection<GameSessionEntity>("GameSessions");

        // Generic method to get a collection, useful for generic repositories
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}