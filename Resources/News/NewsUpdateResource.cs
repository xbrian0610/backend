using System;
using System.ComponentModel.DataAnnotations;

namespace Mywebsite.Resources.Response
{
    public class NewsUpdataResource
    {
        //最新消息標題 
        public int news_Id{get;set;}
        public string News_Title{ get; set;}
        [Required]
        //最新消息內容
        public string News_Content{get;set;}
    }
}