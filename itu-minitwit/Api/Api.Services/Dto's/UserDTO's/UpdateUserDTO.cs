namespace Api.Services.Dto_s;

public class UpdateUserDTO
{
    public required int UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
    
    public override string ToString()
    {
        return "UpdateUserDTO: " +
               $"UserId: {UserId}, " +
               $"Username: {Username}, " +
               $"Email: {Email}, " +
               $"Password: {Password}";
    }
}