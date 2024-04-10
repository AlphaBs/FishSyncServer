using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.Buckets;

[Route("api/buckets")]
public class BucketController : Controller
{
    private readonly ChecksumStorageBucketService _bucketService;

    public BucketController(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var buckets = await _bucketService.GetAllBuckets();
        return Ok(new 
        {
            Buckets = buckets.Select(bucket => bucket.Id)
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetBucket(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        var dto = new BucketDTO()
        {
            Id = id,
            Limitations = bucket.Limitations,
            LastUpdated = bucket.LastUpdated
        };
        return Ok(dto);
    }

    [HttpPost("{id}/sync")]
    public async Task<ActionResult> SyncBucket(string id, BucketSyncRequestDTO body)
    {
        if (body?.Files == null)
        {
            return BadRequest();
        }

        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        var result = await bucket.Sync(body.Files);
        return Ok(result);
    }
}