using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;


    public ApplicationDbContextInitializer(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
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
        if (!_dbContext.Users.Any())
        {
            var userAdmin = new User
            {
                Email = "adminTest@zpi.email",
                Username = "adminTest",
                FirstName = "adminFirstName",
                LastName = "adminLastName",
                DateOfBirth = new DateTime(2001, 10, 10),
                Gender = Gender.Male,
                Role = Role.Administrator,
                VerifiedAt = _dateTimeProvider.UtcNow
            };
            var userAdminPasswordHash = _passwordHasher.HashPassword(userAdmin, "AdminPassword1");
            userAdmin.PasswordHash = userAdminPasswordHash;
            
            await _dbContext.Users.AddAsync(userAdmin);
            
            var normalUser = new User
            {
                Email = "test@zpi.email",
                Username = "userTest",
                FirstName = "userFirstName",
                LastName = "userLastName",
                DateOfBirth = new DateTime(2010, 10, 10),
                Gender = Gender.Female,
                Role = Role.User,
                VerifiedAt = _dateTimeProvider.UtcNow
            };
            var normalUserPasswordHash = _passwordHasher.HashPassword(normalUser, "userPassword1");
            normalUser.PasswordHash = normalUserPasswordHash;
            
            await _dbContext.Users.AddAsync(normalUser);
        }
        
        await _dbContext.SaveChangesAsync();
    }
    
}