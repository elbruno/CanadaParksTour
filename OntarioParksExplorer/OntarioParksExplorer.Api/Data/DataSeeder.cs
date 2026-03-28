using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OntarioParksExplorer.Api.Data.Entities;

namespace OntarioParksExplorer.Api.Data;

public class DataSeeder
{
    public static async Task SeedAsync(ParksDbContext context, string seedDataPath)
    {
        // Check if data already exists
        if (await context.Parks.AnyAsync())
        {
            return; // Database already seeded
        }

        // Read and parse JSON file
        var jsonData = await File.ReadAllTextAsync(seedDataPath);
        var parksData = JsonSerializer.Deserialize<List<ParkSeedData>>(jsonData, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (parksData == null || parksData.Count == 0)
        {
            throw new InvalidOperationException("No park data found in seed file");
        }

        // Collect all unique activities
        var allActivityNames = parksData
            .SelectMany(p => p.Activities ?? new List<string>())
            .Distinct()
            .OrderBy(a => a)
            .ToList();

        // Create Activity entities
        var activityEntities = new Dictionary<string, Activity>();
        foreach (var activityName in allActivityNames)
        {
            var activity = new Activity { Name = activityName };
            activityEntities[activityName] = activity;
            context.Activities.Add(activity);
        }

        await context.SaveChangesAsync();

        // Create Park entities with relationships
        foreach (var parkData in parksData)
        {
            var park = new Park
            {
                Name = parkData.Name,
                Description = parkData.Description,
                Location = parkData.Location,
                Latitude = parkData.Latitude,
                Longitude = parkData.Longitude,
                Website = parkData.Website,
                IsFeatured = parkData.IsFeatured,
                Region = parkData.Region,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Parks.Add(park);
            await context.SaveChangesAsync(); // Save to get park ID

            // Add park activities
            if (parkData.Activities != null)
            {
                foreach (var activityName in parkData.Activities)
                {
                    if (activityEntities.TryGetValue(activityName, out var activity))
                    {
                        context.ParkActivities.Add(new ParkActivity
                        {
                            ParkId = park.Id,
                            ActivityId = activity.Id
                        });
                    }
                }
            }

            // Add park images
            if (parkData.Images != null)
            {
                foreach (var imageData in parkData.Images)
                {
                    context.ParkImages.Add(new ParkImage
                    {
                        ParkId = park.Id,
                        Url = imageData.Url,
                        AltText = imageData.AltText
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }

    private class ParkSeedData
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Website { get; set; }
        public bool IsFeatured { get; set; }
        public string Region { get; set; } = string.Empty;
        public List<string>? Activities { get; set; }
        public List<ImageSeedData>? Images { get; set; }
    }

    private class ImageSeedData
    {
        public string Url { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }
}
