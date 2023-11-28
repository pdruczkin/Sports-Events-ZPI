namespace Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Text { get; set; }
    
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; }
}