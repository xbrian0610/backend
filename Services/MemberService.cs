using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPi;
using CoreAPi.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mywebsite.Resources.Response;
using Mywebsite.utils;

namespace Mywebsite.Services
{
    public class MemberService
    {
        //定義資料庫連結
        private readonly AppDBContext _context;
        private readonly Tools _tools;
        private IMapper _mapper;
        public MemberService(IMapper mapper, AppDBContext AppDBContext, IConfiguration configuration)
        {
            _mapper = mapper ??
                throw new ArgumentNullException();
            _context = AppDBContext;
            _tools = new Tools(configuration);
        }
        #region 註冊
        public async Task<string> Register(MemberRegisterResource RegisterResource, string AuthCode)
        {
            string result = string.Empty;
            var user = await _context.MemberModel.SingleOrDefaultAsync(b => b.Email == RegisterResource.Email);
            if (user != null)
            {
                return "信箱已被註冊";
            }
            #region 將資料寫入資料庫
            var member = _mapper.Map<MemberRegisterResource, MemberModel>(RegisterResource);
            var uuid = Guid.NewGuid().ToString();
            member.Id = uuid;
            //密碼加密
            member.Password = _tools.HashPassword(member.Password);
            member.AuthCode_Register = AuthCode;
            member.IsAdmin = false;

            try
            {
                //將資料加入Context
                _context.Add(member);
                //儲存變更
                await _context.SaveChangesAsync();
                result = "Success";
            }
            catch (Exception)
            {
                result = "Error";
            }
            #endregion
            return result;
        }
        #endregion
        public async Task<String> CheckEmail(string email, string AuthCode, string CreateTime)
        {
            MemberModel member = await this.GetAllData(email);
            DateTime time = Convert.ToDateTime(CreateTime);
            #region 驗證信箱
            if (time.AddDays(1) > DateTime.Now)
            {
                if (member.AuthCode_Register == AuthCode)
                {
                    member.EmailCheck = true;
                }
                else
                {
                    return "驗證失敗";
                }
                await this.UpdateEmailCheck(member);
                return "驗證成功";
            }
            else
            {
                await this.DeleteAuthCode(email);
                return "驗證信超時，請重發驗證碼";
            }
            #endregion
        }
        #region 登入
        public async Task<string> Login(string email, string password)
        {
            MemberModel CheckMember = await _context.MemberModel.SingleOrDefaultAsync(b => b.Email == email);
            //hash密碼
            string HashPassword = _tools.HashPassword(password);
            //確認是否有此帳號
            if (CheckMember != null)
            {
                //確認帳號是否正確
                if (HashPassword == CheckMember.Password)
                {
                    //確認信箱是否驗證
                    if (CheckMember.EmailCheck == true)
                    {
                        return "登入成功";
                    }
                    else
                    {
                        return "信箱未驗證";
                    }
                }
                else
                {
                    return "密碼錯誤";
                }
            }
            else
            {
                return "無此帳號";
            }

        }
        #endregion


        #region 讀取資料(修改會員資料用)
        public async Task<MemberRegisterResource> GetData(string Email)
        {
            MemberRegisterResource data = new MemberRegisterResource();
            #region 載入資料
            try
            {
                MemberModel MemberData = await _context.MemberModel.SingleOrDefaultAsync(u => u.Email == Email);
                data = _mapper.Map<MemberModel, MemberRegisterResource>(MemberData);
                return data;
            }
            catch (Exception)
            {
                data = null;
            }
            #endregion
            return data;

        }
        public async Task<MemberModel> GetAllData(string email)
        {
            MemberModel data = new MemberModel();
            try
            {
                data = await _context.MemberModel.SingleOrDefaultAsync(b => b.Email == email);
            }
            catch
            {
                data = null;
            }


            return data;
        }
        #endregion


