using Npgsql;

namespace Irvin.Fludal.Postgres;

public class Postgres : IDbSource<Postgres>, IProcedureTarget<Postgres>, ICommandTarget<Postgres>
{
    public Postgres()
    {
        Builder = new DbCommandBuilder<NpgsqlCommand>();
    }
    
    private DbCommandBuilder<NpgsqlCommand> Builder { get; set; }
    private CancellationToken CancellationToken { get; set; }
    
    public Postgres UsingConfiguredConnectionNamed(string name)
    {
        Builder.SetConnectionAddressFromName(name);
        return this;
    }

    public Postgres UsingConnectionString(string connectionString)
    {
        Builder.SetConnectionAddress(connectionString);
        return this;
    }

    public Postgres AndExecuteStoredProcedure(string procedurePath)
    {
        Builder.SetUpStoredProcedureExecution(procedurePath);
        return this;
    }

    public Postgres ExecuteStoredProcedure(string procedurePath)
    {
        Builder.SetUpStoredProcedureExecution(procedurePath);
        return this;
    }

    public Postgres RunQuery(string queryText)
    {
        Builder.SetUpDirectQueryExecution(queryText);
        return this;
    }

    public Postgres RunCommand(string commandText)
    {
        return RunQuery(commandText);
    }

    public Postgres WithParameter<T>(string name, T? value)
        where T : struct
    {
        Builder.AddInputParameter(name, value);
        return this;
    }

    public Postgres WithParameter(string name, string value)
    {
        Builder.AddInputParameter(name, value);
        return this;
    }

    public Postgres WithOutputParameter<T>(string parameterName)
    {
        Builder.SetUpOutputParameter(parameterName, typeof(T).ToDbType());
        return this;
    }

    public Postgres WithTimeout(TimeSpan timeSpan)
    {
        Builder.SetTimeout(timeSpan);
        return this;
    }

    public Postgres InLessThan(TimeSpan timeSpan)
    {
        Builder.SetTimeout(timeSpan);
        return this;
    }

    public Postgres WithCancellationToken(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        return this;
    }

    public async Task<IResult<List<TModel>>> ThenReadAsList<TModel>()
    {
        return await Builder.ExecuteList<PgSqlResult<TModel>, TModel>(
            (connectionAddress, command) => new PgSqlResult<TModel>(connectionAddress, command), 
            CancellationToken);
    }

    public async Task<IResult<IAsyncEnumerable<TModel>>> ThenReadAsEnumerable<TModel>()
    {
        return await Builder.ExecuteReaderAsync<PgSqlResult<TModel>, TModel>(
            (connectionAddress, command) => new PgSqlResult<TModel>(connectionAddress, command), 
            CancellationToken);
    }

    public async Task<PgSqlResult> AndReturn()
    {
        Builder.CreateResultFactory = (connectionAddress, command) => new PgSqlResult(connectionAddress, command);
        return await Builder.ExecuteNonQueryAsync<PgSqlResult>(CancellationToken);
    }

    public async Task Go()
    {
        using PgSqlResult result = await AndReturn();
    }

    public async Task<IResult<T?>> ThenReturn<T>()
        where T : struct
    {
        return await Builder.ExecuteScalarAsync<T, PgSqlResult<T?>>(
            (connectionAddress, command) => new PgSqlResult<T?>(connectionAddress, command), 
            CancellationToken);
    }

    public IMultiPartResult ThenReadAsMultipleParts(Action<ModelBindingOptions> options)
    {
        Builder.CreateMultiPartResultFactory = (connectionAddress, command, cancellationToken) => 
            new PgSqlMultiPartResult(connectionAddress, command, cancellationToken); 
        return Builder.ExecuteReaders(options, CancellationToken);
    }
}