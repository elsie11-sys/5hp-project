import { defineConfig } from '@vben/vite-config';

export default defineConfig(async () => {
  return {
    application: {},
    vite: {
      server: {
        proxy: {
          // .NET 后端尚未实现 auth/login，登录链路统一走 Nitro Mock（端口 5320，由 VITE_NITRO_MOCK 启动）
          // 注意：.NET 的 api/user/info 返回的 roles 是对象数组，与 Vben 期望的 string[] 不一致，
          // 因此 user/info 也走 Mock，保证登录后权限/首页跳转正常。
          '/api/auth': {
            changeOrigin: true,
            target: 'http://localhost:5320',
          },
          '/api/user/info': {
            changeOrigin: true,
            target: 'http://localhost:5320',
          },
          // 其余接口直通后端 .NET API（端口对应 backend/src/API/Properties/launchSettings.json 的 applicationUrl）
          '/api': {
            changeOrigin: true,
            rewrite: (path) => path.replace(/^\/api/, ''),
            target: 'http://localhost:5224/api',
            ws: true,
          },
        },
      },
    },
  };
});
