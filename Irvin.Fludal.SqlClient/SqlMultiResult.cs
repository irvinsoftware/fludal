using System.Data.Common;
using System.Data.SqlClient;

namespace Irvin.Fludal.SqlClient;

public class SqlMultiPartResult : DbMultiPartResult
{
    public SqlMultiPartResult(string connectionAddress, SqlCommand command, CancellationToken cancellationToken) 
        : base(connectionAddress, command, cancellationToken)
    {
    }

    protected override DbConnection CreateConnection(string connectionAddress)
    {
        return new SqlConnection(connectionAddress);
    }

    protected override DbCursor<T> CreateCursor<T>()
    {
        return new SqlCursor<T>(_pipeline);
    }
}