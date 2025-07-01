// DevLife.Domain/Entities/GameSessionEntity.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DevLife.Domain.Entities
{
    public class GameSessionEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public Guid UserId { get; set; }
        public string GameType { get; set; } // e.g., "CodeCasino", "BugChase"
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string Status { get; set; } // e.g., "InProgress", "Finished", "Aborted"
        public Dictionary<string, object> GameSpecificData { get; set; } // Flexible storage for various game data

        // For CodeCasino
        public decimal? BetAmount { get; set; }
        public decimal? WinningsLosses { get; set; }

        // For BugChase
        public Guid? ChallengeId { get; set; }
        public bool? ChallengeSolved { get; set; }
        public string SubmittedCode { get; set; }
        public int Attempts { get; set; } = 0;
    }
}