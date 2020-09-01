using CoreAPi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Models.EntityConfiguration
{
    public class MembersEntityConfigration : IEntityTypeConfiguration<MemberModel>
    {
        public void Configure(EntityTypeBuilder<MemberModel> builder)
        {
            builder.ToTable("member");
            builder.HasIndex(u =>u.Email).IsUnique();
        }
    }
}