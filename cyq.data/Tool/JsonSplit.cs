using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CYQ.Data.Tool
{
    /// <summary>
    /// �ָ�Json�ַ���Ϊ�ֵ伯�ϡ�
    /// </summary>
    internal partial class JsonSplit
    {
        internal static bool IsJson(string json)
        {
            int errIndex;
            return IsJson(json, out errIndex);
        }
        internal static bool IsJson(string json, out int errIndex)
        {
            errIndex = 0;

            if (string.IsNullOrEmpty(json) || json.Length < 2 ||
                ((json[0] != '{' && json[json.Length - 1] != '}') && (json[0] != '[' && json[json.Length - 1] != ']')))
            {
                return false;
            }
            CharState cs = new CharState();
            char c;
            for (int i = 0; i < json.Length; i++)
            {
                c = json[i];
                if (SetCharState(c, ref cs) && cs.childrenStart)//���ùؼ�����״̬��
                {
                    string item = json.Substring(i);
                    int err;
                    int length = GetValueLength(item, true, out err);
                    cs.childrenStart = false;
                    if (err > 0)
                    {
                        errIndex = i + err;
                        return false;
                    }
                    i = i + length - 1;
                }
                if (cs.isError)
                {
                    errIndex = i;
                    return false;
                }
            }

            return !cs.arrayStart && !cs.jsonStart; //ֻҪ���������رգ���ʧ��
        }

        /// <summary>
        /// ����Json
        /// </summary>
        /// <param name="json"></param>
        /// <param name="op">����NO�������������ת�����Ĭ���ǣ�YES</param>
        /// <returns></returns>
        internal static List<Dictionary<string, string>> Split(string json)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            if (!string.IsNullOrEmpty(json))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string key = string.Empty;
                StringBuilder value = new StringBuilder();
                CharState cs = new CharState();
                try
                {
                    #region �����߼�
                    char c;
                    for (int i = 0; i < json.Length; i++)
                    {
                        c = json[i];
                        if (!SetCharState(c, ref cs))//���ùؼ�����״̬��
                        {
                            if (cs.jsonStart)//Json�����С�����
                            {
                                if (cs.keyStart > 0)
                                {
                                    key += c;
                                }
                                else if (cs.valueStart > 0)
                                {
                                    value.Append(c);
                                    //value += c;
                                }
                            }
                            else if (!cs.arrayStart)//json�������ֲ������飬���˳���
                            {
                                break;
                            }
                        }
                        else if (cs.childrenStart)//�����ַ���ֵ״̬�¡�
                        {
                            string item = json.Substring(i);
                            int temp;
                            int length = GetValueLength(item, false, out temp);
                            //value = item.Substring(0, length);
                            value.Length = 0;
                            value.Append(item.Substring(0, length));
                            cs.childrenStart = false;
                            cs.valueStart = 0;
                            //cs.state = 0;
                            cs.setDicValue = true;
                            i = i + length - 1;
                        }
                        if (cs.setDicValue)//���ü�ֵ�ԡ�
                        {
                            if (!string.IsNullOrEmpty(key) && !dic.ContainsKey(key))
                            {
                                //if (value != string.Empty)
                                //{
                                string val = value.ToString();
                                bool isNull = json[i - 5] == ':' && json[i] != '"' && value.Length == 4 && val == "null";
                                if (isNull)
                                {
                                    val = "";
                                }

                                dic.Add(key, val);

                                //}
                            }
                            cs.setDicValue = false;
                            key = string.Empty;
                            value.Length = 0;
                        }

                        if (!cs.jsonStart && dic.Count > 0)
                        {
                            result.Add(dic);
                            if (cs.arrayStart)//�������顣
                            {
                                dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception err)
                {
                    Log.WriteLogToTxt(err);
                }
                finally
                {
                    key = null;
                    value.Length = 0;
                    value.Capacity = 16;
                    value = null;
                }
            }
            return result;
        }
        /// <summary>
        /// ��ȡֵ�ĳ��ȣ���JsonֵǶ����"{"��"["��ͷʱ��
        /// </summary>
        private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
        {
            errIndex = 0;
            int len = 0;
            if (!string.IsNullOrEmpty(json))
            {
                CharState cs = new CharState();
                char c;
                for (int i = 0; i < json.Length; i++)
                {
                    c = json[i];
                    if (!SetCharState(c, ref cs))//���ùؼ�����״̬��
                    {
                        if (!cs.jsonStart && !cs.arrayStart)//json�������ֲ������飬���˳���
                        {
                            break;
                        }
                    }
                    else if (cs.childrenStart)//�����ַ���ֵ״̬�¡�
                    {
                        int length = GetValueLength(json.Substring(i), breakOnErr, out errIndex);//�ݹ���ֵ������һ�����ȡ�����
                        cs.childrenStart = false;
                        cs.valueStart = 0;
                        //cs.state = 0;
                        i = i + length - 1;
                    }
                    if (breakOnErr && cs.isError)
                    {
                        errIndex = i;
                        return i;
                    }
                    if (!cs.jsonStart && !cs.arrayStart)//��¼��ǰ����λ�á�
                    {
                        len = i + 1;//���ȱ�����+1
                        break;
                    }
                }
            }
            return len;
        }
        /// <summary>
        /// �ַ�״̬
        /// </summary>
        private class CharState
        {
            internal bool jsonStart = false;//�� "{"��ʼ��...
            internal bool setDicValue = false;// ���������ֵ�ֵ�ˡ�
            internal bool escapeChar = false;//��"\"ת����ſ�ʼ��
            /// <summary>
            /// ���鿪ʼ������һ��ͷ���㡿��ֵǶ�׵��ԡ�childrenStart������ʶ��
            /// </summary>
            internal bool arrayStart = false;//��"[" ���ſ�ʼ��
            internal bool childrenStart = false;//�Ӽ�Ƕ�׿�ʼ�ˡ�
            /// <summary>
            /// ��-1 δ��ʼ������0 ȡ�����С�����1 ȡֵ�С�
            /// </summary>
            internal int state = -1;

            /// <summary>
            /// ��-2 �ѽ�������-1 δ��ʼ������0 δ��ʼ����1 �����ſ�ʼ����2 �����ſ�ʼ����3 ˫���ſ�ʼ��
            /// </summary>
            internal int keyStart = -1;
            /// <summary>
            /// ��-2 �ѽ�������-1 δ��ʼ������0 δ��ʼ����1 �����ſ�ʼ����2 �����ſ�ʼ����3 ˫���ſ�ʼ��
            /// </summary>
            internal int valueStart = -1;

            internal bool isError = false;//�Ƿ��﷨����

            internal void CheckIsError(char c)//ֻ����һ��������ΪGetLength��ݹ鵽ÿһ�������
            {
                switch (c)
                {
                    case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                        isError = jsonStart && state == 0;//�ظ���ʼ���� ͬʱ����ֵ����
                        break;
                    case '}':
                        isError = !jsonStart || (keyStart > 0 && state == 0);//�ظ��������� ���� ��ǰ������
                        break;
                    case '[':
                        isError = arrayStart && state == 0;//�ظ���ʼ����
                        break;
                    case ']':
                        isError = !arrayStart || (state == 1 && valueStart == 0);//�ظ���ʼ����[{},]1,0  ������[111,222] 1,1 [111,"22"] 1,-2 
                        break;
                    case '"':
                        isError = !jsonStart && !arrayStart;//δ��ʼJson��ͬʱҲδ��ʼ���顣
                        break;
                    case '\'':
                        isError = !jsonStart && !arrayStart;//δ��ʼJson
                        break;
                    case ':':
                        isError = (!jsonStart && !arrayStart) || (jsonStart && keyStart < 2 && valueStart < 2 && state == 1);//δ��ʼJson ͬʱ ֻ�ܴ�����ȡֵ֮ǰ��
                        break;
                    case ',':
                        isError = (!jsonStart && !arrayStart)
                            || (!jsonStart && arrayStart && state == -1) //[,111]
                            || (jsonStart && keyStart < 2 && valueStart < 2 && state == 0);//δ��ʼJson ͬʱ ֻ�ܴ�����ȡֵ֮��
                        break;
                    default: //ֵ��ͷ����
                        isError = (!jsonStart && !arrayStart) || (keyStart == 0 && valueStart == 0 && state == 0);//
                        if (!isError && keyStart < 2)
                        {
                            if ((jsonStart && !arrayStart) && state != 1)
                            {
                                //�������ſ�ͷ�ģ�ֻ������ĸ {aaa:1}
                                isError = c < 65 || (c > 90 && c < 97) || c > 122;
                            }
                            else if (!jsonStart && arrayStart && valueStart < 2)//
                            {
                                //�������ſ�ͷ�ģ�ֻ��������[1]
                                isError = c < 48 || c > 57;

                            }
                        }
                        break;
                }
                //if (isError)
                //{

                //}
            }
        }
        /// <summary>
        /// �����ַ�״̬(����true��Ϊ�ؼ��ʣ�����false��Ϊ��ͨ�ַ�����
        /// </summary>
        private static bool SetCharState(char c, ref CharState cs)
        {
            switch (c)
            {
                case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                    #region ������
                    if (cs.keyStart <= 0 && cs.valueStart <= 0)
                    {
                        cs.CheckIsError(c);
                        if (cs.jsonStart && cs.state == 1)
                        {
                            cs.valueStart = 0;
                            cs.childrenStart = true;
                        }
                        else
                        {
                            cs.state = 0;
                        }
                        cs.jsonStart = true;//��ʼ��
                        return true;
                    }
                    #endregion
                    break;
                case '}':
                    #region �����Ž���
                    if (cs.keyStart <= 0 && cs.valueStart < 2)
                    {
                        cs.CheckIsError(c);
                        if (cs.jsonStart)
                        {
                            cs.jsonStart = false;//����������
                            cs.valueStart = -1;
                            cs.state = 0;
                            cs.setDicValue = true;
                        }
                        return true;
                    }
                    // cs.isError = !cs.jsonStart && cs.state == 0;
                    #endregion
                    break;
                case '[':
                    #region �����ſ�ʼ
                    if (!cs.jsonStart)
                    {
                        cs.CheckIsError(c);
                        cs.arrayStart = true;
                        return true;
                    }
                    else if (cs.jsonStart && cs.state == 1 && cs.valueStart < 2)
                    {
                        cs.CheckIsError(c);
                        //cs.valueStart = 1;
                        cs.childrenStart = true;
                        return true;
                    }
                    #endregion
                    break;
                case ']':
                    #region �����Ž���
                    if (!cs.jsonStart && (cs.keyStart <= 0 && cs.valueStart <= 0) || (cs.keyStart == -1 && cs.valueStart == 1))
                    {
                        cs.CheckIsError(c);
                        if (cs.arrayStart)// && !cs.childrenStart
                        {
                            cs.arrayStart = false;
                        }
                        return true;
                    }
                    #endregion
                    break;
                case '"':
                case '\'':
                    cs.CheckIsError(c);
                    #region ����
                    if (cs.jsonStart || cs.arrayStart)
                    {
                        if (!cs.jsonStart && cs.arrayStart)
                        {
                            cs.state = 1;//��������飬ֻ��ȡֵ��û��Key������ֱ������0
                        }
                        if (cs.state == 0)//key�׶�
                        {
                            cs.keyStart = (cs.keyStart <= 0 ? (c == '"' ? 3 : 2) : -2);
                            return true;
                        }
                        else if (cs.state == 1)//ֵ�׶�
                        {
                            if (cs.valueStart <= 0)
                            {
                                cs.valueStart = (c == '"' ? 3 : 2);
                                return true;
                            }
                            else if ((cs.valueStart == 2 && c == '\'') || (cs.valueStart == 3 && c == '"'))
                            {
                                if (!cs.escapeChar)
                                {
                                    cs.valueStart = -2;
                                    return true;
                                }
                                else
                                {
                                    cs.escapeChar = false;
                                }
                            }

                        }
                    }
                    #endregion
                    break;
                case ':':
                    cs.CheckIsError(c);
                    #region ð��
                    if (cs.jsonStart && cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 0)
                    {
                        cs.keyStart = 0;
                        cs.state = 1;
                        return true;
                    }
                    #endregion
                    break;
                case ',':
                    cs.CheckIsError(c);
                    #region ���� {"a": [11,"22", ], "Type": 2}
                    if (cs.jsonStart && cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1)
                    {
                        cs.state = 0;
                        cs.valueStart = 0;
                        cs.setDicValue = true;
                        return true;
                    }
                    else if (cs.arrayStart && !cs.jsonStart) //[a,b]  [",",33] [{},{}]
                    {
                        if ((cs.state == -1 && cs.valueStart == -1) || (cs.valueStart < 2 && cs.state == 1))
                        {
                            cs.valueStart = 0;
                            return true;
                        }
                    }
                    #endregion
                    break;
                case ' ':
                case '\r':
                case '\n':
                case '\t':
                    if (cs.jsonStart && cs.keyStart <= 0 && cs.valueStart <= 0)
                    {
                        return true;//�����ո�
                    }
                    break;
                default: //ֵ��ͷ����
                    cs.CheckIsError(c);
                    if (c == '\\') //ת�����
                    {
                        if (cs.escapeChar)
                        {
                            cs.escapeChar = false;
                        }
                        else
                        {
                            cs.escapeChar = true;
                            //return true;
                        }
                    }
                    else
                    {
                        cs.escapeChar = false;
                    }
                    if (cs.jsonStart)
                    {
                        if (cs.keyStart <= 0 && cs.state <= 0)
                        {
                            cs.keyStart = 1;//�����ŵ�
                        }
                        else if (cs.valueStart <= 0 && cs.state == 1)
                        {
                            cs.valueStart = 1;//�����ŵ�
                        }
                    }
                    else if (cs.arrayStart)
                    {
                        cs.state = 1;
                        if (cs.valueStart < 1)
                        {
                            cs.valueStart = 1;//�����ŵ�
                        }
                    }
                    break;
            }
            return false;
        }


    }
    internal partial class JsonSplit
    {
        /// <summary>
        /// ��json����ֳ��ַ���List
        /// </summary>
        /// <param name="jsonArray">["a,","bbb,,"]</param>
        /// <returns></returns>
        internal static List<string> SplitEscapeArray(string jsonArray)
        {
            if (!string.IsNullOrEmpty(jsonArray))
            {
                jsonArray = jsonArray.Trim(' ', '[', ']');//["a,","bbb,,"]
                if (jsonArray.Length > 0)
                {
                    List<string> list = new List<string>();
                    string[] items = jsonArray.Split(',');
                    string objStr = string.Empty;
                    foreach (string item in items)
                    {
                        if (objStr == string.Empty) { objStr = item; }
                        else { objStr += "," + item; }
                        char firstChar = objStr[0];
                        if (firstChar == '"' || firstChar == '\'')
                        {
                            //���˫���ŵ�����
                            if (GetCharCount(objStr, firstChar) % 2 == 0)//���ų�˫
                            {
                                list.Add(objStr.Trim(firstChar).Replace("\\" + firstChar, firstChar.ToString()));
                                objStr = string.Empty;
                            }
                        }
                        else
                        {
                            list.Add(item);
                            objStr = string.Empty;
                        }
                    }
                    return list;
                }


            }
            return null;
        }
        /// <summary>
        /// ��ȡ�ַ����ַ������ֵĴ���
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static int GetCharCount(string item, char c)
        {
            int num = 0;
            for (int i = 0; i < item.Length; i++)
            {
                if (item[i] == '\\')
                {
                    i++;
                }
                else if (item[i] == c)
                {
                    num++;
                }
            }
            return num;
        }
    }
}
