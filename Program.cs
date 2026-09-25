using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Dummy product list (In-memory data)
var itemList = new List<Item>
{
    new Item { Id = 1, Name = "Laptop", Price = 50000 },
    new Item { Id = 2, Name = "Mobile", Price = 15000 },
    new Item { Id = 3, Name = "Headphones", Price = 2000 }
};

// Home page response
app.MapGet("/", () => "My Simple C# API Project");

// Get all items
app.MapGet("/api/items", () => itemList);

// Get single item by ID
app.MapGet("/api/items/{id}", (int id) =>
{
    var item = itemList.FirstOrDefault(p => p.Id == id);
    if (item == null)
    {
        return Results.NotFound("Item not found!");
    }
    return Results.Ok(item);
});

// Add new item
app.MapPost("/api/items", ([FromBody] Item newItem) =>
{
    newItem.Id = itemList.Count + 1;
    itemList.Add(newItem);
    return Results.Ok(newItem);
});

app.Run();

// Simple model class
public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}
