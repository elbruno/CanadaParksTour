# From Prompt to Production

**How an AI team built a full-stack .NET Aspire app from natural language**

---

## 1. The Prompt

It started with one person, one terminal, and one sentence.

Bruno Capuano opened GitHub Copilot CLI, typed a prompt, and walked away from the keyboard. What came back wasn't a code snippet or a suggestion — it was a working application, built by a team of AI agents that had never existed before that moment.

The prompt that launched the initial build was broad:

> *Build an Ontario Parks Explorer — a full-stack .NET Aspire app with an ASP.NET Core API, Blazor frontend, React frontend, SQLite database, and AI features. Seed it with Ontario Provincial Parks data.*

That prompt created the foundation. But the prompt that stress-tested the team — the one that proved this workflow works — came later:

> *"The AI chat feature should not use OpenAI APIs, it should use Microsoft Agent Framework and Copilot SDK Agents, so we can reuse the current Copilot installation. Also in the Blazor web app, the map is not showing — do we need a key or similar? Add a configuration or settings page if they are needed to set these keys or values. Process all of this and complete the work. Once completed, take screenshots and create a user manual. Push everything to the repo once it's done."*

One natural-language paragraph. Five distinct tasks. Zero pseudocode.

The AI team parsed it, split the work, and delivered.

> 💡 **Insight:** The second prompt is the interesting one. Anyone can scaffold a new project. The real test is whether an AI team can *modify* a running system — swap a dependency, fix a bug, add a feature, document it — without breaking what already works.

---

## 2. Assembling the Team

