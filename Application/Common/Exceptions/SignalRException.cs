namespace Application.Common.Exceptions;

public class SignalRException : Exception
{
    public SignalRException(string message) : base(message)
    {
    }
}