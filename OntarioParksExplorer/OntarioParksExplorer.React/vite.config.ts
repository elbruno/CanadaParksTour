import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Aspire provides env vars: PORT for the dev server, services__api__*__0 for the API URL
const port = process.env.PORT ? parseInt(process.env.PORT) : 5173;
const apiTarget = process.env.services__api__https__0
  || process.env.services__api__http__0
  || 'http://localhost:5000';

export default defineConfig({
  plugins: [react()],
  server: {
    port,
    proxy: {
      '/api': {
        target: apiTarget,
        changeOrigin: true,
        secure: false, // allow self-signed certs in development
      }
    }
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts'
  }
})
