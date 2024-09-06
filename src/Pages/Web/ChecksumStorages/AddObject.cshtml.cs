using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Views.Web.ChecksumStorages;

[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class AddObjectModel : PageModel
{
    private readonly ObjectChecksumStorageService _storageService;

    public AddObjectModel(ObjectChecksumStorageService storageService)
    {
        _storageService = storageService;
    }

    [BindProperty]
    public string Id { get; set; } = default!;

    [BindProperty]
    public bool IsReadonly { get; set; } = false;

    [BindProperty]
    public string AccessKey { get; set; } = default!;

    [BindProperty]
    public string SecretKey { get; set; } = default!;

    [BindProperty]
    public string BucketName { get; set; } = default!;

    [BindProperty]
    public string Prefix { get; set; } = default!;

    [BindProperty]
    public string PublicEndpoint { get; set; } = default!;

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = new ObjectChecksumStorageEntity
        {
            Id = Id,
            IsReadonly = IsReadonly,
            AccessKey = AccessKey,
            SecretKey = SecretKey,
            BucketName = BucketName,
            PublicEndpoint = PublicEndpoint,
            Prefix = Prefix
        };
        await _storageService.Create(entity);

        return RedirectToPage("./List");
    }
}
