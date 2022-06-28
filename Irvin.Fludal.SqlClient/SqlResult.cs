using System.Data.SqlClient;

namespace Irvin.Fludal.SqlClient;

public class SqlResult : IResult
{
    private SqlExecutor Executor { get; }
    
    internal SqlResult(string connectionAddress, SqlCommand command)
    {
        Executor = new SqlExecutor(connectionAddress, command.Clone());
    }

    public int? Code => Executor.ReturnCode;
    public IEnumerable<string> Warnings => Executor.ActualWarnings;

    internal async Task Prepare(CancellationToken cancellationToken = default)
    {
        await Executor.Prepare(cancellationToken).ConfigureAwait(false);
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
}

public class SqlResult<TModel> : IResult<IAsyncEnumerable<TModel>>
{
    private SqlCursor<TModel> Cursor { get; }

    internal SqlResult(string connectionAddress, SqlCommand command)
    {
        Cursor = new SqlCursor<TModel>(connectionAddress, command.Clone());
    }

    internal async Task Prepare(CancellationToken cancellationToken = default)
    {
        await Cursor.Prepare(cancellationToken).ConfigureAwait(false);
    }

    public int? Code => Cursor.ReturnCode;
    public IEnumerable<string> Warnings => Cursor.ActualWarnings;
    public IAsyncEnumerable<TModel> Content => Cursor;
}