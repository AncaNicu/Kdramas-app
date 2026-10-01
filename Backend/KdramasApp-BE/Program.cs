using KdramasApp_BE.Data;
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

app.UseAuthorization();

app.MapControllers();

app.Run();
