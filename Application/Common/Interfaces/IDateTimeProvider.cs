namespace Application.Common.Interfaces;

public interface IDateTimeProvider
{
    public DateTimeOffset Now { get; }
}