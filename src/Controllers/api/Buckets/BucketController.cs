using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Controllers.Api.Buckets;

[Route("api/buckets")]
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
        return Ok(new 
        {
            Buckets = buckets.Select(bucket => bucket.Id)
        });
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