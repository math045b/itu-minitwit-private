using Api.Services.Dto_s;
using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface IUserService
{
    public ReadUserDTO Register(CreateUserDTO createUserDto);
}

public class UserService(IUserRepository userRepository) : IUserService
{
    public ReadUserDTO Register(CreateUserDTO createUserDto)
    {
        return userRepository.Register(createUserDto).Result;
    }
}