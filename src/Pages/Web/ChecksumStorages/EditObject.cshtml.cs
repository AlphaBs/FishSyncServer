using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.ChecksumStorages;

[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class EditObjectModel : PageModel
{
    private readonly ObjectChecksumStorageService _storageService;

    public EditObjectModel(ObjectChecksumStorageService storageService)
    {
        _storageService = storageService;
    }

    [BindProperty] 
    public ObjectChecksumStorageEntity Entity { get; set; } = new();

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

    public async Task<ActionResult> OnPostEdit(string id)
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

        entity.IsReadonly = Entity.IsReadonly;
        entity.AccessKey = Entity.AccessKey;
        entity.SecretKey = Entity.SecretKey;
        entity.BucketName = Entity.BucketName;
        entity.Prefix = Entity.Prefix;
        entity.PublicEndpoint = Entity.PublicEndpoint;
        entity.ServiceEndpoint = Entity.ServiceEndpoint;
        await _storageService.Update(entity);

        return RedirectToPage("EditObject", entity.Id);
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
