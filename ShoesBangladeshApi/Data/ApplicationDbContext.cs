using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Models;

namespace ShoesBangladesh.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<FooterInfo> FooterInfos { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<DisplaySection> DisplaySections { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }
        public DbSet<HomeBanner> HomeBanners { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships or seed data here if needed
        }
    }
}
