using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class EditBucketIndex : PageModel
{
    private readonly BucketIndexService _bucketIndexService;

    public EditBucketIndex(BucketIndexService bucketIndexService)
    {
        _bucketIndexService = bucketIndexService;
    }

    [FromRoute]
    public string Id { get; set; } = default!;
    
    [BindProperty]
    public string? Description { get; set; }

    [BindProperty]
    public bool Searchable { get; set; }
    
    public List<string> Buckets { get; set; } = [];

    public async Task<ActionResult> OnGetAsync()
    {
        var index = await _bucketIndexService.FindIndex(Id);
        if (index == null)
            return NotFound();

        Description = index.Description;
        Searchable = index.Searchable;
        
        Buckets = (await _bucketIndexService.GetBucketsFromIndex(Id)).ToList();
        return Page();
    }

    public async Task<ActionResult> OnPostAsync()
    {
        var index = new BucketIndex(Id)
        {
            Description = Description,
            Searchable = Searchable
        };
        await _bucketIndexService.UpdateIndexMetadata(index);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostAddBucketAsync(string bucket)
    {
        await _bucketIndexService.AddBucketToIndex(Id, bucket);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostRemoveBucketAsync(string bucket)
    {
        await _bucketIndexService.RemoveBucketFromIndex(Id, bucket);
        return RedirectToPage();
    }

    public async Task<ActionResult> OnPostDeleteAsync()
    {
        await _bucketIndexService.RemoveIndex(Id);
        return RedirectToPage("ListBucketIndex");
    }
}