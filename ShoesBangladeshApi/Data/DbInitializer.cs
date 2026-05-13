using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;


namespace ShoesBangladesh.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Manually add columns if they don't exist (Fix for migration issues)
            try
            {
                context.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'Phone') BEGIN ALTER TABLE [Users] ADD [Phone] nvarchar(max) NOT NULL DEFAULT ''; END");
                context.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Users]') AND name = 'Address') BEGIN ALTER TABLE [Users] ADD [Address] nvarchar(max) NOT NULL DEFAULT ''; END");
            }
            catch { /* Ignore if already exists or other SQL issues */ }

            if (context.Products.Any())

            {
                return;   // DB has been seeded
            }

            var categories = new Category[]
            {
                new Category { Name = "Men", Description = "Footwear for men" },
                new Category { Name = "Women", Description = "Footwear for women" },
                new Category { Name = "Kids", Description = "Footwear for children" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // Seed Users
            if (!context.Users.Any())
            {
                var users = new User[]
                {
                    new User { FullName = "Admin User", Email = "admin@shoes.com", Password = "123", Role = "Admin", Phone = "01700000000", Address = "Dhaka, Bangladesh" },
                    new User { FullName = "Customer User", Email = "user@shoes.com", Password = "123", Role = "Customer", Phone = "01800000000", Address = "Chittagong, Bangladesh" }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            var products = new Product[]

            {
                new Product { Name = "Sport Runner", Description = "High performance running shoes", Price = 2500, StockQuantity = 50, CategoryId = categories[0].Id, ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400" },
                new Product { Name = "Classic Leather", Description = "Elegant leather shoes for formal events", Price = 3500, StockQuantity = 30, CategoryId = categories[0].Id, ImageUrl = "https://images.unsplash.com/photo-1614252235316-8c857d38b5f4?w=400" },
                new Product { Name = "Summer Sandals", Description = "Comfortable sandals for hot weather", Price = 1200, StockQuantity = 100, CategoryId = categories[1].Id, ImageUrl = "https://images.unsplash.com/photo-1603487759130-10029bc04294?w=400" }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
