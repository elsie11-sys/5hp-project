<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted, onBeforeUnmount } from 'vue';
import { useRoute } from 'vue-router';
import { message, Modal } from 'ant-design-vue';
import { PlusOutlined, DeleteOutlined, EditOutlined, SearchOutlined, ReloadOutlined } from '@ant-design/icons-vue';
import {
  studentApi,
  mapDtoToForm,
  uploadAvatar,
  type BackendStudentDto,
  type StudentForm,
} from '#/api/vision-archive/student';
import {
  healthArchiveApi,
  type VisionRecordDto,
  type OralRecordDto,
  type MentalRecordDto,
  type WeightRecordDto,
  type BoneRecordDto,
} from '#/api/vision-archive/health-archive';

// ================= Theme Detection =================
const isLight = ref(false);
let themeObserver: MutationObserver | null = null;

const applySystemTheme = () => {
  isLight.value = !document.documentElement.classList.contains('dark');
};

const observeTheme = () => {
  themeObserver = new MutationObserver(() => {
    isLight.value = !document.documentElement.classList.contains('dark');
  });
  themeObserver.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
};

// ================= 1. 搜索表单 =================
const searchForm = reactive({
  school: '',
  grade: '',
  name: '',
  intervention: '' as '' | 'true' | 'false',
});

// ================= 2. 数据管理 =================
const tableData = ref<BackendStudentDto[]>([]);
const loading = ref(false);
const totalCount = ref(0);

// ================= 3. 分页状态 =================
const currentPage = ref(1);
const pageSize = ref(10);
const pageSizeOptions = [10, 20, 50, 100];
const jumpPage = ref<number | null>(null);

// ================= 4. 加载数据 =================
async function loadList() {
  loading.value = true;
  try {
    const intervention =
      searchForm.intervention === 'true'
        ? true
        : searchForm.intervention === 'false'
        ? false
        : undefined;

    const res = await studentApi.getPagedList({
      page: currentPage.value,
      pageSize: pageSize.value,
      school: searchForm.school || undefined,
      className: searchForm.grade || undefined, // 搜索条件里的 grade 实际就是班级
      name: searchForm.name || undefined,
      intervention,
    });
    tableData.value = res.items;
    totalCount.value = res.total;
  } catch (e: any) {
    // 错误提示由 requestClient 的 errorMessageResponseInterceptor 统一处理
    tableData.value = [];
    totalCount.value = 0;
  } finally {
    loading.value = false;
  }
}

const totalPages = computed(() => Math.max(1, Math.ceil(totalCount.value / pageSize.value)));
const pagedData = computed(() => tableData.value);

const resetPage = () => { currentPage.value = 1; };

// ================= 5. 搜索 / 重置 =================
const handleSearch = async () => {
  resetPage();
  await loadList();
};

const handleReset = async () => {
  searchForm.school = '';
  searchForm.grade = '';
  searchForm.name = '';
  searchForm.intervention = '';
  resetPage();
  await loadList();
};

// ================= 6. 新增 / 编辑弹窗 =================
const modalVisible = ref(false);
const modalTitle = ref('新增学生');
const isEdit = ref(false);
const submitting = ref(false);

const defaultForm = (): StudentForm => ({
  id: 0,
  studentNo: '',
  avatar: '👦',
  gradeYear: '',
  school: '',
  className: '',
  name: '',
  gender: '男',
  nation: '汉族',
  birthday: '',
  idCard: '',
  nativePlace: '',
  address: '',
  parentPhone: '',
  parentContact: '',
  allergyHistory: '未知',
  fatherMyopia: '未知',
  motherMyopia: '未知',
  dataSource: '手动录入',
  intervention: false,
});

const formData = reactive<StudentForm>(defaultForm());

// ================= 6.1 头像上传 =================
const avatarInputRef = ref<HTMLInputElement | null>(null);
const avatarUploading = ref(false);

// 根据性别返回默认 emoji 头像（男=👦，女=👧，其他=👦）
const defaultAvatar = computed(() => (formData.gender === '女' ? '👧' : '👦'));

// 判断当前 avatar 是否为图片（后端 URL、绝对链接、base64）
const isAvatarImage = computed(() => {
  const v = formData.avatar;
  if (!v) return false;
  return (
    v.startsWith('data:image') ||
    /^https?:\/\//.test(v) ||
    v.startsWith('/uploads/') ||
    v.startsWith('uploads/')
  );
});

const triggerAvatarInput = () => {
  avatarInputRef.value?.click();
};

/**
 * 把图片 File 压缩到 maxSide px，最长边不超过 maxSide，输出 JPEG Blob。
 * 用于上传前压缩，避免原图太大上传慢。
 */
const compressImage = (file: File, maxSide = 512, quality = 0.85): Promise<Blob> => {
  return new Promise((resolve, reject) => {
    const url = URL.createObjectURL(file);
    const img = new Image();
    img.onload = () => {
      try {
        let { width, height } = img;
        if (width > maxSide || height > maxSide) {
          if (width >= height) {
            height = Math.round((height * maxSide) / width);
            width = maxSide;
          } else {
            width = Math.round((width * maxSide) / height);
            height = maxSide;
          }
        }
        const canvas = document.createElement('canvas');
        canvas.width = width;
        canvas.height = height;
        const ctx = canvas.getContext('2d');
        if (!ctx) {
          URL.revokeObjectURL(url);
          reject(new Error('canvas 不可用'));
          return;
        }
        ctx.drawImage(img, 0, 0, width, height);
        canvas.toBlob(
          (blob) => {
            URL.revokeObjectURL(url);
            if (!blob) {
              reject(new Error('压缩失败'));
              return;
            }
            resolve(blob);
          },
          'image/jpeg',
          quality,
        );
      } catch (err) {
        URL.revokeObjectURL(url);
        reject(err);
      }
    };
    img.onerror = () => {
      URL.revokeObjectURL(url);
      reject(new Error('图片加载失败'));
    };
    img.src = url;
  });
};

const handleAvatarChange = async (e: Event) => {
  const target = e.target as HTMLInputElement;
  const file = target.files?.[0];
  // 允许重复选择同一张图
  target.value = '';
  if (!file) return;
  if (!file.type.startsWith('image/')) {
    message.warning('请选择图片文件');
    return;
  }
  if (file.size > 2 * 1024 * 1024) {
    message.warning('图片大小不能超过 2MB');
    return;
  }

  avatarUploading.value = true;
  try {
    // 先压缩（最长边 512、JPEG 质量 0.85），再上传
    const blob = await compressImage(file);
    const res = await uploadAvatar(blob, `avatar_${Date.now()}.jpg`);
    formData.avatar = res.url; // 后端返回的相对 URL，例如 /uploads/avatars/xxx.jpg
    message.success('头像上传成功');
  } catch (err: any) {
    // 错误已由 requestClient 统一处理，这里仅给一个轻提示
    message.error(err?.message || '头像上传失败');
  } finally {
    avatarUploading.value = false;
  }
};

const clearAvatar = () => {
  // 清除自定义头像，回退到根据当前性别动态计算的默认 emoji
  formData.avatar = defaultAvatar.value;
};

/**
 * 把后端返回的相对路径（如 /uploads/avatars/xxx.png）拼成可访问的完整 URL。
 * 绝对 URL、base64、空值原样返回。
 */
const resolveAvatarUrl = (avatar: string): string => {
  if (!avatar) return '';
  if (
    avatar.startsWith('data:image') ||
    avatar.startsWith('http://') ||
    avatar.startsWith('https://') ||
    avatar.startsWith('blob:')
  ) {
    return avatar;
  }
  // 相对路径：拼当前 origin
  if (avatar.startsWith('/')) {
    return `${window.location.origin}${avatar}`;
  }
  return `${window.location.origin}/${avatar}`;
};

/** 判断行级 avatar 是否为图片（用于表格列） */
const isRowAvatarImage = (avatar: string): boolean => {
  if (!avatar) return false;
  return (
    avatar.startsWith('data:image') ||
    /^https?:\/\//.test(avatar) ||
    avatar.startsWith('/uploads/') ||
    avatar.startsWith('uploads/')
  );
};

const handleAdd = () => {
  isEdit.value = false;
  modalTitle.value = '新增学生';
  Object.assign(formData, defaultForm());
  modalVisible.value = true;
};

const handleEditStudent = (record: BackendStudentDto) => {
  isEdit.value = true;
  modalTitle.value = '编辑学生';
  Object.assign(formData, mapDtoToForm(record));
  modalVisible.value = true;
};

const handleSubmit = async () => {
  if (!formData.name || !formData.studentNo) {
    message.warning('请填写姓名和学号');
    return;
  }
  submitting.value = true;
  try {
    if (isEdit.value) {
      await studentApi.update(formData.id!, { ...formData });
      message.success('编辑成功');
    } else {
      await studentApi.create({ ...formData });
      message.success('新增成功');
    }
    modalVisible.value = false;
    await loadList();
  } catch (e: any) {
    // 错误提示已由 requestClient 统一处理
  } finally {
    submitting.value = false;
  }
};

// ================= 7. 删除 =================
const handleDelete = (record: BackendStudentDto) => {
  Modal.confirm({
    title: '确认删除',
    content: `确定要删除学生 ${record.name} 的档案吗？`,
    okButtonProps: { danger: true },
    onOk: async () => {
      try {
        await studentApi.delete(record.id);
        message.success('删除成功');
        // 如果当前页被删空，自动回退一页
        if (pagedData.value.length === 1 && currentPage.value > 1) {
          currentPage.value--;
        }
        await loadList();
      } catch (e: any) {
        // 错误由拦截器统一处理
      }
    }
  });
};

const handleBatchDelete = () => {
  if (selectedRowKeys.value.length === 0) {
    message.warning('请先勾选需要删除的学生！');
    return;
  }
  Modal.confirm({
    title: '确认批量删除',
    content: `确定要删除选中的 ${selectedRowKeys.value.length} 名学生吗？`,
    okButtonProps: { danger: true },
    onOk: async () => {
      try {
        const res = await studentApi.batchDelete(selectedRowKeys.value);
        message.success(res.message || '批量删除成功！');
        selectedRowKeys.value = [];
        if (pagedData.value.length === 0 && currentPage.value > 1) {
          currentPage.value--;
        }
        await loadList();
      } catch (e: any) {
        // 错误由拦截器统一处理
      }
    }
  });
};

const toggleIntervention = async (row: BackendStudentDto) => {
  const next = !row.intervention;
  try {
    await studentApi.toggleIntervention(row.id, next);
    row.intervention = next;
    message.success(`已${next ? '开启' : '关闭'} ${row.name} 的干预状态`);
  } catch (e: any) {
    // 错误由拦截器统一处理
  }
};

// ================= 8. 勾选状态 =================
const selectedRowKeys = ref<number[]>([]);

const toggleSelectAll = () => {
  const allIds = pagedData.value.map(item => item.id);
  if (selectedRowKeys.value.length === allIds.length && allIds.length > 0) {
    selectedRowKeys.value = [];
  } else {
    selectedRowKeys.value = allIds;
  }
};

// ================= 9. 分页操作 =================
const goToPage = async (page: number) => {
  if (page >= 1 && page <= totalPages.value && page !== currentPage.value) {
    currentPage.value = page;
    await loadList();
  }
};

const prevPage = async () => {
  if (currentPage.value > 1) {
    currentPage.value--;
    await loadList();
  }
};
const nextPage = async () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++;
    await loadList();
  }
};

// 跳转到指定页（从跳页输入框触发）
const handleJump = async () => {
  const p = Number(jumpPage.value);
  if (!Number.isFinite(p) || p < 1) {
    jumpPage.value = null;
    return;
  }
  await goToPage(Math.min(p, totalPages.value));
  jumpPage.value = null;
};

// 切换每页大小
const handleSizeChange = async (size: number) => {
  pageSize.value = size;
  currentPage.value = 1;
  await loadList();
};

// Element Plus 风格页码算法：左右各 2 个 + 1 + 1 ... + 末页
// 例（total=164, current=1）: [1,2,3,4,5,6, '...', 164]
// 例（total=164, current=80）: [1, '...', 78,79,80,81,82, '...', 164]
// 例（total=164, current=160）: [1, '...', 159,160,161,162,163,164]
const SIDE = 2;
const displayedPages = computed<(number | '...')[]>(() => {
  const total = totalPages.value;
  const current = currentPage.value;
  if (total <= 2 * SIDE + 5) {
    return Array.from({ length: total }, (_, i) => i + 1);
  }
  const result: (number | '...')[] = [];
  if (current <= SIDE + 4) {
    // 左侧不折叠
    for (let i = 1; i <= SIDE + 4; i++) result.push(i);
    result.push('...');
    result.push(total);
  } else if (current >= total - SIDE - 3) {
    // 右侧不折叠
    result.push(1);
    result.push('...');
    for (let i = total - (SIDE + 3); i <= total; i++) result.push(i);
  } else {
    // 两边都折叠
    result.push(1);
    result.push('...');
    for (let i = current - SIDE; i <= current + SIDE; i++) result.push(i);
    result.push('...');
    result.push(total);
  }
  return result;
});

