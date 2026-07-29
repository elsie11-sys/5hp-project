// apps/web-antd/src/router/routes/modules/parent.ts
import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/parent',
    name: 'Parent',
    component: () => import('#/views/parent/index.vue'),
    meta: {
      title: '家长端',
      icon: 'lucide:user',
      hideMenu: true,  // 在侧边栏隐藏（家长端独立访问）
    },
  },
];

export default routes;
