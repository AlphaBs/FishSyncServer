using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web;

[Authorize(Roles = UserRoleNames.BucketAdmin)]
public class Config : PageModel
{
    private readonly ConfigService _configService;

    public Config(ConfigService configService)
    {
        _configService = configService;
    }

    [BindProperty]
    public bool MaintenanceMode { get; set; }
    
    public async Task<ActionResult> OnGetAsync()
    {
        MaintenanceMode = await _configService.GetMaintenanceMode();
        return Page();
    }

    public async Task<ActionResult> OnPostAsync()
    {
        await _configService.SetMaintenanceMode(MaintenanceMode);
        return RedirectToPage();
    }
}