        #region 確認email
        public async Task<string> UpdateEmailCheck(MemberModel member)
        {
            string result;
            //利用信箱搜尋此筆資料
            //判斷是否有資料
            if (member != null)
            {
                //進行更改
                _context.Entry(member).CurrentValues.SetValues(member);
                await _context.SaveChangesAsync();
                result = "修改成功";
            }
            else
            {
                result = "修改失敗";
            }
            return result;
        }
        #endregion

        #region 讀取所有會員資料
        public async Task<List<MemberRegisterResource>> GetDataList()
        {
            List<MemberRegisterResource> data = new List<MemberRegisterResource>();
            try
            {
                var member = await _context.MemberModel.ToListAsync();
                data = _mapper.Map<List<MemberModel>, List<MemberRegisterResource>>(member);
            }
            catch
            {
                data = null;
            }


            return data;
        }
        #endregion

        #region 更新會員密碼
        public async Task<string> ChangePassword(MemberChangePasswordResource UpdataMember)
        {
            string result;
            //利用信箱搜尋此筆資料
            //判斷是否有資料
            MemberModel member = await this.GetAllData(UpdataMember.Email);
            UpdataMember.Password = _tools.HashPassword(UpdataMember.Password);
            if (member.Password != UpdataMember.Password)
            {
                return "舊密碼輸入錯誤";
            }
            MemberModel NewMember = member;
            NewMember.Password = _tools.HashPassword(UpdataMember.NewPassword);
            if (member != null)
            {
                //進行更改
                _context.Entry(member).CurrentValues.SetValues(NewMember);
                await _context.SaveChangesAsync();
                result = "修改成功";
            }
            else
            {
                result = "修改失敗";
            }
            return result;
        }
        #endregion
        public async Task<MemberUpdateResource> UpdateResource(string Email)
        {
            MemberModel member = await this.GetAllData(Email);
            MemberUpdateResource Data = _mapper.Map<MemberModel, MemberUpdateResource>(member);
            return Data;
        }
        public async Task<string> UpdateMember(MemberUpdateResource UpdateResource)
        {
            MemberModel member = await this.GetAllData(UpdateResource.Email);
            string result;
            try
            {
                MemberModel NewMember = member;
                NewMember = _mapper.Map<MemberUpdateResource,MemberModel>(UpdateResource,member);
                _context.Entry(member).CurrentValues.SetValues(NewMember);
                await _context.SaveChangesAsync();
                result = "修改成功";
            }
            catch (Exception)
            {
                result = "修改失敗";
            }
            return result;
        }
        #region 忘記密碼產出新密碼
        public async Task<string> GetPassword(string email)
        {
            string[] code ={ "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K"
            , "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y"
            , "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b"
                , "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n"
                    , "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            string password = string.Empty;
            Random rd = new Random();
            for (int i = 0; i < 10; i++)
            {
                password += code[rd.Next(code.Count())];
            }
            try
            {
                MemberModel member = await _context.MemberModel.SingleOrDefaultAsync(b => b.Email == email);
                MemberModel NewMember = member;
                if (member != null)
                {
                    member.Password = _tools.HashPassword(password);
                    _context.Entry(member).CurrentValues.SetValues(NewMember);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                password = "";
            }
            return password;
        }
        #endregion
        #region 刪除驗證碼(驗證信超時用)
        public async Task<string> DeleteAuthCode(string email)
        {
            var DeleteData = new MemberModel();
            try
            {
                DeleteData = _context.MemberModel.SingleOrDefault(u => u.Email == email);
                DeleteData.AuthCode_Register = null;
                _context.Entry(DeleteData).CurrentValues.SetValues(DeleteData);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return "修改失敗";
            }
            return "修改成功";

        }
        #endregion
        #region 重寄驗證碼用
        public async Task<string> ReSendeMail(string email, String authCode)
        {
            try
            {
                MemberModel data = await _context.MemberModel.SingleOrDefaultAsync(u => u.Email == email);
                data.AuthCode_Register = authCode;
                _context.Entry(data).CurrentValues.SetValues(data);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return "資料寫入錯誤";
            }

            return "成功";
        }
        #endregion
    }

}