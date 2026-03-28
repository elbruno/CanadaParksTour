# OntarioParksExplorer E2E Tests

End-to-end tests using Playwright for the Ontario Parks Explorer application.

## Setup

Tests are configured and ready to run. Playwright and Chromium browser are installed.

## Running Tests

```bash
npm test
```

## Test Structure

Tests are organized into three categories:

### 1. Blazor App Smoke Tests (`blazor-smoke.spec.ts`)
- Verify Blazor app title
- Test navigation to Parks page
- **Note:** Requires Blazor app running on https://localhost:7001

### 2. React App Smoke Tests (`react-smoke.spec.ts`)
- Verify React app title  
- Test navigation to Parks page
- **Note:** Requires React app running on http://localhost:5173

### 3. API Health Check (`api-health.spec.ts`)
- Verify API /health endpoint returns 200
- **Note:** Requires API running on http://localhost:5000

## Running Against Live Apps

Most tests are currently skipped because they require running applications. To run them:

1. Start the application host:
   ```bash
   cd ..
   dotnet run --project OntarioParksExplorer.AppHost
   ```

2. Uncomment the `test.skip` lines in the test files to enable tests

3. Run tests:
   ```bash
   npm test
   ```

## Test Configuration

See `playwright.config.ts` for configuration:
- **Browser:** Chromium only (for speed)
- **Base URL:** https://localhost:7001 (Blazor app)
- **Screenshots:** On failure
- **Traces:** Retained on failure

## Current Status

- ✅ Playwright properly configured
- ✅ 3 structure validation tests passing
- ⏭️ 5 integration tests skipped (require running apps)

The test infrastructure is complete and ready for integration testing when apps are running.
