using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoesBangladesh.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

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

            var customerUser = context.Users.FirstOrDefault(u => u.Role == "Customer");

            // 4. Seed Customers
            if (customerUser != null && !context.Customers.Any())
            {
                context.Customers.Add(new Customer 
                { 
                    UserId = customerUser.Id, 
                    Name = customerUser.FullName, 
                    Email = customerUser.Email, 
                    PhoneNumber = customerUser.Phone,
                    CreatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
            }

            // NOTE: Product, Employee and Order seeding removed to allow user to manage their own catalogue.
        }
    }
}
