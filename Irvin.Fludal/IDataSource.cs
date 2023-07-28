namespace Irvin.Fludal
{
    public interface IDataSource<TSelf>
    {
        TSelf UsingConfiguredConnectionNamed(string name);
        TSelf WithTimeout(TimeSpan timeSpan);
        TSelf InLessThan(TimeSpan timeSpan);
        Task<IResult<T?>> ThenReturn<T>() where T : struct;
        Task<IResult<IAsyncEnumerable<TModel>>> ThenReadAsEnumerable<TModel>();
        Task<IResult<List<TModel>>> ThenReadAsList<TModel>();
        IMultiPartResult ThenReadAsMultipleParts(Action<ModelBindingOptions> options);
        TSelf WithCancellationToken(CancellationToken cancellationToken);
    }
}