using System;
using CoreAPi.Models;
using Microsoft.EntityFrameworkCore;
using Models.EntityConfiguration;
using Mywebsite.Models;

namespace CoreAPi
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }
        public DbSet<NewsModel> NewsModel { get; set; }
        public DbSet<MemberModel> MemberModel { get; set; }

        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<CourseTypeModel> CourseTypes { get; set; }

        // 線上學習Model
        public DbSet<LearnOnlineModel> LearningOnline { get; set; }

        // 檔案上傳Model
        public DbSet<FileUploadModel> FileUpload { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MembersEntityConfigration());
            base.OnModelCreating(modelBuilder);

            //CourseModel外來鍵設定
            modelBuilder.ApplyConfiguration(new CourseEntityConfigration());
            base.OnModelCreating(modelBuilder);

            //FileUpload外來鍵設定
            modelBuilder.ApplyConfiguration(new FileUploadModelEntityConfigration());
            base.OnModelCreating(modelBuilder);

            #region seed  
            //newsmodel seed
            modelBuilder.Entity<NewsModel>().HasData(new NewsModel
            {
                News_Id = 1,
                News_Title = "台中科大"
             ,
                News_Content = "news",
                CreateTime = DateTime.Parse("2020-04-20")
            });

            //learningonlinemodel seed
            modelBuilder.Entity<LearnOnlineModel>().HasData(new LearnOnlineModel
            {
                Video_Id = 1,
                Coursel_TypeId = "1"
             ,
                Coursel_Name = "coursel",
                Video_Size = 30,
                Video_Type = "video",
                Video_Url = "123",
                Video_Time = 456,
                CreateTime = DateTime.Parse("2020-04-20")
            });

            //Fileuploadmodel seed 關聯 member
            // modelBuilder.Entity<FileUploadModel>().HasData(new FileUploadModel {File_Id = "2135641", FileName= "1"
            //  ,Member_Id= "1",FileSize= 30,FileType="file",FileUrl="456",CreateTime = DateTime.Parse("2020-04-20")});

            //Coursemodel seed 
            modelBuilder.Entity<CourseModel>().HasData(new CourseModel
            {
                Course_Id = 1,
                CourseName = "course"
             ,
                CourseAddress = "address",
                CourseTypeId = 1,
                CourseStatusId = 0,
                CourseTime = DateTime.Parse("2020-04-20")
            });

            //CourseTypemodel seed 
            modelBuilder.Entity<CourseTypeModel>().HasData(new CourseTypeModel { CourseTypeId = 1, CourseType = "coursetype" });
            #endregion


        }
    }
}