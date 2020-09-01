using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPi;
using CoreAPi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Mywebsite.Resources.Requests;
using Mywebsite.Resources.Responses;
using Mywebsite.Services;

namespace Mywebsite.Controllers
{
    [Route("api/[Controller]")]
    public class LearnOnlineController : ControllerBase
    {
        // 宣告Services
        private readonly LearnOnlineService _LearnOnlineService;
        private readonly static Dictionary<string, string> _ContentTypes = new Dictionary<string, string>
        {
            {".mp4", "video/mp4"}
        };
        public LearnOnlineController(IHostingEnvironment env, IMapper mapper, AppDBContext context)
        {
            _LearnOnlineService = new LearnOnlineService(env, mapper, context);
        }

        /// <summary>
        /// 讀取線上自學陣列
        /// </summary>
        /// <remarks>
        /// Sample Responce：
        /// 
        ///     Get /api/learnonline/getlearnonlinelist
        ///     {
        ///         “Video_Id” = int,
		///         “Coursel_TypeId” = “string”,
		///         “Coursel_Name” = “string”,
		///         “Video_Time” = “long”
        ///     }
        ///     
        /// </remarks>
        /// <returns>Read LearnOnlineList From LearnOnline</returns>
        /// <response code= "201">Read AllLearnOnlineList Successful</response>
        /// <response code= "400">If LearnOnlineData is Null</response>
        [Route("getlearnonlinelist")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(GetLearnOnlineResource), 201)]
        [ProducesResponseType(typeof(GetLearnOnlineResource), 400)]
        public async Task<ActionResult<List<GetLearnOnlineResource>>> GetLearnOnlineList() // 讀取線上自學陣列
        {
            // 宣告回傳格式物件
            var LearnOnlineList = new List<GetLearnOnlineResource>();
            // 呼叫 Services讀取陣列資料
            LearnOnlineList = await _LearnOnlineService.GetDataList();
            return Ok(LearnOnlineList);
        }

        /// <summary>
        /// 新增線上自學資料
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Post /api/learnonline/learnonlineupload
        ///     {
        ///         “Coursel_TypeId” = “string”,
	    ///         “Coursel_Name” = “string”,
	    ///         “Video_Time” = int,
	    ///         IFromForm = File
        ///     }
        ///     
        /// </remarks>
        /// <param name= "NewFile"></param>
        /// <returns>Insert NewFile in LearnOnline</returns>
        /// <response code= "201">Insert NewFile Successful</response>
        /// <response code= "400">Fail InsertInto LearnOnline</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        [RequestSizeLimit(209715200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> VideoPost(CreateLearnOnlineResource NewFile) // 新增線上自學資料
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "資料填寫錯誤" });
            }
            if (NewFile.Video.Length > 0)
            {
                // 呼叫至Services 進行寫入資料庫
                var InsertResult = await _LearnOnlineService.LearningOnlineVideoUpload(NewFile);
                if (InsertResult.Equals("新增成功"))
                {
                    return Ok(new { message = "上傳成功" });
                }
                else if (InsertResult.Equals("資料庫寫入錯誤"))
                {
                    return BadRequest(new { message = "寫入資料庫失敗" });
                }
            }
            return BadRequest(new { message = "無上傳檔案" });
        }

        /// <summary>
        /// 播放影片
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Get /api/fileupload/downloadfile/{VideoId}
        ///     {
        ///         "VideoId" = int
        ///     }
        ///     
        /// </remarks>
        /// <param name= "VideoId"></param>
        /// <returns>Play Video from LearnOnline</returns>
        /// <response code= "201">Play Successful</response>
        /// <response code= "400">If the VideoId is Null</response>
        [Route("downloadvideo/{videoid}")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> FileDownload(int VideoId) // 播放影片
        {
            // 根據 videoid讀取單筆資料
            var video = await _LearnOnlineService.GetVideoById(VideoId);
            // 確認是否有資料
            if (video == null)
            {
                return BadRequest(new { message = "無此檔案" });
            }
            // 取得路徑
            var path = $@"{ video.Video_Url }";
            var MemoryStream = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(MemoryStream);
            }
            MemoryStream.Seek(0, SeekOrigin.Begin);

            // 回傳檔案到 Client 需要附上 Content Type，否則瀏覽器會解析失敗。
            return new FileStreamResult(MemoryStream, _ContentTypes[Path.GetExtension(path).ToLowerInvariant()]);
        }

        /// <summary>
        /// 刪除線上自學資料
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Delete /api/fileupload/{Video_Id}
        ///     {
        ///         "Video_Id" = int
        ///     }
        ///     
        /// </remarks>
        /// <param name= "Video_Id"></param>
        /// <returns>Delete Video from FileUpload</returns>
        /// <response code= "201">Delete Successful</response>
        /// <response code= "400">If the Video_Id is Null</response>
        [HttpDelete("{Video_Id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> FileDelete(int Video_Id) // 刪除線上自學資料
        {
            // 呼叫 Services 刪除資料庫資料
            var DeleteResult = await _LearnOnlineService.LearnOnlineRemove(Video_Id);
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
                return BadRequest(new { message = "無此檔案"});
            }
        }
    }
}