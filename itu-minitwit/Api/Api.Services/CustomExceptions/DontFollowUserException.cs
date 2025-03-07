namespace Api.Services.CustomExceptions;

public class DotFollowUserException: Exception
{
    public DotFollowUserException()
    {
    }

    public DotFollowUserException(string message) : base(message)
    {
    }

    public DotFollowUserException(string message, Exception innerException) : base(message, innerException)
    {
    }
}