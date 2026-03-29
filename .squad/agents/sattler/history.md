# Project Context

- **Owner:** Bruno Capuano
- **Project:** OntarioParksExplorer — a full-stack app to explore Ontario parks with AI features
- **Stack:** .NET 10, ASP.NET Core, EF Core + SQLite, .NET Aspire, Blazor, React (TypeScript), GitHub Copilot SDK
- **Created:** 2026-03-28

## Core Context

**Frontend Architect — Blazor & React Implementation (WI-07-08, WI-16-22, WI-23-31, WI-39-40)**

Sattler led complete dual-frontend implementation for OntarioParksExplorer. Established MudBlazor for Blazor frontend (Material Design components) and custom CSS for React (Vite bundler, TypeScript, modern SPA patterns). Implemented comprehensive feature sets across both: parks listing with search/filter, favorites with localStorage persistence, park details with image galleries, interactive maps, and AI integrations (summaries, recommendations, chat, visit planner).

**Blazor Frontend (WI-16 through WI-22):**
- Created typed ParksApiClient service with 5 core methods (GetParks, GetParkById, SearchParks, FilterParks, GetActivities)
- Built Parks.razor with pagination, debounced search (300ms), multi-select activity filters, responsive MudGrid layout
- Implemented Park Detail with image carousel, breadcrumbs, favorite toggle, Leaflet map integration
- Created Favorites page with localStorage sync and empty state handling
- Integrated Map.razor with Leaflet JavaScript interop (initMap, addMarker, flyTo functions)
- Added AI features: summary generation, visit planner (day-by-day itineraries), recommendations widget, chat interface

**React Frontend (WI-24 through WI-30):**
- Built complete TypeScript type system (ParkListDto, ParkDetailDto, ActivityDto, PagedResult<T>)
- Implemented axios API client with error handling and smart endpoint selection (search vs filter vs default)
- Created responsive Parks component with pagination, debounced search, activity filter chips
- Developed Park Detail page with image gallery (prev/next, thumbnails), linked map view
- Implemented Favorites via custom useFavorites hook and React Context (FavoritesProvider)
- Integrated react-leaflet Map component showing all parks with popups
- Added AI features matching Blazor: park summaries, recommendations page, chat interface, visit planner modal

**AI Features (WI-39, WI-40):**
- Blazor: AI DTOs, ParksApiClient AI methods, UI components for summary/recommendations/chat/planner
- React: AI types, api.ts AI endpoints, styled components with loading states and conversation history
- Both frontends cache AI summaries and maintain full chat history for context

**Latest Work (2026-03-29):**
- Captured 4 React frontend screenshots for README showcase (home, parks, detail, chat)
- Updated Playwright E2E tests with authenticated Aspire Dashboard access
- Added comprehensive React section to USER_MANUAL.md with startup, navigation, features, and development notes
- Verified all GitHub markdown image links and optimized screenshots for docs (1280x800 PNG)

## Learnings

### 2026-03-29 - README Screenshots & User Manual React Section
- **Screenshot Capture**: Captured 4 React frontend screenshots for README showcase: home page, parks list with filters, park details with AI features, AI chat interface. Integrated with Aspire Dashboard screenshots showing all 3 running services.
- **E2E Test Enhancement**: Updated Playwright tests (full-journey.spec.ts) with authenticated Aspire Dashboard access (Bearer token), resource health verification (3 services), and multi-frontend screenshot capture sequence.
- **User Manual React Section**: Added comprehensive React frontend walkthrough to docs/USER_MANUAL.md including startup instructions (npm install, npm run dev), navigation routes, feature descriptions, development notes (Vite, TypeScript, API proxy), build commands, and visual screenshots showing all major features.
- **Team Showcase Integration**: React frontend featured prominently in team roster as Sattler's implementation showcase, highlighting TypeScript, responsive design, and AI feature integration. Screenshots demonstrate modern SPA with Vite bundler and context API state management.
- **Documentation Quality**: All image links verified working in rendered GitHub markdown. Screenshots optimized for GitHub docs (1280x800 PNG format). Maintained consistency with Blazor documentation format while highlighting React-specific features (Vite, hooks, context API vs Blazor services).

