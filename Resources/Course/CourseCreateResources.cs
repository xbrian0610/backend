using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System;
using Mywebsite.Models;


namespace Mywebsite.Resources.Response
{
    [Table("Course")]
    public class CourseCreateResources
    {

        [Required(ErrorMessage="年/月/日")]
        [DisplayFormat(DataFormatString = "{0:YYYY-MM-dd}", ApplyFormatInEditMode = true)]
        //課程時間
        public DateTime CourseTime { get; set; }

        [Required(ErrorMessage="請輸入課程名稱")]
        [StringLength(20,ErrorMessage="課程名稱不可超過20字元")]
        //課程
        public string CourseName { get; set; }
        [Required(ErrorMessage="請輸入課程地點")]
        //課程地址
        public string CourseAddress { get; set; }
        //課程類型Id
        [Required(ErrorMessage="請輸入課程類型")]
        public int CourseTypeId { get; set; }



        
    }
}