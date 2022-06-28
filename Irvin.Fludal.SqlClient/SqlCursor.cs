using System.Data;
using System.Data.SqlClient;
using System.Reflection;

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
        List<string> columnNames = new List<string>();
        for (int i = 0; i < record.FieldCount; i++)
        {
            columnNames.Add(record.GetName(i));
        }

        TModel model = Activator.CreateInstance<TModel>();

        MemberInfo[] memberInfos = typeof(TModel).GetMembers();
        foreach (MemberInfo memberInfo in memberInfos)
        {
            if (memberInfo.MemberType == MemberTypes.Field || memberInfo.MemberType == MemberTypes.Property)
            {
                int columnOrdinal = columnNames.FindIndex(x => x == memberInfo.Name);
                if (columnOrdinal >= 0)
                {
                    object value = record[columnOrdinal];
                    if (value == DBNull.Value)
                    {
                        value = null;
                    }

                    if (memberInfo is PropertyInfo)
                    {
                        PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                        if (propertyInfo.PropertyType == typeof(char?)) //TODO: better mapping
                        {
                            value = value?.ToString().First();
                        }

                        propertyInfo.SetValue(model, value);
                    }
                    else if (memberInfo is FieldInfo)
                    {
                        FieldInfo fieldInfo = memberInfo as FieldInfo;
                        if (fieldInfo.FieldType == typeof(char?)) //TODO: better mapping
                        {
                            value = value?.ToString().First();
                        }

                        fieldInfo.SetValueDirect(__makeref(model), value); //TODO: find better implementation
                    }
                }
                else
                {
                    ActualWarnings.Add($"No column was found to populate member '{memberInfo.Name}'.");
                }
            }
        }

        return model;
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }
}