### 2026-03-28 - Initial Frontend Setup (WI-07, WI-08, WI-23, WI-31)
- **Blazor Setup**: Enhanced existing Blazor Web App with MudBlazor 9.2.0 for UI components. Configured in Program.cs with AddMudServices(), added CSS/JS references in App.razor, and imported MudBlazor namespace in _Imports.razor.
- **Blazor Layout**: Created responsive MainLayout.razor using MudBlazor components (MudLayout, MudAppBar, MudDrawer, MudNavMenu). Navigation includes Home (/), Browse Parks (/parks), My Favorites (/favorites), Map View (/map). Drawer is toggleable for mobile with hamburger menu.
- **Blazor Pages**: Created placeholder pages (Home.razor, Parks.razor, Favorites.razor, Map.razor). Home page features welcome message, feature cards, and action buttons using MudBlazor Grid and Card components.
- **React Setup**: Created new Vite-based React TypeScript project at OntarioParksExplorer.React/. Installed dependencies: react, react-dom, react-router-dom, axios, @vitejs/plugin-react. Configured vite.config.ts with proxy for /api requests.
- **React Layout**: Implemented responsive Layout component with header, collapsible nav menu, and main content area. Navigation uses React Router with routes for Home, Parks, Park Details, Favorites, and Map.
- **React Pages**: Created placeholder page components (Home, Parks, ParkDetail, Favorites, Map) with basic structure and content.
- **React Styling**: Created clean CSS with app.css (global styles, cards, buttons, features grid) and layout.css (header, nav, responsive mobile menu). Mobile-first approach with hamburger menu for small screens.
- **Aspire Integration**: Added Aspire.Hosting.NodeJs package to AppHost. Configured React app in AppHost.cs with AddNpmApp, referencing API and using external HTTP endpoints.
- **TypeScript Configuration**: Had to adjust tsconfig.json to enable JSX support (jsx: "react-jsx") and disable verbatimModuleSyntax to fix build errors.
- **Both frontends verified building successfully**: Blazor with dotnet build, React with npm run build.

### 2026-03-28 - Complete React Frontend Implementation (WI-24 through WI-30)
- **TypeScript Types (WI-24)**: Created src/types/index.ts with all API DTOs: ParkListDto, ParkDetailDto, ActivityDto, ParkImageDto, and generic PagedResult<T>.
- **API Client (WI-24)**: Implemented src/services/api.ts using axios with typed functions for all backend endpoints. Created ApiError class for structured error handling. All functions use /api base URL with Vite proxy forwarding to backend.
- **Favorites System (WI-30)**: Built complete favorites infrastructure with useFavorites custom hook (localStorage persistence), FavoritesContext (React Context for state sharing), and wrapped App with FavoritesProvider. Favorites sync across all components.
- **Parks List Page (WI-25, WI-27, WI-28)**: Fully implemented Parks.tsx with responsive card grid, pagination controls, loading skeletons, featured badges, activity tags, and heart icon favorites. Added debounced search (300ms) with clear button. Implemented multi-select activity filter with checkbox UI and removable filter chips. Smart API selection: searches use searchParks(), filters use filterParks(), default uses getParks().
- **Park Details Page (WI-26)**: Created comprehensive ParkDetail.tsx with breadcrumb navigation, image gallery with prev/next controls and thumbnail strip, full park information (description, location, region, coordinates), activity chips, website link, favorite toggle button, and small map placeholder linking to full map view.
- **Favorites Page (WI-30)**: Implemented Favorites.tsx that loads all favorited parks from localStorage IDs, displays in same card grid as Parks page, shows empty state with call-to-action when no favorites exist, real-time updates when favorites change.
- **Map Visualization (WI-29)**: Added react-leaflet and leaflet dependencies. Implemented Map.tsx with full Ontario map (center 50.0, -85.0, zoom 5) showing all parks as markers with popups. Popup includes park name, region, featured badge, and "View Details" button. Fixed default marker icons issue with explicit icon imports.
- **Comprehensive Styling**: Massively expanded app.css with styles for filters section (search box, activity checkboxes, filter chips), parks grid with hover effects, park cards with images and badges, favorite button positioning, pagination controls, skeleton loading animations, park detail page (breadcrumb, image gallery with controls, thumbnail strip), map popup styles, loading/error states, and full mobile responsiveness.
- **TypeScript Issue**: Changed searchTimeout type from NodeJS.Timeout to number to avoid TypeScript namespace errors in browser environment.
- **Build Verification**: Confirmed successful TypeScript compilation and Vite build (npm run build) with no errors. Final bundle: 439KB JS, 24KB CSS.

