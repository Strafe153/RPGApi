using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using RPGApi.Data;
using RPGApi.Repositories.Interfaces;

namespace RPGApi.Repositories.Implementations
{
    public class PlayerRepository : IPlayerControllerRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public PlayerRepository(DataContext context, IConfiguration configuration,
            ILogger<PlayerRepository> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public void Add(Player entity)
        {
            _context.Players!.Add(entity);
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

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public void Delete(Player entity)
        {
            _context.Players!.Remove(entity);
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players!
                .Include(p => p.Characters)
                .ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(Guid id)
        {
            return await _context.Players!
                .Include(p => p.Characters)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Player?> GetByNameAsync(string name)
        {
            return await _context.Players!.SingleOrDefaultAsync(p => p.Name == name);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Player entity)
        {
            _context.Players!.Update(entity);
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
