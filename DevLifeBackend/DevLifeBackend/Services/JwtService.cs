// DevLife.Api/Services/JwtService.cs
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration; // To read from appsettings

namespace DevLife.Api.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpiresInMinutes;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key is not configured.");
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? "DevLife";
            _jwtAudience = _configuration["Jwt:Audience"] ?? "DevLifeUsers";
            _jwtExpiresInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiresInMinutes"] ?? "60");

            if (_jwtKey.Length < 32) // JWT key should be sufficiently long for HS256 (256 bits = 32 bytes)
            {
                throw new ArgumentException("JWT Key must be at least 32 characters long for HMACSHA256.");
            }
        }

        public string GenerateToken(Guid userId, string username, IList<string> roles)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // For retrieving user ID easily
                new Claim(ClaimTypes.Name, username)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtExpiresInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Optional: Add methods for token validation if needed outside of ASP.NET Core middleware
        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No leeway in expiration date
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception)
            {
                return null; // Token validation failed
            }
        }
    }
}