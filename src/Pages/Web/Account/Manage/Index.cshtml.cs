using System.ComponentModel.DataAnnotations;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Account.Manage;

public class IndexModel : PageModel
{
    private readonly UserService _userService;

    public IndexModel(UserService userService)
    {
        _userService = userService;
    }

    public string Username { get; set; } = null!;
    
    [BindProperty]
    [DataType(DataType.EmailAddress)]
    [StringLength(128)]
    public string? Email { get; set; }
    
    [BindProperty]
    [StringLength(128)]
    public string? Memo { get; set; }
    
    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync([FromRoute] string username)
    {
        if (!checkPermission(username))
            return Forbid();
        
        var user = await _userService.FindUser(username);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{username}'.");
        }

        Username = user.Username;
        Email = user.Email;
        Memo = user.Memo;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromRoute] string username)
    {
        if (!checkPermission(username))
            return Forbid();
        
        var user = await _userService.FindUser(username);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{username}'.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        user.Email = Email;
        user.Memo = Memo;
        
        await _userService.UpdateUser(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }
    
    private bool checkPermission(string username)
    {
        if (User.IsInRole(UserRoleNames.UserAdmin))
            return true;
        if (username == CookieAuthService.GetUsername(User.Claims))
            return true;
        
        return false;
    }
}