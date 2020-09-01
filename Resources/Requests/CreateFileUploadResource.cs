using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Mywebsite.Resources.Requests
{
    public class CreateFileUploadResource
    {
        // 會員Id
        [Required]
        public string Member_Id { get; set; }
        // 上傳資料
        public IFormFile UploadFile { get; set; }
    }
}