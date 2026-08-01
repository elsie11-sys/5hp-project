<template>
  <div class="permission-manage">
    <!-- 子Tab切换 -->
    <div class="sub-tab-bar">
      <div
        class="sub-tab-item"
        :class="{ active: subActiveTab === 'role' }"
        @click="subActiveTab = 'role'"
      >
        <span class="sub-tab-icon">👥</span>
        <span class="sub-tab-label">角色管理</span>
        <span class="sub-tab-badge">{{ filteredRoles.length }}</span>
      </div>
      <div
        class="sub-tab-item"
        :class="{ active: subActiveTab === 'permission' }"
        @click="subActiveTab = 'permission'"
      >
        <span class="sub-tab-icon">🔐</span>
        <span class="sub-tab-label">权限分配</span>
      </div>
    </div>

    <!-- ==================== 角色管理 ==================== -->
    <div v-show="subActiveTab === 'role'" class="sub-tab-content">
      <!-- 操作栏 -->
      <div class="toolbar">
        <div class="toolbar-left">
          <button class="btn btn-primary" @click="handleAddRole">
            <span class="icon">➕</span> 新增角色
          </button>
          <button class="btn btn-danger" @click="handleBatchDeleteRole">
            <span class="icon">🗑️</span> 删除角色
          </button>
          <select v-model="levelFilter" class="btn btn-default btn-filter">
            <option value="">全部级别</option>
            <option :value="1">国家级</option>
            <option :value="2">省级</option>
            <option :value="3">市级</option>
            <option :value="4">区县级</option>
            <option :value="5">学校级</option>
            <option :value="6">班级级</option>
          </select>
          <select v-model="statusFilter" class="btn btn-default btn-filter">
            <option value="">全部状态</option>
            <option :value="1">启用</option>
            <option :value="0">禁用</option>
          </select>
          <button class="btn btn-default btn-sm" @click="handleResetFilter">重置</button>
        </div>
        <div class="toolbar-right">
          <div class="search-box">
            <input
              type="text"
              v-model="roleSearch"
              placeholder="搜索角色名称..."
              @keyup.enter="currentPage = 1"
            />
          </div>
        </div>
      </div>

      <!-- 角色列表：按级别分组 -->
      <div class="role-groups">
        <div
          class="role-group"
          v-for="group in groupedRoles"
          :key="group.levelKey"
        >
          <div class="role-group-header" @click="toggleLevelGroup(group.levelKey)">
            <span class="group-toggle" :class="{ collapsed: collapsedLevelGroups[group.levelKey] }">▶</span>
            <span class="group-icon">{{ group.icon }}</span>
            <span class="group-name">{{ group.levelLabel }}</span>
            <span class="group-count">{{ group.roles.length }} 个角色</span>
          </div>
          <div class="role-group-body" v-show="!collapsedLevelGroups[group.levelKey]">
            <div class="role-list">
              <div
                class="role-card"
                v-for="role in group.roles"
                :key="role.id"
              >
                <div class="role-header">
                  <span class="role-icon">{{ role.icon }}</span>
                  <span class="role-name">{{ role.name }}</span>
                  <span class="role-level">{{ role.levelLabel }}</span>
                  <span class="role-user-count">👤 {{ role.userCount }}人</span>
                  <span class="role-status" :class="role.statusClass">{{ role.statusText }}</span>
                  <div class="role-actions">
                    <button class="btn btn-sm btn-primary" @click="handleAssignPermission(role)">分配权限</button>
                    <button class="btn btn-sm btn-info" @click="handleEditRole(role)">编辑</button>
                    <button class="btn btn-sm btn-danger" @click="handleDeleteRole(role)">删除</button>
                  </div>
                </div>
                <div class="role-permissions">
                  <template v-for="(perm, idx) in getDisplayPerms(role)" :key="idx">
                    <span class="permission-tag" v-if="idx < 4">{{ perm }}</span>
                  </template>
                  <span
                    class="permission-tag more-tag"
                    v-if="role.permissions.length > 4"
                    @click="toggleExpandPerm(role.id)"
                  >
                    {{ expandPermMap[role.id] ? '收起' : `+${role.permissions.length - 4}更多` }}
                  </span>
                </div>
                <div class="role-desc">{{ role.desc }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- 空状态 -->
        <div v-if="filteredRoles.length === 0" class="empty-state">
          <div class="empty-icon">📭</div>
          <div class="empty-text">暂无角色数据</div>
          <div class="empty-desc">点击"新增角色"按钮创建第一个角色</div>
        </div>
      </div>

      <!-- 分页 -->
      <div class="pagination" v-if="filteredRoles.length > pageSize">
        <button class="page-btn" :disabled="currentPage === 1" @click="currentPage--">上一页</button>
        <span class="page-info">第 {{ currentPage }} / {{ totalPages }} 页</span>
        <button class="page-btn" :disabled="currentPage === totalPages" @click="currentPage++">下一页</button>
        <select v-model.number="pageSize" class="page-size-select">
          <option :value="8">8 条/页</option>
          <option :value="12">12 条/页</option>
          <option :value="24">24 条/页</option>
        </select>
      </div>

      <!-- 五级权限体系说明 -->
      <div class="permission-legend">
        <h4>📋 五级权限体系说明</h4>
        <div class="legend-item" v-for="item in legendData" :key="item.level">
          <span class="legend-icon">{{ item.icon }}</span>
          <span class="legend-name">{{ item.name }}</span>
          <span class="legend-desc">{{ item.desc }}</span>
        </div>
      </div>
    </div>

    <!-- ==================== 权限分配 ==================== -->
    <div v-show="subActiveTab === 'permission'" class="sub-tab-content">
      <div class="permission-assign">
        <!-- 左侧：角色列表（按级别分组） -->
        <div class="assign-left">
          <div class="assign-header-left">
            <h4>📋 选择角色</h4>
            <span class="assign-hint">{{ filteredRolesForSelect.length }} 个角色</span>
          </div>
          <!-- 角色搜索 -->
          <div class="role-search">
            <input
              type="text"
              v-model="roleSelectSearch"
              placeholder="搜索角色..."
            />
          </div>
          <div class="role-select-list">
            <div
              class="role-select-group"
              v-for="group in groupedRoleSelect"
              :key="group.levelKey"
            >
              <div
                class="role-select-group-header"
                @click="toggleRoleSelectGroup(group.levelKey)"
              >
                <span class="group-toggle" :class="{ collapsed: collapsedRoleSelectGroups[group.levelKey] }">▶</span>
                <span class="group-icon">{{ group.icon }}</span>
                <span class="group-name">{{ group.levelLabel }}</span>
                <span class="group-count">{{ group.roles.length }}</span>
              </div>
              <div v-show="!collapsedRoleSelectGroups[group.levelKey]">
                <div
                  class="role-select-item"
                  v-for="role in group.roles"
                  :key="role.id"
                  :class="{ active: selectedRoleId === role.id }"
                  @click="selectRole(role)"
                >
                  <span class="role-icon">{{ role.icon }}</span>
                  <span class="role-name">{{ role.name }}</span>
                  <span class="role-level">{{ role.levelLabel }}</span>
                  <span class="role-status-dot" :class="role.statusClass"></span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 右侧：权限配置 -->
        <div class="assign-right">
          <div class="assign-header">
            <h4>
              <span v-if="selectedRole" class="selected-role-name">
                {{ selectedRole.icon }} {{ selectedRole.name }}
              </span>
              <span v-else class="placeholder-text">请选择角色</span>
              <span class="assign-subtitle">- 权限配置</span>
            </h4>
            <div class="assign-actions">
              <template v-if="selectedRole">
                <button class="btn btn-default btn-sm" @click="toggleExpandAll">
                  <span class="icon">{{ allExpanded ? '⤡' : '⤢' }}</span>
                  {{ allExpanded ? '全部收起' : '全部展开' }}
                </button>
                <button class="btn btn-default btn-sm" @click="toggleAllPermissions">
                  <span class="icon">{{ allPermissionsSelected ? '◯' : '☑' }}</span>
                  {{ allPermissionsSelected ? '全不选' : '全选' }}
                </button>
              </template>
              <button class="btn btn-success btn-sm" @click="handleSavePermission" :disabled="!selectedRole">
                <span class="icon">💾</span> 保存权限
              </button>
              <button class="btn btn-default btn-sm" @click="handleResetPermission" :disabled="!selectedRole">
                <span class="icon">↺</span> 重置
              </button>
            </div>
          </div>

          <!-- 权限搜索 -->
          <div class="perm-search-bar" v-if="selectedRole">
            <input
              type="text"
              v-model="permSearch"
              placeholder="搜索权限项..."
            />
            <span class="perm-search-hint" v-if="permSearch">
              匹配 {{ filteredPermItems.length }} 项
            </span>
          </div>

          <!-- 权限树 -->
          <div v-if="selectedRole" class="permission-tree">
            <div
              class="perm-group"
              v-for="group in permissionGroups"
              :key="group.id"
              v-show="!permSearch || groupHasMatch(group)"
            >
              <div class="perm-group-header" @click="togglePermGroup(group.id)">
                <span class="group-toggle" :class="{ collapsed: collapsedPermGroups[group.id] }">▶</span>
                <input
                  type="checkbox"
                  v-model="group.checked"
                  @click.stop
                  @change="toggleGroup(group)"
                  class="group-checkbox"
                />
                <span class="perm-group-icon">{{ group.icon }}</span>
                <span class="perm-group-name">{{ group.name }}</span>
                <span class="perm-group-count">{{ getCheckedCount(group) }}/{{ group.items.length }}</span>
              </div>
              <div class="perm-items" v-show="!collapsedPermGroups[group.id]">
                <div
                  class="perm-item"
                  v-for="item in group.items"
                  :key="item.id"
                  v-show="!permSearch || matchesSearch(item)"
                  @click="toggleItem(item)"
                >
                  <input type="checkbox" v-model="item.checked" @click.stop @change="updateGroupCheck(group)" />
                  <span class="perm-item-name">{{ item.name }}</span>
                  <span class="perm-item-desc">{{ item.desc }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- 未选择角色时的占位 -->
          <div v-else class="permission-placeholder">
            <div class="placeholder-icon">👈</div>
            <div class="placeholder-text">请从左侧选择一个角色</div>
            <div class="placeholder-desc">选择后即可查看和配置该角色的权限</div>
          </div>
        </div>
      </div>
    </div>

    <!-- ===== 新增角色弹窗 ===== -->
    <Modal
      v-model:open="showAddModal"
      title="新增角色"
      :footer="null"
      width="560px"
      @cancel="closeAddModal"
    >
      <div class="modal-form">
        <div class="form-group">
          <label class="form-label required">角色名称</label>
          <input v-model="addForm.name" type="text" placeholder="请输入角色名称" class="form-input" />
        </div>
        <div class="form-group">
          <label class="form-label required">角色级别</label>
          <select v-model="addForm.level" class="form-select">
            <option value="">请选择角色级别</option>
            <option value="1">国家级</option>
            <option value="2">省级</option>
            <option value="3">市级</option>
            <option value="4">区县级</option>
            <option value="5">学校级</option>
            <option value="6">班级级</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label required">状态</label>
          <select v-model="addForm.status" class="form-select">
            <option :value="1">启用</option>
            <option :value="0">禁用</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label">角色描述</label>
          <textarea v-model="addForm.desc" placeholder="请输入角色描述" class="form-textarea" rows="3"></textarea>
        </div>
        <div class="form-actions">
          <button class="btn btn-default" @click="closeAddModal">取消</button>
          <button class="btn btn-primary" @click="submitAddRole">确定</button>
        </div>
      </div>
    </Modal>

    <!-- ===== 编辑角色弹窗 ===== -->
    <Modal
      v-model:open="showEditModal"
      title="编辑角色"
      :footer="null"
      width="560px"
      @cancel="closeEditModal"
    >
      <div class="modal-form">
        <div class="form-group">
          <label class="form-label required">角色名称</label>
          <input v-model="editForm.name" type="text" placeholder="请输入角色名称" class="form-input" />
        </div>
        <div class="form-group">
          <label class="form-label required">角色级别</label>
          <select v-model="editForm.level" class="form-select">
            <option value="">请选择角色级别</option>
            <option value="1">国家级</option>
            <option value="2">省级</option>
            <option value="3">市级</option>
            <option value="4">区县级</option>
            <option value="5">学校级</option>
            <option value="6">班级级</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label required">状态</label>
          <select v-model="editForm.status" class="form-select">
            <option :value="1">启用</option>
            <option :value="0">禁用</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label">角色描述</label>
          <textarea v-model="editForm.desc" placeholder="请输入角色描述" class="form-textarea" rows="3"></textarea>
        </div>
        <div class="form-actions">
          <button class="btn btn-default" @click="closeEditModal">取消</button>
          <button class="btn btn-primary" @click="submitEditRole">确定</button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, reactive, watch, onMounted } from 'vue';
import { message, Modal } from 'ant-design-vue';

import { roleApi, menuApi } from '#/api/vision-archive/system';
import type { RoleDto, MenuDto } from '#/api/vision-archive/system';

// ================= 类型定义 =================
interface RoleInfo {
  id: number;
  icon: string;
  name: string;
  level: number;
  levelLabel: string;
  userCount: number;
  status: number;
  statusText: string;
  statusClass: string;
  permissions: string[];
  desc: string;
  code?: string;
  menuIds?: number[];
}

interface PermissionItem {
  id: string;
  name: string;
  desc: string;
  checked: boolean;
  menuId?: number;
}

interface PermissionGroup {
  id: string;
  icon: string;
  name: string;
  checked: boolean;
  items: PermissionItem[];
  menuId?: number;
}

// ================= 常量配置 =================
const LEVEL_MAP: Record<number, { label: string; icon: string }> = {
  1: { label: '国家级', icon: '🏛️' },
  2: { label: '省级', icon: '🏛️' },
  3: { label: '市级', icon: '🏙️' },
  4: { label: '区县级', icon: '🏘️' },
  5: { label: '学校级', icon: '🏫' },
  6: { label: '班级级', icon: '👨‍🏫' },
};

const LEVEL_ORDER = [1, 2, 3, 4, 5, 6];

const STATUS_MAP: Record<number, { text: string; class: string }> = {
  1: { text: '启用', class: 'status-active' },
  0: { text: '禁用', class: 'status-inactive' },
};

// Mock 回退数据（后端不可用时使用）
const MOCK_ROLES: RoleInfo[] = [
  {
    id: 1, icon: '🏛️', name: '国家管理员', level: 1, levelLabel: '国家级',
    userCount: 1, status: 1, statusText: '启用', statusClass: 'status-active',
    permissions: ['查看全国数据', '查看省级数据', '查看市级数据', '查看县级数据', '查看校级数据', '用户管理', '角色管理', '组织管理', '字典管理', '日志查看'],
    desc: '查看全国所有数据，统一下发标准、生成全国报表，管理省级账号',
    code: 'ROLE_NATIONAL', menuIds: [1, 2, 3, 4, 5, 7, 8, 9, 10, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24],
  },
  {
    id: 2, icon: '🏛️', name: '省级管理员', level: 2, levelLabel: '省级',
    userCount: 34, status: 1, statusText: '启用', statusClass: 'status-active',
    permissions: ['查看省级数据', '查看市级数据', '查看县级数据', '查看校级数据', '生成报表', '查看预警', '发布预警', '组织管理', '字典管理'],
    desc: '查看本省及各地市数据，生成省级报表、监控地市防控，管理市级账号',
    code: 'ROLE_PROVINCE', menuIds: [1, 3, 4, 5, 7, 8, 9, 10, 13, 14, 16, 17, 19, 22, 23],
  },
  {
    id: 3, icon: '🏙️', name: '市级管理员', level: 3, levelLabel: '市级',
    userCount: 333, status: 1, statusText: '启用', statusClass: 'status-active',
    permissions: ['查看市级数据', '查看县级数据', '查看校级数据', '生成报表', '查看预警', '发布预警', '干预管理', '组织管理'],
    desc: '查看本市及各县区数据，监控数据质量、发布预警，管理县级账号',
    code: 'ROLE_CITY', menuIds: [1, 4, 5, 7, 8, 9, 10, 13, 16, 17, 18, 22],
  },
  {
    id: 4, icon: '🏘️', name: '县级管理员', level: 4, levelLabel: '区县级',
    userCount: 2862, status: 1, statusText: '启用', statusClass: 'status-active',
    permissions: ['查看县级数据', '查看校级数据', '新增数据', '编辑数据', '审核数据', '生成报表', '查看预警', '组织管理', '字典管理'],
    desc: '查看本县所有学校数据，审核数据、督导防控工作，管理学校账号',
    code: 'ROLE_DISTRICT', menuIds: [1, 5, 7, 8, 9, 10, 13, 16, 17, 22, 23],
  },
  {
    id: 5, icon: '🏫', name: '学校管理员', level: 5, levelLabel: '学校级',
    userCount: 216600, status: 1, statusText: '启用', statusClass: 'status-active',
    permissions: ['查看校级数据', '新增数据', '编辑数据', '审核数据', '查看预警'],
    desc: '仅管理本校数据，录入/审核视力档案，查看本校统计',
    code: 'ROLE_SCHOOL', menuIds: [1, 6, 7, 8, 9, 10, 16],
  },
  {
    id: 6, icon: '👨‍⚕️', name: '校医', level: 5, levelLabel: '学校级',
    userCount: 185000, status: 1, statusText: '启用', statusClass: 'status-active',
    permissions: ['查看校级数据', '新增数据', '编辑数据', '审核数据'],
    desc: '录入/审核视力数据，查看本校统计分析',
    code: 'ROLE_DOCTOR', menuIds: [1, 6, 7, 8, 9],
  },
  {
    id: 7, icon: '👨‍🏫', name: '班主任', level: 6, levelLabel: '班级级',
    userCount: 980000, status: 1, statusText: '启用', statusClass: 'status-active',
    permissions: ['查看校级数据', '新增数据', '编辑数据'],
    desc: '仅管理本班学生数据，录入视力档案，查看本班统计',
    code: 'ROLE_TEACHER', menuIds: [1, 6, 7],
  },
];

const MOCK_MENU_TREE: MenuDto[] = [
  { id: 1, name: '数据查看权限', icon: '📊', type: 1, parentId: null, sort: 1, status: 1, children: [
    { id: 2, name: '查看全国数据', type: 2, parentId: 1, sort: 1, status: 1 },
    { id: 3, name: '查看省级数据', type: 2, parentId: 1, sort: 2, status: 1 },
    { id: 4, name: '查看市级数据', type: 2, parentId: 1, sort: 3, status: 1 },
    { id: 5, name: '查看县级数据', type: 2, parentId: 1, sort: 4, status: 1 },
    { id: 6, name: '查看校级数据', type: 2, parentId: 1, sort: 5, status: 1 },
  ]},
  { id: 7, name: '数据操作权限', icon: '📝', type: 1, parentId: null, sort: 2, status: 1, children: [
    { id: 8, name: '新增数据', type: 2, parentId: 7, sort: 1, status: 1 },
    { id: 9, name: '编辑数据', type: 2, parentId: 7, sort: 2, status: 1 },
    { id: 10, name: '删除数据', type: 2, parentId: 7, sort: 3, status: 1 },
    { id: 11, name: '审核数据', type: 2, parentId: 7, sort: 4, status: 1 },
    { id: 12, name: '导入导出', type: 2, parentId: 7, sort: 5, status: 1 },
  ]},
  { id: 13, name: '报表权限', icon: '📋', type: 1, parentId: null, sort: 3, status: 1, children: [
    { id: 14, name: '生成报表', type: 2, parentId: 13, sort: 1, status: 1 },
    { id: 15, name: '导出报表', type: 2, parentId: 13, sort: 2, status: 1 },
  ]},
  { id: 16, name: '预警权限', icon: '🔔', type: 1, parentId: null, sort: 4, status: 1, children: [
    { id: 17, name: '查看预警', type: 2, parentId: 16, sort: 1, status: 1 },
    { id: 18, name: '发布预警', type: 2, parentId: 16, sort: 2, status: 1 },
  ]},
  { id: 19, name: '系统管理权限', icon: '⚙️', type: 1, parentId: null, sort: 5, status: 1, children: [
    { id: 20, name: '用户管理', type: 2, parentId: 19, sort: 1, status: 1 },
    { id: 21, name: '角色管理', type: 2, parentId: 19, sort: 2, status: 1 },
    { id: 22, name: '组织管理', type: 2, parentId: 19, sort: 3, status: 1 },
    { id: 23, name: '字典管理', type: 2, parentId: 19, sort: 4, status: 1 },
    { id: 24, name: '日志查看', type: 2, parentId: 19, sort: 5, status: 1 },
  ]},
];

// ================= Tab 状态 =================
const subActiveTab = ref<'role' | 'permission'>('role');

// ================= 筛选状态 =================
const roleSearch = ref('');
const levelFilter = ref<number | string>('');
const statusFilter = ref<number | string>('');

// ================= 分页状态 =================
const currentPage = ref(1);
const pageSize = ref(12);
const totalCount = ref(0);
const loading = ref(false);
const submitting = ref(false);

// ================= 折叠状态 =================
const collapsedLevelGroups = reactive<Record<number, boolean>>({});
const collapsedRoleSelectGroups = reactive<Record<number, boolean>>({});
const collapsedPermGroups = reactive<Record<string, boolean>>({});
const expandPermMap = reactive<Record<number, boolean>>({});

LEVEL_ORDER.forEach(level => {
  collapsedLevelGroups[level] = false;
  collapsedRoleSelectGroups[level] = false;
});

// ================= 弹窗状态 =================
const showAddModal = ref(false);
const showEditModal = ref(false);
const editingRoleId = ref<number | null>(null);

// ================= 角色数据 =================
const roles = ref<RoleInfo[]>([]);

// ================= 菜单 & 权限组数据 =================
const menuTree = ref<MenuDto[]>([]);
const permissionGroups = ref<PermissionGroup[]>([]);

// ================= 权限搜索 =================
const roleSelectSearch = ref('');
const permSearch = ref('');

// ================= 新增/编辑表单 =================
const addForm = reactive({ name: '', level: '' as number | string, status: 1, desc: '' });
const editForm = reactive({ name: '', level: '' as number | string, status: 1, desc: '' });

// ================= 权限说明数据 =================
const legendData = [
  { icon: '🏛️', level: 'national', name: '国家管理员', desc: '查看全国所有数据，统一下发标准、生成全国报表，管理省级账号' },
  { icon: '🏛️', level: 'province', name: '省级管理员', desc: '查看本省及各地市数据，生成省级报表、监控地市防控，管理市级账号' },
  { icon: '🏙️', level: 'city', name: '市级管理员', desc: '查看本市及各县区数据，监控数据质量、发布预警，管理县级账号' },
  { icon: '🏘️', level: 'district', name: '县级管理员', desc: '查看本县所有学校数据，审核数据、督导防控工作，管理学校账号' },
  { icon: '🏫', level: 'school', name: '学校管理员/校医/班主任', desc: '仅管理本校/本班数据，录入/审核视力档案，查看本校/本班统计' },
];

// ================= 数据映射辅助 =================

/** 扁平化菜单树 -> 权限组（保留后端 menuId 用于提交） */
function menuTreeToPermissionGroups(tree: MenuDto[]): PermissionGroup[] {
  const groups: PermissionGroup[] = [];
  for (const group of tree) {
    if (group.type !== 1) continue;
    const items: PermissionItem[] = [];
    if (group.children) {
      for (const child of group.children) {
        const desc = child.name.replace(/^查看|^生成|^导出|^发布|^新增|^编辑|^删除|^审核|^导入|^预览/, '').trim() || child.name;
        items.push({
          id: `P${child.id}`,
          name: child.name,
          desc: desc === child.name ? child.name : desc,
          checked: false,
          menuId: child.id,
        });
      }
    }
    groups.push({
      id: `G${group.id}`,
      icon: group.icon || '📄',
      name: group.name,
      checked: false,
      menuId: group.id,
      items,
    });
  }
  return groups;
}

/** 菜单ID列表 -> 权限名称数组 */
function menuIdsToNames(menuIds: number[] | undefined): string[] {
  if (!menuIds || menuIds.length === 0) return [];
  const names: string[] = [];
  const walk = (nodes: MenuDto[]) => {
    for (const node of nodes) {
      if (menuIds.includes(node.id)) names.push(node.name);
      if (node.children) walk(node.children);
    }
  };
  walk(menuTree.value);
  return names;
}

/** 从权限组提取已勾选的菜单ID */
function getCheckedMenuIds(): number[] {
  const ids: number[] = [];
  for (const g of permissionGroups.value) {
    for (const item of g.items) {
      if (item.checked && item.menuId !== undefined) {
        ids.push(item.menuId);
      }
    }
  }
  return ids;
}

/** 后端 RoleDto -> 前端 RoleInfo */
function mapRoleDtoToInfo(dto: RoleDto): RoleInfo {
  const info = LEVEL_MAP[dto.level] || { label: `级别${dto.level}`, icon: '👤' };
  const statusInfo = STATUS_MAP[dto.status] || { text: '未知', class: 'status-inactive' };
  const menuIds = dto.menuIds || [];
  const permissions = menuIdsToNames(menuIds);
  return {
    id: dto.id,
    icon: info.icon,
    name: dto.name,
    level: dto.level,
    levelLabel: info.label,
    userCount: dto.userCount || 0,
    status: dto.status,
    statusText: statusInfo.text,
    statusClass: statusInfo.class,
    permissions: permissions.length > 0 ? permissions : ['暂无权限'],
    desc: dto.remark || '暂无描述',
    code: dto.code,
    menuIds,
  };
}

// ================= 数据加载 =================

async function fetchRoles() {
  loading.value = true;
  try {
    const params: Record<string, any> = {
      page: currentPage.value,
      pageSize: pageSize.value,
    };
    if (roleSearch.value.trim()) params.name = roleSearch.value.trim();
    if (levelFilter.value !== '' && levelFilter.value !== null && levelFilter.value !== undefined) {
      params.level = Number(levelFilter.value);
    }
    if (statusFilter.value !== '' && statusFilter.value !== null && statusFilter.value !== undefined) {
      params.status = Number(statusFilter.value);
    }
    const res = await roleApi.getPagedList(params);
    const list = Array.isArray(res?.items) ? res.items : [];
    totalCount.value = res?.total ?? 0;
    roles.value = list.map(mapRoleDtoToInfo);
  } catch (error: any) {
    console.warn('[fetchRoles] 后端不可用，使用 mock 数据:', error?.message || error);
    applyMockRoles();
  } finally {
    loading.value = false;
  }
}

function applyMockRoles() {
  let list = MOCK_ROLES;
  if (roleSearch.value.trim()) {
    list = list.filter(r => r.name.includes(roleSearch.value.trim()));
  }
  if (levelFilter.value !== '' && levelFilter.value !== null && levelFilter.value !== undefined) {
    list = list.filter(r => r.level === Number(levelFilter.value));
  }
  if (statusFilter.value !== '' && statusFilter.value !== null && statusFilter.value !== undefined) {
    list = list.filter(r => r.status === Number(statusFilter.value));
  }
  totalCount.value = list.length;
  const start = (currentPage.value - 1) * pageSize.value;
  roles.value = list.slice(start, start + pageSize.value);
  if (!apiWarned) {
    apiWarned = true;
    message.warning('后端服务未启动，已使用本地模拟数据。启动后端后将自动切换至真实数据。');
  }
}

let apiWarned = false;

async function fetchMenuTree() {
  try {
    const res = await menuApi.getTree();
    menuTree.value = Array.isArray(res) ? res : [];
    permissionGroups.value = menuTreeToPermissionGroups(menuTree.value);
  } catch (error: any) {
    console.warn('[fetchMenuTree] 后端不可用，使用 mock 菜单:', error?.message || error);
    menuTree.value = JSON.parse(JSON.stringify(MOCK_MENU_TREE));
    permissionGroups.value = menuTreeToPermissionGroups(menuTree.value);
  }
}

async function fetchRoleMenuIds(roleId: number): Promise<number[]> {
  try {
    const ids = await roleApi.getMenuPermission(roleId);
    return Array.isArray(ids) ? ids : [];
  } catch (error: any) {
    console.warn('[fetchRoleMenuIds] 获取角色权限失败:', error?.message || error);
    const role = roles.value.find(r => r.id === roleId);
    return role?.menuIds || [];
  }
}

// ================= 计算属性 =================

const filteredRoles = computed(() => roles.value);

const paginatedRoles = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  return filteredRoles.value.slice(start, start + pageSize.value);
});