[Squad](https://github.com/bradygaster/squad) doesn't generate code by committee. It creates *specialized agents* — each with a defined role, bounded expertise, and persistent memory. For this project, it assembled a team from the Jurassic Park universe:

| Agent | Role | Specialty |
|-------|------|-----------|
| 🏗️ **Malcolm** | Lead / Architect | Architecture decisions, Aspire orchestration, service discovery, code review |
| 🔧 **Arnold** | Backend Dev | ASP.NET Core APIs, EF Core, SQLite, services layer, DTOs |
| ⚛️ **Sattler** | Frontend Dev | Blazor (MudBlazor), React (TypeScript/Vite), Leaflet maps, UX |
| 🤖 **Grant** | AI Dev | OpenAI → Copilot SDK migration, prompt engineering, AI endpoints |
| 🧪 **Muldoon** | Tester | 46+ unit tests, Playwright E2E specs, screenshot automation |
| ⚙️ **Hammond** | Aspire Expert | .NET Aspire orchestration, service startup ordering, health checks |
| 📋 **Scribe** | Session Logger | Decision tracking, cross-agent context, history management |
| 🔄 **Ralph** | Work Monitor | Work queue tracking, backlog monitoring |

Each agent has:
- A **charter** that defines what they can and cannot touch
- **Persistent memory** — they remember past decisions and can reference them
- Access to **shared decisions** — a living document that records architectural choices

The agents don't just generate code in isolation. They coordinate. When Arnold designs a DTO, Sattler knows the shape of the data her components will consume. When Hammond configures Aspire orchestration, Malcolm reviews the service topology. When Grant changes the AI backend, Muldoon writes tests to verify the new behavior.

> 💡 **Insight:** The character names are easter eggs — they don't change how the agents behave. But they make the team *memorable*. When you say "Arnold built the API layer," it sticks in a way that "Agent 2 handled backend tasks" never would.

---

## 3. First Build: The Foundation

The first commit (`262d55a`) delivered a working application. Not a scaffold. Not a skeleton. A running system with data, UI, and orchestration.

Here's what the team built in parallel:

**Arnold** (Backend) created the API from scratch:
- Three-layer architecture: Controllers → Services → DbContext
- Entity Framework Core with SQLite (`parks.db`)
- Many-to-many relationship: Parks ↔ Activities via explicit join table
- 67 Ontario Provincial Parks seeded from `seed-data/parks.json`
- Paginated endpoints (page 1, 12 items, max 100) with `PagedResultDto<T>`
- Search by name/description, filter by activities with "any" and "all" modes
- Swagger/OpenAPI — enabled in Development only

**Sattler** (Frontend) built two complete UIs:
- **Blazor** — Interactive Server mode with MudBlazor components, Leaflet map via JS interop, localStorage favorites, 300ms debounced search
- **React** — TypeScript + Vite + React Router, custom CSS (no component library), responsive mobile-first layout

**Hammond** (Aspire) orchestrated the whole thing:
- AppHost registering three services: API, Blazor, React (via `AddNpmApp`)
- Service discovery with `http://api` base address convention
- Startup ordering: `.WaitFor(api)` on Blazor to ensure the API is healthy before the frontend starts
- Health checks, telemetry, and OpenTelemetry pipeline to the Aspire Dashboard

**Grant** (AI) wired the initial AI endpoints:
- Four endpoints under `/api/ai`: park summaries, recommendations, chat, visit planner
- OpenAI integration via `Microsoft.Extensions.AI` abstractions
- Graceful degradation — AI endpoints return friendly messages when unconfigured, never throw

The architecture:

```
┌─────────────────────────────────────────────────────────────────┐
│                    ONTARIO PARKS EXPLORER                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────┐           ┌──────────────────┐           │
│  │   Blazor UI      │           │   React UI       │           │
│  │  (MudBlazor)     │           │  (TypeScript)    │           │
│  └────────┬─────────┘           └────────┬─────────┘           │
│           └──────────────┬───────────────┘                     │
│                    ┌─────▼─────────┐                           │
│                    │ ASP.NET Core  │                           │
│                    │   REST API    │                           │
│                    └─────┬────────┘                            │
│              ┌───────────┼──────────┐                          │
│        ┌─────▼──────┐ ┌──▼───────┐ ┌▼────────────┐            │
│        │  SQLite    │ │ GitHub  │ │ Health      │            │
│        │  (EF Core) │ │ Copilot │ │ Checks &   │            │
│        │            │ │ SDK     │ │ Metrics     │            │
│        └────────────┘ └─────────┘ └─────────────┘            │
│                                                                │
│                .NET Aspire Orchestration                       │
└─────────────────────────────────────────────────────────────────┘
```

![Aspire Dashboard showing 3 running services](../screenshots/08-aspire-dashboard.png)
*The Aspire Dashboard after `aspire run` — three healthy services, all discovered automatically.*

At this point, the app ran. You could browse parks, search, filter, view them on a map, and ask AI questions (if you had an OpenAI key configured). The foundation was solid.

Then Bruno changed the requirements.

---

## 4. The Pivot: From OpenAI to Copilot SDK

This is the chapter that matters.

Building something from scratch is one thing. Adapting to a scope change — mid-project, across multiple services, without starting over — is where teams prove themselves. Human or AI.

Bruno's prompt was clear:

> *"The AI chat feature should not use OpenAI APIs, it should use Microsoft Agent Framework and Copilot SDK Agents, so we can reuse the current Copilot installation."*

The reasoning was practical: why manage a separate OpenAI API key when every developer already has GitHub Copilot installed? Use what's already there.

**Grant** (AI Dev) took the lead:
- Removed `OpenAI` and `Microsoft.Extensions.AI.OpenAI` NuGet packages
- Added `Microsoft.Extensions.AI.CopilotSDK` (GitHub Copilot SDK)
- Replaced the `IChatClient` registration to use Copilot's token-based auth
- Updated `AiService` to work with the new provider — same interface, different backend
- No API key configuration needed anymore — Copilot SDK authenticates through the existing Copilot installation

The key insight: because Arnold had originally built the AI service behind a `IAiService` abstraction, and Grant had used `Microsoft.Extensions.AI` as the provider-agnostic layer, the swap was surgical. The controllers didn't change. The frontend didn't change. The tests didn't change (much).

But the prompt asked for more than an AI swap.

**Sattler** (Frontend) fixed the Blazor map:
- The map wasn't rendering — a timing bug. Leaflet was being initialized before the Blazor component had finished its interactive render
- Fix: adjusted the JS interop timing in `Map.razor` to ensure the DOM element existed before Leaflet attached to it
- No API key was needed (Leaflet + OpenStreetMap tiles are free) — but the prompt asked to verify that

**Arnold** (Backend) added a Settings page:
- New `/api/settings` endpoint reporting AI and map configuration status
- Sattler built the UI for it in both Blazor and React
- Lets users verify their setup without digging through config files

**Muldoon** (Tester) verified everything:
- 46 unit tests passing — AI service tests updated for the new provider
- E2E Playwright specs covering the full user journey
- Automated screenshot capture for documentation

All of this landed in commit `56f54de`: *"Replace OpenAI with Microsoft Agent Framework + GitHub Copilot SDK"*.

![AI Chat with Copilot SDK](../screenshots/05-chat.png)
*The AI chat interface — now powered by GitHub Copilot SDK instead of OpenAI.*

> 💡 **Insight:** The abstraction layers paid off. `Microsoft.Extensions.AI` made the AI provider a pluggable dependency. `IAiService` isolated the business logic from the transport. When the requirement changed, one agent swapped the provider while the rest of the team kept working. This is the same principle human teams use — the difference is that the AI team did it from a single prompt.

### What didn't work perfectly

Honesty matters in a postmortem. A few things required iteration:

- **Blazor screenshot timing** — Playwright had to wait for Leaflet tiles to finish loading before capturing the map screenshot. The first attempts captured a grey box. Muldoon added explicit waits for tile load events.
- **React CORS during development** — The React dev server (Vite on port 5173) needed CORS headers from the API. The initial `AllowAnyOrigin` policy worked but was flagged for production tightening. It's documented in decisions.md as a known dev-only configuration.
- **`AddViteApp` not available** — Hammond wanted to use Aspire's `AddViteApp` for the React project, but it wasn't available in `Aspire.Hosting.NodeJs 9.5.2`. Workaround: `AddNpmApp("react", "../OntarioParksExplorer.React", "dev")` — functional, slightly less elegant.

---

## 5. Polish & Documentation

After the pivot landed, the remaining prompts were about refinement:

> *"fix this, as is declared on the main readme, the command is invalid"*

The Aspire CLI install command was wrong. Commit `6040ecb` fixed it to use the official install script: `irm https://aspire.dev/install.ps1 | iex`.

> *"aspire run will show a full url with the token to do the aspire portal login"*

The docs said `dotnet run --project AppHost`. Bruno corrected it: `aspire run` is the canonical command, and it outputs the dashboard URL with an auth token. Commit `374025a`.

> *"all the docs should be in a folder named docs"*

Sattler reorganized. User Manual and Demo Script moved into `docs/`. Screenshots stayed in `screenshots/` at the repo root for easy relative linking. Commit `332c165`.

> *"redo the main readme as explaining that this is a sample E2E application created using Copilot CLI and SQUAD"*

Malcolm rewrote the README from a standard project readme into a showcase document. The original prompt was embedded. The team roster was displayed. Architecture diagram, screenshots, and getting-started instructions — all reframed as "here's what AI agents built." Commit `4b1d64d`.

**Muldoon's screenshot automation** deserves a callout. Instead of manually capturing screenshots, Playwright E2E tests were configured to:
1. Start the Aspire-orchestrated app
2. Navigate through every major page (Home, Parks, Map, Chat, Settings, Swagger)
3. Capture standardized 1280×800 screenshots
4. Save them to `screenshots/` with numbered filenames

Twelve screenshots. Zero manual effort. Repeatable on every build.

![Parks listing with search and filter](../screenshots/02-parks.png)
*Automated screenshot: the Parks page with search bar and activity filter chips.*

---

## 6. The Decisions Trail

AI agents don't just generate code — they make *decisions*. And those decisions are recorded in `decisions.md`, a shared document that any agent can reference and any human can audit.

Here are four entries that show the team thinking, not just typing:

### Decision: Three-Layer Architecture
> **Arnold, 2026-03-28** — Controllers (HTTP concerns) → Services (business logic) → DbContext (data access). Ensures clean separation of concerns, testability, and easy integration of cross-cutting concerns like caching.

*Why this matters:* This is the decision that made the OpenAI-to-Copilot pivot painless. Because the AI logic lived in a service layer behind an interface, swapping the provider didn't ripple through controllers or tests.

### Decision: Idempotent Database Seeding
> **Arnold, 2026-03-28** — Automatic seeding in Program.cs with idempotency check: `if (!await context.Parks.AnyAsync())`. Ensures database always seeded on first run without duplicate data on restart.

*Why this matters:* "Clone and run" is the developer experience goal. No manual migration commands. No seed scripts to remember. The app works on first launch.

### Decision: Graceful AI Degradation
> **Grant, 2026-03-28** — All AI methods check `_isConfigured` flag and return friendly messages when unconfigured. Try/catch with logging, never throw to caller.

*Why this matters:* The app works without AI configured. You can browse parks, use the map, save favorites — all without a Copilot SDK setup. AI is additive, not required.

### Decision: Aspire Startup Ordering
> **Hammond, 2026-03-28** — Added `.WaitFor(api)` to Blazor resource ensuring API starts first. Service discovery via `http://api` base address.

*Why this matters:* Without this, the Blazor frontend might start before the API is healthy, causing failed requests on initial page load. One line of orchestration config prevents a class of race conditions.

> 💡 **Insight:** The decisions trail is the strongest evidence that AI agents are doing more than autocomplete. They're making trade-offs, documenting rationale, and building on each other's choices. A human architect would make these same decisions — the agents just do it faster and write it down every time.

---

## 7. What's in the Box

The final repository contains:

| Component | Details |
|-----------|---------|
| **API** | ASP.NET Core REST API, 5 park endpoints + 4 AI endpoints, EF Core + SQLite |
| **Blazor Frontend** | MudBlazor UI, Interactive Server, Leaflet maps, favorites, AI chat |
| **React Frontend** | TypeScript + Vite, React Router, custom CSS, responsive design |
| **Aspire Orchestration** | AppHost with service discovery, health checks, telemetry dashboard |
| **Database** | SQLite with 67 Ontario parks, 20+ activities, many-to-many relationships |
| **Tests** | 46+ unit tests (xUnit), Playwright E2E specs with screenshot automation |
| **Documentation** | User Manual, Demo Script, this Journey doc, team decisions log |

### How to run it

```bash
git clone https://github.com/elbruno/CanadaParksTour.git
cd CanadaParksTour/OntarioParksExplorer

# Install React dependencies
cd OntarioParksExplorer.React && npm install && cd ..

# Start everything
aspire run
```

The Aspire Dashboard URL (with auth token) appears in the terminal output. From there, you can reach:

| Service | What you'll see |
|---------|----------------|
| **Aspire Dashboard** | Three healthy services, logs, metrics |
| **Blazor Frontend** | Full park explorer with map, search, AI chat |
| **React Frontend** | Same features, different UI framework |
| **API + Swagger** | Interactive API documentation |

### The commit timeline

| Commit | What happened |
|--------|---------------|
| `2a60baf` | Squad initialized — 8 agents assembled, charters defined |
| `262d55a` | Complete implementation — API, Blazor, React, AI, 67 parks seeded |
| `98676b6` | Screenshots captured via Playwright automation |
| `6040ecb` | Aspire CLI install command fixed (community feedback) |
| `374025a` | `aspire run` documented as canonical launch command |
| `56f54de` | **The pivot** — OpenAI replaced with Copilot SDK + Agent Framework |
| `332c165` | Docs reorganized into `docs/` folder |
| `4b1d64d` | README rewritten as Copilot CLI + Squad showcase |
| `f95db0a` | Scribe merged decision inbox, updated agent histories |

---

## The Takeaway

This project started as an experiment: *can a team of AI agents, coordinated by Squad and powered by GitHub Copilot CLI, build a real application from natural language?*

The answer is yes — with caveats.

**What worked well:**
- **Parallel specialization** — agents working simultaneously on backend, frontend, orchestration, and testing, just like a human team
- **Persistent memory** — agents didn't lose context between sessions; they remembered past decisions and built on them
- **Abstraction discipline** — the three-layer architecture and provider-agnostic AI abstractions made the mid-project pivot possible
- **Decision documentation** — every architectural choice was recorded, creating an audit trail that most human teams don't maintain

**What required human guidance:**
- **Scope direction** — Bruno provided the prompts that set priorities and made judgment calls ("use Copilot SDK, not OpenAI")
- **Correctness checks** — the Aspire CLI install command was wrong until a human caught it
- **Quality bar** — "take screenshots and create a user manual" was a human requirement, not something the agents would have prioritized on their own

**The honest summary:**
The AI team is fast, thorough, and never forgets a decision. But it still needs a human to say what to build and when to ship. The prompt is the product specification. The human is the product manager.

Nine commits. Eight agents. One terminal. One person driving.

That's the journey.

---

*This document was written by Malcolm (Lead/Architect) as part of the Squad team's documentation effort. The git history, decisions log, and agent memory files are the primary sources. No facts were hallucinated — though Malcolm would note that "life finds a way" applies to software projects too.*