### 2026-03-28 - Complete Blazor Frontend Implementation (WI-16 through WI-22)
- **DTO Models (WI-16)**: Created typed models in Models/ folder matching backend API: ActivityDto, ParkImageDto, ParkListDto, ParkDetailDto, PagedResultDto<T>. All DTOs use proper C# properties with default values.
- **API Client Service (WI-16)**: Implemented ParksApiClient.cs using typed HttpClient with methods: GetParksAsync, GetParkByIdAsync, SearchParksAsync, FilterParksAsync, GetActivitiesAsync. All methods handle errors gracefully returning empty results instead of throwing exceptions. Registered with DI using AddHttpClient<ParksApiClient> with base URL "http://api" for Aspire service discovery.
- **Favorites Service (WI-22)**: Created FavoritesService.cs using IJSRuntime for localStorage interop. Methods: GetFavoritesAsync, AddFavoriteAsync, RemoveFavoriteAsync, IsFavoriteAsync. Stores favorites as JSON array of park IDs. Registered as scoped service.
- **Parks List Page (WI-17, WI-19, WI-20)**: Fully implemented Parks.razor with MudGrid card layout, MudCarousel for images with gradient fallback, featured badges (MudChip), activity chips (limited to 3 + more indicator), favorite toggle buttons, loading skeletons (MudSkeleton), pagination (MudPagination). Added debounced search (300ms) with MudTextField and clear button. Activity filter using MudSelect with MultiSelection, displays selected filters as removable MudChips. Smart API routing: search → SearchParksAsync, filters → FilterParksAsync, default → GetParksAsync.
- **Park Detail Page (WI-18, WI-21)**: Built ParkDetail.razor with MudBreadcrumbs, MudCarousel for image gallery, park info display, MudChipSet for activities, MudList for location details, favorite heart toggle, external website link. Integrated Leaflet map using JavaScript interop to show park location with marker and popup.
- **Map Visualization (WI-21)**: Implemented Map.razor loading all parks across pages, initializing Leaflet map centered on Ontario (50.0, -85.0, zoom 5). Each park gets a marker with popup containing name, region, and link to detail page. Created wwwroot/js/map.js with JavaScript interop functions: initMap, addMarker, clearMarkers, flyTo.
- **Favorites Page (WI-22)**: Created Favorites.razor fetching favorite park IDs from localStorage, loading park details via API, displaying in same card grid layout as Parks page. Shows empty state with icon, message, and "Browse Parks" button when no favorites exist. Favorite removal updates UI in real-time.
- **Leaflet Integration**: Added Leaflet CSS/JS from CDN (unpkg.com v1.9.4) to App.razor. JavaScript interop provides window.mapInterop object for map control from Blazor components.
- **MudBlazor Type Parameters**: All MudBlazor components requiring generic types properly specified: MudChip<T="string">, MudChipSet<T="string">, MudList<T="string">, MudListItem<T="string">, MudCarousel<TData="object">. Fixed MudSelect MultiSelection binding using IReadOnlyCollection<string> with property setter triggering filter changes.
- **Build Verification**: Blazor project compiles successfully with dotnet build, all Razor components generate without errors.

