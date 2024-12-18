using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketUser)]
public class ViewBucketSyncEventsModel : PageModel
{
    private readonly ChecksumStorageBucketService _bucketService;
    private readonly BucketOwnerService _bucketOwnerService;

    public ViewBucketSyncEventsModel(
        ChecksumStorageBucketService bucketService,
        BucketOwnerService bucketOwnerService)
    {
        _bucketService = bucketService;
        _bucketOwnerService = bucketOwnerService;
    }

    [BindProperty] public string Id { get; set; } = default!;
    [BindProperty] public IReadOnlyCollection<BucketSyncEventEntity> Events { get; set; } = [];
    
    public async Task<ActionResult> OnGetAsync(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        Id = id;
        Events = await _bucketService.GetSyncEvents(id);
        
        return Page();
    }
}
