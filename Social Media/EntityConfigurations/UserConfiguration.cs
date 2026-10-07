using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media.Entities;

namespace Social_Media.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "dbo");
            //builder.ToTable("Users", "SocialMediaDB");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50); // Maps automatically to character varying(50)

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256); // Maps automatically to character varying(256)

            builder.Property(u => u.PasswordHash)
                .IsRequired(); // Maps automatically to text

            builder.Property(u => u.DisplayName)
                .HasMaxLength(100); // Maps automatically to character varying(100)

            builder.Property(u => u.Bio)
                .HasMaxLength(500); // Maps automatically to character varying(500)

            //builder.Property(u => u.CreatedAt)
            //    .IsRequired()
            //    .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}