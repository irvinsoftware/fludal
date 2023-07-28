namespace Irvin.Fludal;

public static class DataConversionExtensions
{
    public static object ConvertTo(this object value, Type targetType)
    {
        if (value == DBNull.Value)
        {
            value = null;
        }

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

        if (targetType == typeof(float?))
        {
            if (value == null)
            {
                return null;
            }

            value = Convert.ToSingle(value);
        }

        return value;
    }
}