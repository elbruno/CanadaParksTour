# Ontario Parks Explorer — Demo Script

A guided walkthrough of the Ontario Parks Explorer application, showcasing all features and capabilities.

**Estimated Duration:** 10-15 minutes  
**Prerequisites:** Application running with `aspire run` or `aspire start`

---

## Demo Outline

1. **Starting the Application & Aspire Dashboard** (1 min)
2. **Exploring the Blazor UI** (4 min)
3. **Using API via Swagger** (2 min)
4. **Switching to React Frontend** (2 min)
5. **AI Features Showcase** (4 min)
6. **Health & Monitoring** (1 min)

---

## Part 1: Starting the Application & Aspire Dashboard

### Launch the Application

```bash
cd C:\src\CanadaParksTour\OntarioParksExplorer
aspire run
```

**Expected Output:**
```
Building Aspire Application...
Aspire Dashboard running on https://localhost:17236
Starting API service...
Starting Blazor service...
Starting React service...

✓ API running at https://localhost:7002
✓ Blazor running at https://localhost:7001
✓ React running at https://localhost:5173
```

### Open Aspire Dashboard

1. Navigate to **https://localhost:17236** in your browser
2. You'll see the **Aspire Dashboard** with:
   - **Resources:** api, blazor, react (all showing green/healthy)
   - **Logs:** Real-time service logs from all running services
   - **Traces:** Distributed tracing across services
   - **Metrics:** Performance indicators

**What to show:**
- Click on **"api"** resource → shows API logs and health checks
- Click on **"blazor"** resource → shows Blazor service activity
- Click on **"react"** resource → shows Node.js/npm dev server logs

---

## Part 2: Exploring the Blazor UI (4 minutes)

### Navigate to Blazor Frontend

1. Go to **https://localhost:7001** (Blazor app)
2. Demonstrate the interactive Blazor interface with MudBlazor components

### Feature Demo: Browsing Parks

**Screenshot/Demo:**
1. **Home Page** — Shows featured parks in a grid/list
   - Parks are paginated (12 per page by default)
   - Each park card shows: name, region, featured badge, activity icons
   
2. **Click a park card** → Navigate to park details page showing:
   - Park name, description
   - Activities available (hiking, camping, fishing, etc.)
   - Contact information & coordinates
   - Park images/gallery

### Feature Demo: Search

1. **Use the search bar** at the top of the page
   - Type: `"Algonquin"` → Shows all parks with "Algonquin" in name or description
   - Results update instantly with pagination
   - Click any result to view full park details

### Feature Demo: Filter by Activities

1. **Use the activity filter** (sidebar or dropdown)
   - Select **"Hiking"** → Shows all parks with hiking
   - Select **"Hiking" + "Camping"** (all mode) → Shows parks with both activities
   - Toggle between **"Any"** and **"All"** filter modes
   
   **Expected Result:** Different park lists based on selection logic

### Feature Demo: Favorites

1. **Click the heart icon** on any park card → Park is added to favorites
2. **View Favorites** section (if implemented) → Shows saved parks
3. **Click heart again** → Removes from favorites
   - Changes are persisted in browser local storage

### Feature Demo: Park Details & Map

1. **Click any park** → Open detailed park view
2. **View Activities:**
   - List of all activities available (water sports, winter activities, etc.)
   - Search/filter activities if needed
   
3. **View Map (if implemented):**
   - Interactive Leaflet map showing park location
   - Click/zoom to explore surroundings

---

## Part 3: Using API via Swagger (2 minutes)

### Open Swagger Documentation

1. Navigate to **https://localhost:7002/swagger** (API Swagger UI)
2. Explore available endpoints organized by resource:
   - **Parks** — Park listing and details
   - **Activities** — Activity management
   - **AI** — AI-powered features

### Try API Endpoints

#### 3.1: List Parks

1. Expand **Parks** → `GET /api/parks`
2. Click **"Try it out"**
3. **Query Parameters:**
   - `page`: 1
   - `pageSize`: 12
4. Click **Execute**
5. **Expected Response:** 
   ```json
   {
     "data": [
       { "id": 1, "name": "Algonquin Park", "region": "Muskoka", ... },
       ...
     ],
     "totalCount": 67,
     "pageSize": 12,
     "currentPage": 1,
     "totalPages": 6
   }
   ```

