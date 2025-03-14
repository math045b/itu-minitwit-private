using Api.Services.Dto_s;
using Api.Services.RepositoryInterfaces;

namespace Api.Services.Services;

public interface IUserService
{
    public Task<ReadUserDTO> Register(CreateUserDTO createUserDto);
}

public class UserService(IUserRepository userRepository) : IUserService
{
    [LogMethodParameters]
    [LogReturnValue]
    public Task<ReadUserDTO> Register(CreateUserDTO createUserDto)
    {
        return userRepository.Register(createUserDto);
    }
}