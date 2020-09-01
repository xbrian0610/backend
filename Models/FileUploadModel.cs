using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPi.Models
{
    [Table("FileUploads")]
    public class FileUploadModel
    {
        // 檔案編號
        [Key]
        public string File_Id { get; set; }
        // 檔案名稱
        [Required]
        public string FileName { get; set; }
        //會員編號
        [Required]
        public string Member_Id { get; set; }
        // 檔案大小
        [Required]
        public long FileSize { get; set; }
        // 檔案類型
        [Required]
        public string FileType { get; set; }
        // 檔案位址
        [Required]
        public string FileUrl { get; set; }
        // 檔案上傳時間
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreateTime { get; set; }

        //外來鍵
        [ForeignKey("Member_Id")]
        //外部關聯
        public virtual MemberModel members { get; set; }
    }
}