const totalPages = computed(() => Math.max(1, Math.ceil(totalCount.value / pageSize.value)));

const groupedRoles = computed(() => {
  const groups: { levelKey: number; levelLabel: string; icon: string; roles: RoleInfo[] }[] = [];
  for (const level of LEVEL_ORDER) {
    const rolesOfLevel = paginatedRoles.value.filter(r => r.level === level);
    if (rolesOfLevel.length > 0) {
      const info = LEVEL_MAP[level];
      groups.push({
        levelKey: level,
        levelLabel: info.label,
        icon: info.icon,
        roles: rolesOfLevel,
      });
    }
  }
  return groups;
});

const filteredRolesForSelect = computed(() => {
  if (!roleSelectSearch.value) return roles.value;
  const kw = roleSelectSearch.value.toLowerCase();
  return roles.value.filter(r => r.name.toLowerCase().includes(kw));
});

const groupedRoleSelect = computed(() => {
  const groups: { levelKey: number; levelLabel: string; icon: string; roles: RoleInfo[] }[] = [];
  for (const level of LEVEL_ORDER) {
    const rolesOfLevel = filteredRolesForSelect.value.filter(r => r.level === level);
    if (rolesOfLevel.length > 0) {
      const info = LEVEL_MAP[level];
      groups.push({
        levelKey: level,
        levelLabel: info.label,
        icon: info.icon,
        roles: rolesOfLevel,
      });
    }
  }
  return groups;
});

