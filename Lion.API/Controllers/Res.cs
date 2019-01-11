#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：返回结果类型格式封装
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
namespace Lion.API.Controllers
{
    /// <summary>
    /// 统一返回格式类
    /// </summary>
    public static class Res
    {
        /// <summary>
        /// 返回成功数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Success(string value)
        {
            return new { success = true, msg = value };
        }
        /// <summary>
        /// 返回成功数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Success(object value)
        {
            return new { success = true, msg = "", rows = value };
        }
        /// <summary>
        /// 返回成功数据和数量
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="rowCount">数量</param>
        /// <returns></returns>
        public static object Success(object value, int rowCount)
        {
            return new { success = true, msg = "", rows = value, total = rowCount };
        }
        /// <summary>
        /// 返回表数据和数量
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="rowCount">数量</param>
        /// <returns></returns>
        public static object LayuiTable(object value, int rowCount)
        {
            return new { code = 0, msg = "", data = value, count = rowCount };
        }
        /// <summary>
        /// 返回表数据
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns></returns>
        public static object LayuiTable(object value)
        {
            return new { code = 0, msg = "", data = value };
        }
        /// <summary>
        /// 返回成功数据、数量、备注
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="remark">备注描述</param>
        /// <param name="rowCount">数量</param>
        /// <returns></returns>
        public static object Success(object value,string remark, int rowCount)
        {
            return new { success = true, msg = remark, rows = value, total = rowCount };
        }
        /// <summary>
        /// 返回失败结果
        /// </summary>
        /// <param name="value">失败原因</param>
        /// <returns></returns>
        public static object Fail(object value)
        {
            return new { success = false, msg = value, rows = "" };
        }
    }
}
