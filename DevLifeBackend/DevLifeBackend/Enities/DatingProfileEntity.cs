// DevLife.Domain/Entities/DatingProfileEntity.cs
using System;
using System.Collections.Generic;
using MongoDB.Bson; // For ObjectId
using MongoDB.Bson.Serialization.Attributes; // For BsonId

namespace DevLife.Domain.Entities
{
    public class DatingProfileEntity
    {
        [BsonId] // Specifies that this property is the primary key for MongoDB
        [BsonRepresentation(BsonType.ObjectId)] // Maps Guid to MongoDB's ObjectId
        public string Id { get; set; } // MongoDB usually uses string or ObjectId for Id

        public Guid UserId { get; set; } // Reference to the UserEntity in PostgreSQL (if not storing user details here)
        // No direct navigation property here if UserEntity is in a different DB context.

        public string Bio { get; set; }
        public string LookingFor { get; set; } // e.g., "Pair Programmer", "Mentor", "Coffee Break Buddy"
        public List<string> Interests { get; set; } = new List<string>(); // e.g., "AI", "Frontend", "Clean Code"
        public string ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int MatchesCount { get; set; } = 0;
        public List<Guid> LikedUserIds { get; set; } = new List<Guid>(); // IDs of users this profile liked
        public List<Guid> MatchedUserIds { get; set; } = new List<Guid>(); // IDs of users this profile matched with
    }
}