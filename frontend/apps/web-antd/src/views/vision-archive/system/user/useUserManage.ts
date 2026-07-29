// src/views/vision-archive/system/user/useUserManage.ts
import { ref, computed, reactive, onMounted } from 'vue';
import { message, Modal } from 'ant-design-vue';

import { userApi, mapDtoToDisplay } from '#/api/vision-archive/system';
import type { FrontendUserForm } from '#/api/vision-archive/system';

// ================= 类型定义 =================
interface UserInfo {
  id: number | string;
  username: string;
  name: string;
  gender: string;
  role: string;
  org: string;
  phone: string;
  email: string;
  status: string;
  statusText?: string;
  statusClass?: string;
  roleClass?: string;
  genderText?: string;
  lastLogin?: string;
  [key: string]: any;
}

export function useUserManage() {
  // ================= 状态管理 =================
  const userSearch = ref('');
  const selectedIds = ref<Array<number | string>>([]);
  const currentPage = ref(1);
  const pageSize = ref(10);
  const totalUsers = ref(0);
  const loading = ref(false);
  const submitting = ref(false);

  // ================= 弹窗状态 =================
  const showAddModal = ref(false);
  const showEditModal = ref(false);
  const showImportModal = ref(false);

  // ================= 上传状态 =================
  const uploadFile = ref<File | null>(null);
  const uploadFileName = ref('');

  // ================= 用户数据 =================
  const users = ref<UserInfo[]>([]);

  // ================= 表单数据 =================
  const defaultAddForm: FrontendUserForm = {
    username: '', name: '', gender: '', role: '',
    org: '', phone: '', email: '', password: '', status: 'active',
  };
  const addForm = reactive<FrontendUserForm>({ ...defaultAddForm });

  const defaultEditForm: FrontendUserForm = {
    id: '' as number | string, username: '', name: '', gender: '', role: '',
    org: '', phone: '', email: '', status: 'active',
  };
  const editForm = reactive<FrontendUserForm>({ ...defaultEditForm });

  // ================= 计算属性 =================
  const paginatedUsers = computed(() => users.value || []);

  const isAllSelected = computed(() => {
    const list = paginatedUsers.value;
    return list.length > 0 && list.every((u) => selectedIds.value.includes(u.id));
  });

  const totalPages = computed(() => Math.max(1, Math.ceil(totalUsers.value / pageSize.value)));

  const pageNumbers = computed(() => {
    const pages: (number | string)[] = [];
    const total = totalPages.value;
    if (total <= 7) {
      for (let i = 1; i <= total; i++) pages.push(i);
    } else {
      pages.push(1);
      if (currentPage.value > 3) pages.push('...');
      const start = Math.max(2, currentPage.value - 1);
      const end = Math.min(total - 1, currentPage.value + 1);
      for (let i = start; i <= end; i++) pages.push(i);
      if (currentPage.value < total - 2) pages.push('...');
      pages.push(total);
    }
    return pages;
  });

  // ================= 核心方法：获取数据 =================
  const fetchUsers = async () => {
    loading.value = true;
    try {
      const params = {
        page: currentPage.value,
        pageSize: pageSize.value,
        keyword: userSearch.value.trim() || undefined,
      };
      const res = await userApi.getUserList(params);

      // 后端返回 { items, total }，items 是 BackendUserDto 列表
      const list = Array.isArray(res?.items) ? res.items : [];
      users.value = list.map(mapDtoToDisplay);
      totalUsers.value = res?.total ?? users.value.length;

      // 如果当前页没数据且不是第一页，自动退回上一页
      if (users.value.length === 0 && currentPage.value > 1) {
        currentPage.value--;
        await fetchUsers();
      }
    } catch (error) {
      console.error('[fetchUsers] 获取用户列表失败:', error);
      users.value = [];
      totalUsers.value = 0;
    } finally {
      loading.value = false;
    }
  };

  onMounted(() => {
    fetchUsers();
  });

  // ================= 分页与搜索 =================
  const goToPage = (page: number | string) => {
    if (typeof page === 'number' && page !== currentPage.value) {
      currentPage.value = page;
      fetchUsers();
    }
  };

  const handleUserSearch = () => {
    currentPage.value = 1;
    fetchUsers();
  };

  // ================= 新增用户 =================
  const handleAddUser = () => {
    Object.assign(addForm, { ...defaultAddForm });
    showAddModal.value = true;
  };

  const submitAddUser = async () => {
    // 1. 前端基础校验
    if (!addForm.username || !addForm.name || !addForm.gender || !addForm.role || !addForm.org || !addForm.phone || !addForm.email || !addForm.password) {
      return message.warning('请填写所有必填字段');
    }

    submitting.value = true;
    try {
      // 调试日志：方便在 F12 看到发给后端的 payload
      console.log('[submitAddUser] 提交参数:', JSON.parse(JSON.stringify(addForm)));
      await userApi.createUser(addForm);
      message.success(`用户 ${addForm.name} 创建成功`);
      showAddModal.value = false;
      currentPage.value = 1; // 新增后回到第一页
      await fetchUsers();
    } catch (error: any) {
      console.error('[submitAddUser] 创建用户异常:', error);
      // requestClient 的 errorMessageResponseInterceptor 已经弹过 message.error 了
    } finally {
      submitting.value = false;
    }
  };

  // ================= 编辑用户 =================
  const handleEditUser = (user: UserInfo) => {
    Object.assign(editForm, {
      ...defaultEditForm,
      ...user,
      // 强制用后端给的中文名（role/org），保证下拉能选中
      role: user.role,
      org: user.org,
      // gender 已经是 'male'/'female'/'' 直接可用
      gender: user.gender,
      status: user.status,
    });
    showEditModal.value = true;
  };

  const submitEditUser = async () => {
    if (!editForm.username || !editForm.name || !editForm.gender || !editForm.role || !editForm.org || !editForm.phone || !editForm.email) {
      return message.warning('请填写所有必填字段');
    }
    if (editForm.id === '' || editForm.id == null) {
      return message.warning('缺少用户ID');
    }
    submitting.value = true;
    try {
      await userApi.updateUser(editForm.id, editForm);
      message.success(`用户 ${editForm.name} 信息已更新`);
      showEditModal.value = false;
      await fetchUsers();
    } catch (error: any) {
      console.error('[submitEditUser] 更新异常:', error);
    } finally {
      submitting.value = false;
    }
  };

  // ================= 删除与重置 =================
  const handleDeleteUser = (user: UserInfo) => {
    Modal.confirm({
      title: '确认删除',
      content: `确定要删除用户：${user.name}（${user.username}）吗？`,
      okType: 'danger',
      async onOk() {
        try {
          await userApi.deleteUser(user.id);
          message.success(`用户 ${user.name} 已删除`);
          await fetchUsers();
        } catch (error) {
          console.error('[handleDeleteUser] 删除失败:', error);
        }
      },
    });
  };

  const handleBatchDelete = () => {
    if (selectedIds.value.length === 0) return message.warning('请至少选择一个用户');
    Modal.confirm({
      title: '确认批量删除',
      content: `确定要删除选中的 ${selectedIds.value.length} 个用户吗？此操作不可恢复。`,
      okType: 'danger',
      async onOk() {
        try {
          const res = await userApi.batchDelete(selectedIds.value);
          message.success(res?.message || `已删除 ${res?.deletedCount ?? 0} 个用户`);
          selectedIds.value = [];
          await fetchUsers();
        } catch (error) {
          console.error('[handleBatchDelete] 批量删除失败:', error);
        }
      },
    });
  };

  const handleResetPassword = (user: UserInfo) => {
    Modal.confirm({
      title: '重置密码',
      content: `确定要重置 ${user.name} 的密码为默认密码 123456 吗？`,
      async onOk() {
        try {
          await userApi.resetPassword(user.id);
          message.success(`已重置 ${user.name} 的密码`);
        } catch (error) {
          console.error('[handleResetPassword] 重置失败:', error);
        }
      },
    });
  };

  // ================= 批量导入 =================
  const handleFileUpload = (event: Event) => {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) {
      uploadFile.value = file;
      uploadFileName.value = file.name;
    }
  };

  const submitImport = async () => {
    if (!uploadFile.value) return message.warning('请先选择文件');
    const file = uploadFile.value; // 局部 const 让 TS 收窄类型
    const formData = new FormData();
    formData.append('file', file);
    submitting.value = true;
    try {
      const result = await userApi.importUsers(formData);
      // 详细反馈：成功 / 失败分别提示，并显示前 5 条错误
      const { totalRows, successCount, failedCount, errors = [] } = result;
      if (failedCount === 0) {
        message.success(`导入完成：共 ${totalRows} 条，全部成功`);
      } else {
        const top = errors.slice(0, 5).map((e) => `第 ${e.row} 行：${e.message}`).join('\n');
        const more = errors.length > 5 ? `\n... 还有 ${errors.length - 5} 条` : '';
        Modal.warning({
          title: `部分导入失败（成功 ${successCount} / 失败 ${failedCount}）`,
          content: top + more,
        });
      }
      // 即便有失败也刷新列表（成功的那部分已经入库）
      showImportModal.value = false;
      uploadFile.value = null;
      uploadFileName.value = '';
      await fetchUsers();
    } catch (error) {
      console.error('[submitImport] 导入失败:', error);
    } finally {
      submitting.value = false;
    }
  };

  // ================= 辅助方法 =================
  const toggleSelectAll = (e: any) => {
    const isChecked = e?.target?.checked ?? !isAllSelected.value;
    selectedIds.value = isChecked ? paginatedUsers.value.map((u) => u.id) : [];
  };

  const prevPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--;
      fetchUsers();
    }
  };
  const nextPage = () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++;
      fetchUsers();
    }
  };

  // ================= 暴露给模板的接口 =================
  return {
    userSearch, selectedIds, currentPage, pageSize, totalUsers, loading, submitting,
    showAddModal, showEditModal, showImportModal, uploadFileName,
    addForm, editForm, isAllSelected, paginatedUsers, pageNumbers, totalPages,
    goToPage, handleUserSearch, handleAddUser, submitAddUser,
    handleEditUser, submitEditUser, handleDeleteUser, handleBatchDelete,
    handleResetPassword, handleFileUpload, submitImport, toggleSelectAll,
    closeAddModal: () => (showAddModal.value = false),
    closeEditModal: () => (showEditModal.value = false),
    closeImportModal: () => (showImportModal.value = false),
    downloadTemplate: () => userApi.downloadTemplate(),
    prevPage, nextPage,
  };
}
