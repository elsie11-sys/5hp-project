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
        <span class="sub-tab-badge">{{ roles.length }}</span>
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

    <!-- 角色管理 -->
    <div v-show="subActiveTab === 'role'" class="sub-tab-content">
      <!-- 操作栏 -->
      <div class="toolbar">
        <button class="btn btn-primary" @click="handleAddRole">
          <span class="icon">➕</span> 新增角色
        </button>
        <button class="btn btn-danger" @click="handleBatchDeleteRole">
          <span class="icon">🗑️</span> 删除角色
        </button>
        <div class="search-box">
          <input 
            type="text" 
            v-model="roleSearch" 
            placeholder="搜索角色名称..." 
            @keyup.enter="handleRoleSearch"
          />
          <button class="btn btn-sm btn-primary" @click="handleRoleSearch">搜索</button>
        </div>
      </div>

      <!-- 角色列表 -->
      <div class="role-list">
        <div class="role-card" v-for="role in filteredRoles" :key="role.id">
          <div class="role-header">
            <span class="role-icon">{{ role.icon }}</span>
            <span class="role-name">{{ role.name }}</span>
            <span class="role-level">{{ role.level }}</span>
            <span class="role-user-count">👤 {{ role.userCount }}人</span>
            <span class="role-status" :class="role.statusClass">{{ role.status }}</span>
            <div class="role-actions">
              <button class="btn btn-sm btn-primary" @click="handleAssignPermission(role)">分配权限</button>
              <button class="btn btn-sm btn-info" @click="handleEditRole(role)">编辑</button>
              <button class="btn btn-sm btn-danger" @click="handleDeleteRole(role)">删除</button>
            </div>
          </div>
          <div class="role-permissions">
            <span class="permission-tag" v-for="perm in role.permissions" :key="perm">
              {{ perm }}
            </span>
          </div>
          <div class="role-desc">{{ role.desc }}</div>
        </div>
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

    <!-- 权限分配 -->
    <div v-show="subActiveTab === 'permission'" class="sub-tab-content">
      <div class="permission-assign">
        <!-- 左侧：角色列表 -->
        <div class="assign-left">
          <div class="assign-header-left">
            <h4>📋 选择角色</h4>
            <span class="assign-hint">点击角色加载权限</span>
          </div>
          <div class="role-select-list">
            <div 
              class="role-select-item" 
              v-for="role in roles" 
              :key="role.id"
              :class="{ active: selectedRoleId === role.id }"
              @click="selectRole(role)"
            >
              <span class="role-icon">{{ role.icon }}</span>
              <span class="role-name">{{ role.name }}</span>
              <span class="role-level">{{ role.level }}</span>
              <span class="role-status-dot" :class="role.statusClass"></span>
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
              <button class="btn btn-success btn-sm" @click="handleSavePermission" :disabled="!selectedRole">
                <span class="icon">💾</span> 保存权限
              </button>
              <button class="btn btn-default btn-sm" @click="handleResetPermission" :disabled="!selectedRole">
                <span class="icon">↺</span> 重置
              </button>
            </div>
          </div>

          <!-- 权限树 -->
          <div v-if="selectedRole" class="permission-tree">
            <div class="perm-group" v-for="group in permissionGroups" :key="group.id">
              <!-- 权限组头部 - 点击复选框全选/取消全选 -->
              <div class="perm-group-header">
                <input 
                  type="checkbox" 
                  v-model="group.checked" 
                  @change="toggleGroup(group)" 
                  class="group-checkbox"
                />
                <span class="perm-group-icon">{{ group.icon }}</span>
                <span class="perm-group-name">{{ group.name }}</span>
                <span class="perm-group-count">{{ getCheckedCount(group) }}/{{ group.items.length }}</span>
              </div>
              <div class="perm-items">
                <div 
                  class="perm-item" 
                  v-for="item in group.items" 
                  :key="item.id"
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
            <option value="国家级">国家级</option>
            <option value="省级">省级</option>
            <option value="市级">市级</option>
            <option value="区县级">区县级</option>
            <option value="学校级">学校级</option>
            <option value="班级级">班级级</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label required">状态</label>
          <select v-model="addForm.status" class="form-select">
            <option value="启用">启用</option>
            <option value="禁用">禁用</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label">角色描述</label>
          <textarea v-model="addForm.desc" placeholder="请输入角色描述" class="form-textarea" rows="3"></textarea>
        </div>
        <div class="form-group">
          <label class="form-label">权限列表</label>
          <div class="permission-checkbox-group">
            <div class="perm-check-item" v-for="perm in addForm.permissions" :key="perm.id">
              <label>
                <input type="checkbox" v-model="perm.checked" />
                {{ perm.name }}
              </label>
            </div>
          </div>
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
            <option value="国家级">国家级</option>
            <option value="省级">省级</option>
            <option value="市级">市级</option>
            <option value="区县级">区县级</option>
            <option value="学校级">学校级</option>
            <option value="班级级">班级级</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label required">状态</label>
          <select v-model="editForm.status" class="form-select">
            <option value="启用">启用</option>
            <option value="禁用">禁用</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label">角色描述</label>
          <textarea v-model="editForm.desc" placeholder="请输入角色描述" class="form-textarea" rows="3"></textarea>
        </div>
        <div class="form-group">
          <label class="form-label">权限列表</label>
          <div class="permission-checkbox-group">
            <div class="perm-check-item" v-for="perm in editForm.permissions" :key="perm.id">
              <label>
                <input type="checkbox" v-model="perm.checked" />
                {{ perm.name }}
              </label>
            </div>
          </div>
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
import { ref, computed, reactive, watch } from 'vue';
import { message, Modal } from 'ant-design-vue';

