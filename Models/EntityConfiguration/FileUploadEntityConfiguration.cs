using CoreAPi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Models.EntityConfiguration
{
    public class FileUploadModelEntityConfigration : IEntityTypeConfiguration<FileUploadModel>
    {
        public void Configure(EntityTypeBuilder<FileUploadModel> builder)
        {
            builder.ToTable("FileUploads");//對應資料表的資料結構

            //外來鍵
            builder.HasOne(c => c.members)
                   .WithMany(a => a.FileUploads)
                   .HasForeignKey(c => c.Member_Id);
        }
    }
}
