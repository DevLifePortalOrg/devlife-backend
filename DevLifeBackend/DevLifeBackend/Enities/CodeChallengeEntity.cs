// DevLife.Domain/Entities/CodeChallengeEntity.cs
using System;
using System.Collections.Generic;

namespace DevLife.Domain.Entities
{
    public class CodeChallengeEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ProblemDescription { get; set; }
        public string InitialCode { get; set; } // Code snippet provided for the bug chase
        public string Language { get; set; }
        public string TestCasesJson { get; set; } // JSON string of input/output pairs for validation
        public string Difficulty { get; set; } // "Easy", "Medium", "Hard"
        public DateTime CreatedDate { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}