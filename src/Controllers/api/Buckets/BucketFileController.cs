using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.Buckets;

[Route("api/buckets/{id}")]
public class BucketFileController : Controller
{
    private readonly ChecksumStorageBucketService _bucketService;
    private readonly ILogger<BucketFileController> _logger;

    public BucketFileController(
        ChecksumStorageBucketService bucketService,
        ILogger<BucketFileController> logger)
    {
        _bucketService = bucketService;
        _logger = logger;
    }

    [HttpGet("files")]
    public async Task<ActionResult> GetFiles(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        var files = await bucket.GetFiles();
        return Ok(new BucketFilesDTO
        {
            Id = id,
            LastUpdated = bucket.LastUpdated,
            Files = files.ToArray()
        });
    }

    [HttpPost("sync")]
    public async Task<ActionResult> PostSync(string id, BucketSyncRequestDTO request)
    {
        if (request.Files == null)
        {
            return BadRequest("request.Files was null");
        }

        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        var syncFiles = request.Files.Select(f => new BucketSyncFile
        {
            Path = f.Path,
            Size = f.Size,
            Checksum = f.Checksum
        });

        var result = await bucket.Sync(syncFiles);
        if (result.IsSuccess)
        {
            await _bucketService.UpdateBucket(id, bucket);
            return Ok(result.UpdatedAt);
        }
        else
        {
            return BadRequest(result.RequiredActions);
        }
    }
}