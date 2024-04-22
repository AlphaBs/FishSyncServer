using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("/")]
public class IndexController : Controller
{
    public ActionResult Index()
    {
        return RedirectToActionPermanent("Index", "Home");
    }
}