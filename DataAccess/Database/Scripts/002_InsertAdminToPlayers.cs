using DataAccess.Extensions;
using DbUp.Engine;
using Domain.Enums;
using Domain.Helpers;
using Domain.Shared;
using Microsoft.Extensions.Options;
using System.Data;

namespace DataAccess.Database.Scripts;

public class _002_InsertAdminToPlayers : IScript
{
    private readonly AdminOptions _adminOptions;

    public _002_InsertAdminToPlayers(IOptions<AdminOptions> adminOptions)
    {
        _adminOptions = adminOptions.Value;
    }

    public string ProvideScript(Func<IDbCommand> dbCommandFactory)
    {
        const string NameParam = "name";
        const string RoleParam = "role";
        const string PasswordHashParam = "passwordHash";
        const string PasswordSaltParam = "passwordSalt";

        var (passwordHash, passwordSalt) = PasswordHelper.GeneratePasswordHashAndSalt(_adminOptions.Password);

        var dbCommand = dbCommandFactory();

        dbCommand.CommandText = $@"
            INSERT INTO Players (""Name"", ""Role"", ""PasswordHash"", ""PasswordSalt"")
            VALUES (@{NameParam}, @{RoleParam}, @{PasswordHashParam}, @{PasswordSaltParam});";

        dbCommand.CreateParameter(NameParam, _adminOptions.Username, DbType.String);
        dbCommand.CreateParameter(RoleParam, (int)PlayerRole.Admin, DbType.Int16);
        dbCommand.CreateParameter(PasswordHashParam, passwordHash, DbType.Binary);
        dbCommand.CreateParameter(PasswordSaltParam, passwordSalt, DbType.Binary);

        dbCommand.ExecuteNonQuery();

        return string.Empty;
    }
}