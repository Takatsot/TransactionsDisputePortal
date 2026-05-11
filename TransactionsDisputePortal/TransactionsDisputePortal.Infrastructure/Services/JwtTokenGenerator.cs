using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Duende.IdentityModel;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TransactionsDisputePortal.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace TransactionsDisputePortal.Infrastructure.Services
{
    /// <summary>
    /// Service for generating JWT tokens
    /// </summary>
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GenerateToken(Guid userId, string email, string firstName, string lastName)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not configured")));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtClaimTypes.Subject, userId.ToString()),
                new Claim(JwtClaimTypes.Email, email),
                new Claim(JwtClaimTypes.GivenName, firstName),
                new Claim(JwtClaimTypes.FamilyName, lastName),
                new Claim(JwtClaimTypes.Name, $"{firstName} {lastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
