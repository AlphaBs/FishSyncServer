using AlphabetUpdateServer.Entities;
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

    public string? Id { get; set; } 
    public IReadOnlyCollection<BucketSyncEventEntity> Events { get; set; } = [];
    
    public async Task<ActionResult> OnGetAsync(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {        
            Events = await _bucketService.GetAllSyncEvents();

        }
        else
        {
            Id = id;
            Events = await _bucketService.GetSyncEvents(id);
        }
        return Page();
    }
}
