using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.ViewModels.Buckets;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.Buckets;

[Route("web/buckets/{bucket}")]
public class BucketController : Controller
{
    private readonly BucketService _bucketService;
    private readonly ChecksumStorageService _checksumStorageService;

    public BucketController(
        BucketService bucketService, 
        ChecksumStorageService checksumStorageService)
    {
        _bucketService = bucketService;
        _checksumStorageService = checksumStorageService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAsync(string id)
    {
        var bucket = await _bucketService.GetBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }
        var checksumStorage = await _checksumStorageService.CreateStorageForBucket(id);
        var files = new List<BucketFileLocation>();
        await foreach (var file in bucket.GetFiles(checksumStorage))
        {
            files.Add(file);
        }

        return View("Views/Buckets/Bucket", new BucketViewModel
        {
            Bucket = bucket,
            Files = files
        });
    }
}