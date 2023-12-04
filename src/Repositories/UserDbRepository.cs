using AlphabetUpdateServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Repositories;

public class UserDbRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserDbRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<UserEntity?> FindUserById(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async ValueTask<IEnumerable<UserEntity>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }
}