// DevLife.Domain/Interfaces/ICodeChallengeRepository.cs
using DevLife.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevLife.Domain.Interfaces
{
    public interface ICodeChallengeRepository
    {
        Task<CodeChallengeEntity> GetChallengeByIdAsync(Guid id);
        Task<IEnumerable<CodeChallengeEntity>> GetAllChallengesAsync();
        Task<CodeChallengeEntity> GetRandomChallengeAsync(string difficulty = null);
        Task AddChallengeAsync(CodeChallengeEntity challenge);
        Task UpdateChallengeAsync(CodeChallengeEntity challenge);
        Task DeleteChallengeAsync(Guid id);
    }
}