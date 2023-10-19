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


    public ApplicationDbContextInitializer(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher,
        IDateTimeProvider dateTimeProvider)
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
        if (!_dbContext.Users.Any() && !_dbContext.Meetings.Any())
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

            var testUser = new User
            {
                Email = "testuser@zpi.email",
                Username = "userTest",
                FirstName = "userFirstName",
                LastName = "userLastName",
                DateOfBirth = new DateTime(2010, 10, 10),
                Gender = Gender.Male,
                Role = Role.User,
                VerifiedAt = _dateTimeProvider.UtcNow
            };
            var testUserPasswordHash = _passwordHasher.HashPassword(testUser, "testPassword1");
            testUser.PasswordHash = testUserPasswordHash;

            await _dbContext.Users.AddAsync(testUser);


            var utcDate = TrimSeconds(_dateTimeProvider.UtcNow);
            
            var testMeeting = new Meeting
            {
                Title = "TestMeeting",
                Description = "Description",
                StartDateTimeUtc = utcDate.AddDays(14),
                EndDateTimeUtc = utcDate.AddDays(14).AddHours(2),
                Difficulty = Difficulty.Amateur,
                SportsDiscipline = SportsDiscipline.Basketball,
                Visibility = MeetingVisibility.Public
            };

            await _dbContext.Meetings.AddAsync(testMeeting);

            var testMeetingEmpty = new Meeting
            {
                Title = "Empty meeting",
                Description = "Empty meeting description",
                StartDateTimeUtc = utcDate.AddDays(14).AddHours(1),
                EndDateTimeUtc = utcDate.AddDays(14).AddHours(2).AddMinutes(15),
                Difficulty = Difficulty.Professional,
                SportsDiscipline = SportsDiscipline.Football,
                Visibility = MeetingVisibility.Public
            };
            
            await _dbContext.Meetings.AddAsync(testMeetingEmpty);
            
            var testMeetingPrivate = new Meeting
            {
                Title = "Private meeting 2",
                Description = "private meeting description",
                StartDateTimeUtc = utcDate.AddDays(14).AddHours(3),
                EndDateTimeUtc = utcDate.AddDays(14).AddHours(7),
                Difficulty = Difficulty.Intermediate,
                SportsDiscipline = SportsDiscipline.Other,
                Visibility = MeetingVisibility.Private
            };

            await _dbContext.Meetings.AddAsync(testMeetingPrivate);
        }

        await _dbContext.SaveChangesAsync();
    }

    public static DateTime TrimSeconds(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);
    }
}