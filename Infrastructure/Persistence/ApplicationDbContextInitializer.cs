using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _dbContext;


    public ApplicationDbContextInitializer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task InitializeAsync()
    {

        if (_dbContext.Database.IsSqlServer()) 
        { 
            await _dbContext.Database.MigrateAsync();
        }
    }
    
    public async Task SeedAsync()
    {
        if (!_dbContext.Cars.Any())
        {
            _dbContext.Add(new Car
            {
                MaxSpeed = 100,
                Name = "Audi",
                Color = Color.Black
            });
        }
        
        await _dbContext.SaveChangesAsync();
    }
    
}