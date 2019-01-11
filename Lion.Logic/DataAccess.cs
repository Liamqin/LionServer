#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：切换数据库类，根据配置文件利用反射切换访问数据库的方法
* 简单工厂的方法，利用反射去代替switch分支
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System.Reflection;
using Lion.ICore;


namespace Lion.Logic
{
    public static class DataAccess
    {
        private static readonly string AsssemblyName = "Lion.Logic";
        public static string db = "Mysql";//默认mysql

        public static IAPIBase DataBase()
        {
            string className = AsssemblyName + "." + db + "Base";
            return (IAPIBase)Assembly.Load(AsssemblyName).CreateInstance(className);
        }
    }
}
