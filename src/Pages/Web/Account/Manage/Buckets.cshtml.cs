using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Account.Manage;

[Authorize(Roles = UserRoleNames.BucketUser)]
public class BucketsModel : PageModel
{
    private readonly BucketOwnerService _bucketOwnerService;

    public BucketsModel(BucketOwnerService bucketOwnerService)
    {
        _bucketOwnerService = bucketOwnerService;
    }

    public string? StatusMessage { get; set; }
    public IEnumerable<BucketListItem> Buckets { get; set; } = [];
    
    public async Task<IActionResult> OnGet([FromRoute] string username)
    {
        if (!checkPermission(username))
            return Forbid();
        
        Buckets = await _bucketOwnerService.GetBuckets(username);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        [FromRoute] string username, 
        [FromBody] string bucketId)
    {
        if (!checkPermission(username))
            return Forbid();
        
        await _bucketOwnerService.RemoveOwner(bucketId, username);
        return RedirectToPage();
    }

    private bool checkPermission(string username)
    {
        return User.IsInRole(UserRoleNames.BucketAdmin);
    }
}