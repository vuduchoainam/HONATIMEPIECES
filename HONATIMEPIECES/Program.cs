using HONATIMEPIECES.Data;
using HONATIMEPIECES.Interfaces;
using HONATIMEPIECES.Models;
using HONATIMEPIECES.Repository;
using HONATIMEPIECES.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registed IWebHostEnvironment


//repositories
builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
builder.Services.AddScoped<IRepository<Brand>, Repository<Brand>>();
builder.Services.AddScoped<IRepository<Product>, Repository<Product>>();
builder.Services.AddScoped<IRepository<ProductImage>, Repository<ProductImage>>();


//services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

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
