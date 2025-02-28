namespace Api.Services.Dto_s;

public class ReadUserDTO
{
    public required int UserId { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }
}