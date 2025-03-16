using Web.Services.DTO_s;

namespace Web.Services.Repositories;

public interface IUserRepository
{
    public Task<bool> Login(LoginUserDTO dto);
}