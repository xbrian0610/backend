using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Mywebsite.Resources.Requests
{
    public class CreateLearnOnlineResource
    {
        // 影片編號類型
        [Required]
        public string Coursel_TypeId { get; set; }
        // 影片名稱
        [Required(ErrorMessage = "請輸入課程名稱")]
        [StringLength(20, ErrorMessage = "課程名稱不可超過20字元")]
        public string Coursel_Name { get; set; }
        // 影片長度
        [Required(ErrorMessage = "請輸入影片長度")]
        [Range(typeof(double), "0", "24", ErrorMessage = "以小時為單位(0-24)")]
        public double Video_Time { get; set; }
        // 上傳影片檔案
        public IFormFile Video { get; set; }
    }
}