const subActiveTab = ref('role');
const roleSearch = ref('');
const selectedRoleId = ref<number | null>(null);

// ================= 弹窗状态 =================
const showAddModal = ref(false);
const showEditModal = ref(false);
const editingRoleId = ref<number | null>(null);

// ================= 角色数据 =================
const roles = ref([
  {
    id: 1,
    icon: '🏛️',
    name: '国家管理员',
    level: '国家级',
    userCount: 1,
    status: '启用',
    statusClass: 'status-active',
    permissions: ['查看全国数据', '下发标准', '生成全国报表', '管理省级账号'],
    desc: '查看全国所有数据，统一下发标准、生成全国报表，管理省级账号'
  },
  {
    id: 2,
    icon: '🏛️',
    name: '省级管理员',
    level: '省级',
    userCount: 34,
    status: '启用',
    statusClass: 'status-active',
    permissions: ['查看本省数据', '生成省级报表', '监控地市防控', '管理市级账号'],
    desc: '查看本省及各地市数据，生成省级报表、监控地市防控，管理市级账号'
  },
  {
    id: 3,
    icon: '🏙️',
    name: '市级管理员',
    level: '市级',
    userCount: 333,
    status: '启用',
    statusClass: 'status-active',
    permissions: ['查看本市数据', '监控数据质量', '发布预警', '管理县级账号'],
    desc: '查看本市及各县区数据，监控数据质量、发布预警，管理县级账号'
  },
  {
    id: 4,
    icon: '🏘️',
    name: '县级管理员',
    level: '区县级',
    userCount: 2862,
    status: '启用',
    statusClass: 'status-active',
    permissions: ['查看本县数据', '审核数据', '督导防控', '管理学校账号'],
    desc: '查看本县所有学校数据，审核数据、督导防控工作，管理学校账号'
  },
  {
    id: 5,
    icon: '🏫',
    name: '学校管理员',
    level: '学校级',
    userCount: 216600,
    status: '启用',
    statusClass: 'status-active',
    permissions: ['管理本校数据', '录入档案', '审核档案', '查看本校统计'],
    desc: '仅管理本校数据，录入/审核视力档案，查看本校统计'
  },
  {
    id: 6,
    icon: '👨‍⚕️',
    name: '校医',
    level: '学校级',
    userCount: 185000,
    status: '启用',
    statusClass: 'status-active',
    permissions: ['录入数据', '审核数据', '查看本校统计'],
    desc: '录入/审核视力数据，查看本校统计分析'
  },
  {
    id: 7,
    icon: '👨‍🏫',
    name: '班主任',
    level: '班级级',
    userCount: 980000,
    status: '启用',
    statusClass: 'status-active',
    permissions: ['管理本班数据', '录入档案', '查看本班统计'],
    desc: '仅管理本班学生数据，录入视力档案，查看本班统计'
  }
]);

