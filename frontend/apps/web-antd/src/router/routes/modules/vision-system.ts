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
        name: 'VisionRole',
        path: '/vision-system/role',
        component: () => import('#/views/vision-archive/system/role/index.vue'),
        meta: {
          icon: 'lucide:lock',
          title: '角色管理',
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
        name: 'VisionMenu',
        path: '/vision-system/menu',
        component: () => import('#/views/vision-archive/system/menu/index.vue'),
        meta: {
          icon: 'lucide:menu',
          title: '菜单管理',
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
        meta: {
          icon: 'lucide:file-text',
          title: '日志管理',
        },
        name: 'VisionLog',
        path: '/vision-system/log',
        // 父路由不带页面，访问 /vision-system/log 默认重定向到操作日志，兼容老链接
        redirect: '/vision-system/log/operation',
        children: [
          {
            name: 'VisionOperationLog',
            path: '/vision-system/log/operation',
            component: () => import('#/views/vision-archive/system/log/index.vue'),
            meta: {
              title: '操作日志',
            },
          },
          {
            name: 'VisionLoginLog',
            path: '/vision-system/log/login',
            component: () => import(
              '#/views/vision-archive/system/log/login_index.vue'
            ),
            meta: {
              icon: 'lucide:key-round',
              title: '登录日志',
            },
          },
        ],
      },
    ],
  },
];

export default routes;
