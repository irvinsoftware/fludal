using System.Data.Common;
using System.Data.SqlClient;

namespace Irvin.Fludal.SqlClient;

public class SqlResult : DbResult
{
    public SqlResult(string connectionAddress, SqlCommand command) 
        : base(connectionAddress, command)
    {
    }

    protected override DbExecutor CreateExecutor(string connectionAddress, DbCommand command)
    {
        return new SqlExecutor(connectionAddress, command as SqlCommand);
    }

    protected override string CleanParameterName(string parameterName)
    {
        return parameterName.ToCanonicalParameterName();
    }
}

internal class SqlResult<TModel> : DbResult<TModel>
{
    public SqlResult(string connectionAddress, SqlCommand command) 
        : base(connectionAddress, command)
    {
    }

    protected override DbCursor<TModel> CreateCursor(string connectionAddress, DbCommand command)
    {
        return new SqlCursor<TModel>(connectionAddress, command as SqlCommand);
    }
}