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
using System.Text;

namespace Lion.Model
{
    public class Sys_Company
    {
        /// <summary>
        /// 主键GuID
        /// </summary>
        [Key,MaxLength(50)]
        public string Guid { get; set; }
        /// <summary>
        /// 公司唯一识别号
        /// </summary>
        [MaxLength(50)]
        public string Account { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [Required,MaxLength(100)]
        public string ComName { get; set; }
        /// <summary>
        /// 公司简称
        /// </summary>
        [MaxLength(50)]
        public string SortName { get; set; }
        /// <summary>
        /// 公司地址
        /// </summary>
        [MaxLength(100)]
        public string Address { get; set; }
        /// <summary>
        /// 公司联系人(法人)
        /// </summary>
        [MaxLength(10)]
        public string Person { get; set; }
        /// <summary>
        /// 公司联系人电话
        /// </summary>
        [MaxLength(10)]
        public string Phone { get; set; }
        /// <summary>
        /// 公司电话（传真）
        /// </summary>
        [MaxLength(10)]
        public string Tell { get; set; }
        /// <summary>
        /// 公司邮箱
        /// </summary>
        [MaxLength(100)]
        public string Email { get; set; }
        /// <summary>
        /// 公司网址
        /// </summary>
        [MaxLength(100)]
        public string Url { get; set; }
        /// <summary>
        /// 状态 是否激活
        /// </summary>
        [DefaultValue(0)]
        public int IsActive { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(1000)]
        public string Reamrk { get; set; }
    }
}
