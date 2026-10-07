using Microsoft.EntityFrameworkCore;
using Social_Media.Entities;
using System.Collections.Generic;

namespace Social_Media.Data
{
    public class SocialMediaDBContext : DbContext
    {
        public SocialMediaDBContext(DbContextOptions<SocialMediaDBContext> context) : base(context)
        {
              
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<UserAccess> UserAccess => Set<UserAccess>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<PostReaction> PostReactions => Set<PostReaction>();
        public DbSet<CommentReaction> CommentReaction => Set<CommentReaction>();
        public DbSet<Friendship> Friendships => Set<Friendship>();
        public DbSet<Notification> Notifications => Set<Notification>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("dbo");

            // This forces EF Core to query the "SocialMediaDB" schema instead of "public" or "dbo"
            //modelBuilder.HasDefaultSchema("cleanarch");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SocialMediaDBContext).Assembly);

        }
    }
}
