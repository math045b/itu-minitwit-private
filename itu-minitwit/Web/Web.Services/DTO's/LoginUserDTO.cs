namespace Web.Services.DTO_s;

public class LoginUserDTO(string username, string password)
{
    public string Username { get; } = username;
    public string Password { get; } = password;
}