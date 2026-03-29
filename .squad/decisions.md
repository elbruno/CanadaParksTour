# Squad Decisions

## Active Decisions

### 1. Service Layer Pattern (Arnold, 2026-03-28)
**Status:** Implemented

Three-layer architecture: Controllers (HTTP concerns) → Services (business logic) → DbContext (data access). Ensures clean separation of concerns, testability, and easy integration of cross-cutting concerns like caching and authorization.

**Key Decision:** All LINQ queries and EF Include statements in service layer. Controllers depend on IParksService via DI.

---

### 2. Idempotent Database Seeding (Arnold, 2026-03-28)
**Status:** Implemented

Automatic seeding in Program.cs startup with idempotency check: `if (!await context.Parks.AnyAsync()) { await DataSeeder.SeedAsync(context, seedDataPath); }`. Ensures database always seeded on first run without manual commands or duplicate data on restart.

---

### 3. Activity Filtering Logic (Arnold, 2026-03-28)
**Status:** Implemented

Two modes via LINQ queries:
- **"any" mode** (default): `Where(p => p.ParkActivities.Any(pa => activityNames.Contains(...)))`
- **"all" mode**: `Where(p => activityNames.All(activityName => p.ParkActivities.Any(...)))`

EF Core translates to efficient SQL. Enables precise filtering without manual SQL.

---

### 4. Swagger in Development Only (Arnold, 2026-03-28)
**Status:** Implemented

Swagger/SwaggerUI enabled only in Development environment. Standard ASP.NET Core practice: reduces production attack surface while supporting frontend devs and testers.

---

### 5. Pagination Defaults (Arnold, 2026-03-28)
**Status:** Implemented

Page 1, PageSize 12 (max 100). Fits standard grid layouts (3x4, 4x3, 2x6). Prevents accidental large queries. Returns consistent `PagedResultDto<T>` pagination metadata: totalCount, page, pageSize, totalPages.

---

### 6. Backend Foundation: Entity Model & CORS (Arnold, 2026-03-28)
**Status:** Implemented

**Entity Design:**
- Single SQLite database (parks.db)
- Many-to-many Parks ↔ Activities via explicit ParkActivity join table
- One-to-many Parks → Images for photo galleries
- Auto-managed CreatedAt/UpdatedAt with SQLite CURRENT_TIMESTAMP

**CORS:** Configured for `localhost:5173` (React/Vite) with credentials support. Used `.WithOrigins()` instead of `.AllowAnyOrigin()` for better security.

**Database Indexes:** Park.Name, Park.Region, Park.IsFeatured, Activity.Name (unique), ParkImage.ParkId (FK).

---

### 7. DTO Design Pattern (Arnold, 2026-03-28)
**Status:** Implemented

Separate `ParkListDto` (lightweight) for lists and `ParkDetailDto` (full data) for details. Generic `PagedResultDto<T>` for consistent pagination. Optimizes data transfer and promotes consistency.

---

### 8. AI API Endpoints Architecture (Grant, 2026-03-28)
**Status:** Implemented

Single `AiController` with route prefix `/api/ai` containing four AI-powered endpoints:
1. **POST /api/ai/parks/{id}/summary** — Park summaries
2. **POST /api/ai/recommendations** — Personalized recommendations
3. **POST /api/ai/chat** — Q&A chatbot
4. **POST /api/ai/plan-visit** — Visit itinerary planner

**Rationale:** Separation of concerns (AI vs CRUD), consistent routing for rate limiting/monitoring, proper HTTP semantics (POST for non-idempotent operations).

---

### 9. AI Setup: Package Selection & Service Architecture (Grant, 2026-03-28)
**Status:** Implemented

**Package Stack:** `Microsoft.Extensions.AI` + `Microsoft.Extensions.AI.OpenAI` + base `OpenAI`. Official Microsoft abstraction provides provider-agnostic design and native .NET 10 support.

**Service Architecture:** Dedicated `Services/AI/` folder with `IAiService`/`AiService`, centralized `Prompts/PromptTemplates.cs`, AI-specific DTOs.

**Graceful Degradation:** All AI methods check `_isConfigured` flag and return friendly messages when unconfigured. Try/catch with logging, never throw to caller.

**Configuration:** Store settings in `appsettings.json` under `"AI"` section (Provider, Model, Endpoint, ApiKey).

---

### 10. Aspire Service Discovery & CORS (Malcolm, 2026-03-28)
**Status:** Accepted

