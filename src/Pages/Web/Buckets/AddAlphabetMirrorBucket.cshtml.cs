using System.ComponentModel.DataAnnotations;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class AddAlphabetMirrorBucketModel : PageModel
{
    private readonly BucketServiceFactory _bucketServiceFactory;
    
    public AddAlphabetMirrorBucketModel(BucketServiceFactory bucketServiceFactory)
    {
        _bucketServiceFactory = bucketServiceFactory;
    }

    [BindProperty]
    public string Id { get; set; } = default!;

    [BindProperty]
    [Required]
    public string OriginUrl { get; set; } = default!;


    public ActionResult OnGetAsync()
    {
        return Page();
    }

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var service = (AlphabetMirrorBucketService)_bucketServiceFactory.GetRequiredService(AlphabetMirrorBucketService.AlphabetMirrorType);
        await service.Create(Id, OriginUrl);
        return RedirectToPage("./List");
    }
}
