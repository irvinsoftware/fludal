using System.Data.SqlClient;

namespace Irvin.Fludal.SqlClient;

public class SqlResult : IResult, IDisposable
{
    private SqlExecutor Executor { get; set; }
    
    internal SqlResult(string connectionAddress, SqlCommand command)
    {
        Executor = new SqlExecutor(connectionAddress, command.Clone());
    }

    public int? Code => Executor.ReturnCode;
    public IEnumerable<string> Warnings => Executor.ActualWarnings;

    internal async Task Prepare(CancellationToken cancellationToken = default)
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
        parameterName = parameterName.ToCanonicalParameterName();

        Dictionary<string, object> outputs = Executor.OutputParameters;

        if (!outputs.ContainsKey(parameterName))
        {
            throw new InvalidOperationException(
                $"The output parameter '{parameterName}' was not found. " +
                $"Please use {nameof(SqlServer.WithOutputParameter)} to prepare the command properly.");
        }

        object rawValue = outputs[parameterName];
        if (rawValue == DBNull.Value)
        {
            rawValue = null;
        }

        return rawValue;
    }

    public void Dispose()
    {
        Executor?.Dispose();
        Executor = null;
    }
}

internal class SqlResult<TModel> : IResult<IAsyncEnumerable<TModel>>, IDisposable, IAsyncDisposable
{
    private bool IsDisposed { get; set; }
    private SqlCursor<TModel> Cursor { get; set; }

    internal SqlResult(string connectionAddress, SqlCommand command)
    {
        Options = new ModelBindingOptions();
        Cursor = new SqlCursor<TModel>(connectionAddress, command.Clone());
        Cursor.Options = Options;
    }

    internal async Task Prepare(CancellationToken cancellationToken = default)
    {
        await Cursor.ExecuteAsync(cancellationToken).ConfigureAwait(false);
    }

    public int? Code => Cursor.ReturnCode;
    public IEnumerable<string> Warnings => Cursor.ActualWarnings;
    public IAsyncEnumerable<TModel> Content => Cursor;
    public ModelBindingOptions Options { get; }

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