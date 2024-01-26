using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.ViewModels;
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

        await _bucketService.AddNewBucket(request.Id, request.Limitations);
        return NoContent();
    }
}