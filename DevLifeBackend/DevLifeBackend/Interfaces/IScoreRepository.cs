// DevLife.Domain/Interfaces/IScoreRepository.cs
using DevLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevLife.Domain.Interfaces
{
    public interface IScoreRepository
    {
        Task<ScoreEntity> GetScoreByIdAsync(Guid id);
        Task<IEnumerable<ScoreEntity>> GetScoresByUserIdAsync(Guid userId);
        Task<IEnumerable<ScoreEntity>> GetTopScoresByGameTypeAsync(string gameType, int count);
        Task AddScoreAsync(ScoreEntity score);
        Task UpdateScoreAsync(ScoreEntity score);
        Task DeleteScoreAsync(Guid id);
    }
}