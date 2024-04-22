using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.ViewModels.Buckets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.Buckets;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("web/buckets/{bucketId}")]
public class BucketController : Controller
{
    private readonly ChecksumStorageBucketService _bucketService;

    public BucketController(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [HttpGet]
    public async Task<ActionResult> Index(string bucketId)
    {
        var bucket = await _bucketService.FindBucketById(bucketId);
        if (bucket == null)
        {
            return NotFound();
        }

        var files = await bucket.GetFiles();
        var storageId = await _bucketService.GetStorageId(bucketId);
        return View("/Views/Buckets/Bucket.cshtml", new BucketViewModel
        {
            Id = bucketId,
            Limitations = bucket.Limitations,
            LastUpdated = bucket.LastUpdated,
            Files = files,
            StorageId = storageId,
        });
    }

    [HttpPost]
    [Authorize(Roles = UserRoleNames.BucketAdmin)]
    public async Task<ActionResult> Post(string bucketId, BucketViewModel request)
    {
        if (request.Limitations == null || string.IsNullOrEmpty(request.StorageId))
        {
            return BadRequest();
        }

        await _bucketService.UpdateLimitationsAndStorageId(bucketId, request.Limitations, request.StorageId);
        return NoContent();
    }
}