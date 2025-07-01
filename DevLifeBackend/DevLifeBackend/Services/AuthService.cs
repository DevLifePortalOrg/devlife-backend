// DevLife.Api/Services/AuthService.cs
using BCrypt.Net; // For password hashing
using DevLife.Api.Models; // For AuthDto, UserProfile, etc.
using DevLife.Domain.Entities;
using DevLife.Domain.Interfaces;
using DevLife.Infrastructure.Mappers; // For mapping entities to DTOs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevLife.Api.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public AuthService(JwtService jwtService)
        {
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }
        public AuthService(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // Check if user already exists by username or email
            if (await _userRepository.UserExistsByUsernameOrEmailAsync(request.Username, request.Email))
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Username or email already registered." };
            }

            // Hash password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                ReputationPoints = 100, // Starting points for new users
                Roles = new List<string> { "User" } // Assign default role
            };

            await _userRepository.AddUserAsync(user);

            var token = _jwtService.GenerateToken(user.Id, user.Username, user.Roles);
            return AuthResponseDto.Success(token, user.Id, user.Username, user.Email, "Registration successful.");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return AuthResponseDto.Fail("Invalid credentials.");
            }

            // Update last login time
            user.LastLogin = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);

            var token = _jwtService.GenerateToken(user.Id, user.Username, user.Roles);
            return AuthResponseDto.Success(token, user.Id, user.Username, user.Email, "Login successful.");
        }
    }
}