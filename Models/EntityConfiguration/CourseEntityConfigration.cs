using CoreAPi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mywebsite.Models;

namespace Models.EntityConfiguration
{
    public class CourseEntityConfigration : IEntityTypeConfiguration<CourseModel>
    {
        public void Configure(EntityTypeBuilder<CourseModel> builder)
        {
            builder.ToTable("Courses");//對應資料表的資料結構

            //Course  一對多 CourseTypes
            builder.HasOne(c => c.CourseTypes)
                   .WithMany(a => a.Courses)
                   .HasForeignKey(c => c.CourseTypeId);
        }
    }
}