// DevLife.Infrastructure/Repositories/CodeChallengeRepository.cs
using DevLife.Api.Data;
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevLife.Infrastructure.Repositories
{
    public class CodeChallengeRepository : ICodeChallengeRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly Random _random = new Random();

        public CodeChallengeRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CodeChallengeEntity> GetChallengeByIdAsync(Guid id)
        {
            return await _dbContext.CodeChallenges.FindAsync(id);
        }

        public async Task<IEnumerable<CodeChallengeEntity>> GetAllChallengesAsync()
        {
            return await _dbContext.CodeChallenges.ToListAsync();
        }

        public async Task<CodeChallengeEntity> GetRandomChallengeAsync(string difficulty = null)
        {
            IQueryable<CodeChallengeEntity> query = _dbContext.CodeChallenges;

            if (!string.IsNullOrWhiteSpace(difficulty))
            {
                query = query.Where(c => c.Difficulty == difficulty);
            }

            var challenges = await query.ToListAsync();
            if (!challenges.Any())
            {
                return null;
            }

            return challenges[_random.Next(challenges.Count)];
        }

        public async Task AddChallengeAsync(CodeChallengeEntity challenge)
        {
            await _dbContext.CodeChallenges.AddAsync(challenge);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateChallengeAsync(CodeChallengeEntity challenge)
        {
            _dbContext.CodeChallenges.Update(challenge);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteChallengeAsync(Guid id)
        {
            var challenge = await _dbContext.CodeChallenges.FindAsync(id);
            if (challenge != null)
            {
                _dbContext.CodeChallenges.Remove(challenge);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}