using System.Data.SqlClient;
using Irvin.Extensions.Collections;

namespace Irvin.Fludal.SqlClient;

internal class SqlMultiPartResult : SqlCursor, IMultiPartResult
{
    public SqlMultiPartResult(string connectionAddress, SqlCommand command, CancellationToken cancellationToken)
        : base(connectionAddress, command)
    {
        _cancellationToken = cancellationToken;
    }

    public int? Code => ReturnCode;
    public IEnumerable<string> Warnings => ActualWarnings;

    public async Task<T> ReadSingle<T>()
    {
        SqlCursor<T> cursor = await GetSubCursor<T>();

        if (await cursor.MoveNextAsync())
        {
            return cursor.Current;
        }

        return default;
    }

    public async Task<List<T>> ReadList<T>()
    {
        SqlCursor<T> cursor = await GetSubCursor<T>();
        return await cursor.ToListAsync(_cancellationToken);
    }

    private async Task<SqlCursor<T>> GetSubCursor<T>()
    {
        SqlDataReader reader = _pipeline.Tip as SqlDataReader;
        if (reader != null)
        {
            await reader.NextResultAsync(_cancellationToken);
        }
        else
        {
            await Prepare();
        }

        return new SqlCursor<T>(_pipeline);
    }
}