using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using CoreAPi;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace TokenDecrypt
{
    public class AuthorizeTokenDecrypt
    {
        public readonly RequestDelegate _next;
        public AuthorizeTokenDecrypt(RequestDelegate next)
        {
            this._next = next ??
                throw new ArgumentNullException(nameof(next));
        }
        public async Task InvokeAsync(HttpContext context, AppDBContext DBcontext)
        {
            //定義Request Response
            var Request = context.Request;
            var Response = context.Response;
            //設定路徑
            string Path = Request.Path.Value;
            //抓取傳送方式
            string Method = Request.Method;
            //抓取tolken
            string token = Request.Headers["Authorization"];
            string SercetKey = "4{!-!Rjx2.W]fX~jN:<Ae$D'dLlnG%%xo`W2D5TatBx&MJjZ>(+ujy*G*Y<XH.X";
            var hash256 = new HMACSHA256(Encoding.ASCII.GetBytes(SercetKey));
            if (!RouteCheck(Path, Method))
            {
                try
                {
                    var jwt = token.Split('.');
                    var header = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwt[0]));
                    var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwt[1]));
                    bool susses = true;
                    susses = susses & string.Equals(jwt[2], Base64UrlEncoder.Encode(hash256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(jwt[0], ".", jwt[1])))));
                    if (susses)
                    {
                        if (await MemberCheck(payload["Email"].ToString(), DBcontext))
                        {
                            string[] roles =new string[1];
                            if (payload["IsAdmin"].ToString() == "1")
                            {
                                roles[0] = "Admin";
                            }  
                            GenericIdentity authenticatedGenericIdentity = new GenericIdentity(payload["Email"].ToString());
                            GenericPrincipal genericPrincipal =new GenericPrincipal(authenticatedGenericIdentity, roles);
                        }
                        else
                        {
                            await BadResponse(Response);
                        }
                    }
                    else
                    {
                        await BadResponse(Response);
                    }
                }
                catch (Exception e)
                {
                    context.Response.ContentType = "text/html; charset = utf-8";
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("權限驗證失敗" + "\r\n" + e.ToString(), Encoding.GetEncoding("utf-8"));
                    await Task.CompletedTask;
                    return;
                }
            }
            else
            {
                await BadResponse(Response);
            }
            await this._next(context);
        }
        public Boolean RouteCheck(string Path, string Method)
        {
            if (Path == "/api/member/Login" && Method == "POST")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<Boolean> MemberCheck(string Email, AppDBContext context)
        {
            var member = await context.MemberModel.SingleOrDefaultAsync(u => u.Email == Email);
            if (member == null)
            {
                return false;
            }
            return true;
        }
        public async Task BadResponse(HttpResponse Response)
        {
            Response.ContentType = "text/html; charset = utf-8";
            Response.StatusCode = 401;
            await Response.WriteAsync("權限驗證失敗", Encoding.GetEncoding("utf-8"));
            await Task.CompletedTask;
            return;
        }
    }
}