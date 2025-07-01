// DevLife.Api/Models/User.cs
using System;
using System.Collections.Generic;

namespace DevLife.Api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int ReputationPoints { get; set; }
        // Note: PasswordHash or sensitive info should NOT be here.
    }
}