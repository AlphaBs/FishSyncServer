using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Controllers.Buckets;

[Route("buckets/{id}")]
public class BucketFileController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ChecksumStorageService _checksumStorageService;
    private readonly ILogger<BucketFileController> _logger;

    public BucketFileController(
        ApplicationDbContext dbContext,
        ChecksumStorageService checksumStorageService,
        ILogger<BucketFileController> logger)
    {
        _dbContext = dbContext;
        _checksumStorageService = checksumStorageService;
        _logger = logger;
    }

    [HttpGet("files")]
    public async Task<ActionResult> GetFiles(string id)
    {
        var bucket = await _dbContext.Buckets
            .Where(bucket => bucket.Id == id)
            .FirstOrDefaultAsync();
        if (bucket == null)
        {
            return NotFound();
        }

        var checksumStorage = await _checksumStorageService.CreateStorageForBucket(id);
        var files = bucket.GetFiles(checksumStorage);
        return Ok(new BucketFilesDTO
        {
            Id = bucket.Id,
            LastUpdated = bucket.LastUpdated,
            Files = files
        });
    }

    [HttpPost("sync")]
    public async Task<ActionResult> PostSync(string id, BucketSyncRequestDTO request)
    {
        if (request.Files == null)
        {
            return BadRequest("request.Files was null");
        }

        var bucket = await _dbContext.Buckets
            .Where(bucket => bucket.Id == id)
            .FirstOrDefaultAsync();
        if (bucket == null)
        {
            return NotFound();
        }

        var checksumStorage = await _checksumStorageService.CreateStorageForBucket(id);
        var syncFiles = request.Files.Select(f => new BucketSyncFile
        {
            Path = f.Path,
            Size = f.Size,
            Checksum = f.Checksum
        });

        var result = await bucket.Sync(syncFiles, checksumStorage);
        if (result.IsSuccess)
        {
            await _dbContext.SaveChangesAsync();
            return Ok(result.UpdatedAt);
        }
        else
        {
            return BadRequest(result.RequiredActions);
        }
    }
}