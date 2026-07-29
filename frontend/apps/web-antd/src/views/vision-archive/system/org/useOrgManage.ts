// src/views/vision-archive/system/org/useOrgManage.ts
import { ref, computed, reactive, onMounted } from 'vue';
import { message, Modal } from 'ant-design-vue';

import { orgApi, ORG_STATUS_MAP } from '#/api/vision-archive/system';
import type { OrgDto, OrgForm } from '#/api/vision-archive/system';

// ================= 类型定义 =================
interface OrgInfo {
  id: number;
  name: string;
  code?: string;
  level?: string;
  parentId?: number | null;
  sort: number;
  status: number;
  statusText: string;
  statusColor: string;
  createdAt?: string;
  children?: OrgInfo[];
  [key: string]: any;
}

/** 将后端 DTO 转换为前端展示对象 */
function mapDtoToDisplay(dto: OrgDto): OrgInfo {
  const statusInfo = ORG_STATUS_MAP[dto.status] || { text: '未知', color: '#999' };
  return {
    id: dto.id,
    name: dto.name,
    code: dto.code,
    level: dto.level,
    parentId: dto.parentId,
    sort: dto.sort,
    status: dto.status,
    statusText: statusInfo.text,
    statusColor: statusInfo.color,
    createdAt: dto.createdAt,
    children: dto.children?.map(mapDtoToDisplay),
  };
}

/** 扁平化树形数据为行列表（包含层级信息，用于渲染） */
interface FlatRow {
  org: OrgInfo;
  depth: number;
  hasChildren: boolean;
  isExpanded: boolean;
  visible: boolean;
}

function flattenToRows(nodes: OrgInfo[], depth = 0, parentExpanded = true): FlatRow[] {
  const rows: FlatRow[] = [];
  for (const node of nodes) {
    const hasChildren = !!(node.children && node.children.length > 0);
    rows.push({
      org: node,
      depth,
      hasChildren,
      isExpanded: false, // 展开状态由外部控制
      visible: parentExpanded,
    });
    if (hasChildren) {
      const childExpanded = false; // 默认折叠，后续根据 expandedIds 设置
      const childRows = flattenToRows(node.children!, depth + 1, childExpanded);
      rows.push(...childRows);
    }
  }
  return rows;
}

