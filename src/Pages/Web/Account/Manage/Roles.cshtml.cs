using System.ComponentModel.DataAnnotations;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Account.Manage;

[Authorize(Roles = UserRoleNames.UserAdmin)]
public class RolesModel : PageModel
{
    private readonly UserService _userService;
    
    public RolesModel(UserService userService)
    {
        _userService = userService;
    }

    public string? StatusMessage { get; set; }
    
    public string[] RoleNames { get; set; } = 
    [
        UserRoleNames.BucketUser,
        UserRoleNames.BucketAdmin,
        UserRoleNames.StorageAdmin,
        UserRoleNames.UserAdmin
    ];
    
    [BindProperty]
    public IList<bool> RoleEnables { get; set; } = [];
    
    public async Task<IActionResult> OnGet([FromRoute] string username)
    {
        var user = await _userService.FindUser(username);
        if (user == null)
            return NotFound();

        var roles = new bool[RoleNames.Length];
        for (int i = 0; i < RoleNames.Length; i++)
        {
            roles[i] = user.Roles.Contains(RoleNames[i]);
        }

        RoleEnables = roles;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromRoute] string username)
    {   
        var user = await _userService.FindUser(username);
        if (user == null)
            return NotFound();

        var roles = new List<string>();
        for (int i = 0; i < RoleEnables.Count; i++)
        {
            if (RoleEnables[i])
                roles.Add(RoleNames[i]);
        }

        await _userService.UpdateRoles(user, roles);
        return RedirectToPage();
    }
}