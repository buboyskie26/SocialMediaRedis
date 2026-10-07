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
                        .HasMaxLength(50); //  

            builder.Property(u => u.CreatedBy)
                        .HasMaxLength(256); // 

            builder.Property(ua => ua.CreatedAt)
                        .IsRequired()
                        .HasDefaultValueSql("GETUTCDATE()");
            builder.HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}