// ================= 9.1 性别变化时同步默认 emoji 头像 =================
// 仅在当前 avatar 是默认 emoji（不是用户上传的图片）时，才跟着性别切换。
watch(
  () => formData.gender,
  () => {
    if (!isAvatarImage.value) {
      formData.avatar = defaultAvatar.value;
    }
  },
);

// ================= 10. 学生详情弹窗 =================
const studentDetailVisible = ref(false);
const currentStudent = ref<BackendStudentDto | null>(null);
const activeDetailTab = ref('vision');
const detailLoading = ref(false);

// 5 个健康维度的历史记录
const visionRecords = ref<VisionRecordDto[]>([]);
const oralRecords = ref<OralRecordDto[]>([]);
const mentalRecords = ref<MentalRecordDto[]>([]);
const weightRecords = ref<WeightRecordDto[]>([]);
const boneRecords = ref<BoneRecordDto[]>([]);

// 计算属性：最新一条（数组第一项 = 后端按 check_date desc 已排好）
const latestVision = computed(() => visionRecords.value[0]);
const latestOral = computed(() => oralRecords.value[0]);
const latestMental = computed(() => mentalRecords.value[0]);
const latestWeight = computed(() => weightRecords.value[0]);
const latestBone = computed(() => boneRecords.value[0]);

async function loadHealthArchives(studentId: number) {
  detailLoading.value = true;
  try {
    // 5 个维度并发拉取
    const [v, o, m, w, b] = await Promise.all([
      healthArchiveApi.vision.getByStudent(studentId),
      healthArchiveApi.oral.getByStudent(studentId),
      healthArchiveApi.mental.getByStudent(studentId),
      healthArchiveApi.weight.getByStudent(studentId),
      healthArchiveApi.bone.getByStudent(studentId),
    ]);
    visionRecords.value = v;
    oralRecords.value = o;
    mentalRecords.value = m;
    weightRecords.value = w;
    boneRecords.value = b;
  } catch (e: any) {
    // 错误由拦截器统一处理
    visionRecords.value = [];
    oralRecords.value = [];
    mentalRecords.value = [];
    weightRecords.value = [];
    boneRecords.value = [];
  } finally {
    detailLoading.value = false;
  }
}

const handleStudentNoClick = async (record: BackendStudentDto) => {
  currentStudent.value = record;
  studentDetailVisible.value = true;
  activeDetailTab.value = 'vision';
  await loadHealthArchives(record.id);
};

const closeDetailModal = () => {
  studentDetailVisible.value = false;
  currentStudent.value = null;
  // 清空 5 个维度数据，避免下次打开看到上次学生的内容
  visionRecords.value = [];
  oralRecords.value = [];
  mentalRecords.value = [];
  weightRecords.value = [];
  boneRecords.value = [];
};

const switchDetailTab = (tab: string) => {
  activeDetailTab.value = tab;
};

const handlePrintStudent = (record: BackendStudentDto) => {
  message.info(`打印学生 ${record.name} 的档案`);
};

const route = useRoute();

onMounted(async () => {
  applySystemTheme();
  observeTheme();

  // 首次进入页面时拉第一页
  await loadList();

  // 支持通过 ?studentId=xxx 路由参数定位学生
  const studentId = route.query.studentId as string;
  if (studentId) {
    let student = tableData.value.find(
      (item) => String(item.studentNo) === studentId || String(item.id) === studentId
    );
    if (!student) {
      // 列表里没命中，主动去后端查
      try {
        student = await studentApi.getByNo(studentId);
      } catch (_e) {
        // 找不到也无所谓，当作未命中处理
      }
    }
    if (student) {
      searchForm.name = student.name;
      resetPage();
      await loadList();
      message.success(`已定位到学生：${student.name}`);
      setTimeout(() => {
        handleStudentNoClick(student!);
      }, 200);
    } else {
      // 既然是路由定位，把学号放进搜索框尝试模糊匹配
      searchForm.name = studentId;
      resetPage();
      await loadList();
      message.info('未在现有档案中找到该学生，可手动查询');
    }
  }
});

onBeforeUnmount(() => {
  if (themeObserver) {
    themeObserver.disconnect();
    themeObserver = null;
  }
});
</script>

