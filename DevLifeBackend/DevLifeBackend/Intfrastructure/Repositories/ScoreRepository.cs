// DevLife.Infrastructure/Repositories/ScoreRepository.cs
using DevLife.Api.Data;
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Infrastructure.Repositories
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly AppDbContext _dbContext;

        public ScoreRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ScoreEntity> GetScoreByIdAsync(Guid id)
        {
            return await _dbContext.Scores.FindAsync(id);
        }

        public async Task<IEnumerable<ScoreEntity>> GetScoresByUserIdAsync(Guid userId)
        {
            return await _dbContext.Scores.Where(s => s.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<ScoreEntity>> GetTopScoresByGameTypeAsync(string gameType, int count)
        {
            return await _dbContext.Scores
                .Where(s => s.GameType == gameType)
                .OrderByDescending(s => s.Points)
                .Take(count)
                .ToListAsync();
        }

        public async Task AddScoreAsync(ScoreEntity score)
        {
            await _dbContext.Scores.AddAsync(score);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateScoreAsync(ScoreEntity score)
        {
            _dbContext.Scores.Update(score);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteScoreAsync(Guid id)
        {
            var score = await _dbContext.Scores.FindAsync(id);
            if (score != null)
            {
                _dbContext.Scores.Remove(score);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}