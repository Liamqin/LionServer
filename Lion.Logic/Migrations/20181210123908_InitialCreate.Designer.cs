﻿// <auto-generated />
using System;
using Lion.Logic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lion.Logic.Migrations
{
    [DbContext(typeof(MySqlContext))]
    [Migration("20181210123908_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Lion.Model.Sys_Company", b =>
                {
                    b.Property<string>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Account")
                        .HasMaxLength(50);

                    b.Property<string>("Address")
                        .HasMaxLength(100);

                    b.Property<string>("ComName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Email")
                        .HasMaxLength(100);

                    b.Property<int>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("Person")
                        .HasMaxLength(10);

                    b.Property<string>("Phone")
                        .HasMaxLength(10);

                    b.Property<string>("Reamrk")
                        .HasMaxLength(1000);

                    b.Property<string>("SortName")
                        .HasMaxLength(50);

                    b.Property<string>("Tell")
                        .HasMaxLength(10);

                    b.Property<string>("Url")
                        .HasMaxLength(100);

                    b.HasKey("Guid");

                    b.ToTable("Sys_Companies");
                });

            modelBuilder.Entity("Lion.Model.Sys_DataBase", b =>
                {
                    b.Property<string>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("ConnonStr")
                        .HasMaxLength(50);

                    b.Property<string>("DataName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Reamrk")
                        .HasMaxLength(1000);

                    b.HasKey("Guid");

                    b.ToTable("Sys_DataBases");
                });

            modelBuilder.Entity("Lion.Model.Sys_Depart", b =>
                {
                    b.Property<string>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Comid")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Pid")
                        .HasMaxLength(50);

                    b.Property<string>("Remark")
                        .HasMaxLength(500);

                    b.Property<int>("Sort")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<DateTime>("Ts")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Guid");

                    b.ToTable("Sys_Departs");
                });

            modelBuilder.Entity("Lion.Model.Sys_Menu", b =>
                {
                    b.Property<string>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Comid")
                        .HasMaxLength(50);

                    b.Property<string>("Icon")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Pid")
                        .HasMaxLength(50);

                    b.Property<string>("Remark")
                        .HasMaxLength(500);

                    b.Property<int>("Sort")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<DateTime>("Ts")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.HasKey("Guid");

                    b.ToTable("Sys_Menus");
                });

            modelBuilder.Entity("Lion.Model.Sys_Role", b =>
                {
                    b.Property<string>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Comid")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.Property<string>("Remark")
                        .HasMaxLength(500);

                    b.Property<DateTime>("Ts")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Guid");

                    b.ToTable("Sys_Roles");
                });

            modelBuilder.Entity("Lion.Model.Sys_RoleMenu", b =>
                {
                    b.Property<string>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Menuid");

                    b.Property<string>("Roleid")
                        .HasMaxLength(50);

                    b.Property<DateTime>("Ts")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Guid");

                    b.ToTable("Sys_RoleMenus");
                });

            modelBuilder.Entity("Lion.Model.Sys_User", b =>
                {
                    b.Property<string>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Comid")
                        .HasMaxLength(50);

                    b.Property<string>("Departid")
                        .HasMaxLength(50);

                    b.Property<string>("Logincode")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Phone")
                        .HasMaxLength(20);

                    b.Property<string>("Pwd")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Remark")
                        .HasMaxLength(500);

                    b.Property<string>("Roleid")
                        .HasMaxLength(50);

                    b.Property<int>("State")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<DateTime>("Ts")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Username")
                        .HasMaxLength(50);

                    b.HasKey("Guid");

                    b.ToTable("Sys_Users");
                });

            modelBuilder.Entity("Lion.Model.Sys_UserDepart", b =>
                {
                    b.Property<string>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<string>("Departid")
                        .HasMaxLength(50);

                    b.Property<DateTime>("Ts")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Userid")
                        .HasMaxLength(50);

                    b.HasKey("Guid");

                    b.ToTable("Sys_UserDeparts");
                });
#pragma warning restore 612, 618
        }
    }
}
