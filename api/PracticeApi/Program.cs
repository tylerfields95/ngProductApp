using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using PracticeApi.Data;

// Load .env file from database folder - use absolute path
var solutionRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".."));
var envPath = Path.Combine(solutionRoot, "database", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}
else
{
    Console.WriteLine($"Warning: .env file not found at {envPath}");
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

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
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

// Use CORS - must be early in pipeline
app.UseCors("AllowAngularApp");

// Global exception handler - returns consistent ProblemDetails format for all errors
app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext context) =>
{
    return Results.Problem();
});

// Disable HTTPS redirection in development to avoid CORS issues with Angular
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
