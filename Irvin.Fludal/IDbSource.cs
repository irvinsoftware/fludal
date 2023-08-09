namespace Irvin.Fludal;

public interface IDbSource<TSelf> : IDataSource<TSelf>, ICommandTarget<TSelf>
{
    TSelf UsingConnectionString(string connectionString);
    TSelf RunQuery(string commandText);
}