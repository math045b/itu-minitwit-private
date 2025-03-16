using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Web.DataAccess.Abstract;
using Web.Services; 
using Web.Services.DTO_s;

namespace Web.DataAccess;

public class MessageRepository(HttpClient httpClient, IConfiguration configuration) : 
    BaseAPIRepository(httpClient, configuration), IMessageRepository
{
    private string Endpoint { get; } = "msgs";

    public Task<IEnumerable<DisplayMessageDto>> GetMessages()
    {
        return GetAllAsync<DisplayMessageDto>(Endpoint);
    }
    
    public Task<DisplayMessageDto> CreateMessage(CreateMessageDto message, string Username)
    {
        return CreateAsync<DisplayMessageDto, CreateMessageDto>(Endpoint, Username, message);
    }
}