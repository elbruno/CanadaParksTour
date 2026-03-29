See the PRD below and create a team and a plan to complete this.
Do not implement anything, just create the team and the plan.
---

# LifeLog AI — Product Requirements Document

## Product Overview

Build a modern full-stack application named **LifeLogAI** — a chronological life journal that captures text notes, photos, and audio recordings, organized into sessions and displayed as a timeline. AI features powered by GitHub Copilot SDK provide transcription, summarization, and conversational search over entries.

**Target audience:** Personal productivity users — meeting notes, travel logs, daily journaling, project documentation.

**Demo-ready:** This app must look polished for live presentations. Seed data should be realistic and varied. Both frontends consume the same backend APIs and share the same data.

---

## Stack Requirements

> ⚠️ **CRITICAL — AI Provider:** Do NOT use OpenAI. Do NOT use Azure OpenAI. Use ONLY GitHub Copilot SDK via `Microsoft.Extensions.AI.CopilotSDK`. Use `Microsoft.Extensions.AI` as the provider-agnostic abstraction layer.

| Layer | Technology | Notes |
|-------|-----------|-------|
| Runtime | .NET 10 | Latest stable SDK |
| Backend | ASP.NET Core Web API | Controllers, not minimal APIs |
| ORM | Entity Framework Core + SQLite | Single `lifelog.db` file |
| Orchestration | .NET Aspire | AppHost + ServiceDefaults |
| Blazor Frontend | Blazor Web App, Interactive Server | MudBlazor component library |
| React Frontend | TypeScript + Vite + React Router | Custom CSS or lightweight library |
| AI | GitHub Copilot SDK | Via `Microsoft.Extensions.AI.CopilotSDK` |
| AI Abstraction | `Microsoft.Extensions.AI` | Provider-agnostic interface |
| Maps (if needed) | Leaflet + OpenStreetMap | Free, no API key required |
| Packages | Latest stable NuGet + npm | Pin versions in project files |

---

## Architecture Constraints

These are non-negotiable. They encode lessons from a previous build.

### Three-Layer Architecture
```
Controllers (HTTP concerns) → Services (business logic) → DbContext (data access)
```
No business logic in controllers. Controllers depend on service interfaces via DI.

### AI Service Abstraction
Create an `IAiService` interface. All AI endpoints call through this interface. This makes provider swaps surgical — one file changes, not twenty.

### Graceful AI Degradation
All AI methods must check an `_isConfigured` flag. Return friendly messages when AI is unconfigured (e.g., `"AI features require configuration. See Settings page."`). Never throw exceptions to the caller. **The app must be fully functional WITHOUT AI configured.**

### Idempotent Database Seeding
Seed data in `Program.cs` with idempotency check:
```csharp
if (!await context.Sessions.AnyAsync()) { await DataSeeder.SeedAsync(context); }
```
The app must work on first run with zero manual migration steps. Use `EnsureCreated()` or auto-apply migrations at startup.

### DTOs for API Responses
Never expose EF Core entities directly. Use `PagedResultDto<T>` for all list endpoints. Separate lightweight list DTOs from full detail DTOs.

### Aspire Service Discovery
- Service names: lowercase, simple — `"api"`, `"blazor"`, `"react"`
- Base address convention: `http://api` for service-to-service calls
- Startup ordering: `.WaitFor(api)` on both frontend services
- Health checks: `AddServiceDefaults()` and `MapDefaultEndpoints()` on every service

---

## Data Model

### Entities

| Entity | Fields |
|--------|--------|
| **Session** | Id (Guid), Name, Description, CreatedAt, UpdatedAt, IsPinned |
| **Entry** | Id (Guid), SessionId (FK), Type (Text/Photo/Audio), Content, Timestamp, IsPinned, CreatedAt, UpdatedAt |
| **Tag** | Id (Guid), Name (unique) |
| **EntryTag** | EntryId (FK), TagId (FK) — join table |

**Indexes:** Session.CreatedAt, Entry.SessionId, Entry.Timestamp, Entry.Type, Tag.Name (unique).

