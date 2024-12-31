using AlphabetUpdateServer.Services.Buckets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

public class ViewBucketIndex : PageModel
{
    private readonly BucketIndexService _bucketIndexService;

    public ViewBucketIndex(BucketIndexService bucketIndexService)
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
}