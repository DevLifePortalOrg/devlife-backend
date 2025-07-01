// DevLife.Infrastructure/Repositories/MongoDatingProfileRepository.cs
using DevLife.Api.Data;
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using MongoDB.Driver;

namespace DevLife.Infrastructure.Repositories
{
    // This concrete implementation could potentially inherit from a generic MongoRepository<T>
    // for common CRUD operations, if you want to avoid code duplication.
    // For simplicity, here's a direct implementation using MongoDbContext.
    public class MongoDatingProfileRepository : IMongoRepository<DatingProfileEntity>
    {
        private readonly IMongoCollection<DatingProfileEntity> _datingProfiles;

        public MongoDatingProfileRepository(MongoDbContext dbContext)
        {
            _datingProfiles = dbContext.DatingProfiles;
        }

        public async Task<DatingProfileEntity> GetByIdAsync(string id)
        {
            // MongoDB ObjectId conversion if using ObjectId type
            return await _datingProfiles.Find(profile => profile.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DatingProfileEntity>> GetAllAsync()
        {
            return await _datingProfiles.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(DatingProfileEntity entity)
        {
            await _datingProfiles.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(DatingProfileEntity entity)
        {
            await _datingProfiles.ReplaceOneAsync(profile => profile.Id == entity.Id, entity);
        }

        public async Task DeleteAsync(string id)
        {
            await _datingProfiles.DeleteOneAsync(profile => profile.Id == id);
        }

        public async Task<IEnumerable<DatingProfileEntity>> FilterByAsync(System.Linq.Expressions.Expression<Func<DatingProfileEntity, bool>> filterExpression)
        {
            return await _datingProfiles.Find(filterExpression).ToListAsync();
        }
    }
}