using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreAPi.Models;

namespace Mywebsite.Resources.Responses
{
    [Table("FileUploads")]
    public class GetFileUploadResource
    {
        // 檔案編號
        [Key]
        public string File_Id { get; set; }
        // 檔案名稱
        public string FileName { get; set; }
        // 上傳時間
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreateTime { get; set; }
        // 對映 Member(Name, Email)
        public GetMemberResource members { get; set; }
    }
}