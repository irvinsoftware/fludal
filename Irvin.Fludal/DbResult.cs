using System.Data.Common;

namespace Irvin.Fludal;

public abstract class DbResult : IResult, IDisposable
{
    private DbExecutor Executor { get; set; }

    protected DbResult(string connectionAddress, DbCommand command)
    {
        Executor = CreateExecutor(connectionAddress, command);
    }

    public int? Code => Executor.ReturnCode;
    public IEnumerable<string> Warnings => Executor.ActualWarnings;

    public async Task Prepare(CancellationToken cancellationToken = default)
    {
        await Executor.ExecuteAsync(cancellationToken).ConfigureAwait(false);
    }
    
    public T? GetOutputValue<T>(string parameterName)
        where T : struct
    {
        return (T?) GetOutputParameterValue(parameterName);
    }

    public string GetOutputValue(string parameterName)
    {
        return GetOutputParameterValue(parameterName)?.ToString();
    }

    private object GetOutputParameterValue(string parameterName)
    {
        parameterName = CleanParameterName(parameterName);

        Dictionary<string, object> outputs = Executor.OutputParameters;

        if (!outputs.ContainsKey(parameterName))
        {
            throw new InvalidOperationException(
                $"The output parameter '{parameterName}' was not found. " +
                $"Please use {nameof(IProcedureTarget<object>.WithOutputParameter)} to prepare the command properly.");
        }

        object rawValue = outputs[parameterName];
        if (rawValue == DBNull.Value)
        {
            rawValue = null;
        }

        return rawValue;
    }

    protected abstract DbExecutor CreateExecutor(string connectionAddress, DbCommand command);
    protected abstract string CleanParameterName(string parameterName);

    public void Dispose()
    {
        Executor?.Dispose();
        Executor = null;
    }
}

public abstract class DbResult<TModel> : IResult<IAsyncEnumerable<TModel>>, IDisposable, IAsyncDisposable
{
    private bool IsDisposed { get; set; }
    private DbCursor<TModel> Cursor { get; set; }

    protected DbResult(string connectionAddress, DbCommand command)
    {
        Options = new ModelBindingOptions();
        Cursor = CreateCursor(connectionAddress, command);
        Cursor.Options = Options;
    }

    public async Task Prepare(CancellationToken cancellationToken = default)
    {
        await Cursor.ExecuteAsync(cancellationToken).ConfigureAwait(false);
    }

    public int? Code => Cursor.ReturnCode;
    public IEnumerable<string> Warnings => Cursor.ActualWarnings;
    public IAsyncEnumerable<TModel> Content => Cursor;
    public ModelBindingOptions Options { get; }

    protected abstract DbCursor<TModel> CreateCursor(string connectionAddress, DbCommand command);

    public void Dispose()
    {
        if (!IsDisposed)
        {
            Cursor.Dispose();
            IsDisposed = true;
        }
    }

    public ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            return Cursor.DisposeAsync();
        }

        return new ValueTask();
    }
}