**Service Naming:** Lowercase, simple identifiers ("api", "blazor"). Service discovery base URLs follow Aspire convention: `http://servicename`.

**CORS Policy:** `AllowFrontends` with `AllowAnyOrigin()`, `AllowAnyMethod()`, `AllowAnyHeader()` for rapid dev. **DEV-ONLY** — production must restrict origins, lock down methods/headers.

**ServiceDefaults:** All services call `builder.AddServiceDefaults()` (health checks, telemetry, resilience) and `app.MapDefaultEndpoints()` (expose health endpoints).

---

### 11. Aspire Orchestration Review (Hammond, 2026-03-28)
**Status:** ✅ Completed

**Changes Applied:**
- Added `.WaitFor(api)` to Blazor resource ensuring API starts first, Blazor waits for healthy API
- Verified service discovery, health checks, telemetry integration

**Service Topology:** API (port dynamic) → Blazor (port 7001) + React (port 5173). All report to Aspire Dashboard via OTLP (OpenTelemetry). Service discovery for Blazor/React → API via `http://api`.

**Validation:** ✅ Build, ✅ ServiceDefaults, ✅ API/Blazor/React integration, ✅ startup ordering, ✅ service discovery, ✅ health checks, ✅ external endpoints.

**Known Limitation:** `AddViteApp` not yet available in Aspire.Hosting.NodeJs 9.5.2. Workaround: Using `AddNpmApp("react", "../OntarioParksExplorer.React", "dev")` which works correctly.

---

### 12. Blazor Frontend Architecture (Sattler, 2026-03-28)
**Status:** Implemented

**UI:** MudBlazor for Material Design components, reduces custom CSS.

**API Client:** Dedicated `ParksApiClient` service class via typed HttpClient with service discovery (base address `http://api`).

**Favorites:** `FavoritesService` using `IJSRuntime` to access localStorage, stores JSON array of park IDs. Scoped service enabling component-wide favorites context.

**Maps:** JavaScript interop with Leaflet (wwwroot/js/map.js). No stable Blazor wrapper exists; JS interop is well-supported alternative.

**Render Mode:** Interactive Server (requires SignalR). Enables real-time updates without full page reloads. May reconsider WebAssembly for production scale.

**Search:** 300ms debounce with System.Threading.Timer prevents excessive API calls while typing.

**Filters:** Multi-select via MudSelect binding to `IReadOnlyCollection<string>` property with custom setter triggering API calls.

---

### 13. Frontend Setup: UI Framework & Navigation (Sattler, 2026-03-28)
**Status:** Implemented

**UI Framework Choice:** MudBlazor for Blazor (comprehensive Material Design components), custom CSS for React (smaller bundle, full control).

**Navigation Structure (both frontends):**
- `/` — Home page with welcome and features
- `/parks` — Browse/search parks
- `/favorites` — User's favorite parks
- `/map` — Interactive map view
- `/chat` — AI chat interface (added post-MVP)
- `/recommendations` — AI recommendations (added post-MVP)

**Aspire Integration:** React via `AddNpmApp`, dev server proxies `/api` requests. Aspire orchestrates all services. Service discovery and health checks automatic.

**Mobile Responsiveness:** Mobile-first with breakpoint at 768px. Blazor uses MudDrawer toggle, React uses hamburger menu. Consistent experience across devices.

---

### 14. Blazor Interactive Server Render Mode — Routes Component (Sattler, 2026-03-29)
**Status:** Implemented

**Problem:** Blazor app interactivity was broken — favorite button clicks and MudSelect activity filter dropdown were unresponsive. All interactive components using event handlers were non-functional.

**Root Cause:** Missing `@rendermode InteractiveServer` directive on the `Components/Routes.razor` component. In .NET 10 Blazor, while individual pages can specify render modes, the Routes component itself must also have the render mode for SignalR-based interactivity to work app-wide.

**Solution:** Added `@rendermode InteractiveServer` to the top of `Components/Routes.razor`. This establishes the SignalR circuit across all routed components.

**Impact:**
- ✅ Favorite buttons now work with localStorage persistence
- ✅ MudSelect activity filters respond to clicks and selection
- ✅ All Blazor interactive components functional (search, pagination, navigation)
- ✅ SignalR connection established properly for real-time updates

**Key Learning:** In .NET 10 Blazor with Interactive Server mode, the Routes component must also specify the render mode. Individual pages with `@rendermode` directive alone are insufficient for SignalR circuit establishment.

---

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
