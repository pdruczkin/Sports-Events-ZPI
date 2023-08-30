using System.Text;
using Application;
using Application.Common.Models;
using Microsoft.IdentityModel.Tokens;

namespace Api;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = new AuthenticationSettings();

        configuration.GetSection("Authentication").Bind(authSettings);

        services.AddSingleton(authSettings);
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = "Bearer";
            opt.DefaultScheme = "Bearer";
            opt.DefaultChallengeScheme = "Bearer";
        }).AddJwtBearer(config =>
        {
            config.RequireHttpsMetadata = false;
            config.SaveToken = true;
            config.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = authSettings.JwtIssuer,
                ValidIssuer = authSettings.JwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey))
            };
        });
        
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHttpContextAccessor();
        services.AddCors(options =>
        {
            options.AddPolicy("FrontEndClient", opt =>
            {
                opt.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(configuration["AllowedOrigins"]);
            });
        });
        
        
        

        return services;
    }
}