#### 3.2: Get Park Details

1. Expand **Parks** → `GET /api/parks/{id}`
2. Click **"Try it out"**
3. **Set ID:** `1`
4. Click **Execute**
5. **Expected Response:**
   ```json
   {
     "id": 1,
     "name": "Algonquin Park",
     "description": "...",
     "latitude": 45.5,
     "longitude": -79.5,
     "activities": [...],
     "contact": "..."
   }
   ```

#### 3.3: Search Parks

1. Expand **Parks** → `GET /api/parks/search`
2. Click **"Try it out"**
3. **Set Query:** `q=Algonquin&page=1&pageSize=12`
4. Click **Execute**
5. **Show:** Search results filtered by query string

#### 3.4: Filter by Activities

1. Expand **Parks** → `GET /api/parks/filter`
2. Click **"Try it out"**
3. **Set Query:** `activities=Hiking,Camping&mode=any&page=1&pageSize=12`
4. Click **Execute**
5. **Show:** Parks with hiking OR camping activities

#### 3.5: List Activities

1. Expand **Activities** → `GET /api/activities`
2. Click **Execute**
3. **Show:** Alphabetically sorted list of all available activities
   ```json
   [
     { "id": 1, "name": "Hiking" },
     { "id": 2, "name": "Camping" },
     { "id": 3, "name": "Fishing" },
     ...
   ]
   ```

---

## Part 4: Switching to React Frontend (2 minutes)

### Navigate to React App

1. Go to **https://localhost:5173** (React app)
2. **Note:** Identical features as Blazor, different UI framework
   - Modern SPA (Single Page Application) built with React 19
   - TypeScript for type safety
   - Vite for fast development

### Demo React Features

1. **Park Grid** — Same parks, modern card layout
2. **Search** — Type in search bar → Parks filter in real-time
3. **Filter** — Select activities → Results update instantly
4. **Map** — Click a park → Leaflet map shows location
5. **Responsive Design** — Resize browser → Notice responsive layout

**Key Difference:** React is a client-side SPA, Blazor is server-rendered  
**Same Data:** Both frontends consume the same API endpoints

---

## Part 5: AI Features Showcase (4 minutes)

### IMPORTANT: AI Requirements

**Before showing AI features:**
- Verify the **Copilot CLI** is installed and authenticated (`copilot auth login`)
- If the Copilot CLI is not available, AI endpoints will return graceful fallback messages

### Feature 1: Park Summary Generation

**Endpoint:** `POST /api/ai/parks/{id}/summary`

**Demo Steps (Using Swagger):**
1. Go to **https://localhost:7002/swagger**
2. Expand **AI** → `POST /api/ai/parks/{id}/summary`
3. Click **"Try it out"**
4. **Set ID:** `1` (Algonquin Park)
5. Click **Execute**
6. **Result:** AI-generated 200-300 word summary of the park
   ```
   "Algonquin Park is one of Ontario's most iconic destinations, 
    spanning over 7,500 square kilometers of pristine wilderness. 
    Known for its exceptional hiking trails, crystal-clear lakes, 
    and abundant wildlife including moose and wolf populations, 
    Algonquin offers both adventure and tranquility..."
   ```

**Show in UI:**
- If implemented in Blazor/React: Click "Generate Summary" button on park details page
- Summary displays in a modal/card below park info

### Feature 2: Personalized Recommendations

**Endpoint:** `POST /api/ai/recommendations`

**Demo Steps (Using Swagger):**
1. Expand **AI** → `POST /api/ai/recommendations`
2. Click **"Try it out"**
3. **Request Body:**
   ```json
   {
     "activities": ["Hiking", "Photography"],
     "region": "Muskoka",
     "preferenceText": "I love scenic views and wildlife"
   }
   ```
4. Click **Execute**
5. **Result:** Ranked list of recommended parks
   ```json
   [
     {
       "parkId": 1,
       "name": "Algonquin Park",
       "matchScore": 95,
       "reason": "Outstanding hiking trails with wildlife viewing opportunities..."
     },
     {
       "parkId": 5,
       "name": "Huntsville State Park",
       "matchScore": 87,
       "reason": "Excellent photography spots with scenic lake views..."
     }
   ]
   ```

**Show in UI:**
- If implemented: "Get Recommendations" form on home page
- User fills in preferences → Results displayed with explanations

