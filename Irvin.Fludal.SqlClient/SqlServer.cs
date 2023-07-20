using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Irvin.Extensions.Collections;

namespace Irvin.Fludal.SqlClient;

public class SqlServer : IDataSource<SqlServer>
{
    private string ConnectionAddress { get; set; }
    private SqlCommand Command { get; set; }
    private CancellationToken CancellationToken { get; set; }
    
    public SqlServer UsingConfiguredConnectionNamed(string name)
    {
        if (File.Exists("app.config"))
        {
            XmlDocument document = new XmlDocument();
            document.Load("app.config");
            ConnectionAddress = document.SelectSingleNode($"//connectionStrings/add[@name='{name}']/@connectionString")?.Value;
        }

        return this;
    }

    public SqlServer UsingConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        
        ConnectionAddress = connectionString;
        return this;
    }

    public SqlServer AndExecuteStoredProcedure(string procedurePath)
    {
        return ExecuteStoredProcedure(procedurePath);
    }

    public SqlServer ExecuteStoredProcedure(string procedurePath)
    {
        if (string.IsNullOrWhiteSpace(procedurePath))
        {
            throw new ArgumentNullException(nameof(procedurePath));
        }

        if (Command == null)
        {
            Command = new SqlCommand();
        }
        
        Command.CommandText = procedurePath;
        Command.CommandType = CommandType.StoredProcedure;
        return this;
    }

    public SqlServer RunQuery(string commandText)
    {
        if (string.IsNullOrWhiteSpace(commandText))
        {
            throw new ArgumentNullException(nameof(commandText));
        }

        if (Command == null)
        {
            Command = new SqlCommand();
        }
        
        Command.CommandText = commandText;
        Command.CommandType = CommandType.Text;
        return this;
    }

    public SqlServer WithParameter<T>(string name, T? value)
        where T : struct
    {
        AddInputParameter(name, value);
        return this;
    }

    public SqlServer WithParameter(string name, string value)
    {
        AddInputParameter(name, value);
        return this;
    }

    private void AddInputParameter(string name, object value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }
        
        if (Command == null)
        {
            Command = new SqlCommand();
        }

        Command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }

    public SqlServer WithOutputParameter<T>(string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentNullException(nameof(parameterName));
        }
        
        if (Command == null)
        {
            Command = new SqlCommand();
        }

        Command.Parameters.Add(new SqlParameter(parameterName, typeof(T).ToDefaultSqlType()) { Direction = ParameterDirection.Output });
        return this;
    }

    public SqlServer WithCancellationToken(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        return this;
    }

    public async Task<IResult<List<TModel>>> ThenReadAsList<TModel>()
    {
        IResult<IAsyncEnumerable<TModel>> rawResult = await ThenReadAsEnumerable<TModel>().ConfigureAwait(false);
        List<TModel> loadedContent = await rawResult.Content.ToListAsync(CancellationToken).ConfigureAwait(false);
        return new BasicResult<List<TModel>>(rawResult.Code, loadedContent);
    }

    public async Task<IResult<IAsyncEnumerable<TModel>>> ThenReadAsEnumerable<TModel>()
    {
        if (string.IsNullOrWhiteSpace(ConnectionAddress))
        {
            throw new InvalidOperationException(
                $"Please use {nameof(UsingConfiguredConnectionNamed)} to define the connection target.");
        }

        if (Command == null || string.IsNullOrWhiteSpace(Command.CommandText))
        {
            throw new InvalidOperationException($"Please specify a stored procedure or statement to run.");
        }

        SqlResult<TModel> result = new SqlResult<TModel>(ConnectionAddress, Command);
        await result.Prepare(CancellationToken).ConfigureAwait(false);
        return result;
    }

    public async Task<SqlResult> AndReturn()
    {
        SqlResult result = new SqlResult(ConnectionAddress, Command);
        await result.Prepare(CancellationToken).ConfigureAwait(false);
        return result;
    }
    
    public async Task Go()
    {
        using SqlResult result = new SqlResult(ConnectionAddress, Command);
        await result.Prepare(CancellationToken).ConfigureAwait(false);
    }

    public async Task<IResult<T?>> ThenReturn<T>()
        where T : struct
    {
        SqlResult<T?> result = new SqlResult<T?>(ConnectionAddress, Command);
        result.Options.PopulateFields();
        await result.Prepare(CancellationToken).ConfigureAwait(false);
        List<T?> content = await result.Content.ToListAsync(cancellationToken: CancellationToken);
        return new BasicResult<T?>(result.Code, content.FirstOrDefault());
    }

    public IMultiPartResult ThenReadAsMultipleParts(Action<ModelBindingOptions> options)
    {
        SqlMultiPartResult result = new SqlMultiPartResult(ConnectionAddress, Command, CancellationToken);
        options(result.Options);
        return result;
    }
}