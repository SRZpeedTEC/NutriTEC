import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { resolve } from 'path';
import { fileURLToPath } from 'url';

const __dirname = fileURLToPath(new URL('.', import.meta.url));

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      // Shared UI library and services (same source as the web client)
      '@nutritec/shared': resolve(__dirname, '../../frontend/shared/src'),
      // Client pages reused directly (single source of truth)
      '@pages': resolve(__dirname, '../../frontend/nutritec-cliente/src/pages'),
    },
    // Force all react imports to resolve from this project's node_modules,
    // even when coming from aliased paths outside this directory.
    dedupe: ['react', 'react-dom'],
  },
  build: {
    outDir: 'dist',
    sourcemap: false,
  },
});
