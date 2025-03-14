namespace Api.Services.CustomExceptions;

public class AlreadyFollowsUserException : Exception
{
    public AlreadyFollowsUserException()
    {
    }

    public AlreadyFollowsUserException(string message) : base(message)
    {
    }

    public AlreadyFollowsUserException(string message, Exception innerException) : base(message, innerException)
    {
    }
}