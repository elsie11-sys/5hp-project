<template>
  <div class="system-wrapper">
    <!-- 页面标题 -->
    <div class="page-header">
      <h2>⚙️ 系统管理</h2>
    </div>

    <!-- Tab切换栏 -->
    <div class="tab-bar">
      <div 
        class="tab-item" 
        v-for="tab in tabs" 
        :key="tab.key"
        :class="{ active: activeTab === tab.key }"
        @click="switchTab(tab.key)"
      >
        <span class="tab-icon">{{ tab.icon }}</span>
        <span class="tab-label">{{ tab.label }}</span>
        <span v-if="tab.badge" class="tab-badge">{{ tab.badge }}</span>
      </div>
    </div>

    <!-- Tab内容区 - 使用动态组件 -->
    <div class="tab-content">
      <component :is="currentComponent" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, defineAsyncComponent } from 'vue';

// ================= 异步加载子组件 =================
const OrgManage = defineAsyncComponent(() => import('./org/index.vue'));
const PermissionManage = defineAsyncComponent(() => import('./permission/index.vue'));
const UserManage = defineAsyncComponent(() => import('./user/index.vue'));
const LogManage = defineAsyncComponent(() => import('./log/index.vue'));
const DictManage = defineAsyncComponent(() => import('./dict/index.vue'));

// ================= Tab配置 =================
const tabs = [
  { key: 'organization', label: '组织架构管理', icon: '🏛️', badge: '' },
  { key: 'permission', label: '权限管理', icon: '🔐', badge: '' },
  { key: 'user', label: '用户账号管理', icon: '👤', badge: '' },
  { key: 'log', label: '操作日志管理', icon: '📋', badge: '2,847' },
  { key: 'dict', label: '基础字典管理', icon: '📚', badge: '' }
];

const activeTab = ref('organization');

// ================= 组件映射 =================
const componentMap: Record<string, any> = {
  organization: OrgManage,
  permission: PermissionManage,
  user: UserManage,
  log: LogManage,
  dict: DictManage
};

const currentComponent = computed(() => componentMap[activeTab.value] || OrgManage);

// ================= Tab切换 =================
const switchTab = (tabKey: string) => {
  activeTab.value = tabKey;
};
</script>

<style scoped>
.system-wrapper {
  padding: 20px;
  background-color: #f5f7fa;
  min-height: 100vh;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
}

.page-header {
  margin-bottom: 20px;
}
.page-header h2 {
  font-size: 24px;
  font-weight: 700;
  color: #303133;
  margin: 0;
}

.tab-bar {
  display: flex;
  gap: 4px;
  background: #fff;
  padding: 8px 20px;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.05);
  margin-bottom: 20px;
  flex-wrap: wrap;
}
.tab-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #606266;
  font-size: 14px;
  font-weight: 500;
}
.tab-item:hover {
  background-color: #f5f7fa;
  color: #409eff;
}
.tab-item.active {
  background: linear-gradient(135deg, #e8f4fd, #d4e8f7);
  color: #409eff;
  box-shadow: 0 2px 8px rgba(64, 158, 255, 0.12);
}
.tab-item .tab-icon { font-size: 18px; }
.tab-item .tab-label { font-weight: 500; }
.tab-item .tab-badge {
  background: #f56c6c;
  color: #fff;
  font-size: 11px;
  padding: 1px 8px;
  border-radius: 10px;
  margin-left: 4px;
}

.tab-content {
  min-height: 500px;
  animation: fadeIn 0.3s ease;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

@media (max-width: 768px) {
  .tab-item {
    padding: 8px 14px;
    font-size: 13px;
  }
  .page-header h2 {
    font-size: 20px;
  }
}
</style>
