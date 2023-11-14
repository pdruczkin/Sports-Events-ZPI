namespace Domain.Entities;

public class Image
{
    public string PublicId { get; set; } = "";
    public string Url { get; set; } = "";
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}