**Relationships:** Session 1→∞ Entry. Entry ∞↔∞ Tag via EntryTag.

### Seed Data
Provide 4 sample sessions with realistic entries:
1. **"Monday Standup — Sprint 42"** — 5 text entries (status updates, blockers, action items)
2. **"Vancouver Trip 2025"** — 8 entries (text + photo descriptions, restaurant notes, hiking log)
3. **"Product Design Review"** — 4 entries (meeting notes, decisions, follow-ups with TODO markers)
4. **"Personal Journal — March"** — 6 entries (daily reflections, reading notes, ideas)

---

## Core Features

### Sessions
- **Create:** Name + optional description
- **List:** Paginated, sorted by UpdatedAt descending, pinned sessions first
- **Rename/Edit:** Update name and description
- **Delete:** Cascade-delete all entries and tag associations
- **Pin/Unpin:** Toggle IsPinned for quick access

### Entries
- **Create:** Text, photo (URL/path), or audio (URL/path) with timestamp and optional tags
- **Edit:** Update content, tags, timestamp
- **Delete:** Remove entry and tag associations
- **Pin/Unpin:** Mark important entries

### Timeline View
- Chronological display of entries within a session
- Group entries by date
- Visual type indicators (text icon, camera icon, microphone icon)
- Pinned entries highlighted

### Search
- Full-text search across entry content
- Scope: within a session or globally across all sessions
- Results show session context and entry snippet

### Pagination
- Default: 20 items per page
- Maximum: 100 items per page
- Response format: `PagedResultDto<T>` with totalCount, page, pageSize, totalPages

---

## AI Features (GitHub Copilot SDK)

All AI features go through `IAiService`. All degrade gracefully when unconfigured.

| Feature | Endpoint | Description |
|---------|----------|-------------|
| **Audio Transcription** | `POST /api/ai/transcribe` | Upload audio → Copilot SDK transcribes → stored as text entry with audio reference |
| **Session Summary** | `POST /api/ai/sessions/{id}/summarize` | Summarize a session's entries into key points and themes |
| **Timeline Extraction** | `POST /api/ai/sessions/{id}/timeline` | Extract key events and dates from entries into a structured timeline |
| **Follow-up Detection** | `POST /api/ai/sessions/{id}/followups` | Identify action items, TODO markers, and commitments |
| **Q&A Chat** | `POST /api/ai/chat` | Conversational search: "What happened in my Monday standup?" |
| **Smart Tagging** | `POST /api/ai/entries/{id}/suggest-tags` | Auto-suggest tags based on entry content |

**AI Configuration:** Store in `appsettings.json` under `"AI"` section. Provide a Settings page in both frontends to verify configuration status.

---

## Frontend Requirements

### Blazor (MudBlazor, Interactive Server)

| Page | Route | Description |
|------|-------|-------------|
| Home | `/` | Welcome page with feature highlights |
| Sessions | `/sessions` | Paginated session list with search, pin indicators |
| Session Detail | `/sessions/{id}` | Timeline view of entries, grouped by date |
| Entry Editor | (dialog/panel) | Create/edit entries with type selector and tag input |
| AI Chat | `/chat` | Conversational interface over entries |
| Settings | `/settings` | AI configuration status, user preferences |

- Typed `ApiClient` service via HttpClient with base address `http://api`
- localStorage for user preferences (theme, default page size)
- Responsive layout with MudDrawer navigation

### React (TypeScript + Vite + React Router)

| Page | Route | Description |
|------|-------|-------------|
| Home | `/` | Welcome page with feature highlights |
| Sessions | `/sessions` | Paginated session list with search, pin indicators |
| Session Detail | `/sessions/:id` | Timeline view of entries, grouped by date |
| Entry Editor | (modal/panel) | Create/edit entries with type selector and tag input |
| AI Chat | `/chat` | Conversational interface over entries |
| Settings | `/settings` | AI configuration status, user preferences |

- Custom CSS (no heavy component library) — mobile-first responsive design
- Proxy API calls through Vite dev server config or handle CORS
- Same data and behavior as Blazor — both are first-class frontends

