using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Lion.Logic;
using Lion.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Lion.API
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            //关闭全局缓存

            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //开启跨域
            services.AddCors();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //---------begin 获取appsettings 配置文件信息
            services
                .Configure<Setting>(Configuration.GetSection("Settings"))
                .AddMvc();
            //---------end

            //依赖注入 数据库上下文
            services.AddDbContext<MySqlContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlConn")));
            //-------begin-----Identity server4 验证服务 
            services.AddAuthentication("Bearer")
               .AddIdentityServerAuthentication(options =>
               {
                   options.RequireHttpsMetadata = false;
                   options.Authority = Configuration["Authority"];// "http://47.93.59.199:5001";
                   options.ApiName = Configuration["ApiName"]; //"socialnetwork";// 需要保持和IdentityServer4服务ApiResource一样
                   options.SaveToken = true;

               });
            //------end-------
            //------begin---------- Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                //swashbuckle5.0预览版  暂未调通 后续完善
                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                //// Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                //c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                //{
                //    Type = SecuritySchemeType.OAuth2,
                //    Flows = new OpenApiOAuthFlows
                //    {
                //        Implicit = new OpenApiOAuthFlow
                //        {
                //            AuthorizationUrl = new Uri("/auth-server/connect/authorize", UriKind.Relative),
                //            Scopes = new Dictionary<string, string>
                //            {
                //                { "readAccess", "Access read operations" },
                //                { "writeAccess", "Access write operations" }
                //            }
                //        }
                //    }
                //});
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                //        },
                //        new[] { "readAccess", "writeAccess" }
                //    }
                //});

                //// Assign scope requirements to operations based on AuthorizeAttribute
                // c.OperationFilter<HttpHeaderOperation>();

                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                //Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
            });

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            //});

            //------end-----------
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpContextAccessor();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //异常信息页
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            #region cors 跨域相关
            //设置跨域范围  这里是允许所有跨域
            app.UseCors(builder =>
                 builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            #endregion

            app.UseHttpsRedirection();
            #region cyq.data core 相关
            //cyq.data引用必加项，必须在UseHttpsRedirection下方
            app.UseHttpContext();
            #endregion

            #region Identity Server4相关
            //添加中间件  Identity Server4 验证
            app.UseAuthentication();
            #endregion

            #region swagger 相关
            // Enable middleware to serve generated Swagger as a JSON endpoint.  swagger接口文档自动生成中间件
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            #endregion

            app.UseMvc();
        }
    }
}
