using Api.Services.Dto_s;

namespace Api.Services.RepositoryInterfaces;

public interface IUserRepository
{
    public Task<ReadUserDTO> Register(CreateUserDTO createUserDto);
    public Task<bool> Login(LoginUserDTO dto);
}