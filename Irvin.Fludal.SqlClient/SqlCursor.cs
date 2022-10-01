using System.Data;
using System.Data.SqlClient;
using Irvin.Extensions;
using Irvin.Extensions.Reflection;
using Irvin.TypeConversion;

namespace Irvin.Fludal.SqlClient;

internal sealed class SqlCursor<TModel> : SqlExecutor, IAsyncEnumerable<TModel>, IAsyncEnumerator<TModel>
{
    public SqlCursor(string connectionAddress, SqlCommand command)
        : base(connectionAddress, command)
    {
    }

    protected override async Task Execute(SqlCommand command)
    {
        SqlDataReader reader = await command.ExecuteReaderAsync(_cancellationToken).ConfigureAwait(false);
        _pipeline.Push(reader);
    }

    public IAsyncEnumerator<TModel> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;
        return this;
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_pipeline == null)
        {
            throw new ObjectDisposedException("The enumerator has been disposed.");
        }

        SqlDataReader reader = (SqlDataReader)_pipeline.Tip;
        if (await reader.ReadAsync(_cancellationToken).ConfigureAwait(false))
        {
            Current = BuildFrom(reader);
            return true;
        }
        
        _pipeline.Dispose();
        _pipeline = null;
        return false;
    }

    public TModel Current { get; private set; }

    private TModel BuildFrom(IDataRecord record)
    {
        Type itemType =
            typeof(TModel).IsTypeOf(typeof(Nullable<>))
                ? typeof(TModel).GetGenericArguments().First()
                : typeof(TModel);
        
        if (itemType.IsPrimitive)
        {
            return (TModel) record[0];
        }
        
        List<string> columnNames = new List<string>();
        for (int i = 0; i < record.FieldCount; i++)
        {
            columnNames.Add(record.GetName(i));
        }

        var binders = itemType.GetBinders();
        
        foreach (DataMemberInfo binder in binders)
        {
            int columnOrdinal = columnNames.FindIndex(name => name.Equals(binder.Name, StringComparison.InvariantCultureIgnoreCase));
            if (columnOrdinal >= 0)
            {
                object value = record[columnOrdinal];
                value = value.ConvertTo(binder.DataType);
                binder.Value = value;
            }
            else
            {
                ActualWarnings.Add($"No column was found to populate member '{binder.Name}'.");
            }
        }

        return binders.Build<TModel>();
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }
}