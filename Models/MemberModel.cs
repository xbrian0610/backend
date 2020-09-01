using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAPi.Models
{
    [Table("members")]
    public class MemberModel
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Password { get; set; }
        [StringLength(20)]
        [Required]
        public string Name { get; set; }
        [StringLength(100)]
        [Required]
        public string Address { get; set; }
        [Required]
        [StringLength(20)]
        public string Service_Unit { get; set; }
        [Required]
        [StringLength(20)]
        public string Race { get; set; }
        [Required]
        [StringLength(20)]
        public string Position { get; set; }
        [Phone]
        [StringLength(20)]       
         public string Phone { get; set; }
        public string AuthCode_Register{ get; set; }
        public string AuthCode_Password{get;set;}
        public bool IsAdmin{get;set;}
        public bool EmailCheck { get; set; }

        //外部關聯
        public virtual ICollection<FileUploadModel> FileUploads { get; set; }
    }
}