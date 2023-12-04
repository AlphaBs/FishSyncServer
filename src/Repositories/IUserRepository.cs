using AlphabetUpdateServer.Entities;

namespace AlphabetUpdateServer.Repositories;

public interface IUserRepository
{
    ValueTask<IEnumerable<UserEntity>> GetAllUsers();
    ValueTask<UserEntity?> FindUserById(string id);
}