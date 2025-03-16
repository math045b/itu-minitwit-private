using Api.DataAccess.Models;
using Api.Services.CustomExceptions;
using Api.Services.Dto_s;
using Api.Services.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        if (await db.Users.AnyAsync(u => u.Username == user.Username))
            throw new UserAlreadyExists($"User \"{user.Username}\" already exists");
        
        var createdUser = await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
        return new ReadUserDTO()
            { UserId = createdUser.Entity.UserId, Username = user.Username, Email = user.Email };
    }

    public async Task<bool> Login(LoginUserDTO dto)
    {
        if (!await db.Users.AnyAsync(u => u.Username == dto.Username)) return false;

        var user = (await db.Users.Where(u => u.Username == dto.Username).FirstOrDefaultAsync())!;
        var verifyHashedPassword
            = passwordHasher.VerifyHashedPassword(user, user.PwHash, dto.Password);

        return verifyHashedPassword != PasswordVerificationResult.Failed;
    }
}