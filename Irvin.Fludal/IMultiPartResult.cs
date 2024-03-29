﻿namespace Irvin.Fludal;

public interface IMultiPartResult : IResult, IDisposable
{
    public Task<T> ReadSingle<T>();
    public Task<IAsyncEnumerable<T>> ReadEnumerable<T>();
    public Task<List<T>> ReadList<T>();
}