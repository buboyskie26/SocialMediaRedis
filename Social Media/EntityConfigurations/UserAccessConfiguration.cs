using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media.Entities;

namespace Social_Media.EntityConfigurations
{
    public class UserAccessConfiguration : IEntityTypeConfiguration<UserAccess>
    {
        public void Configure(EntityTypeBuilder<UserAccess> builder)
        {
            //builder.ToTable("tblUserAccess", "SocialMediaDB");
            builder.ToTable("tblUserAccess", "dbo");

            builder.HasKey(u => u.UserAccessID);

            builder.Property(u => u.UserAccessID)
                .ValueGeneratedOnAdd();

            builder.Property(u => u.UserId)
                      .IsRequired();

            builder.Property(u => u.AccessType)
                        .IsRequired()
                        .HasMaxLength(50); // Maps automatically to character varying(50)

            builder.Property(u => u.CreatedBy)
                        .HasMaxLength(256); // Maps automatically to character varying(256)

            builder.Property(u => u.CreatedAt)
                        .IsRequired()
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}