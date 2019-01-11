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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Lion.Model
{
    /// <summary>
    /// 文件信息模型
    /// </summary>
    public class File
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
        /// 名称
        /// </summary>
        [Required, MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        [MaxLength(10)]
        public string Length { get; set; }
        /// <summary>
        /// 物理路径
        /// </summary>
        [Required, MaxLength(500)]
        public string Url { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        [MaxLength(10)]
        public string Ext { get; set; }
        /// <summary>
        /// 来源id
        /// </summary>
        [MaxLength(50)]
        public string Fromid { get; set; }
        [MaxLength(500)]
        public string Remark { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Ts { get; set; }
    }
}
