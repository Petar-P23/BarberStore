using System.Xml.XPath;
using BarberStore.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarberStore.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<OrderProduct>().HasKey(e => new { e.OrderId, e.ProductId });
            builder.Entity<CartProduct>().HasKey(e => new { e.CartId, e.ProductId });

            builder.Entity<ApplicationUser>()
                .HasOne(e => e.Cart)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
    }
}