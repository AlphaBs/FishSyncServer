using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.Buckets;

[Route("api/buckets/{bucket}/storages")]
public class BucketStorageController : Controller
{
    private ChecksumStorageService _checksumStorageService;

    public BucketStorageController(ChecksumStorageService checksumStorageService)
    {
        _checksumStorageService = checksumStorageService;
    }

    [HttpGet]
    public async Task<ActionResult> Index(string bucket)
    {
        var entities = await _checksumStorageService.GetAllEntities(bucket);
        return Ok(new 
        {
            Id = bucket,
            Storages = entities
        });
    }

    [HttpGet("{storage}")]
    public async Task<ActionResult> GetStorage(string bucket, string storage)
    {
        var entity = await _checksumStorageService.GetEntityById(bucket, storage);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    [HttpPut("{storage}")]
    public async Task<ActionResult> PutStorage(string bucket, string storage, FileChecksumStorageEntity entity)
    {
        if (entity.BucketId != bucket)
        {
            return BadRequest();
        }
        if (entity.StorageId != storage)
        {
            return BadRequest();
        }

        // System.Text.Json 의 요구사항에 따라 json 에서 $type 속성이 discriminator 
        // 역할을 하게 됨. 하지만 deserialize 된 이후 $type 값에 접근하는 방법은 제공하지 않음
        // 따라서 deserialize 된 이후 객체가 직접 자신의 $type 값을 알려주는 메서드를 만듦
        entity.Type = entity.GetEntityType();

        await _checksumStorageService.AddStorage(entity);
        // TODO: return Conflict();
        return Created();
    }

    [HttpDelete("{storage}")]
    public async Task<ActionResult> DeleteStorage(string bucket, string storage)
    {
        await _checksumStorageService.RemoveStorage(bucket, storage);
        // TODO: return NotFound();
        return NoContent();
    }
}