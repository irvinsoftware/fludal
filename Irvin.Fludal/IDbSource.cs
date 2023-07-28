namespace Irvin.Fludal;

public interface IDbSource<TSelf>
{
    TSelf UsingConnectionString(string connectionString);
    TSelf RunQuery(string commandText);
}