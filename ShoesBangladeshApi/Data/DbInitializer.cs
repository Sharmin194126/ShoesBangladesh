using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
                context.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[SystemSettings]') AND name = 'HeroImageUrl') BEGIN ALTER TABLE [SystemSettings] ADD [HeroImageUrl] nvarchar(max) NOT NULL DEFAULT ''; END");
                context.Database.ExecuteSqlRaw("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[SystemSettings]') AND name = 'HeroBgImageUrl') BEGIN ALTER TABLE [SystemSettings] ADD [HeroBgImageUrl] nvarchar(max) NOT NULL DEFAULT ''; END");
            }
            catch { /* Ignore if already exists */ }

            // 1. Seed Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Men", Description = "Footwear for men" },
                    new Category { Name = "Women", Description = "Footwear for women" },
                    new Category { Name = "Kids", Description = "Footwear for children" }
                );
                context.SaveChanges();
            }
            var categories = context.Categories.ToList();

            // 2. Seed SystemSettings
            if (!context.SystemSettings.Any())
            {
                context.SystemSettings.Add(new SystemSettings());
                context.SaveChanges();
            }

            // 3. Seed Users
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { FullName = "Admin User", Email = "admin@gmail.com", Password = "123", Role = "Admin", Phone = "01700000000", Address = "Dhaka, Bangladesh" },
                    new User { FullName = "Karim Ahmed", Email = "karim@example.com", Password = "123", Role = "Customer", Phone = "01812345678", Address = "Dhanmondi, Dhaka" }
                );
                context.SaveChanges();
            }
            var adminUser = context.Users.First(u => u.Role == "Admin");
            var customerUser = context.Users.First(u => u.Role == "Customer");

            // 4. Seed Customers
            if (!context.Customers.Any())
            {
                context.Customers.Add(new Customer 
                { 
                    UserId = customerUser.Id, 
                    Name = customerUser.FullName, 
                    Email = customerUser.Email, 
                    PhoneNumber = customerUser.Phone, 
                    Status = "Active" 
                });
                context.SaveChanges();
            }

            // 5. Seed Employees
            if (!context.Employees.Any())
            {
                context.Employees.Add(new Employee 
                { 
                    UserId = adminUser.Id, 
                    Name = "Sabbir Hossain", 
                    Email = "sabbir@shoes.com", 
                    PhoneNumber = "01911111111", 
                    Designation = "Sales Manager", 
                    Salary = 25000, 
                    JoiningDate = DateTime.Now.AddMonths(-6), 
                    Status = "Active" 
                });
                context.SaveChanges();
            }
            var employee = context.Employees.First();

            // 6. Seed Products
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Name = "Sport Runner X1", Description = "High performance running shoes", Price = 2500, DiscountPrice = 2200, StockQuantity = 50, CategoryId = categories[0].Id, ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400", IsFeatured = true },
                    new Product { Name = "Classic Oxford", Description = "Elegant leather shoes", Price = 3500, DiscountPrice = 3200, StockQuantity = 30, CategoryId = categories[0].Id, ImageUrl = "https://images.unsplash.com/photo-1614252235316-8c857d38b5f4?w=400", IsFeatured = true },
                    new Product { Name = "Summer Breeze Sandals", Description = "Comfortable sandals", Price = 1200, DiscountPrice = 1000, StockQuantity = 100, CategoryId = categories[1].Id, ImageUrl = "https://images.unsplash.com/photo-1603487759130-10029bc04294?w=400", IsEidOffer = true }
                );
                context.SaveChanges();
            }
            var allProducts = context.Products.ToList();

            // 7. Seed Orders
            if (!context.Orders.Any())
            {
                var order1 = new Order 
                { 
                    UserId = customerUser.Id, 
                    OrderDate = DateTime.Now.AddDays(-2), 
                    TotalAmount = 4700, 
                    Status = "Delivered", 
                    PaymentStatus = "Paid", 
                    PaymentMethod = "SSLCommerz", 
                    ShippingAddress = "House 12, Road 5, Dhanmondi, Dhaka",
                    City = "Dhaka",
                    AssignedEmployeeId = employee.Id
                };
                var order2 = new Order 
                { 
                    UserId = customerUser.Id, 
                    OrderDate = DateTime.Now.AddDays(-1), 
                    TotalAmount = 2500, 
                    Status = "Pending", 
                    PaymentStatus = "Pending", 
                    PaymentMethod = "Cash on Delivery", 
                    ShippingAddress = "House 12, Road 5, Dhanmondi, Dhaka",
                    City = "Dhaka"
                };

                context.Orders.AddRange(new List<Order> { order1, order2 });
                context.SaveChanges();

                var details = new List<OrderDetails>
                {
                    new OrderDetails { OrderId = order1.Id, ProductId = allProducts[0].Id, Quantity = 1, Price = 2500, PriceWithVat = 2700, VatAmount = 200 },
                    new OrderDetails { OrderId = order1.Id, ProductId = allProducts[2].Id, Quantity = 2, Price = 1000, PriceWithVat = 1000, VatAmount = 0 },
                    new OrderDetails { OrderId = order2.Id, ProductId = allProducts[0].Id, Quantity = 1, Price = 2500, PriceWithVat = 2500, VatAmount = 0 }
                };
                context.OrderDetails.AddRange(details);
                context.SaveChanges();
            }
        }
    }
}
