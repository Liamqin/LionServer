#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：基础操作控制器，主要包括通用的增删改查
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using Lion.ICore;
using Lion.Logic;
using Lion.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lion.API.Controllers
{
    /// <summary>
    /// 基础操作控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class APIBaseController : ControllerBase
    {
        IAPIBase api = null;
        /// <summary>
        /// 构造函数获取配置参数
        /// </summary>
        public APIBaseController(IOptions<Setting> set)
        {
            DataAccess.db = set.Value.DataBase;
            api = DataAccess.DataBase();
        }
        /// <summary>
        /// 数据查询操作
        /// </summary>
        /// <remarks>
        /// 例 api/ApiUnified/Search
        /// {
        /// "table":"aaa"
        /// ,"para":"*"
        /// ,"pageindex":1
        /// ,"pagesize":10
        /// ,"字段名":"内容（模糊查询，逗号隔开为单字段多内容查询）"
        /// }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost("Search")]
        public IActionResult Serch(dynamic dy)
        {
            int rowCount = 0;
            var result = api.Search(dy, out rowCount);
            if(rowCount==0)
            {
                return Ok(Res.Fail("暂无查询结果"));
            }
            else
            {
                return Ok(Res.Success(result, rowCount));
            }
        }

        /// <summary>
        /// 数据保存或更新操作
        /// </summary>
        /// <remarks>
        /// 例：api/ApiUnified/Save
        /// {
        /// "table":"aaa"
        /// ,"字段名":"内容"
        /// ,"id":"主键（空为新增）"
        /// }
        /// 注：新增时必填字段不能为空，必填字段详见下方Model
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost("Save")]
        public IActionResult Save(dynamic dy)
        {
            if(api.Save(dy))
            {
                return Ok(Res.Success("操作成功！"));
            }
            else
            {
                return Ok(Res.Fail("操作失败！"));
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <remarks>
        /// 例：api/ApiUnitfied/Del
        /// {
        /// "table":"aaa"
        /// ,"guids":"1,2,3"
        /// }
        /// </remarks>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost("Del")]
        public IActionResult Del(dynamic dy)
        {
            if (api.Del(dy))
            {
                return Ok(Res.Success("操作成功！"));
            }
            else
            {
                return Ok(Res.Fail("操作失败！"));
            }
        }
    }
}