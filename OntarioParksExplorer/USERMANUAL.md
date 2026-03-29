# Ontario Parks Explorer — User Manual

## Table of Contents

1. [Overview](#overview)
2. [Getting Started](#getting-started)
3. [Home Page](#home-page)
4. [Browse Parks](#browse-parks)
5. [Park Details](#park-details)
6. [Interactive Map](#interactive-map)
7. [AI Chat Assistant](#ai-chat-assistant)
8. [Favorites](#favorites)
9. [Settings](#settings)
10. [AI Features](#ai-features)
11. [Troubleshooting](#troubleshooting)

---

## Overview

Ontario Parks Explorer is a full-stack web application for discovering, exploring, and planning visits to Ontario Provincial Parks. It features:

- **Browse & Search** — Filter parks by region, activities, and keywords
- **Interactive Maps** — View all parks on a Leaflet-powered map
- **AI-Powered Chat** — Ask questions about parks using the AI assistant
- **Visit Planning** — Get AI-generated day-by-day itineraries
- **Park Recommendations** — Personalized suggestions based on your preferences
- **Favorites** — Save parks you love for quick access

The application is built with **.NET Aspire**, providing a **Blazor Server** frontend, **React** frontend, and **ASP.NET Core API** backend, all orchestrated together.

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 18+ (for React frontend)
- Aspire CLI (`irm https://aspire.dev/install.ps1 | iex`)
- GitHub Copilot CLI (for AI features)

### Starting the Application

```bash
cd OntarioParksExplorer
aspire run
```

This starts all services:
- **Aspire Dashboard** — Service health and metrics
- **API** — Backend REST API
- **Blazor App** — Primary web frontend
- **React App** — Alternative web frontend

Open the Aspire Dashboard URL shown in the terminal to access all service endpoints.

---

## Home Page

The Home page (`/`) is the entry point for the application. It provides:

- **Welcome banner** with app description
- **Quick navigation cards** linking to:
  - **Browse Parks** — Full park listing with search and filters
  - **View Map** — Interactive map of all parks
  - **AI Chat** — Conversational AI assistant
- **Recommendations widget** — AI-suggested parks based on popular activities

---

## Browse Parks

Navigate to **Parks** (`/parks`) to browse the complete list of Ontario Provincial Parks.

### Features

- **Search** — Type keywords to search by park name or description
- **Region Filter** — Filter parks by geographic region (Northern, Southern, etc.)
- **Activity Filter** — Filter by available activities (Hiking, Camping, Swimming, etc.)
- **Pagination** — Browse through results page by page
- **Park Cards** — Each card shows:
  - Park name and region
  - Brief description
  - Featured badge (for highlighted parks)
  - "View Details" link

### Tips

- Use the search bar at the top for quick lookups
- Combine region and activity filters for precise results
- Click any park card to see full details

---

## Park Details

Click on any park to view its detail page (`/parks/{id}`).

### Information Displayed

- **Park name and region**
- **Description** — Full park description
- **AI Summary** — AI-generated engaging overview (auto-generated on first visit)
- **Activities** — Full list of available activities with icons
- **Location** — GPS coordinates and map preview
- **Website link** — Direct link to the official park page
- **Favorites button** — Add/remove from your favorites list
- **Visit Planner** — AI-powered itinerary generator

### Visit Planner

1. Select the number of days (1–14)
2. Choose your interests (e.g., Hiking, Photography, Wildlife)
3. Optionally select a season
4. Click "Plan My Visit"
5. Receive a day-by-day itinerary with activities and practical tips

---

## Interactive Map

Navigate to **Map** (`/map`) to see all parks plotted on an interactive map.

### Features

- **OpenStreetMap tiles** — Free, high-quality map tiles (no API key required)
- **Park markers** — Each park is shown as a marker at its GPS location
- **Popups** — Click a marker to see:
  - Park name
  - Region
  - "View Details" link
- **Zoom & Pan** — Standard Leaflet controls for navigation
- **Tooltips** — Hover over markers to see park names

### Map Center

The map is centered on Ontario at coordinates (50.0°N, 85.0°W) with zoom level 5.

---

## AI Chat Assistant

Navigate to **AI Chat** (`/chat`) to interact with the AI parks assistant.

### How to Use

1. Type your question in the input field
2. Press **Enter** or click **Send**
3. Wait for the AI response
4. Continue the conversation — the assistant remembers context from previous messages

### Example Questions

- "What parks are best for hiking in northern Ontario?"
- "Tell me about Algonquin Provincial Park"
- "What activities can I do at Killarney?"
- "Recommend a park for a family camping trip"
- "What's the best time to visit Pukaskwa?"

### Features

- **Conversation history** — The AI maintains context across messages (up to 10 turns)
- **Context-aware** — The assistant loads relevant park data based on your question
- **Clear chat** — Click "Clear Chat" to start a fresh conversation
- **Keyboard shortcut** — Press Shift+Enter for a new line without sending

### AI Backend

The chat is powered by the **GitHub Copilot SDK** through the **Microsoft Agent Framework**. It uses your existing GitHub Copilot installation — no separate API keys required.

---

## Favorites

Navigate to **Favorites** (`/favorites`) to view your saved parks.

### How It Works

- Click the heart/star icon on any park card or detail page to add it to favorites
- Favorites are stored in your browser's localStorage
- Remove parks from favorites by clicking the icon again
- Favorites persist across browser sessions (same device only)

---

## Settings

Navigate to **Settings** (`/settings`) to view application configuration and status.

### Configuration Sections

#### AI Configuration
- **Provider** — Shows "GitHub Copilot SDK"
- **Model** — Current AI model (default or configured)
- **Status** — Connected or Not Connected
- **API Key Required** — No (uses Copilot CLI)

If AI status shows "Not Connected", ensure:
1. GitHub Copilot CLI is installed
2. You are authenticated (`copilot auth login`)
3. Your Copilot subscription is active

#### Map Configuration
- **Tile Provider** — OpenStreetMap (free, no key needed)
- **API Key Required** — No
- **Status** — Active

#### Application Info
- Application version
- Framework information (.NET 10 with .NET Aspire)
- AI backend (Microsoft Agent Framework + GitHub Copilot SDK)
- Frontend technologies (Blazor Server + React)

---

## AI Features

### How AI Works

The application uses the **Microsoft Agent Framework** with the **GitHub Copilot SDK** as the AI backend. This means:

1. **No separate API keys** — Uses your existing GitHub Copilot installation
2. **Copilot CLI as backend** — The SDK communicates with the Copilot CLI via JSON-RPC
3. **Automatic model selection** — Uses the best available model from your Copilot subscription
4. **Graceful degradation** — If Copilot is not available, all features still work with informational messages

### AI-Powered Features

| Feature | Endpoint | Description |
|---------|----------|-------------|
| Park Summaries | `POST /api/ai/parks/{id}/summary` | AI-generated engaging park overviews |
| Recommendations | `POST /api/ai/recommendations` | Personalized park suggestions |
| Chat | `POST /api/ai/chat` | Conversational Q&A about parks |
| Visit Planning | `POST /api/ai/plan-visit` | Day-by-day itinerary generation |

### Caching

- Park summaries are cached for 24 hours to reduce API calls
- Subsequent requests for the same park summary return cached results instantly

---

## Troubleshooting

### AI Features Not Working

**Symptoms:** AI responses show "AI features are not configured" messages.

**Solutions:**
1. Install GitHub Copilot CLI: `gh extension install github/gh-copilot`
2. Authenticate: `copilot auth login`
3. Check Settings page for AI status
4. Restart the application after authentication

### Map Not Loading

**Symptoms:** Map page shows empty area or loading skeleton.

**Solutions:**
1. Check internet connection (map tiles load from OpenStreetMap CDN)
2. Clear browser cache and refresh
3. Check browser console for JavaScript errors
4. Verify the Blazor app has connected (look for the SignalR connection indicator)

### Application Won't Start

**Symptoms:** `aspire run` fails with errors.

**Solutions:**
1. Kill any orphaned Aspire processes: Check Task Manager for `aspire` or `dcp` processes
2. Ensure .NET 10 SDK is installed: `dotnet --version`
3. Ensure Aspire CLI is installed: `aspire --version`
4. Try `dotnet restore` in the `OntarioParksExplorer` folder
5. Check Aspire logs at `~/.aspire/logs/`

### Port Conflicts

If ports are already in use:
- **Blazor (7001):** Check `launchSettings.json` in Blazor project
- **React (5173):** Set `PORT` environment variable
- **API (7002):** Check `launchSettings.json` in API project
- **Aspire Dashboard:** Aspire auto-selects another port if busy

---

*Ontario Parks Explorer v1.0.0 — Built with .NET Aspire, Blazor, React, and GitHub Copilot SDK*
