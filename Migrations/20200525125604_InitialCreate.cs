using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mywebsite.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseTypes",
                columns: table => new
                {
                    CourseTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTypes", x => x.CourseTypeId);
                });

            migrationBuilder.CreateTable(
                name: "LearnOnlines",
                columns: table => new
                {
                    Video_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Coursel_TypeId = table.Column<string>(nullable: false),
                    Coursel_Name = table.Column<string>(maxLength: 20, nullable: false),
                    Video_Size = table.Column<long>(nullable: false),
                    Video_Type = table.Column<string>(nullable: true),
                    Video_Url = table.Column<string>(nullable: true),
                    Video_Time = table.Column<double>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnOnlines", x => x.Video_Id);
                });

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: false),
                    Gender = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Address = table.Column<string>(maxLength: 100, nullable: false),
                    Service_Unit = table.Column<string>(maxLength: 20, nullable: false),
                    Race = table.Column<string>(maxLength: 20, nullable: false),
                    Position = table.Column<string>(maxLength: 20, nullable: false),
                    Phone = table.Column<string>(maxLength: 20, nullable: true),
                    AuthCode_Register = table.Column<string>(nullable: true),
                    AuthCode_Password = table.Column<string>(nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    EmailCheck = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "news",
                columns: table => new
                {
                    News_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    News_Title = table.Column<string>(nullable: false),
                    News_Content = table.Column<string>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_news", x => x.News_Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Course_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseName = table.Column<string>(nullable: false),
                    CourseAddress = table.Column<string>(nullable: false),
                    CourseTypeId = table.Column<int>(nullable: false),
                    CourseStatusId = table.Column<int>(nullable: false),
                    CourseTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Course_Id);
                    table.ForeignKey(
                        name: "FK_Courses_CourseTypes_CourseTypeId",
                        column: x => x.CourseTypeId,
                        principalTable: "CourseTypes",
                        principalColumn: "CourseTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileUploads",
                columns: table => new
                {
                    File_Id = table.Column<string>(nullable: false),
                    FileName = table.Column<string>(nullable: false),
                    Member_Id = table.Column<string>(nullable: false),
                    FileSize = table.Column<long>(nullable: false),
                    FileType = table.Column<string>(nullable: false),
                    FileUrl = table.Column<string>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUploads", x => x.File_Id);
                    table.ForeignKey(
                        name: "FK_FileUploads_member_Member_Id",
                        column: x => x.Member_Id,
                        principalTable: "member",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CourseTypes",
                columns: new[] { "CourseTypeId", "CourseType" },
                values: new object[] { 1, "coursetype" });

            migrationBuilder.InsertData(
                table: "LearnOnlines",
                columns: new[] { "Video_Id", "Coursel_Name", "Coursel_TypeId", "CreateTime", "Video_Size", "Video_Time", "Video_Type", "Video_Url" },
                values: new object[] { 1, "coursel", "1", new DateTime(2020, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 30L, 456.0, "video", "123" });

            migrationBuilder.InsertData(
                table: "news",
                columns: new[] { "News_Id", "CreateTime", "News_Content", "News_Title" },
                values: new object[] { 1, new DateTime(2020, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "news", "台中科大" });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Course_Id", "CourseAddress", "CourseName", "CourseStatusId", "CourseTime", "CourseTypeId" },
                values: new object[] { 1, "address", "course", 0, new DateTime(2020, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseTypeId",
                table: "Courses",
                column: "CourseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_Member_Id",
                table: "FileUploads",
                column: "Member_Id");

            migrationBuilder.CreateIndex(
                name: "IX_member_Email",
                table: "member",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "FileUploads");

            migrationBuilder.DropTable(
                name: "LearnOnlines");

            migrationBuilder.DropTable(
                name: "news");

            migrationBuilder.DropTable(
                name: "CourseTypes");

            migrationBuilder.DropTable(
                name: "member");
        }
    }
}