/** 扁平化树形数据（用于统计等） */
function flattenTree(nodes: OrgInfo[]): OrgInfo[] {
  const result: OrgInfo[] = [];
  const walk = (items: OrgInfo[]) => {
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

export function useOrgManage() {
  // ================= 状态管理 =================
  const searchName = ref('');
  const searchStatus = ref<number | string>('');
  const selectedIds = ref<Array<number | string>>([]);
  const loading = ref(false);
  const submitting = ref(false);
  
  // 树形相关
  const treeMode = ref(true);  // 默认树形模式
  const expandedIds = ref<number[]>([]);  // 使用数组以便 Vue 能正确追踪依赖
  const showAllChildren = ref(true);

  // ================= 弹窗状态 =================
  const showAddModal = ref(false);
  const showEditModal = ref(false);

  // ================= 组织数据 =================
  const orgTree = ref<OrgInfo[]>([]);  // 树形数据
  const orgList = ref<OrgInfo[]>([]);  // 扁平列表数据（用于分页）
  const allOrgList = ref<OrgInfo[]>([]);
  const apiConnected = ref(true);
  const apiErrorMsg = ref('');

  // ================= 表单数据 =================
  const defaultAddForm: OrgForm = {
    name: '',
    code: '',
    level: '市级',
    parentId: null,
    sort: 0,
    status: 1,
  };
  const addForm = reactive<OrgForm>({ ...defaultAddForm });

  const defaultEditForm: OrgForm = {
    id: undefined,
    name: '',
    code: '',
    level: '',
    parentId: null,
    sort: 0,
    status: 1,
  };
  const editForm = reactive<OrgForm>({ ...defaultEditForm });

  // ================= 计算属性 =================
  const filteredTree = computed(() => {
    if (!searchName.value.trim() && (!searchStatus.value || searchStatus.value === '')) {
      return orgTree.value;
    }
    
    const keyword = searchName.value.trim().toLowerCase();
    const statusFilter = searchStatus.value !== '' && searchStatus.value !== null ? Number(searchStatus.value) : null;
    
    const filterNodes = (nodes: OrgInfo[]): OrgInfo[] => {
      const result: OrgInfo[] = [];
      nodes.forEach(node => {
        const nameMatch = !keyword || node.name.toLowerCase().includes(keyword);
        const statusMatch = statusFilter === null || node.status === statusFilter;
        const children = node.children ? filterNodes(node.children) : [];
        if ((nameMatch && statusMatch) || children.length > 0) {
          result.push({ ...node, children });
        }
      });
      return result;
    };
    
    return filterNodes(orgTree.value);
  });

  // 总机构数（全部，不受搜索影响）
  const totalOrgCount = computed(() => flattenTree(orgTree.value).length);
  // 当前筛选后的条数
  const filteredCount = computed(() => flattenTree(filteredTree.value).length);
  // 兼容旧引用
  const totalCount = computed(() => totalOrgCount.value);
  const flatOrgList = computed(() => flattenTree(filteredTree.value));

  // 生成扁平行列表，考虑展开/折叠状态
  const tableRows = computed(() => {
    const rows: FlatRow[] = [];
    
    const buildRows = (nodes: OrgInfo[], depth: number, parentVisible: boolean) => {
      for (const node of nodes) {
        const hasChildren = !!(node.children && node.children.length > 0);
        const expanded = expandedIds.value.includes(node.id);  // 使用 includes 以便 Vue 追踪依赖
        
        rows.push({
          org: node,
          depth,
          hasChildren,
          isExpanded: expanded,
          visible: parentVisible,
        });
        
        if (hasChildren) {
          buildRows(node.children!, depth + 1, parentVisible && expanded);
        }
      }
    };
    
    buildRows(filteredTree.value, 0, true);
    return rows;
  });

  // TreeSelect 树节点数据（支持检索和层级显示）
  const treeSelectData = computed(() => {
    const buildNodes = (nodes: OrgInfo[]): any[] => {
      return nodes.map(node => ({
        title: `${node.level} - ${node.name}`,
        value: node.id,
        key: node.id,
        raw: node,
        children: node.children && node.children.length > 0 ? buildNodes(node.children) : undefined,
      }));
    };
    return buildNodes(orgTree.value);
  });

  const isAllSelected = computed(() => {
    const list = flatOrgList.value;
    return list.length > 0 && list.every((u) => selectedIds.value.includes(u.id));
  });

  // 半选状态（部分选中但非全选）
  const isIndeterminate = computed(() => {
    const list = flatOrgList.value;
    const selCount = list.filter((u) => selectedIds.value.includes(u.id)).length;
    return selCount > 0 && selCount < list.length;
  });

  // ================= 统计数据 =================
  const orgStatsData = computed(() => {
    const counts: Record<string, number> = {
      '机构总数': 0,
      '国家级': 0,
      '省级': 0,
      '市级': 0,
      '区县级': 0,
      '学校级': 0,
    };
    // allOrgList 已经是扁平化的数组，直接遍历即可
    allOrgList.value.forEach((node) => {
      counts['机构总数']++;
      if (counts[node.level || ''] !== undefined) {
        counts[node.level || '']++;
      }
    });
    return [
      { label: '机构总数', value: counts['机构总数'] },
      { label: '国家级', value: counts['国家级'] },
      { label: '省级', value: counts['省级'] },
      { label: '市级', value: counts['市级'] },
      { label: '区县级', value: counts['区县级'] },
      { label: '学校级', value: counts['学校级'] },
    ];
  });

  // ================= 核心方法：获取数据 =================
  const fetchTree = async () => {
    loading.value = true;
    try {
      const res = await orgApi.getTree();
      orgTree.value = Array.isArray(res) ? res.map(mapDtoToDisplay) : [];
      allOrgList.value = flattenTree(orgTree.value);
      // 默认展开所有节点
      if (showAllChildren.value) {
        const allIds: number[] = [];
        const collectIds = (nodes: OrgInfo[]) => {
          nodes.forEach(node => {
            if (node.children && node.children.length > 0) {
              allIds.push(node.id);
              collectIds(node.children);
            }
          });
        };
        collectIds(orgTree.value);
        expandedIds.value = allIds;
      }
      apiConnected.value = true;
      apiErrorMsg.value = '';
    } catch (error: any) {
      console.warn('[fetchTree] 获取组织树失败:', error?.message || error);
      orgTree.value = [];
      allOrgList.value = [];
      if (apiConnected.value) {
        apiConnected.value = false;
        apiErrorMsg.value = error?.message || '后端服务未启动';
        message.warning('后端服务未启动，数据暂不可用。请启动后端服务后重试。');
      }
    } finally {
      loading.value = false;
    }
  };

  // 保留 fetchList 兼容性（扁平列表模式）
  const fetchList = async () => {
    loading.value = true;
    try {
      const params: Record<string, any> = {};
      if (searchName.value.trim()) {
        params.name = searchName.value.trim();
      }
      if (searchStatus.value !== '' && searchStatus.value !== null) {
        params.status = searchStatus.value;
      }

      const res = await orgApi.getPagedList({ ...params, page: 1, pageSize: 1000 });
      const list = Array.isArray(res?.items) ? res.items : [];
      orgList.value = list.map(mapDtoToDisplay);
      apiConnected.value = true;
      apiErrorMsg.value = '';
    } catch (error: any) {
      console.warn('[fetchList] 获取组织列表失败:', error?.message || error);
      orgList.value = [];
      if (apiConnected.value) {
        apiConnected.value = false;
        apiErrorMsg.value = error?.message || '后端服务未启动';
        message.warning('后端服务未启动，数据暂不可用。请启动后端服务后重试。');
      }
    } finally {
      loading.value = false;
    }
  };

  onMounted(() => {
    fetchTree();
  });

  // ================= 树形操作 =================
  const toggleExpand = (id: number) => {
    const idx = expandedIds.value.indexOf(id);
    if (idx > -1) {
      expandedIds.value = expandedIds.value.filter(x => x !== id);
    } else {
      expandedIds.value = [...expandedIds.value, id];
    }
  };

  const isExpanded = (id: number) => expandedIds.value.includes(id);

  const toggleExpandAll = () => {
    if (expandedIds.value.length > 0) {
      expandedIds.value = [];
      showAllChildren.value = false;
    } else {
      const allIds: number[] = [];
      const collectIds = (nodes: OrgInfo[]) => {
        nodes.forEach(node => {
          if (node.children && node.children.length > 0) {
            allIds.push(node.id);
            collectIds(node.children);
          }
        });
      };
      collectIds(orgTree.value);
      expandedIds.value = allIds;
      showAllChildren.value = true;
    }
  };

  // ================= 搜索 =================
  const handleSearch = () => {
    fetchTree();
  };

  const handleReset = () => {
    searchName.value = '';
    searchStatus.value = '';
    fetchTree();
  };

  // ================= 新增组织 =================
  const handleAddOrg = () => {
    Object.assign(addForm, { ...defaultAddForm, code: generateCode(defaultAddForm.level) });
    showAddModal.value = true;
  };

  const generateCode = (level: string): string => {
    const prefixMap: Record<string, string> = {
      '国家级': 'GJ',
      '省级': 'SJ',
      '市级': 'SHI',
      '区县级': 'QX',
      '学校级': 'XX',
    };
    const prefix = prefixMap[level] || 'ORG';
    const timestamp = Date.now().toString().slice(-4);
    return `${prefix}-${timestamp}`;
  };

  const onAddLevelChange = () => {
    if (addForm.level) {
      addForm.code = generateCode(addForm.level);
    }
  };

  const submitAddOrg = async () => {
    if (!addForm.name || !addForm.level) {
      message.warning('请填写部门名称和级别');
      return;
    }

    submitting.value = true;
    try {
      // 清理数据，移除可能的空值
      const payload: any = {
        name: addForm.name.trim(),
        level: addForm.level,
        sort: addForm.sort ?? 0,
        status: addForm.status ?? 1,
      };
      if (addForm.code) {
        payload.code = addForm.code;
      }
      if (addForm.parentId !== null && addForm.parentId !== undefined) {
        payload.parentId = addForm.parentId;
      } else {
        payload.parentId = null;
      }

      await orgApi.create(payload);
      message.success(`部门 "${addForm.name}" 创建成功`);
      showAddModal.value = false;
      await fetchTree();
    } catch (error: any) {
      console.error('[submitAddOrg] 创建部门异常:', error);
      const errorMsg = error?.response?.data?.message || error?.message || '未知错误';
      message.error(`创建失败：${errorMsg}`);
    } finally {
      submitting.value = false;
    }
  };

  // ================= 编辑组织 =================
  const handleEditOrg = (org: OrgInfo) => {
    Object.assign(editForm, {
      id: org.id,
      name: org.name,
      code: org.code,
      level: org.level || '',
      parentId: org.parentId ?? null,
      sort: org.sort,
      status: org.status,
    });
    showEditModal.value = true;
  };

  const submitEditOrg = async () => {
    if (!editForm.name || !editForm.level) {
      message.warning('请填写部门名称和级别');
      return;
    }
    if (editForm.id === undefined || editForm.id === null) {
      message.warning('缺少部门ID');
      return;
    }

    submitting.value = true;
    try {
      // 清理数据
      const payload: any = {
        id: editForm.id,
        name: editForm.name.trim(),
        level: editForm.level,
        sort: editForm.sort ?? 0,
        status: editForm.status ?? 1,
      };
      if (editForm.code) {
        payload.code = editForm.code;
      } else {
        payload.code = null;
      }
      if (editForm.parentId !== null && editForm.parentId !== undefined) {
        payload.parentId = editForm.parentId;
      } else {
        payload.parentId = null;
      }

      await orgApi.update(editForm.id, payload);
      message.success(`部门 "${editForm.name}" 已更新`);
      showEditModal.value = false;
      await fetchTree();
    } catch (error: any) {
      console.error('[submitEditOrg] 更新异常:', error);
      const errorMsg = error?.response?.data?.message || error?.message || '未知错误';
      message.error(`更新失败：${errorMsg}`);
    } finally {
      submitting.value = false;
    }
  };

  // ================= 删除与批量操作 =================
  const handleDeleteOrg = (org: OrgInfo) => {
    Modal.confirm({
      title: '确认删除',
      content: `确定要删除部门 "${org.name}" 吗？此操作不可恢复。`,
      okType: 'danger',
      async onOk() {
        try {
          // 后端通过 ResultWrapperFilter 统一包装响应，requestClient 会自动解包 data 字段。
          // 删除成功时返回的 data 为 null，异常时拦截器会抛出错误。
          await orgApi.delete(org.id);
          message.success(`部门 "${org.name}" 已删除`);
          // 清理相关的 expandedIds 和 selectedIds
          expandedIds.value = expandedIds.value.filter(id => id !== org.id);
          selectedIds.value = selectedIds.value.filter(id => id !== org.id);
          await fetchTree();
        } catch (error: any) {
          console.error('[handleDeleteOrg] 删除失败:', error);
          // 尝试从错误响应中获取信息
          const errorMsg = error?.response?.data?.message || error?.message || '删除失败';
          message.error(errorMsg);
        }
      },
    });
  };

  const handleBatchDelete = () => {
    if (selectedIds.value.length === 0) {
      message.warning('请至少选择一个部门');
      return;
    }
    Modal.confirm({
      title: '确认批量删除',
      content: `确定要删除选中的 ${selectedIds.value.length} 个部门吗？此操作不可恢复。`,
      okType: 'danger',
      async onOk() {
        try {
          const res = await orgApi.batchDelete(selectedIds.value);
          message.success(res?.message || `已删除 ${res?.deletedCount ?? 0} 个部门`);
          // 清理 expandedIds 和 selectedIds
          expandedIds.value = [];
          selectedIds.value = [];
          await fetchTree();
        } catch (error: any) {
          console.error('[handleBatchDelete] 批量删除失败:', error);
          if (error?.response?.data?.message) {
            message.error(error.response.data.message);
          }
        }
      },
    });
  };

  const handleRefresh = () => {
    fetchTree();
    message.success('数据已刷新');
  };

  // ================= 辅助方法 =================
  const toggleSelectAll = (e: any) => {
    const isChecked = e?.target?.checked ?? !isAllSelected.value;
    selectedIds.value = isChecked ? flatOrgList.value.map((u) => u.id) : [];
  };

  const handleAddChild = (parentOrg: OrgInfo) => {
    Object.assign(addForm, {
      ...defaultAddForm,
      code: generateCode(defaultAddForm.level),
      parentId: parentOrg.id,
      level: getChildLevel(parentOrg.level),
    });
    showAddModal.value = true;
  };

  const getChildLevel = (parentLevel?: string): string => {
    const levelMap: Record<string, string> = {
      '国家级': '省级',
      '省级': '市级',
      '市级': '区县级',
      '区县级': '学校级',
      '学校级': '学校级',
    };
    return levelMap[parentLevel || ''] || '市级';
  };

  // ================= 暴露给模板的接口 =================
  return {
    // 状态
    searchName, searchStatus, selectedIds, loading, submitting,
    apiConnected, apiErrorMsg,
    // 树形
    treeMode, expandedIds, showAllChildren,
    // 弹窗
    showAddModal, showEditModal,
    // 数据与表单
    addForm, editForm, 
    orgTree, orgList, filteredTree, flatOrgList,
    tableRows, treeSelectData,
    totalCount, totalOrgCount, filteredCount,
    isAllSelected, isIndeterminate,
    orgStatsData,
    // 方法
    fetchTree, fetchList,
    toggleExpand, isExpanded, toggleExpandAll,
    handleSearch, handleReset, handleAddOrg, submitAddOrg,
    handleEditOrg, submitEditOrg, handleDeleteOrg, handleBatchDelete,
    handleRefresh, toggleSelectAll, handleAddChild, onAddLevelChange,
    closeAddModal: () => (showAddModal.value = false),
    closeEditModal: () => (showEditModal.value = false),
  };
}
