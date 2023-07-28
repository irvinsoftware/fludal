using System.Data.Common;
using System.Data.SqlClient;

namespace Irvin.Fludal.SqlClient;

internal class SqlExecutor : DbExecutor
{
    internal SqlExecutor(string connectionAddress, SqlCommand command)
        : base(connectionAddress, command)
    {
    }

    protected override DbConnection CreateConnection(string connectionAddress)
    {
        return new SqlConnection(connectionAddress);
    }
}
