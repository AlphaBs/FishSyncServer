using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Account.Manage;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class BucketsModel : PageModel
{
    private readonly BucketOwnerService _bucketOwnerService;

    public BucketsModel(BucketOwnerService bucketOwnerService)
    {
        _bucketOwnerService = bucketOwnerService;
    }

    public string? StatusMessage { get; set; }
    public IAsyncEnumerable<string> Buckets { get; set; } = AsyncEnumerable.Empty<string>();
    
    public IActionResult OnGet([FromRoute] string username)
    {
        Buckets = _bucketOwnerService.GetBuckets(username);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        [FromRoute] string username, 
        [FromForm] string bucket)
    {
        await _bucketOwnerService.RemoveOwner(bucket, username);
        return RedirectToPage();
    }
}