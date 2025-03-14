namespace Api.Services.Dto_s.MessageDTO_s;

public class DisplayMessageDTO
{
    public string Username { get; set; }
    public string? Text { get; set; }
    public int? PubDate { get; set; }
    
    public override string ToString()
    {
        return "DisplayMessageDTO{" +
               $"Username='{Username}'" +
               $", Text='{Text}'" +
               $", PubDate={PubDate}" +
               '}';
    }
}