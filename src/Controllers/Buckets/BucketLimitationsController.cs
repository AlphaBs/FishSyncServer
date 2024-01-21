using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Models.Buckets;

[Route("buckets/{id}/limitations")]
public class BucketLimitationsController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public BucketLimitationsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult> Index(string id)
    {
        var bucket = await _dbContext.Buckets
            .Where(bucket => bucket.Id == id)
            .FirstOrDefaultAsync();
        if (bucket == null)
        {
            return NotFound();
        }

        return Ok(bucket.Limitations);
    }

    [HttpPut]
    public async Task<ActionResult> Put(string id, BucketLimitations updateTo)
    {
        var bucket = await _dbContext.Buckets
            .Where(bucket => bucket.Id == id)
            .FirstOrDefaultAsync();
        if (bucket == null)
        {
            return NotFound();
        }

        bucket.Limitations = updateTo;
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
}