namespace Irvin.Fludal;

public interface IProcedureTarget<TSelf>
{
    TSelf ExecuteStoredProcedure(string procedurePath);
    TSelf AndExecuteStoredProcedure(string procedurePath);
    public TSelf WithOutputParameter<T>(string parameterName);
}