namespace Irvin.Fludal
{
    public interface IDataSource<TSelf>
    {
        TSelf UsingConfiguredConnectionNamed(string name);
        Task<IResult<IAsyncEnumerable<TModel>>> ThenReadAsEnumerable<TModel>();
        Task<IResult<List<TModel>>> ThenReadAsList<TModel>();
        TSelf WithCancellationToken(CancellationToken cancellationToken);
    }
}