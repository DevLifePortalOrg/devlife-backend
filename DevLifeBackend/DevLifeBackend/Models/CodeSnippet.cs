// DevLife.Api/Models/CodeSnippet.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevLife.Api.Models
{
    public class CodeSnippet
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } // Denormalized for easier display
        [Required]
        public string CodeContent { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Type { get; set; } // "Roast", "Casino", "Challenge"
        public List<string> Comments { get; set; } = new List<string>(); // For CodeRoast
        public string Language { get; set; }
        public string Title { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }

    public class SubmitCodeRoastDto
    {
        [Required]
        public string Code { get; set; }
        public string Language { get; set; } = "C#"; // Default
        public string Title { get; set; } = "Untitled Roast";
    }

    public class AddRoastCommentDto
    {
        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string CommentText { get; set; }
    }

    public class CodeCasinoPlayRequest
    {
        [Required]
        public string Code { get; set; }
        [Range(1, 1000)] // Example bet range
        public decimal Bet { get; set; }
    }

    public class GameResult
    {
        public string Outcome { get; set; } // "Win", "Lose", "Draw"
        public decimal Winnings { get; set; } // Positive for win, negative for loss
        public string Message { get; set; }
    }
}