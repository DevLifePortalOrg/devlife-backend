// DevLife.Domain/Entities/CodeSnippetEntity.cs
using System;
using System.Collections.Generic;

namespace DevLife.Domain.Entities
{
    public class CodeSnippetEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // Foreign key to User
        public UserEntity User { get; set; } // Navigation property
        public string CodeContent { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Type { get; set; } // e.g., "Roast", "Casino", "Challenge"
        public List<string> Comments { get; set; } = new List<string>(); // Simple list for roast comments
        public string Language { get; set; } // e.g., "C#", "JavaScript"
        public string Title { get; set; }
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
    }
}