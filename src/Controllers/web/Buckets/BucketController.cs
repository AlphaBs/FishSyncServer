using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.ViewModels.Buckets;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.Buckets;

[Route("web/buckets/{bucketId}")]
public class BucketController : Controller
{
    private readonly ChecksumStorageBucketService _bucketService;

    public BucketController(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAsync(string bucketId)
    {
        var bucket = await _bucketService.FindBucketById(bucketId);
        if (bucket == null)
        {
            return NotFound();
        }

        var files = await bucket.GetFiles();
        return View("/Views/Buckets/Bucket.cshtml", new BucketViewModel
        {
            BucketId = bucketId,
            Bucket = bucket,
            Files = files
        });
    }
}