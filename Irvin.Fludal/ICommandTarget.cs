namespace Irvin.Fludal;

public interface ICommandTarget<TSelf>
{
    TSelf RunCommand(string commandText);
    Task Go();
}