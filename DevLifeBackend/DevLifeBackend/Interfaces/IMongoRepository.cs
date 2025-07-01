// DevLife.Domain/Interfaces/IMongoRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevLife.Domain.Interfaces
{
    // Generic interface for MongoDB operations, T is the entity type with string Id
    public interface IMongoRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
        Task<IEnumerable<T>> FilterByAsync(System.Linq.Expressions.Expression<Func<T, bool>> filterExpression);
    }
}