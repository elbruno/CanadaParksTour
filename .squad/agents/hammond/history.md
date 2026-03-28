# Project Context

- **Owner:** Bruno Capuano
- **Project:** OntarioParksExplorer — a full-stack app to explore Ontario parks with AI features
- **Stack:** .NET 10, ASP.NET Core, EF Core + SQLite, .NET Aspire, Blazor, React (TypeScript), GitHub Copilot SDK
- **Created:** 2026-03-28

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-28 — Aspire Orchestration Review and Update

**Current Service Topology:**
- **OntarioParksExplorer.Api** (API resource: "api") — ASP.NET Core Web API with EF Core + SQLite
- **OntarioParksExplorer.Blazor** (Blazor resource: "blazor") — Blazor Web App with MudBlazor
- **OntarioParksExplorer.React** (React resource: "react") — React TypeScript app (Vite-based)
- **OntarioParksExplorer.ServiceDefaults** — Shared Aspire configuration

**AppHost Configuration (AppHost.cs):**
- Uses Aspire.AppHost.Sdk 13.2.0
- Uses Aspire.Hosting.NodeJs 9.5.2 for npm app hosting
- All services registered with descriptive names ("api", "blazor", "react")
- Startup ordering enforced: API starts first, frontends wait via `.WaitFor(api)`
- Service discovery configured: Blazor and React both use `.WithReference(api)` to discover API endpoint
- React app uses `.AddNpmApp("react", "../OntarioParksExplorer.React", "dev")` with "dev" script
- All services have `.WithExternalHttpEndpoints()` for dashboard access

**Service Integration Status:**
- ✅ API uses `builder.AddServiceDefaults()` for Aspire integration
- ✅ API registers health checks including DbContext check for database
- ✅ API uses `app.MapDefaultEndpoints()` to expose health and telemetry endpoints
- ✅ Blazor uses `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`
- ✅ Blazor configures HttpClient with base address `http://api` for service discovery
- ✅ ServiceDefaults configures OpenTelemetry (logs, metrics, traces), health checks, and service discovery
- ✅ ServiceDefaults adds resilience handlers to all HttpClients by default

**Aspire Patterns Applied:**
1. **Service discovery** — Blazor/React discover API via `WithReference(api)`, resolves `http://api` to actual endpoint
2. **Startup ordering** — `.WaitFor(api)` ensures API is healthy before frontends start
3. **Health checks** — All services expose `/health` (ready) and `/alive` (liveness) endpoints in development
4. **OpenTelemetry** — Logs, metrics, and distributed traces automatically exported to Aspire Dashboard
5. **Resilience** — Standard resilience handlers (retry, circuit breaker, timeout) on all HttpClients
6. **External endpoints** — All services exposed through dashboard for easy access during development

**Key Configuration Details:**
- Database: SQLite at `parks.db` with EF Core migrations
- CORS: API allows `http://localhost:5173` and `https://localhost:5173` for React dev server
- React dev server: Uses Vite on PORT env variable, proxies `/api` requests internally (configured in vite.config.ts)
- Health check endpoints: `/health` (all checks) and `/alive` (liveness only) in development mode

**Package Versions:**
- Aspire.AppHost.Sdk: 13.2.0
- Aspire.Hosting.NodeJs: 9.5.2 (latest available, `AddViteApp` not yet available in this version)
- .NET: 10.0

**Note on AddViteApp vs AddNpmApp:**
- Aspire 13.x documentation mentions `AddViteApp` as the preferred method for Vite-based apps
- However, `AddViteApp` is not yet available in Aspire.Hosting.NodeJs 9.5.2
- Using `AddNpmApp` with "dev" script works correctly for Vite apps
- Will update to `AddViteApp` when package is updated to support it
