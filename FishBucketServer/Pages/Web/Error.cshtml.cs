using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace AlphabetUpdateServer.Pages.Web;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    [BindProperty]
    public string? RequestId { get; set; }

    [BindProperty]
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public IActionResult OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return Page();
    }
}