// ================= 可用权限列表 =================
const availablePermissions = [
  { id: 'P001', name: '查看全国数据' },
  { id: 'P002', name: '查看省级数据' },
  { id: 'P003', name: '查看市级数据' },
  { id: 'P004', name: '查看县级数据' },
  { id: 'P005', name: '查看校级数据' },
  { id: 'P006', name: '新增数据' },
  { id: 'P007', name: '编辑数据' },
  { id: 'P008', name: '删除数据' },
  { id: 'P009', name: '审核数据' },
  { id: 'P010', name: '导入数据' },
  { id: 'P011', name: '导出数据' },
  { id: 'P012', name: '生成报表' },
  { id: 'P013', name: '预览报表' },
  { id: 'P014', name: '导出报表' },
  { id: 'P015', name: '查看预警' },
  { id: 'P016', name: '发布预警' },
  { id: 'P017', name: '干预管理' },
  { id: 'P018', name: '用户管理' },
  { id: 'P019', name: '角色管理' },
  { id: 'P020', name: '组织管理' },
  { id: 'P021', name: '字典管理' },
  { id: 'P022', name: '日志查看' }
];

// ================= 权限组数据 =================
const permissionGroups = ref([
  {
    id: 'G001',
    icon: '📊',
    name: '数据查看权限',
    checked: false,
    items: [
      { id: 'P001', name: '查看全国数据', desc: '查看全国所有数据', checked: false },
      { id: 'P002', name: '查看省级数据', desc: '查看本省数据', checked: false },
      { id: 'P003', name: '查看市级数据', desc: '查看本市数据', checked: false },
      { id: 'P004', name: '查看县级数据', desc: '查看本县数据', checked: false },
      { id: 'P005', name: '查看校级数据', desc: '查看本校数据', checked: false },
    ]
  },
  {
    id: 'G002',
    icon: '📝',
    name: '数据操作权限',
    checked: false,
    items: [
      { id: 'P006', name: '新增数据', desc: '新增学生档案/检测数据', checked: false },
      { id: 'P007', name: '编辑数据', desc: '编辑已有数据', checked: false },
      { id: 'P008', name: '删除数据', desc: '删除数据', checked: false },
      { id: 'P009', name: '审核数据', desc: '审核数据', checked: false },
      { id: 'P010', name: '导入数据', desc: '批量导入数据', checked: false },
      { id: 'P011', name: '导出数据', desc: '导出报表/数据', checked: false },
    ]
  },
  {
    id: 'G003',
    icon: '📋',
    name: '报表权限',
    checked: false,
    items: [
      { id: 'P012', name: '生成报表', desc: '生成统计报表', checked: false },
      { id: 'P013', name: '预览报表', desc: '预览报表', checked: false },
      { id: 'P014', name: '导出报表', desc: '导出Excel/PDF报表', checked: false },
    ]
  },
  {
    id: 'G004',
    icon: '🔔',
    name: '预警权限',
    checked: false,
    items: [
      { id: 'P015', name: '查看预警', desc: '查看预警信息', checked: false },
      { id: 'P016', name: '发布预警', desc: '发布预警通知', checked: false },
      { id: 'P017', name: '干预管理', desc: '管理干预方案', checked: false },
    ]
  },
  {
    id: 'G005',
    icon: '⚙️',
    name: '系统管理权限',
    checked: false,
    items: [
      { id: 'P018', name: '用户管理', desc: '管理用户账号', checked: false },
      { id: 'P019', name: '角色管理', desc: '管理角色', checked: false },
      { id: 'P020', name: '组织管理', desc: '管理组织架构', checked: false },
      { id: 'P021', name: '字典管理', desc: '管理基础字典', checked: false },
      { id: 'P022', name: '日志查看', desc: '查看操作日志', checked: false },
    ]
  }
]);

// ================= 新增表单 =================
const defaultAddForm = {
  name: '',
  level: '',
  status: '启用',
  desc: '',
  permissions: availablePermissions.map(p => ({ ...p, checked: false }))
};
const addForm = reactive({ ...defaultAddForm });

// ================= 编辑表单 =================
const defaultEditForm = {
  name: '',
  level: '',
  status: '启用',
  desc: '',
  permissions: availablePermissions.map(p => ({ ...p, checked: false }))
};
const editForm = reactive({ ...defaultEditForm });

// ================= 权限说明数据 =================
const legendData = [
  { icon: '🏛️', level: 'national', name: '国家管理员', desc: '查看全国所有数据，统一下发标准、生成全国报表，管理省级账号' },
  { icon: '🏛️', level: 'province', name: '省级管理员', desc: '查看本省及各地市数据，生成省级报表、监控地市防控，管理市级账号' },
  { icon: '🏙️', level: 'city', name: '市级管理员', desc: '查看本市及各县区数据，监控数据质量、发布预警，管理县级账号' },
  { icon: '🏘️', level: 'district', name: '县级管理员', desc: '查看本县所有学校数据，审核数据、督导防控工作，管理学校账号' },
  { icon: '🏫', level: 'school', name: '学校管理员/校医/班主任', desc: '仅管理本校/本班数据，录入/审核视力档案，查看本校/本班统计' }
];

