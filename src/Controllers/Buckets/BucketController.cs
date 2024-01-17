using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Buckets;

[Route("buckets")]
public class BucketController : Controller
{
    private readonly BucketService _bucketService;

    public BucketController(BucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var buckets = await _bucketService.GetAllBuckets();
        return Ok(buckets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetBucket(string id)
    {
        var bucket = await _bucketService.GetBucketById(id);
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
}