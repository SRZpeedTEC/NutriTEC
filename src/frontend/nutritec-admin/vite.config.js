// Configuración de Vite: plugin de React y proxy de /api hacia el backend local.
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5174,
    proxy: {
      '/api': 'http://localhost:5000',
    },
  },
});
