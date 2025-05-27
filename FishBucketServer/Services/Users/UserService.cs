using AlphabetUpdateServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Services.Users;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IAsyncEnumerable<UserEntity> GetAllUsers()
    {
        return _context.Users
            .AsNoTracking()
            .AsAsyncEnumerable();
    }
    
    public async Task<UserEntity?> FindUser(string username)
    {
        return await _context.Users
            .Where(user => user.Username == username)
            .FirstOrDefaultAsync();
    }

    public bool VerifyPassword(UserEntity user, string plainPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(plainPassword, user.HashedPassword);
    }

    public async Task ChangePassword(UserEntity user, string newPassword)
    {
        user.HashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
        updateConcurrencyStamp(user);
        await _context.SaveChangesAsync();
    }

    public async Task AddUser(UserEntity user, string plainPassword)
    {
        user.HashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(plainPassword);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(UserEntity user)
    {
        _context.Users.Attach(user);
        updateConcurrencyStamp(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(string username)
    {
        var rows = await _context.Users
            .Where(user => user.Username == username)
            .ExecuteDeleteAsync();
        
        if (rows == 0)
            throw new UserNotFoundException(username);
    }

    public async Task UpdateRoles(UserEntity user, IList<string> roles)
    {
        updateConcurrencyStamp(user);
        user.Roles = roles;
        await _context.SaveChangesAsync();
    }
    
    private void updateConcurrencyStamp(UserEntity user)
    {
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
    }
    
    public async Task<bool> LoginByPassword(UserEntity user, string plainPassword)
    {
        // TODO: 로그인 기록
        if (VerifyPassword(user, plainPassword))
            return true;
        else
            return false;
    }
}