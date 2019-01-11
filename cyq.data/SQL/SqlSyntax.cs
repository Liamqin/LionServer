using System;
using System.Collections.Generic;
using System.Text;

namespace CYQ.Data.SQL
{
    /// <summary>
    /// SQL�﷨��������
    /// </summary>
    internal class SqlSyntax
    {
        /// <summary>
        /// �����﷨
        /// </summary>
        public static SqlSyntax Analyze(string sql)
        {
            return new SqlSyntax(sql);
        }
        private SqlSyntax(string sql)
        {
            FormatSqlText(sql);
        }
        public string TableName = string.Empty;
        public string Where = string.Empty;
        public bool IsInsert = false;
        public bool IsInsertInto = false;
        public bool IsSelect = false;
        public bool IsUpdate = false;
        public bool IsDelete = false;
        public bool IsFrom = false;
        public bool IsGetCount = false;
        // bool IsAll = false;
        public bool IsTopN = false;
        public bool IsDistinct = false;
        public int TopN = -1;
        public List<string> FieldItems = new List<string>();

        void FormatSqlText(string sqlText)
        {
            string[] items = sqlText.Split(' ');
            foreach (string item in items)
            {
                switch (item.ToLower())
                {
                    case "insert":
                        IsInsert = true;
                        break;
                    case "into":
                        if (IsInsert)
                        {
                            IsInsertInto = true;
                        }
                        break;
                    case "select":
                        IsSelect = true;
                        break;
                    case "update":
                        IsUpdate = true;
                        break;
                    case "delete":
                        IsDelete = true;
                        break;
                    case "from":
                        IsFrom = true;
                        break;
                    case "count(*)":
                        IsGetCount = true;
                        break;
                    case "where":
                        Where = sqlText.Substring(sqlText.IndexOf(item) + item.Length + 1);
                        //�ý�������ˡ�
                        goto end;
                    case "top":
                        if (IsSelect && !IsFrom)
                        {
                            IsTopN = true;
                        }
                        break;
                    case "distinct":
                        if (IsSelect && !IsFrom)
                        {
                            IsDistinct = true;
                        }
                        break;
                    case "set":
                        if (IsUpdate && !string.IsNullOrEmpty(TableName) && FieldItems.Count == 0)
                        {
                            #region ����Update���ֶ���ֵ

                            int start = sqlText.IndexOf(item) + item.Length;
                            int end = sqlText.ToLower().IndexOf("where");
                            string itemText = sqlText.Substring(start, end == -1 ? sqlText.Length - start : end - start);
                            int quoteCount = 0, commaIndex = 0;

                            for (int i = 0; i < itemText.Length; i++)
                            {
                                if (i == itemText.Length - 1)
                                {
                                    string keyValue = itemText.Substring(commaIndex).Trim();
                                    if (!FieldItems.Contains(keyValue))
                                    {
                                        FieldItems.Add(keyValue);
                                    }
                                }
                                else
                                {
                                    switch (itemText[i])
                                    {
                                        case '\'':
                                            quoteCount++;
                                            break;
                                        case ',':
                                            if (quoteCount % 2 == 0)//˫����������ָ���
                                            {
                                                string keyValue = itemText.Substring(commaIndex, i - commaIndex).Trim();
                                                if (!FieldItems.Contains(keyValue))
                                                {
                                                    FieldItems.Add(keyValue);
                                                }
                                                commaIndex = i + 1;
                                            }
                                            break;

                                    }
                                }
                            }
                            #endregion
                        }
                        break;
                    default:
                        if (IsTopN && TopN == -1)
                        {
                            int.TryParse(item, out TopN);//��ѯTopN
                            IsTopN = false;//�ر�topN
                        }
                        else if ((IsFrom || IsUpdate || IsInsertInto) && string.IsNullOrEmpty(TableName))
                        {
                            TableName = item.Split('(')[0].Trim();//��ȡ������
                        }
                        else if (IsSelect && !IsFrom)//��ȡ��ѯ���м�������
                        {
                            #region Select �ֶ��Ѽ�
                            switch (item)
                            {
                                case "*":
                                case "count(*)":
                                case "distinct":
                                    break;
                                default:
                                    fieldText.Append(item + " ");
                                    break;
                            }

                            #endregion
                        }
                        else if (IsInsertInto && !string.IsNullOrEmpty(TableName) && FieldItems.Count == 0)
                        {
                            #region ����Insert Into���ֶ���ֵ

                            int start = sqlText.IndexOf(TableName) + TableName.Length;
                            int end = sqlText.IndexOf("values", start, StringComparison.OrdinalIgnoreCase);
                            string keys = sqlText.Substring(start, end - start).Trim();
                            string[] keyItems = keys.Substring(1, keys.Length - 2).Split(',');//ȥ�����������ٰ����ŷָ���

                            string values = sqlText.Substring(end + 6).Trim();
                            values = values.Substring(1, values.Length - 2);//ȥ����������
                            int quoteCount = 0, commaIndex = 0, valueIndex = 0;

                            for (int i = 0; i < values.Length; i++)
                            {
                                if (valueIndex >= keyItems.Length)
                                {
                                    break;
                                }
                                if (i == values.Length - 1)
                                {
                                    string value = values.Substring(commaIndex).Trim();
                                    keyItems[valueIndex] += "=" + value;
                                }
                                else
                                {
                                    switch (values[i])
                                    {
                                        case '\'':
                                            quoteCount++;
                                            break;
                                        case ',':
                                            if (quoteCount % 2 == 0)//˫����������ָ���
                                            {
                                                string value = values.Substring(commaIndex, i - commaIndex).Trim();
                                                keyItems[valueIndex] += "=" + value;
                                                commaIndex = i + 1;
                                                valueIndex++;
                                            }
                                            break;

                                    }
                                }
                            }
                            FieldItems.AddRange(keyItems);

                            #endregion
                        }
                        break;
                }
            }
        end:
            #region Select �ֶν���
            if (fieldText.Length > 0)
            {
                string[] fields = fieldText.ToString().Split(',');
                FieldItems.AddRange(fields);
                fieldText.Length = 0;
            }
            #endregion
        }
        private StringBuilder fieldText = new StringBuilder();
        
    }
}
