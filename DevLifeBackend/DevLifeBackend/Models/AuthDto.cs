// DevLife.Api/Models/AuthDto.cs
using System.ComponentModel.DataAnnotations; // For data annotations
using System;

namespace DevLife.Api.Models
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        // Optional: ConfirmPassword for client-side validation
        // public string ConfirmPassword { get; set; }
    }

    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; } // Can also be Email
        [Required]
        public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        // შეცვლილია Success-დან IsSuccess-ზე
        public bool IsSuccess { get; set; }
        // -------------------------------------------------------------

        public string Message { get; set; }
        public string Token { get; set; }
        public Guid? UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        // Static helper methods
        public static AuthResponseDto Success(string token, Guid? userId, string username, string email, string message = "Operation successful.")
        {
            return new AuthResponseDto
            {
                IsSuccess = true, // განახლებულია, რომ გამოიყენოს IsSuccess
                Token = token,
                UserId = userId,
                Username = username,
                Email = email,
                Message = message
            };
        }

        public static AuthResponseDto Fail(string message = "Operation failed.")
        {
            return new AuthResponseDto
            {
                IsSuccess = false, // განახლებულია, რომ გამოიყენოს IsSuccess
                Token = null,
                UserId = null,
                Username = null,
                Email = null,
                Message = message
            };
        }
    }
}