const selectedRoleId = ref<number | null>(null);
const selectedRole = computed(() => roles.value.find(r => r.id === selectedRoleId.value) || null);

const allPermissionsSelected = computed(() => {
  const allItems = permissionGroups.value.flatMap(g => g.items);
  return allItems.length > 0 && allItems.every(i => i.checked);
});

const allExpanded = computed(() => {
  return permissionGroups.value.length > 0 &&
    permissionGroups.value.every(g => !collapsedPermGroups[g.id]);
});

const filteredPermItems = computed(() => {
  if (!permSearch.value) return [];
  const kw = permSearch.value.toLowerCase();
  return permissionGroups.value
    .flatMap(g => g.items)
    .filter(i => i.name.toLowerCase().includes(kw) || i.desc.toLowerCase().includes(kw));
});

// ================= 辅助方法 =================

const getDisplayPerms = (role: RoleInfo) => {
  return role.permissions;
};

const toggleExpandPerm = (roleId: number) => {
  expandPermMap[roleId] = !expandPermMap[roleId];
};

const getCheckedCount = (group: PermissionGroup) => group.items.filter(i => i.checked).length;

const updateGroupCheck = (group: PermissionGroup) => {
  group.checked = group.items.length > 0 && group.items.every(i => i.checked);
};

// ================= 折叠控制 =================

