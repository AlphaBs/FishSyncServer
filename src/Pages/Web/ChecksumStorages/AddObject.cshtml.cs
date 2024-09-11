using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.ChecksumStorages;

[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class AddObjectModel : PageModel
{
    private readonly ObjectChecksumStorageService _storageService;

    public AddObjectModel(ObjectChecksumStorageService storageService)
    {
        _storageService = storageService;
    }

    [BindProperty] public ObjectChecksumStorageEntity Entity { get; set; } = new();

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = new ObjectChecksumStorageEntity
        {
            Id = Entity.Id,
            IsReadonly = Entity.IsReadonly,
            AccessKey = Entity.AccessKey,
            SecretKey = Entity.SecretKey,
            BucketName = Entity.BucketName,
            ServiceEndpoint = Entity.ServiceEndpoint,
            PublicEndpoint = Entity.PublicEndpoint,
            Prefix = Entity.Prefix
        };
        await _storageService.Create(entity);

        return RedirectToPage("./List");
    }
}
