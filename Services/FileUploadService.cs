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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mywebsite.Resources.Requests;
using Mywebsite.Resources.Responses;

namespace Mywebsite.Services
{
    public class FileUploadService
    {
        private readonly AppDBContext _context;
        // 宣告AutoMapper
        private readonly IMapper _mapper;
        // 宣告檔案存放路徑
        private readonly string _folder;

        public FileUploadService(IHostingEnvironment env, IMapper mapper, AppDBContext context)
        {
            // 把上傳目錄設為：wwwroot\UploadFolder
            _folder = $@"{env.WebRootPath}\UploadFolder";
            this._mapper = mapper ??
                throw new ArgumentException(nameof(mapper));
            this._context = context ??
                throw new ArgumentException(nameof(context));
        }

        public async Task<List<GetFileUploadResource>> GetFileList() // 讀取所有上傳檔案資料
        {
            // 宣告回傳格式 Resource
            List<GetFileUploadResource> FilesView;
            List<FileUploadModel> DataList;
            try
            {
                DataList = await _context.FileUpload
                                .Include(c => c.members)
                                .OrderByDescending(o => o.CreateTime)
                                .ToListAsync();
                // if (FileType.Equals("word"))
                // {
                //     // 讀取資料庫資料
                //     DataList = await _context.FileUpload
                //                     .Include(c => c.members)
                //                     .Where(t => EF.Functions.Like(t.FileName, "%doc%"))
                //                     .OrderByDescending(o => o.CreateTime)
                //                     .ToListAsync();
                // }
                // else if (FileType.Equals("ppt"))
                // {
                //     // 讀取資料庫資料
                //     DataList = await _context.FileUpload
                //                     .Include(c => c.members)
                //                     .Where(t => EF.Functions.Like(t.FileName, "%ppt%"))
                //                     .OrderByDescending(o => o.CreateTime)
                //                     .ToListAsync();

                // }
                // else
                // {
                //     return null;
                // }
                // 對映讀取的資料至Resource
                FilesView = _mapper.Map<List<FileUploadModel>, List<GetFileUploadResource>>(DataList);
            }
            catch (ArgumentException)
            {
                return null;
            }
            return FilesView;
        }

        public async Task<FileUploadModel> GetFileById(string File_Id) // 讀取單筆檔案資料
        {
            FileUploadModel FileResult = new FileUploadModel();
            try
            {
                // 讀取資料庫單筆資料
                FileResult = await _context.FileUpload.Where(x => x.File_Id.Equals(File_Id)).FirstOrDefaultAsync();
            }
            catch (ArgumentException)
            {
                return FileResult = null;
            }
            return FileResult;
        }

        public async Task<string> FileUpload(CreateFileUploadResource NewFile) // 新增上傳檔案進資料庫
        {
            string InsertResult = string.Empty;

            #region 取得上傳檔案內容

            // 取得附檔名
            string FileExt = Path.GetExtension(NewFile.UploadFile.FileName);
            if (!CheckContentType(FileExt))
            {
                return ("非指定檔格式");
            }
            // 檔案原始檔名
            string FileName = Path.GetFileName(NewFile.UploadFile.FileName);
            // 儲存在server上的檔名
            string PathfileName = NewFile.Member_Id + "_" + FileName;
            string FilePath = _folder;
            // // 根據副檔名分配路徑
            // if (FileExt == ".docx" || FileExt == ".doc")
            // {
            //     // 存在server上的路徑
            //     FilePath = Path.Combine(_folder, "Word");
            // }
            // else if (FileExt == ".ppt" || FileExt == ".pptx")
            // {
            //     // 存在server上的路徑
            //     FilePath = Path.Combine(_folder, "PowerPoint");
            // }
            #endregion

            #region 儲存至 Server

            // 如果沒有此路徑，則建立
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            using (var stream = new FileStream(Path.Combine(FilePath, PathfileName), FileMode.Create))
            {
                await NewFile.UploadFile.CopyToAsync(stream);
            }
            #endregion

            #region 將檔案內容填至 Model

            FileUploadModel NewData = new FileUploadModel();
            NewData.File_Id = Guid.NewGuid().ToString();
            NewData.FileName = FileName;
            NewData.FileSize = NewFile.UploadFile.Length;
            NewData.FileType = NewFile.UploadFile.ContentType;
            NewData.FileUrl = Path.Combine(FilePath, PathfileName);
            NewData.Member_Id = NewFile.Member_Id;
            NewData.CreateTime = DateTime.Now;
            #endregion

            #region 資料庫處理
            try
            {
                // 使用AutoMapper 比對寫入資料庫的資料
                var insertmapperfile = _mapper.Map<FileUploadModel>(NewData);
                // 寫入資料庫
                _context.FileUpload.Add(insertmapperfile);
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

        public async Task<string> FileRemove(string File_Id) // 刪除上傳檔案
        {
            string DeleteResult = string.Empty;

            #region 確認資料庫是否有資料

            FileUploadModel file = await GetFileById(File_Id);
            if (file == null)
            {
                return ("資料庫無此檔案");
            }
            #endregion

            #region 刪除 Server中的檔案

            var addUrl = file.FileUrl;
            if (System.IO.File.Exists(addUrl))
            {
                System.IO.File.Delete(addUrl);
            }
            #endregion

            #region 資料庫刪除處理

            try
            {
                // 刪除檔案
                _context.FileUpload.Remove(_context.FileUpload.SingleOrDefault(d => d.File_Id.Equals(File_Id)));
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

        public bool CheckContentType(string FileExt) // 確認檔案類型
        {
            switch (FileExt)
            {
                case ".docx":
                case ".doc":
                case ".ppt":
                case ".pptx":
                case ".png":
                case ".gif":
                case ".jpeg":
                case ".jpg":
                case ".psd":
                case ".mp4":
                case ".mov":
                case ".mpeg":
                case ".mpg":
                case ".flv":
                case ".mkv":
                case ".avi":
                case ".wmv":
                case ".asf":
                case ".pdf":
                    return true;
            }
            return false;
        }
    }
}