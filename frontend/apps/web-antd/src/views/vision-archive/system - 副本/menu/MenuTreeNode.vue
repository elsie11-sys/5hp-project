<template>
  <template v-for="menu in nodes" :key="menu.id">
    <tr :class="['menu-row', `row-type-${menu.type}`, { 'is-expanded': isExpanded(menu.id) }]">
      <td class="col-menu-name" :style="{ paddingLeft: (level * 24) + 'px' }">
        <span v-if="hasChildren(menu)" class="tree-toggle" @click="toggleExpand(menu.id)">
          <span class="toggle-icon" :class="{ rotated: isExpanded(menu.id) }">▶</span>
        </span>
        <span v-else class="tree-toggle-placeholder"></span>
        <span class="menu-icon">{{ menu.icon || getDefaultIcon(menu.type) }}</span>
        <span class="menu-name">{{ menu.name }}</span>
        <span class="type-badge" :class="`type-${menu.type === 1 ? 'catalog' : menu.type === 2 ? 'menu' : 'button'}`">
          {{ getTypeLabel(menu.type) }}
        </span>
      </td>
      <td class="col-icon">{{ menu.icon || '-' }}</td>
      <td class="col-sort">{{ menu.sort }}</td>
      <td class="col-permission">{{ menu.permission || '-' }}</td>
      <td class="col-component">{{ menu.component || '-' }}</td>
      <td class="col-status">
        <span class="status-tag" :class="menu.statusClass">
          {{ menu.statusText }}
        </span>
      </td>
      <td class="col-created">{{ menu.createdAt || '-' }}</td>
      <td class="col-actions">
        <div class="action-btns">
          <button class="btn-action btn-edit" @click="$emit('edit', menu)">修改</button>
          <button
            v-if="menu.type === 1"
            class="btn-action btn-add"
            @click="$emit('add', menu)"
          >新增</button>
          <button class="btn-action btn-delete" @click="$emit('delete', menu)">删除</button>
        </div>
      </td>
    </tr>
    <!-- 递归渲染子节点 -->
    <menu-tree-node
      v-if="hasChildren(menu) && isExpanded(menu.id)"
      :nodes="menu.children"
      :level="level + 1"
      :is-expanded="isExpanded"
      :toggle-expand="toggleExpand"
      @edit="$emit('edit', $event)"
      @add="$emit('add', $event)"
      @delete="$emit('delete', $event)"
    />
  </template>
</template>

<script setup lang="ts">
interface MenuInfo {
  id: number;
  name: string;
  icon?: string;
  type: number;
  parentId: number | null;
  sort: number;
  status: number;
  statusText?: string;
  statusClass?: string;
  typeText?: string;
  permission?: string;
  component?: string;
  createdAt?: string;
  children?: MenuInfo[];
}

defineProps<{
  nodes: MenuInfo[];
  level?: number;
  isExpanded: (id: number) => boolean;
  toggleExpand: (id: number) => void;
}>();

defineEmits<{
  (e: 'edit', menu: MenuInfo): void;
  (e: 'add', menu: MenuInfo): void;
  (e: 'delete', menu: MenuInfo): void;
}>();

function hasChildren(menu: MenuInfo): boolean {
  return !!(menu.children && menu.children.length > 0);
}

function getTypeLabel(type: number): string {
  const map: Record<number, string> = { 1: '目录', 2: '菜单', 3: '按钮' };
  return map[type] || '未知';
}

function getDefaultIcon(type: number): string {
  const map: Record<number, string> = { 1: '📁', 2: '📄', 3: '🔘' };
  return map[type] || '📄';
}
</script>
