using Asadotela.Api.Configurations;
using Asadotela.Api.Data;
using Asadotela.Api.IRepository;
using Asadotela.Api.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;


Log.Information("Application Is Starting");
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddDbContext<DataBaseContext>(o =>
    o.UseSqlServer("Server=.;Database=Asadotela.db;Trusted_Connection=True;TrustServerCertificate=True")
);
builder.Services.AddCors(o => {
    o.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddAutoMapper(typeof(MapperInitializer));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1" , new OpenApiInfo{Title = "Asadotela", Version = "v1"});
});
builder.Services.AddControllers().AddNewtonsoftJson(builder => builder.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

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