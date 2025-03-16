namespace Web.Services.DTO_s;

public class GetUsersMessageDTO(string username, int numberOfMessages)
{
    public string Username { get; } = username;
    public int NumberOfMessages { get; } = numberOfMessages;
}