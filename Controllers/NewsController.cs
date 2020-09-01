using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mywebsite.Resources.Request;
using Mywebsite.Resources.Response;
using Mywebsite.Services;

namespace Mywebsite.Controllers
{
    [Route("api/[Controller]")]

    public class NewsController : ControllerBase
    {
        private readonly NewsService _newsService;
        public NewsController(IMapper _mapper, AppDBContext _Context)
        {
            _newsService = new NewsService(_mapper, _Context);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> InsertAsyce([FromBody] NewsInsertResource insertResource)
        {
            #region 新增最新消息
            if (ModelState.IsValid)
            {
                //進入service寫入資料
                string result = await _newsService.InsertNews(insertResource);
                if (result == "新增成功")
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("資料未過驗證");
            #endregion
        }

        [Route("GetDataList")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<NewsViewResource>>> GetDataList()
        {
            #region 讀取最新消息
            List<NewsViewResource> view = new List<NewsViewResource>();
            #endregion
            view = await _newsService.GetDataList();
            return Ok(view);

        }
        [Route("GetData")]
        [HttpGet("{Id}")]
        [Authorize]
        public async Task<ActionResult<List<NewsViewResource>>> GetData(int Id)
        {
            #region 讀取最新消息
            NewsViewResource view = new NewsViewResource();
            #endregion
            view = await _newsService.GetData(Id);
            if (view.news_Id.ToString() != "")
            {
                return Ok(view);
            }
            return BadRequest(view);
        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Updata([FromBody]NewsUpdataResource UpdataResource)
        {
            string result = await _newsService.UpdateNews(UpdataResource);
            if (result == "修改成功")
            {
                return Ok(result);
            }
            return BadRequest(result);
        }



    }
}