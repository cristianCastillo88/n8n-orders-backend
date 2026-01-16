using Microsoft.EntityFrameworkCore;
using WebApiPizzeria.Models;
using WebApiPizzeria.Repositories.Implementations;
using WebApiPizzeria.Repositories.Interfaces;
using WebApiPizzeria.Services.Implementations;
using WebApiPizzeria.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configurar DbContext
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar Repositorios
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

// Registrar Servicios
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.GetConnectionString("DefaultConnection");

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
