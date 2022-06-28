namespace Irvin.Fludal.SqlClient;

public static class SqlClientExtensionMethods
{
    public static string ToCanonicalParameterName(this string parameterName)
    {
        parameterName = parameterName.Trim();
        
        if (!parameterName.StartsWith("@"))
        {
            parameterName = $"@{parameterName}";
        }

        return parameterName;
    }
}