### Feature 3: Park Planning Assistant (Chat)

**Endpoint:** `POST /api/ai/chat`

**Demo Steps (Using Swagger):**
1. Expand **AI** → `POST /api/ai/chat`
2. Click **"Try it out"**
3. **Request Body:**
   ```json
   {
     "message": "What's the best time to visit Algonquin Park for wildlife watching?"
   }
   ```
4. Click **Execute**
5. **Result:** AI response
   ```
   "The best time to visit Algonquin Park for wildlife watching is 
    late spring (May-June) and early fall (September-October). 
    During these seasons, moose are particularly active, especially 
    at dusk around lakes and water bodies. June is ideal for seeing 
    loon nesting and hearing their haunting calls..."
   ```

**Show in UI:**
- If implemented: Chat interface at bottom of page or sidebar
- User types questions → AI responds with park information

### Feature 4: AI-Powered Visit Planner

**Endpoint:** `POST /api/ai/plan-visit`

**Demo Steps (Using Swagger):**
1. Expand **AI** → `POST /api/ai/plan-visit`
2. Click **"Try it out"**
3. **Request Body:**
   ```json
   {
     "parkId": 1,
     "durationDays": 3,
     "interests": "hiking, wildlife photography, camping",
     "season": "summer"
   }
   ```
4. Click **Execute**
5. **Result:** Day-by-day visit plan
   ```json
   {
     "parkName": "Algonquin Park",
     "duration": 3,
     "plan": [
       {
         "day": 1,
         "activities": ["Set up base camp", "Scenic loop hike (5km)", "Lake kayaking"],
         "tips": "Arrive early to secure a good campsite. Start hiking in afternoon to avoid crowds."
       },
       {
         "day": 2,
         "activities": ["Sunrise wildlife viewing", "Photography workshop trails", "Canoe adventure"],
         "tips": "Wake up at 5 AM for best light. Bring binoculars for moose viewing."
       },
       {
         "day": 3,
         "activities": ["Breakfast at the lake", "Interpretive nature walk", "Departure"],
         "tips": "Take the scenic route home to spot additional wildlife."
       }
     ],
     "packing": ["Tent", "Hiking boots", "Camera", "Binoculars", "Weather-appropriate clothing"]
   }
   ```

**Show in UI:**
- If implemented: "Plan Your Visit" modal/form on park details page
- Displays interactive visit plan with daily breakdown

---

## Part 6: Health & Monitoring (1 minute)

### View Health Status

**Option 1: Aspire Dashboard**
1. Go to **https://localhost:17236**
2. Each resource shows a **Health** indicator:
   - 🟢 **Green** — Service healthy
   - 🟡 **Yellow** — Service degraded
   - 🔴 **Red** — Service down

**Option 2: Direct Health Check**
```bash
curl https://localhost:7002/health
# Output: Healthy (200 OK)
```

### View Logs

1. **Aspire Dashboard** → Click any resource → **Logs** tab
2. Filter by time, severity level
3. Show API requests/responses in real-time as you interact with the app

### View Metrics

1. **Aspire Dashboard** → **Metrics** tab
2. Shows:
   - Request count and latency
   - Database query performance
   - AI service call metrics
   - Error rates

---

## Demo Scenarios & Talking Points

### Scenario 1: "I want a park with hiking and waterfalls"

1. **Go to Blazor UI** → Search "waterfall"
2. **Filter by "Hiking"** activity
3. **Show results** with parks matching both criteria
4. **Click a park** → View details and map
5. **Generate AI Summary** → See engaging description
6. **Use Chat feature** → Ask "Where are the best waterfalls for photography?"

### Scenario 2: "Plan a weekend camping trip"

1. **Blazor UI** → Filter by "Camping" activity
2. **Click a park** → View full details
3. **Use Visit Planner** → Generate 2-day camping plan
4. **Show map** → See park location and nearby facilities
5. **Get Recommendations** → Find similar parks

### Scenario 3: "Compare parks programmatically"

1. **Use Swagger API**
2. **Call `/api/parks/filter?activities=hiking,fishing&mode=all`**
3. **Call `/api/ai/recommendations`** with specific preferences
4. **Show** how external applications can integrate with the API

---

## Common Questions During Demo

