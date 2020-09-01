using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mywebsite.Models;
using Mywebsite.Resources.Request;
using Mywebsite.Resources.Response;
using Mywebsite.Services;
using MyWebsite.Models;

namespace Mywebsite.Controllers
{
    [Route("api/[Controller]")]
    [Produces("application/json")]
    public class CourseController : ControllerBase
    {
        private readonly CourseServices _CourseServices;

        public CourseController(IMapper _mapper, AppDBContext DBContext)
        {
            _CourseServices = new CourseServices(_mapper, DBContext);
        }


        /// <summary>
        /// 新增課程資料
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Post /api/Course
        ///     {
        ///         "IFromForm" = "u"
        ///     }
        ///     
        /// </remarks>
        /// <param name= "resources"></param>
        /// <returns>Insert new File in Course</returns>
        /// <response code= "201">Insert Course Successful</response>
        /// <response code= "404">If the Course_Id is Null</response>
        //API路徑
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(201)] //201 資源新增成功
        [ProducesResponseType(404)] //請求的資源不存在
        public async Task<ActionResult> CreateAsync([FromBody]CourseCreateResources resources)
        {
            #region 新增課程
            string result = string.Empty;
            //判斷使用者是否登入
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            result = await _CourseServices.InsertCourseCreate(resources);

            if (result == "OK")
            {
                return Ok();
            }
            else
            {
                return BadRequest(new { msg = "輸入格式錯誤!" });
            }

            #endregion
        }

        /// <summary>
        /// 讀取課程資料
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Get /api/Course
        ///     {
        ///         "Course_Id" = {id},
        ///         "CourseTime" = "CourseTime",
        ///         "CourseName" = "CourseName",
        ///         "CourseAddress" = "CourseAddress",
        ///         "CourseTypeId" = {CourseTypeId},
        ///         "CourseStatusId" = {CourseStatusId}
        ///     }
        ///     
        /// </remarks>
        /// <param name= "Course_Id"></param>
        /// <returns>Read Courses From Course</returns>
        /// <response code= "200">Read Courses Successful</response>
        /// <response code= "404">If the Course_Id is Null</response>
        //API路徑
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(CourseResources), 200)] //成功
        [ProducesResponseType(typeof(CourseResources), 404)] //請求的資源不存在
        public async Task<ActionResult<List<CourseResources>>> ReadAllAsync(int Course_Id)
        {
            #region 課程頁面
            List<CourseResources> course = new List<CourseResources>();

            //讀取CourseModel資料
            course = await _CourseServices.GetDataList();
            return Ok(course);
            #endregion
        }

        /// <summary>
        /// 修改課程資料
        /// </summary>
        /// <remarks>
        /// Sample Request：
        /// 
        ///     Put /api/Course/{Course_Id}
        ///     {
        ///         "Course_Id" = Course_Id
        ///     }
        ///     
        /// </remarks>
        /// <param name= "Course_Id"></param>
        /// <param name= "resources"></param>
        /// <returns>Update new File in Course</returns>
        /// <response code= "204">Update Course Successful</response>
        /// <response code= "404">If the Course_Id is Null</response>
        [HttpPut("{Course_Id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CourseUpdateResources), 204)] //請求成功，沒有返回內容
        [ProducesResponseType(typeof(CourseUpdateResources), 404)] //請求的資源不存在
        public async Task<IActionResult> UpdateAsync([FromRoute]int Course_Id, [FromBody]CourseUpdateResources resources)
        {
            #region 修改課程
            string result = string.Empty;
            //判斷使用者是否登入
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            resources.Course_Id = Course_Id;
            result = await _CourseServices.UpdateCourse(Course_Id, resources);

            if (result == "OK")
            {
                return Ok(new { msg = "修改成功" });
            }
            else
            {
                return BadRequest(new { msg = "輸入格式錯誤!" });
            }

            #endregion
        }

    }
}
