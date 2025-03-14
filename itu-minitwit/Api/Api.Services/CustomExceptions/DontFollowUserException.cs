namespace Api.Services.CustomExceptions;

public class DontFollowUserException: Exception
{
    public DontFollowUserException()
    {
    }

    public DontFollowUserException(string message) : base(message)
    {
    }

    public DontFollowUserException(string message, Exception innerException) : base(message, innerException)
    {
    }
}