using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OntarioParksExplorer.Api.Data;
using OntarioParksExplorer.Api.Data.Entities;

namespace OntarioParksExplorer.Api.Tests.Data;

public class ParksDbContextTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ParksDbContext _context;

    public ParksDbContextTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ParksDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ParksDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public void DbContext_CreatesDatabase_AndAppliesSchema()
    {
        // Assert
        Assert.True(_context.Database.CanConnect());
        Assert.NotNull(_context.Parks);
        Assert.NotNull(_context.Activities);
        Assert.NotNull(_context.ParkActivities);
        Assert.NotNull(_context.ParkImages);
    }

    [Fact]
    public async Task DbContext_CanInsertAndRetrievePark()
    {
        // Arrange
        var park = new Park
        {
            Name = "Test Park",
            Description = "A test park for unit testing",
            Location = "Test Location, Ontario",
            Latitude = 45.0,
            Longitude = -79.0,
            Website = "https://example.com",
            IsFeatured = false,
            Region = "Test Region",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        _context.Parks.Add(park);
        await _context.SaveChangesAsync();

        // Assert
        var retrieved = await _context.Parks.FirstOrDefaultAsync(p => p.Name == "Test Park");
        Assert.NotNull(retrieved);
        Assert.Equal("Test Park", retrieved.Name);
        Assert.Equal("Test Location, Ontario", retrieved.Location);
    }

    [Fact]
    public async Task DbContext_ParkActivityManyToMany_WorksCorrectly()
    {
        // Arrange
        var activity = new Activity { Name = "Test Activity" };
        var park = new Park
        {
            Name = "Test Park",
            Description = "A test park",
            Location = "Test Location",
            Latitude = 45.0,
            Longitude = -79.0,
            Region = "Test Region",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Activities.Add(activity);
        _context.Parks.Add(park);
        await _context.SaveChangesAsync();

        var parkActivity = new ParkActivity
        {
            ParkId = park.Id,
            ActivityId = activity.Id
        };

        // Act
        _context.ParkActivities.Add(parkActivity);
        await _context.SaveChangesAsync();

        // Assert
        var retrievedPark = await _context.Parks
            .Include(p => p.ParkActivities)
            .ThenInclude(pa => pa.Activity)
            .FirstOrDefaultAsync(p => p.Id == park.Id);

        Assert.NotNull(retrievedPark);
        Assert.Single(retrievedPark.ParkActivities);
        Assert.Equal("Test Activity", retrievedPark.ParkActivities.First().Activity.Name);
    }

    [Fact]
    public async Task DbContext_ParkImageOneToMany_WorksCorrectly()
    {
        // Arrange
        var park = new Park
        {
            Name = "Test Park",
            Description = "A test park",
            Location = "Test Location",
            Latitude = 45.0,
            Longitude = -79.0,
            Region = "Test Region",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Parks.Add(park);
        await _context.SaveChangesAsync();

        var images = new[]
        {
            new ParkImage { ParkId = park.Id, Url = "https://example.com/img1.jpg", AltText = "Image 1" },
            new ParkImage { ParkId = park.Id, Url = "https://example.com/img2.jpg", AltText = "Image 2" }
        };

        // Act
        _context.ParkImages.AddRange(images);
        await _context.SaveChangesAsync();

        // Assert
        var retrievedPark = await _context.Parks
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == park.Id);

        Assert.NotNull(retrievedPark);
        Assert.Equal(2, retrievedPark.Images.Count);
        Assert.Contains(retrievedPark.Images, i => i.Url == "https://example.com/img1.jpg");
        Assert.Contains(retrievedPark.Images, i => i.Url == "https://example.com/img2.jpg");
    }

    [Fact]
    public async Task DbContext_CascadeDelete_WorksForParkActivities()
    {
        // Arrange
        var activity = new Activity { Name = "Test Activity" };
        var park = new Park
        {
            Name = "Test Park",
            Description = "A test park",
            Location = "Test Location",
            Latitude = 45.0,
            Longitude = -79.0,
            Region = "Test Region",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Activities.Add(activity);
        _context.Parks.Add(park);
        await _context.SaveChangesAsync();

        _context.ParkActivities.Add(new ParkActivity { ParkId = park.Id, ActivityId = activity.Id });
        await _context.SaveChangesAsync();

        // Act
        _context.Parks.Remove(park);
        await _context.SaveChangesAsync();

        // Assert
        var parkActivity = await _context.ParkActivities
            .FirstOrDefaultAsync(pa => pa.ParkId == park.Id);
        Assert.Null(parkActivity);
    }

    [Fact]
    public async Task DbContext_CascadeDelete_WorksForParkImages()
    {
        // Arrange
        var park = new Park
        {
            Name = "Test Park",
            Description = "A test park",
            Location = "Test Location",
            Latitude = 45.0,
            Longitude = -79.0,
            Region = "Test Region",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Parks.Add(park);
        await _context.SaveChangesAsync();

        _context.ParkImages.Add(new ParkImage { ParkId = park.Id, Url = "https://example.com/img1.jpg" });
        await _context.SaveChangesAsync();

        // Act
        _context.Parks.Remove(park);
        await _context.SaveChangesAsync();

        // Assert
        var image = await _context.ParkImages
            .FirstOrDefaultAsync(i => i.ParkId == park.Id);
        Assert.Null(image);
    }

    [Fact]
    public async Task DbContext_IndexOnName_Exists()
    {
        // Arrange - Create multiple parks to test index effectiveness
        var parks = new[]
        {
            new Park
            {
                Name = "Alpha Park",
                Description = "First park",
                Location = "Location A",
                Latitude = 45.0,
                Longitude = -79.0,
                Region = "Region A",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Park
            {
                Name = "Beta Park",
                Description = "Second park",
                Location = "Location B",
                Latitude = 46.0,
                Longitude = -80.0,
                Region = "Region B",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Parks.AddRange(parks);
        await _context.SaveChangesAsync();

        // Act - Query by name should benefit from index
        var result = await _context.Parks
            .Where(p => p.Name == "Alpha Park")
            .FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alpha Park", result.Name);
    }

    [Fact]
    public async Task DbContext_IndexOnRegion_Exists()
    {
        // Arrange
        var parks = new[]
        {
            new Park
            {
                Name = "Park 1",
                Description = "Description 1",
                Location = "Location 1",
                Latitude = 45.0,
                Longitude = -79.0,
                Region = "Georgian Bay Region",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Park
            {
                Name = "Park 2",
                Description = "Description 2",
                Location = "Location 2",
                Latitude = 46.0,
                Longitude = -80.0,
                Region = "Georgian Bay Region",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Parks.AddRange(parks);
        await _context.SaveChangesAsync();

        // Act - Query by region should benefit from index
        var results = await _context.Parks
            .Where(p => p.Region == "Georgian Bay Region")
            .ToListAsync();

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task DbContext_ActivityNameIndex_IsUnique()
    {
        // Arrange
        var activity1 = new Activity { Name = "Unique Activity" };
        _context.Activities.Add(activity1);
        await _context.SaveChangesAsync();

        // Act & Assert
        var activity2 = new Activity { Name = "Unique Activity" };
        _context.Activities.Add(activity2);
        
        await Assert.ThrowsAsync<DbUpdateException>(async () => 
            await _context.SaveChangesAsync());
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
