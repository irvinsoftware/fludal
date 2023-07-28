using System.Data.Common;
using Irvin.Extensions.Collections;

namespace Irvin.Fludal;

public abstract class DbMultiPartResult : DbCursor, IMultiPartResult
{
    protected DbMultiPartResult(string connectionAddress, DbCommand command, CancellationToken cancellationToken)
        : base(connectionAddress, command)
    {
        Options = new ModelBindingOptions();
        _cancellationToken = cancellationToken;
    }

    public int? Code => ReturnCode;
    public IEnumerable<string> Warnings => ActualWarnings;
    public ModelBindingOptions Options { get; }

    public async Task<T> ReadSingle<T>()
    {
        DbCursor<T> cursor = await GetSubCursor<T>();

        if (await cursor.MoveNextAsync())
        {
            return cursor.Current;
        }

        return default;
    }

    public async Task<IAsyncEnumerable<T>> ReadEnumerable<T>()
    {
        return await GetSubCursor<T>();
    }

    public async Task<List<T>> ReadList<T>()
    {
        DbCursor<T> cursor = await GetSubCursor<T>();
        return await cursor.ToListAsync(_cancellationToken);
    }

    private async Task<DbCursor<T>> GetSubCursor<T>()
    {
        DbDataReader reader = _pipeline.Tip as DbDataReader;
        if (reader != null)
        {
            await reader.NextResultAsync(_cancellationToken);
        }
        else
        {
            await Execute();
        }

        DbCursor<T> subCursor = CreateCursor<T>();
        subCursor.Options = Options;
        return subCursor;
    }

    protected abstract DbCursor<T> CreateCursor<T>();
}