<template>
  <div class="app-root" :class="{ 'theme-light': isLight }">
    <!-- 背景装饰 -->
    <div class="bg-decor" aria-hidden="true">
      <div class="bg-grid"></div>
      <div class="bg-glow bg-glow-1"></div>
      <div class="bg-glow bg-glow-2"></div>
    </div>

    <!-- 扫描线 -->
    <div class="scan-line"></div>

    <!-- 顶部装饰光带 -->
    <div class="tech-topbar">
      <div class="tech-corner tech-corner-l"><span class="tc-dot"></span></div>
      <div class="tech-line"><span class="line-pulse"></span></div>
      <div class="tech-dots">
        <span></span><span></span><span></span><span></span><span></span>
      </div>
      <div class="tech-line"><span class="line-pulse"></span></div>
      <div class="tech-corner tech-corner-r"><span class="tc-dot"></span></div>
    </div>

    <div class="page-container">

      <!-- 页面标题 -->
      <div class="page-header">
        <div class="header-deco header-deco-l">
          <span class="hd-line"></span>
          <span class="hd-dot"></span>
        </div>
        <div class="header-main">
          <h1 class="page-title">
            <span class="title-bracket">【</span>
            <span class="title-text">学生档案管理</span>
            <span class="title-bracket">】</span>
            <span class="title-shine"></span>
          </h1>
          <div class="header-stats">
            <div class="stat-item">
              <span class="stat-label">档案总数</span>
              <span class="stat-value">{{ totalCount }}</span>
            </div>
            <span class="stat-divider"></span>
            <div class="stat-item">
              <span class="stat-label">干预中</span>
              <span class="stat-value stat-warn">{{ tableData.filter(i => i.intervention).length }}</span>
            </div>
            <span class="stat-divider"></span>
            <div class="stat-item">
              <span class="stat-label">活跃档案</span>
              <span class="stat-value stat-ok">{{ tableData.length }}</span>
            </div>
          </div>
        </div>
        <div class="header-deco header-deco-r">
          <span class="hd-dot"></span>
          <span class="hd-line"></span>
        </div>
      </div>

      <!-- 1. 搜索栏 -->
      <div class="search-panel">
        <div class="search-filters">
          <div class="filter-item">
            <label class="filter-label">学校:</label>
            <select v-model="searchForm.school" class="tech-select">
              <option value="">请选择学校</option>
              <option value="淮安外国语学校">淮安外国语学校</option>
              <option value="大冶市基二实验...">大冶市基二实验...</option>
              <option value="坊前中学">坊前中学</option>
              <option value="学铺侨实验小学">学铺侨实验小学</option>
              <option value="寒亭实验中学">寒亭实验中学</option>
            </select>
          </div>

          <div class="filter-item">
            <label class="filter-label">班级:</label>
            <select v-model="searchForm.grade" class="tech-select">
              <option value="">请选择班级</option>
              <option value="七年级五班">七年级五班</option>
              <option value="六年级一班">六年级一班</option>
            </select>
          </div>

          <div class="filter-item">
            <label class="filter-label">姓名:</label>
            <input v-model="searchForm.name" type="text" placeholder="请输入学生姓名" class="tech-input" />
          </div>

          <div class="filter-item">
            <label class="filter-label">干预:</label>
            <select v-model="searchForm.intervention" class="tech-select">
              <option value="">全部</option>
              <option value="true">是</option>
              <option value="false">否</option>
            </select>
          </div>
        </div>

        <div class="search-actions">
          <button @click="handleSearch" class="tech-btn tech-btn-primary">
            <SearchOutlined /> 搜索
          </button>
          <button @click="handleReset" class="tech-btn tech-btn-secondary">
            <ReloadOutlined /> 重置
          </button>
        </div>
      </div>

      <!-- 2. 操作栏 -->
      <div class="action-bar">
        <button class="tech-btn tech-btn-primary" @click="handleAdd">
          <PlusOutlined /> 新增
        </button>
        <button class="tech-btn tech-btn-danger" @click="handleBatchDelete">
          <DeleteOutlined /> 批量删除
        </button>
        <span class="action-bar-total">共 {{ totalCount }} 条记录</span>
      </div>

      <!-- 3. 列表 -->
      <div class="table-panel">
        <div class="table-scroll">
          <table class="tech-table">
            <thead>
              <tr>
                <th class="col-check">
                  <input type="checkbox" @change="toggleSelectAll" :checked="pagedData.length > 0 && selectedRowKeys.length === pagedData.length" class="tech-checkbox" />
                </th>
                <th class="col-avatar">头像</th>
                <th class="col-student-no">学号</th>
                <th class="col-grade">级/届</th>
                <th class="col-school">学校</th>
                <th class="col-class">班级</th>
                <th class="col-name">姓名</th>
                <th class="col-gender">性别</th>
                <th class="col-nation">民族</th>
                <th class="col-birthday">出生日期</th>
                <th class="col-idcard">身份证</th>
                <th class="col-native">籍贯</th>
                <th class="col-address">现居住地</th>
                <th class="col-phone">家长电话</th>
                <th class="col-contact">家长微信/QQ</th>
                <th class="col-allergy">过敏史</th>
                <th class="col-father">父亲近视</th>
                <th class="col-mother">母亲近视</th>
                <th class="col-source">数据来源</th>
                <th class="col-intervention">是否干预</th>
              </tr>
            </thead>
            <tbody>
              <template v-if="pagedData.length > 0">
                <tr v-for="(row, idx) in pagedData" :key="row.id" :class="{ 'row-alt': idx % 2 === 1 }">
                  <td class="col-check">
                    <input type="checkbox" :value="row.id" v-model="selectedRowKeys" class="tech-checkbox" />
                  </td>
                  <td class="col-avatar">
                    <div class="avatar-circle">
                      <img v-if="isRowAvatarImage(row.avatar)" :src="resolveAvatarUrl(row.avatar)" class="avatar-circle__img" />
                      <span v-else>{{ row.avatar }}</span>
                    </div>
                  </td>
                  <td class="col-student-no">
                    <span @click="handleStudentNoClick(row)" class="student-no-link">{{ row.studentNo }}</span>
                  </td>
                  <td class="col-grade text-strong">{{ row.gradeYear }}</td>
                  <td class="col-school">{{ row.school }}</td>
                  <td class="col-class">{{ row.className }}</td>
                  <td class="col-name text-strong">{{ row.name }}</td>
                  <td class="col-gender">{{ row.gender }}</td>
                  <td class="col-nation">{{ row.nation }}</td>
                  <td class="col-birthday">{{ row.birthday }}</td>
                  <td class="col-idcard mono-text">{{ row.idCard }}</td>
                  <td class="col-native">{{ row.nativePlace }}</td>
                  <td class="col-address">{{ row.address }}</td>
                  <td class="col-phone">{{ row.parentPhone || '-' }}</td>
                  <td class="col-contact">{{ row.parentContact || '-' }}</td>
                  <td class="col-allergy"><span class="tag tag-default">{{ row.allergyHistory }}</span></td>
                  <td class="col-father"><span class="tag tag-default">{{ row.fatherMyopia }}</span></td>
                  <td class="col-mother"><span class="tag tag-default">{{ row.motherMyopia }}</span></td>
                  <td class="col-source">{{ row.dataSource }}</td>
                  <td class="col-intervention">
                    <span @click="toggleIntervention(row)" class="tag tag-clickable" :class="row.intervention ? 'tag-danger' : 'tag-success'">
                      {{ row.intervention ? '是' : '否' }}
                    </span>
                  </td>
                </tr>
              </template>
              <tr v-else>
                <td colspan="21" class="empty-state">
                  <div class="empty-content">
                    <span class="empty-icon">{{ loading ? '⏳' : '📭' }}</span>
                    <span class="empty-text">{{ loading ? '数据加载中…' : '暂无数据' }}</span>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- 浮动操作列：从表格中独立出来，钉在 .table-panel 视口最右 -->
        <div class="floating-actions">
          <div class="floating-actions__head">操作</div>
          <div
            v-for="(row, idx) in pagedData"
            :key="row.id"
            class="floating-actions__row"
            :class="{ 'row-alt': idx % 2 === 1 }"
          >
            <div class="action-btns">
              <button @click="handleEditStudent(row)" class="icon-btn icon-btn-success" title="编辑">
                <EditOutlined />
              </button>
              <button @click="handleDelete(row)" class="icon-btn icon-btn-danger" title="删除">
                <DeleteOutlined />
              </button>
            </div>
          </div>
          <!-- 空状态行：保持与表格 empty 行同高 -->
          <div v-if="pagedData.length === 0" class="floating-actions__row floating-actions__row--empty"></div>
        </div>

        <!-- 分页器（Element Plus 风格：左 2 右 2 + 省略 + 末页 + 跳页 + 每页大小） -->
        <div class="pagination">
          <div class="pagination-info">共 {{ totalCount }} 条</div>

          <div class="pagination-controls">
            <!-- 上一页 -->
            <button
              class="page-btn page-btn-icon"
              :disabled="currentPage === 1"
              @click="prevPage"
              title="上一页"
            >
              <span class="page-btn-chevron">‹</span>
            </button>

            <!-- 页码 + 省略号 -->
            <template v-for="(item, idx) in displayedPages" :key="`${item}-${idx}`">
              <span v-if="item === '...'" class="page-ellipsis">···</span>
              <button
                v-else
                class="page-btn"
                :class="{ 'page-btn-active': item === currentPage }"
                @click="goToPage(item)"
              >
                {{ item }}
              </button>
            </template>

            <!-- 下一页 -->
            <button
              class="page-btn page-btn-icon"
              :disabled="currentPage === totalPages"
              @click="nextPage"
              title="下一页"
            >
              <span class="page-btn-chevron">›</span>
            </button>
          </div>

          <!-- 每页大小 -->
          <div class="pagination-sizes">
            <select
              class="tech-select pagination-size-select"
              :value="pageSize"
              @change="(e) => handleSizeChange(Number((e.target as HTMLSelectElement).value))"
            >
              <option v-for="opt in pageSizeOptions" :key="opt" :value="opt">{{ opt }} 条/页</option>
            </select>
          </div>

          <!-- 跳页 -->
          <div class="pagination-jumper">
            前往
            <input
              v-model.number="jumpPage"
              type="number"
              min="1"
              :max="totalPages"
              class="tech-input pagination-jumper__input"
              @keyup.enter="handleJump"
            />
            页
          </div>
        </div>
      </div>

      <!-- ================= 新增/编辑弹窗 ================= -->
      <Modal v-model:open="modalVisible" :title="modalTitle" width="700px" @ok="handleSubmit" @cancel="modalVisible = false" :confirmLoading="submitting" class="tech-modal" :getContainer="false">
        <!-- 头像上传 -->
        <div class="avatar-uploader">
          <label class="avatar-uploader__label">头像</label>
          <div class="avatar-uploader__main">
            <div class="avatar-uploader__preview" @click="triggerAvatarInput" :title="isAvatarImage ? '点击更换头像' : '点击添加头像'">
              <img v-if="isAvatarImage" :src="resolveAvatarUrl(formData.avatar)" class="avatar-uploader__img" />
              <span v-else-if="formData.avatar" class="avatar-uploader__emoji">{{ formData.avatar }}</span>
              <span v-else-if="avatarUploading" class="avatar-uploader__plus">…</span>
              <span v-else class="avatar-uploader__plus">+</span>
            </div>
            <div class="avatar-uploader__actions">
              <button type="button" class="avatar-uploader__btn" :disabled="avatarUploading" @click="triggerAvatarInput">
                {{ avatarUploading ? '上传中…' : (isAvatarImage ? '更换' : '添加') }}
              </button>
              <button v-if="!avatarUploading && isAvatarImage" type="button" class="avatar-uploader__btn avatar-uploader__btn--ghost" @click="clearAvatar">
                清除
              </button>
            </div>
          </div>
          <input
            ref="avatarInputRef"
            type="file"
            accept="image/*"
            style="display: none"
            @change="handleAvatarChange"
          />
        </div>

        <div class="form-grid">
          <div class="form-item">
            <label class="form-label">学号 *</label>
            <input v-model="formData.studentNo" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">姓名 *</label>
            <input v-model="formData.name" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">性别</label>
            <select v-model="formData.gender" class="tech-select">
              <option value="男">男</option>
              <option value="女">女</option>
            </select>
          </div>
          <div class="form-item">
            <label class="form-label">民族</label>
            <select v-model="formData.nation" class="tech-select">
              <option value="">请选择民族</option>
              <option value="汉族">汉族</option>
              <option value="蒙古族">蒙古族</option>
              <option value="回族">回族</option>
              <option value="藏族">藏族</option>
              <option value="维吾尔族">维吾尔族</option>
              <option value="苗族">苗族</option>
              <option value="彝族">彝族</option>
              <option value="壮族">壮族</option>
              <option value="布依族">布依族</option>
              <option value="朝鲜族">朝鲜族</option>
              <option value="满族">满族</option>
              <option value="侗族">侗族</option>
              <option value="瑶族">瑶族</option>
              <option value="白族">白族</option>
              <option value="土家族">土家族</option>
              <option value="哈尼族">哈尼族</option>
              <option value="哈萨克族">哈萨克族</option>
              <option value="傣族">傣族</option>
              <option value="黎族">黎族</option>
              <option value="傈僳族">傈僳族</option>
              <option value="佤族">佤族</option>
              <option value="畲族">畲族</option>
              <option value="高山族">高山族</option>
              <option value="拉祜族">拉祜族</option>
              <option value="水族">水族</option>
              <option value="东乡族">东乡族</option>
              <option value="纳西族">纳西族</option>
              <option value="景颇族">景颇族</option>
              <option value="柯尔克孜族">柯尔克孜族</option>
              <option value="土族">土族</option>
              <option value="达斡尔族">达斡尔族</option>
              <option value="仫佬族">仫佬族</option>
              <option value="羌族">羌族</option>
              <option value="布朗族">布朗族</option>
              <option value="撒拉族">撒拉族</option>
              <option value="毛南族">毛南族</option>
              <option value="仡佬族">仡佬族</option>
              <option value="锡伯族">锡伯族</option>
              <option value="阿昌族">阿昌族</option>
              <option value="普米族">普米族</option>
              <option value="塔吉克族">塔吉克族</option>
              <option value="怒族">怒族</option>
              <option value="乌孜别克族">乌孜别克族</option>
              <option value="俄罗斯族">俄罗斯族</option>
              <option value="鄂温克族">鄂温克族</option>
              <option value="德昂族">德昂族</option>
              <option value="保安族">保安族</option>
              <option value="裕固族">裕固族</option>
              <option value="京族">京族</option>
              <option value="塔塔尔族">塔塔尔族</option>
              <option value="独龙族">独龙族</option>
              <option value="鄂伦春族">鄂伦春族</option>
              <option value="赫哲族">赫哲族</option>
              <option value="门巴族">门巴族</option>
              <option value="珞巴族">珞巴族</option>
              <option value="基诺族">基诺族</option>
            </select>
          </div>
          <div class="form-item">
            <label class="form-label">出生日期</label>
            <input type="date" v-model="formData.birthday" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">身份证</label>
            <input v-model="formData.idCard" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">籍贯</label>
            <input v-model="formData.nativePlace" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">现居住地</label>
            <input v-model="formData.address" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">家长电话</label>
            <input v-model="formData.parentPhone" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">家长微信/QQ</label>
            <input v-model="formData.parentContact" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">过敏史</label>
            <input v-model="formData.allergyHistory" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">父亲近视</label>
            <select v-model="formData.fatherMyopia" class="tech-select">
              <option value="">请选择</option>
              <option value="是">是</option>
              <option value="否">否</option>
              <option value="未知">未知</option>
            </select>
          </div>
          <div class="form-item">
            <label class="form-label">母亲近视</label>
            <select v-model="formData.motherMyopia" class="tech-select">
              <option value="">请选择</option>
              <option value="是">是</option>
              <option value="否">否</option>
              <option value="未知">未知</option>
            </select>
          </div>
          <div class="form-item">
            <label class="form-label">年级/届</label>
            <select v-model="formData.gradeYear" class="tech-select">
              <option value="">请选择年级</option>
              <option value="一年级">一年级</option>
              <option value="二年级">二年级</option>
              <option value="三年级">三年级</option>
              <option value="四年级">四年级</option>
              <option value="五年级">五年级</option>
              <option value="六年级">六年级</option>
              <option value="初一">初一</option>
              <option value="初二">初二</option>
              <option value="初三">初三</option>
              <option value="高一">高一</option>
              <option value="高二">高二</option>
              <option value="高三">高三</option>
            </select>
          </div>
          <div class="form-item">
            <label class="form-label">学校</label>
            <select v-model="formData.school" class="tech-select">
              <option value="">请选择学校</option>
              <option value="淮安外国语学校">淮安外国语学校</option>
              <option value="大冶市基二实验小学">大冶市基二实验小学</option>
              <option value="坊前中学">坊前中学</option>
              <option value="学铺侨实验小学">学铺侨实验小学</option>
              <option value="寒亭实验中学">寒亭实验中学</option>
            </select>
          </div>
          <div class="form-item">
            <label class="form-label">班级</label>
            <input v-model="formData.className" class="tech-input" />
          </div>
          <div class="form-item">
            <label class="form-label">数据来源</label>
            <input v-model="formData.dataSource" class="tech-input" />
          </div>
          <div class="form-item form-item-full">
            <label class="form-label">是否干预</label>
            <input type="checkbox" v-model="formData.intervention" class="tech-checkbox" />
            <span class="checkbox-label">{{ formData.intervention ? '是' : '否' }}</span>
          </div>
        </div>
      </Modal>

      <!-- ================= 学生详情弹窗 ================= -->
      <Modal v-model:open="studentDetailVisible" :title="`学生档案 - ${currentStudent?.name || ''}`" width="950px" :footer="null" @cancel="closeDetailModal" class="student-detail-modal" :getContainer="false">
        <div v-if="currentStudent" class="student-detail-content">
          <!-- 学生基本信息 -->
          <div class="student-basic-info">
            <div class="info-grid">
              <div class="info-item"><label>学号：</label><span class="info-value mono-text">{{ currentStudent.studentNo }}</span></div>
              <div class="info-item"><label>姓名：</label><span class="info-value text-strong">{{ currentStudent.name }}</span></div>
              <div class="info-item"><label>性别：</label><span class="info-value">{{ currentStudent.gender }}</span></div>
              <div class="info-item"><label>年级：</label><span class="info-value">{{ currentStudent.gradeYear }}</span></div>
              <div class="info-item"><label>班级：</label><span class="info-value">{{ currentStudent.className }}</span></div>
              <div class="info-item"><label>学校：</label><span class="info-value">{{ currentStudent.school }}</span></div>
              <div class="info-item"><label>出生日期：</label><span class="info-value">{{ currentStudent.birthday }}</span></div>
              <div class="info-item"><label>家长电话：</label><span class="info-value">{{ currentStudent.parentPhone }}</span></div>
              <div class="info-item">
                <label>是否干预：</label>
                <span class="info-value" :class="currentStudent.intervention ? 'text-danger' : 'text-success'">
                  {{ currentStudent.intervention ? '是' : '否' }}
                </span>
              </div>
            </div>
          </div>

          <!-- 详情Tab切换 -->
          <div class="detail-tabs">
            <div class="detail-tab" :class="{ active: activeDetailTab === 'vision' }" @click="switchDetailTab('vision')"><span class="tab-icon">👁️</span> 视力健康</div>
            <div class="detail-tab" :class="{ active: activeDetailTab === 'oral' }" @click="switchDetailTab('oral')"><span class="tab-icon">🦷</span> 口腔健康</div>
            <div class="detail-tab" :class="{ active: activeDetailTab === 'mental' }" @click="switchDetailTab('mental')"><span class="tab-icon">🧠</span> 心理健康</div>
            <div class="detail-tab" :class="{ active: activeDetailTab === 'weight' }" @click="switchDetailTab('weight')"><span class="tab-icon">⚖️</span> 健康体重</div>
            <div class="detail-tab" :class="{ active: activeDetailTab === 'bone' }" @click="switchDetailTab('bone')"><span class="tab-icon">🦴</span> 骨骼健康</div>
          </div>

          <!-- 视力健康 -->
          <div v-show="activeDetailTab === 'vision'" class="detail-panel">
            <div class="current-status">
              <div class="status-item"><span class="status-label">左眼视力</span><span class="status-value">{{ latestVision?.leftEye || '—' }}</span></div>
              <div class="status-item"><span class="status-label">右眼视力</span><span class="status-value">{{ latestVision?.rightEye || '—' }}</span></div>
              <div class="status-item"><span class="status-label">视力等级</span>
                <span class="status-value level-tag" :class="{'level-normal': latestVision?.visionLevel === '正常','level-low': latestVision?.visionLevel === '轻度近视','level-mid': latestVision?.visionLevel === '中度近视','level-high': latestVision?.visionLevel === '高度近视'}">{{ latestVision?.visionLevel || '—' }}</span>
              </div>
              <div class="status-item"><span class="status-label">最近检查</span><span class="status-value">{{ latestVision?.checkDate || '—' }}</span></div>
            </div>
            <div class="history-record">
              <h4 class="history-title">📋 历史记录</h4>
              <table class="history-table">
                <thead><tr><th>检查日期</th><th>左眼</th><th>右眼</th><th>视力等级</th></tr></thead>
                <tbody>
                  <tr v-if="visionRecords.length === 0"><td colspan="4" class="empty-row">{{ detailLoading ? '加载中…' : '暂无视力记录' }}</td></tr>
                  <tr v-for="item in visionRecords" :key="item.id">
                    <td>{{ item.checkDate }}</td>
                    <td>{{ item.leftEye }}</td>
                    <td>{{ item.rightEye }}</td>
                    <td>{{ item.visionLevel }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- 口腔健康 -->
          <div v-show="activeDetailTab === 'oral'" class="detail-panel">
            <div class="current-status">
              <div class="status-item"><span class="status-label">口腔状态</span>
                <span class="status-value" :class="{'status-good': latestOral?.toothStatus === '良好','status-warning': latestOral?.toothStatus === '轻微龋齿'}">{{ latestOral?.toothStatus || '—' }}</span>
              </div>
              <div class="status-item"><span class="status-label">龋齿数量</span><span class="status-value">{{ latestOral?.cavityCount ?? 0 }} 颗</span></div>
              <div class="status-item"><span class="status-label">最近检查</span><span class="status-value">{{ latestOral?.checkDate || '—' }}</span></div>
            </div>
            <div class="history-record">
              <h4 class="history-title">📋 历史记录</h4>
              <table class="history-table">
                <thead><tr><th>检查日期</th><th>口腔状态</th><th>龋齿数量</th></tr></thead>
                <tbody>
                  <tr v-if="oralRecords.length === 0"><td colspan="3" class="empty-row">{{ detailLoading ? '加载中…' : '暂无口腔记录' }}</td></tr>
                  <tr v-for="item in oralRecords" :key="item.id">
                    <td>{{ item.checkDate }}</td>
                    <td>{{ item.toothStatus }}</td>
                    <td>{{ item.cavityCount }} 颗</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- 心理健康 -->
          <div v-show="activeDetailTab === 'mental'" class="detail-panel">
            <div class="current-status">
              <div class="status-item"><span class="status-label">压力水平</span>
                <span class="status-value" :class="{'status-good': latestMental?.stressLevel === '轻度','status-warning': latestMental?.stressLevel === '中度','status-danger': latestMental?.stressLevel === '重度'}">{{ latestMental?.stressLevel || '—' }}</span>
              </div>
              <div class="status-item"><span class="status-label">睡眠质量</span>
                <span class="status-value" :class="{'status-good': latestMental?.sleepQuality === '良好','status-warning': latestMental?.sleepQuality === '一般'}">{{ latestMental?.sleepQuality || '—' }}</span>
              </div>
              <div class="status-item"><span class="status-label">情绪状态</span>
                <span class="status-value" :class="{'status-good': latestMental?.moodStatus === '稳定','status-warning': latestMental?.moodStatus === '波动'}">{{ latestMental?.moodStatus || '—' }}</span>
              </div>
              <div class="status-item"><span class="status-label">最近评估</span><span class="status-value">{{ latestMental?.checkDate || '—' }}</span></div>
            </div>
            <div class="history-record">
              <h4 class="history-title">📋 历史记录</h4>
              <table class="history-table">
                <thead><tr><th>评估日期</th><th>压力水平</th><th>睡眠质量</th><th>情绪状态</th></tr></thead>
                <tbody>
                  <tr v-if="mentalRecords.length === 0"><td colspan="4" class="empty-row">{{ detailLoading ? '加载中…' : '暂无心理记录' }}</td></tr>
                  <tr v-for="item in mentalRecords" :key="item.id">
                    <td>{{ item.checkDate }}</td>
                    <td>{{ item.stressLevel }}</td>
                    <td>{{ item.sleepQuality }}</td>
                    <td>{{ item.moodStatus }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- 健康体重 -->
          <div v-show="activeDetailTab === 'weight'" class="detail-panel">
            <div class="current-status">
              <div class="status-item"><span class="status-label">身高</span><span class="status-value">{{ latestWeight?.height || '—' }}</span></div>
              <div class="status-item"><span class="status-label">体重</span><span class="status-value">{{ latestWeight?.weight || '—' }}</span></div>
              <div class="status-item"><span class="status-label">BMI</span><span class="status-value">{{ latestWeight?.bmi || '—' }}</span></div>
              <div class="status-item"><span class="status-label">BMI等级</span>
                <span class="status-value level-tag" :class="{'level-normal': latestWeight?.bmiLevel === '正常','level-low': latestWeight?.bmiLevel === '偏瘦','level-mid': latestWeight?.bmiLevel === '超重','level-high': latestWeight?.bmiLevel === '肥胖'}">{{ latestWeight?.bmiLevel || '—' }}</span>
              </div>
              <div class="status-item"><span class="status-label">腰围</span><span class="status-value">{{ latestWeight?.waistCircumference || '—' }}</span></div>
              <div class="status-item"><span class="status-label">臀围</span><span class="status-value">{{ latestWeight?.hipCircumference || '—' }}</span></div>
              <div class="status-item"><span class="status-label">腰臀比</span><span class="status-value">{{ latestWeight?.whr || '—' }}</span></div>
              <div class="status-item"><span class="status-label">最近测量</span><span class="status-value">{{ latestWeight?.checkDate || '—' }}</span></div>
            </div>
            <div class="history-record">
              <h4 class="history-title">📋 体重管理历史记录</h4>
              <table class="history-table">
                <thead><tr><th>测量日期</th><th>身高</th><th>体重</th><th>BMI</th><th>BMI等级</th><th>腰围</th><th>臀围</th><th>腰臀比</th></tr></thead>
                <tbody>
                  <tr v-if="weightRecords.length === 0"><td colspan="8" class="empty-row">{{ detailLoading ? '加载中…' : '暂无体重记录' }}</td></tr>
                  <tr v-for="item in weightRecords" :key="item.id">
                    <td>{{ item.checkDate }}</td>
                    <td>{{ item.height }}</td>
                    <td>{{ item.weight }}</td>
                    <td>{{ item.bmi }}</td>
                    <td>{{ item.bmiLevel }}</td>
                    <td>{{ item.waistCircumference }}</td>
                    <td>{{ item.hipCircumference }}</td>
                    <td>{{ item.whr }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- 骨骼健康 -->
          <div v-show="activeDetailTab === 'bone'" class="detail-panel">
            <div class="current-status">
              <div class="status-item"><span class="status-label">骨密度</span>
                <span class="status-value" :class="{'status-good': latestBone?.boneDensity === '正常','status-warning': latestBone?.boneDensity === '偏低','status-danger': latestBone?.boneDensity === '骨质疏松'}">{{ latestBone?.boneDensity || '—' }}</span>
              </div>
              <div class="status-item"><span class="status-label">骨龄</span><span class="status-value">{{ latestBone?.boneAge || '—' }}</span></div>
              <div class="status-item"><span class="status-label">维生素D</span>
                <span class="status-value" :class="{'status-good': latestBone?.vitaminD === '充足' || latestBone?.vitaminD === '良好','status-warning': latestBone?.vitaminD === '不足'}">{{ latestBone?.vitaminD || '—' }}</span>
              </div>
              <div class="status-item"><span class="status-label">钙水平</span>
                <span class="status-value" :class="{'status-good': latestBone?.calciumLevel === '正常','status-warning': latestBone?.calciumLevel === '偏低'}">{{ latestBone?.calciumLevel || '—' }}</span>
              </div>
              <div class="status-item"><span class="status-label">最近检查</span><span class="status-value">{{ latestBone?.checkDate || '—' }}</span></div>
            </div>
            <div class="history-record">
              <h4 class="history-title">📋 骨骼健康历史记录</h4>
              <table class="history-table">
                <thead><tr><th>检查日期</th><th>骨密度</th><th>骨龄</th><th>维生素D</th><th>钙水平</th></tr></thead>
                <tbody>
                  <tr v-if="boneRecords.length === 0"><td colspan="5" class="empty-row">{{ detailLoading ? '加载中…' : '暂无骨骼记录' }}</td></tr>
                  <tr v-for="item in boneRecords" :key="item.id">
                    <td>{{ item.checkDate }}</td>
                    <td>{{ item.boneDensity }}</td>
                    <td>{{ item.boneAge }}</td>
                    <td>{{ item.vitaminD }}</td>
                    <td>{{ item.calciumLevel }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </Modal>

    </div>
  </div>
</template>

<style scoped>
/* ===== CSS Variables ===== */
.app-root {
  /* Dark mode - sci-fi tech blue (same as index.vue) */
  --bg-page: #060d1f;
  --bg-card: rgba(10, 22, 50, 0.82);
  --bg-soft: rgba(56, 189, 248, 0.04);
  --bg-hover: rgba(56, 189, 248, 0.08);
  --border: rgba(56, 189, 248, 0.22);
  --border-strong: rgba(56, 189, 248, 0.45);
  --border-soft: rgba(56, 189, 248, 0.12);
  --text: #e2e8f0;
  --text-strong: #f1f5f9;
  --text-dim: #94a3b8;
  --text-muted: #64748b;
  --primary: #38bdf8;
  --primary-hover: #7dd3fc;
  --primary-bg: rgba(56, 189, 248, 0.12);
  --secondary: #a78bfa;
  --accent: #f472b6;
  --success: #34d399;
  --warning: #fbbf24;
  --danger: #fb7185;
  --glow: rgba(56, 189, 248, 0.45);
  --shadow: 0 4px 24px rgba(0, 0, 0, 0.35), 0 0 0 1px rgba(56, 189, 248, 0.06);
  --shadow-card: 0 8px 32px rgba(0, 0, 0, 0.4), 0 0 0 1px rgba(56, 189, 248, 0.08);
  --scrollbar-thumb: rgba(56, 189, 248, 0.3);

  min-height: 100vh;
  width: 100%;
  position: relative;
  background: var(--bg-page);
  background-size: cover;
  background-attachment: fixed;
  color: var(--text);
  font-family: 'Inter', 'PingFang SC', 'Microsoft YaHei', system-ui, sans-serif;
}

.app-root > * {
  position: relative;
  z-index: 2;
}

.app-root.theme-light {
  /* Light mode - light sci-fi with cyan accents */
  --bg-page: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 40%, #f0f7ff 100%);
  --bg-card: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  --bg-soft: rgba(8, 145, 178, 0.04);
  --bg-hover: rgba(8, 145, 178, 0.07);
  --border: rgba(8, 145, 178, 0.2);
  --border-strong: rgba(8, 145, 178, 0.38);
  --border-soft: rgba(8, 145, 178, 0.12);
  --text: #334155;
  --text-strong: #0f172a;
  --text-dim: #64748b;
  --text-muted: #94a3b8;
  --primary: #0891b2;
  --primary-hover: #0e7490;
  --primary-bg: rgba(8, 145, 178, 0.1);
  --secondary: #7c3aed;
  --accent: #db2777;
  --success: #16a34a;
  --warning: #d97706;
  --danger: #dc2626;
  --glow: rgba(8, 145, 178, 0.3);
  --shadow: 0 4px 20px rgba(15, 23, 42, 0.06), 0 0 0 1px rgba(8, 145, 178, 0.1);
  --shadow-card: 0 8px 28px rgba(15, 23, 42, 0.08), 0 0 0 1px rgba(8, 145, 178, 0.08);
  --scrollbar-thumb: rgba(8, 145, 178, 0.35);
}

/* ===== Background Decorations ===== */
.bg-decor {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
  overflow: hidden;
}

.bg-grid {
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(0, 212, 255, 0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.05) 1px, transparent 1px);
  background-size: 60px 60px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  -webkit-mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  animation: grid-drift 30s linear infinite;
}

@keyframes grid-drift {
  0% { background-position: 0 0; }
  100% { background-position: 60px 60px; }
}

.bg-glow {
  position: absolute;
  border-radius: 50%;
  filter: blur(120px);
  animation: glow-float 18s ease-in-out infinite;
}

.bg-glow-1 {
  width: 700px;
  height: 700px;
  background: var(--primary);
  top: -200px;
  left: -150px;
  opacity: 0.18;
}

.bg-glow-2 {
  width: 800px;
  height: 800px;
  background: var(--secondary);
  bottom: -250px;
  right: -150px;
  opacity: 0.12;
  animation-delay: -6s;
}

@keyframes glow-float {
  0%, 100% { transform: translate(0, 0) scale(1); }
  33% { transform: translate(60px, -40px) scale(1.08); }
  66% { transform: translate(-40px, 50px) scale(0.96); }
}

/* ===== Scan Line ===== */
.scan-line {
  position: absolute;
  left: 0;
  right: 0;
  top: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  animation: scan-move 10s linear infinite;
  z-index: 1;
  pointer-events: none;
  opacity: 0.5;
}

@keyframes scan-move {
  0% { transform: translateY(-100px); opacity: 0; }
  10% { opacity: 0.5; }
  90% { opacity: 0.5; }
  100% { transform: translateY(100vh); opacity: 0; }
}

/* ===== Tech Topbar ===== */
.tech-topbar {
  position: relative;
  z-index: 5;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 3px 24px;
  background: linear-gradient(90deg, transparent, var(--primary-bg), transparent);
  border-bottom: 1px solid var(--border-soft);
}

.tech-corner {
  width: 20px;
  height: 20px;
  border: 1px solid var(--primary);
  position: relative;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.tech-corner:hover {
  box-shadow: 0 0 12px var(--glow);
}

.tech-corner-l {
  border-right: none;
  border-bottom: none;
}

.tech-corner-r {
  border-left: none;
  border-bottom: none;
}

.tc-dot {
  position: absolute;
  width: 4px;
  height: 4px;
  background: var(--primary);
  border-radius: 50%;
  box-shadow: 0 0 6px var(--primary);
  animation: dot-blink 2s ease-in-out infinite;
}

.tech-corner-l .tc-dot {
  right: -2px;
  bottom: -2px;
}

.tech-corner-r .tc-dot {
  left: -2px;
  bottom: -2px;
}

@keyframes dot-blink {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.3; transform: scale(0.6); }
}

.tech-line {
  flex: 1;
  height: 1px;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  margin: 0 12px;
  position: relative;
  overflow: hidden;
}

.tech-line .line-pulse {
  position: absolute;
  top: 0;
  height: 100%;
  width: 20%;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  animation: line-pulse-move 4s linear infinite;
}

.tech-dots {
  display: flex;
  gap: 5px;
}

.tech-dots span {
  width: 4px;
  height: 4px;
  background: var(--primary);
  border-radius: 50%;
  opacity: 0.35;
  animation: dot-blink 2.5s ease-in-out infinite;
}

.tech-dots span:nth-child(2) { animation-delay: 0.3s; }
.tech-dots span:nth-child(3) { animation-delay: 0.6s; }
.tech-dots span:nth-child(4) { animation-delay: 0.9s; }
.tech-dots span:nth-child(5) { animation-delay: 1.2s; }

@keyframes line-pulse-move {
  0% { transform: translateX(-100%); opacity: 0; }
  20% { opacity: 1; }
  80% { opacity: 1; }
  100% { transform: translateX(600%); opacity: 0; }
}

/* ===== Scrollbar ===== */
.app-root ::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}
.app-root ::-webkit-scrollbar-track {
  background: transparent;
}
.app-root ::-webkit-scrollbar-thumb {
  background: var(--scrollbar-thumb);
  border-radius: 4px;
}
.app-root ::-webkit-scrollbar-thumb:hover {
  background: var(--border-strong);
}

/* ===== Layout ===== */
.page-container {
  max-width: 1600px;
  margin: 0 auto;
  padding: 20px;
}

.page-header {
  margin-bottom: 12px;
  position: relative;
  padding: 10px 16px;
  background: linear-gradient(90deg, var(--primary-bg) 0%, transparent 70%);
  border: 1px solid var(--border-soft);
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 14px;
  overflow: hidden;
}
.page-header::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), var(--primary), transparent);
  border-radius: 8px 8px 0 0;
  opacity: 0.9;
}
.page-header::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 120px;
  height: 2px;
  background: linear-gradient(90deg, var(--primary), var(--secondary), transparent);
  border-radius: 2px;
}
.header-deco {
  display: flex;
  flex-direction: column;
  gap: 3px;
  flex-shrink: 0;
}
.header-deco-l { align-items: flex-start; }
.header-deco-r { align-items: flex-end; }
.hd-line {
  width: 30px;
  height: 2px;
  background: linear-gradient(90deg, var(--primary), transparent);
  position: relative;
}
.header-deco-r .hd-line {
  background: linear-gradient(270deg, var(--primary), transparent);
}
.hd-line::after {
  content: '';
  position: absolute;
  right: 0;
  top: -2px;
  width: 2px;
  height: 5px;
  background: var(--primary);
  box-shadow: 0 0 6px var(--primary);
}
.hd-dot {
  width: 5px;
  height: 5px;
  background: var(--primary);
  border-radius: 50%;
  box-shadow: 0 0 6px var(--primary);
  animation: dot-blink 2s ease-in-out infinite;
}
.header-deco-r .hd-dot {
  animation-delay: -0.7s;
}
.header-main {
  display: flex;
  align-items: center;
  gap: 20px;
  flex: 1;
}
.page-title {
  font-size: 20px;
  font-weight: 700;
  color: var(--text-strong);
  margin: 0;
  letter-spacing: 2px;
  position: relative;
  display: flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;
}
.title-bracket {
  color: var(--primary);
  font-weight: 400;
  opacity: 0.7;
}
.title-text {
  background: linear-gradient(180deg, #f1f5f9 0%, var(--primary) 100%);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 8px var(--glow));
}
.title-shine {
  position: absolute;
  top: 0;
  right: 0;
  width: 20px;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  animation: shine-sweep 3s ease-in-out infinite;
}
@keyframes shine-sweep {
  0% { transform: translateX(-100%); }
  50% { transform: translateX(200%); }
  100% { transform: translateX(200%); }
}
.header-stats {
  display: flex;
  align-items: center;
  gap: 12px;
}
.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1px;
}
.stat-label {
  font-size: 11px;
  color: var(--text-dim);
  letter-spacing: 0.5px;
}
.stat-value {
  font-size: 18px;
  font-weight: 700;
  color: var(--primary);
  font-family: 'Orbitron', 'Consolas', monospace;
  text-shadow: 0 0 10px var(--glow);
  letter-spacing: 1px;
}
.stat-value.stat-warn {
  color: var(--warning);
  text-shadow: 0 0 10px rgba(var(--warning), 0.4);
}
.stat-value.stat-ok {
  color: var(--success);
  text-shadow: 0 0 10px rgba(var(--success), 0.4);
}
.stat-divider {
  width: 1px;
  height: 24px;
  background: linear-gradient(180deg, transparent, var(--border-strong), transparent);
}

