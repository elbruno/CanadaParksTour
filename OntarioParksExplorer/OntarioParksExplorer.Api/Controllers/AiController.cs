using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OntarioParksExplorer.Api.Data;
using OntarioParksExplorer.Api.Models.DTOs.AI;
using OntarioParksExplorer.Api.Services.AI;

namespace OntarioParksExplorer.Api.Controllers;

/// <summary>
/// AI-powered features for park exploration and recommendations
/// </summary>
[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ParksDbContext _context;
    private readonly ILogger<AiController> _logger;

    public AiController(IAiService aiService, ParksDbContext context, ILogger<AiController> logger)
    {
        _aiService = aiService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Generate an AI-powered summary for a specific park
    /// </summary>
    /// <param name="id">Park ID</param>
    /// <returns>A concise, engaging summary of the park</returns>
    [HttpPost("parks/{id}/summary")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<string>> GenerateParkSummary(int id)
    {
        try
        {
            var park = await _context.Parks.FindAsync(id);
            
            if (park == null)
            {
                return NotFound($"Park with ID {id} not found");
            }

            var summary = await _aiService.GenerateParkSummaryAsync(park);
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating summary for park {ParkId}", id);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                "AI service is temporarily unavailable. Please try again later.");
        }
    }

    /// <summary>
    /// Get personalized park recommendations based on preferences
    /// </summary>
    /// <param name="request">Recommendation preferences including activities, region, and description</param>
    /// <returns>List of recommended parks with match scores and reasons</returns>
    [HttpPost("recommendations")]
    [ProducesResponseType(typeof(List<ParkRecommendation>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<List<ParkRecommendation>>> GetRecommendations(
        [FromBody] RecommendationRequest request)
    {
        if ((request.Activities == null || request.Activities.Count == 0) && 
            string.IsNullOrWhiteSpace(request.Region) && 
            string.IsNullOrWhiteSpace(request.PreferenceText))
        {
            return BadRequest("At least one preference field (Activities, Region, or PreferenceText) must be provided");
        }

        try
        {
            var recommendations = await _aiService.GetRecommendationsAsync(request);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recommendations");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                "AI service is temporarily unavailable. Please try again later.");
        }
    }

    /// <summary>
    /// Chat with an AI assistant about Ontario parks
    /// </summary>
    /// <param name="request">Chat message and optional conversation history</param>
    /// <returns>AI assistant's response</returns>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<string>> Chat([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest("Message cannot be empty");
        }

        try
        {
            var response = await _aiService.ChatAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                "AI service is temporarily unavailable. Please try again later.");
        }
    }

    /// <summary>
    /// Generate a personalized visit plan for a park
    /// </summary>
    /// <param name="request">Visit planning parameters including park, duration, interests, and season</param>
    /// <returns>Day-by-day visit plan with activities and tips</returns>
    [HttpPost("plan-visit")]
    [ProducesResponseType(typeof(VisitPlan), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<VisitPlan>> PlanVisit([FromBody] VisitPlanRequest request)
    {
        if (request.DurationDays < 1 || request.DurationDays > 14)
        {
            return BadRequest("DurationDays must be between 1 and 14");
        }

        try
        {
            var park = await _context.Parks
                .Include(p => p.ParkActivities)
                    .ThenInclude(pa => pa.Activity)
                .FirstOrDefaultAsync(p => p.Id == request.ParkId);
            
            if (park == null)
            {
                return NotFound($"Park with ID {request.ParkId} not found");
            }

            var plan = await _aiService.PlanVisitAsync(request);
            return Ok(plan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating visit plan for park {ParkId}", request.ParkId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                "AI service is temporarily unavailable. Please try again later.");
        }
    }
}
