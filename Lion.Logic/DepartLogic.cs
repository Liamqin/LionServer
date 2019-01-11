#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-08
* 文件描述：部门业务逻辑处理
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using CYQ.Data;
using Lion.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Lion.Logic
{
    public static class DepartLogic
    {
        /// <summary>
        /// 根据公司id获取部门树结构，并根据userid 获取部门的选中状态
        /// </summary>
        /// <param name="comid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static object DepartTree(string comid,string userid)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                using (MAction action = new MAction(TableNames.sys_departs))
                {
                    List<Sys_Depart> list_all = action.Select("comid='" + comid+"' order by sort asc").ToList<Sys_Depart>();
                    action.ResetTable(TableNames.sys_userdeparts);
                    List<Sys_UserDepart> list_ud = action.Select("userid='" + userid + "'").ToList<Sys_UserDepart>();
                    list = GetNodes("0", list_all, list_ud);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogToTxt(ex.Message, LogType.Error);
            }
            

            return list;
        }

        public static bool ChangedSort(string guid,int sort, int type)
        {
            bool result = false;
            try
            {

            }
            catch (Exception ex)
            {
                Log.WriteLogToTxt(ex.Message, LogType.Error);
                result = false;
            }
            return result;
        }


        private static List<dynamic> GetNodes(string id, List<Sys_Depart> dt, List<Sys_UserDepart> dt_departs)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                string sql = string.Empty;

                var dts = dt.Where(a => a.Pid == id).ToList();
                if (dts.Count > 0)
                {
                    foreach (Sys_Depart dr in dts)
                    {

                        dynamic dy = new ExpandoObject();
                        dy.Guid = dr.Guid;
                        dy.Name = dr.Name;
                        dy.Pid = dr.Pid;
                        dy.Sort = dr.Sort;
                        dy.Remark = dr.Remark;
                        dy.Ts = dr.Ts;
                        List<Sys_UserDepart> list_departids = dt_departs.Where(p => p.Userid == dr.Guid).ToList();
                        if (list_departids.Count > 0)
                        {
                            dy.IsCheck = true;
                        }
                        else
                        {
                            dy.IsCheck = false;
                        }
                        List<dynamic> children = GetNodes(dr.Guid, dt, dt_departs);
                        if (children.Count > 0)
                        {
                            dy.Children = children;
                        }
                        list.Add(dy);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
    }
}
