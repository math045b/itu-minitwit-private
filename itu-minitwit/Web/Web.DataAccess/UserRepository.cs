using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Web.DataAccess.Abstract;
using Web.Services.DTO_s;
using Web.Services.Repositories;

namespace Web.DataAccess;

public class UserRepository (HttpClient httpClient, IConfiguration configuration) : 
    BaseAPIRepository(httpClient, configuration), IUserRepository
{
    private string Endpoint { get; } = "login";
    public async Task<bool> Login(LoginUserDTO dto)
    {
        var response = await HttpClient.PostAsJsonAsync($"{ApiBaseUrl}/{Endpoint}", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<bool>())!;
    }
}