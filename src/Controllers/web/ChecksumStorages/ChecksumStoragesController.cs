using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.ViewModels.ChecksumStorages;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Web.ChecksumStorages;

[Route("web/checksum-storages")]
public class ChecksumStoragesController : Controller
{
    private readonly RFilesChecksumStorageService _checksumStorageService;

    public ChecksumStoragesController(RFilesChecksumStorageService service)
    {
        _checksumStorageService = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetAsync()
    {
        var storages = await _checksumStorageService.GetAllStorages();
        return View("/Views/ChecksumStorages/Index.cshtml", new ChecksumStoragesViewModel
        {
            ChecksumStorages = storages.ToList()
        });
    }

    [HttpGet("add")]
    public ActionResult GetAddAsync()
    {
        return View("/Views/ChecksumStorages/Add.cshtml");
    }

    [HttpPost("add")]
    public async Task<ActionResult> PostAddAsync(AddChecksumStorageViewModel request)
    {
        if (string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.Host) || string.IsNullOrEmpty(request.ClientSecret))
            return BadRequest();

        await _checksumStorageService.AddStorage(new RFilesChecksumStorageEntity
        {
            Id = request.Id,
            IsReadonly = request.IsReadonly,
            Host = request.Host,
            ClientSecret = request.ClientSecret
        });
        return NoContent();
    }
}