using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using CoreAPi;
using Microsoft.EntityFrameworkCore;
using Mywebsite.Models;
using Mywebsite.Resources.Request;
using Mywebsite.Resources.Response;
using MyWebsite.Models;

namespace Mywebsite.Services
{
    public class CourseServices
    {
        private readonly AppDBContext _DBContext;
        private IMapper _mapper;
        public CourseServices(IMapper mapper, AppDBContext DBcontext)
        {
            _mapper = mapper ??
                throw new ArgumentNullException();
            _DBContext = DBcontext;
        }

        public async Task<string> InsertCourseCreate(CourseCreateResources resources)
        {
            #region 新增課程
            string result = string.Empty;
            try
            {
                CourseModel CourseModel = new CourseModel();
                //將存取的CourseCreateResources 存成 CourseModel
                CourseModel = _mapper.Map<CourseModel>(resources);
                CourseModel.CourseStatusId = 0;

                _DBContext.Add(CourseModel);
                await _DBContext.SaveChangesAsync();

                result = "OK";
            }
            catch (ArgumentException)
            {
                result = "Error";
            }

            return result;
            #endregion
        }

        public async Task<string> UpdateCourse(int Id, CourseUpdateResources UpdateData)
        {
            #region 修改課程
            string result = string.Empty;
            //讀取CourseModel的一筆資料
            var ReadData = await _DBContext.Courses.SingleOrDefaultAsync(x => x.Course_Id == Id);

            try
            {
                //修改讀取到的資料
                _DBContext.Entry(ReadData).CurrentValues.SetValues(UpdateData);
                await _DBContext.SaveChangesAsync();
                result = "OK";
            }
            catch (ArgumentException)
            {
                return result = "Error";
            }

            return result;
            #endregion
        }

        public async Task<List<CourseResources>> GetDataList()
        {
            #region 取得課程陣列資料 
            List<CourseResources> DataList = new List<CourseResources>();
            try
            {
                //將CourseModel資料存成List型態
                var Data = await _DBContext.Courses.ToListAsync();
                //將存取的List<CourseMoel>資料 存成 List<CourseResources>
                DataList = _mapper.Map<List<CourseResources>>(Data);
                return DataList;
            }
            catch (ArgumentException)
            {
                return DataList = null;
            }

            #endregion
        }

    }
}