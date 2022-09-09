using InvoiceManagement.BL;
using InvoiceManagement.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
var random = new Random();

IConfiguration configuration = builder.Configuration;

var server = configuration["DB_HOST"] ?? "localhost";
var port = "1433"; // Default SQL Server port
var user = "SA"; // Warning do not use the SA account
var password = configuration["DB_PASSWORD"] ?? "password@12345#";
var database = configuration["DB_NAME"] ?? "Invoice";

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer($"Server={server}, {port};Initial Catalog={database};User ID={user};Password={password}");
});

//builder.Services.AddDbContext<DataContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("AssetSqlConnection"));
//});
builder.Services.AddScoped<IBusnessLayer, BusnessLayer>();
builder.Services.AddScoped<IRabitMQProducer, RabitMQProducer>();
builder.Services.AddHttpClient<AssetClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:8002/api/invoice");
})
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
        5,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                        + TimeSpan.FromSeconds(random.Next(0,1000)),
        onRetry: (outcome, timespan, retyrAttempt) =>
        {
            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<ILogger<AssetClient>>()?
                            .LogWarning("");
        }
     ))
    .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
        5,
        TimeSpan.FromSeconds(15),
        onBreak: (outcome, timespan) =>
        {
            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<ILogger<AssetClient>>()?
                            .LogWarning("");
        },
         onReset: () =>
         {
             var serviceProvider = services.BuildServiceProvider();
             serviceProvider.GetService<ILogger<AssetClient>>()?
                             .LogWarning("");
         }
     ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder =>
    {
        builder.WithOrigins().AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true) // allow any origin
       .AllowCredentials().Build();
    });
});

builder.Services.AddControllers();
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
