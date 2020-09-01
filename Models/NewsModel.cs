using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPi.Models
{
    [Table("news")]
    public class NewsModel
    {   //最新消息ID
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int News_Id{ get; set;}
        [Required]
        //最新消息標題
        public string News_Title{ get; set;}
        [Required]
        //最新消息內容
        public string News_Content{get;set;}
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //最新消息寫入時間
        public DateTime CreateTime{get;set;}         
    }
}