using System.Data.Common;
using System.Data.SqlClient;

namespace Irvin.Fludal.SqlClient;

internal class SqlCursor : DbCursor
{
    protected SqlCursor(string connectionAddress, SqlCommand command)
        : base(connectionAddress, command)
    {
    }

    protected override DbConnection CreateConnection(string connectionAddress)
    {
        return new SqlConnection(connectionAddress);
    }
}

internal class SqlCursor<TModel> : DbCursor<TModel>
{
    public SqlCursor(string connectionAddress, SqlCommand command) 
        : base(connectionAddress, command)
    {
    }

    public SqlCursor(ResourceStack pipeline) 
        : base(pipeline)
    {
    }

    protected override DbConnection CreateConnection(string connectionAddress)
    {
        return new SqlConnection(connectionAddress);
    }
}