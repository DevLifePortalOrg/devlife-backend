// DevLife.Api/Models/DatingProfile.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevLife.Api.Models
{
    public class DatingProfile
    {
        public Guid UserId { get; set; } // Linked to the User's Id
        [StringLength(500)]
        public string Bio { get; set; }
        [Required]
        public string LookingFor { get; set; } // e.g., "Pair Programmer", "Mentor", "Coffee Break Buddy"
        public List<string> Interests { get; set; } = new List<string>(); // e.g., "AI", "Frontend", "Clean Code"
        public string ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int MatchesCount { get; set; } // Number of matches this profile has
        public List<Guid> LikedUserIds { get; set; } = new List<Guid>(); // For internal use, might not expose directly
        public List<Guid> MatchedUserIds { get; set; } = new List<Guid>(); // For internal use, might not expose directly
    }

    public class CreateUpdateDatingProfileDto
    {
        [StringLength(500)]
        public string Bio { get; set; }
        [Required]
        public string LookingFor { get; set; }
        public List<string> Interests { get; set; } = new List<string>();
        public string ProfilePictureUrl { get; set; } // For uploading or referencing
    }

    public class DatingMatchDto
    {
        public Guid MatchedUserId { get; set; }
        public string MatchedUsername { get; set; }
        public string MatchedUserBio { get; set; }
        public List<string> CommonInterests { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}