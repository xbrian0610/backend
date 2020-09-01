using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPi.Models
{
    [Table("LearnOnlines")]
    public class LearnOnlineModel
    {
        // 影片編號
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Video_Id { get; set; }
        // 課程編號類型
        [Required]
        public string Coursel_TypeId { get; set; }
        // 影片名稱
        [Required(ErrorMessage = "請輸入課程名稱")]
        [StringLength(20, ErrorMessage = "課程名稱不可超過20字元")]
        public string Coursel_Name { get; set; }
        // 影片大小
        public long Video_Size { get; set; }
        // 影片類型
        public string Video_Type { get; set; }
        // 影片位址
        public string Video_Url { get; set; }
        // 影片長度
        [Required(ErrorMessage = "請輸入影片長度")]
        [Range(typeof(double), "0", "24", ErrorMessage = "以小時為單位(0-24)")]
        public double Video_Time { get; set; }
        // 課程影片上傳時間
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreateTime { get; set; }
    }
}