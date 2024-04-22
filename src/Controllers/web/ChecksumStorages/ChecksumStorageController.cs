using AlphabetUpdateServer.Areas.Identity.Data;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.ChecksumStorages;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("web/checksum-storages/common/{storageId}")]
[Authorize(Roles = UserRoleNames.StorageAdmin)]
public class ChecksumStorageController : Controller
{
    private readonly RFilesChecksumStorageService _storageService;

    public ChecksumStorageController(RFilesChecksumStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpGet]
    public async Task<ActionResult> Index(string storageId)
    {
        var storage = await _storageService.FindEntityById(storageId);
        if (storage == null)
        {
            return NotFound();
        }

        return View("/Views/ChecksumStorages/ChecksumStorage.cshtml", storage);
    }
}