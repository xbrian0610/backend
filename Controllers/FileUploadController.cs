using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using _Core.FilesCenter._Core.Helper;
using AutoMapper;
using CoreAPi;
using CoreAPi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using Mywebsite.Resources.Requests;
using Mywebsite.Resources.Responses;
using Mywebsite.Services;

namespace Mywebsite.Controllers
{
    [Route("api/[Controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly FileUploadService _FileUploadService;
        private readonly static Dictionary<string, string> _ContentTypes = new Dictionary<string, string>
        {
            #region 檔案格式字典
            {".docx", "application/vnd.ms-word"},
            {".doc", "application/vnd.ms-word"},
            {".ppt", "application/vnd.ms-powerpoint"},
            {".pptx", "application/vnd.ms-powerpoint"},
            {".png", "image/png"},
            {".gif", "image/gif"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".psd", "image/vnd.adobe.photoshop"},
            {".mp4", "video/mp4"},
            {".mov", "video/quicktime"},
            {".avi", "video/x-msvideo"},
            {".mpeg", "video/mpeg"},
            {".mpg", "video/mpeg"},
            {".flv", "video/x-flv"},
            {".wmv", "video/x-ms-wmv"},
            {".mkv", "video/x-matroska"},
            {".asf", "video/x-ms-asf"},
            {".pdf", "application/pdf"}
            #endregion
        };

        // 建立路徑
        public FileUploadController(IHostingEnvironment env, IMapper mapper, AppDBContext context)
        {
            _FileUploadService = new FileUploadService(env, mapper, context);
        }

        /// <summary>
        /// 讀取上傳檔案陣列
        /// </summary>
        /// <remarks>
        /// Sample Responce：
        /// 
        ///     Get /api/fileupload/getallfiles
        ///     {
        ///         "File_Id": "string",
        ///         "FileName": "string",
        ///         "CreateTime": "DateTime",
        ///         "members": 
        ///         {
        ///             "Name": "string",
        ///             "Email": 123@gmail.com
        ///         }
        ///     }
        ///     
        /// </remarks>
        /// <returns>Read Files From FileUpload</returns>
        /// <response code= "201">Read AllFiles Successful</response>
        /// <response code= "400">If FIles are Null</response>
        [Route("getallfiles")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GetFileUploadResource), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<List<GetFileUploadResource>>> GetAllFileList() // 讀取上傳檔案陣列
        {
            List<GetFileUploadResource> FileUploadList = new List<GetFileUploadResource>();
            FileUploadList = await _FileUploadService.GetFileList();
            if (FileUploadList == null)
            {
                return BadRequest(new { message = "查無資料" });
            }
            return Ok(FileUploadList);
        }

        /// <summary>
        /// 新增上傳檔案
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Post /api/fileupload/upload
        ///     {
        ///         "Member_Id" = "string",
        ///         "IFromForm" = "u"
        ///     }
        ///     
        /// </remarks>
        /// <param name= "NewFile"></param>
        /// <returns>Insert new File in FileUpload</returns>
        /// <response code= "201">Insert NewFile Successful</response>
        /// <response code= "400">If the Member_Id is Null</response>
        [HttpPost]
        [Authorize]
        [EnableCors("CorsPolicy")]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> FileUploadPost([FromForm] CreateFileUploadResource NewFile) // 新增上傳檔案
        {
            // 判斷欄位是否正確
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "資料填寫錯誤" });
            }
            // 判斷是否有上傳檔案
            if (NewFile.UploadFile != null)
            {
                // 呼叫至Services 進行寫入資料庫
                var InsertResult = await _FileUploadService.FileUpload(NewFile);
                if (InsertResult.Equals("新增成功"))
                {
                    // 回傳OK 
                    return Ok(new { message = "上傳成功" });
                }
                else if (InsertResult.Equals("非指定檔格式"))
                {
                    return BadRequest(new { message = "非指定檔格式" });
                }
                else if (InsertResult.Equals("資料庫寫入錯誤"))
                {
                    return BadRequest(new { message = "寫入資料庫失敗" });
                }
            }
            return BadRequest(new { message = "無上傳檔案" });
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Get /api/fileupload/downloadfile/{fileid}
        ///     {
        ///         "File_Id" = "string"
        ///     }
        ///     
        /// </remarks>
        /// <param name= "File_Id"></param>
        /// <returns>Download File from FileUpload</returns>
        /// <response code= "201">Download Successful</response>
        /// <response code= "400">If the fileid is Null</response>
        [Route("downloadfile/{File_Id}")]
        [HttpGet]
        // [Authorize(Roles = "Admin")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> FileDownload(string File_Id) // 下載檔案
        {
            var file = await _FileUploadService.GetFileById(File_Id);
            if (file == null)
            {
                return BadRequest(new { message = "無此檔案" });
            }
            var FileExt = Path.GetExtension(file.FileName);
            var path = file.FileUrl;
            var memoryStream = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            string encodeFilename = System.Web.HttpUtility.UrlEncode(file.FileName, System.Text.Encoding.GetEncoding("UTF-8"));
            Response.Headers.Add("Content-Disposition", "attachment; filename=" + encodeFilename);
            // 回傳檔案到 Client 需要附上 Content Type，否則瀏覽器會解析失敗。
            return new FileStreamResult(memoryStream, _ContentTypes[Path.GetExtension(path).ToLowerInvariant()]);
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Delete /api/fileupload/{fileid}
        ///     {
        ///         "File_Id" = int
        ///     }
        ///     
        /// </remarks>
        /// <param name= "File_Id"></param>
        /// <returns>Delete File from FileUpload</returns>
        /// <response code= "201">Delete Successful</response>
        /// <response code= "400">If the fileid is Null</response>
        [HttpDelete("{File_Id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> FileDelete(string File_Id) // 刪除檔案
        {
            var DeleteResult = await _FileUploadService.FileRemove(File_Id);
            if (DeleteResult.Equals("刪除成功"))
            {
                return Ok(new { message = "刪除成功" });
            }
            else if (DeleteResult.Equals("資料庫刪除錯誤"))
            {
                return BadRequest(new { message = "資料庫刪除失敗" });
            }
            else
            {
                return BadRequest(new { message = "無此檔案" });
            }
        }
    }
}