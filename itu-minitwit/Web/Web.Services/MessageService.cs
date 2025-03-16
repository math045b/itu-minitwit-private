using Web.Services.DTO_s;

namespace Web.Services;

public interface IMessageService
{
    public Task<IEnumerable<DisplayMessageDto>> GetMessages();
    public Task<DisplayMessageDto> CreateMessage(CreateMessageDto message, string Username);
}

public class MessageService(IMessageRepository messageRepository) : IMessageService
{
    public Task<IEnumerable<DisplayMessageDto>> GetMessages()
    {
        return messageRepository.GetMessages();
    }
    public Task<DisplayMessageDto> CreateMessage(CreateMessageDto message, string Username)
    {
        return messageRepository.CreateMessage(message, Username);
    }
}