using Microsoft.EntityFrameworkCore;
using OntarioParksExplorer.Api.Data;
using OntarioParksExplorer.Api.Models.DTOs;

namespace OntarioParksExplorer.Api.Services;

public class ParksService : IParksService
{
    private readonly ParksDbContext _context;

    public ParksService(ParksDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<ParkListDto>> GetParksAsync(int page, int pageSize)
    {
        var query = _context.Parks
            .AsNoTracking()
            .Include(p => p.ParkActivities)
                .ThenInclude(pa => pa.Activity)
            .Include(p => p.Images)
            .OrderByDescending(p => p.IsFeatured)
            .ThenBy(p => p.Name);

        var totalCount = await query.CountAsync();
        
        var parks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ParkListDto
            {
                Id = p.Id,
                Name = p.Name,
                Region = p.Region,
                IsFeatured = p.IsFeatured,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                MainImageUrl = p.Images.OrderBy(i => i.Id).Select(i => i.Url).FirstOrDefault(),
                ActivityNames = p.ParkActivities.Select(pa => pa.Activity.Name).OrderBy(n => n).ToList()
            })
            .ToListAsync();

        return new PagedResultDto<ParkListDto>
        {
            Items = parks,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ParkDetailDto?> GetParkByIdAsync(int id)
    {
        var park = await _context.Parks
            .AsNoTracking()
            .Include(p => p.ParkActivities)
                .ThenInclude(pa => pa.Activity)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (park == null)
            return null;

        return new ParkDetailDto
        {
            Id = park.Id,
            Name = park.Name,
            Description = park.Description,
            Location = park.Location,
            Latitude = park.Latitude,
            Longitude = park.Longitude,
            Website = park.Website,
            IsFeatured = park.IsFeatured,
            Region = park.Region,
            CreatedAt = park.CreatedAt,
            UpdatedAt = park.UpdatedAt,
            Activities = park.ParkActivities
                .Select(pa => new ActivityDto { Id = pa.Activity.Id, Name = pa.Activity.Name })
                .OrderBy(a => a.Name)
                .ToList(),
            Images = park.Images
                .Select(i => new ParkImageDto { Id = i.Id, Url = i.Url, AltText = i.AltText })
                .OrderBy(i => i.Id)
                .ToList()
        };
    }

    public async Task<PagedResultDto<ParkListDto>> SearchParksAsync(string query, int page, int pageSize)
    {
        var searchQuery = _context.Parks
            .AsNoTracking()
            .Include(p => p.ParkActivities)
                .ThenInclude(pa => pa.Activity)
            .Include(p => p.Images)
            .Where(p => EF.Functions.Like(p.Name, $"%{query}%") || 
                       EF.Functions.Like(p.Description, $"%{query}%"))
            .OrderByDescending(p => p.IsFeatured)
            .ThenBy(p => p.Name);

        var totalCount = await searchQuery.CountAsync();
        
        var parks = await searchQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ParkListDto
            {
                Id = p.Id,
                Name = p.Name,
                Region = p.Region,
                IsFeatured = p.IsFeatured,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                MainImageUrl = p.Images.OrderBy(i => i.Id).Select(i => i.Url).FirstOrDefault(),
                ActivityNames = p.ParkActivities.Select(pa => pa.Activity.Name).OrderBy(n => n).ToList()
            })
            .ToListAsync();

        return new PagedResultDto<ParkListDto>
        {
            Items = parks,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResultDto<ParkListDto>> FilterParksByActivitiesAsync(string[] activityNames, string mode, int page, int pageSize)
    {
        IQueryable<Data.Entities.Park> query;

        if (mode.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            // Parks must have ALL specified activities
            query = _context.Parks
                .AsNoTracking()
                .Include(p => p.ParkActivities)
                    .ThenInclude(pa => pa.Activity)
                .Include(p => p.Images)
                .Where(p => activityNames.All(activityName => 
                    p.ParkActivities.Any(pa => pa.Activity.Name == activityName)));
        }
        else
        {
            // Parks must have at least ONE of the specified activities (default "any" mode)
            query = _context.Parks
                .AsNoTracking()
                .Include(p => p.ParkActivities)
                    .ThenInclude(pa => pa.Activity)
                .Include(p => p.Images)
                .Where(p => p.ParkActivities.Any(pa => activityNames.Contains(pa.Activity.Name)));
        }

        query = query.OrderByDescending(p => p.IsFeatured)
                     .ThenBy(p => p.Name);

        var totalCount = await query.CountAsync();
        
        var parks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ParkListDto
            {
                Id = p.Id,
                Name = p.Name,
                Region = p.Region,
                IsFeatured = p.IsFeatured,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                MainImageUrl = p.Images.OrderBy(i => i.Id).Select(i => i.Url).FirstOrDefault(),
                ActivityNames = p.ParkActivities.Select(pa => pa.Activity.Name).OrderBy(n => n).ToList()
            })
            .ToListAsync();

        return new PagedResultDto<ParkListDto>
        {
            Items = parks,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<List<ActivityDto>> GetActivitiesAsync()
    {
        return await _context.Activities
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .Select(a => new ActivityDto
            {
                Id = a.Id,
                Name = a.Name
            })
            .ToListAsync();
    }
}
