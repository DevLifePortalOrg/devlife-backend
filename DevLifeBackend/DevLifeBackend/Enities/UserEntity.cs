// DevLife.Domain/Entities/UserEntity.cs
namespace DevLife.Domain.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int ReputationPoints { get; set; } = 0;
        public string Bio { get; set; }
        public string GitHubUsername { get; set; }
        public DateTime? BirthDate { get; set; } // For HoroscopeService
        public List<string> Roles { get; set; } = new List<string>(); // e.g., "User", "Admin"

        // Navigation property for related data, if using EF Core relationships
        public ICollection<CodeSnippetEntity> CodeSnippets { get; set; }
        public ICollection<ScoreEntity> Scores { get; set; }
        // If DatingProfile is in PostgreSQL and 1-to-1
        // public DatingProfileEntity DatingProfile { get; set; }
    }
}