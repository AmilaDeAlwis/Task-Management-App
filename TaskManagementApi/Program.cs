// Program.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TaskManagementApi.Data;
using TaskManagementApi.Models;
using TaskManagementApi.Middleware;
using TaskManagementApi.Filter;
using TaskManagementApi.Interfaces;
using TaskManagementApi.Services;
using ModelTask = TaskManagementApi.Models.Task;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Add DbContext with SQL Server configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Register IPasswordHasher for user password handling
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// 3. Register your Services and their Interfaces for Dependency Injection
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();

// 4. Add CORS policies (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

// 1. Register Custom Exception Handling Middleware FIRST
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. Developer Exception Page
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapControllers();

// Seed data (run once on application startup if no users exist)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Ensures latest migrations are applied

        var userService = services.GetRequiredService<IUserService>(); // Get the UserService

        // Seed a default user if none exist
        if (await context.Users.AnyAsync() == false)
        {
            var seedUsername = builder.Configuration["SeedUser:Username"];
            var seedPassword = builder.Configuration["SeedUser:Password"];

            if (!string.IsNullOrEmpty(seedUsername) && !string.IsNullOrEmpty(seedPassword))
            {
                var defaultUser = new User { Username = seedUsername };
                await userService.AddUserAsync(defaultUser, seedPassword);
                Console.WriteLine($"Default user '{seedUsername}' seeded successfully.");
            }
            else
            {
                Console.WriteLine("Warning: SeedUser credentials not found in configuration. Default user will not be created. Ensure 'dotnet user-secrets set' is used for development or environment variables for production.");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the database.");
    }
}

app.Run();