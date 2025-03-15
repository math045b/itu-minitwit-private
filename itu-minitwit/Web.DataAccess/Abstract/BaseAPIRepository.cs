using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Web.DataAccess.Abstract;

public abstract class BaseAPIRepository(HttpClient httpClient,  IConfiguration configuration)
{
    protected HttpClient HttpClient { get; } = httpClient;
    protected string ApiBaseUrl { get; } = configuration["ApiSettings:BaseUrl"]
                                           ?? throw new ArgumentException("Cant find api url");


    public async Task<IEnumerable<T>> GetAllAsync<T>(string endpoint)
    {
        var response = await HttpClient.GetAsync($"{ApiBaseUrl}{endpoint}");
        var content = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<IEnumerable<T>>();
        return list ?? [];
    }

    public async Task<T> GetOneAsync<T, TId>(string endpoint, TId id)
    {
        var response = await HttpClient.GetAsync($"{ApiBaseUrl}/{endpoint}/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>()!;
    }
    
    public async Task<T> CreateAsync<T,TD>(string endpoint, TD data)
    {
        var response = await HttpClient.PostAsJsonAsync($"{ApiBaseUrl}/{endpoint}", data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task UpdateAsync<TId, TD>(string endpoint, TId id, TD data)
    {
        var response = await HttpClient.PutAsJsonAsync($"{ApiBaseUrl}/{endpoint}/{id}", data);
        response.EnsureSuccessStatusCode();
    }
}