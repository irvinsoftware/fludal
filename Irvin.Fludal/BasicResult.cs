namespace Irvin.Fludal;

public class BasicResult<TValue> : IResult<TValue>
{
    public BasicResult(int? resultCode, TValue value)
    {
        Code = resultCode;
        Content = value;
    }

    public int? Code { get; }
    public TValue Content { get; }
    public IEnumerable<string> Warnings => Enumerable.Empty<string>();
}