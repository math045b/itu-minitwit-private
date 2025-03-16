using Web.Services.DTO_s;

namespace Web.Services;

public interface IMessageRepository
{
    public Task<IEnumerable<DisplayMessageDto>> GetMessages();
    
    public Task<DisplayMessageDto> CreateMessage(CreateMessageDto message, string Username);
}