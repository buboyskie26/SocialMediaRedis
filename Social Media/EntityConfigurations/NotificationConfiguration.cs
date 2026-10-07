using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media.Entities;

namespace Social_Media.EntityConfigurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            //builder.ToTable("Notifications", "SocialMediaDB");
            builder.ToTable("Notifications", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.RecipientId)
                .IsRequired();

            builder.Property(x => x.LatestActorId)
                .IsRequired();

            builder.Property(x => x.GroupKey)
                .IsRequired()
                .HasMaxLength(200); // Maps automatically to character varying(200)

            builder.Property(x => x.ActorCount)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(x => x.EntityId)
                .IsRequired();

            builder.Property(x => x.NotificationType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.NotificationEntityType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.LastUpdatedAt);

            builder.HasOne(x => x.Recipient)
                .WithMany()
                .HasForeignKey(x => x.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LatestActor)
                .WithMany()
                .HasForeignKey(x => x.LatestActorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}