using Api.Services.Dto_s.MessageDTO_s;
using Api.Services.RepositoryInterfaces;

namespace Api.DataAccess.Repositories;

public class MessageRepository : IMessageRepository
{
    public Task<List<ReadMessageDTO>> ReadMessages()
    {
        throw new NotImplementedException();
    }

    public Task<List<ReadMessageDTO>> ReadFilteredMessages(string username)
    {
        throw new NotImplementedException();
    }

    public Task<ReadMessageDTO> PostMessage(string username, string content)
    {
        throw new NotImplementedException();
    }
}