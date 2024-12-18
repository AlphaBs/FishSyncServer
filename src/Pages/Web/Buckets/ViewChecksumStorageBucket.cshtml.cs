using System.ComponentModel;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

[Authorize(Roles = UserRoleNames.BucketUser)]
public class ViewChecksumStorageBucketModel : PageModel
{
    private readonly ChecksumStorageBucketService _bucketService;
    private readonly BucketOwnerService _bucketOwnerService;

    public ViewChecksumStorageBucketModel(
        ChecksumStorageBucketService bucketService,
        BucketOwnerService bucketOwnerService)
    {
        _bucketService = bucketService;
        _bucketOwnerService = bucketOwnerService;
    }

    [BindProperty]
    public string Id { get; set; } = default!;
    public IEnumerable<string> Owners { get; set; } = [];
    public BucketLimitations Limitations { get; set; } = default!;
    [DisplayName("사용중인 버킷 용량")]
    public long CurrentBucketSize { get; set; }
    [DisplayName("가장 큰 파일의 용량")]
    public long CurrentMaxFileSize { get; set; }
    [DisplayName("저장된 파일 수")]
    public int CurrentFileCount { get; set; }
    [DisplayName("이번달 동기화 횟수")]
    public int CurrentMonthlySyncCount { get; set; }
    public string StorageId { get; set; } = default!;
    public IEnumerable<BucketFile> Files { get; set; } = [];
    
    public async Task<ActionResult> OnGetAsync(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        Id = id;
        Limitations = bucket.Limitations;
        Files = await bucket.GetFiles();
        CurrentMonthlySyncCount = await _bucketService.GetMonthlySuccessfulSyncCount(id);
        StorageId = await _bucketService.GetStorageId(id);
        Owners = await _bucketOwnerService.GetOwners(id);

        foreach (var file in Files)
        {
            CurrentBucketSize += file.Metadata.Size;
            CurrentMaxFileSize = Math.Max(CurrentMaxFileSize, file.Metadata.Size);
            CurrentFileCount++;
        }
        return Page();
    }
}
