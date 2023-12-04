using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers;

[Route("buckets")]
public class BucketsController : Controller
{
    private readonly IBucketFactory _bucketFactory;
    private readonly IBucketRepository _bucketRepository;
    private readonly ILogger<BucketsController> _logger;

    public BucketsController(
        IBucketFactory bucketFactory,
        IBucketRepository bucketRepository,
        ILogger<BucketsController> logger)
    {
        _bucketFactory = bucketFactory;
        _bucketRepository = bucketRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var buckets = await _bucketRepository.GetAllBuckets();
        ViewData["buckets"] = buckets;
        return View();
    }

    [HttpGet("{id}/files")]
    public async Task<ActionResult> GetFiles(string id)
    {
        var bucket = await getBucket(id);
        var files = bucket.GetFiles();

        ViewData["id"] = id;
        ViewData["lastUpdated"] = bucket.LastUpdated;
        ViewData["files"] = files;

        return View();
    }

    [HttpPost("{id}/sync")]
    public async Task<ActionResult> PostSync(string id, BucketSyncRequestDTO request)
    {
        if (request.Files == null)
        {
            return BadRequest("request.Files was null");
        }

        var bucket = await getBucket(id);
        await bucket.Sync(request.Files.Select(f => new BucketSyncFile()
        {
            Path = f.Path,
            Size = f.Size,
            Checksum = f.Checksum
        }));
        return NoContent();
    }

    private async Task<IBucket> getBucket(string id)
    {
        var bucketEntity = await _bucketRepository.FindBucketById(id);
        return await _bucketFactory.Create(
            bucketEntity.Id, 
            bucketEntity.LastUpdated);
    }
}