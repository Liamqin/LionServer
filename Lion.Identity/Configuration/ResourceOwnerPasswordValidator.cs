#region << 版 本 注 释 >>
/****************************************************
* 文 件 名： 
* Copyright(c) 郑州源伍通
* CLR 版本:1.0.1
* 创 建 人：张志钦
* 电子邮箱：
* 创建日期：2018-11-15
* 文件描述： 对请求token用户的身份进行验证，并返回身份信息
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
//using CYQ.Data;
//using CYQ.Data.Table;
using CSRedis;
using CYQ.Data;
using CYQ.Data.Cache;
using CYQ.Data.Table;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lion.Identity.Configuration
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private CSRedisClient csredis;
        public ResourceOwnerPasswordValidator(Microsoft.Extensions.Options.IOptions<Settings> set)
        {
            // 初始化 redis
            //csredis = new CSRedis.CSRedisClient(null, set.Value.CacheBase);
            //RedisHelper.Initialization(csredis);
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //根据context.UserName和context.Password与数据库的数据做校验，判断是否合法
            try
            {
                //首先获取缓存里数据，并根据缓存里的数据查询结果对比数据
                
                //如果缓存里不存在数据 则根据条件查询数据库，并把查询结果push到缓存里
                using (MAction action = new MAction("sys_users"))
                {
                    var list = action.Select(" 1=1 and logincode='" + context.UserName + "'");
                    if (list != null && list.Rows.Count > 0)
                    {

                        if (list.Rows[0]["pwd"].ToString() == context.Password)
                        {
                            context.Result = new GrantValidationResult(
                                 subject: context.UserName,
                                 authenticationMethod: "custom",
                                 claims: GetUserClaims(list));
                        }
                        else
                        {
                            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "密码错误");
                        }
                    }
                    else
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户名不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ex.Message);
            }
           
                //if (context.UserName == "wjk" && context.Password == "123")
                //{
                //    context.Result = new GrantValidationResult(
                //     subject: context.UserName,
                //     authenticationMethod: "custom",
                //     claims: GetUserClaims(1));
                //}
                //else if (context.UserName == "aa" && context.Password == "11")
                //{
                //    context.Result = new GrantValidationResult(
                //     subject: context.UserName,
                //     authenticationMethod: "custom",
                //     claims: GetUserClaims(2));
                //}
                //else
                //{

                //    //验证失败
                //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
                //}
        }

        private Claim[] GetUserClaims(MDataTable mdt)
        {
            return new Claim[]
           {
            new Claim(JwtClaimTypes.Id, mdt.Rows[0]["guid"].ToString()),
            new Claim("logincode",mdt.Rows[0]["logincode"].ToString()),
            new Claim("comid",mdt.Rows[0]["comid"].ToString()),
            new Claim("username",mdt.Rows[0]["username"].ToString()),
            new Claim("roleid",mdt.Rows[0]["roleid"].ToString()),
            new Claim("departid",mdt.Rows[0]["departid"].ToString()),
            new Claim("state",mdt.Rows[0]["state"].ToString()),
            new Claim("phone",mdt.Rows[0]["phone"].ToString())
           };
        }

        //可以根据需要设置相应的Claim
        private Claim[] GetUserClaims(int a)
        {
            if(a==1)
            {
                return new Claim[]
            {
            new Claim("UserId", 1.ToString()),
            new Claim(JwtClaimTypes.Name,"wjk"),
            new Claim(JwtClaimTypes.GivenName, "jaycewu"),
            new Claim(JwtClaimTypes.FamilyName, "yyy"),
            new Claim(JwtClaimTypes.Email, "977865769@qq.com"),
            new Claim(JwtClaimTypes.Role,"admin")
            };
            }
            else
            {
 return new Claim[]
            {
            new Claim("UserId", 2.ToString()),
            new Claim(JwtClaimTypes.Name,"aa"),
            new Claim(JwtClaimTypes.GivenName, "aa"),
            new Claim(JwtClaimTypes.FamilyName, "aa"),
            new Claim(JwtClaimTypes.Email, "9778657aa69@qq.com"),
            new Claim(JwtClaimTypes.Role,"admin")
            };
            }
           
        }
    }
}
