#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-08
* 文件描述：菜单管理业务逻辑处理
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
    public class MenusLogic
    {
        /// <summary>
        /// 根据公司id获取菜单树结构，并根据roleid 获取菜单的选中状态
        /// </summary>
        /// <param name="comid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static object MenusTree(string comid, string roleid)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                using (MAction action = new MAction(TableNames.sys_menus))
                {
                    action.SetAopState(CYQ.Data.Aop.AopOp.OnlyOuter);
                    List<Sys_Menu> list_all = action.Select("comid = '" + comid + "' order by sort asc").ToList<Sys_Menu>();
                    action.ResetTable(TableNames.sys_rolemenus);
                    List<Sys_RoleMenu> list_rm = action.Select("roleid='" + roleid + "'").ToList<Sys_RoleMenu>();
                    list = GetNodes("0", list_all, list_rm);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLogToTxt(ex.Message, LogType.Error);
            }
            return list;
        }
        /// <summary>
        /// 根据角色id获取菜单tree
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public static object MenusTreeByRoleid(string roleid)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                string table = "select * from menus where id in(select menuid from rolemenus where roleid=" + roleid + ")";
               
                    using (MAction action = new MAction(table))
                    {
                        var menu = action.Select().ToList<Sys_Menu>();
                        if (menu != null)
                        {
                            list = GetNodes("0", menu);
                        }
                    }
            }
            catch (Exception ex)
            {
                Log.WriteLogToTxt(ex.Message, LogType.Error);
            }
            return list;
        }



        private static List<dynamic> GetNodes(string id, List<Sys_Menu> dt)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                string sql = string.Empty;

                var dts = dt.Where(a => a.Pid == id).ToList();
                if (dts.Count > 0)
                {
                    foreach (Sys_Menu dr in dts)
                    {

                        dynamic dy = new ExpandoObject();
                        dy.id = dr.Guid;
                        dy.title = dr.Name;
                        dy.path = dr.Url;
                        dy.pid = dr.Pid;
                        dy.icon = dr.Icon;
                        dy.sort = dr.Sort;
                        dy.remark = dr.Remark;
                        dy.ts = dr.Ts;
                        dy.open = false;
                        List<dynamic> children = GetNodes(dr.Guid, dt);
                        if (children.Count > 0)
                        {
                            dy.children = children;
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

        private static List<dynamic> GetNodes(string id, List<Sys_Menu> dt, List<Sys_RoleMenu> dt_menus)
        {
            List<dynamic> list = new List<dynamic>();
            try
            {
                string sql = string.Empty;

                var dts = dt.Where(a => a.Pid == id).ToList();
                if (dts.Count > 0)
                {
                    foreach (Sys_Menu dr in dts)
                    {

                        dynamic dy = new ExpandoObject();
                        dy.id = dr.Guid;
                        dy.title = dr.Name;
                        dy.path = dr.Url;
                        dy.pid = dr.Pid;
                        dy.icon = dr.Icon;
                        dy.sort = dr.Sort;
                        dy.remark = dr.Remark;
                        dy.ts = dr.Ts;
                        List<Sys_RoleMenu> list_menuids = dt_menus.Where(p => p.Menuid == dr.Guid).ToList();
                        if (list_menuids.Count > 0)
                        {
                            dy.isCheck = true;
                        }
                        else
                        {
                            dy.isCheck = false;
                        }
                        List<dynamic> children = GetNodes(dr.Guid, dt, dt_menus);
                        if (children.Count > 0)
                        {
                            dy.children = children;
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
