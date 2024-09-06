using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Views.Web.ChecksumStorages;

[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class EditRFilesModel : PageModel
{
    private readonly RFilesChecksumStorageService _storageService;

    public EditRFilesModel(RFilesChecksumStorageService storageService)
    {
        _storageService = storageService;
    }

    [BindProperty]
    public string Id { get; set; } = default!;

    [BindProperty]
    public bool IsReadonly { get; set; } = false;

    [BindProperty]
    public string Endpoint { get; set; } = default!;

    [BindProperty]
    public string? ClientSecret { get; set; }

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = new RFilesChecksumStorageEntity
        {
            Id = Id,
            IsReadonly = IsReadonly,
            Host = Endpoint,
            ClientSecret = ClientSecret,
        };
        await _storageService.Create(entity);

        return RedirectToPage("./List");
    }
}
