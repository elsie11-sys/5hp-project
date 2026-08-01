import { ref, computed, reactive, onMounted, watch } from 'vue';
import { message, Modal } from 'ant-design-vue';
import { dictApi } from '#/api/vision-archive/system';
import type { DictDto, DictQuery } from '#/api/vision-archive/system';

export interface DictItemDto {
  id?: number | string;
  dictType?: string;
  itemCode?: string;
  itemLabel: string;
  itemValue: string;
  sortOrder: number;
  status: number;
  remark?: string;
  isNew?: boolean;
  isDeleted?: boolean;
  _touched?: boolean;
  _statusModified?: boolean;
}

const STATUS_MAP: Record<number, string> = { 1: '正常', 0: '停用' };
const OWNER_TYPE_MAP: Record<string, string> = { SYSTEM: '系统', USER: '用户' };

const HEADERS = ['字典编号', '字典名称', '字典类型', '归属类型', '状态', '备注', '创建时间'];

function colWidth(text: string): number {
  let width = 0;
  for (const ch of text) {
    width += /[\u4e00-\u9fa5]/.test(ch) ? 2 : 1;
  }
  return Math.min(Math.max(width + 2, 8), 50);
}

function escapeCell(val: unknown): string {
  const s = val === null || val === undefined ? '' : String(val);
  return s.replace(/"/g, '""');
}

function downloadExcel(data: DictDto[], filename: string) {
  const rows = data.map((item) => {
    const statusText = STATUS_MAP[item.status ?? 1] ?? '未知';
    const ownerText = OWNER_TYPE_MAP[item.ownerType ?? 'USER'] ?? '用户';
    return [
      item.id ?? '',
      item.dictName ?? '',
      item.dictType ?? '',
      ownerText,
      statusText,
      item.remark ?? '',
      item.createdAt ? formatDateLocal(item.createdAt) : '',
    ];
  });

  const allText = [HEADERS, ...rows];
  const widths = HEADERS.map((_, ci) =>
    Math.max(...allText.map((r) => colWidth(String(r[ci] ?? '')))),
  );

  const colDefs = widths.map((w) => `<col style="width:${w * 10}pt"/>`).join('');
  const headerRow = HEADERS.map((h) => `<th>${escapeCell(h)}</th>`).join('');
  const bodyRows = rows
    .map((r) => `<tr>${r.map((c) => `<td>${escapeCell(c)}</td>`).join('')}</tr>`)
    .join('');

  const html = `
<html xmlns:o="urn:schemas-microsoft-com:office:office"
      xmlns:x="urn:schemas-microsoft-com:office:excel"
      xmlns="http://www.w3.org/TR/REC-html40">
<head>
<meta charset="UTF-8">
<!--[if gte mso 9]>
<xml>
  <x:ExcelWorkbook>
    <x:ExcelWorksheets>
      <x:ExcelWorksheet>
        <x:Name>字典管理</x:Name>
        <x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions>
      </x:ExcelWorksheet>
    </x:ExcelWorksheets>
  </x:ExcelWorkbook>
</xml>
<![endif]-->
<style>
  table { border-collapse: collapse; font-family: "宋体", SimSun, serif; font-size: 12px; }
  th { background: #f5f7fa; font-weight: bold; border: 1px solid #999; padding: 4px 6px; }
  td { border: 1px solid #ccc; padding: 3px 6px; white-space: nowrap; }
</style>
</head>
<body>
<table>
  <thead><tr>${headerRow}</tr></thead>
  <tbody>${bodyRows}</tbody>
</table>
</body>
</html>`;

  const blob = new Blob(['\uFEFF' + html], {
    type: 'application/vnd.ms-excel;charset=utf-8;',
  });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
}

function formatDateLocal(iso: string): string {
  const d = new Date(iso);
  if (isNaN(d.getTime())) return iso;
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
}

export function useDictManage() {
  const dictName = ref('');
  const dictType = ref('');
  const statusFilter = ref<number | undefined>(undefined);
  const ownerTypeFilter = ref<'SYSTEM' | 'USER' | undefined>(undefined);
  const startDate = ref('');
  const endDate = ref('');

  const currentPage = ref(1);
  const pageSize = ref(10);
  const totalDicts = ref(0);
  const loading = ref(false);
  const submitting = ref(false);

  const selectedIds = ref<Array<number | string>>([]);

  const showAddModal = ref(false);
  const showEditModal = ref(false);
  const addSubmitted = ref(false);
  const editSubmitted = ref(false);

  const dicts = ref<DictDto[]>([]);

  const addDictItems = ref<DictItemDto[]>([]);
  const editDictItems = ref<DictItemDto[]>([]);

  const selectedAddItemIds = ref<Array<number | string>>([]);
  const selectedEditItemIds = ref<Array<number | string>>([]);
  const editingAddItemIndex = ref<number | null>(null);
  const editingEditItemIndex = ref<number | null>(null);

  const itemPageSize = ref(10);
  const addItemCurrentPage = ref(1);
  const editItemCurrentPage = ref(1);

  // 悬浮预览字典项
  const hoverDictItems = ref<DictItemDto[]>([]);
  const hoverDictLoading = ref(false);
  const hoverDictId = ref<number | string | null>(null);

  const defaultForm = (): DictDto => ({
    id: undefined,
    dictName: '',
    dictType: '',
    status: 1,
    ownerType: 'USER',
    remark: '',
  });

  const defaultItem = (): DictItemDto => ({
    id: undefined,
    itemCode: '',
    itemLabel: '',
    itemValue: '',
    sortOrder: 0,
    status: 1,
    remark: '',
    isNew: true,
    _touched: false,
    _statusModified: false,
  });

  const addForm = reactive<DictDto>(defaultForm());
  const editForm = reactive<DictDto>(defaultForm());

  const handleItemStatusChange = (target: 'add' | 'edit', index: number) => {
    const list = target === 'add' ? addDictItems.value : editDictItems.value;
    const item = list[index];
    if (item) {
      item._statusModified = true;
    }
  };

  watch(
    () => addForm.status,
    (newStatus) => {
      addDictItems.value.forEach((item) => {
        if (!item.isDeleted && !item._statusModified) {
          item.status = newStatus;
        }
      });
    },
  );

  watch(
    () => editForm.status,
    (newStatus) => {
      editDictItems.value.forEach((item) => {
        if (!item.isDeleted && !item._statusModified) {
          item.status = newStatus;
        }
      });
    },
  );

  const paginatedDicts = computed(() => {
    const list = dicts.value || [];
    return [...list].sort((a, b) => {
      const idA = a.id;
      const idB = b.id;
      const na = typeof idA === 'number' ? idA : parseInt(String(idA ?? '0'), 10) || 0;
      const nb = typeof idB === 'number' ? idB : parseInt(String(idB ?? '0'), 10) || 0;
      return na - nb;
    });
  });

  const isAllSelected = computed(() => {
    const list = paginatedDicts.value;
    return list.length > 0 && list.every((d) => selectedIds.value.includes(d.id!));
  });

  const totalPages = computed(() => Math.max(1, Math.ceil(totalDicts.value / pageSize.value)));

  const fetchDicts = async () => {
    loading.value = true;
    try {
      const query: DictQuery = {
        page: currentPage.value,
        pageSize: pageSize.value,
        dictName: dictName.value.trim() || undefined,
        dictType: dictType.value.trim() || undefined,
        status: statusFilter.value,
        ownerType: ownerTypeFilter.value,
        startDate: startDate.value || undefined,
        endDate: endDate.value || undefined,
      };
      const res = await dictApi.getPaged(query);
      dicts.value = res.items || [];
      totalDicts.value = res.total ?? dicts.value.length;

      if (dicts.value.length === 0 && currentPage.value > 1) {
        currentPage.value--;
        await fetchDicts();
      }
    } catch (error) {
      console.error('[fetchDicts] 获取字典列表失败:', error);
      dicts.value = [];
      totalDicts.value = 0;
    } finally {
      loading.value = false;
    }
  };

  onMounted(() => {
    fetchDicts();
  });

  const handleSearch = () => {
    currentPage.value = 1;
    fetchDicts();
  };

  const handleReset = () => {
    dictName.value = '';
    dictType.value = '';
    statusFilter.value = undefined;
    ownerTypeFilter.value = undefined;
    startDate.value = '';
    endDate.value = '';
    currentPage.value = 1;
    fetchDicts();
  };

  const goToPage = (page: number | string) => {
    if (typeof page === 'number' && page !== currentPage.value) {
      currentPage.value = page;
      fetchDicts();
    }
  };
  const prevPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--;
      fetchDicts();
    }
  };
  const nextPage = () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++;
      fetchDicts();
    }
  };

  const handleAdd = () => {
    Object.assign(addForm, defaultForm());
    addDictItems.value = [];
    selectedAddItemIds.value = [];
    editingAddItemIndex.value = null;
    addItemCurrentPage.value = 1;
    addSubmitted.value = false;
    showAddModal.value = true;
  };

  const handleAddItem = (target: 'add' | 'edit') => {
    const list = target === 'add' ? addDictItems.value : editDictItems.value;
    const maxSort = list.length > 0 ? Math.max(...list.map((i) => i.sortOrder ?? 0)) : 0;
    // 新添加的字典项状态始终跟随字典状态
    const dictStatus = target === 'add' ? addForm.status : editForm.status;
    const newItem = { ...defaultItem(), sortOrder: maxSort + 1, status: dictStatus };
    list.push(newItem);
    if (target === 'add') {
      const totalPages = Math.max(1, Math.ceil(list.length / itemPageSize.value));
      addItemCurrentPage.value = totalPages;
    } else {
      const totalPages = Math.max(1, Math.ceil(list.length / itemPageSize.value));
      editItemCurrentPage.value = totalPages;
    }
  };

  const handleRemoveItem = (target: 'add' | 'edit', index: number) => {
    const list = target === 'add' ? addDictItems.value : editDictItems.value;
    const item = list[index];
    if (!item) return;
    const currentPageRef = target === 'add' ? addItemCurrentPage : editItemCurrentPage;
    if (!item.isNew && item.id !== undefined) {
      item.isDeleted = true;
      message.success('标记删除成功，保存后生效');
      // 标记删除后 active 列表缩短，需按 active 长度校正当前页
      const activeLen =
        target === 'add' ? activeAddDictItems.value.length : activeEditDictItems.value.length;
      const totalPages = Math.max(1, Math.ceil(activeLen / itemPageSize.value));
      if (currentPageRef.value > totalPages) {
        currentPageRef.value = totalPages;
      }
    } else {
      list.splice(index, 1);
      const totalPages = Math.max(1, Math.ceil(list.length / itemPageSize.value));
      if (currentPageRef.value > totalPages) {
        currentPageRef.value = totalPages;
      }
    }
  };

  const handleAddItemSubmitAdd = () => handleAddItem('add');
  const handleAddItemSubmitEdit = () => handleAddItem('edit');
  const handleRemoveItemAdd = (index: number) => handleRemoveItem('add', index);
  const handleRemoveItemEdit = (index: number) => handleRemoveItem('edit', index);

  const submitAdd = async () => {
    addSubmitted.value = true;
    if (!addForm.dictName?.trim()) return message.warning('请填写字典名称');
    if (!addForm.dictType?.trim()) return message.warning('请填写字典类型');

    // 校验字典项必填字段
    const validAddItems = addDictItems.value.filter((i) => !i.isDeleted);
    for (let i = 0; i < validAddItems.length; i++) {
      const item = validAddItems[i];
      if (!item.itemLabel?.trim()) return message.warning(`第 ${i + 1} 行：字典标签不能为空`);
      if (!item.itemValue?.trim()) return message.warning(`第 ${i + 1} 行：字典键值不能为空`);
      if (item.sortOrder === null || item.sortOrder === undefined || isNaN(item.sortOrder)) {
        return message.warning(`第 ${i + 1} 行：排序必须为数字`);
      }
      if (item.status === undefined || item.status === null) {
        return message.warning(`第 ${i + 1} 行：状态不能为空`);
      }
    }

    submitting.value = true;
    try {
      const { exists } = await dictApi.checkType(addForm.dictType.trim());
      if (exists) {
        message.error(`字典类型 '${addForm.dictType}' 已存在`);
        return;
      }
      const dictItems = addDictItems.value
        .filter((i) => !i.isDeleted)
        .map((i, idx) => ({
          id: i.id,
          dictType: addForm.dictType,
          itemCode: i.itemCode,
          itemLabel: i.itemLabel,
          itemValue: i.itemValue,
          sortOrder: Number(i.sortOrder ?? idx + 1),
          status: Number(i.status ?? 1),
          remark: i.remark,
        }));
      await dictApi.create({ ...addForm, dictItems });
      message.success('字典创建成功');
      showAddModal.value = false;
      currentPage.value = 1;
      await fetchDicts();
    } catch (error) {
      console.error('[submitAdd] 失败:', error);
    } finally {
      submitting.value = false;
    }
  };

  const handleEdit = async (record: DictDto) => {
    Object.assign(editForm, defaultForm(), record);
    editDictItems.value = [];
    selectedEditItemIds.value = [];
    editingEditItemIndex.value = null;
    editItemCurrentPage.value = 1;
    editSubmitted.value = false;
    showEditModal.value = true;

    try {
      const detail = await dictApi.getById(record.id!);
      const items =
        (detail as any)?.dictItems ?? (detail as any)?.DictItems ?? [];
      if (detail && items.length > 0) {
        editDictItems.value = items.map((item: any) => ({
          id: item.id ?? item.Id,
          dictType: item.dictType ?? item.DictType,
          itemLabel: item.itemLabel ?? item.ItemLabel ?? '',
          itemValue: item.itemValue ?? item.ItemValue ?? '',
          sortOrder: Number(item.sortOrder ?? item.SortOrder ?? 0),
          status: Number(item.status ?? item.Status ?? 1),
          remark: item.remark ?? item.Remark ?? '',
          isNew: false,
          isDeleted: false,
          _touched: false,
          _statusModified: false,
        }));
      }
    } catch (error) {
      console.error('[handleEdit] 获取字典详情失败:', error);
    }
  };

  const submitEdit = async () => {
    editSubmitted.value = true;
    if (!editForm.dictName?.trim()) return message.warning('请填写字典名称');
    if (!editForm.dictType?.trim()) return message.warning('请填写字典类型');
    if (editForm.id == null) return message.warning('缺少字典 ID');

    // 校验字典项必填字段
    const validEditItems = editDictItems.value.filter((i) => !i.isDeleted);
    for (let i = 0; i < validEditItems.length; i++) {
      const item = validEditItems[i];
      if (!item.itemLabel?.trim()) return message.warning(`第 ${i + 1} 行：字典标签不能为空`);
      if (!item.itemValue?.trim()) return message.warning(`第 ${i + 1} 行：字典键值不能为空`);
      if (item.sortOrder === null || item.sortOrder === undefined || isNaN(item.sortOrder)) {
        return message.warning(`第 ${i + 1} 行：排序必须为数字`);
      }
      if (item.status === undefined || item.status === null) {
        return message.warning(`第 ${i + 1} 行：状态不能为空`);
      }
    }

    submitting.value = true;
    try {
      const { exists } = await dictApi.checkType(editForm.dictType.trim(), editForm.id);
      if (exists) {
        message.error(`字典类型 '${editForm.dictType}' 已被其他字典占用`);
        return;
      }
      const dictItems = editDictItems.value
        .filter((i) => !i.isDeleted)
        .map((i, idx) => ({
          id: i.id,
          dictType: editForm.dictType,
          itemCode: i.itemCode,
          itemLabel: i.itemLabel,
          itemValue: i.itemValue,
          sortOrder: Number(i.sortOrder ?? idx + 1),
          status: Number(i.status ?? 1),
          remark: i.remark,
        }));
      await dictApi.update(editForm.id, { ...editForm, dictItems });
      message.success('字典已更新');
      showEditModal.value = false;
      await fetchDicts();
    } catch (error) {
      console.error('[submitEdit] 失败:', error);
    } finally {
      submitting.value = false;
    }
  };

  const handleDelete = (record: DictDto) => {
    Modal.confirm({
      title: '确认删除',
      content: `确定要删除字典「${record.dictName}」吗？`,
      okType: 'danger',
      async onOk() {
        try {
          await dictApi.delete(record.id!);
          message.success('已删除');
          await fetchDicts();
        } catch (error) {
          console.error('[handleDelete] 失败:', error);
        }
      },
    });
  };

  const handleBatchDelete = () => {
    if (selectedIds.value.length === 0) return message.warning('请至少选择一个字典');
    Modal.confirm({
      title: '确认批量删除',
      content: `确定要删除选中的 ${selectedIds.value.length} 个字典吗？`,
      okType: 'danger',
      async onOk() {
        try {
          const res = await dictApi.batchDelete(selectedIds.value);
          message.success(res?.message || `已删除 ${res?.deletedCount} 个`);
          selectedIds.value = [];
          await fetchDicts();
        } catch (error) {
          console.error('[handleBatchDelete] 失败:', error);
        }
      },
    });
  };

  const handleExport = async () => {
    try {
      const query: DictQuery = {
        dictName: dictName.value.trim() || undefined,
        dictType: dictType.value.trim() || undefined,
        status: statusFilter.value,
        ownerType: ownerTypeFilter.value,
        startDate: startDate.value || undefined,
        endDate: endDate.value || undefined,
        page: 1,
        pageSize: 65535,
      };
      const res = await dictApi.getPaged(query);
      let data = (res.items || []) as DictDto[];
      data = [...data].sort((a, b) => {
        const na = typeof a.id === 'number' ? a.id : parseInt(String(a.id ?? '0'), 10) || 0;
        const nb = typeof b.id === 'number' ? b.id : parseInt(String(b.id ?? '0'), 10) || 0;
        return na - nb;
      });
      const filename = `字典管理_${new Date().toISOString().slice(0, 10)}.xls`;
      downloadExcel(data, filename);
      message.success('导出成功');
    } catch (error) {
      console.error('[handleExport] 失败:', error);
      message.error('导出失败，请重试');
    }
  };

  const handleRefreshCache = async () => {
    try {
      const res = await dictApi.refreshCache();
      message.success(res?.message || '缓存已刷新');
    } catch (error) {
      console.error('[handleRefreshCache] 失败:', error);
    }
  };

  // 悬浮时加载字典项列表
  const loadHoverDictItems = async (record: DictDto) => {
    if (hoverDictId.value === record.id && hoverDictItems.value.length > 0) return;
    hoverDictId.value = record.id!;
    hoverDictLoading.value = true;
    hoverDictItems.value = [];
    try {
      const detail = await dictApi.getById(record.id!);
      // 兼容 PascalCase (DictItems) 和 camelCase (dictItems)
      const items =
        (detail as any)?.dictItems ?? (detail as any)?.DictItems ?? [];
      hoverDictItems.value = items.map((item: any) => ({
        id: item.id ?? item.Id,
        dictType: item.dictType ?? item.DictType,
        //itemCode: item.itemCode ?? item.ItemCode,
        itemLabel: item.itemLabel ?? item.ItemLabel ?? '',
        itemValue: item.itemValue ?? item.ItemValue ?? '',
        sortOrder: item.sortOrder ?? item.SortOrder ?? 0,
        status: item.status ?? item.Status ?? 1,
        remark: item.remark ?? item.Remark ?? '',
      }));
    } catch (error) {
      console.error('[loadHoverDictItems] 获取字典项失败:', error);
      hoverDictItems.value = [];
    } finally {
      hoverDictLoading.value = false;
    }
  };

  const clearHoverDictItems = () => {
    hoverDictId.value = null;
    hoverDictItems.value = [];
  };

  const toggleSelectAll = (e: any) => {
    const checked = e?.target?.checked ?? !isAllSelected.value;
    selectedIds.value = checked ? paginatedDicts.value.map((d) => d.id!) : [];
  };

  const formatDate = (iso?: string | null) => {
    if (!iso) return '—';
    const d = new Date(iso);
    if (isNaN(d.getTime())) return iso;
    return formatDateLocal(iso);
  };

  const getSelectedItemId = (item: DictItemDto, index: number): number | string => {
    return item.id ?? index;
  };

  const isAllAddItemsSelected = computed(() => {
    const pageItems = paginatedAddDictItems.value;
    if (pageItems.length === 0) return false;
    return pageItems.every((item) => {
      const idx = addDictItems.value.indexOf(item);
      return selectedAddItemIds.value.includes(getSelectedItemId(item, idx));
    });
  });

  const isAllEditItemsSelected = computed(() => {
    const pageItems = paginatedEditDictItems.value;
    if (pageItems.length === 0) return false;
    return pageItems.every((item) => {
      const idx = editDictItems.value.indexOf(item);
      return selectedEditItemIds.value.includes(getSelectedItemId(item, idx));
    });
  });

  const activeAddDictItems = computed(() => addDictItems.value.filter((i) => !i.isDeleted));
  const activeEditDictItems = computed(() => editDictItems.value.filter((i) => !i.isDeleted));

  const addItemTotalPages = computed(() => Math.max(1, Math.ceil(activeAddDictItems.value.length / itemPageSize.value)));
  const editItemTotalPages = computed(() => Math.max(1, Math.ceil(activeEditDictItems.value.length / itemPageSize.value)));

  const paginatedAddDictItems = computed(() => {
    const start = (addItemCurrentPage.value - 1) * itemPageSize.value;
    return activeAddDictItems.value.slice(start, start + itemPageSize.value);
  });

  const paginatedEditDictItems = computed(() => {
    const start = (editItemCurrentPage.value - 1) * itemPageSize.value;
    return activeEditDictItems.value.slice(start, start + itemPageSize.value);
  });

  const goAddItemPage = (page: number) => {
    addItemCurrentPage.value = Math.max(1, Math.min(page, addItemTotalPages.value));
  };

  const goEditItemPage = (page: number) => {
    editItemCurrentPage.value = Math.max(1, Math.min(page, editItemTotalPages.value));
  };

  const addItemPageNumbers = computed(() => {
    const total = addItemTotalPages.value;
    const current = addItemCurrentPage.value;
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
    const pages: number[] = [];
    pages.push(1);
    let start = Math.max(2, current - 1);
    let end = Math.min(total - 1, current + 1);
    if (current <= 4) {
      start = 2;
      end = Math.min(total - 1, 5);
    } else if (current >= total - 3) {
      start = Math.max(2, total - 4);
      end = total - 1;
    }
    for (let i = start; i <= end; i++) pages.push(i);
    pages.push(total);
    return pages;
  });

  const editItemPageNumbers = computed(() => {
    const total = editItemTotalPages.value;
    const current = editItemCurrentPage.value;
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
    const pages: number[] = [];
    pages.push(1);
    let start = Math.max(2, current - 1);
    let end = Math.min(total - 1, current + 1);
    if (current <= 4) {
      start = 2;
      end = Math.min(total - 1, 5);
    } else if (current >= total - 3) {
      start = Math.max(2, total - 4);
      end = total - 1;
    }
    for (let i = start; i <= end; i++) pages.push(i);
    pages.push(total);
    return pages;
  });

  const toggleSelectAllAddItems = () => {
    const pageItems = paginatedAddDictItems.value;
    if (isAllAddItemsSelected.value) {
      // 取消全选：只移除当前页的项
      const pageIds = pageItems.map((item) => {
        const idx = addDictItems.value.indexOf(item);
        return getSelectedItemId(item, idx);
      });
      selectedAddItemIds.value = selectedAddItemIds.value.filter((id) => !pageIds.includes(id));
    } else {
      // 全选：将当前页的项添加到已选列表
      const newIds = pageItems
        .map((item) => {
          const idx = addDictItems.value.indexOf(item);
          return getSelectedItemId(item, idx);
        })
        .filter((id) => !selectedAddItemIds.value.includes(id));
      selectedAddItemIds.value = [...selectedAddItemIds.value, ...newIds];
    }
  };

  const toggleSelectAllEditItems = () => {
    const pageItems = paginatedEditDictItems.value;
    if (isAllEditItemsSelected.value) {
      // 取消全选：只移除当前页的项
      const pageIds = pageItems.map((item) => {
        const idx = editDictItems.value.indexOf(item);
        return getSelectedItemId(item, idx);
      });
      selectedEditItemIds.value = selectedEditItemIds.value.filter((id) => !pageIds.includes(id));
    } else {
      // 全选：将当前页的项添加到已选列表
      const newIds = pageItems
        .map((item) => {
          const idx = editDictItems.value.indexOf(item);
          return getSelectedItemId(item, idx);
        })
        .filter((id) => !selectedEditItemIds.value.includes(id));
      selectedEditItemIds.value = [...selectedEditItemIds.value, ...newIds];
    }
  };

  const toggleSelectAddItem = (item: DictItemDto, index: number) => {
    const id = getSelectedItemId(item, index);
    const idx = selectedAddItemIds.value.indexOf(id);
    if (idx > -1) {
      selectedAddItemIds.value.splice(idx, 1);
    } else {
      selectedAddItemIds.value.push(id);
    }
  };

  const toggleSelectEditItem = (item: DictItemDto, index: number) => {
    const id = getSelectedItemId(item, index);
    const idx = selectedEditItemIds.value.indexOf(id);
    if (idx > -1) {
      selectedEditItemIds.value.splice(idx, 1);
    } else {
      selectedEditItemIds.value.push(id);
    }
  };

  const batchDeleteAddItems = () => {
    if (selectedAddItemIds.value.length === 0) return message.warning('请至少选择一项');
    const ids = selectedAddItemIds.value;
    let removeCount = 0;
    addDictItems.value = addDictItems.value.filter((item, index) => {
      const itemId = getSelectedItemId(item, index);
      if (ids.includes(itemId)) {
        if (item.isNew || item.id === undefined) {
          removeCount++;
          return false;
        } else {
          item.isDeleted = true;
          return true;
        }
      }
      return true;
    });
    selectedAddItemIds.value = [];
    // 用 active（已过滤 isDeleted）的长度计算总页数，否则删除后当前页可能超出有效范围
    const totalPages = Math.max(1, Math.ceil(activeAddDictItems.value.length / itemPageSize.value));
    if (addItemCurrentPage.value > totalPages) {
      addItemCurrentPage.value = totalPages;
    }
    message.success('删除成功');
  };

  const batchDeleteEditItems = () => {
    if (selectedEditItemIds.value.length === 0) return message.warning('请至少选择一项');
    const ids = selectedEditItemIds.value;
    let removeCount = 0;
    editDictItems.value = editDictItems.value.filter((item, index) => {
      const itemId = getSelectedItemId(item, index);
      if (ids.includes(itemId)) {
        if (item.isNew || item.id === undefined) {
          removeCount++;
          return false;
        } else {
          item.isDeleted = true;
          return true;
        }
      }
      return true;
    });
    selectedEditItemIds.value = [];
    // 用 active（已过滤 isDeleted）的长度计算总页数，否则删除后当前页可能超出有效范围
    const totalPages = Math.max(1, Math.ceil(activeEditDictItems.value.length / itemPageSize.value));
    if (editItemCurrentPage.value > totalPages) {
      editItemCurrentPage.value = totalPages;
    }
    message.success('删除成功');
  };

  const handleEditItem = (target: 'add' | 'edit', index: number) => {
    if (target === 'add') {
      editingAddItemIndex.value = index;
    } else {
      editingEditItemIndex.value = index;
    }
  };

  const handleSaveItem = (target: 'add' | 'edit') => {
    if (target === 'add') {
      editingAddItemIndex.value = null;
    } else {
      editingEditItemIndex.value = null;
    }
  };

  const handleCancelEditItem = (target: 'add' | 'edit') => {
    if (target === 'add') {
      editingAddItemIndex.value = null;
    } else {
      editingEditItemIndex.value = null;
    }
  };

  const handleItemStatusChangeAdd = (index: number) => handleItemStatusChange('add', index);
  const handleItemStatusChangeEdit = (index: number) => handleItemStatusChange('edit', index);

  return {
    dictName,
    dictType,
    statusFilter,
    ownerTypeFilter,
    startDate,
    endDate,
    currentPage,
    pageSize,
    totalDicts,
    loading,
    submitting,
    selectedIds,
    showAddModal,
    showEditModal,
    addSubmitted,
    editSubmitted,
    dicts,
    addForm,
    editForm,
    addDictItems,
    editDictItems,
    paginatedDicts,
    isAllSelected,
    totalPages,
    fetchDicts,
    handleSearch,
    handleReset,
    goToPage,
    prevPage,
    nextPage,
    handleAdd,
    submitAdd,
    handleEdit,
    submitEdit,
    handleDelete,
    handleBatchDelete,
    handleExport,
    handleRefreshCache,
    toggleSelectAll,
    formatDate,
    handleAddItemSubmitAdd,
    handleAddItemSubmitEdit,
    handleRemoveItemAdd,
    handleRemoveItemEdit,
    selectedAddItemIds,
    selectedEditItemIds,
    editingAddItemIndex,
    editingEditItemIndex,
    isAllAddItemsSelected,
    isAllEditItemsSelected,
    toggleSelectAllAddItems,
    toggleSelectAllEditItems,
    toggleSelectAddItem,
    toggleSelectEditItem,
    batchDeleteAddItems,
    batchDeleteEditItems,
    handleEditItem,
    handleSaveItem,
    handleCancelEditItem,
    itemPageSize,
    addItemCurrentPage,
    editItemCurrentPage,
    activeAddDictItems,
    activeEditDictItems,
    addItemTotalPages,
    editItemTotalPages,
    addItemPageNumbers,
    editItemPageNumbers,
    paginatedAddDictItems,
    paginatedEditDictItems,
    goAddItemPage,
    goEditItemPage,
    hoverDictItems,
    hoverDictLoading,
    hoverDictId,
    loadHoverDictItems,
    clearHoverDictItems,
    handleItemStatusChangeAdd,
    handleItemStatusChangeEdit,
  };
}
