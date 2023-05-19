using Asadotela.Api;
using Asadotela.Api.Configurations;
using Asadotela.Api.Data;
using Asadotela.Api.IRepository;
using Asadotela.Api.Repository;
using Asadotela.Api.Services;
using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

Log.Information("Application Is Starting");
var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("MyDatabase");
// Add Serilog Logger
builder.Host.UseSerilog();


        #region Services

// Add services to the container.
builder.Services.AddDbContext<DataBaseContext>(o =>
    o.UseSqlServer(connectionString)
);


builder.Services.AddMemoryCache();

builder.Services.ConfigureRateLimiting();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureHttpCacheHeaders();
//IdentityUser


//builder.Services.ConfigureHttpContextAccessor();
builder.Services.AddResponseCaching();
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);


builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

//builder.Services.ConfigureAutoMapper();


builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddAutoMapper(typeof(MapperInitializer));

builder.Services.AddControllers(config =>
{
    config.CacheProfiles.Add("120SecondsDuration", new Microsoft.AspNetCore.Mvc.CacheProfile
    {
        Duration = 120,
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Asadotela", Version = "v1" }));

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
            Enter 'Bearer' [space] and then your token in the text input below.
            Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "Oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Asadotela", Version = "v1" });
});

builder.Services.AddControllers().AddNewtonsoftJson(op =>
    op.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.ConfigureVersioning();

        #endregion


// Config Serilog Logger
builder.Host.UseSerilog((ctx, lc) => lc
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

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseResponseCaching();

app.UseHttpCacheHeaders();

app.UseIpRateLimiting();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Log.Fatal(e, "Application Failed to start");
// Log.CloseAndFlush();