<template>
  <div class="dashboard-wrapper" :class="{ 'theme-light': isLight }">
    <!-- 背景装饰 -->
    <div class="bg-decor" aria-hidden="true">
      <div class="bg-grid"></div>
      <div class="bg-glow bg-glow-1"></div>
      <div class="bg-glow bg-glow-2"></div>
    </div>

    <!-- 扫描线 -->
    <div class="scan-line"></div>

    <!-- ================= 页面标题 ================= -->
    <div class="page-header">
      <div class="header-deco header-deco-l">
        <span class="hd-line"></span>
        <span class="hd-dot"></span>
      </div>
      <div class="header-main">
        <h1 class="page-title">
          <span class="title-bracket">【</span>
          <span class="title-text">核心可视化图表</span>
          <span class="title-bracket">】</span>
          <span class="title-shine"></span>
        </h1>
      </div>
      <div class="header-deco header-deco-r">
        <span class="hd-dot"></span>
        <span class="hd-line"></span>
      </div>
    </div>

    <!-- ================= Tab切换栏 ================= -->
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

    <!-- ================= Tab内容区 ================= -->
    <div class="tab-content">
      <!-- ===== 视力 Tab ===== -->
      <div v-show="activeTab === 'vision'" class="tab-panel">
        <div class="filter-bar">
          <div class="filter-row">
            <div class="filter-item">
              <label>年份：</label>
              <div class="select-box">
                <select v-model="visionFilters.year">
                  <option value="2026">2026年</option>
                  <option value="2025">2025年</option>
                  <option value="2024">2024年</option>
                  <option value="2023">2023年</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>学期：</label>
              <div class="select-box">
                <select v-model="visionFilters.semester">
                  <option value="all">全部学期</option>
                  <option value="spring">春季学期</option>
                  <option value="autumn">秋季学期</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>区域：</label>
              <div class="select-box">
                <select v-model="visionFilters.region">
                  <option value="全国">全国</option>
                  <option v-for="p in provinceList" :key="p" :value="p">{{ p }}</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="btn-group">
              <button class="btn-search" @click="handleVisionSearch">
                <span class="icon">🔍</span> 分析
              </button>
              <button class="btn-reset" @click="handleVisionReset">
                <span class="icon">↺</span> 重置
              </button>
            </div>
          </div>
        </div>

        <div class="charts-grid">
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">📈 近视率年度/学期变化趋势</h3>
              <div ref="trendChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">📊 学段/年级近视率对比</h3>
              <div ref="gradeChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">👫 性别近视率对比</h3>
              <div ref="genderChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🍩 视力分档占比分布</h3>
              <div ref="pieChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">🗺️ 区域近视率热力分布</h3>
              <div ref="mapChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🏆 学校/班级近视率排行榜</h3>
              <div ref="rankingChartRef" class="chart-container"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 口腔 Tab ===== -->
      <div v-show="activeTab === 'oral'" class="tab-panel">
        <div class="filter-bar">
          <div class="filter-row">
            <div class="filter-item">
              <label>年份：</label>
              <div class="select-box">
                <select v-model="oralFilters.year">
                  <option value="2026">2026年</option>
                  <option value="2025">2025年</option>
                  <option value="2024">2024年</option>
                  <option value="2023">2023年</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>学期：</label>
              <div class="select-box">
                <select v-model="oralFilters.semester">
                  <option value="all">全部学期</option>
                  <option value="spring">春季学期</option>
                  <option value="autumn">秋季学期</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>区域：</label>
              <div class="select-box">
                <select v-model="oralFilters.region">
                  <option value="全国">全国</option>
                  <option v-for="p in provinceList" :key="p" :value="p">{{ p }}</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="btn-group">
              <button class="btn-search" @click="handleOralSearch">
                <span class="icon">🔍</span> 分析
              </button>
              <button class="btn-reset" @click="handleOralReset">
                <span class="icon">↺</span> 重置
              </button>
            </div>
          </div>
        </div>

        <div class="charts-grid">
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">📈 龋患率年度/学期变化趋势</h3>
              <div ref="oralTrendChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">📊 学段龋患率对比</h3>
              <div ref="oralGradeChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">👫 性别龋患率对比</h3>
              <div ref="oralGenderChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🍩 龋齿分档占比分布</h3>
              <div ref="oralPieChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">🗺️ 区域龋患率热力分布</h3>
              <div ref="oralMapChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🏆 学校龋患率排行榜</h3>
              <div ref="oralRankingChartRef" class="chart-container"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 心理 Tab ===== -->
      <div v-show="activeTab === 'mental'" class="tab-panel">
        <div class="filter-bar">
          <div class="filter-row">
            <div class="filter-item">
              <label>年份：</label>
              <div class="select-box">
                <select v-model="mentalFilters.year">
                  <option value="2026">2026年</option>
                  <option value="2025">2025年</option>
                  <option value="2024">2024年</option>
                  <option value="2023">2023年</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>学期：</label>
              <div class="select-box">
                <select v-model="mentalFilters.semester">
                  <option value="all">全部学期</option>
                  <option value="spring">春季学期</option>
                  <option value="autumn">秋季学期</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>区域：</label>
              <div class="select-box">
                <select v-model="mentalFilters.region">
                  <option value="全国">全国</option>
                  <option v-for="p in provinceList" :key="p" :value="p">{{ p }}</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="btn-group">
              <button class="btn-search" @click="handleMentalSearch">
                <span class="icon">🔍</span> 分析
              </button>
              <button class="btn-reset" @click="handleMentalReset">
                <span class="icon">↺</span> 重置
              </button>
            </div>
          </div>
        </div>

        <div class="charts-grid">
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">📈 心理健康预警率年度/学期变化趋势</h3>
              <div ref="mentalTrendChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">📊 各学段心理健康预警率对比</h3>
              <div ref="mentalGradeChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">👫 性别心理健康预警率对比</h3>
              <div ref="mentalGenderChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🍩 心理健康风险维度分布</h3>
              <div ref="mentalPieChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">🗺️ 区域心理健康预警率热力分布</h3>
              <div ref="mentalMapChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🏆 学校心理健康预警率排行榜</h3>
              <div ref="mentalRankingChartRef" class="chart-container"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 健康体重 Tab ===== -->
      <div v-show="activeTab === 'weight'" class="tab-panel">
        <div class="filter-bar">
          <div class="filter-row">
            <div class="filter-item">
              <label>年份：</label>
              <div class="select-box">
                <select v-model="weightFilters.year">
                  <option value="2026">2026年</option>
                  <option value="2025">2025年</option>
                  <option value="2024">2024年</option>
                  <option value="2023">2023年</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>学期：</label>
              <div class="select-box">
                <select v-model="weightFilters.semester">
                  <option value="all">全部学期</option>
                  <option value="spring">春季学期</option>
                  <option value="autumn">秋季学期</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>区域：</label>
              <div class="select-box">
                <select v-model="weightFilters.region">
                  <option value="全国">全国</option>
                  <option v-for="p in provinceList" :key="p" :value="p">{{ p }}</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="btn-group">
              <button class="btn-search" @click="handleWeightSearch">
                <span class="icon">🔍</span> 分析
              </button>
              <button class="btn-reset" @click="handleWeightReset">
                <span class="icon">↺</span> 重置
              </button>
            </div>
          </div>
        </div>

        <div class="charts-grid">
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">📈 BMI年度/学期变化趋势</h3>
              <div ref="weightTrendChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">📊 各学段超重率对比</h3>
              <div ref="weightGradeChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">👫 性别超重率对比</h3>
              <div ref="weightGenderChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🍩 体重状态占比分布</h3>
              <div ref="weightPieChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">🗺️ 区域超重率热力分布</h3>
              <div ref="weightMapChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🏆 学校超重率排行榜</h3>
              <div ref="weightRankingChartRef" class="chart-container"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 骨骼健康 Tab ===== -->
      <div v-show="activeTab === 'bone'" class="tab-panel">
        <div class="filter-bar">
          <div class="filter-row">
            <div class="filter-item">
              <label>年份：</label>
              <div class="select-box">
                <select v-model="boneFilters.year">
                  <option value="2026">2026年</option>
                  <option value="2025">2025年</option>
                  <option value="2024">2024年</option>
                  <option value="2023">2023年</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>学期：</label>
              <div class="select-box">
                <select v-model="boneFilters.semester">
                  <option value="all">全部学期</option>
                  <option value="spring">春季学期</option>
                  <option value="autumn">秋季学期</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="filter-item">
              <label>区域：</label>
              <div class="select-box">
                <select v-model="boneFilters.region">
                  <option value="全国">全国</option>
                  <option v-for="p in provinceList" :key="p" :value="p">{{ p }}</option>
                </select>
                <span class="arrow-down">▼</span>
              </div>
            </div>
            <div class="btn-group">
              <button class="btn-search" @click="handleBoneSearch">
                <span class="icon">🔍</span> 分析
              </button>
              <button class="btn-reset" @click="handleBoneReset">
                <span class="icon">↺</span> 重置
              </button>
            </div>
          </div>
        </div>

        <div class="charts-grid">
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">📈 骨密度年度/学期变化趋势</h3>
              <div ref="boneTrendChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">📊 各学段骨密度对比</h3>
              <div ref="boneGradeChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">👫 性别骨密度对比</h3>
              <div ref="boneGenderChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🍩 骨密度状态占比分布</h3>
              <div ref="bonePieChartRef" class="chart-container"></div>
            </div>
          </div>
          <div class="chart-row">
            <div class="chart-card">
              <h3 class="chart-title">🗺️ 区域骨密度偏低率热力分布</h3>
              <div ref="boneMapChartRef" class="chart-container"></div>
            </div>
            <div class="chart-card">
              <h3 class="chart-title">🏆 学校骨密度偏低率排行榜</h3>
              <div ref="boneRankingChartRef" class="chart-container"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, onBeforeUnmount, nextTick } from 'vue';
