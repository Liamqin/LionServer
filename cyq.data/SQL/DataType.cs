using System;
using System.Data;

using CYQ.Data.Table;


namespace CYQ.Data.SQL
{
    /// <summary>
    /// ��������ת����
    /// </summary>
    public static partial class DataType
    {
        /// <summary>
        /// ����������Ͷ�Ӧ��Type
        /// </summary>
        public static Type GetType(SqlDbType sqlType)
        {
            return GetType(sqlType, DalType.None);
        }
        /// <summary>
        /// ����������Ͷ�Ӧ��Type
        /// </summary>
        public static Type GetType(SqlDbType sqlType, DalType dalType)
        {

            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(Int64);
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);
                case SqlDbType.Bit:
                    return typeof(Boolean);
                case SqlDbType.Text:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                case SqlDbType.Time:
                case SqlDbType.Xml:
                    return typeof(String);
                case SqlDbType.SmallDateTime:
                case SqlDbType.DateTimeOffset:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTime:
                case SqlDbType.Date:
                    return typeof(DateTime);
                case SqlDbType.Timestamp:
                    switch (dalType)
                    {
                        case DalType.MsSql:
                        case DalType.Sybase:
                            return typeof(byte[]);
                        default:
                            return typeof(DateTime);
                    }
                case SqlDbType.Money:
                case SqlDbType.Decimal:
                case SqlDbType.SmallMoney:
                //case SqlDbType.Udt://����Ƕ�Numeric���͡�
                    return typeof(Decimal);
                case SqlDbType.Float:
                    return typeof(Single);
                case SqlDbType.Int:
                    return typeof(int);
                case SqlDbType.Real:
                    return typeof(double);
                case SqlDbType.TinyInt:
                    return typeof(Byte);
                case SqlDbType.SmallInt:
                    return typeof(Int16);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);
                default:
                    return typeof(object);
            }
        }
        /// <summary>
        /// ��DbType���Ͷ�Ӧӳ�䵽SqlDbType����
        /// </summary>
        /// <param name="type">DbType����</param>
        /// <returns></returns>
        public static SqlDbType GetSqlType(Type type)
        {
            string name = type.Name.ToString();
            if (type.IsEnum)
            {
                name = "string";
            }
            else if (type.IsGenericType)
            {
                if (type.Name.StartsWith("Nullable"))
                {
                    name = Nullable.GetUnderlyingType(type).Name;
                }
                else
                {
                    name = "object";
                }
            }
            else if (!type.FullName.StartsWith("System.") || type.FullName.Split('.').Length > 2)//�Զ����ࡣ
            {
                name = "object";
            }
            return GetSqlType(name);
        }
        /// <summary>
        /// ��DbType�����ַ�����﷽ʽ��Ӧӳ�䵽SqlDbType����
        /// </summary>
        /// <param name="typeName">��������</param>
        /// <returns></returns>
        public static SqlDbType GetSqlType(string typeName)
        {
            typeName = typeName.ToLower().Replace("system.", "").Split('(')[0];
            switch (typeName)
            {
                case "char":
                case "ansistringfixedlength":
                    return SqlDbType.Char;
                case "nchar":
                case "unichar"://Sybase
                case "stringfixedlength":
                    return SqlDbType.NChar;
                case "set"://mysql���� 
                case "enum"://mysql����
                case "varchar":
                case "ansistring":
                case "varchar2":
                    return SqlDbType.VarChar;
                case "nvarchar":
                case "nvarchar2":
                case "string":
                case "univarchar":
                    return SqlDbType.NVarChar;
                case "timestamp":
                case "timestamp(6)"://oracle
                    return SqlDbType.Timestamp;
                case "raw":
                case "bfile":
                case "binary":
                case "tinyblob":
                case "blob":
                case "mediumblob":
                case "longblob":
                case "binary_double":
                case "binary_float":
                case "byte[]":
                case "oleobject":
                    return SqlDbType.Binary;
                case "varbinary":
                    return SqlDbType.VarBinary;
                case "image":
                    return SqlDbType.Image;
                case "bit":
                case "boolean":
                    return SqlDbType.Bit;
                case "tinyint":
                case "byte":
                case "sbyte":
                    return SqlDbType.TinyInt;
                case "money":
                case "currency":
                    return SqlDbType.Money;
                case "smallmoney":
                    return SqlDbType.SmallMoney;
                case "smalldatetime":
                    return SqlDbType.SmallDateTime;
                case "datetime2":
                    return SqlDbType.DateTime2;
                case "datetimeoffset":
                    return SqlDbType.DateTimeOffset;
                case "datetime":
                    return SqlDbType.DateTime;
                case "time":
                    return SqlDbType.Time;
                case "date":
                    return SqlDbType.Date;
                case "numeric":
                    //return SqlDbType.Udt;//�����������û�У�����������á�
                case "decimal":
                    return SqlDbType.Decimal;
                case "real":
                case "double":
                    return SqlDbType.Real;
                case "uniqueidentifier":
                case "guid":
                    return SqlDbType.UniqueIdentifier;
                case "smallint":
                case "int16":
                case "uint16":
                    return SqlDbType.SmallInt;
                case "int":
                case "int32":
                case "uint32":
                case "number":
                case "integer":
                case "mediumint":
                    return SqlDbType.Int;
                case "bigint":
                case "int64":
                case "uint64":
                case "varnumeric":
                case "long":
                    return SqlDbType.BigInt;
                case "variant":
                case "sql_variant":
                case "object":
                    return SqlDbType.Variant;
                case "float":
                case "single":
                    return SqlDbType.Float;
                case "xml":
                case "xmltype":
                    return SqlDbType.Xml;
                case "ntext":
                case "nclob":
                case "unitext":
                    return SqlDbType.NText;
                case "tinytext":
                case "text":
                case "mediumtext":
                case "longtext":
                case "clob":
                    return SqlDbType.Text;
                default:
                    if (typeName.EndsWith("[]"))
                    {
                        return SqlDbType.Variant;
                    }
                    break;
            }

            return SqlDbType.Variant;
        }

        /// <summary>
        /// ��DbType�����ַ��������ʽ��Ӧӳ�䵽DbType����
        /// </summary>
        /// <param name="typeName">��������</param>
        /// <returns></returns>
        public static DbType GetDbType(string typeName)
        {
            return GetDbType(typeName, DalType.None);
        }
        /// <summary>
        /// ��DbType�����ַ��������ʽ��Ӧӳ�䵽DbType����
        /// </summary>
        /// <param name="typeName">��������</param>
        /// <returns></returns>
        public static DbType GetDbType(string typeName, DalType dalType)
        {
            switch (typeName.ToLower().Replace("system.", ""))
            {
                case "ansistring":
                    return DbType.AnsiString;
                case "ansistringfixedlength":
                    return DbType.AnsiStringFixedLength;
                case "text":
                case "ntext":
                case "unitext":
                case "string":
                case "nvarchar":
                    return DbType.String;
                case "char":
                    return DbType.AnsiStringFixedLength;
                case "unichar":
                case "nchar":
                case "stringfixedlength":
                    return DbType.StringFixedLength;
                case "varchar":
                    return DbType.AnsiString;
                case "varbinary":
                case "binary":
                case "image":
                case "byte[]":
                    return DbType.Binary;
                case "bit":
                case "boolean":
                    return DbType.Boolean;
                case "tinyint":
                case "byte":
                    return DbType.Byte;
                case "smallmoney":
                case "currency":
                    return DbType.Currency;
                case "date":
                    return DbType.Date;
                case "time":
                    return DbType.Time;
                case "smalldatetime":
                case "datetime":
                    return DbType.DateTime;
                case "timestamp":
                    switch (dalType)
                    {
                        case DalType.MsSql:
                        case DalType.Sybase:
                            return DbType.Binary;
                        default:
                            return DbType.DateTime;
                    }
                case "udt":
                case "numeric":
                case "decimal":
                    return DbType.Decimal;
                case "real":
                case "money":
                case "double":
                    return DbType.Double;
                case "uniqueidentifier":
                case "guid":
                    return DbType.Guid;
                case "smallint":
                case "int16":
                case "uint16":
                    return DbType.Int16;
                case "int":
                case "int32":
                case "uint32":
                    return DbType.Int32;
                case "bigint":
                case "int64":
                case "uint64":
                    return DbType.Int64;
                case "variant":
                case "object":
                    return DbType.Object;
                case "sbyte":
                    return DbType.SByte;
                case "float":
                case "single":
                    return DbType.Single;
                case "varnumeric":
                    return DbType.VarNumeric;
                case "xml":
                    return DbType.Xml;
            }

            return DbType.String;
        }
        /// <summary>
        /// ��SqlDbType���Ͷ�Ӧӳ�䵽DbType����
        /// </summary>
        /// <param name="type">SqlDbType����</param>
        /// <returns></returns>
        public static DbType GetDbType(Type type)
        {
            return GetDbType(type.Name.ToString());
        }

        #region ��������
        /// <summary>
        /// ��ĸ�ͷ���0�������ͷ���1�������ͷ���2��bool����3��guid����4����������999
        /// </summary>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static int GetGroup(SqlDbType sqlDbType)
        {
            return GetGroup(sqlDbType, DalType.None);
        }
        /// <summary>
        /// ��ĸ�ͷ���0�������ͷ���1�������ͷ���2��bool����3��guid����4����������999
        /// </summary>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        public static int GetGroup(SqlDbType sqlDbType, DalType dalType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.Int:
                case SqlDbType.TinyInt:
                case SqlDbType.BigInt:
                case SqlDbType.SmallInt:
                case SqlDbType.Float:
                case SqlDbType.Real:
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return 1;
                case SqlDbType.Xml:
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                case SqlDbType.NChar:
                case SqlDbType.Char:
                case SqlDbType.Text:
                case SqlDbType.NText:
                case SqlDbType.Time:
                    return 0;
                case SqlDbType.Date:
                case SqlDbType.DateTimeOffset:
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.DateTime2:
                    return 2;
                case SqlDbType.Bit:
                    return 3;
                case SqlDbType.UniqueIdentifier:
                    return 4;
                default:
                    if (sqlDbType == SqlDbType.Timestamp)
                    {
                        if (dalType != DalType.MsSql && dalType != DalType.Sybase)
                        {
                            return 2;
                        }
                    }
                    return 999;
            }
        }


        #endregion
    }

    public static partial class DataType
    {
        /// <summary>
        /// �����ݣ�����������
        /// </summary>
        /// <param name="ms">��Ԫ�ṹ</param>
        /// <param name="dalTo">���ݿ�����</param>
        /// <param name="version">���ݿ�汾��</param>
        /// <returns></returns>
        internal static string GetDataType(MCellStruct ms, DalType dalTo, string version)
        {
            DalType dalFrom = DalType.None;
            if (ms.MDataColumn != null)
            {
                dalFrom = ms.MDataColumn.dalType;
            }
            bool isSameDalType = dalFrom == dalTo;//dalFrom == DalType.None || ��ʵ��ת�нṹʱDalTypeΪNone�����ʺ��������

            SqlDbType sqlType = ms.SqlType;
            int maxSize = ms.MaxSize == 0 ? 255 : ms.MaxSize;
            short scale = ms.Scale;
            version = version ?? string.Empty;
            bool is2000 = version.StartsWith("08") || dalTo == DalType.Sybase;//Sybase ��2000���ơ�

            switch (sqlType)
            {
                case SqlDbType.BigInt:
                case SqlDbType.Int:
                case SqlDbType.TinyInt:
                case SqlDbType.SmallInt:
                    //if (maxSize == 1 && dalType != DalType.Oracle)//oracle number(1)��ʾbit
                    //{
                    //    return "bit";
                    //}
                    switch (dalTo)
                    {
                        case DalType.Access:
                            if (sqlType == SqlDbType.BigInt)
                            {
                                return "long";
                            }
                            break;
                        case DalType.SQLite:
                            if (sqlType == SqlDbType.BigInt)
                            {
                                return "INT64";
                            }
                            return "INTEGER";
                        case DalType.Oracle:
                            if (sqlType == SqlDbType.BigInt)
                            {
                                if (maxSize > 10)
                                {
                                    return "NUMBER(" + maxSize + ")";
                                }
                                return "LONG";
                            }
                            return maxSize < 1 ? "NUMBER" : "NUMBER(" + maxSize + ")";
                        case DalType.MySql:
                            if (sqlType == SqlDbType.TinyInt)
                            {
                                return "tinyint(" + (maxSize > 0 ? maxSize : 4) + ") UNSIGNED";
                            }
                            break;

                    }
                    return sqlType.ToString().ToLower();
                case SqlDbType.Time:
                    if (dalTo == DalType.MySql || dalTo == DalType.SQLite || isSameDalType)
                    {
                        return sqlType.ToString().ToLower();
                    }
                    return "char(12)";
                case SqlDbType.Date:
                    if (isSameDalType) { return sqlType.ToString().ToLower(); }
                    switch (dalTo)
                    {
                        case DalType.MySql:
                        case DalType.SQLite:
                        case DalType.Oracle:
                            return sqlType.ToString().ToLower();
                    }
                    return "datetime";
                case SqlDbType.Timestamp:

                    if (isSameDalType) { return "timestamp"; }
                    if (dalFrom == DalType.MySql || dalFrom == DalType.Oracle)
                    {
                        if (dalTo == DalType.MySql || dalTo == DalType.Oracle)
                        {
                            return "timestamp";
                        }
                        return "datetime";
                    }
                    else if (dalFrom == DalType.MsSql || dalFrom == DalType.Sybase)
                    {
                        if (dalTo == DalType.MsSql || dalTo == DalType.Sybase)
                        {
                            return "timestamp";
                        }
                        return "binary(8)";
                    }
                    return "datetime";
                case SqlDbType.SmallDateTime:
                case SqlDbType.DateTimeOffset:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTime:
                    if (isSameDalType) { return sqlType.ToString().ToLower(); }
                    switch (dalTo)
                    {
                        case DalType.MySql:
                        case DalType.Oracle:
                            if (dalTo == DalType.Oracle)
                            {
                                return "date";
                            }
                            break;
                        case DalType.MsSql:
                        case DalType.Sybase:
                            if (sqlType == SqlDbType.SmallDateTime)
                            {
                                return sqlType.ToString().ToLower();
                            }
                            break;
                    }
                    return "datetime";
                case SqlDbType.Bit:
                    switch (dalTo)
                    {
                        case DalType.Oracle:
                            return "NUMBER(1)";
                    }
                    return "bit";
                case SqlDbType.Udt://��Numeric�����á�
                case SqlDbType.Money:
                case SqlDbType.Decimal:
                case SqlDbType.SmallMoney:
                    switch (dalTo)
                    {
                        case DalType.Access:
                            return "Currency";
                        case DalType.Oracle:
                            return maxSize == -1 ? "NUMBER" : "NUMBER(" + maxSize + "," + scale + ")";
                        case DalType.MsSql:
                        case DalType.Sybase:
                            if (sqlType == SqlDbType.Money || sqlType == SqlDbType.SmallMoney)
                            {
                                return sqlType.ToString().ToLower();
                            }
                            else if (sqlType == SqlDbType.Udt)
                            {
                                return "numeric(" + maxSize + "," + scale + ")";
                            }
                            break;
                    }
                    return "decimal(" + maxSize + "," + scale + ")";
                case SqlDbType.Float:
                    return "float";
                case SqlDbType.Real:
                    switch (dalTo)
                    {
                        case DalType.Oracle:
                            return maxSize == -1 ? "NUMBER" : "NUMBER(" + maxSize + "," + scale + ")";
                        case DalType.Access:
                            return "double";
                        case DalType.MsSql:
                            return ms.SqlType.ToString();
                    }
                    return ms.SqlTypeName;
                case SqlDbType.Variant:
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.VarBinary:
                    if (sqlType == SqlDbType.Variant && isSameDalType && !string.IsNullOrEmpty(ms.SqlTypeName))
                    {
                        return ms.SqlTypeName;
                    }
                    switch (dalTo)
                    {
                        case DalType.SQLite:
                        case DalType.Oracle:
                            return "BLOB";
                        case DalType.Access:
                            return "oleobject";
                        case DalType.MySql:
                            if (ms.SqlTypeName.ToLower() == "binary" && maxSize > 0 && maxSize <= 255)
                            {
                                if (maxSize <= 16) { maxSize = 255; }
                                return "binary(" + maxSize + ")";
                            }
                            if (maxSize < 0 || maxSize > 16777215)
                            {
                                return "longblob";
                            }
                            else if (maxSize <= 255)
                            {
                                return "tinyblob";
                            }
                            else if (maxSize <= 65535) //�����-1���������ˡ�
                            {
                                return "blob";
                            }
                            else
                            {
                                return "mediumblob";
                            }
                        case DalType.MsSql:
                        case DalType.Sybase:
                            string key = sqlType.ToString().ToLower();
                            bool isSybase = dalTo == DalType.Sybase;
                            if (key == "image" || (maxSize < 0 && is2000) || (isSybase && maxSize > 1962))
                            {
                                return "image";
                            }
                            else if (key == "variant")
                            {
                                if (isSybase) { return "text"; }
                                return "sql_variant";
                            }
                            if (maxSize > 0 && maxSize <= 16) { maxSize = 255; }//����MySql��16���ֽڴ���36������
                            return key + "(" + (maxSize < 0 ? "max" : maxSize.ToString()) + ")";
                    }
                    return "binary";

                case SqlDbType.Text:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                    string t = sqlType.ToString().ToLower();
                    switch (dalTo)
                    {
                        case DalType.Access:
                            return (maxSize < 1 || maxSize > 255) ? "memo" : "text(" + maxSize + ")";
                        case DalType.MsSql:
                        case DalType.Sybase:
                            if (maxSize < 1 || maxSize > 8000)//ntext��text
                            {
                                if (dalTo == DalType.Sybase)
                                {
                                    return t[0] == 'n' ? "unitext" : "text";
                                }
                                else if (is2000 || (t[1] != 'v' && t[1] != 'a'))
                                {
                                    return t[0] == 'n' ? "ntext" : "text";
                                }
                                return t + "(max)";
                            }
                            else
                            {
                                if (dalTo == DalType.Sybase && t[0] == 'n')
                                {
                                    t = "uni" + t.Substring(1);
                                }
                                return t + "(" + maxSize + ")";
                            }
                        case DalType.SQLite:
                            return (maxSize < 1 || maxSize > 65535) ? "TEXT" : "TEXT(" + maxSize + ")";
                        case DalType.MySql://mysqlû��nchar֮��ġ�
                            if (ms.IsPrimaryKey && t.EndsWith("text"))
                            {
                                return "varchar(255)";
                            }
                            if (t[0] == 'n')
                            {
                                t = t.Substring(1);
                            }
                            if (t.ToLower() == "text" || maxSize == -1 || maxSize > 21000)
                            {
                                if (maxSize == -1 || maxSize > 16777215)
                                {
                                    return "longtext";

                                }
                                else if (maxSize <= 255)
                                {
                                    return "tinytext";
                                }
                                else if (maxSize <= 65535)
                                {
                                    return "text";
                                }
                                else
                                {
                                    return "mediumtext";
                                }
                            }
                            else
                            {
                                return t + "(" + maxSize + ")";
                            }
                        //return (maxSize < 1 || maxSize > 8000) ? "longtext" : ();
                        case DalType.Oracle:
                            if (maxSize < 1 || maxSize > 4000 || (maxSize > 2000 && (sqlType == SqlDbType.NVarChar || sqlType == SqlDbType.Char))
                                || sqlType == SqlDbType.Text || sqlType == SqlDbType.NText)
                            {
                                return (t[0] == 'n' ? "N" : "") + "CLOB";
                            }
                            else if (sqlType == SqlDbType.Char || sqlType == SqlDbType.NChar)
                            {
                                return "char(" + maxSize + ")";
                            }
                            return t + "2(" + maxSize + ")";
                    }
                    break;
                case SqlDbType.UniqueIdentifier:
                    switch (dalTo)
                    {
                        case DalType.Access:
                            return "GUID";
                        case DalType.MySql:
                        case DalType.Oracle:
                        case DalType.Sybase:
                            return "char(36)";
                    }
                    return "uniqueidentifier";
                case SqlDbType.Xml:
                    switch (dalTo)
                    {
                        case DalType.MsSql:
                            if (is2000)
                            {
                                return "ntext";
                            }
                            else
                            {
                                return "xml";
                            }
                        case DalType.Sybase:
                        case DalType.SQLite:
                            return "text";
                        case DalType.MySql:
                            return "mediumtext";
                        case DalType.Oracle:
                            return "CLOB";
                        case DalType.Access:
                            return "memo";
                    }
                    break;
            }
            return "varchar";
        }
        /// <summary>
        /// CodeFirstʱ��Ĭ�����͵��ֶγ��ȡ�
        /// </summary>
        /// <param name="sqlDbType"></param>
        /// <returns></returns>
        internal static int GetMaxSize(SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.Int:
                    return int.MaxValue.ToString().Length;
                case SqlDbType.TinyInt:
                    return 3;
                case SqlDbType.BigInt:
                    return Int64.MaxValue.ToString().Length;
                case SqlDbType.SmallInt:
                    return Int16.MaxValue.ToString().Length;
                case SqlDbType.Float:
                    return Single.MaxValue.ToString().Length;
                case SqlDbType.Real:
                    return Double.MaxValue.ToString().Length;
                case SqlDbType.Decimal:
                    return Decimal.MaxValue.ToString().Length;
                case SqlDbType.Money:
                    return 19;
                case SqlDbType.NVarChar:
                    return 2000;
                case SqlDbType.VarChar:
                    return 4000;
                case SqlDbType.NChar:
                    return 2000;
                case SqlDbType.Char:
                    return 4000;
                case SqlDbType.Text:
                    return 2147483647;
                case SqlDbType.NText:
                    return 1073741823;
                case SqlDbType.Bit:
                    return 1;
                case SqlDbType.UniqueIdentifier:
                    return Guid.Empty.ToString().Length;
                case SqlDbType.Time:
                    return 18;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Timestamp:
                case SqlDbType.DateTime2:
                default:
                    return -1;
            }
        }
    }
}
