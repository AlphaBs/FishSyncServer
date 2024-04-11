using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.ChecksumStorages;

[ApiController]
[Route("api/checksum-storages")]
public class ChecksumStorageController : ControllerBase
{
    private readonly RFilesChecksumStorageService _storageService;

    public ChecksumStorageController(RFilesChecksumStorageService service)
    {
        _storageService = service;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var storages = await _storageService.GetAllStorages();
        return Ok(new
        {
            Storages = storages
        });
    }

    [HttpGet("common/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetStorage(string id)
    {
        var storage = await _storageService.FindEntityById(id);
        if (storage == null)
            return NotFound();

        return Ok(new
        {
            storage.Id,
            storage.IsReadonly,
            storage.Host
        });
    }
}