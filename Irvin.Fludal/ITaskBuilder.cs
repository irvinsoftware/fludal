namespace Irvin.Fludal;

public interface ITaskBuilder<TSelf>
{
    TSelf WithCancellationToken(CancellationToken cancellationToken);
}