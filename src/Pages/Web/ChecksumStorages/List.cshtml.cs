using AlphabetUpdateServer.Services.ChecksumStorages;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.ChecksumStorages;

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
