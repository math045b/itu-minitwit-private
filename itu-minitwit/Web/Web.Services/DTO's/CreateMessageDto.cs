namespace Web.Services.DTO_s;

public class CreateMessageDto
{
    public required string Username { get; set; }
    public required string Content { get; set; }
    
    public override string ToString()
    {
        return "CreateMessageDto{" +
               $"Content='{Content}'" +
               '}';
    }
}