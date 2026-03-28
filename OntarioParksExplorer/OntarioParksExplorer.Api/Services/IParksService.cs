using OntarioParksExplorer.Api.Models.DTOs;

namespace OntarioParksExplorer.Api.Services;

public interface IParksService
{
    Task<PagedResultDto<ParkListDto>> GetParksAsync(int page, int pageSize);
    Task<ParkDetailDto?> GetParkByIdAsync(int id);
    Task<PagedResultDto<ParkListDto>> SearchParksAsync(string query, int page, int pageSize);
    Task<PagedResultDto<ParkListDto>> FilterParksByActivitiesAsync(string[] activityNames, string mode, int page, int pageSize);
    Task<List<ActivityDto>> GetActivitiesAsync();
}
