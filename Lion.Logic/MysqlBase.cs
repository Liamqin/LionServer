#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：支持mysql数据库的通用的增删改查
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using CYQ.Data;
using CYQ.Data.Table;
using Lion.ICore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Lion.Logic
{
    public class MysqlBase:IAPIBase
    {
        public object Search(dynamic dy,out int rowCount)
        {
            try
            {
                string v_table = Convert.ToString(dy.table);// 获取要操作的表明
                int pageindex = Convert.ToInt32(Convert.ToString(dy.pageindex));
                int pagesize = Convert.ToInt32(Convert.ToString(dy.pagesize));
                string where = "1=1";
                //根据表名获取表结构
                string table = @"SELECT lower(t.COLUMN_NAME),lower(t.DATA_TYPE) FROM `information_schema`.`COLUMNS` t where t.TABLE_NAME='" + v_table.ToUpper() + "'";
                MAction ac = new MAction(table);
                MDataTable mdt = ac.Select();
                if (mdt != null && mdt.Rows.Count > 0)
                {
                    //循环表结构字段
                    for (int i = 0; i < mdt.Rows.Count; i++)
                    {
                        //从动态参数里获取是否包含字段有则添加到
                        if (dy.Property(mdt.Rows[i][0].ToString()) != null)
                        {
                            string data = Convert.ToString(dy.Property(mdt.Rows[i][0].ToString()).Value);
                            List<string> queryDataList = data.Split(',').ToList().Where(s => !string.IsNullOrEmpty(s)).ToList();
                            //日期类型
                            if (mdt.Rows[i][1].ToString() == "date")
                            {
                                where += @" and date_format(" + mdt.Rows[i][0] + ",'yyyy-MM-dd') like '%" + Convert.ToString(dy.Property(mdt.Rows[i][0].ToString()).Value) + "%'";
                            }
                            //数字类型  整型或者decimal 或者 double   float
                            else if (mdt.Rows[i][1].ToString().Contains("int") || mdt.Rows[i][1].ToString().Contains("decimal") || mdt.Rows[i][1].ToString().Contains("double") || mdt.Rows[i][1].ToString().Contains("float"))
                            {
                                if (queryDataList.Count > 0)
                                {
                                    where += @" and (";
                                    queryDataList.ForEach(q =>
                                    {
                                        where += "" + mdt.Rows[i][0] + "=" + q + " OR ";
                                    });
                                    if (where.EndsWith("OR ")) where = where.Remove(where.LastIndexOf("OR"), 2);
                                    where += ") ";
                                }
                            }
                            //字符串类型
                            else if (mdt.Rows[i][1].ToString().Contains("char"))
                            {

                                if (queryDataList.Count > 0)
                                {
                                    where += @" and (";
                                    queryDataList.ForEach(q =>
                                    {
                                        where += "" + mdt.Rows[i][0] + " like '%" + q + "%' OR ";
                                    });
                                    if (where.EndsWith("OR ")) where = where.Remove(where.LastIndexOf("OR"), 2);
                                    where += ") ";
                                }

                            }
                        }
                    }
                }
                rowCount = 0;
                using (MAction action = new MAction(v_table))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dy.para)))
                    {
                        action.SetSelectColumns(Convert.ToString(dy.para));
                    }
                    var list = action.Select(pageindex, pagesize, where, out rowCount);
                    return list.ToDataTable();
                }

            }
            catch (Exception ex)
            {
                rowCount = 0;
                return null;
            }
        }

        public bool Save(dynamic dy)
        {
            bool result = false;
            try
            {
                string v_table = Convert.ToString(dy.table);
                //获取主键
                string guid = string.Empty;
                string table_guid = string.Format(@" SELECT c.COLUMN_NAME  FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS t,INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS c where t.TABLE_NAME = c.TABLE_NAME
                                    and t.TABLE_NAME='{0}'  /*AND t.TABLE_SCHEMA = 'demodata'*/  AND t.CONSTRAINT_TYPE = 'PRIMARY KEY'", v_table.ToLower());
                using (MAction action_guid = new MAction(table_guid))
                {
                    MDataTable mdt_guid = action_guid.Select();
                    if (mdt_guid != null && mdt_guid.Rows.Count > 0)
                    {
                        guid = mdt_guid.Rows[0][0].ToString().ToLower();
                    }
                }
                //目前支针对单个主键的

                string table = @"SELECT lower(t.COLUMN_NAME),lower(t.DATA_TYPE) FROM `information_schema`.`COLUMNS` t where t.TABLE_NAME='" + v_table.ToUpper() + "'";
                MAction action = new MAction(table);
                MDataTable mdt = action.Select();
                if (mdt != null && mdt.Rows.Count > 0)
                {
                    using (MAction act = new MAction(v_table))
                    {
                        for (int i = 0; i < mdt.Rows.Count; i++)
                        {
                            if (dy.Property(mdt.Rows[i][0].ToString()) != null)
                            {
                                act.Set(mdt.Rows[i][0], Convert.ToString(dy.Property(mdt.Rows[i][0].ToString()).Value));
                            }
                            if (mdt.Rows[i][0].ToString().ToUpper() == "TS") act.Set("TS", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(dy.Property(guid))))
                        {
                            result = act.Update(guid + "=" + Convert.ToString(dy.Property(guid).Value) + "");
                        }
                        else
                        {
                            result = act.Insert(InsertOp.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogToTxt(ex.Message, LogType.Error);
                result = false;
            }
            finally
            {

            }
            return result;
        }

        public bool Del(dynamic dy)
        {
            bool result = false;
            try
            {
                string v_table = Convert.ToString(dy.table);

                //获取主键
                string guid = string.Empty;
                string table_guid = string.Format(@" SELECT c.COLUMN_NAME  FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS t,INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS c where t.TABLE_NAME = c.TABLE_NAME
                                    and t.TABLE_NAME='{0}'  /*AND t.TABLE_SCHEMA = 'demodata'*/  AND t.CONSTRAINT_TYPE = 'PRIMARY KEY'", v_table.ToLower());
                using (MAction action_guid = new MAction(table_guid))
                {
                    MDataTable mdt_guid = action_guid.Select();
                    if (mdt_guid != null && mdt_guid.Rows.Count > 0)
                    {
                        guid = mdt_guid.Rows[0][0].ToString();
                    }
                }
                string ids = Convert.ToString(dy.guids);
                //ids = ids.Replace(",", "','");
                using (MAction action = new MAction(v_table))
                {
                    action.BeginTransation();
                    result = action.Delete("" + guid + " in (" + ids + ")");
                    if (result)
                    {
                        action.EndTransation();
                    }
                    else
                    {
                        action.RollBack();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogToTxt(ex.Message, LogType.Error);
                result = false;
            }
            return result;
        }

    }
}
