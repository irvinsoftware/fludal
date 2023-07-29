using System.Data.Common;
using Npgsql;

namespace Irvin.Fludal.Postgres;

public class PgSqlResult : DbResult
{
    public PgSqlResult(string connectionAddress, NpgsqlCommand command) 
        : base(connectionAddress, command)
    {
    }

    protected override DbExecutor CreateExecutor(string connectionAddress, DbCommand command)
    {
        return new PgSqlExecutor(connectionAddress, command as NpgsqlCommand);
    }

    protected override string CleanParameterName(string parameterName)
    {
        return parameterName;
    }
}

internal class PgSqlResult<TModel> : DbResult<TModel>
{
    public PgSqlResult(string connectionAddress, NpgsqlCommand command) 
        : base(connectionAddress, command)
    {
    }

    protected override DbCursor<TModel> CreateCursor(string connectionAddress, DbCommand command)
    {
        return new PgSqlCursor<TModel>(connectionAddress, command as NpgsqlCommand);
    }
}