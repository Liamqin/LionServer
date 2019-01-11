using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using CYQ.Data.Table;
using CYQ.Data.SQL;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace CYQ.Data.Tool
{
    /// <summary>
    /// ����ת����֧��jsonתʵ�壩
    /// </summary>
    public static class ConvertTool
    {
        /// <summary>
        /// ����ת��(��׼ǿ��)
        /// </summary>
        /// <param name="value">ֵ����</param>
        /// <param name="t">����</param>
        /// <returns></returns>
        public static object ChangeType(object value, Type t)
        {
            return StaticTool.ChangeType(value, t);
        }
    }
    /// <summary>
    /// ���乤�ߣ������棩
    /// </summary>
    public static class ReflectTool
    {
        /// <summary>
        /// ��ȡ���Ͳ����ĳ���
        /// </summary>
        public static int GetGenericArgumentLength(ref Type t)
        {
            return StaticTool.GetArgumentLength(ref t);
        }
        /// <summary>
        /// ��ȡ���Ͳ����ĳ��ȣ������ͣ�
        /// </summary>
        public static int GetGenericArgumentLength(ref Type t, out Type[] argTypes)
        {
            return StaticTool.GetArgumentLength(ref t, out argTypes);
        }
        /// <summary>
        /// ��÷������ԣ��ڲ��л��棩
        /// </summary>
        public static List<PropertyInfo> GetPropertys(Type t)
        {
            return StaticTool.GetPropertyInfo(t);
        }
        /// <summary>
        /// ��ȡϵͳ���ͣ�����Nullable���ͣ���תΪ�������͡�
        ///  </summary>
        public static SysType GetSystemType(ref Type t)
        {
            return StaticTool.GetSystemType(ref t);
        }
    }
    /// <summary>
    /// ��̬����������
    /// </summary>
    internal static class StaticTool
    {
        /// <summary>
        /// ��PropertyInfo[] �ĳ�PropertyInfo List������Ϊ.NET��CLR�������ڴ��д�쳣������IntelliTraceʱ��
        /// </summary>
        static MDictionary<string, List<PropertyInfo>> propCache = new MDictionary<string, List<PropertyInfo>>();
        /// <summary>
        /// ��ȡ�����б�
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPropertyInfo(Type t)
        {
            string key = t.GUID.ToString();
            if (propCache.ContainsKey(key))
            {
                return propCache[key];
            }
            else
            {
                bool isInheritOrm = t.BaseType.Name == "OrmBase" || t.BaseType.Name == "SimpleOrmBase";
                PropertyInfo[] pInfo = isInheritOrm ? t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) : t.GetProperties();
                List<PropertyInfo> list = new List<PropertyInfo>(pInfo.Length);
                try
                {

                    list.AddRange(pInfo);
                    propCache.Set(key, list);
                }
                catch (Exception err)
                {
                    Log.WriteLogToTxt(err);
                }
                return list;
            }
        }

        static Dictionary<string, Type[]> argumentCache = new Dictionary<string, Type[]>();
        /// <summary>
        ///  ��ȡ���͵Ĳ������ȣ��Ƿ��Ͱ�Ĭ�Ϸ������㣩
        /// </summary>
        public static int GetArgumentLength(ref Type t)
        {
            Type[] argTypes;
            return GetArgumentLength(ref t, out argTypes);
        }
        /// <summary>
        /// ��ȡ���͵Ĳ������ȣ��Ƿ��Ͱ�Ĭ�Ϸ������㣩
        /// </summary>
        public static int GetArgumentLength(ref Type t, out Type[] argTypes)
        {
            if (argumentCache.ContainsKey(t.FullName))
            {
                argTypes = argumentCache[t.FullName];
                return argTypes.Length;
            }
            else
            {
                int len = 0;
                if (t.IsGenericType)
                {
                    argTypes = t.GetGenericArguments();
                    len = argTypes.Length;
                    for (int i = 0; i < argTypes.Length; i++)
                    {
                        if (argTypes[i].IsGenericType && argTypes[i].Name.StartsWith("Nullable"))
                        {
                            argTypes[i] = Nullable.GetUnderlyingType(argTypes[i]);
                        }
                    }
                    if (t.Name.StartsWith("Nullable"))
                    {
                        t = Nullable.GetUnderlyingType(t);
                    }
                }
                else
                {
                    if (t.Name.EndsWith("[]") || t.Name == "MDataRowCollection")
                    {
                        len = 1;
                    }
                    else if (t.Name == "NameValueCollection" || (t.BaseType != null && t.BaseType.Name == "NameValueCollection"))
                    {
                        len = 2;
                    }
                    else
                    {
                        System.Reflection.MethodInfo mi = t.GetMethod("Add");
                        if (mi != null)
                        {
                            len = mi.GetParameters().Length;
                        }
                    }
                    argTypes = new Type[len];
                    for (int i = 0; i < argTypes.Length; i++)
                    {
                        argTypes[i] = typeof(object);
                    }
                }
                try
                {
                    argumentCache.Add(t.FullName, argTypes);
                }
                catch
                {

                }
                return len;
            }
        }

        /// <summary>
        /// ��ȡϵͳ���ͣ�����Nullable���ͣ���תΪ�������͡�
        ///  </summary>
        public static SysType GetSystemType(ref Type t)
        {
            if (t.IsEnum)
            {
                return SysType.Enum;
            }
            if (t.FullName.StartsWith("System.")) // ϵͳ����
            {
                if (t.IsGenericType)
                {
                    if (t.Name.StartsWith("Nullable"))//int? id
                    {
                        t = Nullable.GetUnderlyingType(t);
                        return SysType.Base;
                    }
                    return SysType.Generic;
                }
                else if (t.FullName.StartsWith("System.Collections."))
                {
                    return SysType.Collection;
                }
                else if (t.Name.EndsWith("[]"))
                {
                    return SysType.Array;
                }
                if (t.FullName.Split('.').Length > 2)
                {
                    return SysType.Custom;
                }
                return SysType.Base;
            }
            else
            {
                return SysType.Custom;
            }
        }

        /// <summary>
        /// ��GUIDת��16�ֽ��ַ���
        /// </summary>
        /// <returns></returns>
        internal static string ToGuidByteString(string guid)
        {
            return BitConverter.ToString(new Guid(guid).ToByteArray()).Replace("-", "");
        }

        /// <summary>
        /// ��ȡԼ��ö�ٵ����ݿ�����
        /// </summary>
        /// <param name="tableNamesEnum">��ö�ٻ����</param>
        /// <returns></returns>
        internal static string GetDbName(ref object tableNamesEnum)
        {
            string dbName = string.Empty;
            if (tableNamesEnum is Enum)
            {
                Type t = tableNamesEnum.GetType();
                string enumName = t.Name;
                if (enumName != "TableNames" && enumName != "ViewNames")
                {
                    if (enumName.Length > 1 && enumName[1] == '_')
                    {
                        dbName = enumName.Substring(2, enumName.Length - 6);//.Replace("Enum", "Conn");
                    }
                    else
                    {
                        string[] items = t.FullName.Split('.');
                        if (items.Length > 1)
                        {
                            dbName = items[items.Length - 2];// +"Conn";
                            items = null;
                        }
                    }
                }
                t = null;
            }
            else if (tableNamesEnum is string)
            {
                string tName = tableNamesEnum.ToString();
                int index = tName.LastIndexOf(')');
                if (index > 0) // ��ͼ
                {
                    string viewSQL = tName;
                    string a = tName.Substring(0, index + 1);//a����
                    tName = tName.Substring(index + 1).Trim();//b���֡�ddd.v_xxx
                    //�޸�ԭ����

                    if (tName.Contains("."))
                    {
                        tableNamesEnum = a + " " + tName.Substring(tName.LastIndexOf('.') + 1);
                    }
                }
                if (tName.Contains(".") && !tName.Trim().Contains(" "))
                {
                    dbName = tName.Split('.')[0];
                }

            }
            return dbName;
        }

        /// <summary>
        /// ����ת��(��׼ǿ��)
        /// </summary>
        /// <param name="value">ֵ����</param>
        /// <param name="t">����</param>
        /// <returns></returns>
        public static object ChangeType(object value, Type t)
        {
            if (t == null)
            {
                return null;
            }
            string strValue = Convert.ToString(value);
            if (t.IsGenericType && t.Name.StartsWith("Nullable"))
            {
                t = Nullable.GetUnderlyingType(t);
                if (strValue == "")
                {
                    return null;
                }
            }
            if (t.Name == "String")
            {
                return strValue;
            }
            if (strValue == "")
            {
                return Activator.CreateInstance(t);
            }
            else if (t.IsValueType)
            {
                if (t.Name == "Guid")
                {
                    return new Guid(strValue);
                }
                else if (t.Name.StartsWith("Int") && strValue.IndexOf('.') > -1)
                {
                    strValue = strValue.Split('.')[0];
                }
                else if (t.Name == "Boolean")
                {
                    switch (strValue.ToLower())
                    {
                        case "yes":
                        case "true":
                        case "1":
                        case "on":
                        case "��":
                            return true;
                        case "no":
                        case "false":
                        case "0":
                        case "":
                        case "��":
                        default:
                            return false;
                    }
                }
                return Convert.ChangeType(strValue, t);
            }
            else
            {
                if (strValue != t.FullName)
                {
                    switch (GetSystemType(ref t))
                    {
                        case SysType.Custom:
                            return MDataRow.CreateFrom(strValue).ToEntity(t);
                        case SysType.Generic:
                            if (t.Name.StartsWith("List"))
                            {
                                return MDataTable.CreateFrom(strValue).ToList(t);
                            }
                            break;
                        case SysType.Array:
                            if (t.Name == "Byte[]" && value.GetType().Name != t.Name)
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    new BinaryFormatter().Serialize(ms, value);
                                    return ms.ToArray();
                                }
                            }
                            break;
                    }
                }
                return Convert.ChangeType(value, t);
            }
        }

        #region ���ַ�����HashKey
        static MDictionary<string, string> hashKeyCache = new MDictionary<string, string>(32);
        internal static string GetHashKey(string sourceString)
        {
            try
            {
                if (hashKeyCache.ContainsKey(sourceString))
                {
                    return hashKeyCache[sourceString];
                }
                else
                {
                    if (hashKeyCache.Count > 512)
                    {
                        hashKeyCache.Clear();
                        hashKeyCache = new MDictionary<string, string>(64);
                    }
                    string value = "K" + Math.Abs(sourceString.GetHashCode()) + sourceString.Length;
                    hashKeyCache.Add(sourceString, value);
                    return value;
                }
            }
            catch
            {
                return sourceString;
            }
        }
        #endregion

    }
}
