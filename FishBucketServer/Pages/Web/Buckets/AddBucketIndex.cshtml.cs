using AlphabetUpdateServer.Models;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using FishBucket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class AddBucketIndex : PageModel
{
    private readonly BucketIndexService _bucketIndexService;

    public AddBucketIndex(BucketIndexService bucketIndexService)
    {
        _bucketIndexService = bucketIndexService;
    }

    [BindProperty] public string Id { get; set; } = default!;
    
    [BindProperty]
    public string? Description { get; set; }
    
    [BindProperty]
    public bool Searchable { get; set; }
    
    public ActionResult OnGet()
    {
        return Page();
    }

    public async Task<ActionResult> OnPostAsync()
    {
        var index = new BucketIndex(Id)
        {
            Description = Description,
            Searchable = Searchable
        };

        await _bucketIndexService.AddIndex(index);
        return RedirectToPage("ListBucketIndex");
    }
}