import { ref, reactive, computed, onMounted } from 'vue';
import { message, Modal } from 'ant-design-vue';

import { menuApi } from '#/api/vision-archive/system';
import type { MenuDto, MenuForm } from '#/api/vision-archive/system';

// ================= 类型定义 =================
interface MenuInfo extends MenuDto {
  statusText?: string;
  statusClass?: string;
  typeText?: string;
  children?: MenuInfo[];
}

// ================= Mock 数据（后端不可用时使用）=================
const MOCK_MENU_TREE: MenuInfo[] = [
  {
    id: 1, name: '数据管理', icon: '📊', type: 1, parentId: null, sort: 0, status: 1,
    statusText: '正常', statusClass: 'status-active', typeText: '目录',
    children: [
      { id: 2, name: '大屏管理', icon: '🖥️', type: 1, parentId: 1, sort: 1, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '目录' },
    ],
  },
  {
    id: 3, name: '系统管理', icon: '⚙️', type: 1, parentId: null, sort: 2, status: 1,
    statusText: '正常', statusClass: 'status-active', typeText: '目录',
    children: [
      {
        id: 4, name: '用户管理', icon: '👥', type: 1, parentId: 3, sort: 1, status: 1,
        statusText: '正常', statusClass: 'status-active', typeText: '目录',
        children: [
          { id: 5, name: '用户查询', type: 2, parentId: 4, sort: 1, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '菜单', permission: 'system:user:query', component: 'system/user/index' },
          { id: 6, name: '用户新增', type: 2, parentId: 4, sort: 2, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '菜单', permission: 'system:user:add', component: 'system/user/index' },
          { id: 7, name: '用户修改', type: 2, parentId: 4, sort: 3, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '菜单', permission: 'system:user:edit', component: 'system/user/index' },
          { id: 8, name: '用户删除', type: 2, parentId: 4, sort: 4, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '菜单', permission: 'system:user:remove', component: 'system/user/index' },
          { id: 9, name: '用户导出', type: 2, parentId: 4, sort: 5, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '菜单', permission: 'system:user:export', component: 'system/user/index' },
          { id: 10, name: '用户导入', type: 2, parentId: 4, sort: 6, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '菜单', permission: 'system:user:import', component: 'system/user/index' },
          { id: 11, name: '重置密码', type: 2, parentId: 4, sort: 7, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '菜单', permission: 'system:user:resetPwd', component: 'system/user/index' },
        ],
      },
      { id: 12, name: '角色管理', icon: '🔐', type: 1, parentId: 3, sort: 2, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '目录', permission: 'system:role:list', component: 'system/role/index' },
      { id: 13, name: '菜单管理', icon: '📋', type: 1, parentId: 3, sort: 3, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '目录', permission: 'system:menu:list', component: 'system/menu/index' },
      { id: 14, name: '部门管理', icon: '🏢', type: 1, parentId: 3, sort: 4, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '目录', permission: 'system:dept:list', component: 'system/dept/index' },
      { id: 15, name: '岗位管理', icon: '💼', type: 1, parentId: 3, sort: 5, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '目录', permission: 'system:post:list', component: 'system/post/index' },
      { id: 16, name: '字典管理', icon: '📖', type: 1, parentId: 3, sort: 6, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '目录', permission: 'system:dict:list', component: 'system/dict/index' },
      { id: 17, name: '参数设置', icon: '⚙️', type: 1, parentId: 3, sort: 7, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '目录', permission: 'system:config:list', component: 'system/config/index' },
      { id: 18, name: '通知公告', icon: '📢', type: 1, parentId: 3, sort: 8, status: 1, statusText: '正常', statusClass: 'status-active', typeText: '目录', permission: 'system:notice:list', component: 'system/notice/index' },
    ],
  },
];

// ================= 常量配置 =================
const TYPE_MAP: Record<number, { label: string; class: string }> = {
  1: { label: '目录', class: 'type-catalog' },
  2: { label: '菜单', class: 'type-menu' },
  3: { label: '按钮', class: 'type-button' },
};

