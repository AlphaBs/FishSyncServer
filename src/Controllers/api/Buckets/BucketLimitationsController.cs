using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.Buckets;

[Route("api/buckets/{id}/limitations")]
public class BucketLimitationsController : Controller
{
    private readonly ChecksumStorageBucketService _bucketService;

    public BucketLimitationsController(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [HttpGet]
    public async Task<ActionResult> Index(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        return Ok(bucket.Limitations);
    }

    [HttpPut]
    public async Task<ActionResult> Put(string id, BucketLimitations updateTo)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        bucket.Limitations = updateTo;
        await _bucketService.UpdateBucket(id, bucket);
        return NoContent();
    }
}