/* ===== Panels ===== */
.search-panel,
.action-bar,
.table-panel,
.student-basic-info,
.current-status,
.history-record {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  box-shadow: var(--shadow);
  backdrop-filter: blur(8px);
  position: relative;
  transition: box-shadow 0.4s cubic-bezier(0.4, 0, 0.2, 1), border-color 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.search-panel::before,
.table-panel::before,
.student-basic-info::before,
.current-status::before,
.history-record::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.7;
  transition: opacity 0.4s ease;
}

.search-panel:hover,
.table-panel:hover {
  box-shadow: var(--shadow-card);
  border-color: var(--border-strong);
}

.search-panel:hover::before,
.table-panel:hover::before {
  opacity: 1;
}

.search-panel::after,
.table-panel::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  border-radius: 10px;
  pointer-events: none;
  z-index: -1;
  background: linear-gradient(135deg, var(--primary) 0%, transparent 30%, transparent 70%, var(--secondary) 100%);
  opacity: 0;
  transition: opacity 0.6s cubic-bezier(0.4, 0, 0.2, 1);
  mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  -webkit-mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  mask-composite: exclude;
  -webkit-mask-composite: xor;
  padding: 1px;
}

.search-panel:hover::after,
.table-panel:hover::after {
  opacity: 0.5;
}

