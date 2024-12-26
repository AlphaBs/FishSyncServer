using AlphabetUpdateServer.Services.ChecksumStorages;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Controllers.Api.Buckets;

[ApiController]
[Route("api/buckets/checksum-storage-bucket/file-manager")]
[Authorize(AuthenticationSchemes = JwtAuthService.SchemeName, Roles = UserRoleNames.BucketAdmin)]
public class ChecksumStorageBucketFileManagerController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ChecksumStorageService _storageService;

    public ChecksumStorageBucketFileManagerController(
        ApplicationDbContext context,
        ChecksumStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    [HttpGet("{storageId}")]
    public async Task<ActionResult> OnGetAsync(string storageId)
    {
        var storage = await _storageService.GetStorage(storageId);
        if (storage is null)
        {
            return NotFound();
        }

        var orphanChecksumSet = new List<string>();
        var files = await storage.GetAllFiles();
        foreach (var file in files)
        {
            orphanChecksumSet.Add(file.Metadata.Checksum);
        }

        var checksums = await _context.ChecksumStorageBuckets
            .Include(bucket => bucket.Files)
            .Where(bucket => bucket.ChecksumStorageId == storageId)
            .SelectMany(bucket => bucket.Files)
            .Select(file => file.Metadata.Checksum)
            .Distinct()
            .ToListAsync();

        foreach (var checksum in checksums)
        {
            orphanChecksumSet.Remove(checksum);
        }
        
        return Ok(new
        {
            usedChecksums = checksums,
            orphanChecksums = orphanChecksumSet
        });
    }
}