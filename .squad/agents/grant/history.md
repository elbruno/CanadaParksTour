# Project Context

- **Owner:** Bruno Capuano
- **Project:** OntarioParksExplorer — a full-stack app to explore Ontario parks with AI features
- **Stack:** .NET 10, ASP.NET Core, EF Core + SQLite, .NET Aspire, Blazor, React (TypeScript), GitHub Copilot SDK
- **Created:** 2026-03-28

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-28: AI SDK Integration (WI-33, WI-34)
- **AI Packages**: Implemented GitHub Copilot SDK integration using `Microsoft.Extensions.AI` (v10.4.1) and `Microsoft.Extensions.AI.OpenAI` (v10.4.1) with the base `OpenAI` package (v2.9.1)
- **AI Service Pattern**: Created dedicated AI service layer at `Services/AI/` with:
  - `IAiService` interface defining all AI operations (park summaries, recommendations, chat, visit planning)
  - `AiService` implementation using `IChatClient` from Microsoft.Extensions.AI
  - Prompt templates in `Services/AI/Prompts/PromptTemplates.cs`
  - AI-specific DTOs in `Models/DTOs/AI/` (requests, responses, structured data)
- **API Methods**: Microsoft.Extensions.AI uses `IChatClient.GetResponseAsync()` (not CompleteAsync). Response has `Text` property directly (not Choices[0].Text)
- **DI Registration**: Register IChatClient using `new OpenAI.Chat.ChatClient(model, apiKey).AsIChatClient()` singleton
- **Graceful Degradation**: All AI features check if client is configured and return friendly fallback messages when API key is missing
- **Configuration**: AI settings in appsettings.json under "AI" section (Provider, Model, Endpoint, ApiKey)
- **Error Handling**: Each AI method has try/catch with logging and sensible fallback responses

### 2026-03-28: AI API Endpoints (WI-35, WI-36, WI-37, WI-38)
- **Controller**: Created `Controllers/AiController.cs` with route prefix `[Route("api/ai")]` to expose all AI features as HTTP endpoints
- **Endpoints Implemented**:
  - `POST /api/ai/parks/{id}/summary` - Generate AI-powered park summaries (WI-35)
  - `POST /api/ai/recommendations` - Get personalized park recommendations (WI-36)
  - `POST /api/ai/chat` - Q&A chatbot for Ontario parks (WI-37)
  - `POST /api/ai/plan-visit` - Generate visit itineraries (WI-38)
- **Architecture**: Single controller, injected IAiService and ParksDbContext for park entity lookups
- **Validation**: Input validation on all endpoints (required fields, duration limits 1-14 days, preference fields)
- **Error Handling**: Proper HTTP status codes (404 for missing parks, 400 for invalid input, 503 for AI service errors)
- **Documentation**: XML comments and `[ProducesResponseType]` attributes for Swagger integration
- **Park Lookups**: Use ParksDbContext.FindAsync() for simple lookups, Include() for related entities when needed by AiService

### 2026-03-28: AI Service Optimization (WI-41)
- **Prompt Templates**: Optimized all prompts in `PromptTemplates.cs` for token efficiency - reduced verbosity while maintaining clarity and output quality
- **Token Optimization**: Limited activities to 10 per prompt, truncate descriptions to 100 chars in chat, limit conversation history to 10 messages
- **Response Caching**: Implemented IMemoryCache for park summaries with 24-hour cache duration using key `park-summary-{parkId}`
- **Memory Cache Setup**: Registered `AddMemoryCache()` in Program.cs DI container
- **Context Injection**: Optimized chat feature with smart context filtering - loads only 5 relevant parks based on region keywords (north/south/east/west) in user question
- **Response Parsing**: Added robust JSON deserialization with case-insensitive options, validation of AI output, and clamping of match scores (0.0-1.0)
- **Timeout Handling**: Added 30-second `CancellationToken` timeout to all AI calls to prevent hanging requests
- **Error Handling**: Enhanced with specific catch blocks for `OperationCanceledException` (timeout), `JsonException` (parse errors), and general exceptions
- **Fallback Responses**: All methods provide graceful degradation with informative fallback content when AI fails
- **Documentation**: Added comprehensive XML documentation to all public methods and interface, created `PROMPTS.md` guide explaining each template's purpose, inputs, outputs, and optimizations
- **Build Verification**: Fixed XML documentation warning (Q&A entity reference), confirmed clean build with no warnings
