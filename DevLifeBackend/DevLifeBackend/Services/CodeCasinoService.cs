// DevLife.Api/Services/CodeCasinoService.cs
using DevLife.Api.Models;
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace DevLife.Api.Services
{
    public class CodeCasinoService
    {
        private readonly Random _random;
        private readonly UserService _userService;
        private readonly IMongoRepository<GameSessionEntity> _gameSessionRepository; // Using generic MongoDB repo

        public CodeCasinoService(UserService userService, IMongoRepository<GameSessionEntity> gameSessionRepository)
        {
            _random = new Random();
            _userService = userService;
            _gameSessionRepository = gameSessionRepository;
        }

        public async Task<GameResult> PlayCodeCasino(Guid userId, CodeCasinoPlayRequest request)
        {
            // 1. Deduct bet amount from user's reputation points first
            if (!await _userService.DeductReputationPointsAsync(userId, (int)request.Bet))
            {
                return new GameResult { Outcome = "Fail", Winnings = 0, Message = "Not enough reputation points to place this bet." };
            }

            // Simulate code execution outcome
            // In a real scenario, this would involve static analysis, running tests, or AI evaluation
            // For simplicity: 45% chance to win, 50% chance to lose, 5% chance of "critical success"
            double outcomeRoll = _random.NextDouble();
            bool didWin = false;
            decimal winnings = 0;
            string message = "";
            string outcomeType = "Lose";

            if (outcomeRoll < 0.45) // 45% chance to win
            {
                didWin = true;
                winnings = request.Bet * 1.8m; // 80% profit
                outcomeType = "Win";
                message = "Your code compiled perfectly! You won!";
            }
            else if (outcomeRoll < 0.95) // 50% chance to lose (0.45 to 0.95)
            {
                didWin = false;
                winnings = -request.Bet; // Lose the bet amount
                outcomeType = "Lose";
                message = "Syntax error! You lost your bet.";
            }
            else // 5% chance of critical success (0.95 to 1.0)
            {
                didWin = true;
                winnings = request.Bet * 2.5m; // 150% profit
                outcomeType = "Critical Win";
                message = "Critical success! Your code is flawless, earning you a huge bonus!";
            }

            // 2. Add/Deduct winnings to user's reputation points
            if (didWin)
            {
                await _userService.AddReputationPointsAsync(userId, (int)winnings);
            }

            // 3. Log game session data in MongoDB
            var gameSession = new GameSessionEntity
            {
                UserId = userId,
                GameType = "CodeCasino",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow,
                IsCompleted = true,
                Status = "Finished",
                BetAmount = request.Bet,
                WinningsLosses = winnings,
                GameSpecificData = new Dictionary<string, object>
                {
                    { "CodeSubmitted", request.Code },
                    { "OutcomeType", outcomeType }
                }
            };
            await _gameSessionRepository.AddAsync(gameSession);


            return new GameResult { Outcome = outcomeType, Winnings = winnings, Message = message };
        }
    }
}