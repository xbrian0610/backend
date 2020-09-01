using System.ComponentModel.DataAnnotations;

namespace Mywebsite.Resources.Request
{
    public class NewsInsertResource
    {
        [Required]
        //最新消息標題
         public string News_Title{ get; set;}
        [Required]
        //最新消息內容
        public string News_Content{get;set;}
    }
}