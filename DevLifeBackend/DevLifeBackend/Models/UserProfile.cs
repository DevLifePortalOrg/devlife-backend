// DevLife.Api/Models/UserProfile.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace DevLife.Api.Models
{
    public class UserProfile
    {
        public Guid UserId { get; set; } // Corresponds to the User's Id
        public string Username { get; set; }
        public string Email { get; set; } // Often included for profile display
        public int ReputationPoints { get; set; }
        [StringLength(500)]
        public string Bio { get; set; }
        [StringLength(50)]
        public string GitHubUsername { get; set; }
        public DateTime? BirthDate { get; set; } // For HoroscopeService
        public string HoroscopeToday { get; set; } // To return the generated horoscope
    }

    public class UpdateUserProfileDto
    {
        [StringLength(500)]
        public string Bio { get; set; }
        [StringLength(50)]
        public string GitHubUsername { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}