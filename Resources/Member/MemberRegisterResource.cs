using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mywebsite.Resources.Response
{
    public class MemberRegisterResource
    {
        [Required]
        [EmailAddress]
        [StringLength(200)]
        //會員信箱
        public string Email { get; set; }
        [Required]
        //會員性別
        public string Gender { get; set; }
        [Required]
        //會員密碼
        public string Password { get; set; }
        [StringLength(20)]
        [Required]
        //會員名稱
        public string Name { get; set; }
        [StringLength(100)]
        [Required]
        //會員地址
        public string Address { get; set; }
        [Required]
        //服務單位
        public string Service_Unit { get; set; }
        [Required]
        //族別
        public string Race { get; set; }
        [Required]
        //職稱
        public string Position { get; set; }
        
        //電話
        public string Phone { get; set; }
    }
}