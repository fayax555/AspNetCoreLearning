using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(user => user.Name).HasMaxLength(100);
                entity.Property(user => user.Email).HasMaxLength(500);
                entity.HasIndex(user => user.Email).IsUnique();
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(post => post.Title).HasMaxLength(1000);

                entity.HasOne(post => post.User)
                    .WithMany(user => user.Posts)
                    .HasForeignKey(post => post.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(comment => comment.Post)
                    .WithMany(post => post.Comments)
                    .HasForeignKey(comment => comment.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(comment => comment.User)
                    .WithMany(user => user.Comments)
                    .HasForeignKey(comment => comment.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
