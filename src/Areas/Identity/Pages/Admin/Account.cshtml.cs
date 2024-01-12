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

        public User Account { get; set; } = null!;
        public string[] RoleNames { get; set; } = null!;
        public bool[] RoleEnables { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            Account = user;

            var roleList = await _roleManager.Roles.ToListAsync();
            var roleDict= roleList.ToDictionary(r => r.Name!, _ => false);
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                roleDict[role] = true;
            }
            var roleKVs = roleDict.ToArray();
            RoleNames = roleKVs.Select(kv => kv.Key).ToArray();
            RoleEnables = roleKVs.Select(kv => kv.Value).ToArray();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string username)
        {
            if (RoleNames.Length != RoleEnables.Length)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            user.Email = Account.Email;
            user.Discord = Account.Discord;
            await _userManager.UpdateAsync(user);

            var roleResolver = 
                (bool isEnabled) => RoleEnables
                    .Where(r => r == isEnabled)
                    .Select((_, i) => i)
                    .Select(i => RoleNames[i]);

            var enabledRoles = roleResolver(true);
            var disabledRoles = roleResolver(false);
            var r1 = await _userManager.AddToRolesAsync(user, enabledRoles);
            var r2 = await _userManager.RemoveFromRolesAsync(user, disabledRoles);
            
            return await OnGetAsync(username);
        }
    }
}
