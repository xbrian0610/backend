using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Mywebsite.Resources.Response;
namespace Mywebsite.Services
{
    public class MailService
    {
        private readonly IHostingEnvironment _HostingEnvironment;
        private string gmail_account = "aqz02468@gmail.com"; //Gmail帳號
        private string gmail_password = "uhppbhzpyteadsbi"; //Gmail密碼
        private string gmail_mail = "aqz02468@gmail.com"; //Gmail信箱

        public MailService(IHostingEnvironment hostingEnvironment)
        {
            _HostingEnvironment = hostingEnvironment;
        }
        public string GetValidateCode()
        {
            #region 產生驗證碼
            //設定驗證碼字元的陣列
            string[] Code ={ "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K"
        , "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y"
            , "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b"
                , "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n"
                    , "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            //宣告初始為空的驗證碼字串
            string ValidateCode = string.Empty;
            //宣告可產生隨機數值的物件
            Random rd = new Random();
            //使用迴圈產生出驗證碼
            for (int i = 0; i < 10; i++)
            {
                ValidateCode += Code[rd.Next(Code.Count())];
            }
            #endregion
            //回傳驗證碼
            return ValidateCode;
        }
        public async Task<string> RegisterMail(string authcode, MemberRegisterResource RegisterResource, Uri ValidateUrl)
        {
            string result = string.Empty;
            try
            {
                #region 寄信
                //取得寫好的驗證信範本內容
                var Webroot = _HostingEnvironment.WebRootPath;
                var file = System.IO.Path.Combine(Webroot, "RegisterMail.html");
                string TempMail = System.IO.File.ReadAllText(file);
                string CreateTime = DateTime.Now.ToString();
                //藉由Service將使用者資料填入驗證信範本中
                string MailBody = this.GetRegisterMailBody(TempMail, CreateTime, ValidateUrl.ToString().Replace("%3F", "?"));
                //呼叫Service寄出驗證信
                await this.SendRegisterMail(MailBody, RegisterResource.Email);
                #endregion
            }
            catch (Exception)
            {
                result = "寄信失敗";
            }
            return result;

        }
        public async Task<string> ForgetPasswordMail(string name, string password, string email)
        {
            string result = string.Empty;
            try
            {
                #region 寄信
                var webroot = _HostingEnvironment.WebRootPath;
                var file = System.IO.Path.Combine(webroot, "ForgetPasswordMail.html");
                string TempMail = System.IO.File.ReadAllText(file);
                string CreateTime = DateTime.Now.ToString();
                //藉由Service將使用者資料填入驗證信範本中
                string MailBody = this.GetForgetPasswordMailBody(TempMail, password, CreateTime, name);
                //呼叫Service寄出驗證信
                await this.SendForgetPasswordMail(MailBody, email);
                result = "寄信成功";
                #endregion
            }
            catch (Exception)
            {
                result = "寄信失敗";
            }
            return result;


        }
        //將使用者資料填入驗證信範本中
        public string GetRegisterMailBody(string TempString, string CreateTime, string ValidateUrl)
        {
            #region 將使用者資料填入
            TempString = TempString.Replace("{{CreateTime}}", CreateTime);
            TempString = TempString.Replace("{{ValidateUrl}}", ValidateUrl);
            #endregion
            //回傳最後結果
            return TempString;
        }
        public string GetForgetPasswordMailBody(string TempString, string Password, string CreateTime, string Name)
        {
            #region 
            //將使用者資料填入
            TempString = TempString.Replace("{{Password}}", Password);
            TempString = TempString.Replace("{{CreateTime}}", CreateTime);
            TempString = TempString.Replace("{{Name}}", Name);
            //回傳最後結果
            #endregion
            return TempString;
        }
        //寄驗證信的方法
        public async Task SendRegisterMail(string MailBody, string ToEmail)
        {
            #region 寄信設定
            //建立寄信用Smtp物件，這裡使用Gmail為例
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            //設定使用的Port，這裡設定Gmail所使用的
            SmtpServer.Port = 587;
            //建立使用者憑據，這裡要設定您的Gmail帳戶
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            //開啟SSL
            SmtpServer.EnableSsl = true;
            //宣告信件內容物件
            MailMessage mail = new MailMessage();
            //設定來源信箱
            mail.From = new MailAddress(gmail_mail);
            //設定收信者信箱
            mail.To.Add(ToEmail);
            //設定信件主旨
            mail.Subject = "會員註冊確認信";
            //設定信件內容
            mail.Body = MailBody;
            //設定信件內容為HTML格式
            mail.IsBodyHtml = true;
            //送出信件
            await SmtpServer.SendMailAsync(mail);
            #endregion
        }
        public async Task SendForgetPasswordMail(string MailBody, string ToEmail)
        {
            #region 寄信設定
            //建立寄信用Smtp物件，這裡使用Gmail為例
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            //設定使用的Port，這裡設定Gmail所使用的
            SmtpServer.Port = 587;
            //建立使用者憑據，這裡要設定您的Gmail帳戶
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            //開啟SSL
            SmtpServer.EnableSsl = true;
            //宣告信件內容物件
            MailMessage mail = new MailMessage();
            //設定來源信箱
            mail.From = new MailAddress(gmail_mail);
            //設定收信者信箱
            mail.To.Add(ToEmail);
            //設定信件主旨
            mail.Subject = "忘記密碼";
            //設定信件內容
            mail.Body = MailBody;
            //設定信件內容為HTML格式
            mail.IsBodyHtml = true;
            //送出信件
            await SmtpServer.SendMailAsync(mail);
            #endregion
        }
    }
}