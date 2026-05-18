import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/eureka-soap': {
        target: 'http://10.40.26.222:8080',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/eureka-soap/, ''),
      },
    },
  },
})
