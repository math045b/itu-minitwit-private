using Api.DataAccess.Models;
using Api.Services.CustomExceptions;
using Api.Services.Dto_s;
using Api.Services.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;

namespace Api.DataAccess.Repositories;

public class UserRepository(MinitwitDbContext db, IPasswordHasher<User> passwordHasher) : IUserRepository
{
    public async Task<ReadUserDTO> Register(CreateUserDTO createUserDto)
    {
        User user = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
        };
        user.PwHash = passwordHasher.HashPassword(user, createUserDto.Pwd);

        try
        {
            var createdUser = db.Users.Add(user);
            await db.SaveChangesAsync();
            return new ReadUserDTO()
                { UserId = createdUser.Entity.UserId, Username = user.Username, Email = user.Email };
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            throw new UserAlreadyExists();
        }
    }
}