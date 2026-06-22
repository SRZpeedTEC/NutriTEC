import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5174,
    proxy: {
      '/api': 'http://localhost:5024',
      '/mongo-api': { target: 'http://localhost:5191', rewrite: (path) => path.replace(/^\/mongo-api/, '/api') },
    },
  },
});
