using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CoreAPi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mywebsite.Resources.Response;

namespace Mywebsite.utils
{

    public class Tools
    {

        public Tools(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration;
        public string HashPassword(string Password)
        {
            string saltkey = "1234dwsfqwfjhqlnkqwl";
            string saltAndPassword = String.Concat(Password, saltkey);
            //定義SHA256的HASH物件
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            //取得密碼轉換成byte資料
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            //取得Hash後byte資料
            byte[] HashDate = sha256Hasher.ComputeHash(PasswordData);
            //將Hash後byte資料轉換成string
            string Hashresult = Convert.ToBase64String(HashDate);
            //回傳Hash後結果
            return Hashresult;
        }

        public string Token(MemberModel Data)
        {

            var userClaims = new ClaimsIdentity(new[] {
                new Claim(JwtRegisteredClaimNames.Email, Data.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, Data.IsAdmin==true? "Admin":"user"),
                });
            //取得對稱式加密金鑰
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            //用於描述 JWT 的 TokenDescriptor
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = Configuration["Jwt:Issuer"],
                Audience = Configuration["Jwt:Issuer"],
                Subject = userClaims,
                Expires = DateTime.Now.AddHours(12),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };
            //產出JWT物件
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            //產出序列化token字串
            var serializeToken = tokenHandler.WriteToken(securityToken);
            return serializeToken;
        }
    }
}