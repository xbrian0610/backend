using System.ComponentModel.DataAnnotations;

namespace Mywebsite.Resources.Response
{
    public class MemberLoginResource
    {
        [Required]
        [EmailAddress]
        [StringLength(200)]
        //會員信箱
        public string Email { get; set; }
        [Required]
        //會員密碼
        public string Password { get; set; }
    }
}