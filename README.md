# Ontario Parks Explorer

Explore Ontario Provincial Parks with AI-powered insights, intelligent recommendations, and interactive maps.

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=.net) ![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8-512BD4) ![Blazor](https://img.shields.io/badge/Blazor-Interactive-7952B3) ![React](https://img.shields.io/badge/React-19-61DAFB?logo=react) ![SQLite](https://img.shields.io/badge/SQLite-Latest-003B57?logo=sqlite) ![.NET Aspire](https://img.shields.io/badge/.NET%20Aspire-Orchestration-512BD4)

## Overview

Ontario Parks Explorer is a full-stack application for discovering, exploring, and planning visits to Ontario Provincial Parks. The application combines traditional park discovery features (search, filter, map visualization) with AI-powered features like intelligent summaries, personalized recommendations, and an interactive park planning assistant.

### Features

- **Park Discovery**: Browse and paginate through Ontario Provincial Parks with featured highlights
- **Search & Filter**: Search parks by name/description and filter by activities (hiking, camping, fishing, etc.)
- **Interactive Maps**: View park locations with Leaflet-based map visualization
- **Park Details**: Comprehensive information including activities, contact info, coordinates, and images
- **Favorites Management**: Save favorite parks for quick access (client-side)
- **AI Features**:
  - **Park Summaries**: Generate engaging, AI-powered summaries for any park
  - **Smart Recommendations**: Get personalized park recommendations based on your preferences and interests
  - **AI Chat Assistant**: Ask questions about Ontario parks and get intelligent responses
  - **Visit Planner**: Generate day-by-day visit plans tailored to your duration, interests, and season
- **Dual Frontends**: Choose between interactive Blazor or modern React interface — same data, same features
- **Modern Architecture**: Orchestrated with .NET Aspire, microservice-ready design

---

## Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                      ONTARIO PARKS EXPLORER                              │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                           │
│  ┌──────────────────────┐         ┌──────────────────────┐               │
│  │   Blazor UI          │         │   React UI           │               │
│  │  (MudBlazor)         │         │  (TypeScript/Vite)   │               │
│  │                      │         │                      │               │
│  │ • Park List          │         │ • Park Grid          │               │
│  │ • Search & Filter    │         │ • Map Integration    │               │
│  │ • Favorites          │         │ • Chat Interface     │               │
│  │ • AI Features        │         │ • Recommendations    │               │
│  └──────────┬───────────┘         └──────────┬───────────┘               │
│             │                                │                           │
│             └────────────────┬───────────────┘                           │
│                              │                                           │
│                    ┌─────────▼──────────┐                               │
│                    │  ASP.NET Core API  │                               │
│                    │  (REST Endpoints)  │                               │
│                    │                    │                               │
│                    │ • Parks Endpoints  │                               │
│                    │ • Activities List  │                               │
│                    │ • AI Services      │                               │
│                    └─────────┬──────────┘                               │
│                              │                                           │
│              ┌───────────────┼───────────────┐                          │
│              │               │               │                          │
│       ┌──────▼────────┐  ┌───▼────────┐ ┌──▼────────────┐              │
│       │    SQLite     │  │  GitHub   │ │ Health Checks │              │
│       │   Database    │  │  Copilot  │ │ & Metrics    │              │
│       │  (EF Core)    │  │  SDK      │ │              │              │
│       └───────────────┘  └────────────┘ └──────────────┘              │
│                                                                         │
│                    .NET Aspire Orchestration                           │
│                    (Service Discovery & Health)                        │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### Tech Stack

**Backend:**
- **.NET 10** — Latest .NET runtime
- **ASP.NET Core** — High-performance web framework
- **Entity Framework Core** — ORM for data access
- **SQLite** — Lightweight embedded database
- **.NET Aspire** — Orchestration and service discovery

**Frontends:**
- **Blazor Web** — Interactive server-rendered UI with MudBlazor components
- **React 19** — Modern SPA with TypeScript
- **Vite** — Lightning-fast build tool for React
- **Leaflet** — Interactive map visualization
- **React Router** — Client-side routing

**AI & Services:**
- **Microsoft Agent Framework** — Standardized AI agent runtime
- **GitHub Copilot SDK** — AI backend powered by your existing Copilot installation
- **Aspire Dashboard** — Real-time monitoring and service health

---

## Prerequisites

- **.NET 10 SDK** ([download](https://dotnet.microsoft.com/download))
- **Node.js 18+** ([download](https://nodejs.org/))
- **Aspire CLI** (optional, installed automatically with .NET 10)

---

## Getting Started

### 1. Clone and Navigate

```bash
git clone https://github.com/elbruno/CanadaParksTour.git
cd OntarioParksExplorer
```

### 2. Install Dependencies

```bash
# Install Aspire CLI (one-time) — see https://aspire.dev/install
irm https://aspire.dev/install.ps1 | iex

# Install Node dependencies for React frontend
cd OntarioParksExplorer/OntarioParksExplorer.React
npm install
cd ../..
```

### 3. Configure AI (Optional)

AI features use the **GitHub Copilot SDK** and require the Copilot CLI to be installed and authenticated:

```bash
# Install Copilot CLI (if not already installed via GitHub CLI)
gh extension install github/gh-copilot

# Authenticate
copilot auth login
```

Optionally set a preferred model in `OntarioParksExplorer/OntarioParksExplorer.Api/appsettings.json`:

```json
{
  "AI": {
    "Provider": "GitHubCopilot",
    "Model": ""
  }
}
```

If the Copilot CLI is not available, AI endpoints will gracefully degrade with informational messages.

### 4. Run the Application

**Using Aspire CLI (Recommended):**

```bash
aspire run
```

**What launches:**
- **Aspire Dashboard** → https://localhost:17236 (service monitoring)
- **Blazor App** → https://localhost:7001 (interactive server UI)
- **React App** → https://localhost:5173 (modern SPA)
- **API** → https://localhost:7002 (REST endpoints)

### 5. Access the Dashboards

| Service | URL | Purpose |
|---------|-----|---------|
| **Aspire Dashboard** | https://localhost:17236 | Monitor services, health, and logs |
| **Blazor Frontend** | https://localhost:7001 | Interactive park explorer (server-rendered) |
| **React Frontend** | https://localhost:5173 | Modern park explorer (SPA) |
| **Swagger API Docs** | https://localhost:7002/swagger | API documentation and testing |

---

## API Endpoints

### Parks

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/parks` | List all parks (paginated, 12 per page) |
| `GET` | `/api/parks/{id}` | Get detailed park information |
| `GET` | `/api/parks/search?q=query` | Search parks by name or description |
| `GET` | `/api/parks/filter?activities=hiking,camping&mode=any` | Filter by activities (any/all mode) |
| `GET` | `/api/activities` | List all available activities |

**Query Parameters:**
- `page` — Page number (default: 1)
- `pageSize` — Items per page (default: 12, max: 100)
- `q` — Search query
- `activities` — Comma-separated activity names
- `mode` — Filter mode: `any` (at least one activity) or `all` (all activities)

### AI Features

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/ai/parks/{id}/summary` | Generate AI summary for a park |
| `POST` | `/api/ai/recommendations` | Get personalized park recommendations |
| `POST` | `/api/ai/chat` | Chat with AI about parks |
| `POST` | `/api/ai/plan-visit` | Generate a visit plan for a park |

**Request Bodies:**

**Recommendations:**
```json
{
  "activities": ["hiking", "camping"],
  "region": "Muskoka",
  "preferenceText": "I love scenic views and wildlife photography"
}
```

**Chat:**
```json
{
  "message": "What's the best time to visit Ontario parks for hiking?"
}
```

**Visit Plan:**
```json
{
  "parkId": 1,
  "durationDays": 3,
  "interests": "hiking, photography",
  "season": "summer"
}
```

---

## AI Features

### Enabling AI Services

All AI features are powered by the **GitHub Copilot SDK** using the **Microsoft Agent Framework**. No separate API keys are needed — the app uses your existing GitHub Copilot installation.

**Requirements:**
- GitHub Copilot CLI installed and authenticated
- Active GitHub Copilot subscription (includes free tier)

**Configuration** (optional — in `appsettings.json`):
```json
{
  "AI": {
    "Provider": "GitHubCopilot",
    "Model": ""
  }
}
```

> **Tip:** You can also set the model via the `GITHUB_COPILOT_MODEL` environment variable. Leave empty to use the default model.

### Available AI Features

#### 1. **Park Summaries**
Generate engaging, AI-crafted summaries for any park.
- **Endpoint:** `POST /api/ai/parks/{id}/summary`
- **Returns:** Plain text summary (200-300 words)

#### 2. **Recommendations**
Get personalized park suggestions based on activities, region, and interests.
- **Endpoint:** `POST /api/ai/recommendations`
- **Input:** Activities list, region preference, free-form preference text
- **Returns:** Ranked list of parks with match scores and reasons

#### 3. **Chat Assistant**
Ask questions about Ontario parks and get intelligent answers.
- **Endpoint:** `POST /api/ai/chat`
- **Input:** User message (optional conversation history)
- **Returns:** AI-generated response

#### 4. **Visit Planner**
Generate customized day-by-day visit plans.
- **Endpoint:** `POST /api/ai/plan-visit`
- **Input:** Park ID, duration (1-14 days), interests, season
- **Returns:** Structured visit plan with daily activities and tips

---

## Project Structure

```
OntarioParksExplorer/
├── OntarioParksExplorer.sln                 # Solution file
├── OntarioParksExplorer.AppHost/            # Aspire orchestration host
│   ├── AppHost.cs                           # Service registration
│   └── appsettings.json                     # Aspire config
├── OntarioParksExplorer.Api/                # ASP.NET Core REST API
│   ├── Controllers/                         # API endpoints
│   │   ├── ParksController.cs               # Park operations
│   │   ├── ActivitiesController.cs          # Activities listing
│   │   └── AiController.cs                  # AI features
│   ├── Services/                            # Business logic
│   │   ├── ParksService.cs                  # Park data service
│   │   └── AI/
│   │       └── AiService.cs                 # AI integration service
│   ├── Data/                                # Database
│   │   ├── ParksDbContext.cs                # EF Core context
│   │   └── DataSeeder.cs                    # Seed data loading
│   ├── Models/                              # Entity models
│   │   └── DTOs/                            # Data transfer objects
│   ├── Program.cs                           # Configuration
│   └── parks.db                             # SQLite database
├── OntarioParksExplorer.Blazor/             # Blazor Web UI
│   ├── Components/                          # Blazor components
│   ├── Services/                            # Client-side services
│   ├── Program.cs                           # Blazor configuration
│   └── wwwroot/                             # Static assets
├── OntarioParksExplorer.React/              # React SPA
│   ├── src/
│   │   ├── pages/                           # Page components
│   │   ├── components/                      # Reusable components
│   │   ├── services/                        # API client services
│   │   └── App.tsx                          # Root component
│   ├── vite.config.ts                       # Vite configuration
│   ├── tsconfig.json                        # TypeScript config
│   └── package.json                         # Dependencies
├── OntarioParksExplorer.ServiceDefaults/    # Shared Aspire config
│   └── Extensions.cs                        # Health checks & telemetry
├── seed-data/                               # Database seed data
│   └── parks.json                           # Sample park data
└── aspire.config.json                       # Aspire configuration

```

---

## Running Tests

### Run All Tests

```bash
cd OntarioParksExplorer
dotnet test
```

### Run Specific Test Project

```bash
dotnet test OntarioParksExplorer.Api.Tests
```

### Run with Verbose Output

```bash
dotnet test --verbosity detailed
```

---

## Development

### Building the Solution

```bash
cd OntarioParksExplorer
dotnet build
```

### Building Just the API

```bash
dotnet build OntarioParksExplorer.Api
```

### Building the React Frontend

```bash
cd OntarioParksExplorer/OntarioParksExplorer.React
npm run build
```

### Watching for Changes

**Blazor (automatic with Aspire):**
```bash
dotnet watch
```

**React:**
```bash
cd OntarioParksExplorer/OntarioParksExplorer.React
npm run dev
```

---

## Database

The application uses **SQLite** with **Entity Framework Core**. The database is automatically seeded from `seed-data/parks.json` on first run.

### Database Migrations

```bash
# Create a new migration
dotnet ef migrations add MigrationName --project OntarioParksExplorer.Api

# Apply migrations
dotnet ef database update --project OntarioParksExplorer.Api

# Remove last migration
dotnet ef migrations remove --project OntarioParksExplorer.Api
```

---

## Configuration

### appsettings.json (API)

```json
{
  "ConnectionStrings": {
    "ParksDb": "Data Source=parks.db"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173", "https://localhost:5173"]
  },
  "AI": {
    "Provider": "GitHubCopilot",
    "Model": ""
  }
}
```

---

## Troubleshooting

### Port Conflicts

If ports are already in use:
- **Blazor (7001):** Check `launchSettings.json` in Blazor project
- **React (5173):** Set `PORT` environment variable
- **API (7002):** Check `launchSettings.json` in API project
- **Aspire Dashboard (17236):** Aspire will auto-select another port if busy

### AI Features Not Working

- Verify GitHub Copilot CLI is installed (`copilot --version`)
- Ensure you are authenticated (`copilot auth login`)
- Check the Settings page in the Blazor app for AI status
- Check API logs in Aspire Dashboard for errors
- Verify your GitHub Copilot subscription is active

### Database Issues

```bash
# Reset database (remove and recreate)
rm OntarioParksExplorer/OntarioParksExplorer.Api/parks.db
aspire run
```

### Aspire CLI Not Found

```powershell
# PowerShell
irm https://aspire.dev/install.ps1 | iex

# Bash
curl -sSL https://aspire.dev/install.sh | bash
```

---

## License

MIT License — See LICENSE file for details.

---

## Support

For issues, questions, or contributions, please open an issue on the [GitHub repository](https://github.com/bcapuano/OntarioParksExplorer).

---

**Last Updated:** 2026-04-11  
**Maintained by:** Ontario Parks Explorer Team
