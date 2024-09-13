using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    [Route("/api/error")]
    public IActionResult HandleError()
    {
        return Problem();
    }
}