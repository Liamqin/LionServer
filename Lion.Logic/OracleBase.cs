#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：支持Oracle数据库的通用的增删改查
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
    public  class OracleBase:IAPIBase
    {
        /// <summary>
        /// 获取信息列表
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public object Search(dynamic dy, out int rowCount)
        {
            DataTable dt = null;
            try
            {
                string v_table = Convert.ToString(dy.table);// 获取要操作的表明
                int pageindex = Convert.ToInt32(Convert.ToString(dy.pageindex));
                int pagesize = Convert.ToInt32(Convert.ToString(dy.pagesize));
                string where = "1=1";
                //根据表明获取表结构和
                string table = @"SELECT lower(t.COLUMN_NAME),lower(t.DATA_TYPE) FROM USER_TAB_COLUMNS t where t.TABLE_NAME='" + v_table.ToUpper() + "'";
                MAction ac = new MAction(table);
                MDataTable mdt = ac.Select();
                if (mdt != null && mdt.Rows.Count > 0)
                {
                    for (int i = 0; i < mdt.Rows.Count; i++)
                    {
                        if (dy.Property(mdt.Rows[i][0].ToString()) != null)
                        {
                            string data = Convert.ToString(dy.Property(mdt.Rows[i][0].ToString()).Value);
                            List<string> queryDataList = data.Split(',').ToList().Where(s => !string.IsNullOrEmpty(s)).ToList();
                            if (mdt.Rows[i][1].ToString() == "date")
                            {
                                where += @" and to_char(" + mdt.Rows[i][0] + ",'yyyy-MM-dd') like '%" + Convert.ToString(dy.Property(mdt.Rows[i][0].ToString()).Value) + "%'";
                            }
                            else if (mdt.Rows[i][1].ToString() == "number")
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
                using (MAction action = new MAction(v_table))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dy.para)))
                    {
                        action.SetSelectColumns(Convert.ToString(dy.para));
                    }
                    dt = action.Select(pageindex, pagesize, where, out rowCount).ToDataTable();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogToTxt(ex.Message, LogType.Error);
                rowCount = 0;
                throw new Exception(ex.Message);
            }
            return dt;
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public bool Save(dynamic dy)
        {
            bool result = false;
            try
            {
                string v_table = Convert.ToString(dy.table);
                //获取主键
                string guid = string.Empty;
                string table_guid = @"select   column_name   from   user_cons_columns   
                                      where   constraint_name   =   (select   constraint_name   from   user_constraints   
                                      where   table_name   =   '" + v_table.ToUpper() + "'  and   constraint_type   ='P')";
                using (MAction action_guid = new MAction(table_guid))
                {
                    MDataTable mdt_guid = action_guid.Select();
                    if (mdt_guid != null && mdt_guid.Rows.Count > 0)
                    {
                        guid = mdt_guid.Rows[0][0].ToString().ToLower();
                    }
                }
                //目前支针对单个主键的

                string table = @"SELECT lower(t.COLUMN_NAME) FROM USER_TAB_COLUMNS t where t.TABLE_NAME='" + v_table.ToUpper() + "'";
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
                            result = act.Update(guid + "='" + Convert.ToString(dy.Property(guid).Value) + "'");
                        }
                        else
                        {
                            act.Set(guid, Guid.NewGuid().ToString("N").ToUpper());
                            result = act.Insert();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogToTxt(ex.Message, LogType.Error);
                result = false;
                throw new Exception(ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public bool Del(dynamic dy)
        {
            bool result = false;
            try
            {
                string v_table = Convert.ToString(dy.table);

                //获取主键
                string guid = string.Empty;
                string table_guid = @"select   column_name   from   user_cons_columns   
                                      where   constraint_name   =   (select   constraint_name   from   user_constraints   
                                      where   table_name   =   '" + v_table.ToUpper() + "'  and   constraint_type   ='P')";
                using (MAction action_guid = new MAction(table_guid))
                {
                    MDataTable mdt_guid = action_guid.Select();
                    if (mdt_guid != null && mdt_guid.Rows.Count > 0)
                    {
                        guid = mdt_guid.Rows[0][0].ToString();
                    }
                }
                string ids = Convert.ToString(dy.guids);
                ids = ids.Replace(",", "','");
                using (MAction action = new MAction(v_table))
                {
                    action.BeginTransation();
                    result = action.Delete("" + guid + " in ('" + ids + "')");
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
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}
