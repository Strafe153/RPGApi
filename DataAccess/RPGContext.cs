using Core.Constants;
using Core.Enums;
using Core.Interfaces.Services;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace DataAccess;

public class RPGContext
{
    private readonly IConfiguration _configuration;
    private readonly IPasswordService _passwordService;
    private readonly string _databaseConnection;
    private readonly string _globalDatabaseConnection;

    private const string DATABASE_NAME = "rpg_api_db";

    public RPGContext(
        IConfiguration configuration,
        IPasswordService passwordService)
    {
        _configuration = configuration;
        _passwordService = passwordService;
        _databaseConnection = _configuration.GetConnectionString(ConnectionStringConstants.DatabaseConnection);
        _globalDatabaseConnection = _configuration.GetConnectionString(ConnectionStringConstants.GlobalPosgresConnection);

        EnsureDatabase();
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(_databaseConnection);

    private void EnsureDatabase()
    {
        var query = @"SELECT datname
                      FROM pg_catalog.pg_database
                      WHERE datname = @DBName";
        var queryParams = new { DBName = DATABASE_NAME };

        using var connection = new NpgsqlConnection(_globalDatabaseConnection);
        connection.Open();

        var queryResult = connection.QueryFirstOrDefault<string>(query, queryParams);

        if (string.IsNullOrEmpty(queryResult))
        {
            using var transaction = connection.BeginTransaction();

            InitializeDatabase();
            InitializePlayers(transaction: transaction);
            InitializeCharacters(transaction: transaction);
            InitializeWeapons(transaction: transaction);
            InitializeCharacterWeapons(transaction: transaction);
            InitializeSpells(transaction: transaction);
            InitializeCharacterSpells(transaction: transaction);
            InitializeMounts(transaction: transaction);
            InitializeCharacterMounts(transaction: transaction);

            transaction.Commit();
        }
    }

    private void InitializeDatabase()
    {
        using var connection = new NpgsqlConnection(_globalDatabaseConnection);
        var query = $"CREATE DATABASE {DATABASE_NAME}";

        connection.Execute(query);
    }

    private void InitializePlayers(IDbTransaction transaction)
    {
        var (hash, salt) = _passwordService.GeneratePasswordHashAndSalt(_configuration.GetSection(AdminConstants.Password).Value);
        var tableQuery = @"CREATE TABLE IF NOT EXISTS public.""Players""
                           (
                               ""Id"" serial NOT NULL,
                               ""Name"" character varying(30) COLLATE pg_catalog.""default"" NOT NULL,
                               ""Role"" integer NOT NULL DEFAULT 1,
                               ""PasswordHash"" bytea NOT NULL,
                               ""PasswordSalt"" bytea NOT NULL,
                               ""RefreshToken"" character varying(128) COLLATE pg_catalog.""default"",
                               ""RefreshTokenExpiryDate"" timestamp without time zone,
                               CONSTRAINT ""PK_Players"" PRIMARY KEY (""Id"")
                           )";
        var indexQuery = @"CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Players_Name""
                           ON public.""Players"" USING btree
                           (""Name"" COLLATE pg_catalog.""default"" ASC NULLS LAST)
                           TABLESPACE pg_default;";
        var adminQuery = @"INSERT INTO ""Players"" (""Name"", ""Role"", ""PasswordHash"", ""PasswordSalt"")
                           VALUES (@Name, @Role, @PasswordHash, @PasswordSalt)";
        var adminQueryParams = new
        {
            Name = _configuration.GetSection(AdminConstants.Username).Value,
            Role = PlayerRole.Admin,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        using var connection = CreateConnection();
        connection.Execute(tableQuery, transaction: transaction);
        connection.Execute(indexQuery, transaction: transaction);
        connection.Execute(adminQuery, adminQueryParams, transaction: transaction);
    }

    private void InitializeCharacters(IDbTransaction transaction)
    {
        var tableQuery = @"CREATE TABLE IF NOT EXISTS public.""Characters""
                           (
                               ""Id"" serial NOT NULL,
                               ""Name"" character varying(30) COLLATE pg_catalog.""default"" NOT NULL,
                               ""Race"" integer NOT NULL DEFAULT 0,
                               ""Health"" integer NOT NULL DEFAULT 100,
                               ""PlayerId"" integer NOT NULL,
                               CONSTRAINT ""PK_Characters"" PRIMARY KEY (""Id""),
                               CONSTRAINT ""FK_Characters_Players_PlayerId"" FOREIGN KEY (""PlayerId"")
                                   REFERENCES public.""Players"" (""Id"") MATCH SIMPLE
                                   ON UPDATE NO ACTION
                                   ON DELETE CASCADE
                           )";
        var indexQuery = @"CREATE INDEX IF NOT EXISTS ""IX_Characters_PlayerId""
                           ON public.""Characters"" USING btree
                           (""PlayerId"" ASC NULLS LAST)
                           TABLESPACE pg_default;";

        using var connection = CreateConnection();
        connection.Execute(tableQuery, transaction: transaction);
        connection.Execute(indexQuery, transaction: transaction);
    }

    private void InitializeWeapons(IDbTransaction transaction)
    {
        var tableQuery = @"CREATE TABLE IF NOT EXISTS public.""Weapons""
                           (
                               ""Id"" serial NOT NULL,
                               ""Name"" character varying(30) COLLATE pg_catalog.""default"" NOT NULL,
                               ""Type"" integer NOT NULL DEFAULT 0,
                               ""Damage"" integer NOT NULL DEFAULT 30,
                               CONSTRAINT ""PK_Weapons"" PRIMARY KEY (""Id"")
                           )";

        using var connection = CreateConnection();
        connection.Execute(tableQuery, transaction: transaction);
    }

    private void InitializeCharacterWeapons(IDbTransaction transaction)
    {
        var tableQuery = @"CREATE TABLE IF NOT EXISTS public.""CharacterWeapons""
                           (
                               ""CharacterId"" integer NOT NULL,
                               ""WeaponId"" integer NOT NULL,
                               CONSTRAINT ""PK_CharacterWeapons"" PRIMARY KEY (""CharacterId"", ""WeaponId""),
                               CONSTRAINT ""FK_CharacterWeapons_Characters_CharacterId"" FOREIGN KEY (""CharacterId"")
                                   REFERENCES public.""Characters"" (""Id"") MATCH SIMPLE
                                   ON UPDATE NO ACTION
                                   ON DELETE CASCADE,
                               CONSTRAINT ""FK_CharacterWeapons_Weapons_WeaponId"" FOREIGN KEY (""WeaponId"")
                                   REFERENCES public.""Weapons"" (""Id"") MATCH SIMPLE
                                   ON UPDATE NO ACTION
                                   ON DELETE CASCADE
                           )";
        var indexQuery = @"CREATE INDEX IF NOT EXISTS ""IX_CharacterWeapons_WeaponId""
                           ON public.""CharacterWeapons"" USING btree
                           (""WeaponId"" ASC NULLS LAST)
                           TABLESPACE pg_default;";

        using var connection = CreateConnection();
        connection.Execute(tableQuery, transaction: transaction);
        connection.Execute(indexQuery, transaction: transaction);
    }

    private void InitializeSpells(IDbTransaction transaction)
    {
        var tableQuery = @"CREATE TABLE IF NOT EXISTS public.""Spells""
                           (
                               ""Id"" serial NOT NULL,
                               ""Name"" character varying(30) COLLATE pg_catalog.""default"" NOT NULL,
                               ""Type"" integer NOT NULL DEFAULT 0,
                               ""Damage"" integer NOT NULL DEFAULT 15,
                               CONSTRAINT ""PK_Spells"" PRIMARY KEY (""Id"")
                           )";

        using var connection = CreateConnection();
        connection.Execute(tableQuery, transaction: transaction);
    }

    private void InitializeCharacterSpells(IDbTransaction transaction)
    {
        var tableQuery = @"CREATE TABLE IF NOT EXISTS public.""CharacterSpells""
                           (
                               ""CharacterId"" integer NOT NULL,
                               ""SpellId"" integer NOT NULL,
                               CONSTRAINT ""PK_CharacterSpells"" PRIMARY KEY (""CharacterId"", ""SpellId""),
                               CONSTRAINT ""FK_CharacterSpells_Spells_SpellId"" FOREIGN KEY (""SpellId"")
                                   REFERENCES public.""Spells"" (""Id"") MATCH SIMPLE
                                   ON UPDATE NO ACTION
                                   ON DELETE CASCADE,
                               CONSTRAINT ""FK_CharacterWeapons_Characters_CharacterId"" FOREIGN KEY (""CharacterId"")
                                   REFERENCES public.""Characters"" (""Id"") MATCH SIMPLE
                                   ON UPDATE NO ACTION
                                   ON DELETE CASCADE
                           )";
        var indexQuery = @"CREATE INDEX IF NOT EXISTS ""IX_CharacterSpells_SpellId""
                           ON public.""CharacterSpells"" USING btree
                           (""SpellId"" ASC NULLS LAST)
                           TABLESPACE pg_default;";

        using var connection = CreateConnection();
        connection.Execute(tableQuery, transaction: transaction);
        connection.Execute(indexQuery, transaction: transaction);
    }

    private void InitializeMounts(IDbTransaction transaction)
    {
        var tableQuery = @"CREATE TABLE IF NOT EXISTS public.""Mounts""
                           (
                               ""Id"" serial NOT NULL,
                               ""Name"" character varying(30) COLLATE pg_catalog.""default"" NOT NULL,
                               ""Type"" integer NOT NULL DEFAULT 0,
                               ""Speed"" integer NOT NULL DEFAULT 10,
                               CONSTRAINT ""PK_Mounts"" PRIMARY KEY (""Id"")
                           )";

        using var connection = CreateConnection();
        connection.Execute(tableQuery, transaction: transaction);
    }

    private void InitializeCharacterMounts(IDbTransaction transaction)
    {
        var tableQuery = @"CREATE TABLE IF NOT EXISTS public.""CharacterMounts""
                           (
                               ""CharacterId"" integer NOT NULL,
                               ""MountId"" integer NOT NULL,
                               CONSTRAINT ""PK_CharacterMounts"" PRIMARY KEY (""CharacterId"", ""MountId""),
                               CONSTRAINT ""FK_CharacterMounts_Characters_CharacterId"" FOREIGN KEY (""CharacterId"")
                                   REFERENCES public.""Characters"" (""Id"") MATCH SIMPLE
                                   ON UPDATE NO ACTION
                                   ON DELETE CASCADE,
                               CONSTRAINT ""FK_CharacterMounts_Mounts_MountId"" FOREIGN KEY (""MountId"")
                                   REFERENCES public.""Mounts"" (""Id"") MATCH SIMPLE
                                   ON UPDATE NO ACTION
                                   ON DELETE CASCADE
                           )";
        var indexQuery = @"CREATE INDEX IF NOT EXISTS ""IX_CharacterMounts_MountId""
                           ON public.""CharacterMounts"" USING btree
                           (""MountId"" ASC NULLS LAST)
                           TABLESPACE pg_default;";

        using var connection = CreateConnection();
        connection.Execute(tableQuery, transaction: transaction);
        connection.Execute(indexQuery, transaction: transaction);
    }
}
