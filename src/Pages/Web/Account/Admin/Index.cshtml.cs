using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Pages.Web.Account.Admin;

[Authorize(Roles = UserRoleNames.UserAdmin)]
public class IndexModel : PageModel
{
    private readonly UserService _userService;

    public IndexModel(UserService userService)
    {
        _userService = userService;
    }

    public IAsyncEnumerable<UserEntity> Users { get; private set; } = AsyncEnumerable.Empty<UserEntity>();

    public IActionResult OnGetAsync()
    {
        Users = _userService.GetAllUsers();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string username)
    {
        await _userService.DeleteUser(username);
        return RedirectToPage();
    }
}