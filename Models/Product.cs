namespace PRN232.Backend.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public string? Category { get; set; }
    public int Stock { get; set; }
}
