namespace Irvin.Fludal;

public interface IProcedureTarget<TSelf>
{
    public TSelf WithOutputParameter<T>(string parameterName);
}