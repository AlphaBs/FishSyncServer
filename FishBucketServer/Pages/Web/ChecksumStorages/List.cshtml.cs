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

    public IAsyncEnumerable<ChecksumStorageListItem> Items { get; set; } =
        AsyncEnumerable.Empty<ChecksumStorageListItem>();

    public void OnGet()
    {
        Items = _storageService.GetAllStorages();
    }
}