### Q: "Which frontend should we use?"
**A:** Both are fully functional. Choose Blazor for server-rendered interactivity with .NET, or React for modern SPA experience. API is the same.

### Q: "How does AI work?"
**A:** Uses the **GitHub Copilot SDK** with Microsoft Agent Framework. The Copilot CLI communicates with GitHub Copilot to provide intelligent, context-aware responses about parks. No separate API keys needed.

### Q: "What happens if AI is disabled?"
**A:** Endpoints return graceful fallback messages with informational text.

### Q: "How many parks are in the database?"
**A:** Currently seeded with ~67 Ontario Provincial Parks. Easily expandable by updating seed-data/parks.json.

### Q: "Can I use this on mobile?"
**A:** React frontend is responsive and mobile-ready. Blazor UI is also responsive with MudBlazor.

### Q: "How does service discovery work?"
**A:** .NET Aspire automatically registers services and provides discovery. Blazor/React find API at `https://api` without hardcoded URLs.

---

## Demo Troubleshooting

### Issue: "AI features not working"
**Solution:** Ensure the Copilot CLI is installed and authenticated:
```bash
# Install Copilot CLI
gh extension install github/gh-copilot

# Authenticate
copilot auth login
```

### Issue: "React not loading (port 5173 error)"
**Solution:** Aspire auto-selects a port if 5173 is busy. Check Aspire Dashboard for actual React URL.

### Issue: "Database missing parks data"
**Solution:** Ensure `seed-data/parks.json` exists and is valid. API logs will show seed errors.

### Issue: "Swagger not loading"
**Solution:** Only available in Development. Verify `if (app.Environment.IsDevelopment())` in Program.cs.

---

## Post-Demo Checklist

- [ ] All three services (API, Blazor, React) running ✓
- [ ] Aspire Dashboard accessible and showing health ✓
- [ ] Parks load in both frontends ✓
- [ ] Search and filter work correctly ✓
- [ ] Swagger API docs are accessible ✓
- [ ] AI features working (if configured) ✓
- [ ] No console errors in browser dev tools ✓
- [ ] Logs visible in Aspire Dashboard ✓

---

## Time Breakdown

| Part | Feature | Duration |
|------|---------|----------|
| 1 | Aspire Dashboard | 1 min |
| 2a | Browsing Parks (Blazor) | 1 min |
| 2b | Search & Filter | 1 min |
| 2c | Park Details & Map | 1 min |
| 2d | Favorites | 0.5 min |
| 3 | Swagger API Examples | 2 min |
| 4 | React Frontend Overview | 2 min |
| 5a | Park Summaries (AI) | 1 min |
| 5b | Recommendations (AI) | 1 min |
| 5c | Chat Assistant (AI) | 1 min |
| 5d | Visit Planner (AI) | 1 min |
| 6 | Health & Monitoring | 1 min |
| **Total** | | **~15 min** |

---

## Advanced Demo Topics (Optional)

### If Time Permits:

1. **Show Database Schema**
   - Parks table with coordinates, region, description
   - ParkActivities junction table
   - Activity master table

2. **Show Entity Framework Migrations**
   ```bash
   dotnet ef migrations list --project OntarioParksExplorer.Api
   ```

3. **Show Service Architecture**
   - IParksService interface and implementation
   - IAiService and GitHub Copilot SDK integration
   - Dependency injection in Program.cs

4. **Show React Component Structure**
   - Pages (ParkList, ParkDetail, Chat)
   - Components (ParkCard, SearchBar, MapView)
   - Services (parkService.ts, aiService.ts)

5. **Show API Performance**
   - Aspire Metrics for request latency
   - Database query performance
   - AI response times

---

## Demo Conclusion

**Recap Key Points:**
- ✅ Full-stack application with dual frontends
- ✅ Modern architecture with .NET Aspire orchestration
- ✅ REST API with comprehensive endpoints
- ✅ AI-powered features for intelligent park discovery
- ✅ Production-ready with health checks and monitoring
- ✅ Easy to extend with new parks, activities, and features

**Next Steps for Audience:**
- Clone the repo and run locally
- Configure AI key and explore intelligent features
- Extend with custom parks or activities
- Integrate into existing systems via REST API
- Deploy to production with confidence (health checks, monitoring ready)

---

**Demo Prepared:** 2026-04-11  
**Maintained by:** Ontario Parks Explorer Team
