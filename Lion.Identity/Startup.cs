#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：
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
using Lion.Identity.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lion.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //开启跨域
            services.AddCors();
            //依赖注入 数据库上下文
            //services.AddDbContext<MysqlContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlConnection")));
            //---------begin 获取appsettings 配置文件信息
            //services
            //    .Configure<Settings>(Configuration.GetSection("Settings"))
            //    .AddMvc();
            //---------end

            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential()
            //    .AddTestUsers(InMemoryConfiguration.Users().ToList())
            //    .AddInMemoryClients(InMemoryConfiguration.Clients())
            //    .AddInMemoryApiResources(InMemoryConfiguration.ApiResources());


            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResourceResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>() // 配置验证用户信息
                .AddProfileService<ProfileService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //设置跨域范围， 允许所有IP Heard 和Method 附带缓存（暂无用）
            app.UseCors(builder =>
                 builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            // 注入IdentityServer服务
            app.UseIdentityServer();
            #region cyq.data core 相关
            app.UseHttpsRedirection();
            //cyq.data引用必加项，必须在UseHttpsRedirection下方
            app.UseHttpContext();
            #endregion
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
