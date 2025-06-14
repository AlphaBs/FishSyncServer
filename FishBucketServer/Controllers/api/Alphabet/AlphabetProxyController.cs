using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.Services.Buckets;
using FishBucket.Alphabet;
using FishBucket.ApiClient;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.Alphabet;

[ApiController]
[Route("api/alphabet")]
[Produces("application/json")]
public class AlphabetProxyController : ControllerBase
{
    private readonly BucketFilesCacheService _bucketService;

    public AlphabetProxyController(BucketFilesCacheService bucketService)
    {
        _bucketService = bucketService;
    }

    // 레거시 API 호환을 위한 endpoint, API 수정 전 호환성 확인하기!
    // files.php?instanceId={id}
    [HttpGet("files.php")]
    public Task<ActionResult> GetFilesLegacy([FromQuery(Name = "instanceId")] string instanceId)
    {
        return GetFiles(instanceId);
    }

    // bucket dependency resolving 한 결과를 Alphabet API 형식에 맞게 응답 
    [HttpGet("files/{id}")]
    [ProducesResponseType<BucketFiles>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetFiles(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            // dependency resolving
            var client = new LocalFishApiClient(_bucketService);
            var bucket = await FishBucketDependencyResolver.Resolve(client, id, 8, cancellationToken);

            // serialize
            var files = bucket.Files.Select(f => new UpdateFile
            {
                Url = f.Location,
                Path = f.Path,
                Hash = f.Metadata.Checksum,
                Tags = null,
                Size = f.Metadata.Size
            });

            var response = new LauncherMetadata
            {
                LastInfoUpdate = bucket.LastUpdated.UtcDateTime,
                Launcher = null,
                Files = new UpdateFileCollection
                {
                    LastUpdate = bucket.LastUpdated,
                    HashAlgorithm = "md5",
                    Files = files
                }
            };

            return Ok(response);
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