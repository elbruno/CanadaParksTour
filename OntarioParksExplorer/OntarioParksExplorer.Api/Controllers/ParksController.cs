using Microsoft.AspNetCore.Mvc;
using OntarioParksExplorer.Api.Models.DTOs;
using OntarioParksExplorer.Api.Services;

namespace OntarioParksExplorer.Api.Controllers;

/// <summary>
/// Parks management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ParksController : ControllerBase
{
    private readonly IParksService _parksService;

    public ParksController(IParksService parksService)
    {
        _parksService = parksService;
    }

    /// <summary>
    /// Get paginated list of parks
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 12)</param>
    /// <returns>Paginated list of parks ordered by featured status then name</returns>
    [HttpGet]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "page", "pageSize" })]
    [ProducesResponseType(typeof(PagedResultDto<ParkListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<ParkListDto>>> GetParks(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 12)
    {
        if (page < 1)
            return BadRequest("Page must be greater than 0");
        
        if (pageSize < 1 || pageSize > 100)
            return BadRequest("PageSize must be between 1 and 100");

        var result = await _parksService.GetParksAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get detailed information about a specific park
    /// </summary>
    /// <param name="id">Park ID</param>
    /// <returns>Park details including all activities and images</returns>
    [HttpGet("{id}")]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "id" })]
    [ProducesResponseType(typeof(ParkDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParkDetailDto>> GetParkById(int id)
    {
        var park = await _parksService.GetParkByIdAsync(id);
        
        if (park == null)
            return NotFound($"Park with ID {id} not found");

        return Ok(park);
    }

    /// <summary>
    /// Search parks by name or description
    /// </summary>
    /// <param name="q">Search query (searches park name and description)</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 12)</param>
    /// <returns>Paginated search results</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResultDto<ParkListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResultDto<ParkListDto>>> SearchParks(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search query 'q' is required");

        if (page < 1)
            return BadRequest("Page must be greater than 0");
        
        if (pageSize < 1 || pageSize > 100)
            return BadRequest("PageSize must be between 1 and 100");

        var result = await _parksService.SearchParksAsync(q, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Filter parks by activities
    /// </summary>
    /// <param name="activities">Comma-separated list of activity names</param>
    /// <param name="mode">Filter mode: 'any' (default) or 'all'</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 12)</param>
    /// <returns>Paginated list of parks matching activity criteria</returns>
    [HttpGet("filter")]
    [ProducesResponseType(typeof(PagedResultDto<ParkListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResultDto<ParkListDto>>> FilterByActivities(
        [FromQuery] string activities,
        [FromQuery] string mode = "any",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        if (string.IsNullOrWhiteSpace(activities))
            return BadRequest("Activities parameter is required");

        if (!mode.Equals("any", StringComparison.OrdinalIgnoreCase) && 
            !mode.Equals("all", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Mode must be 'any' or 'all'");

        if (page < 1)
            return BadRequest("Page must be greater than 0");
        
        if (pageSize < 1 || pageSize > 100)
            return BadRequest("PageSize must be between 1 and 100");

        var activityNames = activities.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(a => a.Trim())
                                     .Where(a => !string.IsNullOrWhiteSpace(a))
                                     .ToArray();

        if (activityNames.Length == 0)
            return BadRequest("At least one activity must be specified");

        var result = await _parksService.FilterParksByActivitiesAsync(activityNames, mode, page, pageSize);
        return Ok(result);
    }
}