const toggleLevelGroup = (level: number) => {
  collapsedLevelGroups[level] = !collapsedLevelGroups[level];
};

const toggleRoleSelectGroup = (level: number) => {
  collapsedRoleSelectGroups[level] = !collapsedRoleSelectGroups[level];
};

const togglePermGroup = (groupId: string) => {
  collapsedPermGroups[groupId] = !collapsedPermGroups[groupId];
};

const toggleExpandAll = () => {
  const shouldExpand = !allExpanded.value;
  permissionGroups.value.forEach(g => {
    collapsedPermGroups[g.id] = !shouldExpand;
  });
};

// ================= 权限搜索辅助 =================

const matchesSearch = (item: PermissionItem) => {
  if (!permSearch.value) return true;
  const kw = permSearch.value.toLowerCase();
  return item.name.toLowerCase().includes(kw) || item.desc.toLowerCase().includes(kw);
};

const groupHasMatch = (group: PermissionGroup) => {
  if (!permSearch.value) return true;
  return group.items.some(item => matchesSearch(item));
};

// ================= 角色选择 & 权限加载 =================

const selectRole = async (role: RoleInfo) => {
  selectedRoleId.value = role.id;

  // 先加载菜单树（如果未加载）
  if (menuTree.value.length === 0) {
    await fetchMenuTree();
  }

  // 加载角色的已分配菜单ID
  const menuIds = await fetchRoleMenuIds(role.id);
  const idSet = new Set(menuIds);

  // 回填到权限组
  permissionGroups.value.forEach(group => {
    group.items.forEach(item => {
      item.checked = item.menuId !== undefined && idSet.has(item.menuId);
    });
    updateGroupCheck(group);
  });

  // 更新角色的 permissions 显示
  const permNames = menuIdsToNames(menuIds);
  const idx = roles.value.findIndex(r => r.id === role.id);
  if (idx !== -1) {
    roles.value[idx] = {
      ...roles.value[idx],
      menuIds,
      permissions: permNames.length > 0 ? permNames : ['暂无权限'],
    };
  }

  // 展开含选中权限的分组
  permissionGroups.value.forEach(group => {
    const hasChecked = group.items.some(item => item.checked);
    if (hasChecked) collapsedPermGroups[group.id] = false;
  });
};