import { message } from 'ant-design-vue';
import * as echarts from 'echarts';

// ================= Theme Detection =================
const isLight = ref(false);
let themeObserver = null;

// 可视化大屏始终使用深色主题，不受系统深浅色模式影响
const applySystemTheme = () => {
  isLight.value = false;
};

const observeTheme = () => {
  // 不再监听系统主题变化，保持深色背景不受系统颜色影响
  themeObserver = null;
};

const getCurrentInitHandler = () => {
  const map = {
    vision: initVisionCharts,
    oral: initOralCharts,
    mental: initMentalCharts,
    weight: initWeightCharts,
    bone: initBoneCharts
  };
  return map[activeTab.value] || null;
};

// ================= ECharts Theme =================
const getEchartsTheme = () => {
  if (isLight.value) {
    return {
      text: '#334155', textDim: '#64748b', textMuted: '#94a3b8',
      axisLine: 'rgba(8, 145, 178, 0.3)', splitLine: 'rgba(8, 145, 178, 0.1)',
      tooltipBg: 'rgba(255,255,255,0.95)', tooltipBorder: 'rgba(8, 145, 178, 0.2)',
      primary: '#0891b2', secondary: '#7c3aed', accent: '#db2777',
      success: '#16a34a', warning: '#d97706', danger: '#dc2626',
      gridBg: 'transparent'
    };
  }
  return {
    text: '#e2e8f0', textDim: '#94a3b8', textMuted: '#64748b',
    axisLine: 'rgba(56, 189, 248, 0.25)', splitLine: 'rgba(56, 189, 248, 0.08)',
    tooltipBg: 'rgba(10, 22, 50, 0.95)', tooltipBorder: 'rgba(56, 189, 248, 0.3)',
    primary: '#38bdf8', secondary: '#a78bfa', accent: '#f472b6',
    success: '#34d399', warning: '#fbbf24', danger: '#fb7185',
    gridBg: 'transparent'
  };
};

// ================= Tab配置 =================
const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️', badge: '' },
  { key: 'oral', label: '口腔健康', icon: '🦷', badge: '' },
  { key: 'mental', label: '心理健康', icon: '🧠', badge: '新' },
  { key: 'weight', label: '健康体重', icon: '⚖️', badge: '' },
  { key: 'bone', label: '骨骼健康', icon: '🦴', badge: '' }
];
const activeTab = ref('vision');

// ================= 省份列表 =================
const provinceList = [
  '北京市', '上海市', '广东省', '浙江省', '江苏省',
  '山东省', '河南省', '四川省', '湖北省', '湖南省'
];

// ================= 视力Tab - 筛选条件 =================
const visionFilters = reactive({
  year: '2026',
  semester: 'all',
  region: '全国'
});

// ================= 口腔Tab - 筛选条件 =================
const oralFilters = reactive({
  year: '2026',
  semester: 'all',
  region: '全国'
});

// ================= 心理Tab - 筛选条件 =================
const mentalFilters = reactive({
  year: '2026',
  semester: 'all',
  region: '全国'
});

// ================= 健康体重Tab - 筛选条件 =================
const weightFilters = reactive({
  year: '2026',
  semester: 'all',
  region: '全国'
});

// ================= 骨骼健康Tab - 筛选条件 =================
const boneFilters = reactive({
  year: '2026',
  semester: 'all',
  region: '全国'
});

// ================= 视力图表引用 =================
const trendChartRef = ref(null);
const gradeChartRef = ref(null);
const genderChartRef = ref(null);
const pieChartRef = ref(null);
const mapChartRef = ref(null);
const rankingChartRef = ref(null);

// ================= 口腔图表引用 =================
const oralTrendChartRef = ref(null);
const oralGradeChartRef = ref(null);
const oralGenderChartRef = ref(null);
const oralPieChartRef = ref(null);
const oralMapChartRef = ref(null);
const oralRankingChartRef = ref(null);

