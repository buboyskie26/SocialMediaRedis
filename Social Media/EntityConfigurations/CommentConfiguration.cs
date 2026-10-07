using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media.Entities;

namespace Social_Media.EntityConfigurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            //builder.ToTable("Comments", "SocialMediaDB");
            builder.ToTable("Comments", "dbo");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.PostId)
                .IsRequired();

            builder.Property(c => c.AuthorId)
                .IsRequired();

            builder.Property(c => c.ParentCommentId)
                .IsRequired(false);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(1000); // Maps automatically to character varying(1000)

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Uses PostgreSQL current timestamp

            builder.Property(c => c.UpdatedAt); // Maps to timestamp automatically

            builder.Property(c => c.IsEdited)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(c => c.PostId)
                .HasDatabaseName("IX_Comments_PostId");

            builder.HasIndex(c => c.AuthorId)
                .HasDatabaseName("IX_Comments_AuthorId");

            builder.HasIndex(c => c.ParentCommentId)
                .HasDatabaseName("IX_Comments_ParentCommentId");

            builder.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