// ================= 计算属性 =================
const selectedRole = computed(() => roles.value.find(r => r.id === selectedRoleId.value));

const filteredRoles = computed(() => {
  if (!roleSearch.value) return roles.value;
  return roles.value.filter(r => r.name.includes(roleSearch.value));
});

// ================= 辅助方法 =================
const getCheckedCount = (group: any) => {
  return group.items.filter((item: any) => item.checked).length;
};

const updateGroupCheck = (group: any) => {
  group.checked = group.items.every((item: any) => item.checked);
};

// ================= 选择角色（加载权限） =================
const selectRole = (role: any) => {
  selectedRoleId.value = role.id;
  const roleData = roles.value.find(r => r.id === role.id);
  if (roleData) {
    permissionGroups.value.forEach(group => {
      group.items.forEach(item => {
        item.checked = roleData.permissions.includes(item.name);
      });
      group.checked = group.items.every(item => item.checked);
    });
  }
};

// ================= 切换单个权限项 =================
const toggleItem = (item: any) => {
  item.checked = !item.checked;
  const group = permissionGroups.value.find(g => g.items.some(i => i.id === item.id));
  if (group) {
    updateGroupCheck(group);
  }
};

// ================= 切换权限组（全选/取消全选） =================
const toggleGroup = (group: any) => {
  // group.checked 已经被 v-model 更新
  group.items.forEach((item: any) => {
    item.checked = group.checked;
  });
};

// ================= 从角色管理跳转到权限分配 =================
const handleAssignPermission = (role: any) => {
  selectRole(role);
  subActiveTab.value = 'permission';
  message.info(`正在为 ${role.name} 配置权限`);
};

// ================= 保存权限（更新角色权限） =================
const handleSavePermission = () => {
  if (!selectedRoleId.value) {
    message.warning('请先选择角色');
    return;
  }
  
  // 获取所有选中的权限名称
  const selectedPerms = permissionGroups.value
    .flatMap(group => group.items)
    .filter(item => item.checked)
    .map(item => item.name);
  
  // 更新对应角色的权限
  const index = roles.value.findIndex(r => r.id === selectedRoleId.value);
  if (index === -1) {
    message.error('角色不存在');
    return;
  }
  
  const roleName = roles.value[index].name;
  roles.value[index] = {
    ...roles.value[index],
    permissions: selectedPerms.length > 0 ? selectedPerms : ['暂无权限']
  };
  
  message.success(`角色 "${roleName}" 权限保存成功！共配置 ${selectedPerms.length} 项权限`);
};

// ================= 重置权限 =================
const handleResetPermission = () => {
  if (!selectedRoleId.value) return;
  
  Modal.confirm({
    title: '确认重置',
    content: '确定要重置当前角色的权限配置吗？重置后将清空所有权限。',
    okText: '确定',
    cancelText: '取消',
    onOk: () => {
      permissionGroups.value.forEach(group => {
        group.items.forEach(item => {
          item.checked = false;
        });
        group.checked = false;
      });
      message.info('权限已重置，请点击"保存权限"生效');
    }
  });
};

// ================= 新增角色 =================
const handleAddRole = () => {
  addForm.name = '';
  addForm.level = '';
  addForm.status = '启用';
  addForm.desc = '';
  addForm.permissions = availablePermissions.map(p => ({ ...p, checked: false }));
  showAddModal.value = true;
};

const closeAddModal = () => {
  showAddModal.value = false;
};

const submitAddRole = () => {
  if (!addForm.name || !addForm.level) {
    message.warning('请填写角色名称和角色级别');
    return;
  }
  
  const maxId = roles.value.reduce((max, r) => r.id > max ? r.id : max, 0);
  const newId = maxId + 1;
  
  const selectedPerms = addForm.permissions
    .filter(p => p.checked)
    .map(p => p.name);
  
  const iconMap: Record<string, string> = {
    '国家级': '🏛️',
    '省级': '🏛️',
    '市级': '🏙️',
    '区县级': '🏘️',
    '学校级': '🏫',
    '班级级': '👨‍🏫'
  };
  
  const newRole = {
    id: newId,
    icon: iconMap[addForm.level] || '👤',
    name: addForm.name,
    level: addForm.level,
    userCount: 0,
    status: addForm.status,
    statusClass: addForm.status === '启用' ? 'status-active' : 'status-inactive',
    permissions: selectedPerms.length > 0 ? selectedPerms : ['暂无权限'],
    desc: addForm.desc || '暂无描述'
  };
  
  roles.value.push(newRole);
  message.success(`角色 ${addForm.name} 创建成功`);
  closeAddModal();
};

