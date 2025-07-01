// DevLife.Domain/Entities/ScoreEntity.cs
using System;

namespace DevLife.Domain.Entities
{
    public class ScoreEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // Foreign key to User
        public UserEntity User { get; set; } // Navigation property
        public string GameType { get; set; } // "CodeCasino", "BugChase", etc.
        public int Points { get; set; } // Score or points gained/lost
        public DateTime AchievedAt { get; set; }
        public Guid? RelatedEntityId { get; set; } // Optional: Link to a specific game session or challenge
    }
}