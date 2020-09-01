using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPi;
using CoreAPi.Models;
using Microsoft.EntityFrameworkCore;
using Mywebsite.Resources.Request;
using Mywebsite.Resources.Response;

namespace Mywebsite.Services
{
    public class NewsService
    {
        private readonly AppDBContext _Context;
        private readonly IMapper _mapper;
        public NewsService(IMapper mapper, AppDBContext context)
        {
            _mapper = mapper ??
                throw new ArgumentNullException();
            _Context = context;
        }
        public async Task<string> InsertNews(NewsInsertResource newsInsertResource)
        {
            #region 建立消息
            //建立newsmodel模型
            //把傳入的資料存入模型
            try
            {
                var newsModel = _mapper.Map<NewsInsertResource, NewsModel>(newsInsertResource);
                //將創建時間改成現在時間
                newsModel.CreateTime = DateTime.Now;
                //寫入資料庫
                _Context.Add(newsModel);
                await _Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return "資料庫寫入失敗";
            }
            return "新增成功";
            #endregion
        }

        public async Task<List<NewsViewResource>> GetDataList()
        {
            #region 讀取所有消息

            List<NewsViewResource> newsview;
            try
            {
                var news = await _Context.NewsModel.ToListAsync();
                //利用automapper把讀取的資料加入viewResource
                newsview = _mapper.Map<List<NewsModel>, List<NewsViewResource>>(news);
            }
            catch (Exception)
            {
                newsview = null;
            }
            finally
            {

            }

            return newsview;
            #endregion
        }
        /// <summary>
        /// 讀取一個最新消息
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Get /api/News
        ///     {
        ///         "news_Id" = {id},
        ///         "News_Title" = "News_Title",
        ///         "News_Content" = "News_Content",
        ///         "CreateTime" = "CreateTime",
        ///     }
        ///     
        /// </remarks>
        /// <returns>Read one news From News</returns>
        /// <response code= "200">Read news Successful</response>
        /// <response code= "404">If datalist is Null</response>

        public async Task<NewsViewResource> GetData(int Id)
        {
            #region 讀取一個

            NewsViewResource newsview;
            try
            {
                var news = await _Context.NewsModel.SingleOrDefaultAsync(u => u.News_Id == Id);
                //利用automapper把讀取的資料加入viewResource
                newsview = _mapper.Map<NewsModel, NewsViewResource>(news);
            }
            catch (Exception)
            {
                newsview = null;
            }
            finally
            {

            }

            return newsview;
            #endregion
        }
        public async Task<string> UpdateNews(NewsUpdataResource UpdateData)
        {
            var news = await _Context.NewsModel.SingleOrDefaultAsync(u => u.News_Id == UpdateData.news_Id);
            NewsModel UpdateNews;
            if (UpdateData != null)
            {
                try
                {
                    UpdateNews = news;
                    UpdateNews = _mapper.Map<NewsUpdataResource, NewsModel>(UpdateData);
                    UpdateNews.CreateTime = DateTime.Now;
                    _Context.Entry(news).CurrentValues.SetValues(UpdateNews);
                    await _Context.SaveChangesAsync();
                }
                catch
                {
                    return "寫入資料庫失敗";
                }
                return "修改成功";

            }
            return "無此資料";
        }

    }
}