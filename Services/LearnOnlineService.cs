using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPi;
using CoreAPi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Mywebsite.Resources.Requests;
using Mywebsite.Resources.Responses;

namespace Mywebsite.Services
{
    public class LearnOnlineService
    {
        private readonly AppDBContext _context;
        // 宣告AutoMapper
        private readonly IMapper _mapper;
        // 宣告檔案存放路徑
        private readonly string _folder;

        public LearnOnlineService(IHostingEnvironment env, IMapper mapper, AppDBContext context)
        {
            // 把上傳目錄設為：wwwroot\UploadFolder
            _folder = $@"{env.WebRootPath}\UploadFolder\Video";
            this._mapper = mapper ??
                throw new ArgumentException(nameof(mapper));
            this._context = context ??
                throw new ArgumentException(nameof(context));
        }

        public async Task<List<GetLearnOnlineResource>> GetDataList() // 讀取所有線上自學陣列資料
        {
            List<GetLearnOnlineResource> LearnOnlineView;
            try
            {
                var DataList = await _context.LearningOnline.ToListAsync();
                LearnOnlineView = _mapper.Map<List<LearnOnlineModel>, List<GetLearnOnlineResource>>(DataList);
            }
            catch (ArgumentException)
            {
                LearnOnlineView = null;
            }
            return LearnOnlineView;
        }

        public async Task<LearnOnlineModel> GetVideoById(int Video_Id) // 讀取單筆檔案資料
        {
            LearnOnlineModel VideoResult = new LearnOnlineModel();
            try
            {
                // 根據 videoid 讀取單筆影片資料
                VideoResult = await _context.LearningOnline.Where(x => x.Video_Id.Equals(Video_Id)).FirstOrDefaultAsync();
            }
            catch (ArgumentException)
            {
                return VideoResult = null;
            }
            return VideoResult;
        }

        public async Task<string> LearningOnlineVideoUpload(CreateLearnOnlineResource NewFile) // 新增線上自學資料至資料庫
        {
            string InsertResult = string.Empty;

            #region 取得上傳影片內容

            // 取得影片無路徑檔名
            string FileName = Path.GetFileNameWithoutExtension(NewFile.Video.FileName);
            // 取得附檔名
            string FileExt = Path.GetExtension(NewFile.Video.FileName);
            // 儲存在server上的檔名
            string PathfileName = NewFile.Coursel_TypeId + NewFile.Coursel_Name + "_" + FileName + FileExt;
            string FilePath = _folder;
            #endregion

            #region 儲存至 Server

            // 如果沒有此路徑，則建立
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            using (var stream = new FileStream(Path.Combine(FilePath, PathfileName), FileMode.Create))
            {
                await NewFile.Video.CopyToAsync(stream);
            }
            #endregion

            #region 將檔案內容填至 Model

            // 宣告Model模型
            LearnOnlineModel NewData = new LearnOnlineModel();
            // ====將資料填入Model中
            NewData.Coursel_TypeId = NewFile.Coursel_TypeId;
            NewData.Coursel_Name = NewFile.Coursel_Name;
            NewData.CreateTime = DateTime.Now;
            NewData.Video_Size = NewFile.Video.Length;
            NewData.Video_Type = NewFile.Video.ContentType;
            NewData.Video_Time = NewFile.Video_Time;
            NewData.Video_Url = Path.Combine(FilePath, PathfileName);
            // ====
            #endregion

            #region 寫入資料庫處理

            try
            {
                // 使用AutoMapper 比對寫入資料庫的資料
                var InsertMapperFile = _mapper.Map<LearnOnlineModel>(NewData);
                // 寫入資料庫
                _context.LearningOnline.Add(InsertMapperFile);
                // 儲存資料庫變更
                await _context.SaveChangesAsync();
            }
            catch (ArgumentException)
            {
                return ("資料庫寫入錯誤");
            }
            return ("新增成功");
            #endregion
        }

        public async Task<string> LearnOnlineRemove(int Video_Id) //刪除資料庫檔案
        {
            string DeleteResult = string.Empty;

            #region 確認資料庫是否有資料

            // 根據 videoid讀取單筆資料 
            LearnOnlineModel video = await GetVideoById(Video_Id);
            if (video == null)
            {
                return ("資料庫無此資料");
            }
            #endregion

            #region 刪除 Server中的檔案

            // 取得路徑
            var addUrl = video.Video_Url;
            // Server 是否有此路徑
            if (System.IO.File.Exists(addUrl))
            {
                // 刪除 Server檔案路徑
                System.IO.File.Delete(addUrl);
            }
            #endregion

            #region 資料庫刪除處理

            try
            {
                // 刪除檔案
                _context.LearningOnline.Remove(_context.LearningOnline.SingleOrDefault(d => d.Video_Id == Video_Id));
                // 儲存資料庫變更
                await _context.SaveChangesAsync();
            }
            catch (ArgumentException)
            {
                return ("資料庫刪除錯誤");
            }
            return ("刪除成功");
            #endregion
        }
    }
}