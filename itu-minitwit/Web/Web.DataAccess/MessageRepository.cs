using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Web.DataAccess.Abstract;
using Web.Services; 
using Web.Services.DTO_s;

namespace Web.DataAccess;

public class MessageRepository(HttpClient httpClient, IConfiguration configuration) : 
    BaseAPIRepository(httpClient, configuration), IMessageRepository
{
    private string Endpoint { get; } = "/msgs";

    public Task<IEnumerable<DisplayMessageDto>> GetMessages()
    {
        return GetAllAsync<DisplayMessageDto>(Endpoint);
    }
    
    // public Task<bool> PostMessageAsync(DisplayMessageDto message)
    // {
    //     return CreateAsync<DisplayMessageDto,>()
    // }
}