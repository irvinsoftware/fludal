using System.Data;
using System.Data.Common;

namespace Irvin.Fludal;

public abstract class DbExecutor : IDisposable
{
    protected ResourceStack _pipeline;
    protected CancellationToken _cancellationToken;
    private readonly DbParameter _returnParameter;

    protected DbExecutor()
    {
        ActualWarnings = new List<string>();
    }

    protected DbExecutor(string connectionAddress, DbCommand template)
        : this()
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }
        
        _pipeline = new ResourceStack();
        DbConnection connection = CreateConnection(connectionAddress);
        _pipeline.Push(connection);

        DbCommand command = connection.CreateCommand();
        PopulateCommand(template, command);

        _returnParameter = command.CreateParameter();
        _returnParameter.ParameterName = "RETURN_VALUE";
        _returnParameter.DbType = DbType.Int32;
        _returnParameter.Direction = ParameterDirection.ReturnValue;
        command.Parameters.Add(_returnParameter);
        
        _pipeline.Push(command);
    }

    private static void PopulateCommand(DbCommand source, DbCommand destination)
    {
        destination.CommandText = source.CommandText;
        destination.CommandType = source.CommandType;
        destination.CommandTimeout = source.CommandTimeout;
        destination.Parameters.AddRange(
            source.Parameters.Cast<DbParameter>()
                             .Select(sourceParameter =>
                             {
                                 DbParameter parameter = destination.CreateParameter();
                                 PopulateParameter(sourceParameter, parameter);
                                 return parameter;
                             })
                             .ToArray());
    }

    private static void PopulateParameter(DbParameter source, DbParameter destination)
    {
        destination.ParameterName = source.ParameterName;
        destination.DbType = source.DbType;
        destination.Size = source.Size;
        destination.Scale = source.Scale;
        destination.Precision = source.Precision;
        destination.Direction = source.Direction;
        destination.IsNullable = source.IsNullable;
        destination.Value = source.Value;
    }

    protected abstract DbConnection CreateConnection(string connectionAddress);

    public int? ReturnCode => (int?)_returnParameter.Value;
    public List<string> ActualWarnings { get; private set; }
    public Dictionary<string, object> OutputParameters { get; private set; }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        DbCommand command = await Execute();

        PopulateOutputParameters(command);
    }

    protected async Task<DbCommand> Execute()
    {
        DbCommand command = _pipeline.Tip as DbCommand;

        await command.Connection.OpenAsync(_cancellationToken).ConfigureAwait(false);

        await Execute(command).ConfigureAwait(false);
        return command;
    }

    protected virtual async Task Execute(DbCommand command)
    {
        await command.ExecuteNonQueryAsync(_cancellationToken).ConfigureAwait(false);
    }

    private void PopulateOutputParameters(DbCommand command)
    {
        OutputParameters =
            command.Parameters.Cast<DbParameter>()
                   .Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput)
                   .ToDictionary(p => p.ParameterName, p => p.Value == DBNull.Value ? null : p.Value);
    }

    public virtual void Dispose()
    {
        _pipeline?.Dispose();
        _pipeline = null;
    }
}