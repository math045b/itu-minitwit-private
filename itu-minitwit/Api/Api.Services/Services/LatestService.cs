using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface ILatestService
{
    public Task<int> GetLatest();
    public Task UpdateLatest(int? latest);
}
