// src/views/vision-archive/system/permission/usePermissionManage.ts
import { ref, computed, reactive, onMounted } from 'vue';
import { message, Modal } from 'ant-design-vue';

import {
  roleApi,
  menuApi,
  ROLE_STATUS_MAP,
  ROLE_LEVEL_OPTIONS,
} from '#/api/vision-archive/system';
import type { RoleDto, RoleForm, MenuDto } from '#/api/vision-archive/system';

// ================= 类型定义 =================
interface RoleInfo {
  id: number;
  name: string;
  code: string;
  level: number;
  levelText: string;
  levelIcon: string;
  status: number;
  statusText: string;
  statusColor: string;
  remark?: string;
  createdAt?: string;
  menuIds: number[];
  userCount: number;
  permissions: string[];
  [key: string]: any;
}

interface MenuInfo {
  id: number;
  name: string;
  code?: string;
  icon?: string;
  type: number;
  parentId: number | null;
  sort: number;
  status: number;
  children?: MenuInfo[];
  checked?: boolean;
  indeterminate?: boolean;
}

// ================= 级别映射 =================
const LEVEL_TEXT_MAP: Record<number, { text: string; icon: string }> = {
  1: { text: '国家级', icon: '🏛️' },
  2: { text: '省级', icon: '🏛️' },
  3: { text: '市级', icon: '🏙️' },
  4: { text: '区县级', icon: '🏘️' },
  5: { text: '学校级', icon: '🏫' },
};

const LEVEL_TEXT_TO_NUM: Record<string, number> = {
  '国家级': 1, '省级': 2, '市级': 3, '区县级': 4, '学校级': 5,
};

/** 后端 DTO -> 前端展示对象 */
function mapRoleDtoToDisplay(dto: RoleDto): RoleInfo {
  const levelInfo = LEVEL_TEXT_MAP[dto.level] || { text: `级别${dto.level}`, icon: '👤' };
  const statusInfo = ROLE_STATUS_MAP[dto.status] || { text: '未知', color: '#999' };
  return {
    id: dto.id,
    name: dto.name,
    code: dto.code,
    level: dto.level,
    levelText: levelInfo.text,
    levelIcon: levelInfo.icon,
    status: dto.status,
    statusText: statusInfo.text,
    statusColor: statusInfo.color,
    remark: dto.remark,
    createdAt: dto.createdAt,
    menuIds: dto.menuIds || [],
    userCount: dto.userCount || 0,
    permissions: [],
  };
}

/** 菜单 DTO -> 前端展示对象（带选中状态） */
function mapMenuDtoToDisplay(dto: MenuDto): MenuInfo {
  return {
    id: dto.id,
    name: dto.name,
    code: dto.code,
    icon: dto.icon,
    type: dto.type,
    parentId: dto.parentId,
    sort: dto.sort,
    status: dto.status,
    children: dto.children?.map(mapMenuDtoToDisplay),
    checked: false,
    indeterminate: false,
  };
}

/** 扁平化菜单树 */
function flattenMenuTree(nodes: MenuInfo[]): MenuInfo[] {
  const result: MenuInfo[] = [];
  const walk = (items: MenuInfo[]) => {
    items.forEach(item => {
      result.push(item);
      if (item.children && item.children.length > 0) {
        walk(item.children);
      }
    });
  };
  walk(nodes);
  return result;
}

