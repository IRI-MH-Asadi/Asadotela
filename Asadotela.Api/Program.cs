using Microsoft.AspNetCore.Components.Web;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;


Log.Information("Application Is Starting");
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll",b => 
            b.AllowAnyOrigin()
                .AllowCredentials()
                .AllowAnyHeader()
    );
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1" , new OpenApiInfo{Title = "Asadotela", Version = "v1"});
});

builder.Host.UseSerilog((ctx,lc) =>lc
    .WriteTo.Console()
    .WriteTo.File(
        path: "f:\\Logs\\Asadotela\\logs\\log-.txt",
        outputTemplate: "{Timestamp: yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

// Log.Fatal(e, "Application Failed to start");
// Log.CloseAndFlush();