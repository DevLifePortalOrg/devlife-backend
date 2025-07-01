// DevLifeBackend/Services/TokenService.cs
using System;
using System.Collections.Generic; // საჭიროა List-ისთვის
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// !!! მნიშვნელოვანი ცვლილება !!!
// გამოიყენეთ თქვენი User კლასის ზუსტი Namespace
using DevLife.Api.Models;
// თუ თქვენი User.cs ფაილი არის DevLife.Api/Models/User.cs, მაშინ ეს using სწორია.
// დარწმუნდით, რომ ეს სწორია თქვენს პროექტში.

namespace DevLife.Api.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            // Key უნდა იყოს მინიმუმ 16 სიმბოლო (128 ბიტი) ან მეტი, Base64-ში დაშიფრული.
            // ის უნდა იყოს appsettings.json-ში: "Jwt:Key"
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException("Jwt:Key is not configured in appsettings.json or is empty.");
            }
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        }

        public string CreateToken(User user) // ახლა იღებს თქვენს DevLife.Api.Models.User ობიექტს
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null when creating a token.");
            }
            if (user.Id == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty when creating a token.", nameof(user.Id));
            }
            if (string.IsNullOrEmpty(user.Username))
            {
                throw new ArgumentException("Username cannot be empty when creating a token.", nameof(user.Username));
            }
            if (string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentException("Email cannot be empty when creating a token.", nameof(user.Email));
            }


            // Claims - მომხმარებლის შესახებ ინფორმაცია, რომელიც ტოკენში იქნება
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username), // იყენებს თქვენს Username property-ს
                new Claim(ClaimTypes.Email, user.Email) // იყენებს თქვენს Email property-ს
                // შეგიძლიათ დაამატოთ სხვა claim-ებიც, მაგალითად როლები:
                // new Claim(ClaimTypes.Role, "Admin"),
            };

            // Credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7), // ტოკენის მოქმედების ვადა
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured."),
                Audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.")
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}