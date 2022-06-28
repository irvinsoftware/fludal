namespace Irvin.TypeConversion;

public static class DataConversionExtensions
{
    public static object ConvertTo(this object value, Type targetType)
    {
        if (targetType == typeof(char))
        {
            return value.ToString().First();
        }
        if (targetType == typeof(char?))
        {
            return value?.ToString().First();
        }

        if (targetType == typeof(string))
        {
            return value?.ToString();
        }

        return value;
    }
}