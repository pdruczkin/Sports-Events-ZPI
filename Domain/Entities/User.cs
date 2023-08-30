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
    public bool IsAccountActive { get; set; } = true; //To be change with email verification


    public ICollection<Meeting> OrganizedEvents { get; set; }
  //  public ICollection<Event> ParticipatedEvents { get; set; }
}