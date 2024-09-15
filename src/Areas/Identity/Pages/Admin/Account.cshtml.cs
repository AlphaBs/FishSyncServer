using AlphabetUpdateServer.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Areas.Identity.Pages.Admin;

[BindProperties]
[Authorize(Roles = UserRoleNames.UserAdmin)]
public class AccountModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountModel(
        UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public User? Account { get; set; } = null!;
    public string? Action { get; set; } = null!;
    public string[] RoleNames { get; set; } = [];
    public bool[] RoleEnables { get; set; } = [];
    public string[] BucketIds { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return NotFound();
        }
        Account = user;

        var userRoles = await _userManager.GetRolesAsync(user);
        await updateRoles(userRoles);
        //await getBucketIds(username);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string username)
    {
        if (Action == "update")
        {
            return await update(username);
        }
        else if (Action == "delete")
        {
            return await delete(username);
        }
        else
        {
            return BadRequest();
        }
    }

    private async Task<IActionResult> update(string username)
    {
        if (Account == null || RoleEnables == null || RoleNames == null)
        {
            return BadRequest();
        }

        if (RoleEnables.Length != RoleNames.Length)
        {
            return BadRequest();
        }

        var updatedUser = await _userManager.FindByNameAsync(username);
        if (updatedUser == null)
        {
            return BadRequest();
        }
        updatedUser.Email = Account.Email;
        updatedUser.Discord = Account.Discord;
        await _userManager.UpdateAsync(updatedUser);

        var userRoles = await _userManager.GetRolesAsync(updatedUser);
        var userRoleSet = new HashSet<string>(userRoles);

        IEnumerable<string> getChangedRole(bool changedTo)
        {
            for (int i = 0; i < RoleEnables.Length; i++)
            {
                if (RoleEnables[i] == changedTo)
                {
                    var isEnabledBefore = userRoleSet.Contains(RoleNames[i]);
                    if (RoleEnables[i] && !isEnabledBefore)
                        yield return RoleNames[i];
                    else if (!RoleEnables[i] && isEnabledBefore)
                        yield return RoleNames[i];
                }
            }
        }

        await _userManager.AddToRolesAsync(updatedUser, getChangedRole(true));
        await _userManager.RemoveFromRolesAsync(updatedUser, getChangedRole(false));
            
        await updateRoles(userRoles);
        return Page();
    }

    private async Task<IActionResult> delete(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return NotFound();
        }

        await _userManager.DeleteAsync(user);
        return RedirectToPage("Admin", "Sucessfully deleted the user: " + username);
    }

    private async Task updateRoles(IEnumerable<string> userRoles)
    {
        var roleList = await _roleManager.Roles.ToListAsync();
        var roleDict= roleList.ToDictionary(r => r.Name!, _ => false);
        foreach (var role in userRoles)
        {
            roleDict[role] = true;
        }
        var roleKVs = roleDict.ToArray();
        RoleNames = roleKVs.Select(kv => kv.Key).ToArray();
        RoleEnables = roleKVs.Select(kv => kv.Value).ToArray();
    }
}