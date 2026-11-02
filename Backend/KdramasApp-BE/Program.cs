using KdramasApp_BE.Data;
using KdramasApp_BE.Data.Seeding;
using KdramasApp_BE.Repositories.Classes;
using KdramasApp_BE.Repositories.Interfaces;
using KdramasApp_BE.Services.Classes;
using KdramasApp_BE.Services.Interfaces;
//JWT authentication services
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

//retrieve the signing key, issuer and audience from configuration
string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT signing key is missing.");

string jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is missing.");

string jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience is missing.");

builder.Services
    //tells ASP.NET Core to use JWT bearer authentication as its
    //default authentication scheme
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

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
builder.Services.AddScoped<IActorService, ActorService>();

//DI for repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IActorRepository, ActorRepository>();

//DI for seeders
builder.Services.AddScoped<AdminSeeder>();

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
    //seed the data
    using var scope = app.Services.CreateScope();

    var seeder = scope.ServiceProvider
        .GetRequiredService<AdminSeeder>();

    await seeder.SeedAsync();
    //end seed

    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

//add cors policy for FE-BE communication
app.UseCors("AllowAngularDev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
