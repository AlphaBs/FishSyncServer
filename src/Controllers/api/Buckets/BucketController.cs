using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Models.Buckets;
using AlphabetUpdateServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.Api.Buckets;

/// <summary>
/// common bucket
/// </summary>
[ApiController]
[Route("api/buckets")]
[Produces("application/json")]
public class BucketController : ControllerBase
{
    private readonly ChecksumStorageBucketService _bucketService;

    public BucketController(ChecksumStorageBucketService bucketService)
    {
        _bucketService = bucketService;
    }

    /// <summary>
    /// ��Ŷ ��� ��������
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var buckets = await _bucketService.GetAllBuckets();
        return Ok(new 
        {
            Buckets = buckets.Select(bucket => bucket.Id)
        });
    }

    /// <summary>
    /// ��Ŷ�� ã�� ���� ��ü�� ��ȯ
    /// </summary>
    /// <param name="id">ã�� ��Ŷ�� id</param>
    /// <returns>ã�� ��Ŷ</returns>
    /// <response code="200">����</response>
    /// <response code="404">ã�� �� ���� ��Ŷ</response>
    [HttpGet("common/{id}")]
    [ProducesResponseType<BucketDTO>(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetBucket(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        var files = await bucket.GetFiles();
        return Ok(new BucketDTO()
        {
            Id = id,
            Limitations = bucket.Limitations,
            LastUpdated = bucket.LastUpdated,
            Files = files.ToArray()
        });
    }

    /// <summary>
    /// ��Ŷ�� ã�� ���� ����� ��ȯ
    /// </summary>
    /// <param name="id">ã�� ��Ŷ�� id</param>
    /// <returns>���� ���</returns>
    /// <response code="200">����</response>
    /// <response code="404">ã�� �� ���� ��Ŷ</response>
    [HttpGet("common/{id}/files")]
    [ProducesResponseType<BucketFilesDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetFiles(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        var files = await bucket.GetFiles();
        return Ok(new BucketFilesDTO
        {
            Id = id,
            LastUpdated = bucket.LastUpdated,
            Files = files.ToArray()
        });
    }

    /// <summary>
    /// ��Ŷ�� ã�� Limitations ��ȯ
    /// </summary>
    /// <param name="id">ã�� ��Ŷ�� id</param>
    /// <returns>Limitations</returns>
    /// <response code="200">����</response>
    /// <response code="404">ã�� �� ���� ��Ŷ</response>
    [HttpGet("common/{id}/limitations")]
    [ProducesResponseType<BucketLimitations>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetLimitations(string id)
    {
        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        return Ok(bucket.Limitations);
    }

    /// <summary>
    /// ��Ŷ�� ã�� push ����ȭ
    /// </summary>
    /// <param name="id">ã�� ��Ŷ�� id</param>
    /// <param name="files">����ȭ ����</param>
    /// <returns>����ȭ ���</returns>
    /// <response code="200">����</response>
    /// <response code="400">��ȿ���� ���� ��û</response>
    /// <response code="404">ã�� �� ���� ��Ŷ</response>
    [HttpPost("common/{id}/sync")]
    [ProducesResponseType<BucketSyncResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PostSync(string id, BucketSyncRequestDTO files)
    {
        if (files?.Files == null)
        {
            return BadRequest();
        }

        var bucket = await _bucketService.FindBucketById(id);
        if (bucket == null)
        {
            return NotFound();
        }

        var result = await bucket.Sync(files.Files);
        if (result.IsSuccess)
        {
            await _bucketService.UpdateBucket(id, bucket);
            return Ok(result.UpdatedAt);
        }
        else
        {
            return Ok(result.RequiredActions);
        }
    }
}