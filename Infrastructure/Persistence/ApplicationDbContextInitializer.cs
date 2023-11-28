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
                Email = "admin@zpi.email",
                Username = "admin",
                FirstName = "adminFirstName",
                LastName = "adminLastName",
                DateOfBirth = new DateTime(2001, 10, 10),
                Gender = Gender.Male,
                Role = Role.Administrator,
                VerifiedAt = _dateTimeProvider.UtcNow,
            };
            var userAdminPasswordHash = _passwordHasher.HashPassword(userAdmin, "Password1");
            userAdmin.PasswordHash = userAdminPasswordHash;

            await _dbContext.Users.AddAsync(userAdmin);

            var normalUser = new User
            {
                Email = "normal@zpi.email",
                Username = "normal",
                FirstName = "normalFirstName",
                LastName = "normalLastName",
                DateOfBirth = new DateTime(2010, 10, 10),
                Gender = Gender.Female,
                Role = Role.User,
                VerifiedAt = _dateTimeProvider.UtcNow
            };
            var normalUserPasswordHash = _passwordHasher.HashPassword(normalUser, "Password1");
            normalUser.PasswordHash = normalUserPasswordHash;

            await _dbContext.Users.AddAsync(normalUser);

            var testUser = new User
            {
                Email = "test@zpi.email",
                Username = "test",
                FirstName = "testFirstName",
                LastName = "testLastName",
                DateOfBirth = new DateTime(2010, 10, 10),
                Gender = Gender.Male,
                Role = Role.User,
                VerifiedAt = _dateTimeProvider.UtcNow
            };
            var testUserPasswordHash = _passwordHasher.HashPassword(testUser, "Password1");
            testUser.PasswordHash = testUserPasswordHash;

            await _dbContext.Users.AddAsync(testUser);


            var friendshipAccepted = new Friendship()
            {
                Inviter = userAdmin,
                Invitee = testUser,
                FriendshipStatus = FriendshipStatus.Accepted
            };
            
            await _dbContext.Friendships.AddAsync(friendshipAccepted);
            
            var friendshipPending = new Friendship()
            {
                Inviter = testUser,
                Invitee = normalUser,
                FriendshipStatus = FriendshipStatus.Invited
            };
            await _dbContext.Friendships.AddAsync(friendshipPending);
            
            
            var utcDate = TrimSeconds(_dateTimeProvider.UtcNow);
            
            var testMeeting = new Meeting
            {
                Title = "TestMeeting",
                Description = "Description",
                Latitude = 51.1,
                Longitude = 17,
                StartDateTimeUtc = utcDate.AddDays(14),
                EndDateTimeUtc = utcDate.AddDays(14).AddHours(2),
                Difficulty = Difficulty.Amateur,
                SportsDiscipline = SportsDiscipline.Basketball,
                Visibility = MeetingVisibility.Public,
                MaxParticipantsQuantity = 10,
                MinParticipantsAge = 10,
                Organizer = userAdmin,
                MeetingParticipants = new List<MeetingParticipant>
                {
                    new()
                    {
                        Participant = testUser,
                        InvitationStatus = InvitationStatus.Pending
                    },
                    new()
                    {
                        Participant = normalUser,
                        InvitationStatus = InvitationStatus.Accepted
                    }
                },
                ChatMessages = new List<ChatMessage>
                {
                    new()
                    {
                        User = normalUser,
                        SentAtUtc = _dateTimeProvider.UtcNow.AddMinutes(-2),
                        Value = "Pierwsza wiadomość"
                    },
                    new()
                    {
                        User = testUser,
                        SentAtUtc = _dateTimeProvider.UtcNow.AddMinutes(-1),
                        Value = "Pierwsza odpowiedź"
                    },
                    new()
                    {
                        User = normalUser,
                        SentAtUtc = _dateTimeProvider.UtcNow,
                        Value = "Druga wiadomość"
                    }
                }
            };

            await _dbContext.Meetings.AddAsync(testMeeting);

            var testMeetingEmpty = new Meeting
            {
                Title = "Empty meeting",
                Description = "Empty meeting description",
                Latitude = 51.2,
                Longitude = 17.1,
                StartDateTimeUtc = utcDate.AddDays(14).AddHours(1),
                EndDateTimeUtc = utcDate.AddDays(14).AddHours(2).AddMinutes(15),
                Difficulty = Difficulty.Professional,
                SportsDiscipline = SportsDiscipline.Football,
                Visibility = MeetingVisibility.Public,
                Organizer = testUser,
                MinParticipantsAge = 3,
                MaxParticipantsQuantity = 3
            };
            
            await _dbContext.Meetings.AddAsync(testMeetingEmpty);
            
            var testMeetingPrivate = new Meeting
            {
                Title = "Private meeting 2",
                Description = "private meeting description",
                Latitude = 51.2,
                Longitude = 17.2,
                StartDateTimeUtc = utcDate.AddDays(14).AddHours(3),
                EndDateTimeUtc = utcDate.AddDays(14).AddHours(7),
                Difficulty = Difficulty.Intermediate,
                SportsDiscipline = SportsDiscipline.Other,
                Visibility = MeetingVisibility.Private,
                Organizer = testUser,
                MaxParticipantsQuantity = 30,
                MinParticipantsAge = 30,
                MeetingParticipants = new List<MeetingParticipant>
                {
                    new()
                    {
                        Participant = userAdmin,
                        InvitationStatus = InvitationStatus.Pending
                    },
                    new()
                    {
                        Participant = normalUser,
                        InvitationStatus = InvitationStatus.Pending
                    }
                }
            };

            await _dbContext.Meetings.AddAsync(testMeetingPrivate);
            
            
            if (!_dbContext.Posts.Any())
            {
                var post1 = new Post()
                {
                    Title = "Wstawanie po bułki",
                    Description = "Trzeba szybciej wstać",
                    Text =
                        "Jak się chce zdążyć do piłki to trzeba się szybciej ruszyć, dokładnie tak samo jak się chce zjeść ciepłe bułki rano, trzeba szybciej wstać",
                    User = testUser
                };
                
                await _dbContext.Posts.AddAsync(post1);

                var post2 = new Post()
                {
                    Title = "Dlaczego backend jest ważniejszy",
                    Text = "No to akurat chyba oczywiste",
                    User = normalUser
                };
                
                await _dbContext.Posts.AddAsync(post2);
                
                var post3 = new Post()
                {
                    Title = "Ważny post",
                    Description = "Baaardzo ważny post",
                    Text = "Żartowałem",
                    User = normalUser
                };
                
                await _dbContext.Posts.AddAsync(post3);
            }
        }
        
        if (!_dbContext.Achievements.Any())
        {
            var achievement1 = new Achievement()
            {
                Id = "ORGN01",
                Description = "Liczba założonych spotkań: 1",
                Category = AchievementCategory.Meetings
            };
            await _dbContext.Achievements.AddAsync(achievement1);
            var achievement2 = new Achievement()
            {
                Id = "ORGN05",
                Description = "Liczba założonych spotkań: 5",
                Category = AchievementCategory.Meetings
            };
            await _dbContext.Achievements.AddAsync(achievement2);
            var achievement3 = new Achievement()
            {
                Id = "ORGN10",
                Description = "Liczba założonych spotkań: 10",
                Category = AchievementCategory.Meetings
            };
            await _dbContext.Achievements.AddAsync(achievement3);
            var achievement4 = new Achievement()
            {
                Id = "ORGN50",
                Description = "Liczba założonych spotkań: 50",
                Category = AchievementCategory.Meetings
            };
            await _dbContext.Achievements.AddAsync(achievement4);
            var achievement5 = new Achievement()
            {
                Id = "PART01",
                Description = "Liczba spotkań jako uczestnik: 1",
                Category = AchievementCategory.Meetings
            };
            await _dbContext.Achievements.AddAsync(achievement5);
            var achievement6 = new Achievement()
            {
                Id = "PART05",
                Description = "Liczba spotkań jako uczestnik: 5",
                Category = AchievementCategory.Meetings
            };
            await _dbContext.Achievements.AddAsync(achievement6);
            var achievement7 = new Achievement()
            {
                Id = "PART10",
                Description = "Liczba spotkań jako uczestnik: 10",
                Category = AchievementCategory.Meetings
            };
            await _dbContext.Achievements.AddAsync(achievement7);
            var achievement8 = new Achievement()
            {
                Id = "PART50",
                Description = "Liczba spotkań jako uczestnik: 50",
                Category = AchievementCategory.Meetings
            };
            await _dbContext.Achievements.AddAsync(achievement8);
            var achievement9 = new Achievement()
            {
                Id = "CHAT",
                Description = "Pierwsza wiadomość na czacie",
                Category = AchievementCategory.Meetings
            };
            await _dbContext.Achievements.AddAsync(achievement9);
            var achievement10 = new Achievement()
            {
                Id = "TIME01",
                Description = "Czas uczestnictwa w spotkaniach: 1h",
                Category = AchievementCategory.Time
            };
            await _dbContext.Achievements.AddAsync(achievement10);
            var achievement11 = new Achievement()
            {
                Id = "TIME10",
                Description = "Czas uczestnictwa w spotkaniach: 10h",
                Category = AchievementCategory.Time
            };
            await _dbContext.Achievements.AddAsync(achievement11);
            var achievement12 = new Achievement()
            {
                Id = "TIME24",
                Description = "Czas uczestnictwa w spotkaniach: 24h",
                Category = AchievementCategory.Time
            };
            await _dbContext.Achievements.AddAsync(achievement12);
            var achievement13 = new Achievement()
            {
                Id = "FRIE01",
                Description = "Liczba znajomych: 1",
                Category = AchievementCategory.Friends
            };
            await _dbContext.Achievements.AddAsync(achievement13);
            var achievement14 = new Achievement()
            {
                Id = "FRIE05",
                Description = "Liczba znajomych: 5",
                Category = AchievementCategory.Friends
            };
            await _dbContext.Achievements.AddAsync(achievement14);
            var achievement15 = new Achievement()
            {
                Id = "FRIE10",
                Description = "Liczba znajomych: 10",
                Category = AchievementCategory.Friends
            };
            await _dbContext.Achievements.AddAsync(achievement15);
            var achievement16 = new Achievement()
            {
                Id = "FRIE50",
                Description = "Liczba znajomych: 50",
                Category = AchievementCategory.Friends
            };
            await _dbContext.Achievements.AddAsync(achievement16);
            var achievement17 = new Achievement()
            {
                Id = "FRIE100",
                Description = "Liczba znajomych: 100",
                Category = AchievementCategory.Friends
            };
            await _dbContext.Achievements.AddAsync(achievement17);
            var achievement18 = new Achievement()
            {
                Id = "FRIE500",
                Description = "Liczba znajomych: 500",
                Category = AchievementCategory.Friends
            };
            await _dbContext.Achievements.AddAsync(achievement18);
        }
        await _dbContext.SaveChangesAsync();
    }

    private static DateTime TrimSeconds(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);
    }
}