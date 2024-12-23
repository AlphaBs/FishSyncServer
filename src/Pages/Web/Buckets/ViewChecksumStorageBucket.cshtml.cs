using System.ComponentModel;
using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Pages.Shared;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketUser)]
public class ViewChecksumStorageBucketModel : PageModel
{
    private readonly BucketService _bucketService;
    private readonly BucketServiceFactory _bucketServiceFactory;
    private readonly BucketOwnerService _bucketOwnerService;

    public ViewChecksumStorageBucketModel(
        BucketService bucketService,
        BucketOwnerService bucketOwnerService, 
        BucketServiceFactory bucketServiceFactory)
    {
        _bucketService = bucketService;
        _bucketOwnerService = bucketOwnerService;
        _bucketServiceFactory = bucketServiceFactory;
    }

    [BindProperty]
    public string Id { get; set; } = default!;
    public string BucketType { get; set; } = ChecksumStorageBucketService.ChecksumStorageType;
    public BucketUsageModel Usage { get; set; } = new();
    public IEnumerable<string> Owners { get; set; } = [];
    public string StorageId { get; set; } = default!;
    public IEnumerable<BucketFile> Files { get; set; } = [];

    private ChecksumStorageBucketService getService()
    {
        return (ChecksumStorageBucketService)_bucketServiceFactory.GetRequiredService(ChecksumStorageBucketService.ChecksumStorageType);
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
        Usage.Limitations = bucket.Limitations;
        Usage.CurrentMonthlySyncCount = await _bucketService.GetMonthlySuccessfulSyncCount(id);
        StorageId = await getService().GetStorageId(id);
        Owners = await _bucketOwnerService.GetOwners(id);

        foreach (var file in Files)
        {
            Usage.CurrentBucketSize += file.Metadata.Size;
            Usage.CurrentMaxFileSize = Math.Max(Usage.CurrentMaxFileSize, file.Metadata.Size);
            Usage.CurrentFileCount++;
        }
        return Page();
    }
}
