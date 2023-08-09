namespace Irvin.Fludal;

public interface IDbSource<TSelf> : IDataSource<TSelf>, ICommandTarget
{
    TSelf UsingConnectionString(string connectionString);
    TSelf RunQuery(string commandText);
}