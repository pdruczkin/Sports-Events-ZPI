using Application.Common.Interfaces;
using Application.Common.Models;
using Hangfire;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var emailSenderSettings = new EmailSenderSettings();
        configuration.GetSection("EmailSender").Bind(emailSenderSettings);
        services.AddSingleton(emailSenderSettings);

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();

        services.AddDbContext<ApplicationDbContext>(options => {
            options.UseSqlServer(configuration.GetConnectionString("DbConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            options.UseTriggers(triggerOptions => {
                triggerOptions.AddTrigger<SendEmailTestTrigger>();
                triggerOptions.AddTrigger<OrgnAchievementTrigger>();
                triggerOptions.AddTrigger<FrieAchievementTrigger>();
                triggerOptions.AddTrigger<TimePartAchievementTrigger>();
            });
        });

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<ApplicationDbContextInitializer>();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContextService, UserContextService>();

        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        
        
        
        services.AddScoped<IEmailSender, EmailSender>();
        
        return services;
    }
}