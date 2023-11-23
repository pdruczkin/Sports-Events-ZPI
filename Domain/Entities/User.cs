using Domain.Enums;

namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    
    public Gender Gender { get; set; }
    public string PasswordHash { get; set; }
    public Role Role { get; set; } = Role.User;
    
    //Account
    public string? VerificationToken { get; set; }
    public DateTime? VerifiedAt { get; set; }
    
    public string? PasswordResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }

    public ICollection<Meeting> OrganizedEvents { get; set; } = new List<Meeting>();
    
    public ICollection<MeetingParticipant> MeetingParticipants { get; } = new List<MeetingParticipant>();
    
    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

    public List<Friendship> AsInviter { get; set; }
    public List<Friendship> AsInvitee { get; set; }
    
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    
    public Image? Image { get; set; }

}