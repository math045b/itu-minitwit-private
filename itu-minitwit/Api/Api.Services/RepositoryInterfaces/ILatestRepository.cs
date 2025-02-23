namespace Api.Services.RepositoryInterfaces;

public interface ILatestRepository
{
    public Task<int> GetLatest();
    public Task UpdateLatest(int latest);
}