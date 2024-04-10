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
        return View("/Views/Buckets/Index.cshtml", new BucketsViewModel
        {
            Buckets = buckets.ToList()
        });
    }

    [HttpGet("add")]
    public ActionResult GetAddAsync()
    {
        return View("/Views/Buckets/Add.cshtml");
    }

    [HttpPost("add")]
    public async Task<ActionResult> PostAddAsync(AddBucketViewModel request)
    {
        if (string.IsNullOrEmpty(request.Id) || request.Limitations == null || string.IsNullOrEmpty(request.StorageId))
        {
            return BadRequest();
        }

        await _bucketService.CreateBucket(request.Id, request.Limitations, request.StorageId);
        return NoContent();
    }
}