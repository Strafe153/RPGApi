using Core.Enums;
using Core.Interfaces.Services;
using FluentMigrator;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccess.Migrations;

[Migration(28291238748223)]
public class InitialMigration : Migration
{
    private readonly IConfiguration _configuration;
    private readonly IPasswordService _passwordService;

    public InitialMigration(
        IConfiguration configuration, 
        IPasswordService passwordService)
    {
        _configuration = configuration;
        _passwordService = passwordService;
    }

    public override void Up()
    {
        // Players table
        Create.Table("Players")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(30).NotNullable()
            .WithColumn("Role").AsInt32().NotNullable().WithDefaultValue((int)PlayerRole.Player)
            .WithColumn("PasswordHash").AsBinary()
            .WithColumn("PasswordSalt").AsBinary();

        Create.Index("IX_Players_Name")
            .OnTable("Players")
            .OnColumn("Name")
            .Ascending()
            .WithOptions()
            .NonClustered();

        (byte[] hash, byte[] salt) = _passwordService.CreatePasswordHash(_configuration.GetSection("AdminSettings:AdminPassword").Value);

        Insert.IntoTable("Players")
            .Row(new
            {
                Name = "Admin",
                Role = (int)PlayerRole.Admin,
                PasswordHash = hash,
                PasswordSalt = salt
            });

        // Characters table
        Create.Table("Characters")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(30).NotNullable()
            .WithColumn("Race").AsInt32().NotNullable().WithDefaultValue((int)CharacterRace.Human)
            .WithColumn("Health").AsInt32().NotNullable().WithDefaultValue(100)
            .WithColumn("PlayerId").AsInt32().NotNullable();

        Create.ForeignKey("FK_Characters_Players_PlayerId")
            .FromTable("Characters")
            .ForeignColumn("PlayerId")
            .ToTable("Players")
            .PrimaryColumn("Id")
            .OnUpdate(Rule.None)
            .OnDelete(Rule.Cascade);

        Create.Index("IX_Characters_PlayerId")
            .OnTable("Characters")
            .OnColumn("PlayerId")
            .Ascending()
            .WithOptions()
            .NonClustered();

        // Weapons table
        Create.Table("Weapons")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(30).NotNullable()
            .WithColumn("Type").AsInt32().NotNullable().WithDefaultValue((int)WeaponType.Sword)
            .WithColumn("Damage").AsInt32().NotNullable().WithDefaultValue(30);

        // Spells table
        Create.Table("Spells")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(30).NotNullable()
            .WithColumn("Type").AsInt32().NotNullable().WithDefaultValue((int)SpellType.Fire)
            .WithColumn("Damage").AsInt32().NotNullable().WithDefaultValue(15);

        // Mounts table
        Create.Table("Mounts")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(30).NotNullable()
            .WithColumn("Type").AsInt32().NotNullable().WithDefaultValue((int)MountType.Horse)
            .WithColumn("Speed").AsInt32().NotNullable().WithDefaultValue(10);

        // CharacterWeapons table
        Create.Table("CharacterWeapons")
            .WithColumn("CharacterId").AsInt32().NotNullable()
            .WithColumn("WeaponId").AsInt32().NotNullable();

        Create.PrimaryKey("PK_CharacterWeapons")
            .OnTable("CharacterWeapons")
            .Columns("CharacterId", "WeaponId");

        Create.ForeignKey("FK_CharacterWeapons_Characters_CharacterId")
            .FromTable("CharacterWeapons")
            .ForeignColumn("CharacterId")
            .ToTable("Characters")
            .PrimaryColumn("Id")
            .OnUpdate(Rule.None)
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_CharacterWeapons_Weapons_WeaponId")
            .FromTable("CharacterWeapons")
            .ForeignColumn("WeaponId")
            .ToTable("Weapons")
            .PrimaryColumn("Id")
            .OnUpdate(Rule.None)
            .OnDelete(Rule.Cascade);

        Create.Index("IX_CharacterWeapons_WeaponId")
            .OnTable("CharacterWeapons")
            .OnColumn("WeaponId")
            .Ascending()
            .WithOptions()
            .NonClustered();

        // CharacterSpells table
        Create.Table("CharacterSpells")
            .WithColumn("CharacterId").AsInt32().NotNullable()
            .WithColumn("SpellId").AsInt32().NotNullable();

        Create.PrimaryKey("PK_CharacterSpells")
            .OnTable("CharacterSpells")
            .Columns("CharacterId", "SpellId");

        Create.ForeignKey("FK_CharacterWeapons_Characters_CharacterId")
            .FromTable("CharacterSpells")
            .ForeignColumn("CharacterId")
            .ToTable("Characters")
            .PrimaryColumn("Id")
            .OnUpdate(Rule.None)
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_CharacterSpells_Spells_SpellId")
            .FromTable("CharacterSpells")
            .ForeignColumn("SpellId")
            .ToTable("Spells")
            .PrimaryColumn("Id")
            .OnUpdate(Rule.None)
            .OnDelete(Rule.Cascade);

        Create.Index("IX_CharacterSpells_SpellId")
            .OnTable("CharacterSpells")
            .OnColumn("SpellId")
            .Ascending()
            .WithOptions()
            .NonClustered();

        // CharacterMounts table
        Create.Table("CharacterMounts")
            .WithColumn("CharacterId").AsInt32().NotNullable()
            .WithColumn("MountId").AsInt32().NotNullable();

        Create.PrimaryKey("PK_CharacterMounts")
            .OnTable("CharacterMounts")
            .Columns("CharacterId", "MountId");

        Create.ForeignKey("FK_CharacterMounts_Characters_CharacterId")
            .FromTable("CharacterMounts")
            .ForeignColumn("CharacterId")
            .ToTable("Characters")
            .PrimaryColumn("Id")
            .OnUpdate(Rule.None)
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_CharacterMounts_Mounts_MountId")
            .FromTable("CharacterMounts")
            .ForeignColumn("MountId")
            .ToTable("Mounts")
            .PrimaryColumn("Id")
            .OnUpdate(Rule.None)
            .OnDelete(Rule.Cascade);

        Create.Index("IX_CharacterMounts_MountId")
            .OnTable("CharacterMounts")
            .OnColumn("MountId")
            .Ascending()
            .WithOptions()
            .NonClustered();
    }

    public override void Down()
    {
    }
}
