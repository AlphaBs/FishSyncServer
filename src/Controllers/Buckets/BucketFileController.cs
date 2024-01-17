using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Buckets;

[Route("buckets/{id}")]
public class BucketFileController : Controller
{
    private readonly BucketService _bucketService;
    private readonly ILogger<BucketFileController> _logger;

    public BucketFileController(
        BucketService bucketService,
        ILogger<BucketFileController> logger)
    {
        _bucketService = bucketService;
        _logger = logger;
    }

    [HttpGet("files")]
    public async Task<ActionResult> GetFiles(string id)
    {
        var bucket = await _bucketService.GetBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }
        var files = _bucketService.GetFiles(bucket);

        ViewData["id"] = id;
        ViewData["lastUpdated"] = bucket.LastUpdated;
        ViewData["files"] = files;

        return View();
    }

    [HttpPost("sync")]
    public async Task<ActionResult> PostSync(string id, BucketSyncRequestDTO request)
    {
        if (request.Files == null)
        {
            return BadRequest("request.Files was null");
        }

        var bucket = await _bucketService.GetBucketById(id);
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
        await _bucketService.Sync(bucket, syncFiles);
        return NoContent();
    }
}