namespace Irvin.Fludal
{
    public interface IDataSource<TSelf>
    {
        TSelf UsingConfiguredConnectionNamed(string name);
        Task<IResult<IAsyncEnumerable<TModel>>> ThenReadAsEnumerable<TModel>();
        Task<IResult<List<TModel>>> ThenReadAsList<TModel>();
        IMultiPartResult ThenReadAsMultipleParts();
        TSelf WithCancellationToken(CancellationToken cancellationToken);
    }
}