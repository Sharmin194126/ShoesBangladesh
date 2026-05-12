using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;

namespace ShoesBangladesh.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

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
