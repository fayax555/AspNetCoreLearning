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

                entity.HasData(
                    new TodoItem(1, "Learn C#", true),  
                    new TodoItem(2, "Build MVC app", false),  
                    new TodoItem(3, "Go for a walk", false)  
                );
            });
        }
    }
}
