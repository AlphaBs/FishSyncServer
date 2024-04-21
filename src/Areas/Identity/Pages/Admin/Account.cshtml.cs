using AlphabetUpdateServer.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Areas.Identity.Pages.Admin
{
    [BindProperties]
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
        public string[]? RoleNames { get; set; } = null!;
        public bool[]? RoleEnables { get; set; } = null!;
        public string[]? BucketIds { get; set; } = null!;

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
            var roleResolver = 
                (bool isEnabled) => RoleEnables
                    .Where(r => r == isEnabled)
                    .Select((_, i) => i)
                    .Select(i => RoleNames[i])
                    .Where(r => userRoleSet.Contains(r) != isEnabled); // set에 없는것만 enable, set에 있는것만 disable

            var enabledRoles = roleResolver(true);
            var disabledRoles = roleResolver(false);
            await _userManager.AddToRolesAsync(updatedUser, enabledRoles);
            await _userManager.RemoveFromRolesAsync(updatedUser, disabledRoles);
            
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
}