export function usePermissionManage() {
  // ================= Tab 状态 =================
  const subActiveTab = ref<'role' | 'permission'>('role');

  // ================= 角色管理状态 =================
  const roleSearch = ref('');
  const levelFilter = ref<string>('');
  const statusFilter = ref<number | string>('');
  const currentPage = ref(1);
  const pageSize = ref(12);
  const totalRoles = ref(0);
  const loading = ref(false);
  const submitting = ref(false);

  // ================= 弹窗状态 =================
  const showAddModal = ref(false);
  const showEditModal = ref(false);
  const editingRoleId = ref<number | null>(null);

  // ================= 数据 =================
  const roles = ref<RoleInfo[]>([]);
  const menuTree = ref<MenuInfo[]>([]);
  const apiConnected = ref(true);
  const apiErrorMsg = ref('');

  // ================= 权限分配状态 =================
  const selectedRoleId = ref<number | null>(null);
  const roleSelectSearch = ref('');
  const collapsedGroups = reactive<Record<string, boolean>>({});
  const collapsedRoleSelectGroups = reactive<Record<string, boolean>>({});
  const collapsedPermGroups = reactive<Record<string, boolean>>({});
  const expandPermMap = reactive<Record<number, boolean>>({});
  const allExpanded = ref(false);

  // ================= 表单数据 =================
  const defaultRoleForm: RoleForm = {
    name: '', code: '', level: 1, status: 1, remark: '',
  };
  const addForm = reactive<RoleForm>({ ...defaultRoleForm });
  const editForm = reactive<RoleForm>({ ...defaultRoleForm });

  // ================= 计算属性 =================
  const filteredRoles = computed(() => {
    const list = roles.value;
    return list.filter(r => {
      if (roleSearch.value && !r.name.includes(roleSearch.value)) return false;
      if (levelFilter.value && String(r.level) !== String(levelFilter.value)) return false;
      if (statusFilter.value !== '' && statusFilter.value !== null && statusFilter.value !== undefined) {
        if (r.status !== Number(statusFilter.value)) return false;
      }
      return true;
    });
  });

  const totalPages = computed(() => Math.max(1, Math.ceil(totalRoles.value / pageSize.value)));

  const paginatedRoles = computed(() => {
    const start = (currentPage.value - 1) * pageSize.value;
    return filteredRoles.value.slice(start, start + pageSize);
  });

  const groupedRoles = computed(() => {
    const groups: { [key: number]: { level: number; label: string; icon: string; roles: RoleInfo[] } } = {};
    ROLE_LEVEL_OPTIONS.forEach(opt => {
      const info = LEVEL_TEXT_MAP[opt.value];
      groups[opt.value] = { level: opt.value, label: info?.text || `级别${opt.value}`, icon: info?.icon || '👤', roles: [] };
    });
    paginatedRoles.value.forEach(role => {
      if (!groups[role.level]) {
        groups[role.level] = { level: role.level, label: role.levelText, icon: role.levelIcon, roles: [] };
      }
      groups[role.level].roles.push(role);
    });
    return ROLE_LEVEL_OPTIONS
      .filter(opt => groups[opt.value] && groups[opt.value].roles.length > 0)
      .map(opt => groups[opt.value]);
  });

  const groupedRoleSelect = computed(() => {
    const searchTerm = roleSelectSearch.value.toLowerCase();
    const filtered = roles.value.filter(r =>
      !searchTerm || r.name.toLowerCase().includes(searchTerm)
    );
    const groups: { [key: number]: { level: number; label: string; icon: string; roles: RoleInfo[] } } = {};
    ROLE_LEVEL_OPTIONS.forEach(opt => {
      const info = LEVEL_TEXT_MAP[opt.value];
      groups[opt.value] = { level: opt.value, label: info?.text || `级别${opt.value}`, icon: info?.icon || '👤', roles: [] };
    });
    filtered.forEach(role => {
      if (!groups[role.level]) {
        groups[role.level] = { level: role.level, label: role.levelText, icon: role.levelIcon, roles: [] };
      }
      groups[role.level].roles.push(role);
    });
    return ROLE_LEVEL_OPTIONS
      .filter(opt => groups[opt.value] && groups[opt.value].roles.length > 0)
      .map(opt => groups[opt.value]);
  });

  const selectedRole = computed(() => roles.value.find(r => r.id === selectedRoleId.value));

  const allPermissionsSelected = computed(() => {
    const flat = flattenMenuTree(menuTree.value);
    return flat.length > 0 && flat.every(m => m.checked);
  });

  // ================= 初始化折叠状态 =================
  ROLE_LEVEL_OPTIONS.forEach(opt => {
    collapsedGroups[String(opt.value)] = false;
    collapsedRoleSelectGroups[String(opt.value)] = false;
  });

  // ================= 核心方法 =================
  // 内置 mock 数据（后端不可用时的 UI 回退）
  const MOCK_ROLES: RoleInfo[] = [
    {
      id: 1, name: '国家管理员', code: 'ROLE_NATIONAL', level: 1, levelText: '国家级', levelIcon: '🏛️',
      status: 1, statusText: '启用', statusColor: '#52c41a',
      remark: '查看全国所有数据，统一下发标准、生成全国报表，管理省级账号',
      userCount: 1, menuIds: [1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24],
      permissions: ['查看全国数据', '查看省级数据', '查看市级数据', '查看县级数据', '查看校级数据', '用户管理', '角色管理', '组织管理', '字典管理', '日志查看']
    },
    {
      id: 2, name: '省级管理员', code: 'ROLE_PROVINCE', level: 2, levelText: '省级', levelIcon: '🏛️',
      status: 1, statusText: '启用', statusColor: '#52c41a',
      remark: '查看本省及各地市数据，生成省级报表、监控地市防控，管理市级账号',
      userCount: 34, menuIds: [1, 3, 4, 5, 7, 8, 9, 10, 13, 14, 16, 17, 19, 22, 23],
      permissions: ['查看省级数据', '查看市级数据', '查看县级数据', '查看校级数据', '生成报表', '查看预警', '发布预警', '组织管理', '字典管理']
    },
    {
      id: 3, name: '市级管理员', code: 'ROLE_CITY', level: 3, levelText: '市级', levelIcon: '🏙️',
      status: 1, statusText: '启用', statusColor: '#52c41a',
      remark: '查看本市及各县区数据，监控数据质量、发布预警，管理县级账号',
      userCount: 333, menuIds: [1, 4, 5, 7, 8, 9, 10, 13, 16, 17, 18, 22],
      permissions: ['查看市级数据', '查看县级数据', '查看校级数据', '生成报表', '查看预警', '发布预警', '干预管理', '组织管理']
    },
    {
      id: 4, name: '县级管理员', code: 'ROLE_DISTRICT', level: 4, levelText: '区县级', levelIcon: '🏘️',
      status: 1, statusText: '启用', statusColor: '#52c41a',
      remark: '查看本县所有学校数据，审核数据、督导防控工作，管理学校账号',
      userCount: 2862, menuIds: [1, 5, 7, 8, 9, 10, 13, 16, 17, 22, 23],
      permissions: ['查看县级数据', '查看校级数据', '新增数据', '编辑数据', '审核数据', '生成报表', '查看预警', '组织管理', '字典管理']
    },
    {
      id: 5, name: '学校管理员', code: 'ROLE_SCHOOL', level: 5, levelText: '学校级', levelIcon: '🏫',
      status: 1, statusText: '启用', statusColor: '#52c41a',
      remark: '仅管理本校数据，录入/审核视力档案，查看本校统计',
      userCount: 216600, menuIds: [1, 6, 7, 8, 9, 10, 16],
      permissions: ['查看校级数据', '新增数据', '编辑数据', '审核数据', '查看预警']
    },
    {
      id: 6, name: '校医', code: 'ROLE_DOCTOR', level: 5, levelText: '学校级', levelIcon: '🏫',
      status: 1, statusText: '启用', statusColor: '#52c41a',
      remark: '录入/审核视力数据，查看本校统计分析',
      userCount: 185000, menuIds: [1, 6, 7, 8, 9],
      permissions: ['查看校级数据', '新增数据', '编辑数据', '审核数据']
    },
    {
      id: 7, name: '班主任', code: 'ROLE_TEACHER', level: 5, levelText: '学校级', levelIcon: '👨‍🏫',
      status: 1, statusText: '启用', statusColor: '#52c41a',
      remark: '仅管理本班学生数据，录入视力档案，查看本班统计',
      userCount: 980000, menuIds: [1, 6, 7],
      permissions: ['查看校级数据', '新增数据', '编辑数据']
    },
  ];

  const MOCK_MENU_TREE: MenuInfo[] = [
    { id: 1, name: '数据查看权限', icon: '📊', type: 1, parentId: null, sort: 1, status: 1, checked: false, children: [
      { id: 2, name: '查看全国数据', type: 2, parentId: 1, sort: 1, status: 1, checked: false },
      { id: 3, name: '查看省级数据', type: 2, parentId: 1, sort: 2, status: 1, checked: false },
      { id: 4, name: '查看市级数据', type: 2, parentId: 1, sort: 3, status: 1, checked: false },
      { id: 5, name: '查看县级数据', type: 2, parentId: 1, sort: 4, status: 1, checked: false },
      { id: 6, name: '查看校级数据', type: 2, parentId: 1, sort: 5, status: 1, checked: false },
    ]},
    { id: 7, name: '数据操作权限', icon: '📝', type: 1, parentId: null, sort: 2, status: 1, checked: false, children: [
      { id: 8, name: '新增数据', type: 2, parentId: 7, sort: 1, status: 1, checked: false },
      { id: 9, name: '编辑数据', type: 2, parentId: 7, sort: 2, status: 1, checked: false },
      { id: 10, name: '删除数据', type: 2, parentId: 7, sort: 3, status: 1, checked: false },
      { id: 11, name: '审核数据', type: 2, parentId: 7, sort: 4, status: 1, checked: false },
      { id: 12, name: '导入导出', type: 2, parentId: 7, sort: 5, status: 1, checked: false },
    ]},
    { id: 13, name: '报表权限', icon: '📋', type: 1, parentId: null, sort: 3, status: 1, checked: false, children: [
      { id: 14, name: '生成报表', type: 2, parentId: 13, sort: 1, status: 1, checked: false },
      { id: 15, name: '导出报表', type: 2, parentId: 13, sort: 2, status: 1, checked: false },
    ]},
    { id: 16, name: '预警权限', icon: '🔔', type: 1, parentId: null, sort: 4, status: 1, checked: false, children: [
      { id: 17, name: '查看预警', type: 2, parentId: 16, sort: 1, status: 1, checked: false },
      { id: 18, name: '发布预警', type: 2, parentId: 16, sort: 2, status: 1, checked: false },
    ]},
    { id: 19, name: '系统管理权限', icon: '⚙️', type: 1, parentId: null, sort: 5, status: 1, checked: false, children: [
      { id: 20, name: '用户管理', type: 2, parentId: 19, sort: 1, status: 1, checked: false },
      { id: 21, name: '角色管理', type: 2, parentId: 19, sort: 2, status: 1, checked: false },
      { id: 22, name: '组织管理', type: 2, parentId: 19, sort: 3, status: 1, checked: false },
      { id: 23, name: '字典管理', type: 2, parentId: 19, sort: 4, status: 1, checked: false },
      { id: 24, name: '日志查看', type: 2, parentId: 19, sort: 5, status: 1, checked: false },
    ]},
  ];

  const fetchRoles = async () => {
    loading.value = true;
    try {
      const params: Record<string, any> = {
        page: currentPage.value,
        pageSize: pageSize.value,
      };
      if (roleSearch.value.trim()) params.name = roleSearch.value.trim();
      if (levelFilter.value) params.level = levelFilter.value;
      if (statusFilter.value !== '' && statusFilter.value !== null && statusFilter.value !== undefined) {
        params.status = statusFilter.value;
      }
      const res = await roleApi.getPagedList(params);
      const list = Array.isArray(res?.items) ? res.items : [];
      roles.value = list.map(mapRoleDtoToDisplay);
      totalRoles.value = res?.total ?? roles.value.length;
      apiConnected.value = true;
      apiErrorMsg.value = '';
    } catch (error: any) {
      console.warn('[fetchRoles] 获取角色列表失败，使用 mock 数据:', error?.message || error);
      // 回退：使用 mock 数据
      let mockList = MOCK_ROLES;
      if (roleSearch.value.trim()) {
        mockList = mockList.filter(r => r.name.includes(roleSearch.value.trim()));
      }
      if (levelFilter.value !== '' && levelFilter.value !== null && levelFilter.value !== undefined) {
        mockList = mockList.filter(r => r.level === Number(levelFilter.value));
      }
      if (statusFilter.value !== '' && statusFilter.value !== null && statusFilter.value !== undefined) {
        mockList = mockList.filter(r => r.status === Number(statusFilter.value));
      }
      totalRoles.value = mockList.length;
      const start = (currentPage.value - 1) * pageSize.value;
      roles.value = mockList.slice(start, start + pageSize.value);
      if (apiConnected.value) {
        apiConnected.value = false;
        apiErrorMsg.value = error?.message || '后端服务未启动';
        message.warning('后端服务未启动，已使用本地 mock 数据。启动后端后将自动切换至真实数据。');
      }
    } finally {
      loading.value = false;
    }
  };

  const fetchMenuTree = async () => {
    try {
      const res = await menuApi.getTree();
      menuTree.value = Array.isArray(res) ? res.map(mapMenuDtoToDisplay) : [];
      apiConnected.value = true;
      apiErrorMsg.value = '';
    } catch (error: any) {
      console.warn('[fetchMenuTree] 获取菜单树失败，使用 mock 数据:', error?.message || error);
      menuTree.value = JSON.parse(JSON.stringify(MOCK_MENU_TREE));
      if (apiConnected.value) {
        apiConnected.value = false;
        apiErrorMsg.value = error?.message || '后端服务未启动';
      }
    }
  };

  const fetchRolePermissions = async (roleId: number) => {
    try {
      const menuIds = await roleApi.getMenuPermission(roleId);
      const idSet = new Set(Array.isArray(menuIds) ? menuIds : []);
      const flat = flattenMenuTree(menuTree.value);
      flat.forEach(m => {
        m.checked = idSet.has(m.id);
        m.indeterminate = false;
      });
      // 更新角色的 permissions 标签
      const role = roles.value.find(r => r.id === roleId);
      if (role) {
        role.menuIds = Array.from(idSet);
        role.permissions = flat.filter(m => m.checked).map(m => m.name);
      }
    } catch (error: any) {
      console.warn('[fetchRolePermissions] 获取角色权限失败:', error?.message || error);
    }
  };

  // ================= 分页 =================
  const handleSearch = () => {
    currentPage.value = 1;
    fetchRoles();
  };

  const handleReset = () => {
    roleSearch.value = '';
    levelFilter.value = '';
    statusFilter.value = '';
    currentPage.value = 1;
    fetchRoles();
  };

  // ================= 折叠操作 =================
  const toggleGroupCollapse = (level: string) => {
    collapsedGroups[level] = !collapsedGroups[level];
  };

  const toggleRoleSelectGroup = (level: string) => {
    collapsedRoleSelectGroups[level] = !collapsedRoleSelectGroups[level];
  };

  const togglePermGroupCollapse = (id: string) => {
    collapsedPermGroups[id] = !collapsedPermGroups[id];
  };

  const toggleExpandAll = () => {
    allExpanded.value = !allExpanded.value;
    // 展开/折叠权限组由前端结构控制
    // 这里操作菜单树的展开状态
  };

  // ================= 权限分配操作 =================
  const selectRole = async (role: RoleInfo) => {
    selectedRoleId.value = role.id;
    // 如果没有菜单树，先加载
    if (menuTree.value.length === 0) {
      await fetchMenuTree();
    }
    // 加载该角色的权限
    await fetchRolePermissions(role.id);
  };

  const toggleMenuChecked = (menu: MenuInfo) => {
    menu.checked = !menu.checked;
    // 级联选中/取消子节点
    const cascade = (node: MenuInfo, checked: boolean) => {
      node.checked = checked;
      node.indeterminate = false;
      if (node.children) {
        node.children.forEach(child => cascade(child, checked));
      }
    };
    if (menu.children) {
      menu.children.forEach(child => cascade(child, !!menu.checked));
    }
    // 更新父节点状态
    updateParentStates();
  };

  const updateParentStates = () => {
    const updateNode = (node: MenuInfo): { checked: boolean; indeterminate: boolean } => {
      if (!node.children || node.children.length === 0) {
        return { checked: !!node.checked, indeterminate: false };
      }
      let allChecked = true;
      let someChecked = false;
      node.children.forEach(child => {
        const childState = updateNode(child);
        if (!childState.checked) allChecked = false;
        if (childState.checked || childState.indeterminate) someChecked = true;
      });
      node.checked = allChecked;
      node.indeterminate = !allChecked && someChecked;
      return { checked: allChecked, indeterminate: node.indeterminate };
    };
    menuTree.value.forEach(updateNode);
  };

  const toggleAllPermissions = () => {
    const flat = flattenMenuTree(menuTree.value);
    const newState = !allPermissionsSelected.value;
    flat.forEach(m => {
      m.checked = newState;
      m.indeterminate = false;
    });
  };

  const handleSavePermission = async () => {
    if (!selectedRoleId.value) {
      message.warning('请先选择角色');
      return;
    }
    const flat = flattenMenuTree(menuTree.value);
    const checkedIds = flat.filter(m => m.checked).map(m => m.id);
    submitting.value = true;
    try {
      await roleApi.assignMenuPermission(selectedRoleId.value, checkedIds);
      const role = roles.value.find(r => r.id === selectedRoleId.value);
      if (role) {
        role.menuIds = checkedIds;
        role.permissions = flat.filter(m => m.checked).map(m => m.name);
      }
      message.success(`角色 "${role?.name}" 权限保存成功！共配置 ${checkedIds.length} 项权限`);
    } catch (error: any) {
      console.error('[handleSavePermission] 保存权限失败:', error);
      const errorMsg = error?.response?.data?.message || error?.message || '保存失败';
      message.error(`保存失败：${errorMsg}`);
    } finally {
      submitting.value = false;
    }
  };

  const handleResetPermission = async () => {
    if (!selectedRoleId.value) return;
    Modal.confirm({
      title: '确认重置',
      content: '确定要重置当前角色的权限配置吗？重置后将清空所有权限。',
      okText: '确定',
      cancelText: '取消',
      onOk: () => {
        const flat = flattenMenuTree(menuTree.value);
        flat.forEach(m => { m.checked = false; m.indeterminate = false; });
        message.info('权限已重置，请点击"保存权限"生效');
      }
    });
  };

  // ================= 跳转权限分配 =================
  const handleAssignPermission = async (role: RoleInfo) => {
    await selectRole(role);
    subActiveTab.value = 'permission';
    message.info(`正在为 ${role.name} 配置权限`);
  };

  // ================= 新增角色 =================
  const handleAddRole = () => {
    Object.assign(addForm, { ...defaultRoleForm });
    showAddModal.value = true;
  };

  const submitAddRole = async () => {
    if (!addForm.name || !addForm.code) {
      message.warning('请填写角色名称和权限字符');
      return;
    }
    submitting.value = true;
    try {
      const payload: RoleForm = {
        name: addForm.name.trim(),
        code: addForm.code.trim(),
        level: addForm.level,
        status: addForm.status,
        remark: addForm.remark,
      };
      await roleApi.create(payload);
      message.success(`角色 "${addForm.name}" 创建成功`);
      showAddModal.value = false;
      currentPage.value = 1;
      await fetchRoles();
    } catch (error: any) {
      console.error('[submitAddRole] 创建角色异常:', error);
      const errorMsg = error?.response?.data?.message || error?.message || '未知错误';
      message.error(`创建失败：${errorMsg}`);
    } finally {
      submitting.value = false;
    }
  };

  // ================= 编辑角色 =================
  const handleEditRole = (role: RoleInfo) => {
    editingRoleId.value = role.id;
    Object.assign(editForm, {
      id: role.id,
      name: role.name,
      code: role.code,
      level: role.level,
      status: role.status,
      remark: role.remark,
    });
    showEditModal.value = true;
  };

  const submitEditRole = async () => {
    if (!editForm.name || !editForm.code) {
      message.warning('请填写角色名称和权限字符');
      return;
    }
    if (!editingRoleId.value) {
      message.warning('缺少角色ID');
      return;
    }
    submitting.value = true;
    try {
      const payload: RoleForm = {
        name: editForm.name.trim(),
        code: editForm.code.trim(),
        level: editForm.level,
        status: editForm.status,
        remark: editForm.remark,
      };
      await roleApi.update(editingRoleId.value, payload);
      message.success(`角色 "${editForm.name}" 已更新`);
      showEditModal.value = false;
      await fetchRoles();
    } catch (error: any) {
      console.error('[submitEditRole] 更新异常:', error);
      const errorMsg = error?.response?.data?.message || error?.message || '未知错误';
      message.error(`更新失败：${errorMsg}`);
    } finally {
      submitting.value = false;
    }
  };

  // ================= 删除 =================
  const handleDeleteRole = (role: RoleInfo) => {
    Modal.confirm({
      title: '确认删除',
      content: `确定要删除角色：${role.name} 吗？此操作不可恢复。`,
      okType: 'danger',
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

  const handleBatchDelete = () => {
    if (roles.value.length === 0) {
      message.warning('暂无角色可删除');
      return;
    }
    Modal.confirm({
      title: '确认批量删除',
      content: `确定要删除所有 ${roles.value.length} 个角色吗？此操作不可恢复。`,
      okType: 'danger',
      async onOk() {
        try {
          const ids = roles.value.map(r => r.id);
          const res = await roleApi.batchDelete(ids);
          message.success(res?.message || `已删除 ${res?.deletedCount ?? 0} 个角色`);
          selectedRoleId.value = null;
          await fetchRoles();
        } catch (error: any) {
          console.error('[handleBatchDelete] 批量删除失败:', error);
          const errorMsg = error?.response?.data?.message || error?.message || '删除失败';
          message.error(errorMsg);
        }
      },
    });
  };

  // ================= 初始化 =================
  onMounted(async () => {
    await Promise.all([fetchRoles(), fetchMenuTree()]);
  });

  // ================= 暴露给模板的接口 =================
  return {
    // Tab
    subActiveTab,

    // 状态
    roleSearch, levelFilter, statusFilter,
    currentPage, pageSize, totalRoles,
    loading, submitting,
    showAddModal, showEditModal,
    editingRoleId,
    apiConnected, apiErrorMsg,

    // 数据
    roles, menuTree,
    filteredRoles, paginatedRoles, groupedRoles,
    groupedRoleSelect, selectedRole,
    totalPages,
    allPermissionsSelected,
    allExpanded,

    // 权限分配状态
    selectedRoleId, roleSelectSearch,
    collapsedGroups, collapsedRoleSelectGroups, collapsedPermGroups,
    expandPermMap,

    // 表单
    addForm, editForm,

    // 方法
    handleSearch, handleReset,
    fetchRoles, fetchMenuTree,
    toggleGroupCollapse, toggleRoleSelectGroup, togglePermGroupCollapse,
    toggleExpandAll,
    selectRole, toggleMenuChecked, toggleAllPermissions,
    handleSavePermission, handleResetPermission, handleAssignPermission,
    handleAddRole, submitAddRole, handleEditRole, submitEditRole,
    handleDeleteRole, handleBatchDelete,
    closeAddModal: () => (showAddModal.value = false),
    closeEditModal: () => (showEditModal.value = false),
    levelTextMap: LEVEL_TEXT_MAP,
    levelTextToNum: LEVEL_TEXT_TO_NUM,
    statusMap: ROLE_STATUS_MAP,
    levelOptions: ROLE_LEVEL_OPTIONS,
  };
}
