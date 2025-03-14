namespace Api.Services.Dto_s.MessageDTO_s;

public class CreateMessageDTO
{
    public string Content { get; set; }

    public override string ToString()
    {
        return "CreateMessageDTO{" +
               $"Content='{Content}'" +
               '}';
    }
}