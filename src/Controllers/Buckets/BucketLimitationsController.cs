using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Models.Buckets;

[Route("buckets/{id}")]
public class BucketLimitationsController : Controller
{
    private readonly BucketService _bucketService;

    public BucketLimitationsController(BucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [HttpGet("limitations")]
    public async Task<ActionResult> Index(string id)
    {
        var bucket = await _bucketService.GetBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        return Ok(bucket.Limitations);
    }

    [HttpPut("limitations")]
    public async Task<ActionResult> Put(string id, BucketLimitations updateTo)
    {
        var bucket = await _bucketService.GetBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        bucket.Limitations = updateTo;
        return NoContent();
    }
}