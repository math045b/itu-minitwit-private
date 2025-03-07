namespace Api.CustomExceptions;

/// <summary>
/// Exception to be thrown if some User already exist in the database
/// </summary>
public class UserAlreadyExists : Exception
{
    public UserAlreadyExists()
    {
        
    }

    public UserAlreadyExists(string message)
    {
        
    }

    public UserAlreadyExists(string message, Exception inner)
    {
        
    }
}