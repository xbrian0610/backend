using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mywebsite.Resources.Responses
{
    [Table("members")]
    public class GetMemberResource
    {
        [StringLength(20)]
        [Required]
        public string Name { get; set; }
        // 會員信箱
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }
        // public MemberModel member { get; set; }
    }
}