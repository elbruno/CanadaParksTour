import { test } from '@playwright/test';
import * as path from 'path';

const SCREENSHOT_DIR = path.join(__dirname, '..', '..', 'screenshots');

const pages = [
  { name: '01-home', url: '/', waitFor: 2000 },
  { name: '02-parks', url: '/parks', waitFor: 3000 },
  { name: '03-map', url: '/map', waitFor: 4000 },
  { name: '04-chat', url: '/chat', waitFor: 2000 },
  { name: '05-favorites', url: '/favorites', waitFor: 2000 },
  { name: '06-settings', url: '/settings', waitFor: 3000 },
];

for (const p of pages) {
  test(`screenshot: ${p.name}`, async ({ page }) => {
    await page.goto(p.url, { waitUntil: 'networkidle', timeout: 30000 });
    await page.waitForTimeout(p.waitFor);
    await page.screenshot({
      path: path.join(SCREENSHOT_DIR, `${p.name}.png`),
      fullPage: true,
    });
  });
}
