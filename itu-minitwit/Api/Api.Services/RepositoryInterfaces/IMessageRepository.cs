using Api.Services.Dto_s.MessageDTO_s;

namespace Api.Services.RepositoryInterfaces;

public interface IMessageRepository
{
    public Task<List<ReadMessageDTO>> ReadMessages();
    public Task<List<ReadMessageDTO>> ReadFilteredMessages(string username);

    public Task<ReadMessageDTO> PostMessage(string username, string content);
}