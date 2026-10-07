using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media.Entities;

namespace Social_Media.EntityConfigurations
{
    public class CommentReactionConfiguration : IEntityTypeConfiguration<CommentReaction>
    {
        public void Configure(EntityTypeBuilder<CommentReaction> builder)
        {
            //builder.ToTable("CommentReactions", "SocialMediaDB");
            builder.ToTable("CommentReactions", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.UserId)
               .IsRequired();

            builder.Property(p => p.CommentId)
              .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.ReactionType)
                .IsRequired()
                .HasConversion<int>();
        }
    }
}