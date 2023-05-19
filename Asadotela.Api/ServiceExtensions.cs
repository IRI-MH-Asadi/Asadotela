using Asadotela.Api.Data;
using Asadotela.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using AspNetCoreRateLimit;

namespace Asadotela.Api;

public static class ServiceExtensions
{
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);
        builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
        builder.AddEntityFrameworkStores<DataBaseContext>().AddDefaultTokenProviders();
    }


    //Configure JWT
    public static void ConfigureJWT(this IServiceCollection services, IConfiguration Configuration)
    {
        var jwtSettings = Configuration.GetSection("Jwt");
        var key = jwtSettings.GetSection("KeyAsadotela").Value;

        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                };
            });
    }

    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(error =>
        {
            error.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "aplication/json";
                var contextFeater = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeater != null)
                {
                    Log.Error($"Something went wrong in the {contextFeater.Error}");

                    await context.Response.WriteAsync(new Error
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Internal Server Error. Please Try Again later."
                    }.ToString());
                }
            });
        });
    }

    public static void ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
        });
    }

    public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
    {
        services.AddResponseCaching();
        services.AddHttpCacheHeaders((expirationOpt) =>
            {
                expirationOpt.MaxAge = 120;
                expirationOpt.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            },
            (validationOpt) => { validationOpt.MustRevalidate = true; });
    }

    public static void ConfigureRateLimiting(this IServiceCollection service)
    {
        var rateLimitRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Limit = 1,
                Period = "5s"
            }
        };
        

        service.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; });
        service.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        service.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        service.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        service.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }
}