// ================= 权限操作 =================
const toggleItem = (item: PermissionItem) => {
  item.checked = !item.checked;
  const group = permissionGroups.value.find(g => g.items.some(i => i.id === item.id));
  if (group) updateGroupCheck(group);
};

const toggleGroup = (group: PermissionGroup) => {
  group.items.forEach(item => {
    item.checked = group.checked;
  });
};

const toggleAllPermissions = () => {
  const newState = !allPermissionsSelected.value;
  permissionGroups.value.forEach(group => {
    group.items.forEach(item => {
      item.checked = newState;
    });
    updateGroupCheck(group);
  });
};

// ================= 权限保存/重置（对接后端） =================
const handleSavePermission = async () => {
  if (!selectedRoleId.value) {
    message.warning('请先选择角色');
    return;
  }
  const checkedMenuIds = getCheckedMenuIds();
  submitting.value = true;
  try {
    await roleApi.assignMenuPermission(selectedRoleId.value, checkedMenuIds);

    // 更新本地角色数据
    const permNames = menuIdsToNames(checkedMenuIds);
    const index = roles.value.findIndex(r => r.id === selectedRoleId.value);
    if (index !== -1) {
      roles.value[index] = {
        ...roles.value[index],
        menuIds: checkedMenuIds,
        permissions: permNames.length > 0 ? permNames : ['暂无权限'],
      };
    }
    const role = roles.value[index];
    message.success(`角色 "${role?.name}" 权限保存成功！共配置 ${checkedMenuIds.length} 项权限`);
  } catch (error: any) {
    console.error('[handleSavePermission] 保存权限失败:', error);
    const errorMsg = error?.response?.data?.message || error?.message || '保存失败';
    message.error(`保存失败：${errorMsg}`);
  } finally {
    submitting.value = false;
  }
};

