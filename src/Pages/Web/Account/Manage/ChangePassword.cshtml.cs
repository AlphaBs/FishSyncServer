using System.ComponentModel.DataAnnotations;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Account.Manage;

[Authorize(Roles = UserRoleNames.BucketUser)]
public class ChangePasswordModel : PageModel
{
    private readonly UserService _userService;

    public ChangePasswordModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; } = null!;

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = null!;
    
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnPostAsync(string username)
    {
        if (!checkPermission(username))
            return Forbid();
        
        if (!ModelState.IsValid) return Page();

        var user = await _userService.FindUser(username);
        if (user == null) 
            return NotFound();

        await _userService.ChangePassword(user, NewPassword);
        StatusMessage = "Your password has been changed.";
        return Page();
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