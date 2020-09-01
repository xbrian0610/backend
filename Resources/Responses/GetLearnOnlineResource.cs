using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mywebsite.Resources.Responses
{
    [Table("LearnOnline")]
    public class GetLearnOnlineResource
    {
        // 影片編號
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Video_Id { get; set; } 
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
    }
}