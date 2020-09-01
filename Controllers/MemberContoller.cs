using System;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPi;
using CoreAPi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mywebsite.Resources.Response;
using Mywebsite.Services;
using Mywebsite.utils;
using Microsoft.AspNetCore.Authorization;
namespace Mywebsite.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly Tools _tools;
        private readonly MemberService _MemberService;
        private readonly MailService _MailService;
        public MemberController(IConfiguration configuration, AppDBContext _context, IMapper mapper, IHostingEnvironment HostingEnvironment)
        {
            _config = configuration;
            _MemberService = new MemberService(mapper, _context, configuration);
            _tools = new Tools(configuration);
            _MailService = new MailService(HostingEnvironment);
        }

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <remarks>
        ///     
        ///     Post /api/member/Register
        ///     {
        ///         "Email":"Email",
        ///         "Gender":"Gender",
        ///         "Password":"Password",
        ///         "Name":"Name",
        ///         "Address":"Address",
        ///         "Service_Unit":"Service_Unit"
        ///         "Race":"Race",
        ///         "Position":"Position",
        ///         "Phone":"Phone"
        ///     }
        /// </remarks>
        /// <param name="RegisterResource">註冊會員資料</param>
        /// <returns>註冊狀況</returns>
        /// <response code= "200">信件已寄出</response>
        /// <response code= "400">寫入資料庫失敗</response>
        /// <response code= "400">資料未過驗證</response>
        [Route("Register")]
        [HttpPost]
        public async Task<ActionResult> RegisterAsyce([FromBody] MemberRegisterResource RegisterResource) // 會員註冊
        {
            if (ModelState.IsValid)
            {
                //把註冊資料傳入資料庫
                #region 寫入資料庫
                string authcode = _MailService.GetValidateCode();
                string error = await _MemberService.Register(RegisterResource, authcode);
                if (error == "信箱已被註冊")
                {
                    return BadRequest(error);
                }
                #endregion
                if (error == "Success")
                {
                    #region 寄信    
                    string CreateTime = DateTime.Now.ToString();
                    //宣告Email驗證用的Url
                    Uri ValidateUrl = new Uri(Url.Link("EmailApi", new { Email = RegisterResource.Email, AuthCode = authcode, CreateTime = CreateTime }));
                    error = await _MailService.RegisterMail(authcode, RegisterResource, ValidateUrl);
                    #endregion
                    return Ok("信件已寄出");
                }
                else
                {
                    return BadRequest("寫入資料庫失敗");
                }
            }
            else
            {
                return BadRequest("資料未過驗證");
            }
        }

        [Route("api/member", Name = "EmailApi")]
        [HttpGet]
        public async Task<ActionResult> EmailValidate(string email, string AuthCode, string CreateTime) // 信箱驗證
        {
            #region 驗證信箱
            string result = await _MemberService.CheckEmail(email, AuthCode, CreateTime);
            if (result == "驗證成功")
            {
                return Redirect("http://192.168.43.56:5501/register_success.html");
            }
            Response.Redirect("http://192.168.43.56:5501/register_success.html");
            return BadRequest(Response);

            #endregion
        }

        /// <summary>
        /// 重新寄送驗證信
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Get /api/Member/ReSendEmail
        ///     {
        ///         "Email" = Email
        ///     }
        ///     
        /// </remarks>
        /// <param name= "Email"></param>
        /// <returns>帳號狀況</returns>
        /// <response code= "200">寄信成功</response>
        /// <response code= "400">尚未註冊</response>
        [Route("ReSendEmail")]
        [HttpGet("Email")]
        public async Task<ActionResult> ReSendEmail(string Email) // 重新寄送驗證信
        {
            MemberRegisterResource member = await _MemberService.GetData(Email);
            if (member != null)
            {
                #region 寫入資料庫
                string authcode = _MailService.GetValidateCode();
                string error = await _MemberService.Register(member, authcode);
                #endregion
                #region 寄信    
                string CreateTime = DateTime.Now.ToString();
                //宣告Email驗證用的Url
                Uri ValidateUrl = new Uri(Url.Link("EmailApi", new { Email = member.Email, AuthCode = authcode, CreateTime = CreateTime }));
                await _MailService.RegisterMail(authcode, member, ValidateUrl);
                #endregion
                return Ok("寄信成功");
            }
            return BadRequest("尚未註冊!");
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <remarks>
        ///     
        ///     Post /api/member/Login
        ///     {
        ///         "Email":"Email",
        ///         "Password":"Password",
        ///     }
        /// </remarks>
        /// <param name="LoginResource">註冊會員資料</param>
        /// <returns>狀況</returns>
        /// <response code= "200">信件已寄出</response>
        /// <response code= "400">信箱為驗證</response>
        /// <response code= "400">密碼錯誤</response>
        /// <response code= "400">無此帳號</response>
        [Route("Login")]
        [HttpPost]
        [ProducesResponseType(typeof(MembertoViewResource), 200)]
        [ProducesResponseType(typeof(MembertoViewResource), 404)]
        public async Task<ActionResult<MembertoViewResource>> LoginAsyce([FromBody] MemberLoginResource LoginResource) // 會員登入
        {
            string result = string.Empty;
            //驗證帳號
            if (ModelState.IsValid)
            {
                //檢查資料正確性
                result = await _MemberService.Login(LoginResource.Email, LoginResource.Password);
            }

            //判斷是否驗證成功
            if (result == "登入成功")
            {
                #region 載入回傳資料
                MemberModel LoginData = await _MemberService.GetAllData(LoginResource.Email);
                MembertoViewResource ResultData = new MembertoViewResource();
                ResultData.Member = LoginData;
                ResultData.Token = _tools.Token(LoginData);
                #endregion
                return Ok(ResultData);
            }
            else
            {
                return BadRequest(result);
            }

        }

        /// <summary>
        /// 讀取個人會員資料
        /// </summary>
        /// <remarks>
        ///     
        ///     Get /api/member/GetData
        ///     {
        ///         "Email":"Email",
        ///     }
        ///     {
        ///         "Email":"Email",
        ///         "Gender":"Gender",
        ///         "Password":"Password",
        ///         "Name":"Name",
        ///         "Address":"Address",
        ///         "Service_Unit":"Service_Unit"
        ///         "Race":"Race",
        ///         "Position":"Position",
        ///         "Phone":"Phone"
        ///     }
        /// </remarks>
        /// <param name="Email"></param>
        /// <returns>讀取個人會員資料</returns>
        /// <response code= "200">Read Member From Member</response>
        /// <response code= "400">查無資料</response>
        [Authorize]
        [Route("GetData")]
        [HttpGet("Email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MemberRegisterResource>> GetDataAsync(string Email) // 讀取個人會員資料
        {
            MemberRegisterResource data = await _MemberService.GetData(Email);
            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest("查無資料");
        }

        /// <summary>
        /// 讀取所有會員資料
        /// </summary>
        /// <remarks>
        ///     
        ///     Get /api/member/ShowAllMember
        ///     {
        ///     }
        ///     {
        ///         "Email":"Email",
        ///         "Gender":"Gender",
        ///         "Password":"Password",
        ///         "Name":"Name",
        ///         "Address":"Address",
        ///         "Service_Unit":"Service_Unit"
        ///         "Race":"Race",
        ///         "Position":"Position",
        ///         "Phone":"Phone"
        ///     }
        /// </remarks>
        /// <returns>讀取個人會員資料</returns>
        /// <response code= "200">Read Members From Member</response>
        [Route("ShowAllMember")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<MemberRegisterResource>> ShowAllMember() // 讀取所有會員資料(管理員使用)
        {
            var member = await _MemberService.GetDataList();
            return Ok(member);
        }

        [Authorize]
        [Route("Update")]
        [HttpPut]
        public async Task<ActionResult> UpdateMember([FromBody] MemberUpdateResource UpdateData) //修改會員資料
        {
            string result = await _MemberService.UpdateMember(UpdateData);
            if (result == "修改成功")
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <remarks>
        /// Sample Response：
        /// 
        ///     Put /api/Member/UpdataMember
        ///     {
        ///         "Email":"Email",
        ///         "Gender":"Gender",
        ///         "Name":"Name",
        ///         "Address":"Address",
        ///         "Service_Unit":"Service_Unit"
        ///         "Position":"Position",
        ///         "Phone":"Phone"
        ///     }
        ///     
        /// </remarks>
        /// <param name= "Email"></param>
        /// <returns>UpdateData</returns>
        /// <response code= "200"></response>
        [Authorize]
        [Route("GetUpdateData")]
        [HttpGet("Email")]
        public async Task<ActionResult> UpdateMemberData(string Email) //取得修改會原料(用於顯示修改頁面)
        {
            MemberUpdateResource data = await _MemberService.UpdateResource(Email);
            return Ok(data);
        }

        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Put /api/Member/UpdataMember
        ///     {
        ///         "Email":"Email",
        ///         "NewPassword":"NewPassword",
        ///         "Password":"Password",
        ///     }
        ///     
        /// </remarks>
        /// <param name= "UpdateResource"></param>
        /// <returns>Update Password</returns>
        /// <response code= "200">修改成功</response>
        /// <response code= "400">舊密碼輸入錯誤</response>
        /// <response code= "400">資料格式錯誤</response>
        [Authorize]
        [Route("ChangePassword")]
        [HttpPut]
        public async Task<ActionResult> UpdatePassword([FromBody] MemberChangePasswordResource UpdateResource) // 修改密碼
        {
            //寫入資料庫
            string result = string.Empty;
            if (ModelState.IsValid)
            {
                result = await _MemberService.ChangePassword(UpdateResource);
            }
            else
            {
                return BadRequest("資料格式錯誤");
            }

            //判斷是否成功修改
            if (result == "修改成功")
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        /// <summary>
        /// 忘記密碼
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Put /api/Member/UpdataMember
        ///     {
        ///         "Email":"Email",
        ///     }
        ///     
        /// </remarks>
        /// <param name= "email"></param>
        /// <returns>Update Password</returns>
        /// <response code= "200">寄信成功</response>
        /// <response code= "400">寄信失敗</response>
        /// <response code= "400">無此帳號</response>

        [Route("ForgetPassword/{Email}")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> ForgetPassword(string email) // 忘記密碼
        {
            MemberRegisterResource data = await _MemberService.GetData(email);
            if (data != null)
            {
                #region 寄信
                string password = await _MemberService.GetPassword(email);
                string result = await _MailService.ForgetPasswordMail(data.Name, password, data.Email);
                #endregion
                if (result == "寄信成功")
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            else
            {
                return BadRequest("無此帳號");
            }
        }
    }
}