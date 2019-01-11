#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-16
* 文件描述：部门管理接口
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lion.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lion.API.Controllers
{
    /// <summary>
    /// 部门管理接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DepartsController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DepartTree(string comid, string userid)
        {
            var result = DepartLogic.DepartTree(comid, userid);
            if (result == null)
            {
                return Ok(Res.Fail("暂无数据"));
            }
            else
            {
                return Ok(Res.Success(result));
            }
        }
    }
}