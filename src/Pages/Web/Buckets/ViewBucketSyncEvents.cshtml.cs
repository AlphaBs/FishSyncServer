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
    private readonly BucketService _bucketService;

    public ViewBucketSyncEventsModel(BucketService bucketService)
    {
        _bucketService = bucketService;
    }

    [BindProperty] public string Id { get; set; } = default!;
    [BindProperty] public IReadOnlyCollection<BucketSyncEventEntity> Events { get; set; } = [];
    
    public async Task<ActionResult> OnGetAsync(string id)
    {
        var bucket = await _bucketService.Find(id);
        if (bucket == null)
        {
            return NotFound();
        }

        Id = id;
        Events = await _bucketService.GetSyncEvents(id);
        
        return Page();
    }
}
