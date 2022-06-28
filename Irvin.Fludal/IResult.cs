namespace Irvin.Fludal;

public interface IResult
{
    /// <summary>
    /// The integer code designating the result of the operation, if applicable/available.
    /// </summary>
    /// <remarks>
    /// Some data sources set the code at the start of the operation, others do not set it till the end.
    /// </remarks>
    public int? Code { get; }
    
    /// <summary>
    /// List of any non-critical, non-breaking issues the result retrieval encountered
    /// </summary>
    public IEnumerable<string> Warnings { get; }
}

public interface IResult<T> : IResult
{
    public T Content { get; }
}