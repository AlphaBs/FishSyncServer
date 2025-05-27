using AlphabetUpdateServer.Services.Buckets;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.Alphabet;

[ApiController]
[Route("api/alphabet")]
[Produces("application/json")]
public class AlphabetProxyController : ControllerBase
{
    private readonly BucketService _bucketService;
    private readonly BucketFilesCacheService _bucketFilesCacheService;
    
    public AlphabetProxyController(BucketService bucketService, BucketFilesCacheService bucketFilesCacheService)
    {
        _bucketService = bucketService;
        _bucketFilesCacheService = bucketFilesCacheService;
    }
    
    // 레거시 API 호환을 위한 endpoint, API 수정 전 호환성 확인하기!
    // files.php?instanceId={id}
    [HttpGet("files.php")]
    public Task<ActionResult> GetFilesLegacy([FromQuery(Name = "instanceId")] string instanceId)
    {
        return GetFiles(instanceId);
    }
    
    [HttpGet("files/{id}")]
    public async Task<ActionResult> GetFiles(string id)
    {
        try
        {
            var files = await _bucketFilesCacheService.GetOrCreate(
                id, 
                () => _bucketService.GetBucketFiles(id));
            return Ok(files);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (FormatException)
        {
            return BadRequest();
        }
    }
}