### 2026-03-28 - AI Features Integration to Blazor (WI-39)
- **AI DTOs**: Created comprehensive AI-related DTOs: AiRecommendationRequestDto, AiRecommendationDto, AiChatRequestDto with ChatMessageDto, AiVisitPlanRequestDto, VisitPlanDto with DayPlanDto. All models properly structured to match backend API contracts.
- **ParksApiClient AI Methods**: Extended ParksApiClient with four AI methods: GenerateSummaryAsync (POST /api/ai/parks/{id}/summary), GetRecommendationsAsync (POST /api/ai/recommendations), ChatAsync (POST /api/ai/chat), PlanVisitAsync (POST /api/ai/plan-visit). All methods handle errors gracefully with null/empty returns.
- **AI Summary on Park Detail (Feature 1)**: Enhanced ParkDetail.razor with "✨ Generate AI Summary" button below park description. Shows loading state while generating, displays result in MudAlert with AutoAwesome icon. Summary cached in component state to avoid re-generation.
- **Visit Planner Component (Feature 4)**: Created Components/Shared/VisitPlanner.razor embedded in ParkDetail sidebar. Form includes MudNumericField for duration (1-14 days), MudSelect for season, MudChipSet for multi-select interests. Displays day-by-day itinerary with MudCard for each day showing theme, activities chips, and descriptions. "Create New Plan" button resets form.
- **Recommendations Widget (Feature 2)**: Created Components/Shared/RecommendationsWidget.razor on Home page. Multi-select activity chips, optional region MudSelect, optional free-text preferences. Results displayed as MudCards with park name, AI-generated reason, and "View Details" button navigating to park detail page.
- **AI Chat Interface (Feature 3)**: Built Components/Pages/AiChat.razor at /chat route. Chat-style UI with 600px scrollable container, user messages styled right with blue background, AI messages left with gray background. MudTextField input at bottom, Enter key sends (Shift+Enter for newlines). Maintains full conversation history in component state for context. Loading indicator shows "AI is thinking..." with MudProgressCircular. Clear Chat button resets conversation.
- **Navigation Updates**: Added "AI Chat" link to MainLayout navigation menu with MudDivider separator. Added "AI Chat" button to Home page card actions. Added @using OntarioParksExplorer.Blazor.Components.Shared to _Imports.razor.
- **MudBlazor SelectionMode**: Fixed MudChipSet warnings by changing MultiSelection="true" to SelectionMode="SelectionMode.MultiSelection" in RecommendationsWidget and VisitPlanner components, following MudBlazor 9.2.0 best practices.
- **Build Verification**: Blazor project compiles successfully with dotnet build, zero warnings. All AI components render properly with InteractiveServer mode.

### 2026-03-28 - AI Features in React Frontend (WI-40)
- **TypeScript AI Types**: Extended types/index.ts with AI-related interfaces: RecommendationRequest, ParkRecommendation, ChatMessage, VisitPlanRequest, VisitPlan, DayItinerary. All types match backend API contracts.
- **AI API Client Methods**: Added four AI endpoints to api.ts: generateSummary(parkId), getRecommendations(request), chat(message, history), planVisit(request). All methods use POST requests to /api/ai/* endpoints with proper error handling.
- **AI Park Summary (ParkDetail.tsx)**: Added "✨ Generate AI Summary" button below park description. Implemented loading state during generation and displays result in styled card with AI badge. Summary cached in component state for persistence during page visit.
- **Visit Planner Component**: Created VisitPlanner.tsx modal dialog component with form for duration (1-14 days), interests (checkboxes), and season (dropdown). Submits to planVisit API and displays itinerary as day-by-day cards with activities, tips, packing list, and general advice. Integrated into ParkDetail page via "📅 Plan My Visit" button.
- **Recommendations Page**: Created Recommendations.tsx at /recommendations route with multi-select activity checkboxes (from activities API), optional region input, and free-text preference field. Displays recommendations as cards with park name, AI-generated reason, and link to park detail page.
- **AI Chat Interface**: Created AiChat.tsx at /chat route with chat-style UI featuring message bubbles (user right, AI left), conversation history maintained in state, input field with send button, loading dots animation during AI response. Full conversation context sent with each message for continuity.
- **Navigation Updates**: Added "AI Recommendations" and "AI Chat" links to Layout.tsx navigation menu. Updated App.tsx router with new routes for /recommendations and /chat.
- **AI Styles**: Extended app.css with comprehensive styles for AI features including gradient buttons (btn-ai, btn-planner), AI summary card with branded styling, modal overlay/dialog for visit planner, form controls (inputs, checkboxes, selects), chat interface (message bubbles, avatars, loading dots), recommendations grid with hover effects. All styles fully responsive with mobile breakpoints.
- **Build Verification**: React project compiles successfully with npm run build. TypeScript validation passes with no errors. Final bundle: 450KB JS, 29KB CSS.

<!-- Append new learnings below. Each entry is something lasting about the project. -->