const STATUS_MAP: Record<number, { text: string; class: string }> = {
  1: { text: '正常', class: 'status-active' },
  0: { text: '停用', class: 'status-inactive' },
};

const ICON_OPTIONS = [
  { value: '📊', label: '📊 数据' },
  { value: '🖥️', label: '🖥️ 大屏' },
  { value: '⚙️', label: '⚙️ 系统' },
  { value: '👥', label: '👥 用户' },
  { value: '🔐', label: '🔐 角色' },
  { value: '📋', label: '📋 菜单' },
  { value: '🏢', label: '🏢 部门' },
  { value: '💼', label: '💼 岗位' },
  { value: '📖', label: '📖 字典' },
  { value: '📢', label: '📢 通知' },
  { value: '📝', label: '📝 编辑' },
  { value: '🔍', label: '🔍 搜索' },
  { value: '📤', label: '📤 导出' },
  { value: '📥', label: '📥 导入' },
  { value: '🗑️', label: '🗑️ 删除' },
  { value: '➕', label: '➕ 新增' },
  { value: '✏️', label: '✏️ 修改' },
  { value: '🔑', label: '🔑 权限' },
];

export function useMenuManage() {
  // ================= 状态管理 =================
  const menuSearch = ref('');
  const statusFilter = ref<number | string>('');
  const loading = ref(false);
  const submitting = ref(false);
  // 使用 Record<number, boolean> 代替 Set，确保 Vue 3 响应性
  const expandedKeys = reactive<Record<number, boolean>>({});

  // ================= 弹窗状态 =================
  const showAddModal = ref(false);
  const showEditModal = ref(false);

  // ================= 菜单数据 =================
  const menuTree = ref<MenuInfo[]>([]);
  const allMenus = ref<MenuInfo[]>([]);

  // ================= 表单数据 =================
  const defaultForm: MenuForm = {
    name: '',
    icon: '',
    type: 1,
    parentId: null,
    sort: 0,
    status: 1,
    path: '',
    component: '',
    permission: '',
  };
  const addForm = reactive<MenuForm>({ ...defaultForm });
  const editForm = reactive<MenuForm>({ ...defaultForm });
  const editingId = ref<number | null>(null);
  const addParentId = ref<number | null>(null);

  // ================= 计算属性 =================
  const totalExpandableCount = computed(() => {
    return getExpandableCount(menuTree.value);
  });

  const isAllExpanded = computed(() => {
    return totalExpandableCount.value > 0 &&
      Object.values(expandedKeys).filter(v => v).length === totalExpandableCount.value;
  });

  // ================= 辅助方法 =================
  function getExpandableCount(nodes: MenuInfo[]): number {
    let count = 0;
    for (const node of nodes) {
      if (node.children && node.children.length > 0) {
        count++;
        count += getExpandableCount(node.children);
      }
    }
    return count;
  }

  function flattenTree(nodes: MenuInfo[]): MenuInfo[] {
    const result: MenuInfo[] = [];
    for (const node of nodes) {
      result.push(node);
      if (node.children && node.children.length > 0) {
        result.push(...flattenTree(node.children));
      }
    }
    return result;
  }

  function mapDtoToInfo(dto: MenuDto): MenuInfo {
    const statusInfo = STATUS_MAP[dto.status] || { text: '未知', class: '' };
    const typeInfo = TYPE_MAP[dto.type] || { label: '未知', class: '' };
    return {
      ...dto,
      statusText: statusInfo.text,
      statusClass: statusInfo.class,
      typeText: typeInfo.label,
      children: dto.children?.map(mapDtoToInfo),
    };
  }

  function filterTree(nodes: MenuInfo[], keyword: string): MenuInfo[] {
    if (!keyword) return nodes;
    const kw = keyword.toLowerCase();
    const result: MenuInfo[] = [];
    for (const node of nodes) {
      const children = node.children ? filterTree(node.children, keyword) : [];
      const matchName = node.name.toLowerCase().includes(kw);
      const matchPermission = (node.permission || '').toLowerCase().includes(kw);
      if (matchName || matchPermission || children.length > 0) {
        result.push({ ...node, children });
      }
    }
    return result;
  }

  function filterByStatus(nodes: MenuInfo[], status: number | string): MenuInfo[] {
    if (status === '' || status === null || status === undefined) return nodes;
    const result: MenuInfo[] = [];
    for (const node of nodes) {
      const children = node.children ? filterByStatus(node.children, status) : [];
      if (node.status === Number(status) || children.length > 0) {
        result.push({ ...node, children });
      }
    }
    return result;
  }

  function setExpandedKeys(nodes: MenuInfo[]) {
    for (const node of nodes) {
      if (node.children && node.children.length > 0) {
        expandedKeys[node.id] = true;
        setExpandedKeys(node.children);
      }
    }
  }

  function clearExpandedKeys() {
    Object.keys(expandedKeys).forEach(key => {
      delete expandedKeys[Number(key)];
    });
  }

  // ================= 数据加载 =================
  let apiWarned = false;

  async function fetchMenus() {
    loading.value = true;
    try {
      const tree = await menuApi.getTree();
      menuTree.value = tree.map(mapDtoToInfo);
      allMenus.value = flattenTree(menuTree.value);
      // 默认展开所有节点
      clearExpandedKeys();
      setExpandedKeys(menuTree.value);
    } catch (error: any) {
      console.warn('[fetchMenus] 后端不可用，使用 mock 数据:', error?.message || error);
      applyMockData();
    } finally {
      loading.value = false;
    }
  }

  function applyMockData() {
    menuTree.value = JSON.parse(JSON.stringify(MOCK_MENU_TREE));
    allMenus.value = flattenTree(menuTree.value);
    clearExpandedKeys();
    setExpandedKeys(menuTree.value);
    if (!apiWarned) {
      apiWarned = true;
      message.warning('后端服务未启动，已使用本地模拟数据。启动后端后将自动切换至真实数据。');
    }
  }

  // ================= 搜索与筛选 =================
  const filteredMenuTree = computed(() => {
    let result = menuTree.value;
    if (menuSearch.value.trim()) {
      result = filterTree(result, menuSearch.value.trim());
    }
    if (statusFilter.value !== '' && statusFilter.value !== null && statusFilter.value !== undefined) {
      result = filterByStatus(result, statusFilter.value);
    }
    return result;
  });

  const handleSearch = () => {
    if (menuSearch.value.trim()) {
      clearExpandedKeys();
      const filtered = filterTree(menuTree.value, menuSearch.value.trim());
      setExpandedKeys(filtered);
    }
  };

  const handleReset = () => {
    menuSearch.value = '';
    statusFilter.value = '';
    clearExpandedKeys();
    setExpandedKeys(menuTree.value);
  };

  // ================= 展开/折叠 =================
  const toggleExpand = (id: number) => {
    expandedKeys[id] = !expandedKeys[id];
  };

  const toggleExpandAll = () => {
    if (isAllExpanded.value) {
      clearExpandedKeys();
    } else {
      clearExpandedKeys();
      setExpandedKeys(menuTree.value);
    }
  };

  const isExpanded = (id: number) => !!expandedKeys[id];

  // ================= 新增菜单 =================
  const handleAddMenu = (parentId: number | null = null) => {
    Object.assign(addForm, { ...defaultForm });
    addForm.parentId = parentId;
    addForm.status = 1;
    addForm.sort = 0;
    addForm.type = parentId === null ? 1 : 2;
    addParentId.value = parentId;
    showAddModal.value = true;
  };

  const submitAddMenu = async () => {
    if (!addForm.name) {
      return message.warning('请填写菜单名称');
    }
    submitting.value = true;
    try {
      const payload: MenuForm = {
        ...addForm,
        parentId: addForm.parentId,
        sort: Number(addForm.sort) || 0,
        status: Number(addForm.status),
        type: Number(addForm.type),
      };
      await menuApi.create(payload);
      message.success(`菜单 "${addForm.name}" 创建成功`);
      showAddModal.value = false;
      await fetchMenus();
    } catch (error: any) {
      console.error('[submitAddMenu] 创建失败:', error);
    } finally {
      submitting.value = false;
    }
  };

  // ================= 编辑菜单 =================
  const handleEditMenu = (menu: MenuInfo) => {
    editingId.value = menu.id;
    Object.assign(editForm, {
      id: menu.id,
      name: menu.name,
      icon: menu.icon || '',
      type: menu.type,
      parentId: menu.parentId,
      sort: menu.sort,
      status: menu.status,
      path: menu.path || '',
      component: menu.component || '',
      permission: menu.permission || '',
    });
    showEditModal.value = true;
  };

  const submitEditMenu = async () => {
    if (!editForm.name) {
      return message.warning('请填写菜单名称');
    }
    if (!editingId.value) {
      return message.warning('缺少菜单ID');
    }
    submitting.value = true;
    try {
      const payload: MenuForm = {
        ...editForm,
        sort: Number(editForm.sort) || 0,
        status: Number(editForm.status),
        type: Number(editForm.type),
      };
      await menuApi.update(editingId.value, payload);
      message.success(`菜单 "${editForm.name}" 已更新`);
      showEditModal.value = false;
      await fetchMenus();
    } catch (error: any) {
      console.error('[submitEditMenu] 更新失败:', error);
    } finally {
      submitting.value = false;
    }
  };

  // ================= 删除菜单 =================
  const handleDeleteMenu = (menu: MenuInfo) => {
    const hasChildren = menu.children && menu.children.length > 0;
    const content = hasChildren
      ? `确定要删除菜单 "${menu.name}" 吗？该菜单包含 ${menu.children!.length} 个子菜单，删除后子菜单也会被一并删除。`
      : `确定要删除菜单 "${menu.name}" 吗？此操作不可恢复。`;
    Modal.confirm({
      title: '确认删除',
      content,
      okType: 'danger',
      okText: '确定',
      cancelText: '取消',
      async onOk() {
        try {
          await menuApi.delete(menu.id);
          message.success(`菜单 "${menu.name}" 已删除`);
          await fetchMenus();
        } catch (error) {
          console.error('[handleDeleteMenu] 删除失败:', error);
        }
      },
    });
  };

  // ================= 关闭弹窗 =================
  const closeAddModal = () => {
    showAddModal.value = false;
    Object.assign(addForm, { ...defaultForm });
  };

  const closeEditModal = () => {
    showEditModal.value = false;
    editingId.value = null;
    Object.assign(editForm, { ...defaultForm });
  };

  // ================= 获取父级菜单选项（用于下拉选择）=================
  const parentMenuOptions = computed(() => {
    const options: { value: number | null; label: string }[] = [
      { value: null, label: '根目录' },
    ];
    const flatten = (nodes: MenuInfo[], depth: number) => {
      for (const node of nodes) {
        if (node.type === 1) {
          options.push({
            value: node.id,
            label: `${'　'.repeat(depth)}${node.icon || ''} ${node.name}`,
          });
          if (node.children) {
            flatten(node.children, depth + 1);
          }
        }
      }
    };
    flatten(menuTree.value, 0);
    return options;
  });

  // ================= 初始化 =================
  onMounted(() => {
    fetchMenus();
  });

  // ================= 暴露给模板的接口 =================
  return {
    // 状态
    menuSearch,
    statusFilter,
    loading,
    submitting,
    showAddModal,
    showEditModal,
    expandedKeys,

    // 数据
    menuTree,
    filteredMenuTree,
    allMenus,

    // 表单
    addForm,
    editForm,
    addParentId,
    editingId,

    // 计算属性
    isAllExpanded,
    isExpanded,
    parentMenuOptions,

    // 常量
    STATUS_MAP,
    TYPE_MAP,
    ICON_OPTIONS,

    // 方法
    handleSearch,
    handleReset,
    toggleExpand,
    toggleExpandAll,
    handleAddMenu,
    submitAddMenu,
    handleEditMenu,
    submitEditMenu,
    handleDeleteMenu,
    closeAddModal,
    closeEditModal,
  };
}
