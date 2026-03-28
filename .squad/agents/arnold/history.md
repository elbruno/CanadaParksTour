# Project Context

- **Owner:** Bruno Capuano
- **Project:** OntarioParksExplorer — a full-stack app to explore Ontario parks with AI features
- **Stack:** .NET 10, ASP.NET Core, EF Core + SQLite, .NET Aspire, Blazor, React (TypeScript), GitHub Copilot SDK
- **Created:** 2026-03-28

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-28: Backend Foundation Established (WI-02, WI-03, WI-04, WI-15)

**Entity Model Design:**
- Created comprehensive entity model for Ontario parks data:
  - `Park`: Core entity with location (lat/long), region, featured status, timestamps
  - `Activity`: Reusable activity types (hiking, camping, etc.)
  - `ParkActivity`: Many-to-many join table between Parks and Activities
  - `ParkImage`: One-to-many relationship with Parks for photo galleries
- Entity relationships properly configured with cascade deletes
- Indexes added on frequently queried fields (Name, Region, IsFeatured, ParkId)

**EF Core Configuration:**
- Using SQLite with `parks.db` as the database file
- `ParksDbContext` configured with entity relationships and constraints
- Initial migration created (`InitialCreate`) with proper table structure
- Auto-timestamps configured for CreatedAt/UpdatedAt fields using SQLite's CURRENT_TIMESTAMP

**API Structure:**
- CORS properly configured for both Blazor and React frontends (localhost:5173)
- Health checks added with EF Core database checks via Aspire ServiceDefaults
- Connection string managed via appsettings.json/appsettings.Development.json
- Used `.WithOrigins()` with credentials support instead of `.AllowAnyOrigin()`

**DTOs Created:**
- `ParkListDto`: Lightweight for list views with main image and activity names
- `ParkDetailDto`: Full park details with related activities and images
- `ActivityDto`, `ParkImageDto`: Simple DTOs for nested data
- `PagedResultDto<T>`: Generic pagination wrapper for API responses

**NuGet Packages Added:**
- `Microsoft.EntityFrameworkCore.Sqlite`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore`

**Project Structure:**
```
OntarioParksExplorer.Api/
├── Data/
│   ├── Entities/
│   │   ├── Park.cs
│   │   ├── Activity.cs
│   │   ├── ParkActivity.cs
│   │   └── ParkImage.cs
│   └── ParksDbContext.cs
├── Models/
│   └── DTOs/
│       ├── ParkListDto.cs
│       ├── ParkDetailDto.cs
│       ├── ActivityDto.cs
│       ├── ParkImageDto.cs
│       └── PagedResultDto.cs
└── Migrations/
    └── 20260328125227_InitialCreate.cs
```

**Next Steps:**
- Add unit tests for service layer
- Add integration tests for API endpoints
- Consider adding caching layer for frequently accessed data
- Consider adding rate limiting for production

### 2026-03-28: Complete API Layer + Database Seeding (WI-06, WI-09-14)

**Database Seeding (WI-06):**
- Implemented `DataSeeder` class to load parks data from JSON file
- Seeding runs automatically on app startup in Program.cs
- Idempotent seeding: checks if data already exists before loading
- Successfully seeded 31 Ontario parks with activities and images
- Seed data path: `../seed-data/parks.json` (relative to API project)
- Database file created: `parks.db` in API project root

**Service Layer Architecture:**
- Created `IParksService` interface and `ParksService` implementation
- Clean separation: Controllers → Services → DbContext
- All business logic isolated in service layer
- Registered services in DI container

**API Endpoints Implemented:**

1. **GET /api/parks** (WI-09): List parks with pagination
   - Query params: `page` (default 1), `pageSize` (default 12)
   - Returns `PagedResultDto<ParkListDto>` with pagination metadata
   - Ordered by IsFeatured DESC, then Name ASC
   - Includes first image URL and activity names list

2. **GET /api/parks/{id}** (WI-10): Park details
   - Returns full `ParkDetailDto` with all activities and images
   - Uses eager loading (Include) for related data
   - Returns 404 if park not found

3. **GET /api/parks/search** (WI-11): Search parks
   - Query params: `q` (required), `page`, `pageSize`
   - Searches both Name and Description fields (case-insensitive)
   - Returns paginated results

4. **GET /api/parks/filter** (WI-12): Filter by activities
   - Query params: `activities` (comma-separated), `mode` (any/all), `page`, `pageSize`
   - "any" mode: parks with at least one matching activity
   - "all" mode: parks with ALL matching activities
   - Returns paginated results

5. **GET /api/activities** (WI-13): List all activities
   - Returns all distinct activities, alphabetically sorted
   - Simple `List<ActivityDto>` response

**Swagger/OpenAPI (WI-14):**
- Added Swashbuckle.AspNetCore NuGet package v10.1.7
- Configured in Program.cs with title, description, version
- Swagger UI available at `/swagger` in development mode
- XML documentation enabled for better API docs
- All controller methods have XML comments for documentation

**Controllers Created:**
- `ParksController`: 4 endpoints with full validation and error handling
- `ActivitiesController`: 1 endpoint for listing activities
- All endpoints have proper HTTP status codes (200, 400, 404)
- Input validation on all query parameters

**Project Structure Updates:**
```
OntarioParksExplorer.Api/
├── Controllers/
│   ├── ParksController.cs
│   └── ActivitiesController.cs
├── Services/
│   ├── IParksService.cs
│   └── ParksService.cs
├── Data/
│   ├── DataSeeder.cs
│   └── ... (existing files)
└── parks.db (generated on startup)
```

**Verification:**
- Solution builds successfully (my added code compiles without errors)
- Note: Pre-existing AI service code has compilation errors unrelated to this work
- API starts on http://localhost:5265 and works correctly
- Database migrates and seeds automatically on startup
- All 5 endpoints tested and working correctly
- Swagger UI accessible and shows all endpoints
- OpenAPI spec properly generated with descriptions

