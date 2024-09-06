using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Views.Web.ChecksumStorages;

[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class EditObjectModel : PageModel
{
    private readonly ObjectChecksumStorageService _storageService;

    public EditObjectModel(ObjectChecksumStorageService storageService)
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

    public async Task<ActionResult> OnGetAsync(string id)
    {
        Id = id;

        var entity = await _storageService.FindEntityById(Id);
        if (entity == null)
        {
            return NotFound();
        }

        IsReadonly = entity.IsReadonly;
        AccessKey = entity.AccessKey;
        SecretKey = entity.SecretKey;
        BucketName = entity.BucketName;
        Prefix = entity.Prefix;
        PublicEndpoint = entity.PublicEndpoint;

        return Page();
    }

    public async Task<ActionResult> OnPostEdit(string id)
    {
        Id = id;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = await _storageService.FindEntityById(Id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.IsReadonly = IsReadonly;
        entity.AccessKey = AccessKey;
        entity.SecretKey = SecretKey;
        entity.BucketName = BucketName;
        entity.Prefix = Prefix;
        entity.PublicEndpoint = PublicEndpoint;
        await _storageService.Update(entity);

        return RedirectToPage("EditObject", entity.Id);
    }

    public async Task<ActionResult> OnPostDelete(string id)
    {
        Id = id;
        var deleted = await _storageService.Delete(Id);

        if (deleted)
        {
            return RedirectToPage("List");
        }
        else
        {
            return NotFound();
        }
    }
}
