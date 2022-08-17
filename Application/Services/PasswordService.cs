using Core.Entities;
using Core.Exceptions;
using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public PasswordService(
            IConfiguration configuration, 
            ILogger<PasswordService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public string CreateToken(Player player)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, player.Name!),
                new Claim(ClaimTypes.Role, player.Role.ToString())
            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public void VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                if (!computedHash.SequenceEqual(passwordHash))
                {
                    _logger.LogWarning("Player failed to log in due to providing an incorrect password");
                    throw new IncorrectPasswordException("Incorrect password");
                }

                _logger.LogInformation("Player successfully logged in");
            }
        }
    }
}
