using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.ChecksumStorages;

[Route("web/checksum-storages/{storageId}")]
public class ChecksumStorageController : Controller
{
    private readonly RFilesChecksumStorageService _storageService;

    public ChecksumStorageController(RFilesChecksumStorageService storageService)
    {
        _storageService = storageService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAsync(string storageId)
    {
        var storage = await _storageService.FindEntityById(storageId);
        if (storage == null)
        {
            return NotFound();
        }

        return View("/Views/ChecksumStorages/ChecksumStorage.cshtml", storage);
    }
}