using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mywebsite.Resources.Response
{
    public class MemberChangePasswordResource
    {
        [Required]
        [EmailAddress]
        [StringLength(200)]
        //會員信箱
        public string Email { get; set; }
        [Required]
        //舊密碼
        public string Password { get; set; }
        [Required]
        //新密碼
        public string NewPassword { get; set; }
    }
}