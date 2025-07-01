// DevLife.Api/Services/BugChaseService.cs
using DevLife.Api.Models;
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevLife.Infrastructure.Mappers;
using System.Text.Json; // For parsing test cases

namespace DevLife.Api.Services
{
    public class BugChaseService
    {
        private readonly ICodeChallengeRepository _challengeRepository;
        private readonly UserService _userService;
        private readonly IScoreRepository _scoreRepository;
        private readonly IMongoRepository<GameSessionEntity> _gameSessionRepository;

        public BugChaseService(ICodeChallengeRepository challengeRepository, UserService userService, IScoreRepository scoreRepository, IMongoRepository<GameSessionEntity> gameSessionRepository)
        {
            _challengeRepository = challengeRepository;
            _userService = userService;
            _scoreRepository = scoreRepository;
            _gameSessionRepository = gameSessionRepository;
        }

        public async Task<CodeChallenge> GetRandomBugChallenge(string difficulty = null)
        {
            var challengeEntity = await _challengeRepository.GetRandomChallengeAsync(difficulty);
            return EntityToDtoMapper.MapCodeChallengeDto(challengeEntity);
        }

        public async Task<BugFixResult> SubmitBugFix(Guid userId, SubmitBugFixDto submitDto)
        {
            var challenge = await _challengeRepository.GetChallengeByIdAsync(submitDto.ChallengeId);
            if (challenge == null)
            {
                return new BugFixResult { IsCorrect = false, Message = "Challenge not found.", PointsGained = 0 };
            }

            // 1. Simulate running tests against the submitted code
            // In a real app, this would involve sandboxed code execution and test runner
            bool isCorrect = SimulateCodeExecution(submitDto.FixedCode, challenge.TestCasesJson);

            string message = "";
            int pointsGained = 0;

            if (isCorrect)
            {
                pointsGained = 50 + (challenge.Difficulty == "Hard" ? 50 : 0); // Award more for harder challenges
                await _userService.AddReputationPointsAsync(userId, pointsGained);
                message = "Congratulations! You squashed the bug!";
            }
            else
            {
                pointsGained = -10; // Small deduction for incorrect attempt
                await _userService.DeductReputationPointsAsync(userId, Math.Abs(pointsGained));
                message = "Oops! The bug persists. Keep trying!";
            }

            // 2. Record score (PostgreSQL)
            await _scoreRepository.AddScoreAsync(new ScoreEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                GameType = "BugChase",
                Points = pointsGained,
                AchievedAt = DateTime.UtcNow,
                RelatedEntityId = challenge.Id
            });

            // 3. Log game session data (MongoDB)
            var gameSession = new GameSessionEntity
            {
                UserId = userId,
                GameType = "BugChase",
                StartTime = DateTime.UtcNow, // Or retrieve an existing session
                EndTime = DateTime.UtcNow,
                IsCompleted = true, // Or set to false if multiple attempts are allowed in one session
                Status = isCorrect ? "Solved" : "Attempted",
                ChallengeId = challenge.Id,
                ChallengeSolved = isCorrect,
                SubmittedCode = submitDto.FixedCode,
                Attempts = 1, // Increment if managing session attempts
                GameSpecificData = new Dictionary<string, object>
                {
                    { "Difficulty", challenge.Difficulty },
                    { "PointsAwarded", pointsGained }
                }
            };
            await _gameSessionRepository.AddAsync(gameSession); // Or update existing session

            return new BugFixResult { IsCorrect = isCorrect, Message = message, PointsGained = pointsGained };
        }

        // A very simplified simulation of code execution and test case checking
        private bool SimulateCodeExecution(string submittedCode, string testCasesJson)
        {
            // In a real scenario, you'd deserialize testCasesJson into a list of objects like:
            // List<TestCase> testCases = JsonSerializer.Deserialize<List<TestCase>>(testCasesJson);
            // and then run the submittedCode against each test case.

            // For demonstration, let's assume a simple check:
            // If the challenge expects a loop fix, check if "i < 5" or "i <= 4" is present
            // This is highly insecure and not how you'd do it in production!
            if (submittedCode.Contains("i < 5") || submittedCode.Contains("i <= 4") || submittedCode.Contains("fixed_bug_marker"))
            {
                return true; // Simulate successful fix
            }
            return false; // Simulate failure
        }
    }
}