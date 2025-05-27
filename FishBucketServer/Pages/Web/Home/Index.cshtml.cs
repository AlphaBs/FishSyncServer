using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlphabetUpdateServer.Pages.Web.Home;

public class IndexModel : PageModel
{
    public ActionResult OnGet()
    {
        return Page();
    }
}
