using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Views.Web.ChecksumStorages;

[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class ListModel : PageModel
{
    private readonly ChecksumStorageService _storageService;

    public ListModel(ChecksumStorageService storageService)
    {
        _storageService = storageService;
    }

    [BindProperty]
    public IEnumerable<ChecksumStorageListItem> Items { get; set; } = [];

    public async Task OnGet()
    {
        Items = await _storageService.GetAllStorages();
    }
}
