#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：基础操作接口，主要包括通用的增删改查
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Lion.ICore
{
    public interface IAPIBase
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="dy"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        object Search(dynamic dy, out int rowCount);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        bool Save(dynamic dy);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        bool Del(dynamic dy);
    }
}
