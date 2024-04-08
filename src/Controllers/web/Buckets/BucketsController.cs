using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.ViewModels.Buckets;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.Buckets;

[Route("web/buckets")]
public class BucketsController : Controller
{
    private readonly ChecksumStorageBucketService _bucketService;

    public BucketsController(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAsync()
    {
        var buckets = await _bucketService.GetAllBuckets();
        return View("Views/Buckets/Index", new BucketsViewModel
        {
            Buckets = buckets.ToArray()
        });
    }

    public ActionResult GetAddAsync()
    {
        return View("Views/Buckets/Add");
    }

    [HttpPost("add")]
    public async Task<ActionResult> PostAddAsync(AddBucketViewModel request)
    {
        if (string.IsNullOrEmpty(request.Id) || request.Limitations == null)
        {
            return BadRequest();
        }

        await _bucketService.CreateBucket(request.Id, request.Limitations);
        return NoContent();
    }
}