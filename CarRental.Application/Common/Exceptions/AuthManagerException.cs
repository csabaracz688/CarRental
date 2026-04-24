namespace CarRental.Application.Common.Exceptions;

public sealed class AuthManagerException : Exception
{
    public AuthManagerException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
