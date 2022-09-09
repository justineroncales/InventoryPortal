using AssetsManagement.BL;
using AssetsManagement.Data;
using InvoiceManagement.Data;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

var server = configuration["DB_HOST"] ?? "localhost";
var port =  "1433"; // Default SQL Server port
var user = "SA"; // Warning do not use the SA account
var password = configuration["DB_PASSWORD"] ?? "password@12345#";
var database = configuration["DB_NAME"] ?? "Asset";

// Add services to the container.
//container config
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer($"Server={server}, {port};Initial Catalog={database};User ID={user};Password={password}");
});
//localhost
//builder.Services.AddDbContext<DataContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetSqlConnection"));
//});
builder.Services.AddScoped<IBusnessLayer, BusnessLayer>();
builder.Services.AddScoped<IRabitMQProducer, RabitMQProducer>();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:8005/", "http://localhost:8005/#/").AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true) // allow any origin
       .AllowCredentials().Build();
    });
});