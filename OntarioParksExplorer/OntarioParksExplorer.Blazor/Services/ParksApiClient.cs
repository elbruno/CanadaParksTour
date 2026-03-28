using System.Net.Http.Json;
using OntarioParksExplorer.Blazor.Models;

namespace OntarioParksExplorer.Blazor.Services;

public class ParksApiClient
{
    private readonly HttpClient _httpClient;

    public ParksApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResultDto<ParkListDto>> GetParksAsync(int page = 1, int pageSize = 12)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<PagedResultDto<ParkListDto>>(
                $"/api/parks?page={page}&pageSize={pageSize}");
            return result ?? new PagedResultDto<ParkListDto>();
        }
        catch (Exception)
        {
            return new PagedResultDto<ParkListDto>();
        }
    }

    public async Task<ParkDetailDto?> GetParkByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ParkDetailDto>($"/api/parks/{id}");
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<PagedResultDto<ParkListDto>> SearchParksAsync(string query, int page = 1, int pageSize = 12)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<PagedResultDto<ParkListDto>>(
                $"/api/parks/search?q={Uri.EscapeDataString(query)}&page={page}&pageSize={pageSize}");
            return result ?? new PagedResultDto<ParkListDto>();
        }
        catch (Exception)
        {
            return new PagedResultDto<ParkListDto>();
        }
    }

    public async Task<PagedResultDto<ParkListDto>> FilterParksAsync(List<string> activities, string mode = "any", int page = 1, int pageSize = 12)
    {
        try
        {
            var activitiesParam = string.Join(",", activities);
            var result = await _httpClient.GetFromJsonAsync<PagedResultDto<ParkListDto>>(
                $"/api/parks/filter?activities={Uri.EscapeDataString(activitiesParam)}&mode={mode}&page={page}&pageSize={pageSize}");
            return result ?? new PagedResultDto<ParkListDto>();
        }
        catch (Exception)
        {
            return new PagedResultDto<ParkListDto>();
        }
    }

    public async Task<List<ActivityDto>> GetActivitiesAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<ActivityDto>>("/api/activities");
            return result ?? new List<ActivityDto>();
        }
        catch (Exception)
        {
            return new List<ActivityDto>();
        }
    }

    // AI Methods
    public async Task<string?> GenerateSummaryAsync(int parkId)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/ai/parks/{parkId}/summary", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<AiRecommendationDto>> GetRecommendationsAsync(AiRecommendationRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/ai/recommendations", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<AiRecommendationDto>>();
            return result ?? new List<AiRecommendationDto>();
        }
        catch (Exception)
        {
            return new List<AiRecommendationDto>();
        }
    }

    public async Task<string?> ChatAsync(string message, List<ChatMessageDto> conversationHistory)
    {
        try
        {
            var request = new AiChatRequestDto 
            { 
                Message = message, 
                ConversationHistory = conversationHistory 
            };
            var response = await _httpClient.PostAsJsonAsync("/api/ai/chat", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<VisitPlanDto?> PlanVisitAsync(AiVisitPlanRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/ai/plan-visit", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<VisitPlanDto>();
        }
        catch (Exception)
        {
            return null;
        }
    }
}
