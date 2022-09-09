using AssetsManagement.BL;
using AssetsManagement.Data;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

var server = configuration["DB_HOST"] ?? "localhost";
var port = "1433"; // Default SQL Server port
var user = "SA"; // Warning do not use the SA account
var password = configuration["DB_PASSWORD"] ?? "password@12345#";
var database = configuration["DB_NAME"] ?? "Asset";

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer($"Server={server}, {port};Initial Catalog={database};User ID={user};Password={password}");
});

//builder.Services.AddDbContext<DataContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetSqlConnection"));
//});
builder.Services.AddScoped<IBusnessLayer, BusnessLayer>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:8005/", "http://localhost:8005/#/").AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true) // allow any origin
       .AllowCredentials().Build();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("MyPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
