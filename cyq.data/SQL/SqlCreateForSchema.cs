using System;
using System.Collections.Generic;
using System.Text;
using CYQ.Data.Table;
using System.Data;
using CYQ.Data.Tool;

namespace CYQ.Data.SQL
{
    /// <summary>
    /// SQL �ṹ���
    /// </summary>
    internal class SqlCreateForSchema
    {
        #region ��ȡCreateTable���SQL���
        /// <summary>
        /// ��ȡ�����еı��������
        /// </summary>
        internal static string GetTableDescriptionSql(string tableName, MCellStruct mcs, DalType dalType, bool isAdd)
        {
            switch (dalType)
            {
                case DalType.MsSql:
                    string spName = isAdd ? "sp_addextendedproperty" : "sp_updateextendedproperty";
                    return string.Format("exec {3} N'MS_Description', N'{0}', N'user', N'dbo', N'table', N'{1}', N'column', N'{2}'", mcs.Description, tableName, mcs.ColumnName, spName);
                case DalType.Oracle:
                    return string.Format("comment on column {0}.{1}  is '{2}'", tableName.ToUpper(), mcs.ColumnName.ToUpper(), mcs.Description);
            }

            return string.Empty;
        }


        /// <summary>
        /// ��ȡָ���ı�ܹ����ɵ�SQL(Create Table)��˵�����
        /// </summary>
        internal static string CreateTableDescriptionSql(string tableName, MDataColumn columns, DalType dalType)
        {
            string result = string.Empty;
            switch (dalType)
            {
                case DalType.MsSql:
                case DalType.Oracle:
                    StringBuilder sb = new StringBuilder();
                    foreach (MCellStruct mcs in columns)
                    {
                        if (!string.IsNullOrEmpty(mcs.Description))
                        {
                            if (dalType == DalType.MsSql)
                            {
                                sb.AppendFormat("exec sp_addextendedproperty N'MS_Description', N'{0}', N'user', N'dbo', N'table', N'{1}', N'column', N'{2}';\r\n", mcs.Description, tableName, mcs.ColumnName);
                            }
                            else if (dalType == DalType.Oracle)
                            {
                                sb.AppendFormat("comment on column {0}.{1}  is '{2}';\r\n", tableName.ToUpper(), mcs.ColumnName.ToUpper(), mcs.Description);
                            }
                        }
                    }
                    if (dalType == DalType.MsSql)//���ӱ������
                    {
                        sb.AppendFormat("exec sp_addextendedproperty N'MS_Description', N'{0}', N'user', N'dbo', N'table', N'{1}';\r\n", columns.Description, tableName);
                    }
                    result = sb.ToString().TrimEnd(';');
                    break;
            }

            return result;
        }


        /// <summary>
        /// ��ȡָ���ı�ܹ����ɵ�SQL(Create Table)���
        /// </summary>
        internal static string CreateTableSql(string tableName, MDataColumn columns, DalType dalType, string version)
        {
            switch (dalType)
            {
                case DalType.Txt:
                case DalType.Xml:
                    return columns.ToJson(true);
                default:
                    string createSql = string.Empty;
                    createSql = "CREATE TABLE " + SqlFormat.Keyword(tableName, dalType) + " \n(";

                    //��ȡ�����ĸ��������������������������������
                    List<MCellStruct> primaryKeyList = new List<MCellStruct>();
                    foreach (MCellStruct column in columns)
                    {
                        if (column.IsPrimaryKey)
                        {
                            primaryKeyList.Add(column);
                        }
                    }
                    foreach (MCellStruct column in columns)
                    {
                        createSql += "\n    " + GetKey(column, dalType, ref primaryKeyList, version);
                    }
                    if (primaryKeyList.Count > 0)
                    {
                        createSql += GetUnionPrimaryKey(dalType, primaryKeyList);
                    }
                    createSql = createSql.TrimEnd(',') + " \n)";
                    // createSql += GetSuffix(dalType);
                    if (dalType == DalType.MySql && createSql.IndexOf("CURRENT_TIMESTAMP") != createSql.LastIndexOf("CURRENT_TIMESTAMP"))
                    {
                        createSql = createSql.Replace("Default CURRENT_TIMESTAMP", string.Empty);//mysql����������������ϵ�CURRENT_TIMESTAMP��
                    }
                    primaryKeyList.Clear();
                    return createSql;
            }
        }

