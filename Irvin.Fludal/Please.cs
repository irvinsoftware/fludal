namespace Irvin.Fludal
{
    public static class Please
    {
        public static T ConnectTo<T>()
            where T : IDataSource<T>, new()
        {
            return new T();
        }
    }
}