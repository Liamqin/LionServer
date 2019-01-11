using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lion.Logic.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sys_Companies",
                columns: table => new
                {
                    Guid = table.Column<string>(maxLength: 50, nullable: false),
                    Account = table.Column<string>(maxLength: 50, nullable: true),
                    ComName = table.Column<string>(maxLength: 100, nullable: false),
                    SortName = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(maxLength: 100, nullable: true),
                    Person = table.Column<string>(maxLength: 10, nullable: true),
                    Phone = table.Column<string>(maxLength: 10, nullable: true),
                    Tell = table.Column<string>(maxLength: 10, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    Url = table.Column<string>(maxLength: 100, nullable: true),
                    IsActive = table.Column<int>(nullable: false, defaultValue: 0),
                    Reamrk = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sys_Companies", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Sys_DataBases",
                columns: table => new
                {
                    Guid = table.Column<string>(maxLength: 50, nullable: false),
                    DataName = table.Column<string>(maxLength: 50, nullable: false),
                    ConnonStr = table.Column<string>(maxLength: 50, nullable: true),
                    Reamrk = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sys_DataBases", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Sys_Departs",
                columns: table => new
                {
                    Guid = table.Column<string>(maxLength: 50, nullable: false),
                    Comid = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Pid = table.Column<string>(maxLength: 50, nullable: true),
                    Sort = table.Column<int>(nullable: false, defaultValue: 0),
                    Remark = table.Column<string>(maxLength: 500, nullable: true),
                    Ts = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sys_Departs", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Sys_Menus",
                columns: table => new
                {
                    Guid = table.Column<string>(maxLength: 50, nullable: false),
                    Comid = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Url = table.Column<string>(maxLength: 500, nullable: true),
                    Icon = table.Column<string>(maxLength: 50, nullable: true),
                    Pid = table.Column<string>(maxLength: 50, nullable: true),
                    Sort = table.Column<int>(nullable: false, defaultValue: 0),
                    Remark = table.Column<string>(maxLength: 500, nullable: true),
                    Ts = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sys_Menus", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Sys_RoleMenus",
                columns: table => new
                {
                    Guid = table.Column<string>(maxLength: 50, nullable: false),
                    Roleid = table.Column<string>(maxLength: 50, nullable: true),
                    Menuid = table.Column<string>(nullable: true),
                    Ts = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sys_RoleMenus", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Sys_Roles",
                columns: table => new
                {
                    Guid = table.Column<string>(maxLength: 50, nullable: false),
                    Comid = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    Remark = table.Column<string>(maxLength: 500, nullable: true),
                    Ts = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sys_Roles", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Sys_UserDeparts",
                columns: table => new
                {
                    Guid = table.Column<string>(maxLength: 50, nullable: false),
                    Userid = table.Column<string>(maxLength: 50, nullable: true),
                    Departid = table.Column<string>(maxLength: 50, nullable: true),
                    Ts = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sys_UserDeparts", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Sys_Users",
                columns: table => new
                {
                    Guid = table.Column<string>(maxLength: 50, nullable: false),
                    Comid = table.Column<string>(maxLength: 50, nullable: true),
                    Logincode = table.Column<string>(maxLength: 50, nullable: false),
                    Pwd = table.Column<string>(maxLength: 50, nullable: false),
                    Username = table.Column<string>(maxLength: 50, nullable: true),
                    Roleid = table.Column<string>(maxLength: 50, nullable: true),
                    Departid = table.Column<string>(maxLength: 50, nullable: true),
                    State = table.Column<int>(nullable: false, defaultValue: 0),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    Remark = table.Column<string>(maxLength: 500, nullable: true),
                    Ts = table.Column<DateTime>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sys_Users", x => x.Guid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sys_Companies");

            migrationBuilder.DropTable(
                name: "Sys_DataBases");

            migrationBuilder.DropTable(
                name: "Sys_Departs");

            migrationBuilder.DropTable(
                name: "Sys_Menus");

            migrationBuilder.DropTable(
                name: "Sys_RoleMenus");

            migrationBuilder.DropTable(
                name: "Sys_Roles");

            migrationBuilder.DropTable(
                name: "Sys_UserDeparts");

            migrationBuilder.DropTable(
                name: "Sys_Users");
        }
    }
}