.search-panel {
  padding: 16px 20px;
  margin-bottom: 16px;
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  align-items: center;
}

.search-filters {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  flex: 1;
}

.filter-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.filter-label {
  font-size: 14px;
  font-weight: 500;
  color: var(--text-dim);
  white-space: nowrap;
}

.tech-input,
.tech-select {
  background: var(--bg-soft);
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 6px 12px;
  font-size: 14px;
  color: var(--text);
  outline: none;
  transition: border-color 0.2s, box-shadow 0.2s;
  min-width: 140px;
}
.tech-input:focus,
.tech-select:focus {
  border-color: var(--primary);
  box-shadow: 0 0 0 2px var(--primary-bg);
}
.tech-input::placeholder {
  color: var(--text-muted);
}

.search-actions {
  display: flex;
  gap: 12px;
  flex-shrink: 0;
}

/* ===== Buttons ===== */
.tech-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 7px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  border: 1px solid transparent;
}
.tech-btn-primary {
  background: var(--primary);
  color: #fff;
  border-color: var(--primary);
}
.tech-btn-primary:hover {
  background: var(--primary-hover);
  border-color: var(--primary-hover);
  box-shadow: 0 0 12px var(--primary-bg);
}
.tech-btn-secondary {
  background: var(--bg-card);
  color: var(--text);
  border-color: var(--border);
}
.tech-btn-secondary:hover {
  background: var(--bg-hover);
  border-color: var(--primary);
  color: var(--primary);
}
.tech-btn-danger {
  background: var(--danger);
  color: #fff;
  border-color: var(--danger);
}
.tech-btn-danger:hover {
  opacity: 0.9;
  box-shadow: 0 0 12px rgba(239, 68, 68, 0.3);
}

/* ===== Action Bar ===== */
.action-bar {
  padding: 14px 20px;
  margin-bottom: 16px;
  display: flex;
  align-items: center;
  gap: 12px;
}
.action-bar-total {
  margin-left: auto;
  font-size: 13px;
  color: var(--text-dim);
}

/* ===== Table Panel ===== */
.table-panel {
  position: relative;
  /* overflow: visible 允许 .floating-actions 溢出到面板右边缘外 */
}

.table-panel::before {
  z-index: 1;
  pointer-events: none;
}

.table-scroll {
  overflow-x: auto;
  width: 100%;
}

/* ===== Tech Table ===== */
.tech-table {
  width: 100%;
  min-width: 2300px;
  font-size: 13px;
  text-align: left;
  border-collapse: separate;
  border-spacing: 0;
}
.tech-table thead {
  background: var(--primary-bg);
  position: relative;
}
.tech-table thead th {
  padding: 10px 14px;
  font-size: 12px;
  font-weight: 600;
  color: var(--primary);
  text-transform: uppercase;
  letter-spacing: 0.5px;
  white-space: nowrap;
  border-right: 1px solid var(--border);
  position: relative;
}
.tech-table thead th::after {
  content: '';
  position: absolute;
  left: 0;
  right: 0;
  bottom: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  opacity: 0.6;
}
.tech-table thead th:last-child {
  border-right: none;
}
.tech-table tbody tr {
  border-bottom: 1px solid var(--border);
  transition: background 0.2s ease, box-shadow 0.2s ease;
  position: relative;
}
.tech-table tbody tr:last-child {
  border-bottom: none;
}
.tech-table tbody tr:hover {
  background: var(--bg-hover);
  box-shadow: inset 0 0 20px rgba(56, 189, 248, 0.08);
}
.tech-table tbody tr:hover td:first-child {
  box-shadow: inset 3px 0 0 var(--primary);
}
.tech-table tbody tr.row-alt {
  background: var(--bg-soft);
}
.tech-table tbody tr.row-alt:hover {
  background: var(--bg-hover);
  box-shadow: inset 0 0 20px rgba(56, 189, 248, 0.08);
}
.tech-table tbody td {
  padding: 10px 14px;
  color: var(--text);
  white-space: nowrap;
  border-right: 1px solid var(--border);
}
.tech-table tbody td:last-child {
  border-right: none;
}

/* ===== Column widths ===== */
.col-check { width: 48px; text-align: center; }
.col-avatar { width: 64px; text-align: center; }
.col-student-no { width: 100px; text-align: center; }
.col-grade { width: 80px; }
.col-school { min-width: 150px; }
.col-class { width: 90px; }
.col-name { width: 80px; }
.col-gender { width: 56px; text-align: center; }
.col-nation { width: 64px; text-align: center; }
.col-birthday { width: 100px; text-align: center; }
.col-idcard { min-width: 180px; text-align: center; }
.col-native { width: 80px; text-align: center; }
.col-address { min-width: 180px; }
.col-phone { width: 110px; text-align: center; }
.col-contact { width: 130px; text-align: center; }
.col-allergy { width: 80px; text-align: center; }
.col-father { width: 80px; text-align: center; }
.col-mother { width: 80px; text-align: center; }
.col-source { width: 90px; text-align: center; }
.col-intervention { width: 72px; text-align: center; }

/* ===== 浮动操作列（从表格中分离出来，钉在 .table-panel 视口最右） ===== */
.floating-actions {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  width: 80px;
  display: flex;
  flex-direction: column;
  z-index: 5; /* 盖在表格之上 */
  background: var(--bg-card);
  border-left: 1px solid var(--border);
  box-shadow: -4px 0 12px -2px rgba(0, 0, 0, 0.35);
  /* 限制在 .table-panel 内部（不超出底部分页器） */
  pointer-events: auto;
}
.floating-actions__head,
.floating-actions__row {
  flex: 0 0 auto;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 10px 14px;
  border-bottom: 1px solid var(--border);
  min-height: 44px; /* 与 .tech-table 单元格视觉对齐 */
}
.floating-actions__head {
  background: var(--primary-bg);
  color: var(--primary);
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
.floating-actions__row.row-alt {
  background: var(--bg-soft);
}
.floating-actions__row--empty {
  /* 空数据时占位，避免高度塌陷 */
  min-height: 200px;
  align-items: center;
  color: var(--text-dim);
}

/* ===== Checkbox ===== */
.tech-checkbox {
  width: 15px;
  height: 15px;
  border: 1px solid var(--border-strong);
  border-radius: 3px;
  accent-color: var(--primary);
  cursor: pointer;
  vertical-align: middle;
}

/* ===== Avatar ===== */
.avatar-circle {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--primary-bg), var(--bg-soft));
  border: 1.5px solid var(--border-strong);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto;
  font-size: 20px;
  box-shadow: 0 0 10px var(--glow);
  position: relative;
}

