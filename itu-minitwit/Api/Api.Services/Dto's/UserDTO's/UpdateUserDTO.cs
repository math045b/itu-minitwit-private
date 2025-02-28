namespace Api.Services.Dto_s;

public class UpdateUserDTO
{
    public required int UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
}