// ================= 心理图表引用 =================
const mentalTrendChartRef = ref(null);
const mentalGradeChartRef = ref(null);
const mentalGenderChartRef = ref(null);
const mentalPieChartRef = ref(null);
const mentalMapChartRef = ref(null);
const mentalRankingChartRef = ref(null);

// ================= 健康体重图表引用 =================
const weightTrendChartRef = ref(null);
const weightGradeChartRef = ref(null);
const weightGenderChartRef = ref(null);
const weightPieChartRef = ref(null);
const weightMapChartRef = ref(null);
const weightRankingChartRef = ref(null);

// ================= 骨骼健康图表引用 =================
const boneTrendChartRef = ref(null);
const boneGradeChartRef = ref(null);
const boneGenderChartRef = ref(null);
const bonePieChartRef = ref(null);
const boneMapChartRef = ref(null);
const boneRankingChartRef = ref(null);

let chartInstances = [];

// ================= Tab切换 =================
const switchTab = (tabKey) => {
  activeTab.value = tabKey;
  nextTick(() => {
    if (tabKey === 'vision') {
      initVisionCharts();
    } else if (tabKey === 'oral') {
      initOralCharts();
    } else if (tabKey === 'mental') {
      initMentalCharts();
    } else if (tabKey === 'weight') {
      initWeightCharts();
    } else if (tabKey === 'bone') {
      initBoneCharts();
    }
  });
};

// ================= 视力Tab - 筛选逻辑 =================
const handleVisionSearch = () => {
  console.log('视力分析查询:', visionFilters);
  message.success('视力分析查询中...');
  initVisionCharts();
};

const handleVisionReset = () => {
  visionFilters.year = '2026';
  visionFilters.semester = 'all';
  visionFilters.region = '全国';
  message.success('已重置视力筛选条件');
  initVisionCharts();
};

// ================= 口腔Tab - 筛选逻辑 =================
const handleOralSearch = () => {
  console.log('口腔分析查询:', oralFilters);
  message.success('口腔分析查询中...');
  initOralCharts();
};

const handleOralReset = () => {
  oralFilters.year = '2026';
  oralFilters.semester = 'all';
  oralFilters.region = '全国';
  message.success('已重置口腔筛选条件');
  initOralCharts();
};

// ================= 心理Tab - 筛选逻辑 =================
const handleMentalSearch = () => {
  console.log('心理分析查询:', mentalFilters);
  message.success('心理分析查询中...');
  initMentalCharts();
};

const handleMentalReset = () => {
  mentalFilters.year = '2026';
  mentalFilters.semester = 'all';
  mentalFilters.region = '全国';
  message.success('已重置心理筛选条件');
  initMentalCharts();
};

// ================= 健康体重Tab - 筛选逻辑 =================
const handleWeightSearch = () => {
  console.log('体重分析查询:', weightFilters);
  message.success('体重分析查询中...');
  initWeightCharts();
};

const handleWeightReset = () => {
  weightFilters.year = '2026';
  weightFilters.semester = 'all';
  weightFilters.region = '全国';
  message.success('已重置体重筛选条件');
  initWeightCharts();
};

// ================= 骨骼健康Tab - 筛选逻辑 =================
const handleBoneSearch = () => {
  console.log('骨骼分析查询:', boneFilters);
  message.success('骨骼分析查询中...');
  initBoneCharts();
};

const handleBoneReset = () => {
  boneFilters.year = '2026';
  boneFilters.semester = 'all';
  boneFilters.region = '全国';
  message.success('已重置骨骼筛选条件');
  initBoneCharts();
};

// ================= 公共：销毁旧图表/绑定resize =================
const disposeOldCharts = () => {
  chartInstances.forEach(chart => {
    if (chart && !chart.isDisposed()) {
      chart.dispose();
    }
  });
  chartInstances = [];
};

const bindResize = () => {
  const resizeHandler = () => {
    chartInstances.forEach(chart => {
      if (chart && !chart.isDisposed()) {
        chart.resize();
      }
    });
  };
  window.removeEventListener('resize', resizeHandler);
  window.addEventListener('resize', resizeHandler);
};

