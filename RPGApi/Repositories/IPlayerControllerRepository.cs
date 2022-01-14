namespace RPGApi.Repositories
{
    public interface IPlayerControllerRepository : IControllerRepository<Player>
    {
        Task<Player?> GetPlayerByNameAsync(string name);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreateToken(Player player);
    }
}
