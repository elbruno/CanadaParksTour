using Microsoft.AspNetCore.Mvc;
using OntarioParksExplorer.Api.Models.DTOs;
using OntarioParksExplorer.Api.Services;

namespace OntarioParksExplorer.Api.Controllers;

/// <summary>
/// Activities management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly IParksService _parksService;

    public ActivitiesController(IParksService parksService)
    {
        _parksService = parksService;
    }

    /// <summary>
    /// Get all available activities
    /// </summary>
    /// <returns>Alphabetically sorted list of all activities</returns>
    [HttpGet]
    [ResponseCache(Duration = 60)]
    [ProducesResponseType(typeof(List<ActivityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ActivityDto>>> GetActivities()
    {
        var activities = await _parksService.GetActivitiesAsync();
        return Ok(activities);
    }
}
