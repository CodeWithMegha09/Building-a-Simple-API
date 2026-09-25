using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Simple logging middleware to print request path
app.Use(async (context, next) =>
{
    Console.WriteLine("Incoming Request: " + context.Request.Path);
    await next();
});

// User list with initial sample data
var userList = new List<User>
{
    new User { Id = 1, Name = "Rahul", Email = "rahul@gmail.com", Age = 22 },
    new User { Id = 2, Name = "Priya", Email = "priya@gmail.com", Age = 20 }
};

// Root home route
app.MapGet("/", () => "User API is running!");

// 1. GET ALL USERS
app.MapGet("/api/users", () => userList);

// 2. GET USER BY ID
app.MapGet("/api/users/{id}", (int id) =>
{
    foreach (var user in userList)
    {
        if (user.Id == id)
        {
            return Results.Ok(user);
        }
    }
    return Results.NotFound("User not found with ID: " + id);
});

// 3. POST - ADD NEW USER (With simple validation)
// Created with help of Copilot prompt for user validation
app.MapPost("/api/users", ([FromBody] User newUser) =>
{
    // Validation: name and email check
    if (string.IsNullOrEmpty(newUser.Name))
    {
        return Results.BadRequest("Name is required!");
    }

    if (string.IsNullOrEmpty(newUser.Email) || !newUser.Email.Contains("@"))
    {
        return Results.BadRequest("Please provide a valid email address!");
    }

    if (newUser.Age <= 0)
    {
        return Results.BadRequest("Age must be greater than 0!");
    }

    // Auto generate ID
    newUser.Id = userList.Count + 1;
    userList.Add(newUser);

    return Results.Ok(newUser);
});

// 4. PUT - UPDATE USER
app.MapPut("/api/users/{id}", (int id, [FromBody] User updatedUser) =>
{
    User existingUser = null;
    
    foreach (var u in userList)
    {
        if (u.Id == id)
        {
            existingUser = u;
            break;
        }
    }

    if (existingUser == null)
    {
        return Results.NotFound("User not found!");
    }

    // Validation check before update
    if (string.IsNullOrEmpty(updatedUser.Name))
    {
        return Results.BadRequest("Name cannot be empty!");
    }

    existingUser.Name = updatedUser.Name;
    existingUser.Email = updatedUser.Email;
    existingUser.Age = updatedUser.Age;

    return Results.Ok(existingUser);
});

// 5. DELETE - DELETE USER
app.MapDelete("/api/users/{id}", (int id) =>
{
    User userToDelete = null;

    foreach (var u in userList)
    {
        if (u.Id == id)
        {
            userToDelete = u;
            break;
        }
    }

    if (userToDelete == null)
    {
        return Results.NotFound("User not found!");
    }

    userList.Remove(userToDelete);
    return Results.Ok("User deleted successfully.");
});

app.Run();

// User model
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public int Age { get; set; }
}
