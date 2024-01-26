using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.ViewModels.Buckets;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.Buckets;

[Route("web/buckets")]
public class BucketsController : Controller
{
    private readonly BucketService _bucketService;

    public BucketsController(BucketService bucketService)
    {
        _bucketService = bucketService;
    }

    public async Task<ActionResult> GetAsync()
    {
        var buckets = await _bucketService.GetAllBuckets();
        return View("Views/Buckets/Index", new BucketsViewModel
        {
            Buckets = buckets.ToArray()
        });
    }
}