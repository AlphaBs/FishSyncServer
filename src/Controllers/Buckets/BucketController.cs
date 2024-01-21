using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Controllers.Buckets;

[Route("buckets")]
public class BucketController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public BucketController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var buckets = await _dbContext.Buckets.ToListAsync();
        return Ok(new 
        {
            Buckets = buckets.Select(bucket => bucket.Id)
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetBucket(string id)
    {
        var bucket = await _dbContext.Buckets
            .Where(bucket => bucket.Id == id)
            .FirstOrDefaultAsync();
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