import type { RouteRecordRaw } from 'vue-router';

import { $t } from '#/locales';

const routes: RouteRecordRaw[] = [
  // ==========================================
  // 可视化管理（保留）
  // ==========================================
  {
    meta: {
      icon: 'lucide:eye',
      order: -1,
      title: '可视化管理',
    },
    name: 'VisionArchive',
    path: '/vision',
    children: [
      {
        name: 'VisionNational',
        path: '/vision/national',
        component: () => import('#/views/vision-archive/index.vue'),
        meta: {
          affixTab: true,
          icon: 'lucide:globe',
          title: '全国总览大屏',
        },
      },
      {
        name: 'VisionProvince',
        path: '/vision/province/:code',
        component: () => import('#/views/vision-archive/province.vue'),
        meta: {
          icon: 'lucide:map',
          title: '省级看板',
          hideMenu: true,
        },
      },
      {
        name: 'VisionCity',
        path: '/vision/city/:code',
        component: () => import('#/views/vision-archive/city.vue'),
        meta: {
          icon: 'lucide:building',
          title: '市级看板',
          hideMenu: true,
        },
      },
      {
        name: 'VisionCounty',
        path: '/vision/county/:code',
        component: () => import('#/views/vision-archive/county.vue'),
        meta: {
          icon: 'lucide:map-pin',
          title: '县区级看板',
          hideMenu: true,
        },
      },
      {
        name: 'VisionSchool',
        path: '/vision/school/:id',
        component: () => import('#/views/vision-archive/school.vue'),
        meta: {
          icon: 'lucide:school',
          title: '学校看板',
          hideMenu: true,
        },
      },
      {
        name: 'VisionCoreCharts',
        path: '/vision/charts',
        component: () => import('#/views/vision-archive/charts/index.vue'),
        meta: {
          icon: 'lucide:pie-chart',
          title: '核心图表',
        },
      },
    ],
  },

  // ==========================================
  // 学生综合管理（与可视化管理平级）
  // ==========================================
  {
    meta: {
      icon: 'lucide:users',
      order: 0,
      title: '学生综合管理',
    },
    name: 'StudentManage',
    path: '/student',
    children: [
      {
        name: 'StudentProfile',
        path: '/student/profile',
        component: () => import('#/views/vision-archive/student/index.vue'),
        meta: {
          icon: 'lucide:user',
          title: '学生电子档案',
        },
      },
      {
        name: 'StudentCollect',
        path: '/student/collect',
        component: () => import('#/views/vision-archive/collect/index.vue'),
        meta: {
          icon: 'lucide:upload',
          title: '数据采集与审核',
        },
      },
      {
        name: 'StudentStatisticsReport',
        path: '/student/statistics-report',
        component: () => import('#/views/vision-archive/statistics-report/index.vue'),
        meta: {
          icon: 'lucide:bar-chart-3',
          title: '统计分析与报表',
        },
      },
    ],
  },

  // ==========================================
  // 预警管理（与可视化管理、学生综合管理平级）
  // ==========================================
  {
    meta: {
      icon: 'lucide:bell',
      order: 1,
      title: '预警管理',
    },
    name: 'WarningManage',
    path: '/warning',
    children: [
      {
        name: 'WarningEarlyWarning',
        path: '/warning/early-warning',
        component: () => import('#/views/vision-archive/early-warning/index.vue'),
        meta: {
          icon: 'lucide:bell-ring',
          title: '预警与干预管理',
        },
      },
    ],
  },
];

export default routes;
