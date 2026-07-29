import { defineConfig } from '@vben/vite-config';

export default defineConfig(async () => {
  return {
    application: {},
    vite: {
      server: {
        proxy: {
          '/api': {
            changeOrigin: true,
            rewrite: (path) => path.replace(/^\/api/, ''),
            // 直通后端 .NET API（端口对应 backend/src/API/Properties/launchSettings.json 的 applicationUrl）
            target: 'http://localhost:5224/api',
            ws: true,
          },
        },
      },
    },
  };
});
