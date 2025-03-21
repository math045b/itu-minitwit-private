using Api.Services.Dto_s.MessageDTO_s;

namespace Api.Services.RepositoryInterfaces;

public interface IMessageRepository
{
    public Task<List<DisplayMessageDTO>> ReadMessages(int pagesize);
    public Task<List<DisplayMessageDTO>> ReadFilteredMessages(string username, int pagesize);
    public Task<List<DisplayMessageDTO>> ReadFilteredMessagesFromUserAndFollows(string username, int pagesize);

    public Task<bool> PostMessage(string username, string content);
}