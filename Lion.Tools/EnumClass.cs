#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：常用枚举类型
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Lion.Tools
{
    public class EnumClass
    {

    }

    /// <summary>
    /// bool状态值
    /// </summary>
    public enum enIsBool
    {
        Yes=0,
        No=1
    }
    /// <summary>
    /// 性别类型
    /// </summary>
    public enum enSex
    {
        Man=0,
        Woman=1,
        No=2
    }
    /// <summary>
    /// 审核状态
    /// </summary>
    public enum enCheck
    {
        NoCheck=0,
        HasCheck=1
    }
    /// <summary>
    /// 排序类型
    /// </summary>
    public enum enSort
    {
        Up=0,
        Down=1
    }
}