.avatar-circle::after {
  content: '';
  position: absolute;
  inset: -3px;
  border-radius: 50%;
  border: 1px solid var(--primary);
  opacity: 0.3;
  animation: avatar-ring 3s ease-in-out infinite;
}

.avatar-circle__img {
  width: 100%;
  height: 100%;
  border-radius: 50%;
  object-fit: cover;
  display: block;
}

@keyframes avatar-ring {
  0%, 100% { transform: scale(1); opacity: 0.3; }
  50% { transform: scale(1.15); opacity: 0; }
}

/* ===== Student No Link ===== */
.student-no-link {
  cursor: pointer;
  font-family: 'SF Mono', 'Monaco', 'Consolas', monospace;
  font-weight: 600;
  color: var(--primary);
  transition: all 0.2s ease;
  padding: 3px 8px;
  border-radius: 4px;
  position: relative;
}
.student-no-link:hover {
  color: var(--primary-hover);
  text-shadow: 0 0 8px var(--primary-bg);
  background: var(--primary-bg);
  box-shadow: 0 0 12px var(--glow);
}

/* ===== Text Helpers ===== */
.text-strong {
  color: var(--text-strong);
  font-weight: 600;
}
.text-success {
  color: var(--success);
  font-weight: 600;
  text-shadow: 0 0 8px rgba(34, 197, 94, 0.3);
}
.text-danger {
  color: var(--danger);
  font-weight: 600;
  text-shadow: 0 0 8px rgba(239, 68, 68, 0.3);
}
.mono-text {
  font-family: 'SF Mono', 'Monaco', 'Consolas', monospace;
  font-size: 12px;
}

/* ===== Tags ===== */
.tag {
  display: inline-block;
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 12px;
  border: 1px solid var(--border);
  position: relative;
  overflow: hidden;
}
.tag-default {
  background: var(--bg-soft);
  color: var(--text-dim);
}
.tag-clickable {
  cursor: pointer;
  transition: all 0.25s ease;
  user-select: none;
}
.tag-clickable:hover {
  opacity: 0.85;
  transform: translateY(-1px);
}
.tag-success {
  background: rgba(34, 197, 94, 0.15);
  color: var(--success);
  border-color: var(--success);
  box-shadow: 0 0 8px rgba(34, 197, 94, 0.2);
}
.tag-danger {
  background: rgba(239, 68, 68, 0.15);
  color: var(--danger);
  border-color: var(--danger);
  box-shadow: 0 0 8px rgba(239, 68, 68, 0.2);
}

/* ===== Action Buttons ===== */
.action-btns {
  display: flex;
  justify-content: center;
  gap: 8px;
}
.icon-btn {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid var(--border);
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 14px;
}
.icon-btn-success {
  background: rgba(14, 165, 233, 0.1);
  color: var(--primary);
  border-color: var(--border);
}
.icon-btn-success:hover {
  background: var(--primary);
  color: #fff;
  border-color: var(--primary);
  box-shadow: 0 0 8px var(--primary-bg);
}
.icon-btn-danger {
  background: rgba(239, 68, 68, 0.1);
  color: var(--danger);
  border-color: var(--border);
}
.icon-btn-danger:hover {
  background: var(--danger);
  color: #fff;
  border-color: var(--danger);
  box-shadow: 0 0 8px rgba(239, 68, 68, 0.3);
}

/* ===== Empty State ===== */
.empty-state {
  padding: 40px 20px;
  text-align: center;
}
.empty-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  color: var(--text-muted);
}
.empty-icon {
  font-size: 40px;
}
.empty-text {
  font-size: 14px;
}

/* ===== Pagination ===== */
.pagination {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  padding: 12px 16px;
  border-top: 1px solid var(--border);
  background: var(--bg-soft);
  flex-wrap: wrap;
  gap: 16px;
}
.pagination-info {
  font-size: 13px;
  color: var(--text-dim);
}
.pagination-controls {
  display: flex;
  align-items: center;
  gap: 6px;
}
.page-btn {
  padding: 5px 12px;
  border: 1px solid var(--border);
  border-radius: 6px;
  background: var(--bg-card);
  color: var(--text-dim);
  font-size: 13px;
  cursor: pointer;
  transition: all 0.2s ease;
}
.page-btn:hover:not(:disabled) {
  background: var(--primary-bg);
  border-color: var(--primary);
  color: var(--primary);
}
.page-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}
.page-btn-active {
  background: var(--primary);
  color: #fff;
  border-color: var(--primary);
}
.page-btn-active:hover {
  background: var(--primary-hover);
  border-color: var(--primary-hover);
}
.page-btn-icon {
  padding: 5px 10px;
  min-width: 32px;
}
.page-btn-chevron {
  display: inline-block;
  font-size: 18px;
  line-height: 1;
  font-weight: 500;
}
.page-ellipsis {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 32px;
  height: 30px;
  font-size: 14px;
  color: var(--text-muted);
  user-select: none;
  letter-spacing: 1px;
}
.pagination-sizes {
  display: flex;
  align-items: center;
}
.pagination-size-select {
  padding: 4px 10px;
  font-size: 13px;
  height: 30px;
  border-radius: 6px;
}
.pagination-jumper {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  color: var(--text-dim);
}
.pagination-jumper__input {
  width: 56px;
  height: 30px;
  padding: 4px 8px;
  font-size: 13px;
  text-align: center;
  border-radius: 6px;
}
/* 隐藏 number input 的箭头（更接近 Element Plus 风格） */
.pagination-jumper__input::-webkit-inner-spin-button,
.pagination-jumper__input::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
.pagination-jumper__input[type='number'] {
  -moz-appearance: textfield;
}

/* ===== Form Grid (Modal) ===== */
.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 14px 20px;
}
.form-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.form-item-full {
  grid-column: 1 / -1;
  flex-direction: row;
  align-items: center;
  gap: 10px;
}
.form-label {
  font-size: 13px;
  font-weight: 500;
  color: var(--text);
}
.checkbox-label {
  font-size: 14px;
  color: var(--text);
}

/* ===== Avatar Uploader (Modal) ===== */
.avatar-uploader {
  display: flex;
  align-items: center;
  gap: 16px;
  padding: 12px 14px;
  margin-bottom: 16px;
  background: var(--bg-soft);
  border: 1px dashed var(--border);
  border-radius: 8px;
}
.avatar-uploader__label {
  font-size: 13px;
  font-weight: 500;
  color: var(--text);
  flex-shrink: 0;
}
.avatar-uploader__main {
  display: flex;
  align-items: center;
  gap: 14px;
  flex: 1;
}
.avatar-uploader__preview {
  width: 64px;
  height: 64px;
  border-radius: 50%;
  background: var(--bg-soft);
  border: 1.5px dashed var(--border-strong);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  overflow: hidden;
  position: relative;
  transition: all 0.2s ease;
  flex-shrink: 0;
}
.avatar-uploader__preview:hover {
  border-color: var(--primary);
  background: var(--bg-hover);
  transform: scale(1.04);
}
.avatar-uploader__img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.avatar-uploader__emoji {
  font-size: 36px;
  line-height: 1;
}
.avatar-uploader__plus {
  font-size: 32px;
  font-weight: 300;
  color: var(--primary);
  line-height: 1;
  user-select: none;
}
.avatar-uploader__actions {
  display: flex;
  gap: 8px;
}
.avatar-uploader__btn {
  padding: 6px 14px;
  font-size: 13px;
  color: var(--text);
  background: var(--primary-bg);
  border: 1px solid var(--border);
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s ease;
}
.avatar-uploader__btn:hover {
  background: var(--bg-hover);
  border-color: var(--primary);
  color: var(--primary);
}
.avatar-uploader__btn--ghost {
  background: transparent;
  color: var(--text-dim);
}
.avatar-uploader__btn--ghost:hover {
  color: var(--danger);
  border-color: var(--danger);
}

/* ===== Student Detail Modal ===== */
.student-detail-content {
  padding: 4px 0;
}

.student-basic-info {
  padding: 16px 20px;
  margin-bottom: 16px;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px 24px;
}
.info-item {
  font-size: 14px;
  color: var(--text-dim);
  display: flex;
  gap: 4px;
}
.info-item label {
  color: var(--text-muted);
  font-weight: 500;
  white-space: nowrap;
}
.info-value {
  color: var(--text);
}

/* ===== Detail Tabs ===== */
.detail-tabs {
  display: flex;
  gap: 4px;
  border-bottom: 1px solid var(--border);
  margin-bottom: 16px;
  flex-wrap: wrap;
}
.detail-tab {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 10px 20px;
  border-radius: 8px 8px 0 0;
  cursor: pointer;
  transition: all 0.25s ease;
  color: var(--text-dim);
  font-size: 14px;
  font-weight: 500;
  border-bottom: 2px solid transparent;
}
.detail-tab:hover {
  background: var(--bg-hover);
  color: var(--primary);
}
.detail-tab.active {
  background: var(--primary-bg);
  color: var(--primary);
  border-bottom-color: var(--primary);
}
.tab-icon {
  font-size: 16px;
}

/* ===== Detail Panel ===== */
.detail-panel {
  animation: fadeIn 0.25s ease;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(6px); }
  to { opacity: 1; transform: translateY(0); }
}

.current-status {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  padding: 16px 20px;
  margin-bottom: 16px;
}

.status-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.status-label {
  font-size: 12px;
  color: var(--text-muted);
}
.status-value {
  font-size: 20px;
  font-weight: 700;
  color: var(--text-strong);
  letter-spacing: 1px;
}
.level-tag {
  display: inline-block;
  padding: 2px 12px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 600;
}
.level-normal { background: rgba(34, 197, 94, 0.15); color: var(--success); border: 1px solid rgba(34, 197, 94, 0.3); }
.level-low { background: rgba(245, 158, 11, 0.15); color: var(--warning); border: 1px solid rgba(245, 158, 11, 0.3); }
.level-mid { background: rgba(249, 115, 22, 0.15); color: #f97316; border: 1px solid rgba(249, 115, 22, 0.3); }
.level-high { background: rgba(239, 68, 68, 0.15); color: var(--danger); border: 1px solid rgba(239, 68, 68, 0.3); }
.status-good { color: var(--success); font-weight: 600; }
.status-warning { color: var(--warning); font-weight: 600; }
.status-danger { color: var(--danger); font-weight: 600; }

/* ===== History ===== */
.history-record {
  padding: 14px 18px;
}

.history-title {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-strong);
  margin: 0 0 10px 0;
}
.history-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
}
.history-table th {
  background: var(--primary-bg);
  padding: 8px 12px;
  text-align: left;
  font-weight: 600;
  color: var(--primary);
  border-bottom: 1px solid var(--border);
}
.history-table td {
  padding: 8px 12px;
  border-bottom: 1px solid var(--border);
  color: var(--text);
}
.history-table tr:hover td {
  background: var(--bg-hover);
}
.history-table .empty-row td {
  text-align: center;
  padding: 20px 12px;
  color: var(--text-dim);
  font-style: italic;
}