        private static string GetKey(MCellStruct column, DalType dalType, ref List<MCellStruct> primaryKeyList, string version)
        {
            string key = SqlFormat.Keyword(column.ColumnName, dalType);//������
            int groupID = DataType.GetGroup(column.SqlType);//���ݿ����͡�
            bool isAutoOrPKey = column.IsPrimaryKey || column.IsAutoIncrement;//�Ƿ������������С�
            if (dalType != DalType.Access || !isAutoOrPKey || !column.IsAutoIncrement)
            {
                SqlDbType sdt = column.SqlType;
                if (sdt == SqlDbType.DateTime && dalType == DalType.MySql && Convert.ToString(column.DefaultValue) == SqlValue.GetDate)
                {
                    sdt = SqlDbType.Timestamp;
                }
                key += " " + DataType.GetDataType(column, dalType, version);
            }
            if (isAutoOrPKey)
            {
                if (column.IsAutoIncrement)
                {
                    if (primaryKeyList.Count == 0 || (!column.IsPrimaryKey && dalType == DalType.MySql))//MySql ����������������.
                    {
                        column.IsPrimaryKey = true;
                        primaryKeyList.Insert(0, column);
                    }
                }
                switch (dalType)
                {
                    case DalType.Access:
                        if (column.IsAutoIncrement)
                        {
                            key += " autoincrement(1,1)";
                        }
                        else// ������
                        {
                            if (groupID == 4)//��������GUID
                            {
                                key += " default GenGUID()";
                            }
                        }
                        break;
                    case DalType.MsSql:
                        if (column.IsAutoIncrement)
                        {
                            key += " IDENTITY(1,1)";
                        }
                        else
                        {
                            if (groupID == 4)//��������GUID
                            {
                                key += " Default (newid())";
                            }
                        }
                        break;
                    case DalType.Oracle:
                        if (Convert.ToString(column.DefaultValue) == SqlValue.Guid)//��������GUID
                        {
                            key += " Default (SYS_GUID())";
                        }
                        break;
                    case DalType.Sybase:
                        if (column.IsAutoIncrement)
                        {
                            key += " IDENTITY";
                        }
                        else
                        {
                            if (groupID == 4)//��������GUID
                            {
                                key += " Default (newid())";
                            }
                        }
                        break;
                    case DalType.MySql:
                        if (column.IsAutoIncrement)
                        {
                            key += " AUTO_INCREMENT";
                            if (!column.IsPrimaryKey)
                            {
                                primaryKeyList.Add(column);
                            }
                        }
                        break;
                    case DalType.SQLite://sqlite��AUTOINCREMENT����д��primarykeyǰ,
                        if (column.IsAutoIncrement)
                        {
                            key += " PRIMARY KEY AUTOINCREMENT";
                            primaryKeyList.Clear();//����������ӣ�ֻ���������һ��������
                        }
                        break;
                }
                key += " NOT NULL";
            }
            else
            {
                string defaultValue = string.Empty;
                if (Convert.ToString(column.DefaultValue).Length > 0 && groupID < 5)//Ĭ��ֵֻ���ǻ��������С�
                {
                    if (dalType == DalType.MySql)
                    {
                        if ((groupID == 0 && (column.MaxSize < 1 || column.MaxSize > 8000)) || (groupID == 2 && key.Contains("datetime"))) //ֻ�ܶ�TIMESTAMP���͵ĸ�Ĭ��ֵ��
                        {
                            goto er;
                        }
                    }
                    defaultValue = SqlFormat.FormatDefaultValue(dalType, column.DefaultValue, 1, column.SqlType);
                    if (!string.IsNullOrEmpty(defaultValue))
                    {
                        if (dalType == DalType.MySql) { defaultValue = defaultValue.Trim('(', ')'); }
                        key += " Default " + defaultValue;
                    }
                }

            er:
                if (dalType != DalType.Access)
                {
                    if (dalType == DalType.Sybase && column.SqlType == SqlDbType.Bit)
                    {
                        if (string.IsNullOrEmpty(defaultValue))
                        {
                            key += " Default 0";
                        }
                        key += " NOT NULL";//Sybase bit ������ΪNull
                    }
                    else
                    {
                        key += column.IsCanNull ? " NULL" : " NOT NULL";
                    }
                }
            }
            if (!string.IsNullOrEmpty(column.Description))
            {
                switch (dalType)
                {
                    case DalType.MySql:
                        key += string.Format(" COMMENT '{0}'", column.Description.Replace("'", "''"));
                        break;
                }
            }
            return key + ",";
        }
        private static string GetUnionPrimaryKey(DalType dalType, List<MCellStruct> primaryKeyList)
        {
            string suffix = "\n    ";

            switch (dalType)
            {
                //case DalType.Access:
                //case DalType.SQLite:
                //case DalType.MySql:
                //case DalType.Oracle:
                //case DalType.MsSql:
                //case DalType.Sybase:
                default:
                    suffix += "PRIMARY KEY (";
                    foreach (MCellStruct st in primaryKeyList)
                    {
                        suffix += SqlFormat.Keyword(st.ColumnName, dalType) + ",";
                    }
                    suffix = suffix.TrimEnd(',') + ")";
                    break;
            }
            return suffix;
        }
        #endregion

        #region ��ȡAlterTable���SQL���

