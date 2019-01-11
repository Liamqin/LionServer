#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：EF Core 数据库上下文  目前暂不支持Oracle  针对Oracle 数据库使用Cyq.data
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using Lion.Model;
using Microsoft.EntityFrameworkCore;
using System;
/******************************
 *code first 帮助
 * 1、工具 - nuget包管理-- 程序包管理器控制台
 * 2、Add-Migration InitialCreate  创建迁移
 * 3、Update-Database 创建库
 * 4、更改 EF Core 模型后，数据库架构将不同步。为使其保持最新，请再添加一个迁移。 迁移名称的用途与版本控制系统中的提交消息类似。
 * Add-Migration AddProductReviews
 * 5、你可能在添加迁移后意识到需要在应用迁移前对 EF Core 模型作出其他更改。 要删除上个迁移，请使用如下命令。
 * PowerShell
 * Remove-Migration
 ****************************/
namespace Lion.Logic
{
    public class MySqlContext:DbContext
    {
        public MySqlContext(DbContextOptions<MySqlContext> options) : base(options)
        {

        }
        
        public DbSet<Sys_DataBase> Sys_DataBases { get; set; }
        public DbSet<Sys_Company> Sys_Companies { get; set; }
        public DbSet<Sys_Menu> Sys_Menus { get; set; }
        public DbSet<Sys_User> Sys_Users { get; set; }
        public DbSet<Sys_Role> Sys_Roles { get; set; }
        public DbSet<Sys_RoleMenu> Sys_RoleMenus { get; set; }
        public DbSet<Sys_Depart> Sys_Departs { get; set; }
        public DbSet<Sys_UserDepart> Sys_UserDeparts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Menu>()
            //    .Property(b => b.Ts)
            //    .HasDefaultValueSql("now()");
            modelBuilder.Entity<Sys_Company>().Property(b => b.IsActive).HasDefaultValue(0);
            modelBuilder.Entity<Sys_User>().Property(b => b.State).HasDefaultValue(0);
            modelBuilder.Entity<Sys_Menu>().Property(b => b.Sort).HasDefaultValue(0);
            modelBuilder.Entity<Sys_Depart>().Property(b => b.Sort).HasDefaultValue(0);
        }
    }
}