/* ===== Ant Design Modal Overrides ===== */
:deep(.ant-modal-root) {
  z-index: 1000;
}
:deep(.ant-modal-mask) {
  background: rgba(0, 0, 0, 0.55) !important;
  backdrop-filter: blur(4px);
}
:deep(.ant-modal) {
  border-radius: 12px;
  overflow: hidden;
}
:deep(.ant-modal-content) {
  background: var(--bg-card) !important;
  border: 1px solid var(--border);
  border-radius: 12px;
  box-shadow: 0 0 30px var(--primary-bg), var(--shadow);
  backdrop-filter: blur(12px);
}
:deep(.ant-modal-header) {
  background: transparent !important;
  border-bottom: 1px solid var(--border) !important;
  padding: 16px 24px;
}
:deep(.ant-modal-title) {
  color: var(--text-strong) !important;
  font-size: 18px;
  font-weight: 700;
}
:deep(.ant-modal-close) {
  color: var(--text-dim) !important;
}
:deep(.ant-modal-close:hover) {
  color: var(--primary) !important;
}
:deep(.ant-modal-close-x) {
  color: var(--text-dim) !important;
  width: 36px;
  height: 36px;
  line-height: 36px;
}
:deep(.ant-modal-close-x:hover) {
  color: var(--primary) !important;
  background: var(--primary-bg) !important;
  border-radius: 6px;
}
:deep(.ant-modal-body) {
  padding: 16px 24px 24px;
  max-height: 600px;
  overflow-y: auto;
  color: var(--text);
}
:deep(.ant-modal-footer) {
  border-top: 1px solid var(--border) !important;
  padding: 12px 24px;
  background: transparent;
}
:deep(.ant-btn-primary) {
  background: var(--primary) !important;
  border-color: var(--primary) !important;
  color: #fff !important;
  box-shadow: 0 0 12px var(--primary-bg);
}
:deep(.ant-btn-primary:hover) {
  background: var(--primary-hover) !important;
  border-color: var(--primary-hover) !important;
}
:deep(.ant-btn) {
  background: var(--bg-card);
  border-color: var(--border);
  color: var(--text);
  border-radius: 6px;
}
:deep(.ant-btn:hover) {
  border-color: var(--primary);
  color: var(--primary);
  background: var(--primary-bg);
}
:deep(.ant-input),
:deep(.ant-select-selector) {
  background: var(--bg-soft) !important;
  border-color: var(--border) !important;
  color: var(--text) !important;
  border-radius: 6px;
}
:deep(.ant-input-affix-wrapper) {
  background: var(--bg-soft) !important;
  border-color: var(--border) !important;
}
:deep(.ant-input:hover),
:deep(.ant-select-selector:hover) {
  border-color: var(--primary) !important;
}
:deep(.ant-input:focus),
:deep(.ant-select-focused .ant-select-selector) {
  border-color: var(--primary) !important;
  box-shadow: 0 0 0 2px var(--primary-bg) !important;
}
:deep(.ant-checkbox-inner) {
  background: var(--bg-soft);
  border-color: var(--border-strong);
}
:deep(.ant-checkbox-checked .ant-checkbox-inner) {
  background: var(--primary);
  border-color: var(--primary);
}
:deep(.ant-pagination) {
  color: var(--text-dim);
}
:deep(.ant-pagination-item) {
  background: transparent;
  border-color: var(--border);
}
:deep(.ant-pagination-item a) {
  color: var(--text-dim);
}
:deep(.ant-pagination-item-active) {
  border-color: var(--primary);
}
:deep(.ant-pagination-item-active a) {
  color: var(--primary);
}
:deep(.ant-pagination-item:hover a) {
  color: var(--primary);
}
:deep(.ant-pagination-prev .ant-pagination-item-link),
:deep(.ant-pagination-next .ant-pagination-item-link) {
  background: transparent;
  border-color: var(--border);
  color: var(--text-dim);
}
:deep(.ant-pagination-prev:hover .ant-pagination-item-link),
:deep(.ant-pagination-next:hover .ant-pagination-item-link) {
  border-color: var(--primary);
  color: var(--primary);
}
:deep(.ant-select-dropdown) {
  background: var(--bg-card) !important;
  border-color: var(--border);
  border-radius: 8px;
  box-shadow: 0 8px 24px rgba(0,0,0,0.3);
}
:deep(.ant-select-item) {
  color: var(--text);
}
:deep(.ant-select-item-option-active) {
  background: var(--primary-bg);
}
:deep(.ant-select-item-option-selected) {
  color: var(--primary);
  font-weight: 600;
}
:deep(.ant-message) {
  background: var(--bg-card) !important;
  border: 1px solid var(--border);
  border-radius: 8px;
  box-shadow: 0 8px 24px rgba(0,0,0,0.3);
  backdrop-filter: blur(8px);
}
:deep(.ant-message-notice-content) {
  background: transparent !important;
  color: var(--text);
}
:deep(.ant-message-success .anticon),
:deep(.ant-message-warning .anticon),
:deep(.ant-message-error .anticon),
:deep(.ant-message-info .anticon) {
  color: var(--primary);
}
:deep(.ant-table) {
  background: transparent;
}
:deep(.ant-table-thead > tr > th) {
  background: var(--primary-bg) !important;
  color: var(--text-strong) !important;
  border-bottom: 1px solid var(--border);
}
:deep(.ant-table-tbody > tr > td) {
  border-bottom: 1px solid var(--border);
  color: var(--text);
  background: transparent;
}
:deep(.ant-table-tbody > tr:hover > td) {
  background: var(--bg-hover);
}
:deep(.ant-table-placeholder) {
  background: transparent;
  color: var(--text-muted);
}

/* ===== Responsive ===== */
@media (max-width: 1200px) {
  .info-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  .current-status {
    grid-template-columns: repeat(2, 1fr);
  }
  .header-main {
    flex-wrap: wrap;
    gap: 10px;
  }
  .header-stats {
    gap: 8px;
  }
  .stat-value {
    font-size: 16px;
  }
}

@media (max-width: 768px) {
  .page-container {
    padding: 12px;
  }
  .tech-topbar {
    padding: 4px 12px;
  }
  .tech-corner {
    width: 18px;
    height: 18px;
  }
  .page-header {
    padding: 8px 12px;
    gap: 8px;
  }
  .page-title {
    font-size: 17px;
    letter-spacing: 1px;
  }
  .header-deco {
    display: none;
  }
  .header-main {
    flex-wrap: wrap;
    gap: 6px;
  }
  .header-stats {
    gap: 6px;
    width: 100%;
  }
  .stat-value {
    font-size: 14px;
  }
  .stat-label {
    font-size: 10px;
  }
  .search-panel {
    flex-direction: column;
    align-items: stretch;
  }
  .search-filters {
    flex-direction: column;
    gap: 12px;
  }
  .filter-item {
    flex-wrap: wrap;
  }
  .tech-input,
  .tech-select {
    width: 100%;
    min-width: 0;
  }
  .search-actions {
    justify-content: flex-end;
  }
  .action-bar {
    flex-wrap: wrap;
  }
  .action-bar-total {
    margin-left: 0;
    width: 100%;
  }
  .info-grid {
    grid-template-columns: 1fr;
  }
  .current-status {
    grid-template-columns: 1fr;
  }
  .detail-tab {
    padding: 8px 12px;
    font-size: 13px;
  }
  .form-grid {
    grid-template-columns: 1fr;
  }
  .pagination {
    flex-direction: column;
    align-items: flex-start;
  }
  :deep(.ant-modal) {
    max-width: 100% !important;
    margin: 10px;
  }
}

/* ===== Light Mode Specific Overrides ===== */
.app-root.theme-light .scan-line {
  opacity: 0.12;
  box-shadow: none;
  height: 1px;
}

.app-root.theme-light .bg-glow {
  opacity: 0.1;
  filter: blur(80px);
}

.app-root.theme-light .bg-grid {
  opacity: 0.4;
  background-image:
    linear-gradient(rgba(8, 145, 178, 0.06) 1px, transparent 1px),
    linear-gradient(90deg, rgba(8, 145, 178, 0.06) 1px, transparent 1px);
}

.app-root.theme-light .tech-topbar {
  background: linear-gradient(90deg, transparent, rgba(8, 145, 178, 0.04), transparent);
  border-bottom-color: rgba(8, 145, 178, 0.2);
}

.app-root.theme-light .tech-corner {
  border-color: rgba(8, 145, 178, 0.55);
  box-shadow: 0 0 6px rgba(8, 145, 178, 0.2);
}

.app-root.theme-light .tc-dot {
  background: #0891b2;
  box-shadow: 0 0 6px rgba(8, 145, 178, 0.35);
}

.app-root.theme-light .tech-line {
  background: linear-gradient(90deg, transparent, rgba(8, 145, 178, 0.5), transparent);
}

.app-root.theme-light .tech-dots span {
  background: #0891b2;
  opacity: 0.45;
}

.app-root.theme-light .line-pulse {
  background: linear-gradient(90deg, transparent, rgba(8, 145, 178, 0.6), transparent);
}

.app-root.theme-light .page-header {
  background: linear-gradient(90deg, rgba(8, 145, 178, 0.08), transparent 65%);
  border-color: rgba(8, 145, 178, 0.2);
}

.app-root.theme-light .page-header::before {
  opacity: 0.8;
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, #0891b2, transparent);
}

.app-root.theme-light .page-header::after {
  background: linear-gradient(90deg, #0891b2, #7c3aed, transparent);
}

.app-root.theme-light .stat-value {
  color: #0891b2;
  text-shadow: 0 0 8px rgba(8, 145, 178, 0.3);
}

.app-root.theme-light .stat-value.stat-warn {
  color: #d97706;
  text-shadow: 0 0 8px rgba(217, 119, 6, 0.3);
}

.app-root.theme-light .stat-value.stat-ok {
  color: #16a34a;
  text-shadow: 0 0 8px rgba(22, 163, 74, 0.3);
}

.app-root.theme-light .stat-divider {
  background: linear-gradient(180deg, transparent, rgba(8, 145, 178, 0.3), transparent);
}

.app-root.theme-light .hd-line {
  background: linear-gradient(90deg, #0891b2, transparent);
}

.app-root.theme-light .hd-line::after {
  background: #0891b2;
  box-shadow: 0 0 4px rgba(8, 145, 178, 0.45);
}

.app-root.theme-light .hd-dot {
  background: #0891b2;
  box-shadow: 0 0 6px rgba(8, 145, 178, 0.45);
}

.app-root.theme-light .title-text {
  background: linear-gradient(180deg, #1e293b 0%, #0891b2 100%);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 10px rgba(8, 145, 178, 0.18));
}

.app-root.theme-light .title-bracket {
  color: #0891b2;
  opacity: 0.6;
}

.app-root.theme-light .title-shine {
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.7), transparent);
}

.app-root.theme-light .search-panel,
.app-root.theme-light .action-bar,
.app-root.theme-light .table-panel,
.app-root.theme-light .student-basic-info,
.app-root.theme-light .current-status,
.app-root.theme-light .history-record {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  box-shadow: 0 2px 8px rgba(15, 23, 42, 0.05), 0 1px 2px rgba(15, 23, 42, 0.04);
  border-color: rgba(8, 145, 178, 0.22);
  backdrop-filter: none;
}

.app-root.theme-light .search-panel::before,
.app-root.theme-light .table-panel::before,
.app-root.theme-light .student-basic-info::before,
.app-root.theme-light .current-status::before,
.app-root.theme-light .history-record::before {
  opacity: 0.65;
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, transparent);
}

.app-root.theme-light .search-panel:hover::after,
.app-root.theme-light .table-panel:hover::after {
  opacity: 0.45;
}

.app-root.theme-light .text-shadow-strong {
  text-shadow: none;
}

.app-root.theme-light .avatar-circle {
  background: linear-gradient(135deg, rgba(8, 145, 178, 0.08), rgba(8, 145, 178, 0.03));
  border-color: rgba(8, 145, 178, 0.35);
  box-shadow: 0 0 8px rgba(8, 145, 178, 0.15);
}

.app-root.theme-light .avatar-circle::after {
  border-color: rgba(8, 145, 178, 0.4);
}

.app-root.theme-light .tech-table tbody tr:hover {
  box-shadow: inset 0 0 16px rgba(8, 145, 178, 0.06);
}

.app-root.theme-light .tech-table tbody tr:hover td:first-child {
  box-shadow: inset 3px 0 0 #0891b2;
}

