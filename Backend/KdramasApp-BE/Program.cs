using KdramasApp_BE.Data;
using KdramasApp_BE.Services.Classes;
using KdramasApp_BE.Services.Interfaces;
using KdramasApp_BE.Repositories.Classes;
using KdramasApp_BE.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//get the connection info from KdramasDatabase conn string
//from KdramasDbContext db context in appsettings.json
//the context uses SQL Server
builder.Services.AddDbContext<KdramasDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("KdramasDatabase")));

builder.Services.AddOpenApi();

//DI for services
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

//DI for repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Get the allowed frontend origins from configuration.
string[] allowedOrigins =
    builder.Configuration
        .GetSection("AllowedOrigins")
        .Get<string[]>() ?? Array.Empty<string>();

// Add the CORS policy for FE-BE communication.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

//add cors policy for FE-BE communication
app.UseCors("AllowAngularDev");

app.UseAuthorization();

app.MapControllers();

app.Run();
