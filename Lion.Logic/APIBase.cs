#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：基础操作类，主要包括通用的增删改查
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using Lion.ICore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Lion.Logic
{
    /// <summary>
    /// 基础操作类
    /// </summary>
    public class APIBase
    {
        IAPIBase api = DataAccess.DataBase();
        /// <summary>
        /// 查询表数据包括视图
        /// </summary>
        /// <param name="dy"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public  object Search(dynamic dy,out int rowCount)
        {
            
            return api.Search(dy,out rowCount);
        }
        /// <summary>
        /// 保存数据信息
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public  bool Save(dynamic dy)
        {
            return api.Save(dy);
        }
        /// <summary>
        /// 删除数据信息
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public  bool Del(dynamic dy)
        {
            return api.Del(dy);
        }
    }
}
