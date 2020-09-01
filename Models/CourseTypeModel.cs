using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mywebsite.Models
{
    [Table("CourseTypes")]
    public class CourseTypeModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //課程類型Id
        public int CourseTypeId { get; set; }

        [Required]
        //課程類型
        public string CourseType { get; set; }
        //外部關聯
        public virtual ICollection<CourseModel> Courses { get; set; }
    }
}