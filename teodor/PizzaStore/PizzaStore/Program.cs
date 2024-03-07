using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore.Sqlite;
using PizzaStore.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";
builder.Services.AddSqlite<PizzaDb>(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PizzaStore API", Description = "Making the pizzas you love", Version = "v1" });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<PizzaDb>();
    dbContext.Database.EnsureCreated();
}

app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync())
    .WithTags("List of all pizzas");

app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id))
    .WithTags("Get pizza by ID");

app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
}).WithTags("Add a pizza");

app.MapPut("/pizza{id}", async (PizzaDb db, Pizza updatePizza, int id) =>
{
    var pizzaToUpdate = await db.Pizzas.FindAsync(id);
    if (pizzaToUpdate == null) return Results.NotFound();
    pizzaToUpdate.Name = updatePizza.Name;
    pizzaToUpdate.Description = updatePizza.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("Edit pizza");

app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
    var pizzaToDelete = await db.Pizzas.FindAsync(id);
    if (pizzaToDelete == null) return Results.NotFound();
    db.Pizzas.Remove(pizzaToDelete);
    await db.SaveChangesAsync();
    return Results.Ok();
}).WithTags("Delete a pizza");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();



