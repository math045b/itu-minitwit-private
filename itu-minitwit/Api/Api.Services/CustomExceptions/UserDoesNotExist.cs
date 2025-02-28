namespace Api.CustomExceptions;

/// <summary>
/// Exception to be thrown if some User does not exist in the database
/// </summary>
public class UserDoesNotExist() : Exception("User does not exist")
{
}