// ================= 编辑角色 =================
const handleEditRole = (role: any) => {
  editingRoleId.value = role.id;
  editForm.name = role.name;
  editForm.level = role.level;
  editForm.status = role.status;
  editForm.desc = role.desc;
  editForm.permissions = availablePermissions.map(p => ({
    ...p,
    checked: role.permissions.includes(p.name)
  }));
  showEditModal.value = true;
};

const closeEditModal = () => {
  showEditModal.value = false;
  editingRoleId.value = null;
};

const submitEditRole = () => {
  if (!editForm.name || !editForm.level) {
    message.warning('请填写角色名称和角色级别');
    return;
  }
  
  const index = roles.value.findIndex(r => r.id === editingRoleId.value);
  if (index === -1) {
    message.error('角色不存在');
    return;
  }
  
  const selectedPerms = editForm.permissions
    .filter(p => p.checked)
    .map(p => p.name);
  
  const iconMap: Record<string, string> = {
    '国家级': '🏛️',
    '省级': '🏛️',
    '市级': '🏙️',
    '区县级': '🏘️',
    '学校级': '🏫',
    '班级级': '👨‍🏫'
  };
  
  roles.value[index] = {
    ...roles.value[index],
    icon: iconMap[editForm.level] || roles.value[index].icon,
    name: editForm.name,
    level: editForm.level,
    status: editForm.status,
    statusClass: editForm.status === '启用' ? 'status-active' : 'status-inactive',
    permissions: selectedPerms.length > 0 ? selectedPerms : ['暂无权限'],
    desc: editForm.desc || '暂无描述'
  };
  
  message.success(`角色 ${editForm.name} 已更新`);
  closeEditModal();
};

// ================= 删除角色（单个） =================
const handleDeleteRole = (role: any) => {
  Modal.confirm({
    title: '确认删除',
    content: `确定要删除角色：${role.name} 吗？此操作不可恢复。`,
    okText: '确定',
    cancelText: '取消',
    onOk: () => {
      const index = roles.value.findIndex(r => r.id === role.id);
      if (index > -1) {
        roles.value.splice(index, 1);
        if (selectedRoleId.value === role.id) {
          selectedRoleId.value = null;
        }
        message.success(`角色 ${role.name} 已删除`);
      }
    }
  });
};

// ================= 删除角色（批量） =================
const handleBatchDeleteRole = () => {
  if (roles.value.length === 0) {
    message.warning('暂无角色可删除');
    return;
  }
  
  Modal.confirm({
    title: '确认批量删除',
    content: `确定要删除所有 ${roles.value.length} 个角色吗？此操作不可恢复。`,
    okText: '确定',
    cancelText: '取消',
    onOk: () => {
      roles.value = [];
      selectedRoleId.value = null;
      message.success('所有角色已删除');
    }
  });
};

// ================= 搜索 =================
const handleRoleSearch = () => {
  message.success(`搜索关键词：${roleSearch.value || '全部'}`);
};
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
.sub-tab-item:hover {
  background-color: #f5f7fa;
  color: #409eff;
}
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
.sub-tab-item.active .sub-tab-badge {
  background: #409eff;
}
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
  gap: 10px;
  flex-wrap: wrap;
  border: 1px solid #ebeef5;
}
.search-box {
  display: flex;
  gap: 8px;
  margin-left: auto;
}
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
.btn .icon { font-size: 14px; }
.btn:disabled { opacity: 0.6; cursor: not-allowed; }

