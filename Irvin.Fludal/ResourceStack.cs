namespace Irvin.Fludal;

public class ResourceStack : IDisposable
{
    private readonly Stack<IDisposable> _resources;

    public ResourceStack()
    {
        _resources = new Stack<IDisposable>();
    }

    public void Push(IDisposable resource)
    {
        _resources.Push(resource);
    }

    public IDisposable Tip => _resources.Peek();

    public int Count => _resources.Count;

    public void Dispose()
    {
        List<Exception> exceptions = new List<Exception>();

        while (_resources.Count > 0)
        {
            IDisposable resource = _resources.Pop();

            try
            {
                resource.Dispose();
            }
            catch (Exception disposeError)
            {
                exceptions.Add(disposeError);
            }
        }

        if (exceptions.Any())
        {
            AggregateException aggregateException = new AggregateException(exceptions);
            throw new ObjectDisposedException("One or more resources could not be disposed properly.", aggregateException);
        }
    }
}
