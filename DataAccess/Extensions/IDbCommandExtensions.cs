using System.Data;

namespace DataAccess.Extensions;

public static class IDbCommandExtensions
{
    public static void CreateParameter<T>(
        this IDbCommand command,
        string name,
        T value,
        DbType type,
        ParameterDirection direction = ParameterDirection.Input)
    {
        var parameter = command.CreateParameter();

        parameter.ParameterName = name;
        parameter.Value = value;
        parameter.DbType = type;
        parameter.Direction = direction;

        command.Parameters.Add(parameter);
    }
}
