using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ecommerce_db_api.Models.orders;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_db_api.EFCore
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId); // Primary Key configuration
                entity.Property(u => u.UserId).HasDefaultValueSql("uuid_generate_v4()"); // Generate UUID for new records
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Address).HasMaxLength(255);
                entity.Property(u => u.IsAdmin).HasDefaultValue(false);
                entity.Property(u => u.IsBanned).HasDefaultValue(false);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(category => category.CategoryId); // Primary Key configuration
                entity.Property(category => category.CategoryId).HasDefaultValueSql("uuid_generate_v4()"); // Generate UUID for new records
                entity.Property(category => category.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(category => category.Name).IsUnique();
                entity.Property(category => category.Slug).IsRequired().HasMaxLength(100);
                entity.Property(category => category.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId); // Primary Key
                entity.Property(p => p.ProductId).HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

            // modelBuilder.Entity<User>()
            // .HasMany(u => u.orders)
            // .WithOne(o => o.User)
            // .HasForeignKey(o => o.UserId)
            // .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many relationship between Order and Product using OrderProduct as join table
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.OrderId, op.ProductId });  // Composite primary key

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.ProductId);


        }
    }
}