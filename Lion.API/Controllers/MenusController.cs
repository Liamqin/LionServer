#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-16
* 文件描述：菜单管理接口
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
using Lion.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Lion.API.Controllers
{
    /// <summary>
    /// 菜单管理接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MenusController : ControllerBase
    {
        /// <summary>
        /// 根据公司id和roleid 获取菜单树
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult MenusTree(SerchModel model)
        {
            var result = MenusLogic.MenusTree(model.comid, model.roleid);
            if(result==null)
            {
                return Ok(Res.Fail("暂无数据"));
            }
            else
            {
                return Ok(result);
            }
            
        }

        /// <summary>
        /// 根据角色id获取Tree
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("RoleId")]
        public IActionResult GetMenu( )
        {
            var result = MenusLogic.MenusTreeByRoleid("");
            //return Ok(Res.LayuiTable(MenusLogic.MenusTree("1", "1")));
            if (result == null)
            {
                return Ok(Res.Fail("暂无数据"));
            }
            else
            {
                return Ok(Res.Success(MenusLogic.MenusTree("1", "1")));
            }
        }
    }
    /// <summary>
    /// 接口参数归类
    /// </summary>
    public class SerchModel
    {
        /// <summary>
        /// 公司id
        /// </summary>
        public string comid { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public string roleid { get; set; }
        /// <summary>
        /// 第几页
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 每页多少 默认10
        /// </summary>
        public int limit { get; set; }
    }
}