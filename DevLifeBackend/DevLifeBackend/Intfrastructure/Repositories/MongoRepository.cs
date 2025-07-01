// DevLife.Infrastructure/Repositories/MongoRepository.cs
using DevLife.Api.Data;
using DevLife.Domain.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace DevLife.Infrastructure.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(MongoDbContext dbContext, string collectionName)
        {
            _collection = dbContext.GetCollection<T>(collectionName); // Assuming MongoDbContext has a generic GetCollection method
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", new MongoDB.Bson.ObjectId(id)); // Assuming ObjectId for _id
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            // This requires the entity to have an ID field named "_id" or annotated correctly
            // A more robust solution might use a specific filter based on the entity's ID property
            var filter = Builders<T>.Filter.Eq("_id", typeof(T).GetProperty("Id").GetValue(entity));
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", new MongoDB.Bson.ObjectId(id));
            await _collection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<T>> FilterByAsync(Expression<Func<T, bool>> filterExpression)
        {
            return await _collection.Find(filterExpression).ToListAsync();
        }
    }
}