.app-root.theme-light .tech-table thead th::after {
  background: linear-gradient(90deg, transparent, #0891b2, transparent);
  opacity: 0.5;
}

.app-root.theme-light .tag-success {
  box-shadow: 0 0 6px rgba(22, 163, 74, 0.25);
}

.app-root.theme-light .tag-danger {
  box-shadow: 0 0 6px rgba(220, 38, 38, 0.25);
}

.app-root.theme-light .tech-btn-primary {
  background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
  border-color: #0891b2;
  color: #fff;
  box-shadow: 0 2px 8px rgba(8, 145, 178, 0.3);
}

.app-root.theme-light .tech-btn-primary:hover {
  background: linear-gradient(135deg, #0e7490 0%, #155e75 100%);
  border-color: #0e7490;
  box-shadow: 0 4px 12px rgba(8, 145, 178, 0.4);
}

.app-root.theme-light .tech-btn-secondary {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.25);
  color: #0891b2;
}

.app-root.theme-light .tech-btn-secondary:hover {
  border-color: #0891b2;
  color: #0891b2;
  background: rgba(8, 145, 178, 0.06);
  box-shadow: 0 2px 8px rgba(8, 145, 178, 0.15);
}

.app-root.theme-light .tech-btn-danger {
  background: #fff;
  border-color: #fca5a5;
  color: #dc2626;
}

.app-root.theme-light .tech-btn-danger:hover {
  background: #fef2f2;
  border-color: #dc2626;
  color: #dc2626;
}

.app-root.theme-light .icon-btn {
  background: #fff;
  border-color: #e2e8f0;
  color: #64748b;
}

.app-root.theme-light .icon-btn-success {
  background: rgba(8, 145, 178, 0.06);
  color: #0891b2;
  border-color: rgba(8, 145, 178, 0.2);
}

.app-root.theme-light .icon-btn-success:hover {
  background: #0891b2;
  color: #fff;
  border-color: #0891b2;
  box-shadow: none;
}

.app-root.theme-light .icon-btn-danger {
  background: rgba(220, 38, 38, 0.06);
  color: #dc2626;
  border-color: rgba(220, 38, 38, 0.2);
}

.app-root.theme-light .icon-btn-danger:hover {
  background: #dc2626;
  color: #fff;
  border-color: #dc2626;
  box-shadow: none;
}

.app-root.theme-light .tag {
  border-radius: 4px;
}

.app-root.theme-light .tag-success {
  background: rgba(22, 163, 74, 0.08);
  color: #16a34a;
  border-color: rgba(22, 163, 74, 0.2);
}

.app-root.theme-light .tag-danger {
  background: rgba(220, 38, 38, 0.08);
  color: #dc2626;
  border-color: rgba(220, 38, 38, 0.2);
}

.app-root.theme-light .tag-default {
  background: #f8fafc;
  color: #64748b;
  border-color: #e2e8f0;
}

.app-root.theme-light .student-no-link {
  color: #0891b2;
}

.app-root.theme-light .student-no-link:hover {
  color: #0e7490;
  text-shadow: none;
  text-decoration: underline;
}

.app-root.theme-light .tech-input,
.app-root.theme-light .tech-select {
  background: #fff;
  border-color: #e2e8f0;
  color: #1e293b;
}

.app-root.theme-light .tech-input::placeholder {
  color: #94a3b8;
}

.app-root.theme-light .tech-input:focus,
.app-root.theme-light .tech-select:focus {
  border-color: #0891b2;
  box-shadow: 0 0 0 3px rgba(8, 145, 178, 0.1);
}

.app-root.theme-light .avatar-circle {
  background: #f1f5f9;
  border-color: #e2e8f0;
}

.app-root.theme-light .tech-table {
  border-radius: 8px;
  overflow: hidden;
}

.app-root.theme-light .tech-table thead {
  background: linear-gradient(180deg, #f0f9ff 0%, #e0f2fe 100%);
}

.app-root.theme-light .tech-table thead th {
  color: #0891b2;
  font-weight: 600;
  font-size: 12px;
  letter-spacing: 0.5px;
  border-right-color: rgba(8, 145, 178, 0.12);
}

.app-root.theme-light .tech-table tbody tr {
  background: #ffffff;
  border-bottom-color: rgba(8, 145, 178, 0.08);
}

.app-root.theme-light .tech-table tbody tr:hover {
  background: rgba(8, 145, 178, 0.05);
}

.app-root.theme-light .tech-table tbody tr.row-alt {
  background: linear-gradient(180deg, #fafcff 0%, #f5f9ff 100%);
}

.app-root.theme-light .tech-table tbody tr.row-alt:hover {
  background: rgba(8, 145, 178, 0.05);
}

.app-root.theme-light .tech-table tbody td {
  color: #334155;
  border-right-color: rgba(8, 145, 178, 0.06);
}

/* ===== 亮色模式：浮动操作列 ===== */
.app-root.theme-light .floating-actions {
  background: #ffffff;
  border-left-color: rgba(8, 145, 178, 0.12);
  box-shadow: -4px 0 12px -2px rgba(8, 145, 178, 0.18);
}
.app-root.theme-light .floating-actions__head {
  background: linear-gradient(180deg, #f0f9ff 0%, #e0f2fe 100%);
  color: #0891b2;
}
.app-root.theme-light .floating-actions__row.row-alt {
  background: linear-gradient(180deg, #fafcff 0%, #f5f9ff 100%);
}

.app-root.theme-light .tech-checkbox {
  border-color: rgba(8, 145, 178, 0.3);
  background: #fff;
}

.app-root.theme-light .page-btn {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.2);
  color: #64748b;
}

.app-root.theme-light .page-btn:hover:not(:disabled) {
  background: rgba(8, 145, 178, 0.08);
  border-color: #0891b2;
  color: #0891b2;
  box-shadow: 0 2px 6px rgba(8, 145, 178, 0.2);
}

.app-root.theme-light .page-btn-active {
  background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
  border-color: #0891b2;
  color: #fff;
}

.app-root.theme-light .page-btn-active:hover {
  background: #0e7490;
  border-color: #0e7490;
}

.app-root.theme-light .pagination {
  border-top-color: #e2e8f0;
  background: #fff;
}
.app-root.theme-light .page-ellipsis {
  color: #94a3b8;
}

.app-root.theme-light .level-normal {
  background: rgba(22, 163, 74, 0.08);
  color: #16a34a;
  border-color: rgba(22, 163, 74, 0.2);
}
.app-root.theme-light .level-low {
  background: rgba(217, 119, 6, 0.08);
  color: #b45309;
  border-color: rgba(217, 119, 6, 0.2);
}
.app-root.theme-light .level-mid {
  background: rgba(234, 88, 12, 0.08);
  color: #c2410c;
  border-color: rgba(234, 88, 12, 0.2);
}
.app-root.theme-light .level-high {
  background: rgba(220, 38, 38, 0.08);
  color: #dc2626;
  border-color: rgba(220, 38, 38, 0.2);
}

.app-root.theme-light .detail-tab {
  color: #64748b;
}

.app-root.theme-light .detail-tab:hover {
  background: rgba(8, 145, 178, 0.06);
  color: #0891b2;
}

.app-root.theme-light .detail-tab.active {
  background: rgba(8, 145, 178, 0.08);
  color: #0891b2;
  border-bottom-color: #0891b2;
  box-shadow: none;
}

.app-root.theme-light .detail-tabs {
  border-bottom-color: #e2e8f0;
}

.app-root.theme-light .status-good { color: #16a34a; }
.app-root.theme-light .status-warning { color: #b45309; }
.app-root.theme-light .status-danger { color: #dc2626; }

.app-root.theme-light .status-value {
  color: #0f172a;
}

.app-root.theme-light .status-label {
  color: #64748b;
}

.app-root.theme-light .history-table th {
  background: #f8fafc;
  color: #475569;
  border-bottom-color: #e2e8f0;
}

.app-root.theme-light .history-table td {
  border-bottom-color: #f1f5f9;
  color: #334155;
}

.app-root.theme-light .history-table tr:hover td {
  background: rgba(8, 145, 178, 0.04);
}

.app-root.theme-light .info-item label {
  color: #94a3b8;
}

.app-root.theme-light .info-value {
  color: #1e293b;
}

.app-root.theme-light .text-strong {
  color: #0f172a;
}

.app-root.theme-light .text-success {
  color: #16a34a;
}

.app-root.theme-light .text-danger {
  color: #dc2626;
}

.app-root.theme-light .filter-label {
  color: #475569;
}

.app-root.theme-light :deep(.ant-modal-mask) {
  background: rgba(15, 23, 42, 0.45) !important;
}

.app-root.theme-light :deep(.ant-modal-content) {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%) !important;
  border: 1px solid rgba(8, 145, 178, 0.2) !important;
  box-shadow: 0 20px 60px rgba(15, 23, 42, 0.12), 0 0 0 1px rgba(8, 145, 178, 0.08) !important;
  backdrop-filter: none;
}

.app-root.theme-light :deep(.ant-modal-header) {
  border-bottom-color: rgba(8, 145, 178, 0.12) !important;
  background: linear-gradient(180deg, #f0f9ff 0%, #ffffff 100%);
}

.app-root.theme-light :deep(.ant-modal-title) {
  color: #0f172a !important;
}

.app-root.theme-light :deep(.ant-modal-body) {
  color: #334155;
}

.app-root.theme-light :deep(.ant-modal-footer) {
  border-top-color: rgba(8, 145, 178, 0.12) !important;
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
}

.app-root.theme-light :deep(.ant-btn) {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.2);
  color: #475569;
}

.app-root.theme-light :deep(.ant-btn:hover) {
  border-color: #0891b2;
  color: #0891b2;
  box-shadow: 0 2px 6px rgba(8, 145, 178, 0.15);
}

.app-root.theme-light :deep(.ant-btn-primary) {
  background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
  border-color: #0891b2;
  color: #fff;
  box-shadow: 0 2px 8px rgba(8, 145, 178, 0.25);
}

.app-root.theme-light :deep(.ant-btn-primary:hover) {
  background: linear-gradient(135deg, #0e7490 0%, #155e75 100%);
  border-color: #0e7490;
  box-shadow: 0 4px 12px rgba(8, 145, 178, 0.35);
}

.app-root.theme-light :deep(.ant-btn-primary) {
  background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%) !important;
  border-color: #0891b2 !important;
  color: #fff !important;
  box-shadow: 0 2px 8px rgba(8, 145, 178, 0.25);
}

.app-root.theme-light :deep(.ant-input),
.app-root.theme-light :deep(.ant-select-selector) {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%) !important;
  border-color: rgba(8, 145, 178, 0.2) !important;
  color: #1e293b !important;
}

.app-root.theme-light :deep(.ant-input-affix-wrapper) {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%) !important;
  border-color: rgba(8, 145, 178, 0.2) !important;
}

.app-root.theme-light :deep(.ant-input:hover),
.app-root.theme-light :deep(.ant-select-selector:hover) {
  border-color: #0891b2 !important;
  box-shadow: 0 2px 6px rgba(8, 145, 178, 0.15);
}

.app-root.theme-light :deep(.ant-input:focus),
.app-root.theme-light :deep(.ant-select-focused .ant-select-selector) {
  border-color: #0891b2 !important;
  box-shadow: 0 0 0 3px rgba(8, 145, 178, 0.12) !important;
}

.app-root.theme-light :deep(.ant-checkbox-inner) {
  background: #fff;
  border-color: rgba(8, 145, 178, 0.35);
}

.app-root.theme-light :deep(.ant-checkbox-checked .ant-checkbox-inner) {
  background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
  border-color: #0891b2;
}

.app-root.theme-light :deep(.ant-pagination-item) {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.2);
}

.app-root.theme-light :deep(.ant-pagination-item a) {
  color: #64748b;
}

.app-root.theme-light :deep(.ant-pagination-item-active) {
  border-color: #0891b2;
  background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
}

.app-root.theme-light :deep(.ant-pagination-item-active a) {
  color: #fff;
}

.app-root.theme-light :deep(.ant-pagination-prev .ant-pagination-item-link),
.app-root.theme-light :deep(.ant-pagination-next .ant-pagination-item-link) {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.2);
  color: #64748b;
}

.app-root.theme-light :deep(.ant-pagination-prev:hover .ant-pagination-item-link),
.app-root.theme-light :deep(.ant-pagination-next:hover .ant-pagination-item-link) {
  border-color: #0891b2;
  color: #0891b2;
  box-shadow: 0 2px 6px rgba(8, 145, 178, 0.15);
}

.app-root.theme-light :deep(.ant-select-dropdown) {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%) !important;
  border-color: rgba(8, 145, 178, 0.18);
  box-shadow: 0 8px 24px rgba(15, 23, 42, 0.12);
}

.app-root.theme-light :deep(.ant-select-item) {
  color: #334155;
}

.app-root.theme-light :deep(.ant-select-item-option-active) {
  background: rgba(8, 145, 178, 0.08);
}

.app-root.theme-light :deep(.ant-select-item-option-selected) {
  color: #0891b2;
  font-weight: 600;
}

.app-root.theme-light :deep(.ant-message) {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%) !important;
  border: 1px solid rgba(8, 145, 178, 0.18);
  box-shadow: 0 8px 24px rgba(15, 23, 42, 0.12) !important;
  backdrop-filter: none;
}

.app-root.theme-light :deep(.ant-message-notice-content) {
  background: transparent !important;
  color: #334155;
}

.app-root.theme-light :deep(.ant-message-success .anticon) {
  color: #16a34a;
}
.app-root.theme-light :deep(.ant-message-warning .anticon) {
  color: #b45309;
}
.app-root.theme-light :deep(.ant-message-error .anticon) {
  color: #dc2626;
}
.app-root.theme-light :deep(.ant-message-info .anticon) {
  color: #0891b2;
}

.app-root.theme-light :deep(.ant-table-thead > tr > th) {
  background: linear-gradient(180deg, #f0f9ff 0%, #e0f2fe 100%) !important;
  color: #0891b2 !important;
  border-bottom-color: rgba(8, 145, 178, 0.12);
}

.app-root.theme-light :deep(.ant-table-tbody > tr > td) {
  border-bottom-color: rgba(8, 145, 178, 0.08);
  color: #334155;
  background: #fff;
}

.app-root.theme-light :deep(.ant-table-tbody > tr:hover > td) {
  background: rgba(8, 145, 178, 0.05);
}

.app-root.theme-light :deep(.ant-table-placeholder) {
  background: #fff;
  color: #94a3b8;
}

.app-root.theme-light :deep(.ant-table) {
  background: #fff;
}

.app-root.theme-light ::-webkit-scrollbar-thumb {
  background: linear-gradient(180deg, rgba(8, 145, 178, 0.4) 0%, rgba(8, 145, 178, 0.25) 100%);
  border-radius: 4px;
}

.app-root.theme-light ::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(180deg, rgba(8, 145, 178, 0.55) 0%, rgba(8, 145, 178, 0.4) 100%);
}
</style>
