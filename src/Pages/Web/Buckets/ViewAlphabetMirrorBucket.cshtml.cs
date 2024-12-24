using System.ComponentModel;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Pages.Shared;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketUser)]
public class ViewAlphabetMirrorBucketModel : PageModel
{
    private readonly BucketService _bucketService;
    private readonly BucketServiceFactory _bucketServiceFactory;

    public ViewAlphabetMirrorBucketModel(
        BucketService bucketService,
        BucketServiceFactory bucketServiceFactory)
    {
        _bucketService = bucketService;
        _bucketServiceFactory = bucketServiceFactory;
    }

    [BindProperty]
    public string Id { get; set; } = default!;
    [DisplayName("전체 용량")]
    public long CurrentBucketSize { get; set; }
    [DisplayName("최대 파일 크기")]
    public long CurrentMaxFileSize { get; set; }
    [DisplayName("파일 갯수")]
    public long CurrentFileCount { get; set; }
    public IEnumerable<string> Owners { get; set; } = [];
    public string OriginUrl { get; set; } = default!;
    public IEnumerable<BucketFile> Files { get; set; } = [];

    private AlphabetMirrorBucketService getService()
    {
        return (AlphabetMirrorBucketService)_bucketServiceFactory.GetRequiredService(AlphabetMirrorBucketService.AlphabetMirrorType);
    }
    
    public async Task<ActionResult> OnGetAsync(string id)
    {
        var bucket = await _bucketService.Find(id);
        if (bucket == null)
        {
            return NotFound();
        }

        Id = id;
        Files = await bucket.GetFiles();
        OriginUrl = await getService().GetOriginUrl(id);

        foreach (var file in Files)
        {
            CurrentBucketSize += file.Metadata.Size;
            CurrentMaxFileSize = Math.Max(CurrentMaxFileSize, file.Metadata.Size);
            CurrentFileCount++;
        }
        return Page();
    }
}
