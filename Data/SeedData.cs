using PRN232.Backend.Models;

namespace PRN232.Backend.Data;

public static class SeedData
{
    public static void EnsureSeedData(AppDbContext db)
    {
        // Đảm bảo có dữ liệu Products trước
        if (!db.Products.Any())
        {
            var products = new List<Product>
            {
                new Product { Name = "Classic White T-Shirt", Description = "Premium cotton t-shirt with a classic fit. Perfect for everyday wear.", Price = 29.99m, Image = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=500&h=500&fit=crop", Category = "T-Shirts", Stock = 50 },
                new Product { Name = "Slim Fit Jeans", Description = "Modern slim fit jeans with stretch fabric for comfort and style.", Price = 79.99m, Image = "https://images.unsplash.com/photo-1542272604-787c3835535d?w=500&h=500&fit=crop", Category = "Jeans", Stock = 30 },
                new Product { Name = "Leather Jacket", Description = "Genuine leather jacket with premium finish. A timeless classic.", Price = 299.99m, Image = "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=500&h=500&fit=crop", Category = "Jackets", Stock = 15 },
                new Product { Name = "Summer Dress", Description = "Light and breezy summer dress perfect for warm weather.", Price = 59.99m, Image = "https://images.unsplash.com/photo-1595777457583-95e059d581b8?w=500&h=500&fit=crop", Category = "Dresses", Stock = 25 },
                new Product { Name = "Casual Sneakers", Description = "Comfortable sneakers for everyday activities. Lightweight and stylish.", Price = 89.99m, Image = "https://images.unsplash.com/photo-1549298916-b41d501d3772?w=500&h=500&fit=crop", Category = "Shoes", Stock = 40 },
                new Product { Name = "Wool Sweater", Description = "Cozy wool sweater to keep you warm during cold days.", Price = 69.99m, Image = "https://images.unsplash.com/photo-1576566588028-4147f3842f27?w=500&h=500&fit=crop", Category = "Sweaters", Stock = 20 },
                new Product { Name = "Denim Jacket", Description = "Classic denim jacket that never goes out of style.", Price = 89.99m, Image = "https://images.unsplash.com/photo-1543076447-215ad9ba6923?w=500&h=500&fit=crop", Category = "Jackets", Stock = 18 },
                new Product { Name = "Striped Polo Shirt", Description = "Elegant polo shirt with classic stripes. Perfect for casual occasions.", Price = 45.99m, Image = "https://images.unsplash.com/photo-1586363104862-3a5e2ab60d99?w=500&h=500&fit=crop", Category = "T-Shirts", Stock = 35 },
                new Product { Name = "Cargo Pants", Description = "Functional cargo pants with multiple pockets. Durable and comfortable.", Price = 65.99m, Image = "https://images.unsplash.com/photo-1624378439575-d8705ad7ae80?w=500&h=500&fit=crop", Category = "Pants", Stock = 28 },
                new Product { Name = "Floral Blouse", Description = "Beautiful floral print blouse for a feminine touch.", Price = 49.99m, Image = "https://images.unsplash.com/photo-1564859228273-274232fdb516?w=500&h=500&fit=crop", Category = "Tops", Stock = 22 },
                new Product { Name = "Running Shoes", Description = "High-performance running shoes with excellent cushioning.", Price = 129.99m, Image = "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=500&h=500&fit=crop", Category = "Shoes", Stock = 32 },
                new Product { Name = "Hooded Sweatshirt", Description = "Comfortable hoodie perfect for lounging or casual outings.", Price = 55.99m, Image = "https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=500&h=500&fit=crop", Category = "Sweaters", Stock = 45 }
            };
            
            db.Products.AddRange(products);
            db.SaveChanges(); // Lưu products trước
            Console.WriteLine("Đã thêm dữ liệu sản phẩm vào database");
        }

        // Đảm bảo có dữ liệu Users
        if (!db.Users.Any())
        {
            var u1 = new User { Email = "user@example.com", Name = "John Doe", CreatedAt = DateTime.Parse("2025-01-01T00:00:00Z").ToUniversalTime() };
            u1.PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123");
            var u2 = new User { Email = "admin@example.com", Name = "Admin User", CreatedAt = DateTime.Parse("2025-01-01T00:00:00Z").ToUniversalTime() };
            u2.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
            
            db.Users.AddRange(u1, u2);
            db.SaveChanges(); // Lưu users trước orders
            Console.WriteLine("Đã thêm dữ liệu người dùng vào database");
        }

        // Đảm bảo có dữ liệu Orders (chỉ tạo khi đã có Products và Users)
        if (!db.Orders.Any() && db.Products.Any() && db.Users.Any())
        {
            // Lấy IDs thực tế từ database
            var productIds = db.Products.Take(6).Select(p => p.Id).ToList();
            var userIds = db.Users.Take(1).Select(u => u.Id).ToList();

            if (productIds.Count >= 6 && userIds.Count >= 1)
            {
                var order1 = new Order
                {
                    UserId = userIds[0],
                    TotalAmount = 359.97m,
                    Status = "delivered",
                    CreatedAt = DateTime.Parse("2025-09-01T10:30:00Z").ToUniversalTime()
                };
                order1.OrderProducts.Add(new OrderProduct { ProductId = productIds[0], Quantity = 2, Price = 29.99m });
                order1.OrderProducts.Add(new OrderProduct { ProductId = productIds[2], Quantity = 1, Price = 299.99m });

                var order2 = new Order
                {
                    UserId = userIds[0],
                    TotalAmount = 89.99m,
                    Status = "shipped",
                    CreatedAt = DateTime.Parse("2025-09-10T14:20:00Z").ToUniversalTime()
                };
                order2.OrderProducts.Add(new OrderProduct { ProductId = productIds[4], Quantity = 1, Price = 89.99m });

                var order3 = new Order
                {
                    UserId = userIds[0],
                    TotalAmount = 149.98m,
                    Status = "pending",
                    CreatedAt = DateTime.Parse("2025-10-01T09:15:00Z").ToUniversalTime()
                };
                order3.OrderProducts.Add(new OrderProduct { ProductId = productIds[1], Quantity = 1, Price = 79.99m });
                order3.OrderProducts.Add(new OrderProduct { ProductId = productIds[5], Quantity = 1, Price = 69.99m });

                db.Orders.AddRange(order1, order2, order3);
                db.SaveChanges(); // Lưu orders cuối cùng
                Console.WriteLine("Đã thêm dữ liệu đơn hàng vào database");
            }
        }
    }
}
