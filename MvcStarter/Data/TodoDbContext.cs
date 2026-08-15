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
                 .IsRequired()
                 .HasDefaultValue(TodoPriority.Medium);

                entity.HasData(
                    new TodoItem(1, "Learn C#", TodoPriority.High, true),  
                    new TodoItem(2, "Build MVC app", TodoPriority.Medium, false),  
                    new TodoItem(3, "Go for a walk", TodoPriority.Low, false)  
                );
            });
        }
    }
}