const handleResetPermission = () => {
  if (!selectedRoleId.value) return;
  Modal.confirm({
    title: '确认重置',
    content: '确定要重置当前角色的权限配置吗？重置后将清空所有权限。',
    okText: '确定',
    cancelText: '取消',
    onOk: () => {
      permissionGroups.value.forEach(group => {
        group.items.forEach(item => { item.checked = false; });
        group.checked = false;
      });
      message.info('权限已重置，请点击"保存权限"生效');
    },
  });
};

// ================= 角色跳转 =================
const handleAssignPermission = (role: RoleInfo) => {
  selectRole(role);
  subActiveTab.value = 'permission';
  message.info(`正在为 ${role.name} 配置权限`);
};

// ================= 新增角色 =================
const handleAddRole = () => {
  addForm.name = '';
  addForm.level = '';
  addForm.status = 1;
  addForm.desc = '';
  showAddModal.value = true;
};

const closeAddModal = () => { showAddModal.value = false; };

const submitAddRole = async () => {
  if (!addForm.name || !addForm.level) {
    message.warning('请填写角色名称和角色级别');
    return;
  }
  submitting.value = true;
  try {
    // 自动生成 code（角色名转英文缩写）
    const code = `ROLE_${Date.now().toString().slice(-6)}`;
    const payload = {
      name: addForm.name,
      code,
      level: Number(addForm.level),
      status: addForm.status,
      remark: addForm.desc,
    };
    await roleApi.create(payload);
    message.success(`角色 "${addForm.name}" 创建成功`);
    closeAddModal();
    currentPage.value = 1;
    await fetchRoles();
  } catch (error: any) {
    console.error('[submitAddRole] 创建失败:', error);
    const errorMsg = error?.response?.data?.message || error?.message || '创建失败';
    message.error(`创建失败：${errorMsg}`);
  } finally {
    submitting.value = false;
  }
};

// ================= 编辑角色 =================
const handleEditRole = (role: RoleInfo) => {
  editingRoleId.value = role.id;
  editForm.name = role.name;
  editForm.level = String(role.level);
  editForm.status = role.status;
  editForm.desc = role.desc;
  showEditModal.value = true;
};

const closeEditModal = () => {
  showEditModal.value = false;
  editingRoleId.value = null;
};

const submitEditRole = async () => {
  if (!editForm.name || !editForm.level) {
    message.warning('请填写角色名称和角色级别');
    return;
  }
  if (!editingRoleId.value) {
    message.error('缺少角色ID');
    return;
  }
  submitting.value = true;
  try {
    const role = roles.value.find(r => r.id === editingRoleId.value);
    const payload = {
      id: editingRoleId.value,
      name: editForm.name,
      code: role?.code || `ROLE_${editingRoleId.value}`,
      level: Number(editForm.level),
      status: editForm.status,
      remark: editForm.desc,
    };
    await roleApi.update(editingRoleId.value, payload);
    message.success(`角色 "${editForm.name}" 已更新`);
    closeEditModal();
    await fetchRoles();
  } catch (error: any) {
    console.error('[submitEditRole] 更新失败:', error);
    const errorMsg = error?.response?.data?.message || error?.message || '更新失败';
    message.error(`更新失败：${errorMsg}`);
  } finally {
    submitting.value = false;
  }
};

// ================= 删除角色 =================
const handleDeleteRole = (role: RoleInfo) => {
  Modal.confirm({
    title: '确认删除',
    content: `确定要删除角色：${role.name} 吗？此操作不可恢复。`,
    okText: '确定',
    cancelText: '取消',
    async onOk() {
      try {
        await roleApi.delete(role.id);
        message.success(`角色 "${role.name}" 已删除`);
        if (selectedRoleId.value === role.id) selectedRoleId.value = null;
        await fetchRoles();
      } catch (error: any) {
        console.error('[handleDeleteRole] 删除失败:', error);
        const errorMsg = error?.response?.data?.message || error?.message || '删除失败';
        message.error(errorMsg);
      }
    },
  });
};

const handleBatchDeleteRole = () => {
  if (roles.value.length === 0) {
    message.warning('暂无角色可删除');
    return;
  }
  Modal.confirm({
    title: '确认批量删除',
    content: `确定要删除当前页 ${roles.value.length} 个角色吗？此操作不可恢复。`,
    okText: '确定',
    cancelText: '取消',
    async onOk() {
      try {
        const ids = roles.value.map(r => r.id);
        const res = await roleApi.batchDelete(ids);
        message.success(res?.message || `已删除 ${res?.deletedCount ?? 0} 个角色`);
        if (selectedRoleId.value && ids.includes(selectedRoleId.value)) {
          selectedRoleId.value = null;
        }
        await fetchRoles();
      } catch (error: any) {
        console.error('[handleBatchDeleteRole] 批量删除失败:', error);
        const errorMsg = error?.response?.data?.message || error?.message || '删除失败';
        message.error(errorMsg);
      }
    },
  });
};

// ================= 筛选重置 =================
const handleResetFilter = () => {
  roleSearch.value = '';
  levelFilter.value = '';
  statusFilter.value = '';
  currentPage.value = 1;
  fetchRoles();
};

// ================= 监听 =================
watch([roleSearch, levelFilter, statusFilter], () => {
  currentPage.value = 1;
});

watch(totalPages, (tp) => {
  if (currentPage.value > tp) {
    currentPage.value = Math.max(1, tp);
  }
});

// 分页变化时重新加载
watch(currentPage, () => {
  if (roles.value.length > 0 || currentPage.value !== 1) {
    fetchRoles();
  }
});

watch(pageSize, () => {
  currentPage.value = 1;
  if (roles.value.length > 0) {
    fetchRoles();
  }
});

// ================= 初始化 =================
onMounted(async () => {
  await Promise.all([fetchRoles(), fetchMenuTree()]);
});
</script>

<style scoped>
.permission-manage { width: 100%; }

