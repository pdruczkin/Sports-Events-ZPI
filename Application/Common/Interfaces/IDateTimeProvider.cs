namespace Application.Common.Interfaces;

public interface IDateTimeProvider
{
    public DateTime Now { get; }
    public DateTime UtcNow { get; }
    public int CalculateAge(DateTime dateOfBirth);
}