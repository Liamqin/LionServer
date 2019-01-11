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
    /// 部门信息模型
    /// </summary>
    public class Sys_Depart
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key,MaxLength(50)]
        public string Guid { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        [MaxLength(50)]
        public string Comid { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [Required, MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        [MaxLength(50)]
        public string Pid { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DefaultValue(0)]
        public int Sort { get; set; }
        [MaxLength(500)]
        public string Remark { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Ts { get; set; }
    }
}
