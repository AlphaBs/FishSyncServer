using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

public class ViewBucketSyncEventsModel : PageModel
{
    private readonly BucketService _bucketService;
    private readonly BucketOwnerService _bucketOwnerService;

    public ViewBucketSyncEventsModel(
        BucketService bucketService,
        BucketOwnerService bucketOwnerService)
    {
        _bucketService = bucketService;
        _bucketOwnerService = bucketOwnerService;
    }

    public string? Id { get; set; } 
    public IReadOnlyCollection<BucketSyncEventEntity> Events { get; set; } = [];
    
    public async Task<ActionResult> OnGetAsync(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            if (User.IsInRole(UserRoleNames.BucketAdmin))
            {
                Events = await _bucketService.GetAllSyncEvents();
            }
            else
            {
                return Forbid();
            }
        }
        else
        {
            if (await checkPermission(id))
            {
                Id = id;
                Events = await _bucketService.GetSyncEvents(id);
            }
            else
            {
                return Forbid();
            }
        }
        return Page();
    }

    private async Task<bool> checkPermission(string bucketId)
    {
        if (User.IsInRole(UserRoleNames.BucketAdmin))
            return true;

        var username = CookieAuthService.GetUsername(User.Claims);
        if (string.IsNullOrEmpty(username))
            return false;

        var ownership = await _bucketOwnerService.CheckOwnershipByUsername(bucketId, username);
        return ownership;
    }
}
