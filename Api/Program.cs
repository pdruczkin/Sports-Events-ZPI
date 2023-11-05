using Api;
using Api.Middleware;
using Application;
using Infrastructure;
using Infrastructure.Hubs;
using Infrastructure.Persistence;
using ConfigureServices = Api.ConfigureServices;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddScoped<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.UseCors("FrontEndClient");

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitializeAsync();
        await initializer.SeedAsync();
    }
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.MapHub<ChatHub>("chat-hub");

app.UseAuthorization();

app.MapControllers();

app.Run();