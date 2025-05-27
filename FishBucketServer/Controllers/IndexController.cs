using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class IndexController : ControllerBase
{
    [HttpGet("/")]
    public ActionResult OnGet()
    {
        return RedirectToPage("/Web/Home/Index");
    }
}
