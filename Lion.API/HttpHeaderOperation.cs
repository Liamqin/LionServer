#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述：swagger ui的hearer 验证
******************************************************
* 修 改 人：张志钦
* 修改日期：2015-12-29
* 备注描述：升级swashbuckle 到最新稳定版4.0.0.1，并更新权限验证方法，更新使用TryGetMethodInfo
* 其实可以再简化代码，后期再进行优化，并保留原有接口方法的实现
*******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lion.API
{
    /// <summary>
    /// 控制器添加header 权限验证
    /// </summary>
    public class HttpHeaderOperation : IOperationFilter
    {
        //原有方法的实现
        //public void Apply(Operation operation, OperationFilterContext context)
        //{
        //    if (operation.Parameters == null)
        //    {
        //        operation.Parameters = new List<IParameter>();
        //    }
        //    var actionAttrs = context.ApiDescription.ActionAttributes();
        //    var isAuthorized = actionAttrs.Any(a => a.GetType() == typeof(AuthorizeAttribute));
        //    if (isAuthorized == false) //提供action都没有权限特性标记，检查控制器有没有
        //    {
        //        var controllerAttrs = context.ApiDescription.ControllerAttributes();

        //        isAuthorized = controllerAttrs.Any(a => a.GetType() == typeof(AuthorizeAttribute));
        //    }

        //    var isAllowAnonymous = actionAttrs.Any(a => a.GetType() == typeof(AllowAnonymousAttribute));

        //    if (isAuthorized && isAllowAnonymous == false)
        //    {
        //        operation.Parameters.Add(new NonBodyParameter()
        //        {
        //            Name = "Authorization",  //添加Authorization头部参数
        //            In = "header",
        //            Type = "string",
        //            Required = false
        //        });
        //    }
        //}
        /// <summary>
        /// 4.0接口方法的实现
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }
            bool ishas = context.ApiDescription.TryGetMethodInfo(out MethodInfo info);
            var actionAttrs = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            var isAuthorized = actionAttrs.MethodInfo.GetCustomAttributes(true).Any(a => a.GetType() == typeof(AuthorizeAttribute));
            if (isAuthorized == false) //提供action都没有权限特性标记，检查控制器有没有
            {
                var controllerAttrs = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;

                isAuthorized = controllerAttrs.ControllerTypeInfo.GetCustomAttributes(true).Any(a => a.GetType() == typeof(AuthorizeAttribute));
            }

            var isAllowAnonymous = actionAttrs.MethodInfo.GetCustomAttributes(true).Any(a => a.GetType() == typeof(AllowAnonymousAttribute));

            if (isAuthorized && isAllowAnonymous == false)
            {
                operation.Parameters.Add(new NonBodyParameter()
                {
                    Name = "Authorization",  //添加Authorization头部参数
                    In = "header",
                    Type = "string",
                    Required = false
                });
            }
        }

        //此方法为5.0预览版方法的实现 主要是符合标准话OpenAPI2,调试暂未通过，等稳定版出来后再做处理更新
        //public void Apply(OpenApiOperation operation, OperationFilterContext context)
        //{
        //    var requiredScopes = context.MethodInfo
        //         .GetCustomAttributes(true)
        //         .OfType<AuthorizeAttribute>()
        //         .Select(attr => attr.Policy)
        //         .Distinct();

        //    if (requiredScopes.Any())
        //    {
        //        //operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        //        //operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

        //        var oAuthScheme = new OpenApiSecurityScheme
        //        {
        //            Reference = new OpenApiReference {
        //                Type = ReferenceType.SecurityScheme,
        //                Id = "oauth2"

        //            }
        //        };

        //        operation.Security = new List<OpenApiSecurityRequirement>
        //        {
        //            new OpenApiSecurityRequirement
        //            {
        //                [ oAuthScheme ] = requiredScopes.ToList()
        //            }
        //        };
        //    }
        //}
    }
}
