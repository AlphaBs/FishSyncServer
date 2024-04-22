using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.ViewModels.Buckets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.Buckets;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("web/buckets")]
public class BucketsController : Controller
{
    private readonly ChecksumStorageBucketService _bucketService;

    public BucketsController(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var buckets = await _bucketService.GetAllBuckets();
        return View("/Views/Buckets/Index.cshtml", new BucketsViewModel
        {
            Buckets = buckets.ToList()
        });
    }

    [HttpGet("add")]
    [Authorize(Roles = UserRoleNames.BucketAdmin)]
    public ActionResult GetAdd()
    {
        return View("/Views/Buckets/Add.cshtml");
    }

    [HttpPost("add")]
    [Authorize(Roles = UserRoleNames.BucketAdmin)]
    public async Task<ActionResult> PostAdd(BucketViewModel request)
    {
        if (string.IsNullOrEmpty(request.Id) || request.Limitations == null || string.IsNullOrEmpty(request.StorageId))
        {
            return BadRequest();
        }

        await _bucketService.CreateBucket(request.Id, request.Limitations, request.StorageId);
        return NoContent();
    }
}