using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services.ChecksumStorages;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.ChecksumStorages;

[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class Edit : PageModel
{
    private readonly ChecksumStorageService _checksumStorageService;

    public Edit(ChecksumStorageService checksumStorageService)
    {
        _checksumStorageService = checksumStorageService;
    }

    public ActionResult OnGet(string id, string type)
    {
        switch (type)
        {
            case RFilesChecksumStorageEntity.RFilesType:
                return RedirectToPage("EditRFiles", new { id });
            case ObjectChecksumStorageEntity.ObjectType:
                return RedirectToPage("EditObject", new { id });
            default:
                return NotFound();
        }
    }
}