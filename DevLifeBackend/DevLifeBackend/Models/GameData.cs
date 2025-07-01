// DevLife.Api/Models/GameData.cs
using System;
using System.Collections.Generic;

namespace DevLife.Api.Models
{
    public class GameData
    {
        public string Id { get; set; } // MongoDB _id
        public Guid UserId { get; set; }
        public string GameType { get; set; }
        public DateTime GameDate { get; set; }
        public decimal? BetAmount { get; set; } // Optional for non-casino games
        public decimal? WinningsLosses { get; set; } // Optional
        public bool? IsWin { get; set; } // Optional
        public Guid? ChallengeId { get; set; } // Optional
        public bool? ChallengeSolved { get; set; } // Optional
        public string CodeSubmitted { get; set; } // Optional
        // Can add more flexible data as a Dictionary<string, object> if needed
    }
}