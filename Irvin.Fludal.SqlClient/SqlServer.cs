using System.Data.SqlClient;

namespace Irvin.Fludal.SqlClient;

public class SqlServer : IDbSource<SqlServer>, IProcedureTarget<SqlServer>, ICommandTarget
{
    public SqlServer()
    {
        Builder = new DbCommandBuilder<SqlCommand>();
    }
    
    private DbCommandBuilder<SqlCommand> Builder { get; set; }
    private CancellationToken CancellationToken { get; set; }
    
    public SqlServer UsingConfiguredConnectionNamed(string name)
    {
        Builder.SetConnectionAddressFromName(name);
        return this;
    }

    public SqlServer UsingConnectionString(string connectionString)
    {
        Builder.SetConnectionAddress(connectionString);
        return this;
    }

    public SqlServer AndExecuteStoredProcedure(string procedurePath)
    {
        Builder.SetUpStoredProcedureExecution(procedurePath);
        return this;
    }

    public SqlServer ExecuteStoredProcedure(string procedurePath)
    {
        Builder.SetUpStoredProcedureExecution(procedurePath);
        return this;
    }

    public SqlServer RunQuery(string commandText)
    {
        Builder.SetUpDirectQueryExecution(commandText);
        return this;
    }

    public SqlServer WithParameter<T>(string name, T? value)
        where T : struct
    {
        Builder.AddInputParameter(name, value);
        return this;
    }

    public SqlServer WithParameter(string name, string value)
    {
        Builder.AddInputParameter(name, value);
        return this;
    }

    public SqlServer WithOutputParameter<T>(string parameterName)
    {
        Builder.SetUpOutputParameter(parameterName, typeof(T).ToDbType());
        return this;
    }
    
    public SqlServer WithTimeout(TimeSpan timeSpan)
    {
        Builder.SetTimeout(timeSpan);
        return this;
    }

    public SqlServer InLessThan(TimeSpan timeSpan)
    {
        Builder.SetTimeout(timeSpan);
        return this;
    }

    public SqlServer WithCancellationToken(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        return this;
    }

    public async Task<IResult<List<TModel>>> ThenReadAsList<TModel>()
    {
        return await Builder.ExecuteList<SqlResult<TModel>, TModel>(
            (connectionAddress, command) => new SqlResult<TModel>(connectionAddress, command), 
            CancellationToken);
    }

    public async Task<IResult<IAsyncEnumerable<TModel>>> ThenReadAsEnumerable<TModel>()
    {
        return await Builder.ExecuteReaderAsync<SqlResult<TModel>, TModel>(
            (connectionAddress, command) => new SqlResult<TModel>(connectionAddress, command), 
            CancellationToken);
    }

    public async Task<SqlResult> AndReturn()
    {
        Builder.CreateResultFactory = (connectionAddress, command) => new SqlResult(connectionAddress, command);
        return await Builder.ExecuteNonQueryAsync<SqlResult>(CancellationToken);
    }

    public async Task Go()
    {
        using SqlResult result = await AndReturn();
    }

    public async Task<IResult<T?>> ThenReturn<T>()
        where T : struct
    {
        return await Builder.ExecuteScalarAsync<T, SqlResult<T?>>(
            (connectionAddress, command) => new SqlResult<T?>(connectionAddress, command), 
            CancellationToken);
    }

    public IMultiPartResult ThenReadAsMultipleParts(Action<ModelBindingOptions> options)
    {
        Builder.CreateMultiPartResultFactory = (connectionAddress, command, cancellationToken) => 
            new SqlMultiPartResult(connectionAddress, command, cancellationToken); 
        return Builder.ExecuteReaders(options, CancellationToken);
    }
}