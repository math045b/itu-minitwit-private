using Web.Services.DTO_s;

namespace Web.Services;

public interface IMessageService
{
    public Task<IEnumerable<DisplayMessageDto>> GetMessages();
    public Task<DisplayMessageDto> CreateMessage(CreateMessageDto message, string username);
    public Task<IEnumerable<DisplayMessageDto>> GetUsersMessages(GetUsersMessageDTO dto);
    public Task<IEnumerable<DisplayMessageDto>> GetUserAndFollowsMessages(GetUsersMessageDTO dto);
}

public class MessageService(IMessageRepository messageRepository) : IMessageService
{
    public Task<IEnumerable<DisplayMessageDto>> GetMessages()
    {
        return messageRepository.GetMessages();
    }
    
    public Task<DisplayMessageDto> CreateMessage(CreateMessageDto message, string username)
    {
        return messageRepository.CreateMessage(message, username);
    }

    public Task<IEnumerable<DisplayMessageDto>> GetUsersMessages(GetUsersMessageDTO dto)
    {
        return messageRepository.GetUsersMessages(dto);
    }

    public Task<IEnumerable<DisplayMessageDto>> GetUserAndFollowsMessages(GetUsersMessageDTO dto)
    {
        return messageRepository.GetUserAndFollowsMessages(dto);
    }
}