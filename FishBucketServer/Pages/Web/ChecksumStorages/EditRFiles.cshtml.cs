using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services.ChecksumStorages;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.ChecksumStorages;

[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class EditRFilesModel : PageModel
{
    private readonly RFilesChecksumStorageService _storageService;

    public EditRFilesModel(RFilesChecksumStorageService storageService)
    {
        _storageService = storageService;
    }

    [BindProperty] public RFilesChecksumStorageEntity Entity { get; set; } = new();

    public async Task<ActionResult> OnGetAsync(string id)
    {
        var entity = await _storageService.FindEntityById(id);
        if (entity == null)
        {
            return NotFound();
        }

        Entity = entity;
        return Page();
    }
    
    public async Task<ActionResult> OnPostAsync(string id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Entity.Id != id)
        {
            return NotFound();
        }

        var entity = await _storageService.FindEntityById(id);
        if (entity == null)
        {
            return NotFound();
        }

        entity.IsReadonly = entity.IsReadonly;
        entity.ClientSecret = entity.ClientSecret;
        entity.Host = entity.Host;
        await _storageService.Update(entity);
        
        return RedirectToPage("EditRFiles");
    }
    
    public async Task<ActionResult> OnPostDelete(string id)
    {
        var deleted = await _storageService.Delete(id);

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
