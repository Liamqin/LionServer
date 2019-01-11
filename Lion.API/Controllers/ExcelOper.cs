#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：操作Excel类，导出导出通用方法封装
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lion.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lion.API.Controllers
{
    /// <summary>
    /// 导入Excel转换
    /// </summary>
    public class ExcelOper
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        /// <summary>
        /// 构造函数，获取路径参数
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public ExcelOper(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// 根据上传的Excel文件保存为DataTable并返回
        /// </summary>
        /// <param name="file">Http里的文件流</param>
        /// <returns></returns>
        public DataTable Import(IFormFile file)
        {
            DataTable dt = null;
            if (file == null)
            {
                return null;
            }
            if (file.Length == 0)
            {
                return null;
            }
            var acceptTypes = new[] { ".xls", ".xlsx"};
            if (acceptTypes.All(t => t != Path.GetExtension(file.FileName).ToLower()))
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(_hostingEnvironment.WebRootPath))
            {
                _hostingEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var uploadsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Uploads/Temp");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(stream);
            }
            NPOIHelper excelHelper = new NPOIHelper(filePath);
            dt = excelHelper.ExcelToDataTable("Sheet1", true);
            return dt;
        }
        /// <summary>
        /// 根据datatable转存为Excel并返回路径提供下载。
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="s_FileName">文件名</param>
        /// <returns>保存成功的路径</returns>
        public string Export(DataTable dt,string s_FileName)
        {
            string fileName = string.Empty;
            fileName = AppDomain.CurrentDomain.BaseDirectory + ("/Upload/Temp/Excel/") + s_FileName + ".xlsx";  //文件存放路径
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + ("/Upload/Temp/Excel/")))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + ("/Upload/Temp/Excel/"));
            }
            if (System.IO.File.Exists(fileName))                                //存在则删除
            {
                System.IO.File.Delete(fileName);
            }
            NPOIHelper np = new NPOIHelper("工资查询");
            int count= np.TableToExcel(dt, fileName);
            if(count<=0)
            {
                fileName = "";
            }
            return fileName;
        }
    }
}