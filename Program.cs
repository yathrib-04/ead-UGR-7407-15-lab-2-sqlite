using Microsoft.EntityFrameworkCore;
using PizzaStoreSQLite.Models;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext SQLite
builder.Services.AddDbContext<PizzaDb>(options =>
    options.UseSqlite("Data Source=pizzas.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => "Hello from SQLite version!");
}

app.MapGet("/pizzas", async (PizzaDb db) =>
    await db.Pizzas.ToListAsync());

app.MapGet("/pizza/{id}", async (PizzaDb db, int id) =>
    await db.Pizzas.FindAsync(id));

app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatePizza, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();

    pizza.Name = updatePizza.Name;
    pizza.Description = updatePizza.Description;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();

    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();
