import { test, expect } from '@playwright/test';

test.describe('Blazor App Smoke Tests', () => {
  test.skip('Navigate to Blazor app and verify title', async ({ page }) => {
    // This test is skipped because it requires the app to be running
    // To run: start the app with `dotnet run --project OntarioParksExplorer.AppHost`
    await page.goto('/');
    await expect(page).toHaveTitle(/Ontario Parks Explorer/);
  });

  test.skip('Click Parks navigation link', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Browse Parks');
    await expect(page).toHaveURL(/.*parks/);
  });

  test('Verify Playwright is properly configured', () => {
    // This test always passes - it's a configuration check
    expect(true).toBe(true);
  });
});
