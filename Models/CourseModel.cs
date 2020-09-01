using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;

namespace Mywebsite.Models
{
    [Table("Courses")]
    public class CourseModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //課程編號
        public int Course_Id { get; set; }

        [Required]
        //課程
        public string CourseName { get; set; }
        [Required]
        //課程地址
        public string CourseAddress { get; set; }
        //課程類型Id
        public int CourseTypeId { get; set; }
        [Required]
        //課程狀態
        public int CourseStatusId { get; set; }
        

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //課程時間
        public DateTime CourseTime { get; set; }
        
        //外來鍵
        [ForeignKey("CourseTypeId")]
        //外部關聯
        public virtual CourseTypeModel CourseTypes { get; set; }

        
    }
}