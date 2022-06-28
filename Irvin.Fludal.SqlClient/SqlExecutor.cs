using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Irvin.Fludal.SqlClient;

internal class SqlExecutor
{
    protected ResourceStack _pipeline;
    protected CancellationToken _cancellationToken;
    private readonly SqlParameter _returnParameter;
    
    public SqlExecutor(string connectionAddress, SqlCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        _pipeline = new ResourceStack();
        SqlConnection connection = new SqlConnection(connectionAddress);
        _pipeline.Push(connection);

        command.Connection = connection;
        _returnParameter = new SqlParameter("RETURN_VALUE", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
        command.Parameters.Add(_returnParameter);
        _pipeline.Push(command);
    }
    
    internal int? ReturnCode => (int?)_returnParameter.Value;
    internal List<string> ActualWarnings { get; set; }
    public Dictionary<string, object> OutputParameters { get; private set; }

    internal async Task Prepare(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        SqlCommand command = _pipeline.Tip as SqlCommand;
        Debug.Assert(command != null);

        await command.Connection.OpenAsync(_cancellationToken).ConfigureAwait(false);
        
        await Execute(command).ConfigureAwait(false);
        
        PopulateOutputParameters(command);
    }

    protected virtual async Task Execute(SqlCommand command)
    {
        await command.ExecuteNonQueryAsync(_cancellationToken).ConfigureAwait(false);
    }

    private void PopulateOutputParameters(SqlCommand command)
    {
        OutputParameters =
            command.Parameters.Cast<SqlParameter>()
                   .Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput)
                   .ToDictionary(p => p.ParameterName, p => p.Value == DBNull.Value ? null : p.Value);
    }
}