---

## API Endpoints

### Sessions
| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/sessions` | List sessions (paginated) |
| GET | `/api/sessions/{id}` | Get session with entries |
| POST | `/api/sessions` | Create session |
| PUT | `/api/sessions/{id}` | Update session |
| DELETE | `/api/sessions/{id}` | Delete session + entries |
| POST | `/api/sessions/{id}/pin` | Toggle pin |

### Entries
| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/sessions/{sessionId}/entries` | List entries (paginated) |
| POST | `/api/sessions/{sessionId}/entries` | Create entry |
| PUT | `/api/entries/{id}` | Update entry |
| DELETE | `/api/entries/{id}` | Delete entry |
| POST | `/api/entries/{id}/pin` | Toggle pin |

### Search & Tags
| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/search?q={query}` | Global search |
| GET | `/api/sessions/{id}/search?q={query}` | Session-scoped search |
| GET | `/api/tags` | List all tags |

---

## Testing Requirements

- **Unit tests:** xUnit for all service methods and controller actions. Test AI degradation (unconfigured returns friendly message, not exception).
- **E2E tests:** Playwright specs covering: create session → add entries → view timeline → AI chat → search.
- **Screenshot automation:** Playwright captures standardized screenshots (1280×800) for documentation.

---

## Documentation Requirements

- **README.md:** Project overview, architecture diagram (ASCII), getting started, API endpoint reference, AI configuration guide
- **User Manual:** Step-by-step guide with embedded screenshots for both frontends
- **Demo Script:** 10–15 minute guided walkthrough for presentations covering sessions, entries, timeline, AI features

---

## Lessons from OntarioParksExplorer

> These are hard-won. Violating them will cause rework.

1. **AI provider drift:** The team WILL default to OpenAI if not explicitly told otherwise. This PRD says GitHub Copilot SDK via `Microsoft.Extensions.AI.CopilotSDK`. Enforce it in code review. Every. Single. Time.
2. **Aspire CLI:** Use `aspire run` to start the app (not `dotnet run --project AppHost`). Install Aspire CLI via `irm https://aspire.dev/install.ps1 | iex`.
3. **Blazor JS interop timing:** When using JavaScript libraries (Leaflet, audio players), ensure the DOM element exists before initializing. Use `OnAfterRenderAsync` with `firstRender` check.
4. **CORS in development:** Use `AllowAnyOrigin()` for dev but document that production must restrict origins.
5. **React service discovery:** The Vite dev server needs proxy configuration in `vite.config.ts` or CORS headers to reach the API. Handle this during scaffolding, not after.
6. **Seed data quality:** Good seed data makes demos compelling. Include realistic, varied entries — not "Test Entry 1".
7. **Graceful degradation:** AI features enhance the core experience but must never gate it. The app runs fully without AI configured.
8. **Settings page:** Always include a settings/config page so users can verify their setup without reading config files or environment variables.
9. **AsNoTracking:** Use `AsNoTracking()` on all read-only EF Core queries — 15-30% performance improvement for free.
10. **Accessibility:** Add ARIA landmarks, alt text, and semantic HTML from the start. Retrofitting is painful.

---

## Execution Instructions

This PRD is designed for GitHub Copilot CLI with Squad agent orchestration.

### Phase 1: Team & Plan
1. Give this file to Copilot CLI with Squad installed
2. Squad will create a specialized team (6–8 agents)
3. The Lead will decompose this into work items with dependencies
4. Review and approve the plan before implementation begins

### Phase 2: Implementation
1. Squad executes the plan with parallel agent work
2. Backend (API + EF Core + AI service), frontends (Blazor + React), and tests happen simultaneously
3. The Lead reviews cross-cutting concerns (service discovery, AI provider, DTOs)
4. **Human reviews deliverables against this PRD — especially the AI provider!**

### Phase 3: Polish
1. Automated screenshot capture via Playwright
2. Documentation generation (README, User Manual, Demo Script)
3. Settings page for configuration verification
4. Final seed data review — make it demo-worthy
5. Push to GitHub and verify public-facing docs