/* ===== 角色列表 ===== */
.role-list { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
.role-card {
  background: #fff;
  border-radius: 10px;
  padding: 16px 20px;
  border: 1px solid #ebeef5;
  transition: all 0.3s;
}
.role-card:hover { box-shadow: 0 4px 12px rgba(0,0,0,0.08); }

.role-header {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
}
.role-icon { font-size: 24px; }
.role-name { font-weight: 600; color: #303133; font-size: 16px; }
.role-level {
  font-size: 11px;
  padding: 1px 10px;
  border-radius: 10px;
  background: #e8f0fe;
  color: #409eff;
}
.role-user-count {
  font-size: 12px;
  color: #909399;
}
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
  gap: 6px;
  margin: 10px 0;
}
.permission-tag {
  font-size: 11px;
  padding: 2px 10px;
  border-radius: 12px;
  background: #e8f0fe;
  color: #409eff;
}
.role-desc { font-size: 13px; color: #909399; }

/* ===== 权限分配 ===== */
.permission-assign {
  display: grid;
  grid-template-columns: 220px 1fr;
  gap: 20px;
  background: #fff;
  border-radius: 10px;
  padding: 20px;
  border: 1px solid #ebeef5;
  min-height: 420px;
}

/* 左侧角色列表 */
.assign-left {
  border-right: 1px solid #ebeef5;
  padding-right: 16px;
}
.assign-header-left {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
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

.role-select-list {
  display: flex;
  flex-direction: column;
  gap: 2px;
  max-height: 400px;
  overflow-y: auto;
}
.role-select-list::-webkit-scrollbar {
  width: 4px;
}
.role-select-list::-webkit-scrollbar-thumb {
  background: #d0d5dd;
  border-radius: 2px;
}
.role-select-list::-webkit-scrollbar-track {
  background: transparent;
}

.role-select-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 12px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}
.role-select-item:hover { background: #f5f7fa; }
.role-select-item.active {
  background: #e8f4fd;
  color: #409eff;
  box-shadow: inset 3px 0 0 #409eff;
}
.role-select-item .role-icon { font-size: 16px; }
.role-select-item .role-name { font-weight: 500; flex: 1; }
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
  margin-bottom: 16px;
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
.assign-actions { display: flex; gap: 8px; }

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
.permission-tree::-webkit-scrollbar {
  width: 4px;
}
.permission-tree::-webkit-scrollbar-thumb {
  background: #d0d5dd;
  border-radius: 2px;
}
.permission-tree::-webkit-scrollbar-track {
  background: transparent;
}

.perm-group {
  border: 1px solid #ebeef5;
  border-radius: 8px;
  overflow: hidden;
  transition: box-shadow 0.2s;
  flex-shrink: 0;
}
.perm-group:hover {
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}
.perm-group-header {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 16px;
  background: #fafbfc;
  border-bottom: 1px solid #ebeef5;
  cursor: pointer;
  transition: background 0.2s;
  cursor: default;
}
.perm-group-header:hover {
  background: #f0f2f5;
}
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
  padding: 8px 16px;
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
.perm-item:hover {
  background: #f5f7fa;
}
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
.perm-item-desc {
  font-size: 12px;
  color: #909399;
}

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
.modal-form {
  padding: 8px 0;
}
.form-group {
  margin-bottom: 16px;
}
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
.form-textarea {
  resize: vertical;
  min-height: 60px;
}
.permission-checkbox-group {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 6px;
  padding: 8px 12px;
  border: 1px solid #ebeef5;
  border-radius: 4px;
  background: #fafbfc;
  max-height: 120px;
  overflow-y: auto;
}
.perm-check-item {
  font-size: 13px;
  color: #606266;
}
.perm-check-item label {
  display: flex;
  align-items: center;
  gap: 4px;
  cursor: pointer;
}
.perm-check-item input[type="checkbox"] {
  width: 15px;
  height: 15px;
  cursor: pointer;
}

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
  .role-list { grid-template-columns: 1fr; }
  .permission-assign { grid-template-columns: 1fr; }
  .assign-left { border-right: none; border-bottom: 1px solid #ebeef5; padding-right: 0; padding-bottom: 16px; }
  .assign-right { padding-left: 0; }
  .perm-items { grid-template-columns: 1fr; }
}
@media (max-width: 768px) {
  .toolbar { flex-direction: column; align-items: stretch; }
  .search-box { margin-left: 0; }
  .search-box input { width: 100%; }
  .role-header { flex-direction: column; align-items: stretch; }
  .role-actions { margin-left: 0; }
  .sub-tab-item { padding: 6px 12px; font-size: 13px; }
  .permission-checkbox-group { grid-template-columns: 1fr 1fr; }
  .assign-header { flex-direction: column; align-items: stretch; }
  .assign-actions { justify-content: flex-start; }
  .perm-items { grid-template-columns: 1fr; }
  .legend-item { flex-wrap: wrap; }
  .legend-name { min-width: auto; }
}
</style>
