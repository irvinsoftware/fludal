using System.Data;

namespace Irvin.Fludal.SqlClient;

public static class SqlTypeConversionExtensions
{
    public static SqlDbType ToDefaultSqlType(this Type clrType)
    {
        if (clrType == typeof(int)) return SqlDbType.Int;
        if (clrType == typeof(uint)) return SqlDbType.Int;
        if (clrType == typeof(bool)) return SqlDbType.Bit;
        if (clrType == typeof(byte)) return SqlDbType.TinyInt;
        if (clrType == typeof(sbyte)) return SqlDbType.TinyInt;
        if (clrType == typeof(char)) return SqlDbType.NChar;
        if (clrType == typeof(decimal)) return SqlDbType.Decimal;
        if (clrType == typeof(double)) return SqlDbType.Float;
        if (clrType == typeof(float)) return SqlDbType.Float;
        if (clrType == typeof(long)) return SqlDbType.BigInt;
        if (clrType == typeof(ulong)) return SqlDbType.BigInt;
        if (clrType == typeof(short)) return SqlDbType.SmallInt;
        if (clrType == typeof(ushort)) return SqlDbType.SmallInt;
        if (clrType == typeof(DateTime)) return SqlDbType.DateTime;
        if (clrType == typeof(DateTimeOffset)) return SqlDbType.DateTimeOffset;
        if (clrType == typeof(string)) return SqlDbType.NVarChar;
        throw new NotSupportedException();
    }

    public static DbType ToDbType(this Type clrType)
    {
        if (clrType == typeof(int)) return DbType.Int32;
        if (clrType == typeof(uint)) return DbType.UInt32;
        if (clrType == typeof(bool)) return DbType.Boolean;
        if (clrType == typeof(byte)) return DbType.Byte;
        if (clrType == typeof(sbyte)) return DbType.SByte;
        if (clrType == typeof(char)) return DbType.String;
        if (clrType == typeof(decimal)) return DbType.Decimal;
        if (clrType == typeof(double)) return DbType.Double;
        if (clrType == typeof(float)) return DbType.Single;
        if (clrType == typeof(long)) return DbType.Int64;
        if (clrType == typeof(ulong)) return DbType.UInt64;
        if (clrType == typeof(short)) return DbType.Int16;
        if (clrType == typeof(ushort)) return DbType.UInt16;
        if (clrType == typeof(DateTime)) return DbType.DateTime;
        if (clrType == typeof(DateTimeOffset)) return DbType.DateTimeOffset;
        if (clrType == typeof(string)) return DbType.String;
        throw new NotSupportedException();
    }
}