        /// <summary>
        /// ��ȡָ���ı�ܹ����ɵ�SQL(Alter Table)���
        /// </summary>
        public static List<string> AlterTableSql(string tableName, MDataColumn columns, string conn)
        {
            List<string> sql = new List<string>();

            DbBase helper = DalCreate.CreateDal(conn);
            helper.ChangeDatabaseWithCheck(tableName);//���dbname.dbo.tablename�����
            if (!helper.TestConn(AllowConnLevel.Master))
            {
                helper.Dispose();
                return sql;
            }
            DalType dalType = helper.dalType;
            string version = helper.Version;
            MDataColumn dbColumn = TableSchema.GetColumns(tableName, ref helper);//��ȡ���ݿ���нṹ
            helper.Dispose();

            //��ʼ�Ƚ���ͬ
            List<MCellStruct> primaryKeyList = new List<MCellStruct>();
            string tbName = SqlFormat.Keyword(tableName, dalType);
            string alterTable = "alter table " + tbName;
            foreach (MCellStruct ms in columns)//�����µĽṹ
            {
                string cName = SqlFormat.Keyword(ms.ColumnName, dalType);
                if (ms.AlterOp != AlterOp.None)
                {
                    bool isContains = dbColumn.Contains(ms.ColumnName);
                    AlterOp op = ms.AlterOp;
                    if ((op & AlterOp.Rename) != 0)
                    {
                        op = (AlterOp)(op - AlterOp.Rename);
                        #region MyRegion Rename
                        if (!string.IsNullOrEmpty(ms.OldName) && ms.OldName != ms.ColumnName && !isContains)
                        {
                            string oName = SqlFormat.Keyword(ms.OldName, dalType);
                            switch (dalType)
                            {
                                case DalType.MsSql:
                                    sql.Add("exec sp_rename '" + tbName + "." + oName + "', '" + ms.ColumnName + "', 'column'");
                                    break;
                                case DalType.Sybase:
                                    sql.Add("exec sp_rename \"" + tableName + "." + ms.OldName + "\", " + ms.ColumnName);
                                    break;
                                case DalType.MySql:
                                    sql.Add(alterTable + " change " + oName + " " + GetKey(ms, dalType, ref primaryKeyList, version).TrimEnd(','));
                                    break;
                                case DalType.Oracle:

                                    sql.Add(alterTable + " rename column " + oName + " to " + cName);
                                    break;
                            }
                            isContains = isContains || dbColumn.Contains(ms.OldName);
                        }
                        #endregion
                    }

                    if (op == AlterOp.Drop)
                    {
                        #region MyRegion
                        if (isContains)
                        {
                            switch (dalType)
                            {
                                case DalType.MsSql:
                                case DalType.Access:
                                case DalType.MySql:
                                case DalType.Oracle:
                                    if (dalType == DalType.MsSql)
                                    {
                                        sql.Add(@"declare @name varchar(50) select  @name =b.name from sysobjects b join syscolumns a on b.id = a.cdefault 
where a.id = object_id('" + tableName + "') and a.name ='" + ms.ColumnName + "'if(@name!='') begin   EXEC('alter table "+tableName+" drop constraint '+ @name) end");
                                    }
                                    sql.Add(alterTable + " drop column " + cName);
                                    break;
                                case DalType.Sybase:
                                    sql.Add(alterTable + " drop " + cName);
                                    break;
                            }
                        }
                        #endregion
                    }
                    //else if (ms.AlterOp == AlterOp.Rename)
                    //{

                    //}
                    else if (op == AlterOp.AddOrModify)
                    {
                        string alterSql = SqlFormat.Keyword(ms.ColumnName, dalType) + " " + DataType.GetDataType(ms, dalType, version);
                        //�����ж�
                        if (isContains) // ���ڣ����޸�
                        {
                            //����Ƿ���ͬ
                            MCellStruct dbStruct = dbColumn[ms.ColumnName] ?? dbColumn[ms.OldName];
                            if (dbStruct.IsCanNull != ms.IsCanNull || dbStruct.SqlType != ms.SqlType || dbStruct.MaxSize != ms.MaxSize || dbStruct.Scale != ms.Scale)
                            {
                                string modify = "";
                                switch (dalType)
                                {
                                    case DalType.Oracle:
                                    case DalType.Sybase:
                                        modify = " modify ";
                                        break;
                                    case DalType.MySql:
                                        modify = " change " + cName + " ";
                                        break;
                                    case DalType.MsSql:
                                    case DalType.Access:
                                        modify = " alter column ";
                                        break;
                                }
                                if (ms.IsCanNull != dbStruct.IsCanNull)
                                {
                                    alterSql += (ms.IsCanNull ? " NULL" : " NOT NULL");
                                }
                                sql.Add(alterTable + modify + alterSql);
                            }
                        }
                        else //���ڣ������
                        {
                            sql.Add(alterTable + " add " + GetKey(ms, dalType, ref primaryKeyList, version).TrimEnd(','));
                            if (!string.IsNullOrEmpty(ms.Description))
                            {
                                string description = SqlCreateForSchema.GetTableDescriptionSql(tableName, ms, dalType, true);
                                if (!string.IsNullOrEmpty(description))
                                {
                                    sql.Add(description);
                                }
                            }
                        }
                    }
                }

            }
            return sql;
        }
        #endregion
    }
}