/* ===== 子Tab切换 ===== */
.sub-tab-bar {
  display: flex;
  gap: 4px;
  background: #fff;
  padding: 6px 20px;
  border-radius: 8px;
  margin-bottom: 16px;
  border: 1px solid #ebeef5;
}
.sub-tab-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 20px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #606266;
  font-size: 14px;
  font-weight: 500;
  position: relative;
}
.sub-tab-item:hover { background-color: #f5f7fa; color: #409eff; }
.sub-tab-item.active {
  background: linear-gradient(135deg, #e8f4fd, #d4e8f7);
  color: #409eff;
  box-shadow: 0 2px 8px rgba(64, 158, 255, 0.12);
}
.sub-tab-item .sub-tab-icon { font-size: 16px; }
.sub-tab-item .sub-tab-badge {
  background: #c0c4cc;
  color: #fff;
  font-size: 11px;
  padding: 1px 8px;
  border-radius: 10px;
  margin-left: 4px;
}
.sub-tab-item.active .sub-tab-badge { background: #409eff; }

.sub-tab-content { animation: fadeIn 0.3s ease; }
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(6px); }
  to { opacity: 1; transform: translateY(0); }
}

/* ===== 工具栏 ===== */
.toolbar {
  background: #fff;
  border-radius: 10px;
  padding: 14px 20px;
  margin-bottom: 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
  flex-wrap: wrap;
  border: 1px solid #ebeef5;
}
.toolbar-left { display: flex; align-items: center; gap: 10px; flex-wrap: wrap; }
.toolbar-right { display: flex; align-items: center; gap: 10px; }
.search-box input {
  padding: 6px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 13px;
  outline: none;
  width: 200px;
  transition: border-color 0.3s;
}
.search-box input:focus { border-color: #409eff; }

.btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 6px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 500;
  transition: all 0.3s;
}
.btn-sm { padding: 4px 12px; font-size: 12px; }
.btn-primary { background: #409eff; color: #fff; }
.btn-primary:hover { background: #66b1ff; }
.btn-success { background: #67c23a; color: #fff; }
.btn-success:hover { background: #85ce61; }
.btn-danger { background: #f56c6c; color: #fff; }
.btn-danger:hover { background: #f78989; }
.btn-info { background: #909399; color: #fff; }
.btn-info:hover { background: #a6a9ad; }
.btn-default { background: #fff; border: 1px solid #dcdfe6; color: #606266; }
.btn-default:hover { color: #409eff; border-color: #409eff; }
.btn-filter { min-width: 100px; }
.btn .icon { font-size: 14px; }
.btn:disabled { opacity: 0.6; cursor: not-allowed; }

/* ===== 角色分组 ===== */
.role-groups { display: flex; flex-direction: column; gap: 16px; }
.role-group {
  background: #fff;
  border-radius: 10px;
  border: 1px solid #ebeef5;
  overflow: hidden;
}
.role-group-header {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 20px;
  background: linear-gradient(135deg, #f8fafd, #eef3fb);
  cursor: pointer;
  user-select: none;
  transition: background 0.2s;
}
.role-group-header:hover { background: linear-gradient(135deg, #eef3fb, #e3ecf8); }
.role-group-header .group-toggle {
  font-size: 10px;
  color: #909399;
  transition: transform 0.2s;
  width: 14px;
  text-align: center;
}
.role-group-header .group-toggle.collapsed { transform: rotate(-90deg); }
.role-group-header .group-icon { font-size: 18px; }
.role-group-header .group-name {
  font-weight: 600;
  color: #303133;
  font-size: 14px;
}
.role-group-header .group-count {
  font-size: 12px;
  color: #909399;
  background: #e8ecf1;
  padding: 1px 10px;
  border-radius: 10px;
  margin-left: auto;
}

/* ===== 角色列表（自适应网格） ===== */
.role-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(340px, 1fr));
  gap: 14px;
  padding: 14px 20px;
}
.role-card {
  background: #fafbfc;
  border-radius: 8px;
  padding: 14px 16px;
  border: 1px solid #ebeef5;
  transition: all 0.3s;
}
.role-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  transform: translateY(-2px);
}

.role-header {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}
.role-icon { font-size: 22px; }
.role-name { font-weight: 600; color: #303133; font-size: 15px; }
.role-level {
  font-size: 11px;
  padding: 1px 10px;
  border-radius: 10px;
  background: #e8f0fe;
  color: #409eff;
}
.role-user-count { font-size: 12px; color: #909399; }
.role-status {
  font-size: 11px;
  padding: 1px 10px;
  border-radius: 10px;
}
.role-status.status-active { background: #e8f5e9; color: #52c41a; }
.role-status.status-inactive { background: #fce4ec; color: #f56c6c; }
.role-actions { display: flex; gap: 4px; margin-left: auto; }

.role-permissions {
  display: flex;
  flex-wrap: wrap;
  gap: 5px;
  margin: 8px 0;
}
.permission-tag {
  font-size: 11px;
  padding: 2px 8px;
  border-radius: 10px;
  background: #e8f0fe;
  color: #409eff;
  cursor: default;
  transition: background 0.2s;
}
.permission-tag.more-tag {
  background: #f0f2f5;
  color: #606266;
  cursor: pointer;
}
.permission-tag.more-tag:hover { background: #d9ecff; color: #409eff; }
.role-desc { font-size: 13px; color: #909399; }

/* ===== 空状态 ===== */
.empty-state {
  background: #fff;
  border-radius: 10px;
  padding: 60px 20px;
  text-align: center;
  border: 1px solid #ebeef5;
}
.empty-icon { font-size: 48px; margin-bottom: 12px; }
.empty-text { font-size: 16px; font-weight: 500; color: #606266; }
.empty-desc { font-size: 13px; color: #909399; margin-top: 4px; }

/* ===== 分页 ===== */
.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  margin-top: 20px;
  padding: 12px 0;
}
.page-btn {
  padding: 6px 16px;
  border: 1px solid #dcdfe6;
  background: #fff;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  color: #606266;
  transition: all 0.2s;
}
.page-btn:hover:not(:disabled) { color: #409eff; border-color: #409eff; }
.page-btn:disabled { opacity: 0.5; cursor: not-allowed; }
.page-info { font-size: 13px; color: #606266; }
.page-size-select {
  padding: 6px 10px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 13px;
  outline: none;
}

/* ===== 权限分配 ===== */
.permission-assign {
  display: grid;
  grid-template-columns: 240px 1fr;
  gap: 20px;
  background: #fff;
  border-radius: 10px;
  padding: 20px;
  border: 1px solid #ebeef5;
  min-height: 480px;
}

/* 左侧角色列表 */
.assign-left { border-right: 1px solid #ebeef5; padding-right: 16px; }
.assign-header-left {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}
.assign-header-left h4 {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
  margin: 0;
}
.assign-hint {
  font-size: 11px;
  color: #909399;
  background: #f5f7fa;
  padding: 2px 10px;
  border-radius: 10px;
}

.role-search { margin-bottom: 12px; }
.role-search input {
  width: 100%;
  padding: 6px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 13px;
  outline: none;
  transition: border-color 0.3s;
  box-sizing: border-box;
}
.role-search input:focus { border-color: #409eff; }

.role-select-list {
  display: flex;
  flex-direction: column;
  gap: 4px;
  max-height: 500px;
  overflow-y: auto;
}
.role-select-list::-webkit-scrollbar { width: 4px; }
.role-select-list::-webkit-scrollbar-thumb { background: #d0d5dd; border-radius: 2px; }
.role-select-list::-webkit-scrollbar-track { background: transparent; }

.role-select-group { display: flex; flex-direction: column; }
.role-select-group-header {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 10px;
  background: #f5f7fa;
  border-radius: 6px;
  cursor: pointer;
  user-select: none;
  font-size: 13px;
  font-weight: 500;
  color: #606266;
  transition: background 0.2s;
}
.role-select-group-header:hover { background: #e8f0fe; color: #409eff; }
.role-select-group-header .group-toggle {
  font-size: 9px;
  color: #909399;
  transition: transform 0.2s;
  width: 12px;
  text-align: center;
}
.role-select-group-header .group-toggle.collapsed { transform: rotate(-90deg); }
.role-select-group-header .group-icon { font-size: 14px; }
.role-select-group-header .group-count {
  margin-left: auto;
  font-size: 11px;
  color: #909399;
  background: #e8ecf1;
  padding: 0 8px;
  border-radius: 8px;
}

.role-select-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px 8px 28px;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 13px;
}
.role-select-item:hover { background: #f5f7fa; }
.role-select-item.active {
  background: #e8f4fd;
  color: #409eff;
  box-shadow: inset 3px 0 0 #409eff;
  font-weight: 500;
}
.role-select-item .role-icon { font-size: 14px; }
.role-select-item .role-name { flex: 1; }
.role-select-item .role-level {
  font-size: 10px;
  padding: 1px 8px;
  border-radius: 10px;
  background: #e8f0fe;
  color: #409eff;
}
.role-select-item .role-status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
}
.role-select-item .role-status-dot.status-active { background: #52c41a; }
.role-select-item .role-status-dot.status-inactive { background: #f56c6c; }

/* 右侧权限配置 */
.assign-right {
  padding-left: 16px;
  display: flex;
  flex-direction: column;
  flex: 1;
  min-height: 0;
}
.assign-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  flex-wrap: wrap;
  gap: 8px;
  flex-shrink: 0;
}
.assign-header h4 {
  font-size: 15px;
  font-weight: 600;
  color: #303133;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 6px;
}
.selected-role-name { color: #409eff; }
.assign-subtitle { color: #909399; font-weight: 400; font-size: 14px; }
.placeholder-text { color: #c0c4cc; }
.assign-actions { display: flex; gap: 8px; flex-wrap: wrap; }

/* 权限搜索 */
.perm-search-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 12px;
}
.perm-search-bar input {
  flex: 1;
  padding: 6px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 13px;
  outline: none;
  transition: border-color 0.3s;
}
.perm-search-bar input:focus { border-color: #409eff; }
.perm-search-hint { font-size: 12px; color: #909399; }

/* 权限树 */
.permission-tree {
  display: flex;
  flex-direction: column;
  gap: 10px;
  flex: 1;
  overflow-y: auto;
  padding-right: 4px;
  min-height: 0;
}
.permission-tree::-webkit-scrollbar { width: 4px; }
.permission-tree::-webkit-scrollbar-thumb { background: #d0d5dd; border-radius: 2px; }
.permission-tree::-webkit-scrollbar-track { background: transparent; }

.perm-group {
  border: 1px solid #ebeef5;
  border-radius: 8px;
  overflow: hidden;
  transition: box-shadow 0.2s;
  flex-shrink: 0;
}
.perm-group:hover { box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06); }

.perm-group-header {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 14px;
  background: #fafbfc;
  border-bottom: 1px solid #ebeef5;
  cursor: pointer;
  transition: background 0.2s;
}
.perm-group-header:hover { background: #f0f2f5; }
.perm-group-header .group-toggle {
  font-size: 10px;
  color: #909399;
  transition: transform 0.2s;
  width: 14px;
  text-align: center;
}
.perm-group-header .group-toggle.collapsed { transform: rotate(-90deg); }
.group-checkbox {
  width: 16px;
  height: 16px;
  cursor: pointer;
  accent-color: #409eff;
}
.perm-group-icon { font-size: 16px; }
.perm-group-name { font-weight: 500; color: #303133; flex: 1; }
.perm-group-count {
  font-size: 11px;
  color: #909399;
  background: #e8ecf1;
  padding: 1px 10px;
  border-radius: 10px;
}

.perm-items {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2px;
  padding: 8px 14px;
}
.perm-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 8px;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.2s;
}
.perm-item:hover { background: #f5f7fa; }
.perm-item input[type="checkbox"] {
  width: 15px;
  height: 15px;
  cursor: pointer;
  accent-color: #409eff;
}
.perm-item-name {
  font-size: 13px;
  color: #303133;
  font-weight: 500;
  min-width: 80px;
}
.perm-item-desc { font-size: 12px; color: #909399; }

/* 权限占位 */
.permission-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  flex: 1;
  color: #c0c4cc;
  background: #fafbfc;
  border-radius: 8px;
  border: 1px dashed #dcdfe6;
  min-height: 320px;
}
.placeholder-icon { font-size: 48px; margin-bottom: 12px; }
.placeholder-text { font-size: 16px; font-weight: 500; color: #909399; }
.placeholder-desc { font-size: 13px; color: #c0c4cc; margin-top: 4px; }

/* ===== 五级权限体系说明 ===== */
.permission-legend {
  background: #fff;
  border-radius: 10px;
  padding: 16px 20px;
  border: 1px solid #ebeef5;
  margin-top: 16px;
}
.permission-legend h4 {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
  margin: 0 0 12px 0;
}
.legend-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 6px 0;
  border-bottom: 1px solid #f0f0f0;
}
.legend-item:last-child { border-bottom: none; }
.legend-icon { font-size: 18px; }
.legend-name { font-weight: 500; color: #303133; min-width: 160px; }
.legend-desc { color: #909399; font-size: 13px; }

/* ===== 弹窗表单 ===== */
.modal-form { padding: 8px 0; }
.form-group { margin-bottom: 16px; }
.form-label {
  display: block;
  font-size: 14px;
  color: #303133;
  font-weight: 500;
  margin-bottom: 4px;
}
.form-label.required::after {
  content: '*';
  color: #f56c6c;
  margin-left: 4px;
}
.form-input, .form-select, .form-textarea {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 14px;
  outline: none;
  transition: border-color 0.3s;
  background: #fff;
  font-family: inherit;
  box-sizing: border-box;
}
.form-input:focus, .form-select:focus, .form-textarea:focus {
  border-color: #409eff;
  box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.1);
}
.form-select {
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23606666' d='M6 8L1 3h10z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 12px center;
}
.form-textarea { resize: vertical; min-height: 60px; }

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding-top: 16px;
  border-top: 1px solid #ebeef5;
  margin-top: 4px;
}

/* ===== 响应式 ===== */
@media (max-width: 1200px) {
  .role-list { grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); }
  .permission-assign { grid-template-columns: 1fr; }
  .assign-left { border-right: none; border-bottom: 1px solid #ebeef5; padding-right: 0; padding-bottom: 16px; }
  .assign-right { padding-left: 0; }
  .perm-items { grid-template-columns: 1fr; }
}
@media (max-width: 768px) {
  .toolbar { flex-direction: column; align-items: stretch; }
  .toolbar-left, .toolbar-right { justify-content: flex-start; }
  .search-box input { width: 100%; }
  .role-header { flex-direction: column; align-items: stretch; }
  .role-actions { margin-left: 0; }
  .sub-tab-item { padding: 6px 12px; font-size: 13px; }
  .assign-header { flex-direction: column; align-items: stretch; }
  .assign-actions { justify-content: flex-start; }
  .perm-items { grid-template-columns: 1fr; }
  .legend-item { flex-wrap: wrap; }
  .legend-name { min-width: auto; }
}
</style>