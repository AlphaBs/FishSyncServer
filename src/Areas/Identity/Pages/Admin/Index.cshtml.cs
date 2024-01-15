using AlphabetUpdateServer.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AlphabetUpdateServer.Areas.Identity.Pages.Admin;

public class IndexModel : PageModel
{
    ApplicationDbContext _dbContext;

    public IndexModel(ApplicationDbContext db)
    {
        _dbContext = db;
    }

    public IEnumerable<UserWithRoles> Users { get; private set; }
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var users = from user in _dbContext.Users
                    join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId into g1
                    from g in g1.DefaultIfEmpty()
                    join role in _dbContext.Roles on g.RoleId equals role.Id into userRoleGroup
                    from userRole in userRoleGroup.DefaultIfEmpty()
                    select new { User = user, Role = userRole };

        var usersWithRoles = from user in users
                             group user by user.User.Id into grouped
                             select new UserWithRoles(grouped.First().User, grouped.Select(u => u.Role));

        Users = await usersWithRoles.ToListAsync();
        return Page();
    }
}

public class UserWithRoles
{
    public UserWithRoles(IdentityUser user, IEnumerable<IdentityRole> roles)
    {
        User = user;
        Roles = roles;
    }

    public IdentityUser User { get; }
    public IEnumerable<IdentityRole> Roles { get; }
}