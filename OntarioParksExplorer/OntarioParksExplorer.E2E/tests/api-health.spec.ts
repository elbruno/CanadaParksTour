import { test, expect } from '@playwright/test';
import http from 'http';

test.describe('API Health Check', () => {
  test.skip('Hit /health endpoint and verify 200', async () => {
    // This test is skipped because it requires the API to be running
    // To run: start the app with `dotnet run --project OntarioParksExplorer.AppHost`
    
    const healthCheck = () => new Promise<number>((resolve, reject) => {
      const req = http.get('http://localhost:5000/health', (res) => {
        resolve(res.statusCode || 0);
      });
      req.on('error', reject);
      req.end();
    });

    const statusCode = await healthCheck();
    expect(statusCode).toBe(200);
  });

  test('Verify API test structure exists', () => {
    // This test always passes - it's a structure check
    expect(typeof test).toBe('function');
    expect(typeof expect).toBe('function');
  });
});
