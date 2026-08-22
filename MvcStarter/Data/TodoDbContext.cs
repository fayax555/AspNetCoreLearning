using Microsoft.EntityFrameworkCore;
using MvcStarter.Models;

namespace MvcStarter.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        public DbSet<TodoItem> Todos => Set<TodoItem>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(todo => todo.Id);

                entity.Property(todo => todo.Title)
                 .IsRequired()
                 .HasMaxLength(100);

                entity.Property(todo => todo.Priority)
                 .IsRequired();

                entity.HasOne(todo => todo.Category)
                    .WithMany()
                    .HasForeignKey(todo => todo.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasData(
                    new TodoItem(1, "Learn C#", TodoPriority.High, true),
                    new TodoItem(2, "Build MVC app", TodoPriority.Medium, false),
                    new TodoItem(3, "Go for a walk", TodoPriority.Low, false)
                );
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(category => category.Id);

                entity.Property(category => category.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(category => category.Name)
                .IsUnique();

                entity.HasData(
                    new Category(1, "Work"),
                    new Category(2, "Personal")
                );
            });
        }
    }
}
