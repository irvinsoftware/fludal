using System.Xml;

namespace Irvin.Fludal
{
    public static class Please
    {
        public static T ConnectTo<T>()
            where T : IDataSource<T>, new()
        {
            return new T();
        }

        public static string GetFrameworkConnectionString(string connectionName)
        {
            XmlDocument document = new XmlDocument();
            document.Load("app.config");
            return document.SelectSingleNode($"//connectionStrings/add[@name='{connectionName}']/@connectionString")?.Value;
        }
    }
}