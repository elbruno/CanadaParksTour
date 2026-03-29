import { test } from '@playwright/test';
import * as path from 'path';

const SCREENSHOT_DIR = path.join(__dirname, '..', '..', '..', 'screenshots');
const BLAZOR_URL = process.env.BASE_URL || 'https://localhost:7113';
const REACT_URL = process.env.REACT_URL || 'http://localhost:5173';
const DASHBOARD_TOKEN_URL = process.env.DASHBOARD_TOKEN_URL || 'https://localhost:17139/login?t=9f70f1953e8758c4c30a86940743125d';
const API_URL = process.env.API_URL || 'https://localhost:7054';

// Blazor pages
const blazorPages = [
  { name: '01-home', url: '/', waitFor: 3000 },
  { name: '02-parks', url: '/parks', waitFor: 4000 },
  { name: '03-park-detail', url: '/parks/1', waitFor: 6000 },
  { name: '04-map', url: '/map', waitFor: 5000 },
  { name: '05-chat', url: '/chat', waitFor: 2000 },
  { name: '06-favorites', url: '/favorites', waitFor: 2000 },
  { name: '07-settings', url: '/settings', waitFor: 3000 },
];

for (const p of blazorPages) {
  test(`screenshot: ${p.name}`, async ({ page }) => {
    // Warm up Blazor circuit by visiting home first
    if (p.url !== '/') {
      await page.goto(`${BLAZOR_URL}/`, { waitUntil: 'networkidle', timeout: 30000 });
      await page.waitForTimeout(2000);
    }
    await page.goto(`${BLAZOR_URL}${p.url}`, { waitUntil: 'networkidle', timeout: 30000 });
    await page.waitForTimeout(p.waitFor);
    await page.screenshot({
      path: path.join(SCREENSHOT_DIR, `${p.name}.png`),
      fullPage: true,
    });
  });
}

// Aspire Dashboard - login with token URL to get the actual dashboard
test('screenshot: 08-aspire-dashboard', async ({ page }) => {
  await page.goto(DASHBOARD_TOKEN_URL, { waitUntil: 'networkidle', timeout: 30000 });
  await page.waitForTimeout(5000);
  await page.screenshot({
    path: path.join(SCREENSHOT_DIR, '08-aspire-dashboard.png'),
    fullPage: true,
  });
});

// API Swagger
test('screenshot: 09-api-swagger', async ({ page }) => {
  await page.goto(`${API_URL}/swagger`, { waitUntil: 'networkidle', timeout: 30000 });
  await page.waitForTimeout(2000);
  await page.screenshot({
    path: path.join(SCREENSHOT_DIR, '09-api-swagger.png'),
    fullPage: true,
  });
});

// React pages
const reactPages = [
  { name: '10-react-home', url: '/', waitFor: 3000 },
  { name: '11-react-parks', url: '/parks', waitFor: 4000 },
  { name: '12-react-map', url: '/map', waitFor: 5000 },
];

for (const p of reactPages) {
  test(`screenshot: ${p.name}`, async ({ page }) => {
    await page.goto(`${REACT_URL}${p.url}`, { waitUntil: 'networkidle', timeout: 30000 });
    await page.waitForTimeout(p.waitFor);
    await page.screenshot({
      path: path.join(SCREENSHOT_DIR, `${p.name}.png`),
      fullPage: true,
    });
  });
}
