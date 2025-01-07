using AlphabetUpdateServer.DTOs;
using AlphabetUpdateServer.Services.Buckets;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Controllers.api.Buckets;

[ApiController]
[Route("api/bucket-indexes")]
[Produces("application/json")]
public class BucketIndexController : ControllerBase
{
    private readonly BucketIndexService _bucketIndexService;

    public BucketIndexController(BucketIndexService bucketIndexService)
    {
        _bucketIndexService = bucketIndexService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType<BucketIndexMetadataDTO>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetBucketIndex(string id)
    {
        var index = await _bucketIndexService.FindIndex(id);
        if (index == null)
            return NotFound();
        
        return Ok(new BucketIndexMetadataDTO
        {
            Id = index.Id,
            Description = index.Description,
            Searchable = index.Searchable
        });
    }

    [HttpGet("{id}/buckets")]
    [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult GetBuckets(string id)
    {
        try
        {
            var buckets = _bucketIndexService.GetBucketsFromIndex(id);
            return Ok(buckets);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}