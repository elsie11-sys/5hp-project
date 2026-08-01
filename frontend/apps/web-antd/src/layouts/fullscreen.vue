<script lang="ts" setup>
/**
 * 全屏布局：不含 BasicLayout 的侧边栏 / 顶栏 / 标签页，
 * 仅提供一个 <router-view />，用于「可视化大屏」等需要沉浸式全屏展示的页面。
 */
defineOptions({ name: 'FullScreenLayout' });
</script>

<template>
  <RouterView />
</template>
