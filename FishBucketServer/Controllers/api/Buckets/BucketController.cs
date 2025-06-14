using System.Security.Claims;
using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Services;
using AlphabetUpdateServer.Services.Buckets;
using AlphabetUpdateServer.Services.Users;
using FishBucket;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ILogger<BucketController> _logger;
    private readonly BucketService _bucketService;
    private readonly BucketFilesCacheService _cachedBucketService;
    private readonly BucketOwnerService _bucketOwnerService;

    public BucketController(
        ILogger<BucketController> logger,
        BucketService bucketService,
        BucketOwnerService bucketOwnerService,
        BucketFilesCacheService cachedBucketService)
    {
        _logger = logger;
        _bucketService = bucketService;
        _bucketOwnerService = bucketOwnerService;
        _cachedBucketService = cachedBucketService;
    }

    /// <summary>
    /// 버킷 목록 가져오기
    /// </summary>
    [HttpGet]
    [ProducesResponseType<BucketListResponse>(StatusCodes.Status200OK)]
    [Authorize(AuthenticationSchemes = JwtAuthService.SchemeName, Roles = UserRoleNames.BucketAdmin)]
    public ActionResult Index()
    {
        var buckets = _bucketService.GetAllBuckets();
        return Ok(new BucketListResponse
        {
            Buckets = buckets.Select(bucket => bucket.Id)
        });
    }

    /// <summary>
    /// 버킷을 찾고 내용 전체를 반환
    /// </summary>
    /// <param name="id">찾을 버킷의 id</param>
    /// <returns>찾은 버킷</returns>
    /// <response code="200">성공</response>
    /// <response code="404">찾을 수 없는 버킷</response>
    [HttpGet("common/{id}")]
    [ProducesResponseType<BucketMetadata>(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetBucket(string id)
    {
        var bucket = await _bucketService.FindBucketMetadata(id);
        if (bucket == null)
        {
            return NotFound();
        }

        return Ok(bucket);
    }

    /// <summary>
    /// 버킷을 찾고 파일 목록을 반환
    /// </summary>
    /// <param name="id">찾을 버킷의 id</param>
    /// <returns>파일 목록</returns>
    /// <response code="200">성공</response>
    /// <response code="404">찾을 수 없는 버킷</response>
    [HttpGet("common/{id}/files")]
    [ProducesResponseType<BucketFiles>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetFiles([FromRoute] string id)
    {
        try
        {
            var files = await _cachedBucketService.GetOrCreate(id, CancellationToken.None);
            return Ok(files);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (FormatException)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// 버킷을 찾고 Limitations 반환
    /// </summary>
    /// <param name="id">찾을 버킷의 id</param>
    /// <returns>Limitations</returns>
    /// <response code="200">성공</response>
    /// <response code="404">찾을 수 없는 버킷</response>
    [HttpGet("common/{id}/limitations")]
    [ProducesResponseType<BucketLimitations>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetLimitations(string id)
    {
        var bucket = await _bucketService.Find(id);
        if (bucket == null)
        {
            return NotFound();
        }

        return Ok(bucket.Limitations);
    }

    /// <summary>
    /// 버킷을 찾고 push 동기화
    /// </summary>
    /// <param name="id">찾을 버킷의 id</param>
    /// <param name="files">동기화 내용</param>
    /// <returns>동기화 결과</returns>
    /// <response code="200">성공</response>
    /// <response code="400">유효하지 않은 요청</response>
    /// <response code="403">동기화 거부</response>
    /// <response code="404">찾을 수 없는 버킷</response>
    /// <response code="503">서버 점검 중</response>
    [HttpPost("common/{id}/sync")]
    [ProducesResponseType<BucketSyncResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status503ServiceUnavailable)]
    [Authorize(AuthenticationSchemes = JwtAuthService.SchemeName, Roles = UserRoleNames.BucketUser)]
    public async Task<ActionResult> PostSync(string id, BucketSyncRequest files)
    {
        if (files.Files == null)
        {
            return BadRequest();
        }

        var userId = getUserId();

        try
        {
            var hasPermission = await checkSyncPermission(id, userId);
            if (!hasPermission)
            {
                _logger.LogWarning("Unauthorized user {UserId} tried to sync a bucket {BucketId}", userId, id);
                return Forbid();
            }

            var result = await _bucketService.SyncAndLog(id, userId, files.Files);
            _logger.LogInformation("User {UserId} requested a bucket sync for {BucketId}, Result: {Result}", userId, id,
                result.IsSuccess);
            _cachedBucketService.Remove(id);
            return Ok(result);
        }
        catch (BucketLimitationException ex)
        {
            _logger.LogError(
                "User {UserId}'s sync request for bucket {BucketId} was rejected due to a BucketLimitationException. Reason: {Reason}",
                userId, id, ex.Reason);
            var detail = ex.Reason switch
            {
                BucketLimitationException.ReadonlyBucket => "읽기 전용 버킷입니다.",
                BucketLimitationException.ExpiredBucket => "사용 기간이 만료되었습니다.",
                BucketLimitationException.ExceedMaxBucketSize => "버킷 용량을 초과하였습니다.",
                BucketLimitationException.ExceedMaxNumberOfFiles => "업로드 가능한 최대 파일 수를 초과하였습니다.",
                BucketLimitationException.ExceedMonthlySyncCount => "이번 달에 가능한 동기화 횟수를 모두 소진하였습니다.",
                _ => "사용 불가능한 버킷입니다."
            };

            return Problem(
                type: "https://fish.alphabeta.pw/web/errors/bucket-limitations.html",
                title: "버킷 사용 제한",
                statusCode: 403,
                detail: detail);
        }
        catch (ServiceMaintenanceException)
        {
            _logger.LogError(
                "User {UserId}'s sync request for bucket {BucketId} was rejected. the server is currently in maintenance mode",
                userId, id);
            return Problem(
                title: "서버 점검중",
                statusCode: 503);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during the sync request for bucket {BucketId} by user {UserId}. {Ex}", id, userId, ex);
            return Problem(
                title: "오류",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    private async Task<bool> checkSyncPermission(string bucketId, string userId)
    {
        if (User.IsInRole(UserRoleNames.BucketAdmin))
            return true;

        // JWT 에서 sub 가 ClaimTypes.NameIdentifier 으로 바뀜
        return await _bucketOwnerService.CheckOwnershipByUsername(bucketId, userId);
    }

    private string getUserId()
    {
        return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
    }
}