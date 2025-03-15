namespace Api.Services.Dto_s;

public class DeleteUserDTO
{
    public required int UserId { get; set; }

    public override string ToString()
    {
        return "DeleteUserDTO: " +
               $"UserId: {UserId}";
    }
}