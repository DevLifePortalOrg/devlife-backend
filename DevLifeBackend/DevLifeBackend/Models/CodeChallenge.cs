// DevLife.Api/Models/CodeChallenge.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DevLife.Api.Models
{
    public class CodeChallenge
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ProblemDescription { get; set; }
        public string InitialCode { get; set; }
        public string Language { get; set; }
        public string TestCasesJson { get; set; } // JSON string, client can parse
        public string Difficulty { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> Tags { get; set; }
    }

    public class SubmitBugFixDto
    {
        [Required]
        public Guid ChallengeId { get; set; }
        [Required]
        public string FixedCode { get; set; }
    }

    public class BugFixResult
    {
        public bool IsCorrect { get; set; }
        public string Message { get; set; }
        public int PointsGained { get; set; }
    }
}