// ================= 视力Tab - 图表初始化 =================
const initVisionCharts = () => {
  disposeOldCharts();
  const t = getEchartsTheme();

  // 1. 折线图
  if (trendChartRef.value) {
    const chart = echarts.init(trendChartRef.value);
    const years = ['2022春', '2022秋', '2023春', '2023秋', '2024春', '2024秋', '2025春', '2025秋', '2026春'];
    const nationalData = [48.2, 49.1, 49.8, 50.5, 51.2, 52.0, 52.8, 53.2, 53.6];
    const urbanData = [45.5, 46.3, 47.0, 47.8, 48.5, 49.2, 50.0, 50.5, 50.8];
    const ruralData = [52.0, 52.8, 53.5, 54.2, 55.0, 55.8, 56.5, 57.0, 57.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['全国', '城市', '农村'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: years, axisLabel: { rotate: 30, color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '近视率 (%)', min: 40, max: 65,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '全国', type: 'line', data: nationalData, smooth: true, lineStyle: { width: 3, color: t.primary }, itemStyle: { color: t.primary }, areaStyle: { opacity: 0.15, color: t.primary } },
        { name: '城市', type: 'line', data: urbanData, smooth: true, lineStyle: { width: 2, color: t.success }, itemStyle: { color: t.success } },
        { name: '农村', type: 'line', data: ruralData, smooth: true, lineStyle: { width: 2, color: t.warning }, itemStyle: { color: t.warning } }
      ]
    });
    chartInstances.push(chart);
  }

  // 2. 学段对比
  if (gradeChartRef.value) {
    const chart = echarts.init(gradeChartRef.value);
    const grades = ['小学', '初中', '高中'];
    const currentData = [35.5, 71.2, 80.5];
    const previousData = [32.0, 65.5, 76.0];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['当前学年', '上学年'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '近视率 (%)', max: 100,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '当前学年', type: 'bar', data: currentData, barWidth: '30%', itemStyle: { color: t.primary, borderRadius: [4, 4, 0, 0] } },
        { name: '上学年', type: 'bar', data: previousData, barWidth: '30%', itemStyle: { color: t.secondary, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 3. 性别对比
  if (genderChartRef.value) {
    const chart = echarts.init(genderChartRef.value);
    const grades = ['小学', '初中', '高中'];
    const maleData = [32.5, 68.0, 77.5];
    const femaleData = [38.5, 74.5, 83.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['男生', '女生'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '近视率 (%)', max: 100,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '男生', type: 'bar', data: maleData, barWidth: '30%', itemStyle: { color: t.primary, borderRadius: [4, 4, 0, 0] } },
        { name: '女生', type: 'bar', data: femaleData, barWidth: '30%', itemStyle: { color: t.accent, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 4. 饼图
  if (pieChartRef.value) {
    const chart = echarts.init(pieChartRef.value);
    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'item',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, left: 'center', textStyle: { color: t.textDim } },
      color: [t.success, t.warning, t.accent, t.danger],
      series: [{
        type: 'pie',
        radius: ['40%', '70%'],
        center: ['50%', '45%'],
        avoidLabelOverlap: false,
        label: { show: true, formatter: '{b}\n{d}%', color: t.textDim },
        data: [
          { value: 46.4, name: '正常' },
          { value: 30.8, name: '低度近视' },
          { value: 17.5, name: '中度近视' },
          { value: 5.3, name: '高度近视' }
        ]
      }]
    });
    chartInstances.push(chart);
  }

  // 5. 地图
  if (mapChartRef.value) {
    const chart = echarts.init(mapChartRef.value);
    const provinces = ['广东', '浙江', '江苏', '山东', '河南', '四川', '湖北', '湖南', '河北', '安徽'];
    const rates = [56.2, 53.6, 52.8, 51.5, 50.1, 49.8, 51.9, 52.1, 50.5, 49.2];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 60, name: '近视率 (%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: provinces, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 55 ? t.danger : val >= 52 ? t.warning : val >= 48 ? t.success : t.primary,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '50%',
        label: { show: true, position: 'right', formatter: '{c}%', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  // 6. 排行榜
  if (rankingChartRef.value) {
    const chart = echarts.init(rankingChartRef.value);
    const schools = ['第一中学', '实验中学', '育才小学', '红星中学', '第三小学', '第二中学', '希望小学', '阳光小学'];
    const rates = [65, 62, 58, 56, 55, 51, 48, 45];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 70, name: '近视率 (%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: schools, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 60 ? t.danger : val >= 55 ? t.warning : t.success,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '45%',
        label: { show: true, position: 'right', formatter: '{c}%', fontWeight: 'bold', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  bindResize();
};

// ================= 口腔Tab - 图表初始化 =================
const initOralCharts = () => {
  disposeOldCharts();
  const t = getEchartsTheme();

  // 1. 折线图
  if (oralTrendChartRef.value) {
    const chart = echarts.init(oralTrendChartRef.value);
    const years = ['2022春', '2022秋', '2023春', '2023秋', '2024春', '2024秋', '2025春', '2025秋', '2026春'];
    const decayedData = [42.5, 41.8, 40.5, 39.2, 38.0, 36.8, 36.0, 35.6, 35.2];
    const permanentData = [24.5, 25.2, 26.0, 26.8, 27.5, 28.0, 28.5, 28.9, 29.5];
    const coverageData = [52.0, 55.0, 58.0, 60.0, 63.0, 65.0, 67.0, 68.5, 70.0];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['乳牙龋', '恒牙龋', '涂氟覆盖率'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: years, axisLabel: { rotate: 30, color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: [
        {
          type: 'value', name: '患病率(%)', min: 0, max: 50,
          nameTextStyle: { color: t.textDim },
          axisLabel: { color: t.textDim },
          axisLine: { lineStyle: { color: t.axisLine } },
          splitLine: { lineStyle: { color: t.splitLine } }
        },
        {
          type: 'value', name: '覆盖率(%)', min: 0, max: 100,
          nameTextStyle: { color: t.textDim },
          axisLabel: { color: t.textDim },
          axisLine: { lineStyle: { color: t.axisLine } },
          splitLine: { lineStyle: { color: t.splitLine } }
        }
      ],
      series: [
        { name: '乳牙龋', type: 'line', data: decayedData, smooth: true, lineStyle: { width: 3, color: t.primary }, itemStyle: { color: t.primary }, areaStyle: { opacity: 0.15, color: t.primary } },
        { name: '恒牙龋', type: 'line', data: permanentData, smooth: true, lineStyle: { width: 3, color: t.success }, itemStyle: { color: t.success }, areaStyle: { opacity: 0.15, color: t.success } },
        { name: '涂氟覆盖率', type: 'line', yAxisIndex: 1, data: coverageData, smooth: true, lineStyle: { width: 2, color: t.warning, type: 'dashed' }, itemStyle: { color: t.warning } }
      ]
    });
    chartInstances.push(chart);
  }

  // 2. 学段对比
  if (oralGradeChartRef.value) {
    const chart = echarts.init(oralGradeChartRef.value);
    const grades = ['小学1-2', '小学3-4', '小学5-6', '初中', '高中'];
    const decayedData = [35.6, 28.5, 18.2, 5.6, 2.1];
    const permanentData = [2.1, 8.5, 18.2, 28.9, 32.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['乳牙龋', '恒牙龋'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '患病率(%)', max: 45,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '乳牙龋', type: 'bar', data: decayedData, barWidth: '30%', itemStyle: { color: t.primary, borderRadius: [4, 4, 0, 0] } },
        { name: '恒牙龋', type: 'bar', data: permanentData, barWidth: '30%', itemStyle: { color: t.success, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 3. 性别对比
  if (oralGenderChartRef.value) {
    const chart = echarts.init(oralGenderChartRef.value);
    const grades = ['小学1-2', '小学3-4', '小学5-6', '初中', '高中'];
    const maleData = [32.5, 25.8, 18.5, 16.5, 14.2];
    const femaleData = [38.6, 30.5, 22.2, 18.8, 16.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['男生', '女生'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '龋患率(%)', max: 45,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '男生', type: 'bar', data: maleData, barWidth: '30%', itemStyle: { color: t.primary, borderRadius: [4, 4, 0, 0] } },
        { name: '女生', type: 'bar', data: femaleData, barWidth: '30%', itemStyle: { color: t.accent, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 4. 饼图
  if (oralPieChartRef.value) {
    const chart = echarts.init(oralPieChartRef.value);
    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'item',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, left: 'center', textStyle: { color: t.textDim } },
      color: [t.success, t.warning, t.accent, t.danger],
      series: [{
        type: 'pie',
        radius: ['40%', '70%'],
        center: ['50%', '45%'],
        avoidLabelOverlap: false,
        label: { show: true, formatter: '{b}\n{d}%', color: t.textDim },
        data: [
          { value: 52.8, name: '无龋齿' },
          { value: 28.5, name: '轻度龋齿' },
          { value: 12.8, name: '中度龋齿' },
          { value: 5.9, name: '重度龋齿' }
        ]
      }]
    });
    chartInstances.push(chart);
  }

  // 5. 地图
  if (oralMapChartRef.value) {
    const chart = echarts.init(oralMapChartRef.value);
    const provinces = ['广东', '浙江', '江苏', '山东', '河南', '四川', '湖北', '湖南'];
    const rates = [48.5, 45.2, 42.3, 40.5, 38.2, 36.8, 35.5, 33.2];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 55, name: '龋患率(%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: provinces, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 45 ? t.danger : val >= 40 ? t.warning : val >= 35 ? t.primary : t.success,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '50%',
        label: { show: true, position: 'right', formatter: '{c}%', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  // 6. 排行榜
  if (oralRankingChartRef.value) {
    const chart = echarts.init(oralRankingChartRef.value);
    const schools = ['第一中学', '育才小学', '红星中学', '实验中学', '第三小学', '希望中学'];
    const rates = [52.5, 48.8, 45.2, 42.3, 38.5, 35.2];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 55, name: '龋患率(%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: schools, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 50 ? t.danger : val >= 45 ? t.warning : t.success,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '45%',
        label: { show: true, position: 'right', formatter: '{c}%', fontWeight: 'bold', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  bindResize();
};

// ================= 健康体重Tab - 图表初始化 =================
const initWeightCharts = () => {
  disposeOldCharts();
  const t = getEchartsTheme();

  // 1. 折线图：BMI年度/学期变化趋势
  if (weightTrendChartRef.value) {
    const chart = echarts.init(weightTrendChartRef.value);
    const years = ['2022春', '2022秋', '2023春', '2023秋', '2024春', '2024秋', '2025春', '2025秋', '2026春'];
    const bmiData = [18.2, 18.5, 18.8, 19.0, 19.2, 19.5, 19.8, 20.0, 20.2];
    const overweightData = [18.5, 19.0, 19.5, 20.0, 20.5, 21.0, 21.5, 22.0, 22.8];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['平均BMI', '超重率'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: years, axisLabel: { rotate: 30, color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: [
        {
          type: 'value', name: 'BMI', min: 16, max: 24,
          nameTextStyle: { color: t.textDim },
          axisLabel: { color: t.textDim },
          axisLine: { lineStyle: { color: t.axisLine } },
          splitLine: { lineStyle: { color: t.splitLine } }
        },
        {
          type: 'value', name: '超重率(%)', min: 15, max: 28,
          nameTextStyle: { color: t.textDim },
          axisLabel: { color: t.textDim },
          axisLine: { lineStyle: { color: t.axisLine } },
          splitLine: { lineStyle: { color: t.splitLine } }
        }
      ],
      series: [
        { name: '平均BMI', type: 'line', data: bmiData, smooth: true, lineStyle: { width: 3, color: t.primary }, itemStyle: { color: t.primary }, areaStyle: { opacity: 0.15, color: t.primary } },
        { name: '超重率', type: 'line', yAxisIndex: 1, data: overweightData, smooth: true, lineStyle: { width: 2, color: t.warning }, itemStyle: { color: t.warning } }
      ]
    });
    chartInstances.push(chart);
  }

  // 2. 各学段超重率对比
  if (weightGradeChartRef.value) {
    const chart = echarts.init(weightGradeChartRef.value);
    const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
    const currentData = [14.5, 16.8, 20.5, 25.2, 28.5];
    const previousData = [12.5, 14.8, 18.5, 22.2, 25.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['当前学年', '上学年'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '超重率 (%)', max: 35,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '当前学年', type: 'bar', data: currentData, barWidth: '30%', itemStyle: { color: t.warning, borderRadius: [4, 4, 0, 0] } },
        { name: '上学年', type: 'bar', data: previousData, barWidth: '30%', itemStyle: { color: t.textMuted, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 3. 性别超重率对比
  if (weightGenderChartRef.value) {
    const chart = echarts.init(weightGenderChartRef.value);
    const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
    const maleData = [16, 18, 22, 28, 32];
    const femaleData = [12, 14, 18, 22, 25];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['男生', '女生'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '超重率 (%)', max: 35,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '男生', type: 'bar', data: maleData, barWidth: '30%', itemStyle: { color: t.primary, borderRadius: [4, 4, 0, 0] } },
        { name: '女生', type: 'bar', data: femaleData, barWidth: '30%', itemStyle: { color: t.accent, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 4. 饼图：体重状态占比
  if (weightPieChartRef.value) {
    const chart = echarts.init(weightPieChartRef.value);
    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'item',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, left: 'center', textStyle: { color: t.textDim } },
      color: [t.success, t.warning, t.accent, t.danger],
      series: [{
        type: 'pie',
        radius: ['40%', '70%'],
        center: ['50%', '45%'],
        avoidLabelOverlap: false,
        label: { show: true, formatter: '{b}\n{d}%', color: t.textDim },
        data: [
          { value: 52.8, name: '正常体重' },
          { value: 15.6, name: '偏瘦' },
          { value: 18.5, name: '超重' },
          { value: 13.1, name: '肥胖' }
        ]
      }]
    });
    chartInstances.push(chart);
  }

  // 5. 地图：区域超重率热力分布
  if (weightMapChartRef.value) {
    const chart = echarts.init(weightMapChartRef.value);
    const provinces = ['广东', '浙江', '江苏', '山东', '河南', '四川', '湖北', '湖南', '河北', '安徽'];
    const rates = [24.5, 22.8, 21.5, 23.2, 25.8, 20.5, 21.2, 20.8, 22.5, 23.8];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 28, name: '超重率 (%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: provinces, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 24 ? t.danger : val >= 21 ? t.warning : t.primary,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '50%',
        label: { show: true, position: 'right', formatter: '{c}%', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  // 6. 排行榜：学校超重率排行榜
  if (weightRankingChartRef.value) {
    const chart = echarts.init(weightRankingChartRef.value);
    const schools = ['第一中学', '育才小学', '红星中学', '实验中学', '第三小学', '希望小学', '阳光小学'];
    const rates = [32.5, 30.2, 28.5, 26.8, 24.5, 22.3, 20.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 35, name: '超重率 (%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: schools, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 30 ? t.danger : val >= 25 ? t.warning : t.primary,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '45%',
        label: { show: true, position: 'right', formatter: '{c}%', fontWeight: 'bold', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  bindResize();
};

// ================= 心理Tab - 图表初始化 =================
const initMentalCharts = () => {
  disposeOldCharts();
  const t = getEchartsTheme();

  // 1. 折线图
  if (mentalTrendChartRef.value) {
    const chart = echarts.init(mentalTrendChartRef.value);
    const years = ['2022春', '2022秋', '2023春', '2023秋', '2024春', '2024秋', '2025春', '2025秋', '2026春'];
    const totalData = [22.5, 21.8, 21.2, 20.5, 19.8, 19.2, 18.8, 18.2, 18.6];
    const anxietyData = [12.5, 12.8, 12.2, 11.8, 11.5, 11.2, 10.8, 10.5, 10.2];
    const depressionData = [8.5, 8.8, 8.2, 7.8, 7.5, 7.2, 6.8, 6.5, 6.2];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['综合预警率', '焦虑倾向', '抑郁倾向'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: years, axisLabel: { rotate: 30, color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '预警率 (%)', min: 0, max: 30,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '综合预警率', type: 'line', data: totalData, smooth: true, lineStyle: { width: 3, color: t.danger }, itemStyle: { color: t.danger }, areaStyle: { opacity: 0.15, color: t.danger } },
        { name: '焦虑倾向', type: 'line', data: anxietyData, smooth: true, lineStyle: { width: 2, color: t.warning }, itemStyle: { color: t.warning } },
        { name: '抑郁倾向', type: 'line', data: depressionData, smooth: true, lineStyle: { width: 2, color: t.primary }, itemStyle: { color: t.primary } }
      ]
    });
    chartInstances.push(chart);
  }

  // 2. 学段对比
  if (mentalGradeChartRef.value) {
    const chart = echarts.init(mentalGradeChartRef.value);
    const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
    const currentData = [12.5, 16.8, 22.5, 28.2, 32.5];
    const previousData = [14.5, 18.2, 24.5, 30.5, 35.2];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['当前学年', '上学年'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '预警率 (%)', max: 40,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '当前学年', type: 'bar', data: currentData, barWidth: '30%', itemStyle: { color: t.secondary, borderRadius: [4, 4, 0, 0] } },
        { name: '上学年', type: 'bar', data: previousData, barWidth: '30%', itemStyle: { color: t.textMuted, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 3. 性别对比
  if (mentalGenderChartRef.value) {
    const chart = echarts.init(mentalGenderChartRef.value);
    const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
    const maleData = [10.5, 14.2, 18.5, 24.5, 28.5];
    const femaleData = [14.5, 18.5, 24.5, 30.5, 35.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['男生', '女生'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '预警率 (%)', max: 40,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '男生', type: 'bar', data: maleData, barWidth: '30%', itemStyle: { color: t.primary, borderRadius: [4, 4, 0, 0] } },
        { name: '女生', type: 'bar', data: femaleData, barWidth: '30%', itemStyle: { color: t.accent, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 4. 饼图
  if (mentalPieChartRef.value) {
    const chart = echarts.init(mentalPieChartRef.value);
    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'item',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, left: 'center', textStyle: { color: t.textDim } },
      color: [t.danger, t.warning, t.primary, t.success, t.secondary, t.accent],
      series: [{
        type: 'pie',
        radius: ['40%', '70%'],
        center: ['50%', '45%'],
        avoidLabelOverlap: false,
        label: { show: true, formatter: '{b}\n{d}%', color: t.textDim },
        data: [
          { value: 28, name: '学习焦虑' },
          { value: 22, name: '人际敏感' },
          { value: 18, name: '抑郁倾向' },
          { value: 15, name: '冲动倾向' },
          { value: 10, name: '身体症状' },
          { value: 7, name: '其他' }
        ]
      }]
    });
    chartInstances.push(chart);
  }

  // 5. 地图
  if (mentalMapChartRef.value) {
    const chart = echarts.init(mentalMapChartRef.value);
    const provinces = ['广东', '浙江', '江苏', '山东', '河南', '四川', '湖北', '湖南', '河北', '安徽'];
    const rates = [24.5, 21.3, 19.8, 18.5, 22.1, 17.6, 18.9, 16.8, 20.2, 19.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 28, name: '预警率 (%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: provinces, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 22 ? t.danger : val >= 19 ? t.warning : t.primary,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '50%',
        label: { show: true, position: 'right', formatter: '{c}%', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  // 6. 排行榜
  if (mentalRankingChartRef.value) {
    const chart = echarts.init(mentalRankingChartRef.value);
    const schools = ['第一中学', '实验中学', '育才小学', '红星中学', '第三小学', '第二中学', '希望小学', '阳光小学'];
    const rates = [32.5, 28.5, 26.2, 24.8, 22.5, 20.3, 18.6, 16.2];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 35, name: '预警率 (%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: schools, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 28 ? t.danger : val >= 22 ? t.warning : t.success,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '45%',
        label: { show: true, position: 'right', formatter: '{c}%', fontWeight: 'bold', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  bindResize();
};

// ================= 骨骼健康Tab - 图表初始化 =================
const initBoneCharts = () => {
  disposeOldCharts();
  const t = getEchartsTheme();

  // 1. 折线图：骨密度年度变化趋势
  if (boneTrendChartRef.value) {
    const chart = echarts.init(boneTrendChartRef.value);
    const years = ['2022春', '2022秋', '2023春', '2023秋', '2024春', '2024秋', '2025春', '2025秋', '2026春'];
    const densityData = [102.5, 103.2, 103.8, 104.5, 105.2, 105.8, 106.3, 106.8, 107.2];
    const lowRateData = [18.5, 18.2, 17.8, 17.5, 17.2, 16.8, 16.5, 16.2, 15.8];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['平均骨密度', '偏低率'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: years, axisLabel: { rotate: 30, color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: [
        {
          type: 'value', name: '骨密度(g/cm²)', min: 95, max: 115,
          nameTextStyle: { color: t.textDim },
          axisLabel: { color: t.textDim },
          axisLine: { lineStyle: { color: t.axisLine } },
          splitLine: { lineStyle: { color: t.splitLine } }
        },
        {
          type: 'value', name: '偏低率(%)', min: 10, max: 25,
          nameTextStyle: { color: t.textDim },
          axisLabel: { color: t.textDim },
          axisLine: { lineStyle: { color: t.axisLine } },
          splitLine: { lineStyle: { color: t.splitLine } }
        }
      ],
      series: [
        { name: '平均骨密度', type: 'line', data: densityData, smooth: true, lineStyle: { width: 3, color: t.primary }, itemStyle: { color: t.primary }, areaStyle: { opacity: 0.15, color: t.primary } },
        { name: '偏低率', type: 'line', yAxisIndex: 1, data: lowRateData, smooth: true, lineStyle: { width: 2, color: t.warning }, itemStyle: { color: t.warning } }
      ]
    });
    chartInstances.push(chart);
  }

  // 2. 各学段骨密度对比
  if (boneGradeChartRef.value) {
    const chart = echarts.init(boneGradeChartRef.value);
    const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
    const currentData = [101.5, 103.2, 105.8, 108.5, 110.2];
    const previousData = [100.8, 102.5, 104.8, 107.2, 109.0];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['当前学年', '上学年'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '骨密度(g/cm²)', min: 98, max: 115,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '当前学年', type: 'bar', data: currentData, barWidth: '30%', itemStyle: { color: t.primary, borderRadius: [4, 4, 0, 0] } },
        { name: '上学年', type: 'bar', data: previousData, barWidth: '30%', itemStyle: { color: t.secondary, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 3. 性别骨密度对比
  if (boneGenderChartRef.value) {
    const chart = echarts.init(boneGenderChartRef.value);
    const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
    const maleData = [102.5, 104.8, 107.2, 110.5, 112.8];
    const femaleData = [100.8, 102.5, 104.5, 106.8, 108.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, data: ['男生', '女生'], textStyle: { color: t.textDim } },
      grid: { left: '3%', right: '4%', top: '8%', bottom: '20%', containLabel: true },
      xAxis: {
        type: 'category', data: grades, axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'value', name: '骨密度(g/cm²)', min: 98, max: 115,
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [
        { name: '男生', type: 'bar', data: maleData, barWidth: '30%', itemStyle: { color: t.primary, borderRadius: [4, 4, 0, 0] } },
        { name: '女生', type: 'bar', data: femaleData, barWidth: '30%', itemStyle: { color: t.accent, borderRadius: [4, 4, 0, 0] } }
      ]
    });
    chartInstances.push(chart);
  }

  // 4. 饼图：骨密度状态占比
  if (bonePieChartRef.value) {
    const chart = echarts.init(bonePieChartRef.value);
    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'item',
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      legend: { bottom: 0, left: 'center', textStyle: { color: t.textDim } },
      color: [t.success, t.warning, t.danger, t.primary],
      series: [{
        type: 'pie',
        radius: ['40%', '70%'],
        center: ['50%', '45%'],
        avoidLabelOverlap: false,
        label: { show: true, formatter: '{b}\n{d}%', color: t.textDim },
        data: [
          { value: 65.2, name: '正常' },
          { value: 18.5, name: '偏低' },
          { value: 10.3, name: '骨质疏松风险' },
          { value: 6.0, name: '需关注' }
        ]
      }]
    });
    chartInstances.push(chart);
  }

  // 5. 区域骨密度偏低率
  if (boneMapChartRef.value) {
    const chart = echarts.init(boneMapChartRef.value);
    const provinces = ['广东', '浙江', '江苏', '山东', '河南', '四川', '湖北', '湖南', '河北', '安徽'];
    const rates = [14.5, 12.8, 11.5, 13.2, 15.8, 10.5, 11.2, 10.8, 12.5, 13.8];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 20, name: '偏低率 (%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: provinces, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 15 ? t.danger : val >= 12 ? t.warning : t.primary,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '50%',
        label: { show: true, position: 'right', formatter: '{c}%', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  // 6. 排行榜
  if (boneRankingChartRef.value) {
    const chart = echarts.init(boneRankingChartRef.value);
    const schools = ['第一中学', '育才小学', '红星中学', '实验中学', '第三小学', '希望小学', '阳光小学'];
    const rates = [20.5, 18.2, 16.8, 15.2, 14.5, 12.3, 10.5];

    chart.setOption({
      textStyle: { color: t.text },
      tooltip: {
        trigger: 'axis', axisPointer: { type: 'shadow' },
        backgroundColor: t.tooltipBg,
        borderColor: t.tooltipBorder,
        textStyle: { color: t.text }
      },
      grid: { left: '3%', right: '8%', top: '5%', bottom: '5%', containLabel: true },
      xAxis: {
        type: 'value', max: 25, name: '偏低率 (%)',
        nameTextStyle: { color: t.textDim },
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      yAxis: {
        type: 'category', data: schools, inverse: true,
        axisLabel: { color: t.textDim },
        axisLine: { lineStyle: { color: t.axisLine } },
        splitLine: { lineStyle: { color: t.splitLine } }
      },
      series: [{
        type: 'bar',
        data: rates.map(val => ({
          value: val,
          itemStyle: {
            color: val >= 18 ? t.danger : val >= 15 ? t.warning : t.success,
            borderRadius: [0, 4, 4, 0]
          }
        })),
        barWidth: '45%',
        label: { show: true, position: 'right', formatter: '{c}%', fontWeight: 'bold', color: t.textDim }
      }]
    });
    chartInstances.push(chart);
  }

  bindResize();
};

// ================= 生命周期 =================
onMounted(() => {
  applySystemTheme();
  observeTheme();
  initVisionCharts();
});

onBeforeUnmount(() => {
  if (themeObserver) {
    themeObserver.disconnect();
    themeObserver = null;
  }
  chartInstances.forEach(chart => {
    if (chart && !chart.isDisposed()) {
      chart.dispose();
    }
  });
  chartInstances = [];
});
</script>

<style scoped>
/* ===== CSS Variables ===== */
.dashboard-wrapper {
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

  padding: 20px;
  min-height: 100vh;
  font-family: 'Inter', 'PingFang SC', 'Microsoft YaHei', system-ui, sans-serif;
  position: relative;
  overflow: hidden;
  background: var(--bg-page);
  color: var(--text);
}

.dashboard-wrapper > * {
  position: relative;
  z-index: 2;
}

.dashboard-wrapper.theme-light {
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

/* ===== Scrollbar ===== */
.dashboard-wrapper ::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}
.dashboard-wrapper ::-webkit-scrollbar-track {
  background: transparent;
}
.dashboard-wrapper ::-webkit-scrollbar-thumb {
  background: var(--scrollbar-thumb);
  border-radius: 4px;
}
.dashboard-wrapper ::-webkit-scrollbar-thumb:hover {
  background: var(--border-strong);
}

/* ===== Page Header ===== */
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

@keyframes dot-blink {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.3; transform: scale(0.6); }
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

/* ===== Tab Bar ===== */
.tab-bar {
  display: flex;
  gap: 6px;
  padding: 4px;
  margin-bottom: 16px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  box-shadow: var(--shadow);
  backdrop-filter: blur(8px);
  position: relative;
}

.tab-bar::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.7;
}

.tab-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 10px 20px;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  color: var(--text-dim);
  font-size: 14px;
  font-weight: 500;
  position: relative;
  border: 1px solid transparent;
  flex: 1;
  justify-content: center;
}

.tab-item:hover {
  background: var(--bg-hover);
  color: var(--primary);
  border-color: var(--border);
}

.tab-item.active {
  background: var(--primary-bg);
  color: var(--primary);
  border-color: var(--border-strong);
  box-shadow: 0 0 12px var(--primary-bg);
}

.tab-item.active::after {
  content: '';
  position: absolute;
  bottom: -5px;
  left: 50%;
  transform: translateX(-50%);
  width: 30px;
  height: 2px;
  background: var(--primary);
  border-radius: 2px;
}

.tab-icon {
  font-size: 16px;
}

.tab-label {
  letter-spacing: 0.5px;
}

.tab-badge {
  background: var(--danger);
  color: #fff;
  font-size: 11px;
  padding: 1px 6px;
  border-radius: 8px;
  font-weight: 600;
}

/* ===== Tab Panel ===== */
.tab-panel {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

/* ===== Filter Bar ===== */
.filter-bar {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  box-shadow: var(--shadow);
  backdrop-filter: blur(8px);
  padding: 14px 18px;
  margin-bottom: 16px;
  position: relative;
  transition: box-shadow 0.4s cubic-bezier(0.4, 0, 0.2, 1), border-color 0.4s;
}

.filter-bar::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.7;
}

.filter-bar:hover {
  box-shadow: var(--shadow-card);
  border-color: var(--border-strong);
}

.filter-row {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  align-items: center;
}

.filter-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.filter-item label {
  font-size: 14px;
  font-weight: 500;
  color: var(--text-dim);
  white-space: nowrap;
}

.select-box {
  position: relative;
  display: inline-flex;
  align-items: center;
}

.select-box select {
  appearance: none;
  background: var(--bg-soft);
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 6px 28px 6px 12px;
  font-size: 14px;
  color: var(--text);
  outline: none;
  cursor: pointer;
  transition: border-color 0.2s, box-shadow 0.2s;
  min-width: 120px;
  font-family: inherit;
}

.select-box select:focus {
  border-color: var(--primary);
  box-shadow: 0 0 0 2px var(--primary-bg);
}

.select-box select option {
  background: var(--bg-card);
  color: var(--text);
}

.arrow-down {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--text-muted);
  font-size: 10px;
  pointer-events: none;
}

.btn-group {
  display: flex;
  gap: 10px;
  margin-left: auto;
}

/* ===== Buttons ===== */
.btn-search, .btn-reset {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 7px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.25s ease;
  border: 1px solid transparent;
  font-family: inherit;
}

.btn-search {
  background: var(--primary);
  color: #fff;
  border-color: var(--primary);
}

.btn-search:hover {
  background: var(--primary-hover);
  border-color: var(--primary-hover);
  box-shadow: 0 0 12px var(--primary-bg);
}

.btn-reset {
  background: var(--bg-card);
  color: var(--text);
  border-color: var(--border);
}

.btn-reset:hover {
  background: var(--bg-hover);
  border-color: var(--primary);
  color: var(--primary);
}

/* ===== Charts Grid ===== */
.charts-grid {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.chart-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

/* ===== Chart Card ===== */
.chart-card {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  box-shadow: var(--shadow);
  backdrop-filter: blur(8px);
  padding: 14px 16px;
  position: relative;
  transition: box-shadow 0.4s cubic-bezier(0.4, 0, 0.2, 1), border-color 0.4s;
  overflow: hidden;
}

.chart-card::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.7;
  transition: opacity 0.4s ease;
}

.chart-card:hover {
  box-shadow: var(--shadow-card);
  border-color: var(--border-strong);
}

.chart-card:hover::before {
  opacity: 1;
}

.chart-title {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-strong);
  margin: 0 0 10px 0;
  letter-spacing: 0.5px;
  display: flex;
  align-items: center;
  gap: 6px;
}

.chart-container {
  width: 100%;
  height: 260px;
}

/* ===== Light Mode Specific ===== */
.dashboard-wrapper.theme-light .scan-line {
  opacity: 0.12;
}

.dashboard-wrapper.theme-light .bg-glow {
  opacity: 0.1;
  filter: blur(80px);
}

.dashboard-wrapper.theme-light .bg-grid {
  opacity: 0.4;
  background-image:
    linear-gradient(rgba(8, 145, 178, 0.06) 1px, transparent 1px),
    linear-gradient(90deg, rgba(8, 145, 178, 0.06) 1px, transparent 1px);
}

.dashboard-wrapper.theme-light .page-header {
  background: linear-gradient(90deg, rgba(8, 145, 178, 0.08), transparent 65%);
  border-color: rgba(8, 145, 178, 0.2);
}

.dashboard-wrapper.theme-light .page-header::before {
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, transparent);
}

.dashboard-wrapper.theme-light .page-title {
  font-size: 20px;
}

.dashboard-wrapper.theme-light .title-text {
  background: linear-gradient(180deg, #1e293b 0%, #0891b2 100%);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 10px rgba(8, 145, 178, 0.18));
}

.dashboard-wrapper.theme-light .title-bracket {
  color: #0891b2;
  opacity: 0.6;
}

.dashboard-wrapper.theme-light .hd-line {
  background: linear-gradient(90deg, #0891b2, transparent);
}

.dashboard-wrapper.theme-light .hd-line::after {
  background: #0891b2;
  box-shadow: 0 0 4px rgba(8, 145, 178, 0.45);
}

.dashboard-wrapper.theme-light .hd-dot {
  background: #0891b2;
  box-shadow: 0 0 6px rgba(8, 145, 178, 0.45);
}

.dashboard-wrapper.theme-light .tab-bar {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.2);
  box-shadow: 0 2px 8px rgba(15, 23, 42, 0.05);
}

.dashboard-wrapper.theme-light .tab-item {
  color: #64748b;
}

.dashboard-wrapper.theme-light .tab-item:hover {
  background: rgba(8, 145, 178, 0.06);
  color: #0891b2;
}

.dashboard-wrapper.theme-light .tab-item.active {
  background: rgba(8, 145, 178, 0.08);
  color: #0891b2;
  border-color: rgba(8, 145, 178, 0.35);
  box-shadow: 0 0 8px rgba(8, 145, 178, 0.15);
}

.dashboard-wrapper.theme-light .filter-bar {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.2);
  box-shadow: 0 2px 8px rgba(15, 23, 42, 0.05);
  backdrop-filter: none;
}

.dashboard-wrapper.theme-light .filter-bar::before {
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, transparent);
  opacity: 0.65;
}

.dashboard-wrapper.theme-light .filter-item label {
  color: #475569;
}

.dashboard-wrapper.theme-light .select-box select {
  background: #fff;
  border-color: #e2e8f0;
  color: #1e293b;
}

.dashboard-wrapper.theme-light .select-box select:focus {
  border-color: #0891b2;
  box-shadow: 0 0 0 3px rgba(8, 145, 178, 0.1);
}

.dashboard-wrapper.theme-light .btn-search {
  background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
  border-color: #0891b2;
  box-shadow: 0 2px 8px rgba(8, 145, 178, 0.3);
}

.dashboard-wrapper.theme-light .btn-search:hover {
  background: linear-gradient(135deg, #0e7490 0%, #155e75 100%);
  box-shadow: 0 4px 12px rgba(8, 145, 178, 0.4);
}

.dashboard-wrapper.theme-light .btn-reset {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.25);
  color: #0891b2;
}

.dashboard-wrapper.theme-light .btn-reset:hover {
  border-color: #0891b2;
  background: rgba(8, 145, 178, 0.06);
  box-shadow: 0 2px 8px rgba(8, 145, 178, 0.15);
}

.dashboard-wrapper.theme-light .chart-card {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.22);
  box-shadow: 0 2px 8px rgba(15, 23, 42, 0.05), 0 1px 2px rgba(15, 23, 42, 0.04);
  backdrop-filter: none;
}

.dashboard-wrapper.theme-light .chart-card::before {
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, transparent);
  opacity: 0.65;
}

.dashboard-wrapper.theme-light .chart-card:hover {
  box-shadow: 0 4px 16px rgba(15, 23, 42, 0.08), 0 0 0 1px rgba(8, 145, 178, 0.12);
  border-color: rgba(8, 145, 178, 0.35);
}

.dashboard-wrapper.theme-light .chart-title {
  color: #0f172a;
}

/* ===== Responsive ===== */
@media (max-width: 1200px) {
  .chart-row {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .dashboard-wrapper {
    padding: 12px;
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
  .filter-row {
    flex-direction: column;
    align-items: stretch;
  }
  .btn-group {
    margin-left: 0;
    justify-content: flex-end;
  }
  .chart-container {
    height: 220px;
  }
  .tab-item {
    padding: 8px 12px;
    font-size: 13px;
  }
}
</style>
