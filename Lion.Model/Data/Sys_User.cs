#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-08
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Lion.Model
{
    /// <summary>
    /// 用户信息模型
    /// </summary>
    public class Sys_User
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key, MaxLength(50)]
        public string Guid { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        [MaxLength(50)]
        public string Comid { get; set; }
        /// <summary>
        /// 登陆Code
        /// </summary>
        [Required, MaxLength(50)]
        public string Logincode { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required, MaxLength(50)]
        public string Pwd { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [MaxLength(50)]
        public string Username { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        [MaxLength(50)]
        public string Roleid { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        [MaxLength(50)]
        public string Departid { get; set; }
        /// <summary>
        /// 状态(0,正常 1、禁用 2、删除)
        /// </summary>
        [DefaultValue(0)]
        public int State { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(500)]
        public string Remark { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Ts { get; set; }
    }
}
