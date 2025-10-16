using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN232.Backend.Data;
using PRN232.Backend.Models;

namespace PRN232.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;

    public OrdersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _db.Orders.Include(o => o.OrderProducts).ToListAsync();
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest req)
    {
        var order = new Order
        {
            UserId = req.UserId,
            CreatedAt = DateTime.UtcNow,
            Status = "pending",
            TotalAmount = req.Products.Sum(p => p.Price * p.Quantity)
        };

        foreach (var p in req.Products)
        {
            order.OrderProducts.Add(new OrderProduct { ProductId = p.ProductId, Quantity = p.Quantity, Price = p.Price });
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = order.Id }, order);
    }
}

public record CreateOrderRequest(int UserId, List<OrderProductRequest> Products);
public record OrderProductRequest(int ProductId, int Quantity, decimal Price);
