using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.ViewModels.Buckets;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.Buckets;

[Route("web/buckets/{bucket}")]
public class BucketController : Controller
{
    private readonly ChecksumStorageBucketService _bucketService;

    public BucketController(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAsync(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        var files = await bucket.GetFiles();
        return View("Views/Buckets/Bucket", new BucketViewModel
        {
            BucketId = id,
            Bucket = bucket,
            Files = files
        });
    }
}