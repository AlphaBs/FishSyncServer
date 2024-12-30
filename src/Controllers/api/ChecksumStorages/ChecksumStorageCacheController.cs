using AlphabetUpdateServer.Services.ChecksumStorageCaches;
using AlphabetUpdateServer.Services.ChecksumStorages;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.ChecksumStorages;

[ApiController]
[Route("api/checksum-storages/cache")]
[Authorize(AuthenticationSchemes = JwtAuthService.SchemeName, Roles = UserRoleNames.StorageAdmin)]
public class ChecksumStorageCacheController : ControllerBase
{
    private readonly IChecksumStorageCache _checksumStorageCache;

    public ChecksumStorageCacheController(IChecksumStorageCache checksumStorageCache)
    {
        _checksumStorageCache = checksumStorageCache;
    }

    [HttpGet("list/{id}")]
    public IActionResult ListCache(string id)
    {
        if (_checksumStorageCache is RedisChecksumStorageCache redisChecksumStorageCache)
        {
            var keys = redisChecksumStorageCache.GetAllCacheKeys(id);
            return Ok(keys);
        }
        
        return Problem("not supported storage type");
    }

    [HttpGet("purge/{id}")]
    public async Task<IActionResult> Purge(string id)
    {
        if (_checksumStorageCache is RedisChecksumStorageCache redisChecksumStorageCache)
        {
            var keys = redisChecksumStorageCache.GetAllCacheKeys(id);
            await redisChecksumStorageCache.DeleteFiles(id, keys);
            return Ok(keys);
        }
        
        return Problem("not supported storage type");
    }
}