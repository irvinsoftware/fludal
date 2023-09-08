using System.Data;
using System.Data.Common;
using System.Xml;
using Irvin.Extensions.Collections;

namespace Irvin.Fludal;

public sealed class DbCommandBuilder<TCommand>
    where TCommand : DbCommand, new()
{
    private string ConnectionAddress { get; set; }
    private TCommand Command { get; set; }

    public void SetConnectionAddressFromName(string connectionName)
    {
        string connectionAddress = null;
        
        if (File.Exists("app.config"))
        {
            connectionAddress = Please.GetFrameworkConnectionString(connectionName);
        }

        SetConnectionAddress(connectionAddress);
    }

    public void SetConnectionAddress(string connectionAddress)
    {
        if (string.IsNullOrWhiteSpace(connectionAddress))
        {
            throw new ArgumentNullException(nameof(connectionAddress));
        }
        
        ConnectionAddress = connectionAddress;
    }

    public void SetUpDirectQueryExecution(string commandText)
    {
        if (string.IsNullOrWhiteSpace(commandText))
        {
            throw new ArgumentNullException(nameof(commandText));
        }

        if (Command == null)
        {
            Command = new TCommand();
        }
        
        Command.CommandText = commandText;
        Command.CommandType = CommandType.Text;
    }

    public void SetUpStoredProcedureExecution(string procedurePath)
    {
        if (string.IsNullOrWhiteSpace(procedurePath))
        {
            throw new ArgumentNullException(nameof(procedurePath));
        }

        if (Command == null)
        {
            Command = new TCommand();
        }
        
        Command.CommandText = procedurePath;
        Command.CommandType = CommandType.StoredProcedure;
    }

    public void AddInputParameter(string name, object value)
    {
        if (Command == null)
        {
            Command = new TCommand();
        }

        DbParameter parameter = Command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        Command.Parameters.Add(parameter);
    }

    public void SetUpOutputParameter(string parameterName, DbType parameterType)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentNullException(nameof(parameterName));
        }
        
        if (Command == null)
        {
            Command = new TCommand();
        }

        DbParameter parameter = Command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.DbType = parameterType;
        parameter.Direction = ParameterDirection.Output;
        Command.Parameters.Add(parameter);
    }
    
    public void SetTimeout(TimeSpan timeSpan)
    {
        Command.CommandTimeout = (int)timeSpan.TotalSeconds;
    }

    public Func<string, TCommand, DbResult> CreateResultFactory { get; set; }
    
    public async Task<TResult> ExecuteNonQueryAsync<TResult>(CancellationToken cancellationToken)
        where TResult : DbResult
    {
        if (CreateResultFactory == null)
        {
            throw new ArgumentException(nameof(CreateResultFactory));
        }
        
        DbResult result = CreateResultFactory(ConnectionAddress, Command);
        await result.Prepare(cancellationToken).ConfigureAwait(false);
        return (TResult) result;
    }
    
    public async Task<IResult<TValue?>> ExecuteScalarAsync<TValue, TWrapper>(
        Func<string, TCommand, TWrapper> factory, 
        CancellationToken cancellationToken)
            where TWrapper : DbResult<TValue?>
            where TValue : struct
    {
        await using (DbResult<TValue?> result = factory(ConnectionAddress, Command))
        {
            result.Options.PopulateFields();
            await result.Prepare(cancellationToken).ConfigureAwait(false);
            List<TValue?> content = await result.Content.ToListAsync(cancellationToken);
            return new BasicResult<TValue?>(result.Code, content.FirstOrDefault());
        }
    }

    public async Task<IResult<IAsyncEnumerable<TModel>>> ExecuteReaderAsync<TResult, TModel>(
        Func<string, TCommand, TResult> factory,
        CancellationToken cancellationToken)
            where TResult : DbResult<TModel>
    {
        if (string.IsNullOrWhiteSpace(ConnectionAddress))
        {
            throw new InvalidOperationException(
                $"Please use {nameof(IDataSource<object>.UsingConfiguredConnectionNamed)} to define the connection target.");
        }

        if (Command == null || string.IsNullOrWhiteSpace(Command.CommandText))
        {
            throw new InvalidOperationException($"Please specify a stored procedure or statement to run.");
        }

        DbResult<TModel> result = factory(ConnectionAddress, Command);
        await result.Prepare(cancellationToken).ConfigureAwait(false);
        return result;
    }

    public async Task<IResult<List<TModel>>> ExecuteList<TResult, TModel>(
        Func<string, TCommand, TResult> factory,
        CancellationToken cancellationToken)
            where TResult : DbResult<TModel>
    {
        IResult<IAsyncEnumerable<TModel>> rawResult = await ExecuteReaderAsync<TResult, TModel>(factory, cancellationToken);
        List<TModel> loadedContent = await rawResult.Content.ToListAsync(cancellationToken);
        return new BasicResult<List<TModel>>(rawResult.Code, loadedContent);
    }

    public Func<string, TCommand, CancellationToken, DbMultiPartResult> CreateMultiPartResultFactory { get; set; }
    
    public IMultiPartResult ExecuteReaders(Action<ModelBindingOptions> options, CancellationToken cancellationToken)
    {
        if (CreateMultiPartResultFactory == null)
        {
            throw new ArgumentException(nameof(CreateMultiPartResultFactory));
        }
        
        DbMultiPartResult result = CreateMultiPartResultFactory(ConnectionAddress, Command, cancellationToken);
        options(result.Options);
        return result;
    }
}