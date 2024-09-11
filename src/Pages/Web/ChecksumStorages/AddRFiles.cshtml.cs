using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.ChecksumStorages;

[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class AddRFilesModel : PageModel
{
    private readonly RFilesChecksumStorageService _storageService;

    public AddRFilesModel(RFilesChecksumStorageService storageService)
    {
        _storageService = storageService;
    }

    [BindProperty] public RFilesChecksumStorageEntity Entity { get; set; } = new();

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = new RFilesChecksumStorageEntity
        {
            Id = Entity.Id,
            IsReadonly = Entity.IsReadonly,
            Host = Entity.Host,
            ClientSecret = Entity.ClientSecret,
        };
        await _storageService.Create(entity);

        return RedirectToPage("./List");
    }
}
