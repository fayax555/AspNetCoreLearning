using InventoryManager.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(category => category.Id);

                entity.Property(category => category.Name).IsRequired().HasMaxLength(50);

                entity.HasIndex(category => category.Name).IsUnique();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(product => product.Id);

                entity.Property(product => product.Name).IsRequired().HasMaxLength(100);

                entity.Property(product => product.Sku).IsRequired().HasMaxLength(30);

                entity.HasIndex(product => product.Sku).IsUnique();

                entity.Property(product => product.Description).HasMaxLength(500);

                entity.Property(product => product.UnitPrice).HasPrecision(18, 2);

                entity.HasOne(product => product.Category)
                    .WithMany(category => category.Products)
                    .HasForeignKey(product => product.CategoryId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
