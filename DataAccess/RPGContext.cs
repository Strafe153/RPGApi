using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class RPGContext : DbContext
    {
        public DbSet<Player> Players => Set<Player>();
        public DbSet<Character> Characters => Set<Character>();
        public DbSet<Weapon> Weapons => Set<Weapon>();
        public DbSet<Spell> Spells => Set<Spell>();
        public DbSet<Mount> Mounts => Set<Mount>();
        public DbSet<CharacterWeapon> CharacterWeapons => Set<CharacterWeapon>();
        public DbSet<CharacterSpell> CharacterSpells => Set<CharacterSpell>();
        public DbSet<CharacterMount> CharacterMounts => Set<CharacterMount>();

        public RPGContext(DbContextOptions<RPGContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
