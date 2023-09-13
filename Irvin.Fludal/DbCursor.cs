using System.Data;
using System.Data.Common;
using System.Reflection;
using Irvin.Extensions;
using Irvin.Extensions.Reflection;

namespace Irvin.Fludal;

public abstract class DbCursor : DbExecutor
{
    protected DbCursor()
    {
    }

    protected DbCursor(string connectionAddress, DbCommand command)
        : base(connectionAddress, command)
    {
    }

    protected override async Task Execute(DbCommand command)
    {
        DbDataReader reader = await command.ExecuteReaderAsync(_cancellationToken).ConfigureAwait(false);
        _pipeline.Push(reader);
    }
    
    public virtual ValueTask DisposeAsync()
    {
        Dispose();
        return new ValueTask();
    }
}

public abstract class DbCursor<TModel> : DbCursor, IAsyncEnumerable<TModel>, IAsyncEnumerator<TModel>
{
    protected DbCursor(string connectionAddress, DbCommand command)
        : base(connectionAddress, command)
    {
        Disposable = true;
    }

    private bool Disposable { get; }
    public ModelBindingOptions Options { get; set; }

    protected DbCursor(ResourceStack pipeline)
    {
        _pipeline = pipeline;
        Disposable = false;
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

        DbDataReader reader = (DbDataReader)_pipeline.Tip;
        if (await reader.ReadAsync(_cancellationToken).ConfigureAwait(false))
        {
            Current = BuildFrom(reader);
            return true;
        }
        
        await DisposeAsync();
        return false;
    }

    public TModel Current { get; private set; }

    private TModel BuildFrom(IDataRecord record)
    {
        Type itemType =
            typeof(TModel).IsTypeOf(typeof(Nullable<>))
                ? typeof(TModel).GetGenericArguments().First()
                : typeof(TModel);
        
        if (itemType.IsBuiltIn())
        {
            return (TModel) record[0];
        }
        
        List<string> columnNames = new List<string>();
        for (int i = 0; i < record.FieldCount; i++)
        {
            columnNames.Add(record.GetName(i));
        }

        IEnumerable<DataMemberInfo> binders = itemType.GetBinders();
        
        if(Options.AreTargetDriven)
        {
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
                    string message = $"No column was found to populate member '{binder.Name}'.";

                    if (Options.Strict)
                    {
                        throw new ArgumentException(message);
                    }

                    ActualWarnings.Add(message);
                }
            }
        }
        else if (Options.AreSourceDriven)
        {
            for (int columnOrdinal = 0; columnOrdinal < columnNames.Count; columnOrdinal++)
            {
                string columnName = columnNames[columnOrdinal];
                
                DataMemberInfo binder = 
                    binders.FirstOrDefault(b => b.Name.Equals(columnName, StringComparison.InvariantCultureIgnoreCase) && 
                                                b.CanSet);

                if (binder == null)
                {
                    string message = $"No field or property found to bind '{columnName}'.";
                    
                    if (Options.Strict)
                    {
                        throw new TargetException(message);
                    }

                    ActualWarnings.Add(message);
                }
                else
                {
                    object value = record[columnOrdinal];
                    value = value.ConvertTo(binder.DataType);
                    binder.Value = value;
                }
            }
        }
        else
        {
            throw new NotSupportedException();
        }

        return binders.Build<TModel>();
    }

    public override ValueTask DisposeAsync()
    {
        return !Disposable
            ? new ValueTask()
            : base.DisposeAsync();
    }
}