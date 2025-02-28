namespace Api.CustomExceptions;

/// <summary>
/// Exception to be thrown if some User already exist in the database
/// </summary>
public class UserAlreadyExists() : Exception("User already exists")
{
}