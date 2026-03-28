using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using OntarioParksExplorer.Api.Data;
using OntarioParksExplorer.Api.Data.Entities;
using OntarioParksExplorer.Api.Models.DTOs.AI;
using OntarioParksExplorer.Api.Services.AI;

namespace OntarioParksExplorer.Api.Tests.Services.AI;

public class AiServiceTests : IDisposable
{
    private readonly ParksDbContext _context;
    private readonly Mock<ILogger<AiService>> _mockLogger;
    private readonly IMemoryCache _cache;

    public AiServiceTests()
    {
        var options = new DbContextOptionsBuilder<ParksDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ParksDbContext(options);
        _mockLogger = new Mock<ILogger<AiService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());

        SeedTestData();
    }

    private void SeedTestData()
    {
        var activities = new[]
        {
            new Activity { Id = 1, Name = "Hiking" },
            new Activity { Id = 2, Name = "Camping" },
            new Activity { Id = 3, Name = "Swimming" }
        };

        _context.Activities.AddRange(activities);

        var park = new Park
        {
            Id = 1,
            Name = "Algonquin Provincial Park",
            Description = "Ontario's most famous park with vast wilderness.",
            Location = "Whitney, Ontario",
            Latitude = 45.5543,
            Longitude = -78.2604,
            Website = "https://www.ontarioparks.com/park/algonquin",
            IsFeatured = true,
            Region = "Algonquin Region",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Parks.Add(park);

        var parkActivities = new[]
        {
            new ParkActivity { ParkId = 1, ActivityId = 1 },
            new ParkActivity { ParkId = 1, ActivityId = 2 }
        };

        _context.ParkActivities.AddRange(parkActivities);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GenerateParkSummaryAsync_ReturnsFallback_WhenNotConfigured()
    {
        // Arrange
        var service = new AiService(null, _context, _cache, _mockLogger.Object);
        var park = await _context.Parks.FirstAsync();

        // Act
        var result = await service.GenerateParkSummaryAsync(park);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("AI features are not configured", result);
    }

    [Fact]
    public async Task GetRecommendationsAsync_ReturnsFallback_WhenNotConfigured()
    {
        // Arrange
        var service = new AiService(null, _context, _cache, _mockLogger.Object);
        var request = new RecommendationRequest
        {
            Activities = new List<string> { "Hiking" }
        };

        // Act
        var result = await service.GetRecommendationsAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("AI Not Configured", result[0].ParkName);
        Assert.Contains("AI features are not configured", result[0].Reason);
    }

    [Fact]
    public async Task ChatAsync_ReturnsFallback_WhenNotConfigured()
    {
        // Arrange
        var service = new AiService(null, _context, _cache, _mockLogger.Object);
        var request = new ChatRequest { Message = "Hello" };

        // Act
        var result = await service.ChatAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("AI features are not configured", result);
    }

    [Fact]
    public async Task ChatAsync_HandlesEmptyConversationHistory()
    {
        // Arrange
        var service = new AiService(null, _context, _cache, _mockLogger.Object);
        var request = new ChatRequest
        {
            Message = "Hello",
            ConversationHistory = null
        };

        // Act
        var result = await service.ChatAsync(request);

        // Assert - Should still return fallback message when not configured
        Assert.NotNull(result);
        Assert.Contains("AI features are not configured", result);
    }

    [Fact]
    public async Task PlanVisitAsync_ThrowsException_WhenParkNotFound()
    {
        // Arrange
        var service = new AiService(null, _context, _cache, _mockLogger.Object);
        var request = new VisitPlanRequest
        {
            ParkId = 999,
            DurationDays = 2
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.PlanVisitAsync(request));
    }

    [Fact]
    public async Task PlanVisitAsync_ReturnsFallback_WhenNotConfigured()
    {
        // Arrange
        var service = new AiService(null, _context, _cache, _mockLogger.Object);
        var request = new VisitPlanRequest
        {
            ParkId = 1,
            DurationDays = 2
        };

        // Act
        var result = await service.PlanVisitAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Algonquin Provincial Park", result.ParkName);
        Assert.Equal(2, result.DurationDays);
        Assert.Empty(result.Days);
        Assert.Single(result.Tips);
        Assert.Contains("AI features are not configured", result.Tips[0]);
    }

    [Fact]
    public async Task PlanVisitAsync_ValidatesDurationDays()
    {
        // Arrange
        var service = new AiService(null, _context, _cache, _mockLogger.Object);
        var request = new VisitPlanRequest
        {
            ParkId = 1,
            DurationDays = 3,
            Interests = new List<string> { "Hiking", "Camping" }
        };

        // Act
        var result = await service.PlanVisitAsync(request);

        // Assert - Should set the correct duration
        Assert.NotNull(result);
        Assert.Equal(3, result.DurationDays);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
