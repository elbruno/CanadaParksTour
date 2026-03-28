using System.Net;
using System.Net.Http.Json;
using OntarioParksExplorer.Api.Models.DTOs;

namespace OntarioParksExplorer.Api.Tests.IntegrationTests;

public class ParksApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ParksApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetParks_Returns200_WithValidJsonAndPaginationStructure()
    {
        // Act
        var response = await _client.GetAsync("/api/parks");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ParkListDto>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.True(result.TotalCount > 0);
        Assert.True(result.Page >= 1);
        Assert.True(result.PageSize > 0);
        Assert.True(result.TotalPages > 0);
    }

    [Fact]
    public async Task GetParks_WithPagination_ReturnsCorrectPageSize()
    {
        // Act
        var response = await _client.GetAsync("/api/parks?page=1&pageSize=5");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ParkListDto>>();
        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.True(result.Items.Count <= 5);
    }

    [Fact]
    public async Task GetParkById_WithValidId_Returns200WithParkDetails()
    {
        // Arrange - get a valid park ID first
        var parksResponse = await _client.GetAsync("/api/parks?page=1&pageSize=1");
        var parks = await parksResponse.Content.ReadFromJsonAsync<PagedResultDto<ParkListDto>>();
        Assert.NotNull(parks);
        Assert.NotEmpty(parks.Items);
        var parkId = parks.Items[0].Id;
        
        // Act
        var response = await _client.GetAsync($"/api/parks/{parkId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var park = await response.Content.ReadFromJsonAsync<ParkDetailDto>();
        Assert.NotNull(park);
        Assert.Equal(parkId, park.Id);
        Assert.NotNull(park.Name);
        Assert.NotNull(park.Description);
        Assert.NotNull(park.Activities);
        Assert.NotNull(park.Images);
    }

    [Fact]
    public async Task GetParkById_WithInvalidId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/parks/99999");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SearchParks_WithQuery_ReturnsMatchingResults()
    {
        // Act
        var response = await _client.GetAsync("/api/parks/search?q=algonquin");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ParkListDto>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        
        // Should find Algonquin Park
        if (result.Items.Count > 0)
        {
            Assert.Contains(result.Items, p => 
                p.Name.Contains("Algonquin", StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public async Task SearchParks_WithNonexistentQuery_ReturnsEmptyResults()
    {
        // Act
        var response = await _client.GetAsync("/api/parks/search?q=nonexistentparkxyz123");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ParkListDto>>();
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task FilterByActivities_WithHiking_ReturnsParksWithHiking()
    {
        // Act
        var response = await _client.GetAsync("/api/parks/filter?activities=Hiking");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ParkListDto>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        
        // All returned parks should have Hiking in their activity names
        foreach (var park in result.Items)
        {
            Assert.Contains("Hiking", park.ActivityNames);
        }
    }

    [Fact]
    public async Task FilterByActivities_WithMultipleActivitiesAllMode_ReturnsParksWithBoth()
    {
        // Act
        var response = await _client.GetAsync("/api/parks/filter?activities=Hiking,Camping&mode=all");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ParkListDto>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        
        // All returned parks should have both Hiking AND Camping
        foreach (var park in result.Items)
        {
            Assert.Contains("Hiking", park.ActivityNames);
            Assert.Contains("Camping", park.ActivityNames);
        }
    }

    [Fact]
    public async Task GetActivities_ReturnsListOfActivities()
    {
        // Act
        var response = await _client.GetAsync("/api/activities");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var activities = await response.Content.ReadFromJsonAsync<List<ActivityDto>>();
        Assert.NotNull(activities);
        Assert.NotEmpty(activities);
        
        // Check that activities are alphabetically sorted
        var sortedActivities = activities.OrderBy(a => a.Name).ToList();
        Assert.Equal(sortedActivities.Select(a => a.Name), activities.Select(a => a.Name));
    }

    [Fact]
    public async Task ApiEndpoints_ReturnProperContentType()
    {
        // Test multiple endpoints return application/json
        var endpoints = new[]
        {
            "/api/parks",
            "/api/activities"
        };
        
        foreach (var endpoint in endpoints)
        {
            // Act
            var response = await _client.GetAsync(endpoint);
            
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content.Headers.ContentType);
            Assert.StartsWith("application/json", response.Content.Headers.ContentType.ToString());
        }
    }
}
