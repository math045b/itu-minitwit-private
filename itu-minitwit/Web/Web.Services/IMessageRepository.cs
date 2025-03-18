using Web.Services.DTO_s;

namespace Web.Services;

public interface IMessageRepository
{
    public Task<IEnumerable<DisplayMessageDto>> GetMessages();
    public Task<IEnumerable<DisplayMessageDto>> GetUsersMessages(GetUsersMessageDTO dto);
    public Task<IEnumerable<DisplayMessageDto>> GetUserAndFollowsMessages(GetUsersMessageDTO dto);
}