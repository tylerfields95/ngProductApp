using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;

// Load .env file from database folder
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "database", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure DbContext with PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString))
{
    // Replace environment variable placeholders
    connectionString = connectionString
        .Replace("${POSTGRES_DB}", Environment.GetEnvironmentVariable("POSTGRES_DB"))
        .Replace("${POSTGRES_USER}", Environment.GetEnvironmentVariable("POSTGRES_USER"))
        .Replace("${POSTGRES_PASSWORD}", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"));
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

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

// Global exception handler - returns consistent ProblemDetails format for all errors
app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext context) =>
{
    return Results.Problem();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
