import type { RouteRecordRaw } from 'vue-router';

import { $t } from '#/locales';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'lucide:settings',
      order: 99,
      title: '系统管理',
    },
    name: 'VisionSystem',
    path: '/vision-system',
    children: [
      {
        name: 'VisionUser',
        path: '/vision-system/user',
        component: () => import('#/views/vision-archive/system/user/index.vue'),
        meta: {
          icon: 'lucide:users',
          title: '用户管理',
        },
      },
      
      {
        name: 'VisionPermission',
        path: '/vision-system/permission',
        component: () => import('#/views/vision-archive/system/permission/index.vue'),
        meta: {
          icon: 'lucide:lock',
          title: '权限管理',
        },
      },
      {
        name: 'VisionOrg',
        path: '/vision-system/org',
        component: () => import('#/views/vision-archive/system/org/index.vue'),
        meta: {
          icon: 'lucide:building-2',
          title: '组织管理',
        },
      },
      {
        name: 'VisionDict',
        path: '/vision-system/dict',
        component: () => import('#/views/vision-archive/system/dict/index.vue'),
        meta: {
          icon: 'lucide:book-open',
          title: '字典管理',
        },
      },
      {
        name: 'VisionLog',
        path: '/vision-system/log',
        component: () => import('#/views/vision-archive/system/log/index.vue'),
        meta: {
          icon: 'lucide:file-text',
          title: '操作日志',
        },
      },
    ],
  },
];

export default routes;
