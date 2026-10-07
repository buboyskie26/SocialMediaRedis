using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media.Entities;
using Social_Media.Enums;

namespace Social_Media.EntityConfigurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            //builder.ToTable("Posts", "SocialMediaDB");
            builder.ToTable("Posts", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.AuthorId)
               .IsRequired();

            builder.Property(p => p.Content)
                      .IsRequired(); // Maps automatically to text

            builder.Property(p => p.Visibility)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(PostVisibility.Public);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.UpdatedAt);

            builder.Property(p => p.IsEdited)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(p => p.AuthorId)
                .HasDatabaseName("IX_Posts_AuthorId");

            builder.HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}