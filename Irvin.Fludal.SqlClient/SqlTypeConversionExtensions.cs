using System.Data;

namespace Irvin.Fludal.SqlClient;

public static class SqlTypeConversionExtensions
{
    public static SqlDbType ToDefaultSqlType(this Type clrType)
    {
        if (clrType == typeof(int)) return SqlDbType.Int;
        if (clrType == typeof(bool)) return SqlDbType.Bit;
        if (clrType == typeof(byte)) return SqlDbType.TinyInt;
        if (clrType == typeof(sbyte)) return SqlDbType.TinyInt;
        if (clrType == typeof(char)) return SqlDbType.NChar;
        if (clrType == typeof(decimal)) return SqlDbType.Decimal;
        if (clrType == typeof(double)) return SqlDbType.Float;
        if (clrType == typeof(int)) return SqlDbType.Int;
        if (clrType == typeof(long)) return SqlDbType.BigInt;
        if (clrType == typeof(short)) return SqlDbType.SmallInt;
        if (clrType == typeof(DateTime)) return SqlDbType.DateTime;
        if (clrType == typeof(DateTimeOffset)) return SqlDbType.DateTimeOffset;
        if (clrType == typeof(string)) return SqlDbType.NVarChar;
        throw new NotSupportedException();
    }
}