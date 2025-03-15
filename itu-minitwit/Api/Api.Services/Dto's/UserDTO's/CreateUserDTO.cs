namespace Api.Services.Dto_s;

public class CreateUserDTO
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Pwd { get; set; } = null!;

    public override string ToString()
    {
        return "CreateUserDTO{" +
               $"Username='{Username}', " +
               $"Email='{Email}', " +
               $"Pwd='{Pwd}'" +
               '}';
    }
}