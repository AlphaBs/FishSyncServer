using AlphabetUpdateServer.Services.ChecksumStorages;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.ChecksumStorages;

[ApiController]
[Route("api/checksum-storages")]
[Authorize(AuthenticationSchemes = JwtAuthService.SchemeName, Roles = UserRoleNames.StorageAdmin)]
public class ChecksumStorageController : ControllerBase
{
    private readonly ChecksumStorageService _storageService;

    public ChecksumStorageController(ChecksumStorageService service)
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
}