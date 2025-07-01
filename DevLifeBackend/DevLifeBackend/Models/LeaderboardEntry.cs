// DevLife.Api/Models/LeaderboardEntry.cs
using System;

namespace DevLife.Api.Models
{
    public class LeaderboardEntry
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string GameType { get; set; }
        public int Score { get; set; }
        public DateTime AchievedAt { get; set; }
    }
}