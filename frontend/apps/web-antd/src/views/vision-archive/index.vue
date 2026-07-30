<template>
  <div class="app-root" :class="{ 'theme-light': isLight }">
    <!-- 背景 -->
    <div class="bg-decor" aria-hidden="true">
      <div class="bg-grid"></div>
      <div class="bg-glow bg-glow-1"></div>
      <div class="bg-glow bg-glow-2"></div>
      <div class="bg-glow bg-glow-3"></div>
      <div class="bg-stars"></div>
    </div>

    <!-- 扫描线 -->
    <div class="scan-line"></div>
    <div class="scan-line-2"></div>

    <!-- 顶部装饰光带 -->
    <div class="tech-topbar">
      <div class="tech-corner tech-corner-l"><span class="tc-dot"></span></div>
      <div class="tech-line"><span class="line-pulse line-pulse-1"></span><span class="line-pulse line-pulse-2"></span></div>
      <div class="tech-dots">
        <span></span><span></span><span></span><span></span><span></span><span></span><span></span>
      </div>
      <div class="tech-line"><span class="line-pulse line-pulse-3"></span><span class="line-pulse line-pulse-4"></span></div>
      <div class="tech-corner tech-corner-r"><span class="tc-dot"></span></div>
    </div>

    <!-- Header -->
    <header class="national-header">
      <div class="header-deco header-deco-l">
        <span class="hd-line"></span>
        <span class="hd-dot"></span>
      </div>
      <h1 class="national-title">
        <span class="title-bracket">【</span>
        <span class="title-text">全国中小学生健康监测大屏</span>
        <span class="title-bracket">】</span>
        <span class="title-shine"></span>
      </h1>
      <div class="national-time">
        <span class="time-date">{{ currentDate }}</span>
        <span class="time-clock">
          <span class="tc-digit">{{ currentTime }}</span>
        </span>
      </div>
      <div class="header-deco header-deco-r" style="visibility:hidden">
        <span class="hd-dot"></span>
        <span class="hd-line"></span>
      </div>
    </header>

    <!-- Tab 切换 -->
    <nav class="national-tabs">
      <button v-for="t in tabs" :key="t.key"
        :class="['tech-tab', { active: activeTab === t.key }]"
        @click="switchTab(t.key)">
        <span class="tech-tab-icon">{{ t.icon }}</span>
        <span class="tech-tab-label">{{ t.label }}</span>
        <span class="tech-tab-glow"></span>
      </button>
    </nav>

    <!-- 顶部筛选栏 -->
    <div class="top-filter-bar">
      <div class="filter-select" ref="provinceRef" :class="{ 'has-value': filter.province, 'is-open': openDropdown === 'province' }">
        <div class="select-trigger" :class="{ disabled: false }" @click.stop="toggleDropdown('province')">
          <span class="select-value" :class="{ placeholder: !filter.province }">
            {{ getSelectedName(provinceList, filter.province) || '请选择省份' }}
          </span>
          <span v-if="filter.province" class="clear-icon" @click.stop="clearProvince">✕</span>
          <span class="arrow-down" :class="{ rotated: openDropdown === 'province' }">▼</span>
        </div>
        <div v-if="openDropdown === 'province'" class="select-dropdown">
          <div class="dropdown-header">选择省份</div>
          <div class="dropdown-list">
            <div v-for="p in provinceList" :key="p.code"
              class="dropdown-item" :class="{ selected: filter.province === p.code }"
              @click.stop="selectOption('province', p.code)">
              {{ p.name }}
              <span v-if="filter.province === p.code" class="check-mark">✓</span>
            </div>
          </div>
        </div>
      </div>

      <div class="filter-select" ref="cityRef" :class="{ 'has-value': filter.city, 'is-open': openDropdown === 'city', 'disabled': !filter.province }">
        <div class="select-trigger" :class="{ disabled: !filter.province }" @click.stop="toggleDropdown('city')">
          <span class="select-value" :class="{ placeholder: !filter.city }">
            {{ getSelectedName(cityOptions, filter.city) || '请选择城市' }}
          </span>
          <span v-if="filter.city" class="clear-icon" @click.stop="clearCity">✕</span>
          <span class="arrow-down" :class="{ rotated: openDropdown === 'city' }">▼</span>
        </div>
        <div v-if="openDropdown === 'city'" class="select-dropdown">
          <div class="dropdown-header">选择城市</div>
          <div class="dropdown-list">
            <div v-for="c in cityOptions" :key="c.code"
              class="dropdown-item" :class="{ selected: filter.city === c.code }"
              @click.stop="selectOption('city', c.code)">
              {{ c.name }}
              <span v-if="filter.city === c.code" class="check-mark">✓</span>
            </div>
            <div v-if="cityOptions.length === 0" class="dropdown-empty">请先选择省份</div>
          </div>
        </div>
      </div>

      <div class="filter-select" ref="districtRef" :class="{ 'has-value': filter.district, 'is-open': openDropdown === 'district', 'disabled': !filter.city }">
        <div class="select-trigger" :class="{ disabled: !filter.city }" @click.stop="toggleDropdown('district')">
          <span class="select-value" :class="{ placeholder: !filter.district }">
            {{ getSelectedName(districtOptions, filter.district) || '请选择区县' }}
          </span>
          <span v-if="filter.district" class="clear-icon" @click.stop="clearDistrict">✕</span>
          <span class="arrow-down" :class="{ rotated: openDropdown === 'district' }">▼</span>
        </div>
        <div v-if="openDropdown === 'district'" class="select-dropdown">
          <div class="dropdown-header">选择区县</div>
          <div class="dropdown-list">
            <div v-for="d in districtOptions" :key="d.code"
              class="dropdown-item" :class="{ selected: filter.district === d.code }"
              @click.stop="selectOption('district', d.code)">
              {{ d.name }}
              <span v-if="filter.district === d.code" class="check-mark">✓</span>
            </div>
            <div v-if="districtOptions.length === 0" class="dropdown-empty">请先选择城市</div>
          </div>
        </div>
      </div>

      <div class="filter-select" ref="schoolRef" :class="{ 'has-value': filter.school, 'is-open': openDropdown === 'school', 'disabled': !filter.district }">
        <div class="select-trigger" :class="{ disabled: !filter.district }" @click.stop="toggleDropdown('school')">
          <span class="select-value" :class="{ placeholder: !filter.school }">
            {{ getSelectedName(schoolOptions, filter.school) || '请选择学校' }}
          </span>
          <span v-if="filter.school" class="clear-icon" @click.stop="clearSchool">✕</span>
          <span class="arrow-down" :class="{ rotated: openDropdown === 'school' }">▼</span>
        </div>
        <div v-if="openDropdown === 'school'" class="select-dropdown">
          <div class="dropdown-header">选择学校</div>
          <div class="dropdown-list">
            <div v-for="s in schoolOptions" :key="s.code"
              class="dropdown-item" :class="{ selected: filter.school === s.code }"
              @click.stop="selectOption('school', s.code)">
              {{ s.name }}
              <span v-if="filter.school === s.code" class="check-mark">✓</span>
            </div>
            <div v-if="schoolOptions.length === 0" class="dropdown-empty">请先选择区县</div>
          </div>
        </div>
      </div>

      <button class="btn-search-top" @click="handleSearch">
        <span class="icon">🔍</span> 搜索
      </button>
      <button class="btn-reset-top" @click="handleResetFilter">
        <span class="icon">↺</span> 重置
      </button>
    </div>

    <!-- 主体 -->
    <main class="national-body">
      <!-- 左栏 -->
      <aside class="left-col">
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">全国核心数据</span>
            <span class="panel-tag">{{ currentMetric.name }}专题</span>
          </header>
          <div class="tech-panel-body">
            <div class="kpi-grid">
              <div class="kpi-row-large">
                <div class="kpi-card-lg">
                  <div class="kpi-card-deco"></div>
                  <div class="kpi-card-hex"><span></span><span></span><span></span></div>
                  <div class="kpi-lg-label">全国{{ currentMetric.name }}率</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ currentNationData.rate }}</span><span class="unit">%</span>
                  </div>
                  <div :class="['kpi-lg-trend', currentNationData.trend > 0 ? 'up' : 'down']">
                    <span class="trend-arrow">{{ currentNationData.trend > 0 ? '▲' : '▼' }}</span>
                    {{ Math.abs(currentNationData.trend) }}% 同比
                  </div>
                </div>
                <div class="kpi-card-lg">
                  <div class="kpi-card-deco"></div>
                  <div class="kpi-card-hex"><span></span><span></span><span></span></div>
                  <div class="kpi-lg-label">学生总数（万）</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ currentNationData.students }}</span>
                  </div>
                  <div class="kpi-lg-trend down">覆盖 {{ currentNationData.cities }} 城市</div>
                </div>
              </div>

              <div class="kpi-row-gender">
                <div class="gender-item male">
                  <span class="gender-icon">♂</span>
                  <div class="gender-info">
                    <span class="gender-label">男生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ currentNationData.maleRate || 55.2 }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: (currentNationData.maleRate || 55.2) + '%' }"></div></div>
                </div>
                <div class="gender-item female">
                  <span class="gender-icon">♀</span>
                  <div class="gender-info">
                    <span class="gender-label">女生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ currentNationData.femaleRate || 42.2 }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: (currentNationData.femaleRate || 42.2) + '%' }"></div></div>
                </div>
              </div>

              <div class="kpi-row-small">
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">小学{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentNationData.primary }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentNationData.primary + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">初中{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentNationData.junior }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentNationData.junior + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">高中{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentNationData.senior }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentNationData.senior + '%' }"></div></div>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">城市{{ currentMetric.name }}率排名</span>
            <span class="panel-tag">TOP 10</span>
          </header>
          <div class="tech-panel-body">
            <div ref="cityRankRef" class="chart-area"></div>
          </div>
        </section>
      </aside>

      <!-- 地图 -->
      <section class="map-col">
        <div class="map-area">
          <div ref="mapRef" class="map-canvas"></div>
          <div v-if="!mapLoaded" class="map-loading">
            <div class="map-radar"></div>
            <span class="map-loading-text">地图加载中…</span>
          </div>
          <div class="map-deco" :class="{ 'map-deco-ready': mapLoaded }">
            <div class="map-grid"></div>
            <div class="map-corner mc-tl"></div>
            <div class="map-corner mc-tr"></div>
            <div class="map-corner mc-bl"></div>
            <div class="map-corner mc-br"></div>
            <div class="map-data-strip">
              <span>经度：<b>{{ mapCenter[0].toFixed(1) }}°E</b></span>
              <span>纬度：<b>{{ mapCenter[1].toFixed(1) }}°N</b></span>
              <span>缩放：<b>{{ mapZoom.toFixed(2) }}x</b></span>
              <span v-if="mapLoaded" class="map-drill-tip">点击省份下钻 ›</span>
              <span v-else>加载中…</span>
            </div>
          </div>
          <div class="map-legend">
            <div class="legend-title">{{ currentMetric.name }}率分布</div>
            <div class="legend-gradient-bar"></div>
            <div class="legend-scale">
              <span>0%</span>
              <span>48%</span>
              <span>55%</span>
              <span>60%</span>
              <span>100%</span>
            </div>
            <div class="legend-items">
              <div class="legend-item"><span class="legend-bar bar-1"></span><span>≤48%</span></div>
              <div class="legend-item"><span class="legend-bar bar-2"></span><span>48-55%</span></div>
              <div class="legend-item"><span class="legend-bar bar-3"></span><span>55-60%</span></div>
              <div class="legend-item"><span class="legend-bar bar-4"></span><span>≥60%</span></div>
            </div>
          </div>
        </div>
      </section>

      <!-- 右栏 -->
      <aside class="right-col">
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">年龄/性别{{ currentMetric.name }}率对比</span>
          </header>
          <div class="tech-panel-body">
            <div ref="ageGenderRef" class="chart-area"></div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">{{ currentMetric.name }}率城乡对比</span>
          </header>
          <div class="tech-panel-body">
            <div class="gauge-row">
              <div ref="urbanGaugeRef" class="gauge-item"></div>
              <div ref="ruralGaugeRef" class="gauge-item"></div>
            </div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">示范区{{ currentMetric.name }}防控效果</span>
          </header>
          <div class="tech-panel-body">
            <div ref="interventionRef" class="chart-area"></div>
          </div>
        </section>
      </aside>
    </main>

    </div>
</template>

<script setup>
import { ref, reactive, computed, watch, onMounted, onBeforeUnmount, nextTick } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import * as echarts from 'echarts';

const route = useRoute();
const router = useRouter();

// ============================================================
//  色板（深浅色主题）
// ============================================================
const DARK_COLOR = {
  primary: '#0bc4e9',
  secondary: '#59ebe8',
  accent: '#00a8d7',
  warning: '#fbbf24',
  success: '#34d399',
  danger: '#fb7185',
  male: '#0bc4e9',
  female: '#59ebe8',
  text: '#e2e8f0',
  textDim: '#94a3b8',
  bg: 'rgba(6, 16, 28, 0.96)',
  mapArea: 'rgba(15, 30, 65, 0.85)',
  mapBorder: 'rgba(11, 196, 233, 0.75)',
  mapLabel: '#e2e8f0',
  splitLine: 'rgba(11, 196, 233, 0.12)',
  gaugeTrack: 'rgba(11, 196, 233, 0.12)',
  initialBar: '#475569',
  mapColors: ['#0bc4e9', '#7dd87d', '#f5c542', '#e8554f'],
  mapGradient: ['#0bc4e9', '#7dd87d', '#f5c542', '#e8554f'],
};

const LIGHT_COLOR = {
  primary: '#0891b2',
  secondary: '#00a8d7',
  accent: '#0bc4e9',
  warning: '#d97706',
  success: '#059669',
  danger: '#dc2626',
  male: '#0891b2',
  female: '#00a8d7',
  text: '#1e293b',
  textDim: '#64748b',
  bg: 'rgba(255, 255, 255, 0.98)',
  mapArea: '#f1f5f9',
  mapBorder: 'rgba(8, 145, 178, 0.35)',
  mapLabel: '#1e293b',
  splitLine: 'rgba(8, 145, 178, 0.1)',
  gaugeTrack: 'rgba(8, 145, 178, 0.08)',
  initialBar: '#cbd5e1',
  mapColors: ['#4dd4f5', '#a8e8a8', '#f7d56e', '#ed807d'],
  mapGradient: ['#4dd4f5', '#a8e8a8', '#f7d56e', '#ed807d'],
};

const PROVINCE_CODES = {
  北京: '110000', 天津: '120000', 河北: '130000', 山西: '140000', 内蒙古: '150000',
  辽宁: '210000', 吉林: '220000', 黑龙江: '230000', 上海: '310000', 江苏: '320000',
  浙江: '330000', 安徽: '340000', 福建: '350000', 江西: '360000', 山东: '370000',
  河南: '410000', 湖北: '420000', 湖南: '430000', 广东: '440000', 广西: '450000',
  海南: '460000', 重庆: '500000', 四川: '510000', 贵州: '520000', 云南: '530000',
  西藏: '540000', 陕西: '610000', 甘肃: '620000', 青海: '630000', 宁夏: '640000',
  新疆: '650000', 台湾: '710000', 香港: '810000', 澳门: '820000',
};

const CITY_CODES = {
  济南: '370100', 青岛: '370200', 烟台: '370600', 德州: '371400', 泰安: '370900',
  菏泽: '371700', 聊城: '371500', 济宁: '370800', 枣庄: '370400', 淄博: '370300',
};

const GEO_SUFFIXES = /(省|市|自治区|壮族|回族|维吾尔|特别行政区)$/;

const buildProvinceNameMap = () => {
  const map = {};
  Object.keys(PROVINCE_CODES).forEach((name) => {
    map[`${name}省`] = name;
    map[`${name}市`] = name;
    map[`${name}自治区`] = name;
    map[`${name}壮族自治区`] = name;
    map[`${name}回族自治区`] = name;
    map[`${name}维吾尔自治区`] = name;
    map[`${name}特别行政区`] = name;
    map[name] = name;
  });
  map['内蒙古自治区'] = '内蒙古';
  map['广西壮族自治区'] = '广西';
  map['宁夏回族自治区'] = '宁夏';
  map['新疆维吾尔自治区'] = '新疆';
  map['西藏自治区'] = '西藏';
  return map;
};

const PROVINCE_NAME_MAP = buildProvinceNameMap();

const getColors = () => (isLight.value ? LIGHT_COLOR : DARK_COLOR);

const makeGrad = (c1, c2, horizontal = true) =>
  new echarts.graphic.LinearGradient(0, 0, horizontal ? 1 : 0, horizontal ? 0 : 1, [
    { offset: 0, color: c1 },
    { offset: 1, color: c2 },
  ]);

// ============================================================
//  Tabs
// ============================================================
const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️' },
  { key: 'oral',   label: '口腔健康', icon: '🦷' },
  { key: 'mental', label: '心理健康', icon: '🧠' },
  { key: 'weight', label: '健康体重', icon: '⚖️' },
  { key: 'bone',   label: '骨骼健康', icon: '🦴' }
];

// ============================================================
//  顶部筛选
// ============================================================
const filter = reactive({
  province: '',
  city: '',
  district: '',
  school: '',
});

const openDropdown = ref(null);

const toggleDropdown = (key) => {
  if (key === 'city' && !filter.province) return;
  if (key === 'district' && !filter.city) return;
  if (key === 'school' && !filter.district) return;
  openDropdown.value = openDropdown.value === key ? null : key;
};

const closeDropdowns = () => {
  openDropdown.value = null;
};

const selectOption = (key, value) => {
  filter[key] = value;
  openDropdown.value = null;
  if (key === 'province') onProvinceChange();
  else if (key === 'city') onCityChange();
  else if (key === 'district') onDistrictChange();
};

const getSelectedName = (list, code) => {
  if (!code) return '';
  const found = list.find(item => item.code === code);
  return found ? found.name : '';
};

const clearSchool = () => {
  filter.school = '';
};

const handleDocumentClick = (e) => {
  if (!e.target.closest('.filter-select')) {
    openDropdown.value = null;
  }
};

const provinceList = computed(() => {
  return Object.entries(PROVINCE_CODES).map(([name, code]) => ({
    code,
    name: name + (['北京', '天津', '上海', '重庆'].includes(name) ? '市' : ['内蒙古', '广西', '西藏', '宁夏', '新疆'].includes(name) ? '' : '省'),
  }));
});

const PROVINCE_CITY_MAP = {
  '370000': [
    { code: '370100', name: '济南市' }, { code: '370200', name: '青岛市' },
    { code: '370300', name: '淄博市' }, { code: '370400', name: '枣庄市' },
    { code: '370500', name: '东营市' }, { code: '370600', name: '烟台市' },
    { code: '370700', name: '潍坊市' }, { code: '370800', name: '济宁市' },
    { code: '370900', name: '泰安市' }, { code: '371000', name: '威海市' },
    { code: '371100', name: '日照市' }, { code: '371300', name: '临沂市' },
    { code: '371400', name: '德州市' }, { code: '371500', name: '聊城市' },
    { code: '371600', name: '滨州市' }, { code: '371700', name: '菏泽市' },
  ],
  '320000': [
    { code: '320100', name: '南京市' }, { code: '320200', name: '无锡市' },
    { code: '320300', name: '徐州市' }, { code: '320500', name: '苏州市' },
    { code: '320600', name: '南通市' }, { code: '320700', name: '连云港市' },
    { code: '320800', name: '淮安市' }, { code: '320900', name: '盐城市' },
    { code: '321000', name: '扬州市' }, { code: '321100', name: '镇江市' },
    { code: '321200', name: '泰州市' }, { code: '321300', name: '宿迁市' },
  ],
  '330000': [
    { code: '330100', name: '杭州市' }, { code: '330200', name: '宁波市' },
    { code: '330300', name: '温州市' }, { code: '330400', name: '嘉兴市' },
    { code: '330500', name: '湖州市' }, { code: '330600', name: '绍兴市' },
    { code: '330700', name: '金华市' }, { code: '330800', name: '衢州市' },
    { code: '330900', name: '舟山市' }, { code: '331000', name: '台州市' },
    { code: '331100', name: '丽水市' },
  ],
  '340000': [
    { code: '340100', name: '合肥市' }, { code: '340200', name: '芜湖市' },
    { code: '340300', name: '蚌埠市' }, { code: '340400', name: '淮南市' },
    { code: '340500', name: '马鞍山市' }, { code: '340600', name: '淮北市' },
    { code: '340700', name: '铜陵市' }, { code: '340800', name: '安庆市' },
    { code: '341000', name: '黄山市' }, { code: '341100', name: '滁州市' },
    { code: '341200', name: '阜阳市' }, { code: '341300', name: '宿州市' },
    { code: '341500', name: '六安市' }, { code: '341600', name: '亳州市' },
    { code: '341700', name: '池州市' }, { code: '341800', name: '宣城市' },
  ],
  '350000': [
    { code: '350100', name: '福州市' }, { code: '350200', name: '厦门市' },
    { code: '350300', name: '莆田市' }, { code: '350400', name: '三明市' },
    { code: '350500', name: '泉州市' }, { code: '350600', name: '漳州市' },
    { code: '350700', name: '南平市' }, { code: '350800', name: '龙岩市' },
    { code: '350900', name: '宁德市' },
  ],
  '360000': [
    { code: '360100', name: '南昌市' }, { code: '360200', name: '景德镇市' },
    { code: '360300', name: '萍乡市' }, { code: '360400', name: '九江市' },
    { code: '360500', name: '新余市' }, { code: '360600', name: '鹰潭市' },
    { code: '360700', name: '赣州市' }, { code: '360800', name: '吉安市' },
    { code: '360900', name: '宜春市' }, { code: '361000', name: '抚州市' },
    { code: '361100', name: '上饶市' },
  ],
  '410000': [
    { code: '410100', name: '郑州市' }, { code: '410200', name: '开封市' },
    { code: '410300', name: '洛阳市' }, { code: '410400', name: '平顶山市' },
    { code: '410500', name: '安阳市' }, { code: '410600', name: '鹤壁市' },
    { code: '410700', name: '新乡市' }, { code: '410800', name: '焦作市' },
    { code: '410900', name: '濮阳市' }, { code: '411000', name: '许昌市' },
    { code: '411100', name: '漯河市' }, { code: '411200', name: '三门峡市' },
    { code: '411300', name: '南阳市' }, { code: '411400', name: '商丘市' },
    { code: '411500', name: '信阳市' }, { code: '411600', name: '周口市' },
    { code: '411700', name: '驻马店市' },
  ],
  '310000': [{ code: '310000', name: '上海市' }],
  '110000': [{ code: '110000', name: '北京市' }],
  '120000': [{ code: '120000', name: '天津市' }],
  '500000': [{ code: '500000', name: '重庆市' }],
  '440000': [
    { code: '440100', name: '广州市' }, { code: '440300', name: '深圳市' },
    { code: '440400', name: '珠海市' }, { code: '440500', name: '汕头市' },
    { code: '440600', name: '佛山市' }, { code: '440700', name: '江门市' },
    { code: '440800', name: '湛江市' }, { code: '440900', name: '茂名市' },
    { code: '441200', name: '肇庆市' }, { code: '441300', name: '惠州市' },
    { code: '441400', name: '梅州市' }, { code: '441500', name: '汕尾市' },
    { code: '441600', name: '河源市' }, { code: '441700', name: '阳江市' },
    { code: '441800', name: '清远市' }, { code: '441900', name: '东莞市' },
    { code: '442000', name: '中山市' }, { code: '445100', name: '潮州市' },
    { code: '445200', name: '揭阳市' }, { code: '445300', name: '云浮市' },
  ],
  '420000': [
    { code: '420100', name: '武汉市' }, { code: '420200', name: '黄石市' },
    { code: '420300', name: '十堰市' }, { code: '420500', name: '宜昌市' },
    { code: '420600', name: '襄阳市' }, { code: '420700', name: '鄂州市' },
    { code: '420800', name: '荆门市' }, { code: '420900', name: '孝感市' },
    { code: '421000', name: '荆州市' }, { code: '421100', name: '黄冈市' },
    { code: '421200', name: '咸宁市' }, { code: '421300', name: '随州市' },
  ],
  '430000': [
    { code: '430100', name: '长沙市' }, { code: '430200', name: '株洲市' },
    { code: '430300', name: '湘潭市' }, { code: '430400', name: '衡阳市' },
    { code: '430500', name: '邵阳市' }, { code: '430600', name: '岳阳市' },
    { code: '430700', name: '常德市' }, { code: '430800', name: '张家界市' },
    { code: '430900', name: '益阳市' }, { code: '431000', name: '郴州市' },
    { code: '431100', name: '永州市' }, { code: '431200', name: '怀化市' },
    { code: '431300', name: '娄底市' },
  ],
  '510000': [
    { code: '510100', name: '成都市' }, { code: '510300', name: '自贡市' },
    { code: '510400', name: '攀枝花市' }, { code: '510500', name: '泸州市' },
    { code: '510600', name: '德阳市' }, { code: '510700', name: '绵阳市' },
    { code: '510800', name: '广元市' }, { code: '510900', name: '遂宁市' },
    { code: '511000', name: '内江市' }, { code: '511100', name: '乐山市' },
    { code: '511300', name: '南充市' }, { code: '511400', name: '眉山市' },
    { code: '511500', name: '宜宾市' }, { code: '511600', name: '广安市' },
    { code: '511700', name: '达州市' }, { code: '511800', name: '雅安市' },
    { code: '511900', name: '巴中市' }, { code: '512000', name: '资阳市' },
  ],
  '610000': [
    { code: '610100', name: '西安市' }, { code: '610200', name: '铜川市' },
    { code: '610300', name: '宝鸡市' }, { code: '610400', name: '咸阳市' },
    { code: '610500', name: '渭南市' }, { code: '610600', name: '延安市' },
    { code: '610700', name: '汉中市' }, { code: '610800', name: '榆林市' },
    { code: '610900', name: '安康市' }, { code: '611000', name: '商洛市' },
  ],
  '210000': [
    { code: '210100', name: '沈阳市' }, { code: '210200', name: '大连市' },
    { code: '210300', name: '鞍山市' }, { code: '210400', name: '抚顺市' },
    { code: '210500', name: '本溪市' }, { code: '210600', name: '丹东市' },
    { code: '210700', name: '锦州市' }, { code: '210800', name: '营口市' },
    { code: '210900', name: '阜新市' }, { code: '211000', name: '辽阳市' },
    { code: '211100', name: '盘锦市' }, { code: '211200', name: '铁岭市' },
    { code: '211300', name: '朝阳市' }, { code: '211400', name: '葫芦岛市' },
  ],
  '220000': [
    { code: '220100', name: '长春市' }, { code: '220200', name: '吉林市' },
    { code: '220300', name: '四平市' }, { code: '220400', name: '辽源市' },
    { code: '220500', name: '通化市' }, { code: '220600', name: '白山市' },
    { code: '220700', name: '松原市' }, { code: '220800', name: '白城市' },
    { code: '222400', name: '延边朝鲜族自治州' },
  ],
  '230000': [
    { code: '230100', name: '哈尔滨市' }, { code: '230200', name: '齐齐哈尔市' },
    { code: '230300', name: '鸡西市' }, { code: '230400', name: '鹤岗市' },
    { code: '230500', name: '双鸭山市' }, { code: '230600', name: '大庆市' },
    { code: '230700', name: '伊春市' }, { code: '230800', name: '佳木斯市' },
    { code: '230900', name: '七台河市' }, { code: '231000', name: '牡丹江市' },
    { code: '231100', name: '黑河市' }, { code: '231200', name: '绥化市' },
  ],
  '130000': [
    { code: '130100', name: '石家庄市' }, { code: '130200', name: '唐山市' },
    { code: '130300', name: '秦皇岛市' }, { code: '130400', name: '邯郸市' },
    { code: '130500', name: '邢台市' }, { code: '130600', name: '保定市' },
    { code: '130700', name: '张家口市' }, { code: '130800', name: '承德市' },
    { code: '130900', name: '沧州市' }, { code: '131000', name: '廊坊市' },
    { code: '131100', name: '衡水市' },
  ],
  '140000': [
    { code: '140100', name: '太原市' }, { code: '140200', name: '大同市' },
    { code: '140300', name: '阳泉市' }, { code: '140400', name: '长治市' },
    { code: '140500', name: '晋城市' }, { code: '140600', name: '朔州市' },
    { code: '140700', name: '晋中市' }, { code: '140800', name: '运城市' },
    { code: '140900', name: '忻州市' }, { code: '141000', name: '临汾市' },
    { code: '141100', name: '吕梁市' },
  ],
  '150000': [
    { code: '150100', name: '呼和浩特市' }, { code: '150200', name: '包头市' },
    { code: '150300', name: '乌海市' }, { code: '150400', name: '赤峰市' },
    { code: '150500', name: '通辽市' }, { code: '150600', name: '鄂尔多斯市' },
    { code: '150700', name: '呼伦贝尔市' }, { code: '150800', name: '巴彦淖尔市' },
    { code: '150900', name: '乌兰察布市' },
  ],
  '450000': [
    { code: '450100', name: '南宁市' }, { code: '450200', name: '柳州市' },
    { code: '450300', name: '桂林市' }, { code: '450400', name: '梧州市' },
    { code: '450500', name: '北海市' }, { code: '450600', name: '防城港市' },
    { code: '450700', name: '钦州市' }, { code: '450800', name: '贵港市' },
    { code: '450900', name: '玉林市' }, { code: '451000', name: '百色市' },
    { code: '451100', name: '贺州市' }, { code: '451200', name: '河池市' },
    { code: '451300', name: '来宾市' }, { code: '451400', name: '崇左市' },
  ],
  '460000': [
    { code: '460100', name: '海口市' }, { code: '460200', name: '三亚市' },
    { code: '460300', name: '三沙市' }, { code: '460400', name: '儋州市' },
  ],
  '520000': [
    { code: '520100', name: '贵阳市' }, { code: '520200', name: '六盘水市' },
    { code: '520300', name: '遵义市' }, { code: '520400', name: '安顺市' },
    { code: '520500', name: '毕节市' }, { code: '520600', name: '铜仁市' },
  ],
  '530000': [
    { code: '530100', name: '昆明市' }, { code: '530300', name: '曲靖市' },
    { code: '530400', name: '玉溪市' }, { code: '530500', name: '保山市' },
    { code: '530600', name: '昭通市' }, { code: '530700', name: '丽江市' },
    { code: '530800', name: '普洱市' }, { code: '530900', name: '临沧市' },
  ],
  '620000': [
    { code: '620100', name: '兰州市' }, { code: '620200', name: '嘉峪关市' },
    { code: '620300', name: '金昌市' }, { code: '620400', name: '白银市' },
    { code: '620500', name: '天水市' }, { code: '620600', name: '武威市' },
    { code: '620700', name: '张掖市' }, { code: '620800', name: '平凉市' },
    { code: '620900', name: '酒泉市' }, { code: '621000', name: '庆阳市' },
    { code: '621100', name: '定西市' }, { code: '621200', name: '陇南市' },
  ],
  '630000': [
    { code: '630100', name: '西宁市' }, { code: '630200', name: '海东市' },
  ],
  '640000': [
    { code: '640100', name: '银川市' }, { code: '640200', name: '石嘴山市' },
    { code: '640300', name: '吴忠市' }, { code: '640400', name: '固原市' },
    { code: '640500', name: '中卫市' },
  ],
  '650000': [
    { code: '650100', name: '乌鲁木齐市' }, { code: '650200', name: '克拉玛依市' },
    { code: '650400', name: '吐鲁番市' }, { code: '650500', name: '哈密市' },
  ],
  '540000': [
    { code: '540100', name: '拉萨市' }, { code: '540200', name: '日喀则市' },
    { code: '540300', name: '昌都市' }, { code: '540400', name: '林芝市' },
    { code: '540500', name: '山南市' }, { code: '540600', name: '那曲市' },
  ],
};

const cityOptions = computed(() => PROVINCE_CITY_MAP[filter.province] || []);

const DISTRICT_FILTER_MAP = {
  '110000': ['东城区', '西城区', '朝阳区', '海淀区', '丰台区', '石景山区', '通州区', '昌平区', '大兴区'],
  '120000': ['和平区', '河东区', '河西区', '南开区', '河北区', '红桥区', '滨海新区'],
  '310000': ['黄浦区', '徐汇区', '长宁区', '静安区', '浦东新区', '闵行区', '普陀区', '虹口区', '杨浦区', '嘉定区'],
  '500000': ['渝中区', '江北区', '南岸区', '九龙坡区', '沙坪坝区', '大渡口区', '北碚区', '渝北区', '巴南区'],
  '370100': ['历下区', '市中区', '槐荫区', '天桥区', '历城区', '长清区', '章丘区', '济阳区'],
  '370200': ['市南区', '市北区', '黄岛区', '崂山区', '李沧区', '城阳区', '即墨区'],
  '370300': ['张店区', '淄川区', '博山区', '临淄区', '周村区'],
  '370400': ['薛城区', '市中区', '峄城区', '台儿庄区'],
  '370500': ['东营区', '河口区', '垦利区'],
  '370600': ['芝罘区', '福山区', '牟平区', '莱山区'],
  '370700': ['潍城区', '奎文区', '坊子区', '寒亭区'],
  '370800': ['任城区', '兖州区', '邹城市', '曲阜市'],
  '370900': ['泰山区', '岱岳区', '新泰市', '肥城市'],
  '371000': ['环翠区', '文登区', '荣成市', '乳山市'],
  '371100': ['东港区', '岚山区', '莒县'],
  '371300': ['兰山区', '罗庄区', '河东区'],
  '371400': ['德城区', '陵城区', '禹城市', '乐陵市'],
  '371500': ['东昌府区', '茌平区', '临清市'],
  '371600': ['滨城区', '沾化区', '邹平市'],
  '371700': ['牡丹区', '定陶区', '曹县', '单县', '巨野县'],
  '320100': ['玄武区', '秦淮区', '建邺区', '鼓楼区', '浦口区', '栖霞区', '雨花台区', '江宁区'],
  '320200': ['锡山区', '惠山区', '滨湖区', '梁溪区', '新吴区'],
  '320300': ['云龙区', '鼓楼区', '泉山区', '铜山区'],
  '320500': ['姑苏区', '虎丘区', '吴中区', '相城区', '吴江区'],
  '320600': ['崇川区', '通州区', '海门区'],
  '320700': ['海州区', '连云区', '赣榆区'],
  '320800': ['清江浦区', '淮安区', '淮阴区', '洪泽区'],
  '320900': ['亭湖区', '盐都区', '大丰区'],
  '321000': ['广陵区', '邗江区', '江都区'],
  '321100': ['京口区', '润州区', '丹徒区'],
  '321200': ['海陵区', '高港区'],
  '321300': ['宿城区', '宿豫区'],
  '330100': ['上城区', '下城区', '江干区', '拱墅区', '西湖区', '滨江区', '萧山区', '余杭区'],
  '330200': ['海曙区', '江北区', '北仑区', '镇海区', '鄞州区'],
  '330300': ['鹿城区', '龙湾区', '瓯海区', '洞头区'],
  '330400': ['南湖区', '秀洲区'],
  '330500': ['吴兴区', '南浔区'],
  '330600': ['越城区', '柯桥区', '上虞区'],
  '330700': ['婺城区', '金东区'],
  '330800': ['柯城区', '衢江区'],
  '330900': ['定海区', '普陀区'],
  '331000': ['椒江区', '黄岩区', '路桥区'],
  '331100': ['莲都区'],
  '340100': ['瑶海区', '庐阳区', '蜀山区', '包河区', '高新区'],
  '340200': ['镜湖区', '弋江区', '鸠江区', '三山区'],
  '340300': ['龙子湖区', '蚌山区', '禹会区', '淮上区'],
  '340400': ['田家庵区', '谢家集区', '八公山区', '潘集区'],
  '340500': ['花山区', '雨山区'],
  '340600': ['杜集区', '相山区'],
  '340700': ['铜官区', '郊区'],
  '340800': ['迎江区', '大观区', '宜秀区'],
  '341000': ['屯溪区', '黄山区', '徽州区'],
  '341100': ['琅琊区', '南谯区'],
  '341200': ['颍州区', '颍东区', '颍泉区'],
  '341300': ['埇桥区'],
  '341500': ['金安区', '裕安区'],
  '341600': ['谯城区'],
  '341700': ['贵池区'],
  '341800': ['宣州区'],
  '350100': ['鼓楼区', '台江区', '仓山区', '马尾区', '晋安区'],
  '350200': ['思明区', '海沧区', '湖里区', '集美区', '同安区'],
  '350300': ['城厢区', '涵江区', '荔城区'],
  '350400': ['三元区', '沙县区'],
  '350500': ['鲤城区', '丰泽区', '洛江区', '泉港区'],
  '350600': ['芗城区', '龙文区'],
  '350700': ['延平区', '建阳区'],
  '350800': ['新罗区', '永定区'],
  '350900': ['蕉城区', '福安市'],
  '360100': ['东湖区', '西湖区', '青云谱区', '青山湖区', '新建区'],
  '360200': ['珠山区', '昌江区'],
  '360300': ['安源区', '湘东区'],
  '360400': ['濂溪区', '浔阳区', '庐山区'],
  '360500': ['渝水区', '分宜县'],
  '360600': ['月湖区'],
  '360700': ['章贡区', '南康区', '赣县区'],
  '360800': ['吉州区', '青原区'],
  '360900': ['袁州区'],
  '361000': ['临川区'],
  '361100': ['信州区'],
  '410100': ['中原区', '二七区', '管城回族区', '金水区', '上街区', '惠济区'],
  '410200': ['龙亭区', '顺河回族区', '鼓楼区', '禹王台区'],
  '410300': ['老城区', '西工区', '瀍河回族区', '涧西区', '吉利区'],
  '410400': ['新华区', '卫东区', '湛河区', '石龙区'],
  '410500': ['文峰区', '北关区', '殷都区', '龙安区'],
  '410600': ['鹤山区', '山城区'],
  '410700': ['红旗区', '卫滨区', '凤泉区', '牧野区'],
  '410800': ['解放区', '中站区', '马村区', '山阳区'],
  '410900': ['华龙区', '濮阳县'],
  '411000': ['魏都区'],
  '411100': ['源汇区', '郾城区'],
  '411200': ['湖滨区'],
  '411300': ['宛城区', '卧龙区'],
  '411400': ['梁园区', '睢阳区'],
  '411500': ['浉河区', '平桥区'],
  '411600': ['川汇区'],
  '411700': ['驿城区'],
  '420100': ['江岸区', '江汉区', '硚口区', '汉阳区', '武昌区', '洪山区', '青山区', '东西湖区'],
  '420200': ['黄石港区', '西塞山区', '下陆区', '铁山区'],
  '420300': ['茅箭区', '张湾区'],
  '420500': ['西陵区', '伍家岗区', '点军区', '猇亭区'],
  '420600': ['襄城区', '樊城区'],
  '420700': ['鄂城区'],
  '420800': ['东宝区', '掇刀区'],
  '420900': ['孝南区'],
  '421000': ['沙市区', '荆州区'],
  '421100': ['黄州区'],
  '421200': ['咸安区'],
  '421300': ['曾都区'],
  '430100': ['芙蓉区', '天心区', '岳麓区', '开福区', '雨花区', '望城区'],
  '430200': ['荷塘区', '芦淞区', '石峰区', '天元区'],
  '430300': ['雨湖区', '岳塘区'],
  '430400': ['珠晖区', '雁峰区', '石鼓区', '蒸湘区'],
  '430500': ['双清区', '大祥区', '北塔区'],
  '430600': ['岳阳楼区', '云溪区'],
  '430700': ['武陵区', '鼎城区'],
  '430800': ['永定区', '武陵源区'],
  '430900': ['赫山区', '资阳区'],
  '431000': ['北湖区', '苏仙区'],
  '431100': ['零陵区', '冷水滩区'],
  '431200': ['鹤城区'],
  '431300': ['娄星区', '涟源市'],
  '440100': ['越秀区', '海珠区', '荔湾区', '天河区', '白云区', '黄埔区', '番禺区', '花都区'],
  '440300': ['福田区', '罗湖区', '南山区', '宝安区', '龙岗区', '盐田区', '龙华区'],
  '440400': ['香洲区', '斗门区', '金湾区'],
  '440500': ['金平区', '龙湖区', '濠江区', '潮南区'],
  '440600': ['禅城区', '南海区', '顺德区', '三水区', '高明区'],
  '440700': ['蓬江区', '江海区', '新会区'],
  '440800': ['赤坎区', '霞山区', '坡头区', '麻章区'],
  '440900': ['茂南区', '电白区'],
  '441200': ['端州区', '鼎湖区'],
  '441300': ['惠城区', '惠阳区'],
  '441400': ['梅江区', '梅县区'],
  '441500': ['城区', '红海湾'],
  '441600': ['源城区'],
  '441700': ['江城区', '阳东区'],
  '441800': ['清城区', '清新区'],
  '441900': ['莞城街道', '南城街道', '东城街道', '万江街道'],
  '442000': ['石岐街道', '东区街道', '西区街道'],
  '445100': ['湘桥区', '潮安区'],
  '445200': ['榕城区', '揭东区'],
  '445300': ['云城区'],
  '450100': ['兴宁区', '青秀区', '江南区', '西乡塘区'],
  '450200': ['城中区', '鱼峰区', '柳南区', '柳北区'],
  '450300': ['秀峰区', '叠彩区', '象山区', '七星区', '雁山区'],
  '450400': ['万秀区', '长洲区'],
  '450500': ['海城区', '银海区'],
  '450600': ['港口区', '防城区'],
  '450700': ['钦南区', '钦北区'],
  '450800': ['港北区', '港南区'],
  '450900': ['玉州区', '福绵区'],
  '451000': ['右江区'],
  '451100': ['八步区'],
  '451200': ['金城江区'],
  '451300': ['兴宾区'],
  '451400': ['江州区'],
  '460100': ['秀英区', '龙华区', '琼山区', '美兰区'],
  '460200': ['海棠区', '吉阳区', '天涯区', '崖州区'],
  '460300': ['吉阳区', '天涯区'],
  '460400': ['那大镇', '白马井镇'],
  '510100': ['锦江区', '青羊区', '金牛区', '武侯区', '成华区', '龙泉驿区', '高新区'],
  '510300': ['自流井区', '贡井区', '大安区', '沿滩区'],
  '510400': ['东区', '西区', '仁和区'],
  '510500': ['江阳区', '纳溪区', '龙马潭区'],
  '510600': ['旌阳区', '罗江区'],
  '510700': ['涪城区', '游仙区'],
  '510800': ['利州区', '元坝区'],
  '510900': ['船山区', '安居区'],
  '511000': ['市中区', '东兴区'],
  '511100': ['市中区', '沙湾区', '五通桥区', '金口河区'],
  '511300': ['顺庆区', '高坪区', '嘉陵区'],
  '511400': ['东坡区'],
  '511500': ['翠屏区', '南溪区'],
  '511600': ['广安区', '前锋区'],
  '511700': ['通川区', '达川区'],
  '511800': ['雨城区', '名山区'],
  '511900': ['巴州区', '恩阳区'],
  '512000': ['雁江区'],
  '520100': ['南明区', '云岩区', '花溪区', '乌当区', '白云区', '观山湖区'],
  '520200': ['钟山区', '水城区'],
  '520300': ['红花岗区', '汇川区'],
  '520400': ['西秀区'],
  '520500': ['七星关区'],
  '520600': ['碧江区'],
  '530100': ['五华区', '盘龙区', '官渡区', '西山区', '呈贡区'],
  '530300': ['麒麟区'],
  '530400': ['红塔区'],
  '530500': ['隆阳区'],
  '530600': ['昭阳区'],
  '530700': ['古城区'],
  '530800': ['思茅区'],
  '530900': ['临翔区'],
  '610100': ['新城区', '碑林区', '莲湖区', '灞桥区', '未央区', '雁塔区', '阎良区'],
  '610200': ['王益区', '印台区'],
  '610300': ['渭滨区', '金台区', '陈仓区'],
  '610400': ['秦都区', '杨陵区', '渭城区'],
  '610500': ['临渭区', '华州区'],
  '610600': ['宝塔区', '安塞区'],
  '610700': ['汉台区', '南郑区'],
  '610800': ['榆阳区', '横山区'],
  '610900': ['汉滨区'],
  '611000': ['商州区'],
  '620100': ['城关区', '七里河区', '西固区', '安宁区', '红古区'],
  '620200': ['长城区', '雄关区'],
  '620300': ['金川区'],
  '620400': ['白银区', '平川区'],
  '620500': ['秦州区', '麦积区'],
  '620600': ['凉州区'],
  '620700': ['甘州区'],
  '620800': ['崆峒区'],
  '620900': ['肃州区'],
  '621000': ['西峰区'],
  '621100': ['安定区'],
  '621200': ['武都区'],
  '630100': ['城东区', '城中区', '城西区', '城北区'],
  '630200': ['乐都区'],
  '640100': ['兴庆区', '西夏区', '金凤区'],
  '640200': ['大武口区', '惠农区'],
  '640300': ['利通区', '红寺堡区'],
  '640400': ['原州区'],
  '640500': ['沙坡头区'],
  '650100': ['天山区', '沙依巴克区', '新市区', '水磨沟区', '头屯河区'],
  '650200': ['克拉玛依区', '独山子区', '白碱滩区'],
  '650400': ['高昌区'],
  '650500': ['伊州区'],
  '210100': ['和平区', '沈河区', '大东区', '皇姑区', '铁西区', '苏家屯区', '浑南区'],
  '210200': ['中山区', '西岗区', '沙河口区', '甘井子区', '旅顺口区', '金州区'],
  '210300': ['铁东区', '铁西区', '立山区', '千山区'],
  '210400': ['新抚区', '东洲区', '望花区'],
  '210500': ['平山区', '明山区', '溪湖区'],
  '210600': ['元宝区', '振兴区', '振安区'],
  '210700': ['太和区', '古塔区', '凌河区'],
  '210800': ['站前区', '西市区', '鲅鱼圈区'],
  '210900': ['海州区', '新邱区', '太平区'],
  '211000': ['白塔区', '文圣区'],
  '211100': ['兴隆台区', '双台子区'],
  '211200': ['银州区', '清河区'],
  '211300': ['双塔区', '龙城区'],
  '211400': ['龙港区', '连山区'],
  '220100': ['南关区', '宽城区', '朝阳区', '二道区', '绿园区'],
  '220200': ['昌邑区', '龙潭区', '船营区', '丰满区'],
  '220300': ['铁西区', '铁东区'],
  '220400': ['西安区', '龙山区'],
  '220500': ['东昌区', '二道江区'],
  '220600': ['浑江区', '江源区'],
  '220700': ['宁江区'],
  '220800': ['洮北区'],
  '230100': ['道里区', '南岗区', '道外区', '平房区', '香坊区'],
  '230200': ['龙沙区', '建华区', '铁锋区', '昂昂溪区'],
  '230300': ['鸡冠区', '城子河区', '恒山区'],
  '230400': ['工农区', '南山区'],
  '230500': ['尖山区', '岭东区'],
  '230600': ['萨尔图区', '龙湖区', '大庆区'],
  '230700': ['伊春区', '南岔区'],
  '230800': ['前进区', '向阳区', '东风区', '郊区'],
  '230900': ['桃山区', '新兴区'],
  '231000': ['东安区', '西安区', '阳明区', '爱民区'],
  '231100': ['爱辉区'],
  '231200': ['北林区'],
  '130100': ['长安区', '桥西区', '新华区', '井陉矿区', '裕华区'],
  '130200': ['路南区', '路北区', '古冶区', '开平区', '丰南区'],
  '130300': ['海港区', '山海关区', '北戴河区'],
  '130400': ['邯山区', '丛台区', '复兴区'],
  '130500': ['襄都区', '信都区', '任泽区', '南和区'],
  '130600': ['竞秀区', '莲池区'],
  '130700': ['桥东区', '桥西区'],
  '130800': ['双桥区', '双滦区'],
  '130900': ['运河区', '新华区'],
  '131000': ['安次区', '广阳区'],
  '131100': ['桃城区'],
  '140100': ['小店区', '迎泽区', '杏花岭区', '尖草坪区', '万柏林区', '晋源区'],
  '140200': ['新荣区', '平城区', '云冈区'],
  '140300': ['城区'],
  '140400': ['潞州区', '上党区'],
  '140500': ['城区'],
  '140600': ['朔城区', '平鲁区'],
  '140700': ['榆次区', '太谷区'],
  '140800': ['盐湖区'],
  '140900': ['忻府区'],
  '141000': ['尧都区'],
  '141100': ['离石区'],
  '150100': ['新城区', '回民区', '玉泉区', '赛罕区'],
  '150200': ['昆都仑区', '东河区', '青山区', '九原区'],
  '150300': ['海勃湾区'],
  '150400': ['红山区', '松山区'],
  '150500': ['科尔沁区'],
  '150600': ['东胜区', '康巴什区'],
  '150700': ['海拉尔区'],
  '150800': ['临河区'],
  '150900': ['集宁区'],
};

const districtOptions = computed(() => {
  const list = DISTRICT_FILTER_MAP[filter.city];
  return list ? list.map((name, i) => ({ code: `${filter.city}${String(i + 1).padStart(2, '0')}`, name })) : [];
});

const SCHOOL_DATA = {
  '3701': ['历下区第一实验小学', '历下区第二中学', '济南市解放路第一小学', '山东大学附属中学', '济南外国语学校', '山东省实验中学'],
  '3702': ['青岛市实验小学', '青岛育才中学', '市南区实验小学', '青岛第二十六中学', '青岛二中', '青岛智荣中学'],
  '3201': ['南京外国语学校', '南京师范大学附属小学', '南京市第一中学', '金陵中学', '南京市游府西街小学'],
  '3202': ['无锡市第一中学', '江苏省天一中学', '无锡师范附属小学', '无锡大桥实验学校'],
  '3203': ['徐州市第一中学', '徐州市实验小学', '徐州师范大学附属中学', '徐州市第二十二中学'],
  '3205': ['苏州中学', '苏州外国语学校', '星海实验中学', '苏州工业园区星湾学校'],
  '3206': ['南通市第一中学', '南通师范附属小学', '南通市第二中学'],
  '3207': ['连云港市第一中学', '连云港师范附属小学'],
  '3208': ['淮安市第一中学', '淮安外国语学校'],
  '3209': ['盐城市第一中学', '盐城市实验小学'],
  '3210': ['扬州中学', '扬州外国语学校'],
  '3211': ['镇江市第一中学', '镇江市实验小学'],
  '3301': ['杭州第二中学', '杭州学军小学', '杭州外国语学校', '杭州高级中学', '绿城育华学校'],
  '3302': ['宁波中学', '宁波外国语学校', '宁波华茂外国语学校'],
  '3303': ['温州市第一中学', '温州市实验小学'],
  '3304': ['嘉兴市第一中学', '嘉兴市实验小学'],
  '3305': ['湖州中学', '湖州市实验小学'],
  '3306': ['绍兴市第一中学', '绍兴市实验小学'],
  '3307': ['金华市第一中学', '金华市实验小学'],
  '3401': ['合肥一中', '合肥六小', '合肥师范附属小学', '安徽省实验中学'],
  '3402': ['芜湖市第一中学', '芜湖市实验小学'],
  '3403': ['蚌埠市第一中学', '蚌埠市实验小学'],
  '3501': ['福州第一中学', '福州实验小学', '福建师大附属中学', '福州屏东中学'],
  '3502': ['厦门双十中学', '厦门外国语学校', '厦门大学附属科技中学'],
  '3505': ['泉州第一中学', '泉州师范附属小学'],
  '3601': ['南昌十中', '南昌实验小学', '江西师大附属中学'],
  '4101': ['郑州一中', '郑州外国语学校', '河南省实验中学', '郑州八中'],
  '4102': ['开封高中', '开封市实验小学'],
  '4103': ['洛阳一中', '洛阳市实验小学'],
  '4104': ['平顶山市第一中学', '平顶山市实验小学'],
  '4201': ['武汉外国语学校', '华师一附中', '武汉小学', '武昌实验中学', '武汉二中'],
  '4202': ['黄石市第一中学', '黄石市实验小学'],
  '4301': ['长沙一中', '湖南师大附中', '长沙市实验小学', '雅礼中学'],
  '4302': ['株洲市第一中学', '株洲市实验小学'],
  '4401': ['广州市第六中学', '广州执信中学', '广东省实验中学', '广州小学', '广州二中'],
  '4403': ['深圳中学', '深圳外国语学校', '深圳市育才中学', '深圳高级中学', '红岭中学'],
  '4406': ['佛山一中', '佛山实验学校'],
  '4419': ['东莞中学', '东莞外国语学校'],
  '4420': ['中山一中', '中山实验学校'],
  '4501': ['南宁市第一中学', '南宁外国语学校'],
  '4503': ['桂林中学', '桂林市实验小学'],
  '4601': ['海口市第一中学', '海口外国语学校'],
  '5000': ['重庆一中', '巴蜀中学', '重庆市实验小学', '重庆第八中学'],
  '5101': ['成都四中', '成都外国语学校', '四川师范大学附属中学', '成都实验小学', '成都七中'],
  '5103': ['自贡市第一中学', '自贡市实验小学'],
  '5201': ['贵阳一中', '贵阳外国语学校'],
  '5301': ['昆明市第一中学', '昆明外国语学校'],
  '6101': ['西安中学', '西安外国语学校', '陕西师大附属中学', '西安实验小学'],
  '6201': ['兰州第一中学', '兰州外国语学校'],
  '6301': ['西宁第一中学', '西宁市实验小学'],
  '6401': ['银川一中', '银川外国语学校'],
  '6501': ['乌鲁木齐第一中学', '乌鲁木齐外国语学校'],
  '2101': ['沈阳实验中学', '东北育才学校', '沈阳市实验小学'],
  '2102': ['大连二十四中', '大连理工大学附属中学', '大连市实验小学'],
  '2201': ['东北师大附中', '长春外国语学校', '长春市实验小学'],
  '2301': ['哈尔滨三中', '哈尔滨师范附属中学', '哈尔滨市实验小学'],
  '1100': ['北京四中', '北京一中', '人大附中', '北京小学', '清华附中', '北京景山学校'],
  '1200': ['天津南开中学', '天津一中', '天津耀华中学', '天津市实验小学'],
  '1301': ['石家庄市第一中学', '石家庄外国语学校'],
  '1401': ['太原五中', '太原外国语学校'],
  '1501': ['呼和浩特第一中学', '内蒙古师范大学附属中学'],
};

const SCHOOL_FILTER_MAP = {};
Object.entries(DISTRICT_FILTER_MAP).forEach(([cityCode, districts]) => {
  const prefix = cityCode.substring(0, 4);
  const schoolList = SCHOOL_DATA[prefix] || [
    `${districts[0]}第一实验小学`,
    `${districts[0]}第二中学`,
    `${districts[0]}实验小学`,
    `${districts[0]}实验中学`,
    `${districts[0]}第一中学`,
    `${districts[0]}外国语学校`,
  ];
  districts.forEach((districtName, i) => {
    const districtCode = `${cityCode}${String(i + 1).padStart(2, '0')}`;
    SCHOOL_FILTER_MAP[districtCode] = schoolList.map(s => `${districtName}${s}`);
  });
});

const schoolOptions = computed(() => {
  const key = filter.district;
  const list = SCHOOL_FILTER_MAP[key] || ['实验小学', '第一中学', '实验中学', '第二小学'];
  return list.map((name, i) => ({ code: `${filter.district}${String(i + 1).padStart(2, '0')}`, name }));
});

const onProvinceChange = () => {
  filter.city = '';
  filter.district = '';
  filter.school = '';
};

const onCityChange = () => {
  filter.district = '';
  filter.school = '';
};

const onDistrictChange = () => {
  filter.school = '';
};

const clearProvince = () => {
  filter.province = '';
  filter.city = '';
  filter.district = '';
  filter.school = '';
};

const clearCity = () => {
  filter.city = '';
  filter.district = '';
  filter.school = '';
};

const clearDistrict = () => {
  filter.district = '';
  filter.school = '';
};

const handleSearch = () => {
  if (filter.school) {
    router.push({ path: `/vision/school/${filter.school}`, query: { tab: activeTab.value } });
  } else if (filter.district) {
    router.push({ path: `/vision/county/${filter.district}`, query: { tab: activeTab.value } });
  } else if (filter.city) {
    router.push({ path: `/vision/city/${filter.city}`, query: { tab: activeTab.value } });
  } else if (filter.province) {
    router.push({ path: `/vision/province/${filter.province}`, query: { tab: activeTab.value } });
  }
};

const handleResetFilter = () => {
  filter.province = '';
  filter.city = '';
  filter.district = '';
  filter.school = '';
};

// ============================================================
//  数据
// ============================================================
const nationData = {
  vision: {
    metric: '近视', rate: 53.6, students: 18600, cities: 31,
    primary: 36.7, junior: 71.4, senior: 79.6,
    maleRate: 55.2, femaleRate: 42.2, trend: -1.4,
    citiesRank: [
      { name: '济南', value: 32.63 }, { name: '青岛', value: 20.26 },
      { name: '烟台', value: 19.82 }, { name: '德州', value: 19.13 },
      { name: '泰安', value: 18.96 }, { name: '菏泽', value: 17.23 },
      { name: '聊城', value: 11.28 }, { name: '济宁', value: 10.24 },
      { name: '枣庄', value: 9.33 },  { name: '淄博', value: 8.96 }
    ],
    ageGender: { grades: ['小学', '初中', '高中'], male: [36, 60, 75], female: [28, 52, 68] },
    urbanRural: { urban: 62.2, rural: 33.2 },
    interventions: [
      { name: '试点小学A', initial: 76.4, final: 68.2, change: -8.2 },
      { name: '试点小学B', initial: 84.5, final: 72.6, change: -11.9 },
      { name: '试点小学C', initial: 66.4, final: 62.2, change: -4.2 },
      { name: '试点小学D', initial: 59.8, final: 54.2, change: -5.6 }
    ]
  },
  oral: {
    metric: '龋齿', rate: 45.2, students: 18600, cities: 31,
    primary: 28.0, junior: 50.2, senior: 65.0,
    maleRate: 40.5, femaleRate: 38.2, trend: 0.8,
    citiesRank: [
      { name: '济南', value: 45.2 }, { name: '青岛', value: 43.8 },
      { name: '烟台', value: 40.5 }, { name: '德州', value: 39.2 },
      { name: '泰安', value: 38.1 }, { name: '菏泽', value: 36.5 },
      { name: '聊城', value: 35.2 }, { name: '济宁', value: 34.1 },
      { name: '枣庄', value: 32.8 }, { name: '淄博', value: 31.5 }
    ],
    ageGender: { grades: ['小学', '初中', '高中'], male: [30, 42, 50], female: [26, 38, 47] },
    urbanRural: { urban: 45.0, rural: 28.5 },
    interventions: [
      { name: '试点小学A', initial: 52.3, final: 48.1, change: -4.2 },
      { name: '试点小学B', initial: 62.5, final: 56.8, change: -5.7 },
      { name: '试点小学C', initial: 70.2, final: 62.1, change: -8.1 },
      { name: '试点小学D', initial: 48.6, final: 44.3, change: -4.3 }
    ]
  },
  mental: {
    metric: '心理风险', rate: 23.5, students: 18600, cities: 31,
    primary: 18.2, junior: 25.6, senior: 30.1,
    maleRate: 24.2, femaleRate: 22.8, trend: 2.1,
    citiesRank: [
      { name: '济南', value: 28.3 }, { name: '青岛', value: 26.7 },
      { name: '烟台', value: 24.5 }, { name: '德州', value: 23.8 },
      { name: '泰安', value: 22.4 }, { name: '菏泽', value: 21.5 },
      { name: '聊城', value: 20.8 }, { name: '济宁', value: 20.1 },
      { name: '枣庄', value: 19.4 }, { name: '淄博', value: 18.7 }
    ],
    ageGender: { grades: ['小学', '初中', '高中'], male: [20, 28, 33], female: [16, 23, 28] },
    urbanRural: { urban: 28.0, rural: 18.5 },
    interventions: [
      { name: '试点小学A', initial: 32.4, final: 28.1, change: -4.3 },
      { name: '试点小学B', initial: 38.2, final: 32.5, change: -5.7 },
      { name: '试点小学C', initial: 42.1, final: 35.2, change: -6.9 },
      { name: '试点小学D', initial: 28.9, final: 24.8, change: -4.1 }
    ]
  },
  weight: {
    metric: '超重肥胖', rate: 30.5, students: 18600, cities: 31,
    primary: 22.3, junior: 30.5, senior: 35.0,
    maleRate: 32.1, femaleRate: 28.9, trend: 1.5,
    citiesRank: [
      { name: '济南', value: 35.2 }, { name: '青岛', value: 33.8 },
      { name: '烟台', value: 31.5 }, { name: '德州', value: 30.8 },
      { name: '泰安', value: 29.5 }, { name: '菏泽', value: 28.4 },
      { name: '聊城', value: 27.5 }, { name: '济宁', value: 26.8 },
      { name: '枣庄', value: 25.9 }, { name: '淄博', value: 25.1 }
    ],
    ageGender: { grades: ['小学', '初中', '高中'], male: [25, 32, 38], female: [20, 28, 32] },
    urbanRural: { urban: 35.0, rural: 22.0 },
    interventions: [
      { name: '试点小学A', initial: 42.3, final: 38.1, change: -4.2 },
      { name: '试点小学B', initial: 48.5, final: 42.2, change: -6.3 },
      { name: '试点小学C', initial: 52.1, final: 44.6, change: -7.5 },
      { name: '试点小学D', initial: 38.6, final: 34.3, change: -4.3 }
    ]
  },
  bone: {
    metric: '脊柱侧弯', rate: 16.2, students: 18600, cities: 31,
    primary: 12.1, junior: 16.8, senior: 20.5,
    maleRate: 18.0, femaleRate: 14.4, trend: -0.3,
    citiesRank: [
      { name: '济南', value: 19.5 }, { name: '青岛', value: 18.3 },
      { name: '烟台', value: 17.2 }, { name: '德州', value: 16.8 },
      { name: '泰安', value: 16.0 }, { name: '菏泽', value: 15.5 },
      { name: '聊城', value: 15.0 }, { name: '济宁', value: 14.5 },
      { name: '枣庄', value: 14.0 }, { name: '淄博', value: 13.6 }
    ],
    ageGender: { grades: ['小学', '初中', '高中'], male: [14, 18, 22], female: [10, 15, 19] },
    urbanRural: { urban: 20.0, rural: 12.0 },
    interventions: [
      { name: '试点小学A', initial: 22.4, final: 19.8, change: -2.6 },
      { name: '试点小学B', initial: 26.5, final: 22.8, change: -3.7 },
      { name: '试点小学C', initial: 28.2, final: 24.1, change: -4.1 },
      { name: '试点小学D', initial: 19.8, final: 17.5, change: -2.3 }
    ]
  }
};

const provinceDataMap = {
  vision: { '广东': 52.3, '江苏': 48.7, '浙江': 45.1, '山东': 54.2, '河南': 56.5, '四川': 51.8, '湖北': 49.4, '湖南': 50.1, '福建': 46.8, '安徽': 53.5, '北京': 58.3, '上海': 56.7, '河北': 49.2, '山西': 48.0, '内蒙古': 51.5, '辽宁': 51.3, '吉林': 49.2, '黑龙江': 48.5, '江西': 46.2, '重庆': 52.6, '陕西': 49.1, '甘肃': 45.3 },
  oral:   { '广东': 45.2, '江苏': 42.1, '浙江': 40.5, '山东': 47.8, '河南': 49.0, '四川': 44.5, '湖北': 43.2, '湖南': 44.0, '福建': 41.3, '安徽': 46.4, '北京': 45.2, '上海': 43.8, '河北': 47.5, '山西': 46.8, '辽宁': 48.0, '吉林': 47.2, '黑龙江': 46.5, '江西': 45.5, '重庆': 45.0, '陕西': 44.2, '甘肃': 43.5 },
  mental: { '广东': 25.3, '江苏': 23.1, '浙江': 21.7, '山东': 26.5, '河南': 28.0, '四川': 24.8, '湖北': 23.5, '湖南': 24.0, '福建': 22.2, '安徽': 25.8, '北京': 28.3, '上海': 26.7, '河北': 27.0, '山西': 26.2, '辽宁': 27.5, '吉林': 26.8, '黑龙江': 26.0, '江西': 25.0, '重庆': 25.5, '陕西': 24.5, '甘肃': 23.8 },
  weight: { '广东': 32.1, '江苏': 29.3, '浙江': 27.9, '山东': 33.8, '河南': 35.2, '四川': 31.5, '湖北': 30.4, '湖南': 31.0, '福建': 28.6, '安徽': 32.9, '北京': 35.2, '上海': 33.8, '河北': 34.5, '山西': 33.5, '辽宁': 34.8, '吉林': 33.9, '黑龙江': 33.2, '江西': 32.0, '重庆': 32.5, '陕西': 31.2, '甘肃': 30.5 },
  bone:   { '广东': 17.2, '江苏': 15.8, '浙江': 14.5, '山东': 18.1, '河南': 19.0, '四川': 16.6, '湖北': 15.9, '湖南': 16.3, '福建': 14.9, '安徽': 17.7, '北京': 19.5, '上海': 18.3, '河北': 18.5, '山西': 17.8, '辽宁': 19.2, '吉林': 18.7, '黑龙江': 18.0, '江西': 17.0, '重庆': 17.5, '陕西': 16.8, '甘肃': 16.2 }
};

// ============================================================
//  状态
// ============================================================
const activeTab = ref('vision');
const mapLoaded = ref(false);
const currentDate = ref('');
const currentTime = ref('');
const isLight = ref(false);
const mapCenter = ref([104, 36]);
const mapZoom = ref(1.25);

const cityRankRef = ref(null);
const ageGenderRef = ref(null);
const urbanGaugeRef = ref(null);
const ruralGaugeRef = ref(null);
const interventionRef = ref(null);
const mapRef = ref(null);

const chartInstances = {};
let timer = null;
let themeObserver = null;

const currentNationData = computed(() => nationData[activeTab.value] || nationData.vision);
const currentMetric = computed(() => ({ name: currentNationData.value.metric }));

// ============================================================
//  方法
// ============================================================
const applySystemTheme = () => {
  isLight.value = !document.documentElement.classList.contains('dark');
  setTimeout(() => renderAllCharts(), 300);
};

const observeTheme = () => {
  themeObserver = new MutationObserver(() => {
    const newIsLight = !document.documentElement.classList.contains('dark');
    if (newIsLight !== isLight.value) {
      isLight.value = newIsLight;
    }
  });
  themeObserver.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
};

const normalizeRegionName = (name = '') => {
  const short = PROVINCE_NAME_MAP[name] || name.replace(GEO_SUFFIXES, '');
  return short;
};

const drillToProvince = (name) => {
  const short = normalizeRegionName(name);
  const code = PROVINCE_CODES[short];
  if (code) router.push({ path: `/vision/province/${code}`, query: { tab: activeTab.value } });
};

const drillToCity = (name) => {
  const code = CITY_CODES[name];
  if (code) router.push({ path: `/vision/city/${code}`, query: { tab: activeTab.value } });
};

const switchTab = (key) => {
  activeTab.value = key;
  setTimeout(() => renderAllCharts(), 100);
};

const getTooltip = (trigger = 'axis') => {
  const c = getColors();
  return {
    trigger,
    backgroundColor: c.bg,
    borderColor: c.primary,
    borderWidth: 1,
    textStyle: { color: c.text, fontSize: 12 },
    extraCssText: isLight.value
      ? 'box-shadow: 0 8px 24px rgba(15,23,42,0.12); border-radius: 8px;'
      : 'box-shadow: 0 0 20px rgba(56,189,248,0.35); border-radius: 6px; backdrop-filter: blur(8px);',
  };
};

const getCityRankOption = () => {
  const c = getColors();
  const data = currentNationData.value.citiesRank;
  const sorted = [...data].sort((a, b) => a.value - b.value);
  const values = sorted.map((d) => d.value);
  const sortedDesc = [...values].sort((a, b) => b - a);
  const p25 = sortedDesc[Math.floor(sortedDesc.length * 0.25)];
  const p50 = sortedDesc[Math.floor(sortedDesc.length * 0.5)];

  const colorOf = (v) => {
    if (v >= p25) return c.accent;
    if (v >= p50) return c.secondary;
    return c.primary;
  };

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow', shadowStyle: { color: `${c.primary}14` } },
      formatter: (params) => {
        const p = params[0];
        return `<div style="font-weight:600;margin-bottom:2px">${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div><div style="font-size:11px;color:${c.textDim};margin-top:4px">点击查看市级详情</div>`;
      },
    },
    grid: { left: 60, right: 45, top: 5, bottom: 5 },
    xAxis: { type: 'value', show: false, max: Math.ceil(Math.max(...values) * 1.15) },
    yAxis: {
      type: 'category',
      data: sorted.map((d) => d.name),
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: {
        fontSize: 11,
        formatter: (v, i) => {
          const num = String(i + 1).padStart(2, '0');
          return `{num|${num}.} {name|${v}}`;
        },
        rich: {
          num: { color: c.primary, fontWeight: 'bold', fontSize: 11, width: 28 },
          name: { color: c.text, fontSize: 11 },
        },
      },
    },
    series: [{
      type: 'bar',
      data: values,
      barWidth: 10,
      cursor: 'pointer',
      itemStyle: {
        color: (p) => ({
          type: 'linear',
          x: 0,
          y: 0,
          x2: 1,
          y2: 0,
          colorStops: [
            { offset: 0, color: `${colorOf(p.value)}bb` },
            { offset: 1, color: colorOf(p.value) },
          ],
        }),
        borderRadius: [0, 6, 6, 0],
        shadowBlur: isLight.value ? 4 : 8,
        shadowColor: `${c.primary}55`,
      },
      label: {
        show: true,
        position: 'right',
        color: c.primary,
        fontSize: 11,
        fontWeight: 'bold',
        formatter: '{c}%',
        distance: 6,
      },
    }],
  };
};

const getAgeGenderOption = () => {
  const c = getColors();
  const d = currentNationData.value.ageGender;
  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        const i = params[0].dataIndex;
        return `<div style="font-weight:600;margin-bottom:4px">${d.grades[i]}</div>
                <div>■男生：<span style="color:${c.primary};font-weight:bold">${d.male[i]}%</span></div>
                <div>■女生：<span style="color:${c.secondary};font-weight:bold">${d.female[i]}%</span></div>`;
      },
    },
    legend: {
      data: ['男生', '女生'],
      textStyle: { color: c.text, fontSize: 11 },
      right: 10,
      top: 2,
      itemWidth: 12,
      itemHeight: 12,
    },
    grid: { left: 40, right: 25, top: 30, bottom: 20 },
    xAxis: {
      type: 'value',
      min: -100,
      max: 100,
      splitNumber: 5,
      axisLine: { show: false },
      axisTick: { show: false },
      splitLine: { lineStyle: { color: c.splitLine } },
      axisLabel: { color: c.textDim, fontSize: 10, formatter: (v) => Math.abs(v) + '' },
    },
    yAxis: {
      type: 'category',
      data: d.grades,
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold' },
    },
    series: [
      {
        name: '男生',
        type: 'bar',
        stack: 'total',
        data: d.male.map((v) => -v),
        barWidth: 14,
        itemStyle: { color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'), borderRadius: [4, 0, 0, 4] },
        label: {
          show: true,
          position: 'left',
          color: c.text,
          fontSize: 10,
          fontWeight: 'bold',
          formatter: (p) => Math.abs(p.value) + '%',
        },
      },
      {
        name: '女生',
        type: 'bar',
        stack: 'total',
        data: d.female,
        barWidth: 14,
        itemStyle: { color: makeGrad(c.secondary, isLight.value ? '#c4f1f0' : '#59ebe8'), borderRadius: [0, 4, 4, 0] },
        label: {
          show: true,
          position: 'right',
          color: c.text,
          fontSize: 10,
          fontWeight: 'bold',
          formatter: '{c}%',
        },
      },
    ],
  };
};

const getGaugeOption = (value, name, color) => {
  const c = getColors();
  return {
    backgroundColor: 'transparent',
    series: [{
      type: 'gauge',
      startAngle: 200,
      endAngle: -20,
      min: 0,
      max: 100,
      radius: '92%',
      center: ['50%', '58%'],
      progress: {
        show: true,
        width: 10,
        roundCap: true,
        itemStyle: {
          color: makeGrad(color, color === c.primary ? c.secondary : c.accent),
          shadowBlur: isLight.value ? 6 : 12,
          shadowColor: `${color}88`,
        },
      },
      axisLine: { lineStyle: { width: 10, color: [[1, c.gaugeTrack]] } },
      pointer: { show: false },
      axisTick: { show: false },
      splitLine: { show: false },
      axisLabel: { show: false },
      anchor: { show: false },
      detail: {
        valueAnimation: true,
        formatter: '{value}%',
        color,
        fontSize: 22,
        fontWeight: 'bold',
        offsetCenter: [0, '15%'],
        textShadowBlur: isLight.value ? 0 : 8,
        textShadowColor: color,
      },
      title: { show: true, offsetCenter: [0, '70%'], color: c.textDim, fontSize: 12 },
      data: [{ value, name }],
    }],
  };
};

const getInterventionOption = () => {
  const c = getColors();
  const data = currentNationData.value.interventions;
  const names = data.map((d) => d.name);
  const initials = data.map((d) => d.initial);
  const finals = data.map((d) => d.final);
  const changes = data.map((d) => d.change);

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        let html = '';
        params.forEach((p) => {
          html += `<div>${p.marker}${p.seriesName}：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>`;
        });
        return html;
      },
    },
    legend: { show: false },
    grid: { left: 90, right: 60, top: 8, bottom: 8 },
    xAxis: { type: 'value', show: false, max: Math.ceil(Math.max(...initials) * 1.15) },
    yAxis: {
      type: 'category',
      data: names,
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11 },
    },
    series: [
      {
        name: '期初',
        type: 'bar',
        data: initials,
        barWidth: 9,
        barGap: '30%',
        itemStyle: { color: c.initialBar, borderRadius: [0, 3, 3, 0] },
        label: { show: true, position: 'right', color: c.textDim, fontSize: 10, formatter: '{c}%', distance: 4 },
      },
      {
        name: '期末',
        type: 'bar',
        data: finals,
        barWidth: 9,
        itemStyle: {
          color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'),
          borderRadius: [0, 3, 3, 0],
          shadowBlur: isLight.value ? 4 : 8,
          shadowColor: `${c.primary}55`,
        },
        label: {
          show: true,
          position: 'right',
          color: c.primary,
          fontSize: 10,
          fontWeight: 'bold',
          formatter: (p) => {
            const ch = changes[p.dataIndex];
            return `${p.value}%  {ch|${ch > 0 ? '+' : ''}${ch}}`;
          },
          rich: { ch: { color: c.success, fontSize: 9, padding: [0, 0, 0, 4] } },
        },
      },
    ],
  };
};

const getMapOption = () => {
  if (!mapLoaded.value) return {};
  const c = getColors();
  const data = provinceDataMap[activeTab.value] || {};
  // 包含全部省份；无数据省份默认 0%，由 visualMap 映射为 #0bc4e9，与图例保持一致
  const mapData = Object.keys(PROVINCE_CODES).map((name) => {
    const val = data[name];
    const hasData = val !== undefined && val !== null;
    return { name, value: hasData ? val : 0 };
  });

  // Major city coordinates for effectScatter
  const majorCities = [
    { name: '北京', value: [116.4, 39.9, 85] },
    { name: '上海', value: [121.47, 31.23, 90] },
    { name: '广州', value: [113.27, 23.13, 78] },
    { name: '深圳', value: [114.05, 22.55, 75] },
    { name: '成都', value: [104.06, 30.67, 65] },
    { name: '西安', value: [108.95, 34.27, 60] },
    { name: '武汉', value: [114.31, 30.52, 70] },
    { name: '杭州', value: [120.15, 30.28, 72] },
    { name: '哈尔滨', value: [126.53, 45.8, 55] },
    { name: '乌鲁木齐', value: [87.62, 43.82, 50] },
  ];

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('item'),
      formatter: (p) => {
        const val = Array.isArray(p.value) ? p.value[p.value.length - 1] : p.value;
        if (p.seriesType === 'effectScatter') {
          return `<div style="font-weight:600">${p.name}</div><div>人口/指标：<span style="color:${c.accent};font-weight:bold">${val || 0}</span></div>`;
        }
        return `<div style="font-weight:600">${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${val || 0}%</span></div><div style="font-size:11px;color:${c.textDim};margin-top:4px">点击进入省级看板</div>`;
      },
    },
    visualMap: {
      show: false,
      type: 'piecewise',
      min: 0,
      max: 100,
      pieces: [
        { min: 0, max: 48, color: '#0bc4e9' },
        { min: 48, max: 55, color: '#7dd87d' },
        { min: 55, max: 60, color: '#f5c542' },
        { min: 60, max: 100, color: '#e8554f' },
      ],
      outOfRange: {
        color: 'rgba(15, 30, 65, 0.4)',
      },
    },
    geo: {
      map: 'china',
      roam: true,
      zoom: mapZoom.value,
      center: mapCenter.value,
      scaleLimit: { min: 0.8, max: 5 },
      nameMap: PROVINCE_NAME_MAP,
      label: {
        show: true,
        color: '#ffffff',
        fontSize: 10,
        fontWeight: '600',
        textShadowColor: 'rgba(0, 0, 0, 0.9)',
        textShadowBlur: 6,
        textShadowOffsetX: 0,
        textShadowOffsetY: 1,
      },
      itemStyle: {
        borderColor: 'rgba(11, 196, 233, 0.6)',
        borderWidth: 1.5,
        shadowBlur: 30,
        shadowColor: 'rgba(0, 212, 255, 0.35)',
      },
      emphasis: {
        label: {
          show: true,
          color: '#ffffff',
          fontSize: 12,
          fontWeight: 700,
          textShadowColor: 'rgba(0, 0, 0, 0.9)',
          textShadowBlur: 8,
        },
        itemStyle: {
          areaColor: '#f5c542',
          borderColor: '#ffffff',
          borderWidth: 2,
          shadowBlur: 40,
          shadowColor: 'rgba(245, 197, 66, 0.6)',
        },
      },
      select: {
        label: { show: true, color: '#ffffff' },
        itemStyle: { areaColor: 'rgba(11, 196, 233, 0.5)' },
      },
      zlevel: 1,
    },
    series: [
      {
        type: 'map',
        geoIndex: 0,
        selectedMode: false,
        data: mapData,
      },
      {
        type: 'effectScatter',
        coordinateSystem: 'geo',
        rippleEffect: {
          scale: 5,
          brushType: 'stroke',
          period: 4,
        },
        symbol: 'circle',
        symbolSize: (val) => Math.max(8, Math.min(22, val[2] / 4)),
        itemStyle: {
          color: '#0efcff',
          shadowBlur: 20,
          shadowColor: '#0efcff',
        },
        label: {
          show: true,
          formatter: '{b}',
          position: 'right',
          color: '#ffffff',
          fontSize: 11,
          fontWeight: 600,
          textShadowColor: 'rgba(0, 0, 0, 0.9)',
          textShadowBlur: 6,
        },
        data: majorCities,
        zlevel: 3,
      },
      {
        type: 'effectScatter',
        coordinateSystem: 'geo',
        rippleEffect: {
          scale: 3,
          brushType: 'stroke',
          period: 3,
        },
        symbol: 'circle',
        symbolSize: (val) => Math.max(4, Math.min(10, val[2] / 10)),
        itemStyle: {
          color: '#f5c542',
          shadowBlur: 10,
          shadowColor: '#f5c542',
        },
        label: { show: false },
        data: majorCities.map(c => ({ ...c })),
        zlevel: 4,
      },
    ],
  };
};

const disposeAll = () => {
  Object.values(chartInstances).forEach(c => { if (c && !c.isDisposed()) c.dispose(); });
  Object.keys(chartInstances).forEach(k => delete chartInstances[k]);
};

const initChart = (ref, key, option, onClick, retryCount = 0) => {
  if (!ref.value) return;
  try {
    const rect = ref.value.getBoundingClientRect();
    if (rect.width === 0 || rect.height === 0) {
      if (retryCount < 3) setTimeout(() => initChart(ref, key, option, onClick, retryCount + 1), 200);
      return;
    }
    const chart = echarts.init(ref.value);
    chartInstances[key] = chart;
    chart.setOption(option);
    if (onClick) chart.on('click', onClick);
    chart.resize();
  } catch (e) {}
};

const renderAllCharts = () => {
  disposeAll();
  requestAnimationFrame(() => {
    nextTick(() => {
      const c = getColors();
      const ur = currentNationData.value.urbanRural;
      initChart(cityRankRef, 'cityRank', getCityRankOption(), (params) => {
        if (params?.name) drillToCity(params.name);
      });
      initChart(ageGenderRef, 'ageGender', getAgeGenderOption());
      initChart(urbanGaugeRef, 'urbanGauge', getGaugeOption(ur.urban, `城区${currentMetric.value.name}率`, c.primary));
      initChart(ruralGaugeRef, 'ruralGauge', getGaugeOption(ur.rural, `县乡${currentMetric.value.name}率`, c.secondary));
      initChart(interventionRef, 'intervention', getInterventionOption());
      if (mapLoaded.value) {
        initChart(mapRef, 'map', getMapOption(), (params) => {
          if (params?.name) drillToProvince(params.name);
        });
        // Update display values when map roams
        const mapChart = chartInstances['map'];
        if (mapChart) {
          mapChart.on('georoam', () => {
            const option = mapChart.getOption();
            if (option && option.geo && option.geo[0]) {
              if (option.geo[0].center) mapCenter.value = option.geo[0].center;
              if (option.geo[0].zoom != null) mapZoom.value = option.geo[0].zoom;
            }
          });
        }
      }
    });
  });
};

const loadMap = async () => {
  const urls = [
    'https://geo.datav.aliyun.com/areas_v3/bound/100000_full.json',
    'https://fastly.jsdelivr.net/npm/echarts@4.9.0/map/json/china.json',
  ];
  for (const url of urls) {
    try {
      const res = await fetch(url);
      if (!res.ok) continue;
      const geoJson = await res.json();
      echarts.registerMap('china', geoJson);
      mapLoaded.value = true;
      setTimeout(() => renderAllCharts(), 200);
      return;
    } catch (e) {
      console.warn('地图加载失败', url, e);
    }
  }
};

const updateDateTime = () => {
  const now = new Date();
  currentDate.value = now.toLocaleDateString('zh-CN', { year: 'numeric', month: 'long', day: 'numeric', weekday: 'short' });
  currentTime.value = now.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
};

const resizeAll = () => {
  Object.values(chartInstances).forEach(c => { if (c && !c.isDisposed()) c.resize(); });
};

onMounted(() => {
  applySystemTheme();
  observeTheme();
  updateDateTime();
  timer = setInterval(updateDateTime, 1000);
  loadMap();
  renderAllCharts();
  window.addEventListener('resize', resizeAll);
  document.addEventListener('click', handleDocumentClick);
  setTimeout(() => resizeAll(), 500);
  setTimeout(() => resizeAll(), 1500);
});

onBeforeUnmount(() => {
  if (timer) clearInterval(timer);
  window.removeEventListener('resize', resizeAll);
  document.removeEventListener('click', handleDocumentClick);
  if (themeObserver) { themeObserver.disconnect(); themeObserver = null; }
  disposeAll();
});

watch(activeTab, () => setTimeout(() => renderAllCharts(), 100));
watch(isLight, () => setTimeout(() => renderAllCharts(), 100));
const VALID_TABS = ['vision', 'oral', 'mental', 'weight', 'bone'];
watch(() => route.query.tab, (tab) => {
  if (tab && typeof tab === 'string' && VALID_TABS.includes(tab)) {
    activeTab.value = tab;
    setTimeout(() => renderAllCharts(), 100);
  }
}, { immediate: true });


defineExpose({ switchTab });
</script>

<style scoped>
/* ============================================================
   CSS 变量 - 深色主题（参考 1.css 青色科技风格）
   ============================================================ */
.app-root {
  --primary: #0bc4e9;
  --primary-soft: rgba(11, 196, 233, 0.12);
  --primary-glow: rgba(11, 196, 233, 0.6);
  --secondary: #59ebe8;
  --secondary-soft: rgba(89, 235, 232, 0.12);
  --secondary-glow: rgba(89, 235, 232, 0.5);
  --accent: #0efcff;
  --accent-glow: rgba(14, 252, 255, 0.4);
  --warning: #fbbf24;
  --success: #34d399;
  --danger: #fb7185;

  --bg: #06101c;
  --bg-deep: #040a14;
  --bg-card: rgba(0, 72, 115, 0.28);
  --bg-card-hover: rgba(0, 90, 140, 0.4);
  --bg-soft: rgba(11, 196, 233, 0.05);

  --border: #0bc4e9;
  --border-hover: #59ebe8;
  --border-soft: rgba(11, 196, 233, 0.3);
  --border-inner: #007297;
  --border-accent: #00d8ff;

  --text: #ffffff;
  --text-dim: #8adeff;
  --text-muted: #4e7a94;
  --text-dimmer: #61d2f7;

  --glow: rgba(11, 196, 233, 0.5);
  --glow-soft: rgba(11, 196, 233, 0.15);
  --glow-strong: rgba(11, 196, 233, 0.8);
  --glow-cyan: rgba(0, 216, 255, 0.5);

  --shadow-card: 0 4px 30px rgba(0, 0, 0, 0.5), 0 0 0 1px rgba(11, 196, 233, 0.15);
  --shadow-hover: 0 8px 40px rgba(11, 196, 233, 0.3);
  --shadow-glow: 0 0 20px rgba(11, 196, 233, 0.25);

  --grad-main: linear-gradient(135deg, #0bc4e9 0%, #59ebe8 100%);
  --grad-cyan: linear-gradient(135deg, #0bc4e9 0%, #00a8d7 100%);
  --grad-accent: linear-gradient(135deg, #0efcff 0%, #00d8ff 100%);
  --grad-deep: linear-gradient(180deg, rgba(0,72,115,.28) 0%, rgba(0,50,90,.4) 100%);
}

/* 浅色主题（青色科技风格） */
.app-root.theme-light {
  --primary: #0891b2;
  --primary-soft: rgba(8, 145, 178, 0.08);
  --primary-glow: rgba(8, 145, 178, 0.25);
  --secondary: #0e7490;
  --secondary-soft: rgba(14, 116, 144, 0.08);
  --secondary-glow: rgba(14, 116, 144, 0.2);
  --accent: #00d8ff;
  --accent-glow: rgba(0, 216, 255, 0.2);
  --bg: #f1f5f9;
  --bg-deep: #e2e8f0;
  --bg-card: rgba(255, 255, 255, 0.95);
  --bg-card-hover: rgba(255, 255, 255, 1);
  --bg-soft: rgba(8, 145, 178, 0.03);
  --border: rgba(8, 145, 178, 0.2);
  --border-hover: rgba(8, 145, 178, 0.4);
  --border-soft: rgba(8, 145, 178, 0.1);
  --border-inner: rgba(0, 114, 151, 0.15);
  --border-accent: rgba(0, 216, 255, 0.4);
  --text: #1e293b;
  --text-dim: #0e7490;
  --text-muted: #64748b;
  --text-dimmer: #0891b2;
  --glow: rgba(8, 145, 178, 0.15);
  --glow-soft: rgba(8, 145, 178, 0.08);
  --glow-strong: rgba(8, 145, 178, 0.3);
  --glow-cyan: rgba(0, 216, 255, 0.2);
  --shadow-card: 0 4px 20px rgba(15, 23, 42, 0.05), 0 0 0 1px rgba(8, 145, 178, 0.08);
  --shadow-hover: 0 8px 28px rgba(8, 145, 178, 0.12);
  --shadow-glow: 0 0 20px rgba(8, 145, 178, 0.15);
  --grad-main: linear-gradient(135deg, #0891b2 0%, #59ebe8 100%);
  --grad-cyan: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
  --grad-accent: linear-gradient(135deg, #00d8ff 0%, #0bc4e9 100%);
  --grad-deep: linear-gradient(180deg, rgba(255,255,255,.95) 0%, rgba(240,249,255,.98) 100%);
}

.app-root *, .app-root *::before, .app-root *::after { box-sizing: border-box; }

.app-root {
  width: 100%;
  height: 100vh;
  position: relative;
  overflow: hidden;
  background: #060f1c url('./images/bg.png') no-repeat center center fixed;
  background-size: cover;
  color: var(--text);
  font-family: 'Inter', 'PingFang SC', 'Microsoft YaHei', system-ui, sans-serif;
  display: flex;
  flex-direction: column;
}

/* ============================================================
   全局背景 - 增强版
   ============================================================ */
.app-root.theme-light .scan-line,
.app-root.theme-light .scan-line-2,
.app-root.theme-light .bg-stars,
.app-root.theme-light .bg-particles { display: none; }
.app-root.theme-light .bg-glow { opacity: 0.04; filter: blur(120px); }
.app-root.theme-light .bg-grid { opacity: 0.3; }
.app-root.theme-light .tech-panel::before,
.app-root.theme-light .tech-panel::after { opacity: 0.35; box-shadow: none; }
.app-root.theme-light .title-text {
  background: linear-gradient(180deg, #1e293b 0%, #0891b2 50%, #00a8d7 100%);
  -webkit-background-clip: text;
  background-clip: text;
  filter: none;
}
.app-root.theme-light .tc-digit { text-shadow: none; }
.app-root.theme-light .kpi-lg-value { filter: none; }
.app-root.theme-light .kpi-sm-value,
.app-root.theme-light .gender-item.male .gender-value,
.app-root.theme-light .gender-item.female .gender-value { text-shadow: none; }
.app-root.theme-light .tech-tab.active {
  box-shadow: 0 4px 16px rgba(124, 58, 237, 0.25);
}
.app-root.theme-light .map-area {
  background: linear-gradient(180deg, rgba(255,255,255,0.6), rgba(238,241,247,0.9));
  box-shadow: var(--shadow-card);
}
.app-root.theme-light .map-deco-ready .map-grid { opacity: 0.2; }
.app-root.theme-light .tech-panel-top-border { display: none; }
.app-root.theme-light .chart-area-glow { display: none; }
.app-root.theme-light {
  background: #f1f5f9 url('./images/bg.png') no-repeat center center fixed;
  background-size: cover;
}
.app-root.theme-light .bg-decor {
  background: linear-gradient(180deg, rgba(248, 250, 252, 0.75) 0%, rgba(241, 245, 249, 0.85) 100%);
}
.bg-decor { 
  position: absolute; inset: 0; pointer-events: none; z-index: 0; overflow: hidden; 
  background: linear-gradient(180deg, rgba(6, 15, 28, 0.65) 0%, rgba(6, 15, 28, 0.75) 100%);
}
.bg-grid {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(0, 229, 255, 0.1) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 229, 255, 0.1) 1px, transparent 1px);
  background-size: 50px 50px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  -webkit-mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  animation: grid-drift 30s linear infinite;
}
@keyframes grid-drift {
  0% { background-position: 0 0; }
  100% { background-position: 50px 50px; }
}

.bg-glow {
  position: absolute; border-radius: 50%; filter: blur(150px);
  animation: glow-float 20s ease-in-out infinite;
}
.bg-glow-1 { width: 800px; height: 800px; background: #00e5ff; top: -200px; left: -150px; opacity: 0.2; }
.bg-glow-2 { width: 900px; height: 900px; background: #00a8d7; bottom: -250px; right: -150px; opacity: 0.18; animation-delay: -6s; }
.bg-glow-3 { width: 700px; height: 700px; background: var(--accent); top: 30%; left: 40%; opacity: 0.08; animation-delay: -12s; }
@keyframes glow-float {
  0%, 100% { transform: translate(0, 0) scale(1); }
  33% { transform: translate(80px, -50px) scale(1.1); }
  66% { transform: translate(-50px, 60px) scale(0.95); }
}

.bg-particles {
  position: absolute; inset: 0; z-index: 1; pointer-events: none;
  background-image:
    radial-gradient(1px 1px at 10% 20%, rgba(0, 229, 255, 0.7) 50%, transparent 100%),
    radial-gradient(1px 1px at 30% 60%, rgba(0, 229, 255, 0.5) 50%, transparent 100%),
    radial-gradient(1px 1px at 50% 10%, rgba(139, 92, 246, 0.6) 50%, transparent 100%),
    radial-gradient(1px 1px at 70% 40%, rgba(0, 229, 255, 0.4) 50%, transparent 100%),
    radial-gradient(1px 1px at 90% 80%, rgba(139, 92, 246, 0.5) 50%, transparent 100%),
    radial-gradient(1.5px 1.5px at 25% 85%, rgba(0, 229, 255, 0.6) 50%, transparent 100%),
    radial-gradient(1px 1px at 65% 75%, rgba(0, 229, 255, 0.45) 50%, transparent 100%),
    radial-gradient(1px 1px at 85% 25%, rgba(139, 92, 246, 0.55) 50%, transparent 100%);
  background-size: 400px 400px, 350px 350px, 500px 500px, 300px 300px, 450px 450px, 380px 380px, 420px 420px, 480px 480px;
  animation: particle-drift 25s linear infinite, star-twinkle 5s ease-in-out infinite alternate;
}
@keyframes particle-drift {
  0% { background-position: 0 0, 0 0, 0 0, 0 0, 0 0, 0 0, 0 0, 0 0; }
  100% { background-position: -400px 300px, -350px 280px, -500px 350px, -300px 220px, -450px 320px, -380px 260px, -420px 300px, -480px 340px; }
}

.bg-stars { position: absolute; inset: 0; }
.bg-stars::before, .bg-stars::after {
  content: ''; position: absolute; inset: 0;
  background-image:
    radial-gradient(1px 1px at 20% 30%, var(--primary) 50%, transparent 100%),
    radial-gradient(1px 1px at 60% 70%, var(--secondary) 50%, transparent 100%),
    radial-gradient(1px 1px at 80% 10%, var(--text) 50%, transparent 100%),
    radial-gradient(1.5px 1.5px at 40% 80%, var(--primary) 50%, transparent 100%),
    radial-gradient(1px 1px at 90% 50%, var(--secondary) 50%, transparent 100%);
  background-size: 300px 300px;
  animation: star-twinkle 4s ease-in-out infinite alternate;
}
.bg-stars::after { animation-delay: 2s; opacity: 0.6; }
@keyframes star-twinkle { 0% { opacity: 0.3; } 100% { opacity: 0.8; } }

/* 扫描线 - 参考 1.css 风格 */
.scan-line {
  position: absolute; left: 0; right: 0; top: 0; height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--accent), var(--primary), transparent);
  background-size: 200% 100%;
  box-shadow: 0 0 20px var(--glow), 0 0 40px var(--glow), 0 0 60px var(--glow-soft);
  animation: scan-move 7s linear infinite;
  z-index: 1; pointer-events: none;
}
.scan-line-2 {
  position: absolute; left: 0; right: 0; top: 0; height: 120px;
  background: linear-gradient(180deg, transparent, rgba(11, 196, 233, 0.08), transparent);
  animation: scan-move 7s linear infinite;
  animation-delay: 1.5s;
  z-index: 1; pointer-events: none;
}
@keyframes scan-move {
  0% { transform: translateY(-120px); }
  100% { transform: translateY(100vh); }
}

/* 顶部光带 - 增强版 */
.tech-topbar {
  position: relative; z-index: 5;
  display: flex; align-items: center; justify-content: space-between;
  padding: 4px 24px;
  background: linear-gradient(90deg, transparent, rgba(11, 196, 233, 0.06), rgba(0, 168, 215, 0.04), transparent);
  border-bottom: 1px solid var(--border-soft);
}
.tech-corner {
  width: 26px; height: 26px;
  border: 1.5px solid var(--primary);
  position: relative; box-shadow: 0 0 10px var(--glow), 0 0 20px var(--glow-soft);
}
.tech-corner-l { border-right: none; border-bottom: none; }
.tech-corner-r { border-left: none; border-bottom: none; }
.tc-dot {
  position: absolute; width: 6px; height: 6px;
  background: var(--primary); border-radius: 50%;
  box-shadow: 0 0 10px var(--primary), 0 0 20px var(--glow);
  animation: dot-blink 1.5s ease-in-out infinite;
}
.tech-corner-l .tc-dot { right: -3px; bottom: -3px; }
.tech-corner-r .tc-dot { left: -3px; bottom: -3px; }
@keyframes dot-blink { 0%, 100% { opacity: 1; transform: scale(1); } 50% { opacity: 0.4; transform: scale(0.7); } }

.tech-line { flex: 1; height: 1px; background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent); margin: 0 8px; position: relative; }
.line-pulse {
  position: absolute; top: 50%; width: 70px; height: 2px;
  background: linear-gradient(90deg, var(--primary), var(--secondary));
  transform: translateY(-50%);
  box-shadow: 0 0 12px var(--primary);
  animation: line-move 3s ease-in-out infinite;
}
.line-pulse-1 { left: 10%; }
.line-pulse-2 { left: 10%; animation-delay: 1.5s; }
.line-pulse-3 { left: 60%; }
.line-pulse-4 { left: 60%; animation-delay: 1.5s; }
@keyframes line-move {
  0% { transform: translateY(-50%) translateX(0); opacity: 0; }
  30% { opacity: 1; }
  100% { transform: translateY(-50%) translateX(200px); opacity: 0; }
}

.tech-dots { display: flex; gap: 6px; }
.tech-dots span { width: 4px; height: 4px; background: var(--primary); border-radius: 50%; box-shadow: 0 0 6px var(--primary), 0 0 12px var(--glow); animation: dot-blink 1.2s ease-in-out infinite; }
.tech-dots span:nth-child(2) { animation-delay: 0.15s; }
.tech-dots span:nth-child(3) { animation-delay: 0.3s; }
.tech-dots span:nth-child(4) { animation-delay: 0.45s; }
.tech-dots span:nth-child(5) { animation-delay: 0.6s; }
.tech-dots span:nth-child(6) { animation-delay: 0.75s; }
.tech-dots span:nth-child(7) { animation-delay: 0.9s; }

/* Header - 增强版 */
.national-header {
  position: relative; z-index: 5;
  display: flex; align-items: center; justify-content: space-between;
  padding: 6px 32px 4px; flex-shrink: 0;
}
.header-deco { display: flex; align-items: center; gap: 8px; flex: 1; }
.header-deco-l { justify-content: flex-end; padding-right: 24px; }
.header-deco-r { padding-left: 24px; }
.hd-line { width: 100px; height: 1px; background: linear-gradient(90deg, transparent, var(--primary), var(--secondary)); }
.hd-dot { width: 8px; height: 8px; background: var(--primary); border-radius: 50%; box-shadow: 0 0 10px var(--primary), 0 0 20px var(--glow); animation: dot-blink 2s ease-in-out infinite; }

.national-title {
  position: relative; display: inline-flex; align-items: center;
  font-size: 40px; font-weight: 700; letter-spacing: 6px;
  margin: 0; white-space: nowrap;
}
.title-bracket { color: var(--primary); font-weight: 700; text-shadow: 0 0 30px var(--glow); }
.title-text {
  color: #fff;
  text-shadow: 0 0 10px var(--primary), 0 0 20px var(--glow), 0 0 40px var(--glow-soft);
  margin: 0 14px;
  font-family: "微软雅黑", sans-serif;
  font-weight: bolder;
  letter-spacing: 8px;
}
.title-shine {
  position: absolute; top: 0; left: -30%; width: 30%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.5), transparent);
  animation: shine-move 4s ease-in-out infinite;
  pointer-events: none;
}
@keyframes shine-move {
  0%, 100% { left: -30%; }
  50% { left: 120%; }
}

.national-title::after {
  content: '';
  position: absolute;
  bottom: -8px;
  left: 50%;
  transform: translateX(-50%);
  width: 60%;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--accent), var(--primary), transparent);
  background-size: 200% 100%;
  animation: title-underline-flow 4s linear infinite;
  box-shadow: 0 0 15px var(--glow);
}
@keyframes title-underline-flow {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}

.national-time { display: flex; flex-direction: column; align-items: flex-end; gap: 2px; }
.time-date { font-size: 12px; color: var(--text-dimmer); letter-spacing: 1px; text-shadow: 0 0 6px var(--glow-soft); }
.time-clock { display: flex; align-items: center; gap: 1px; }
.tc-digit {
  font-size: 20px; font-weight: 700; color: var(--primary);
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  text-shadow: 0 0 14px var(--glow), 0 0 6px var(--primary);
  letter-spacing: 2px;
}
.tc-blink { color: var(--secondary); animation: blink 1s steps(1) infinite; padding: 0 1px; text-shadow: 0 0 10px var(--secondary-glow); }
@keyframes blink { 0%, 50% { opacity: 1; } 51%, 100% { opacity: 0.3; } }

/* Tabs - 增强版 */
.national-tabs {
  position: relative; z-index: 5;
  display: flex; align-items: center; justify-content: center;
  gap: 6px; padding: 4px 20px; flex-shrink: 0;
}
.tech-tab {
  position: relative;
  display: flex; align-items: center; gap: 8px;
  padding: 6px 20px;
  background: var(--bg-soft);
  border: 1px solid var(--border-soft);
  border-radius: 20px;
  color: var(--text-dim);
  font-size: 13px; font-weight: 500;
  font-family: inherit; cursor: pointer;
  transition: all 0.3s; overflow: hidden;
}
.tech-tab:hover { background: var(--primary-soft); color: var(--primary); border-color: var(--border); box-shadow: 0 0 12px var(--glow-soft); }
.tech-tab-icon { font-size: 14px; }
.tech-tab.active {
  background: var(--grad-main);
  border-color: transparent; color: #fff;
  box-shadow: 0 0 24px var(--glow), 0 0 36px var(--glow-soft);
  text-shadow: 0 0 6px rgba(255,255,255,0.5);
}
.tech-tab.active::before {
  content: ''; position: absolute; top: 0; left: 0; right: 0; height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--accent), var(--primary), transparent);
  background-size: 200% 100%;
  animation: tab-border-move 3s linear infinite;
}
.tech-tab.active::after {
  content: ''; position: absolute; bottom: 0; left: 0; right: 0; height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--accent), var(--primary), transparent);
  background-size: 200% 100%;
  animation: tab-border-move 3s linear infinite;
  animation-delay: -1.5s;
}
@keyframes tab-border-move {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}
.tech-tab-glow {
  position: absolute; bottom: 0; left: 50%; width: 60%; height: 1px;
  background: var(--primary); transform: translateX(-50%);
  opacity: 0; transition: opacity 0.3s;
}
.tech-tab.active .tech-tab-glow { opacity: 1; box-shadow: 0 0 10px var(--primary); }

/* 顶部筛选栏 */
.top-filter-bar {
  position: relative; z-index: 6;
  display: flex; align-items: center; justify-content: center;
  gap: 10px; padding: 4px 20px 6px; flex-shrink: 0;
}
.filter-select {
  position: relative; display: inline-flex; align-items: stretch;
}
.filter-select::before {
  content: ''; position: absolute; inset: 0;
  border-radius: 4px; padding: 1px;
  background: linear-gradient(135deg, rgba(11, 196, 233, 0.6), rgba(0, 168, 215, 0.6));
  -webkit-mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
          mask-composite: exclude;
  pointer-events: none;
  transition: opacity 0.3s; opacity: 0;
}
.filter-select:hover::before,
.filter-select.has-value::before,
.filter-select.is-open::before { opacity: 1; }

.select-trigger {
  position: relative;
  background: var(--bg-soft);
  border: 1px solid var(--border);
  border-radius: 4px;
  color: var(--text-dim);
  font-size: 13px;
  padding: 7px 34px 7px 14px;
  font-family: inherit;
  cursor: pointer;
  outline: none;
  transition: all 0.3s;
  box-shadow: inset 0 0 12px rgba(0, 212, 255, 0.08);
  min-width: 150px;
  display: flex;
  align-items: center;
  gap: 4px;
  user-select: none;
}
.select-trigger:hover,
.select-trigger:focus {
  border-color: var(--border-hover);
  color: var(--text);
  box-shadow: 0 0 14px var(--glow-soft), inset 0 0 12px rgba(0, 212, 255, 0.12);
}
.filter-select.has-value .select-trigger { color: var(--text); }
.select-trigger.placeholder { color: var(--text-muted); }
.select-trigger.disabled {
  opacity: 0.45; cursor: not-allowed;
  background: var(--bg-soft);
}
.select-trigger.disabled:hover {
  border-color: var(--border);
  box-shadow: inset 0 0 12px rgba(0, 212, 255, 0.05);
  color: var(--text-muted);
}
.select-value {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.select-value.placeholder { color: var(--text-muted); }

.filter-select .arrow-down {
  position: absolute; right: 12px; top: 50%;
  transform: translateY(-50%);
  color: var(--primary); font-size: 9px;
  pointer-events: none;
  text-shadow: 0 0 4px var(--glow);
  transition: transform 0.25s ease;
}
.filter-select.is-open .arrow-down.rotated {
  transform: translateY(-50%) rotate(180deg);
}

.filter-select .clear-icon {
  position: absolute; right: 28px; top: 50%;
  transform: translateY(-50%);
  width: 16px; height: 16px;
  display: flex; align-items: center; justify-content: center;
  color: var(--text-dim); font-size: 10px;
  cursor: pointer; z-index: 2;
  border-radius: 50%;
  background: var(--bg-card);
  border: 1px solid var(--border);
  transition: all 0.2s;
}
.filter-select .clear-icon:hover {
  color: var(--danger); border-color: var(--danger);
  box-shadow: 0 0 8px var(--glow-danger);
}

.select-dropdown {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  min-width: 100%;
  z-index: 20;
  background: var(--bg);
  border: 1px solid var(--border);
  border-radius: 6px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.35), 0 0 0 1px rgba(0, 212, 255, 0.1);
  overflow: hidden;
  animation: dropdown-slide 0.2s ease-out;
}
@keyframes dropdown-slide {
  from { opacity: 0; transform: translateY(-6px); }
  to { opacity: 1; transform: translateY(0); }
}

.dropdown-header {
  padding: 8px 14px;
  font-size: 11px;
  color: var(--text-dim);
  text-transform: uppercase;
  letter-spacing: 1px;
  border-bottom: 1px solid var(--border);
  background: linear-gradient(180deg, var(--bg-soft) 0%, transparent 100%);
}

.dropdown-list {
  max-height: 240px;
  overflow-y: auto;
  padding: 4px;
}
.dropdown-list::-webkit-scrollbar { width: 5px; }
.dropdown-list::-webkit-scrollbar-track { background: transparent; }
.dropdown-list::-webkit-scrollbar-thumb {
  background: var(--border);
  border-radius: 3px;
}
.dropdown-list::-webkit-scrollbar-thumb:hover { background: var(--primary); }

.dropdown-item {
  padding: 8px 12px 8px 14px;
  font-size: 13px;
  color: var(--text-dim);
  cursor: pointer;
  border-radius: 4px;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 8px;
  position: relative;
}
.dropdown-item:hover {
  background: var(--primary-soft);
  color: var(--text);
  padding-left: 18px;
}
.dropdown-item.selected {
  color: var(--primary);
  background: var(--primary-soft);
  font-weight: 500;
}
.dropdown-item.selected::before {
  content: '';
  position: absolute;
  left: 6px;
  top: 50%;
  transform: translateY(-50%);
  width: 3px;
  height: 14px;
  background: var(--primary);
  border-radius: 2px;
  box-shadow: 0 0 8px var(--glow);
}
.dropdown-item .check-mark {
  font-size: 11px;
  color: var(--primary);
  text-shadow: 0 0 4px var(--glow);
}

.dropdown-empty {
  padding: 16px;
  text-align: center;
  font-size: 12px;
  color: var(--text-muted);
}

.btn-search-top, .btn-reset-top {
  display: inline-flex; align-items: center; gap: 6px;
  padding: 7px 18px;
  border: 1px solid var(--border);
  border-radius: 4px;
  background: var(--bg-soft);
  color: var(--text-dim);
  font-size: 13px;
  font-family: inherit;
  cursor: pointer;
  transition: all 0.3s;
  flex-shrink: 0;
}
.btn-search-top:hover, .btn-reset-top:hover {
  border-color: var(--border-hover);
  color: var(--primary);
  background: var(--primary-soft);
  box-shadow: 0 0 12px var(--glow-soft);
}
.btn-search-top {
  background: var(--grad-main);
  border-color: transparent;
  color: #fff;
  box-shadow: 0 0 18px var(--glow), 0 4px 14px rgba(0, 212, 255, 0.25);
  text-shadow: 0 0 6px rgba(255,255,255,0.4);
}
.btn-search-top:hover {
  filter: brightness(1.1);
  box-shadow: 0 0 25px var(--glow-strong), 0 6px 20px rgba(0, 212, 255, 0.35);
}
.btn-search-top .icon, .btn-reset-top .icon { font-size: 13px; }

/* 主体 */
.national-body {
  position: relative; z-index: 3;
  flex: 1;
  display: grid;
  grid-template-columns: 22% 1fr 22%;
  gap: 14px;
  padding: 8px 20px 16px;
  min-height: 0;
  height: 0;
}
.left-col, .right-col, .map-col { display: flex; flex-direction: column; gap: 12px; min-height: 0; height: 100%; }
.map-col { 
  gap: 12px; 
  padding: 6px;
}
.map-col .map-area { min-height: 0; position: relative; flex: 1; }

/* 科技面板 - 参考 1.css .boxall 风格 */
.tech-panel {
  position: relative;
  background: var(--grad-deep);
  border: 1px solid var(--border);
  border-radius: 0;
  padding: 12px 14px;
  display: flex; flex-direction: column;
  backdrop-filter: blur(12px);
  box-shadow: var(--shadow-card);
  flex: 1; min-height: 0;
  overflow: hidden; transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
}
.tech-panel::before {
  content: '';
  position: absolute;
  width: 80%;
  height: 100%;
  bottom: -1px;
  top: -1px;
  left: 10%;
  border-bottom: 1px solid var(--border-inner);
  border-top: 1px solid var(--border-inner);
  transition: all 0.5s;
  pointer-events: none;
}
.tech-panel::after {
  content: '';
  position: absolute;
  width: 100%;
  height: 80%;
  left: -1px;
  right: -1px;
  top: 10%;
  border-left: 1px solid var(--border-inner);
  border-right: 1px solid var(--border-inner);
  transition: all 0.5s;
  pointer-events: none;
}
.tech-panel:hover::before { width: 0%; }
.tech-panel:hover::after { height: 0%; }
.tech-panel:hover {
  box-shadow: 
    -5px 0 2px rgba(255,255,255,.1),
    0 -5px 2px rgba(255,255,255,.1),
    5px 0 2px rgba(255,255,255,.1),
    0 5px 2px rgba(255,255,255,.1),
    0 0 30px var(--glow-soft);
  background: rgba(0, 90, 140, 0.4);
}
.tech-panel > .mc-tl, .tech-panel > .mc-tr,
.tech-panel > .mc-bl, .tech-panel > .mc-br {
  width: 10px; height: 10px;
}

.tech-panel::selection { background: var(--primary-soft); }

.tech-panel-top-border {
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: var(--grad-main);
  background-size: 200% 100%;
  animation: top-border-flow 4s linear infinite;
  opacity: 0.9;
  z-index: 2;
  pointer-events: none;
}
@keyframes top-border-flow {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}

.tech-panel-inner-glow {
  position: absolute;
  inset: 1px;
  border-radius: 0;
  pointer-events: none;
  box-shadow: inset 0 0 30px rgba(11, 196, 233, 0.04);
  opacity: 0.6;
  transition: opacity 0.3s;
}
.tech-panel:hover .tech-panel-inner-glow {
  opacity: 1;
  box-shadow: inset 0 0 40px rgba(11, 196, 233, 0.08);
}

.tech-panel-header {
  display: flex; align-items: center; gap: 8px;
  padding-bottom: 8px; margin-bottom: 10px;
  border-bottom: 1px solid var(--border-inner);
  flex-shrink: 0;
  position: relative;
}
.tech-panel-header::after {
  content: '';
  position: absolute;
  bottom: -1px;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg, var(--primary), transparent 30%, transparent 70%, var(--secondary));
  background-size: 200% 100%;
  animation: header-line-flow 5s linear infinite;
  opacity: 0.8;
}
@keyframes header-line-flow {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}
.panel-bullet {
  position: relative;
  width: 5px; height: 22px;
  background: #59ebe8;
  border-radius: 0;
  flex-shrink: 0;
  box-shadow: 0 0 10px var(--glow), 0 0 20px var(--glow-soft);
}
.panel-bullet::after {
  content: ''; position: absolute;
  top: 50%; left: 50%; transform: translate(-50%, -50%);
  width: 6px; height: 6px;
  background: var(--primary);
  border-radius: 50%;
  opacity: 0.8;
  animation: bullet-pulse 2s ease-in-out infinite;
}
@keyframes bullet-pulse {
  0%, 100% { box-shadow: 0 0 4px var(--primary); opacity: 0.8; }
  50% { box-shadow: 0 0 12px var(--primary), 0 0 24px var(--glow); opacity: 1; }
}
.panel-title { 
  font-size: 16px; font-weight: 500; color: var(--text); 
  line-height: 35px;
  letter-spacing: 0.5px; flex: 1; 
  text-shadow: 0 0 15px var(--glow);
  padding-left: 4px;
}
.panel-tag {
  font-size: 10px; color: var(--primary);
  padding: 3px 10px;
  border: 1px solid var(--border);
  background: var(--primary-soft);
  letter-spacing: 0.5px; border-radius: 2px;
  box-shadow: 0 0 10px var(--glow-soft);
  font-weight: 600;
}
.tech-panel-body { flex: 1; min-height: 0; position: relative; }

/* KPI - 参考 1.css 风格 */
.kpi-grid { display: flex; flex-direction: column; gap: 12px; height: 100%; min-height: 0; overflow: hidden; }
.kpi-row-large { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.kpi-card-lg {
  position: relative;
  padding: 14px 16px;
  background: rgba(0, 72, 115, 0.28);
  border: 1px solid var(--border);
  border-radius: 0;
  display: flex; flex-direction: column; align-items: center; gap: 4px;
  overflow: hidden; transition: all 0.5s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.03);
}
.kpi-card-lg::before {
  content: '';
  position: absolute;
  top: 0; left: 0;
  width: 4px; height: 100%;
  background: var(--grad-accent);
  box-shadow: 0 0 10px var(--glow);
}
.kpi-card-lg::after {
  content: '';
  position: absolute;
  bottom: 0; left: 0; right: 0;
  height: 1px;
  background: linear-gradient(90deg, var(--primary), transparent 60%);
}
.kpi-card-lg:hover { 
  transform: translateY(-2px); 
  border-color: var(--border-hover); 
  box-shadow: 0 4px 20px var(--glow-soft), 0 0 30px var(--glow); 
}
.kpi-card-deco {
  position: absolute; top: 0; left: 0; right: 0; height: 2px;
  background: var(--grad-main);
  animation: deco-scan 3s linear infinite;
  box-shadow: 0 0 10px var(--glow);
}
@keyframes deco-scan { 0% { transform: translateX(-100%); } 100% { transform: translateX(100%); } }

.kpi-card-hex {
  position: absolute; right: -8px; top: 50%; transform: translateY(-50%);
  width: 55px; height: 55px; opacity: 0.4;
}
.kpi-card-hex span {
  position: absolute; inset: 0;
  border: 1.5px solid var(--primary);
  clip-path: polygon(50% 0, 100% 25%, 100% 75%, 50% 100%, 0 75%, 0 25%);
  animation: hex-rotate 10s linear infinite;
  box-shadow: 0 0 8px var(--glow);
}
.kpi-card-hex span:nth-child(2) { transform: scale(0.7); animation-direction: reverse; }
.kpi-card-hex span:nth-child(3) { transform: scale(0.4); }
@keyframes hex-rotate {
  0% { transform: rotate(0deg) scale(1); }
  100% { transform: rotate(360deg) scale(1); }
}

.kpi-lg-label { font-size: 12px; color: var(--text-dim); letter-spacing: 0.5px; z-index: 1; }
.kpi-lg-value {
  font-size: 36px; font-weight: bolder;
  color: #00a8d7;
  font-family: "等线", "DengXian", 'Consolas', monospace;
  text-shadow: 0 0 20px var(--glow), 0 0 40px var(--glow-soft);
  display: flex; align-items: baseline; gap: 2px;
  line-height: 1.1; z-index: 1;
  letter-spacing: 2px;
}
.kpi-lg-value .unit { 
  font-size: 14px; 
  color: var(--text-dim); 
  margin-left: 4px;
  text-shadow: 0 0 8px var(--glow-soft);
}
.kpi-lg-trend { font-size: 10px; padding: 2px 10px; border-radius: 2px; z-index: 1; }
.kpi-lg-trend.up { color: var(--danger); background: rgba(244, 63, 94, 0.2); border: 1px solid rgba(244, 63, 94, 0.5); box-shadow: 0 0 10px rgba(244, 63, 94, 0.3); }
.kpi-lg-trend.down { color: var(--success); background: rgba(16, 185, 129, 0.2); border: 1px solid rgba(16, 185, 129, 0.5); box-shadow: 0 0 10px rgba(16, 185, 129, 0.3); }
.trend-arrow { font-size: 9px; margin-right: 2px; }

/* 性别 - 参考 1.css 风格 */
.kpi-row-gender { display: flex; flex-direction: column; gap: 10px; }
.gender-item {
  display: flex; align-items: center; gap: 12px;
  padding: 10px 12px;
  background: rgba(0, 72, 115, 0.28);
  border: 1px solid var(--border-soft);
  border-radius: 0; transition: all 0.5s;
  position: relative; overflow: hidden;
}
.gender-item::before {
  content: ''; position: absolute; left: 0; top: 0; bottom: 0;
  width: 4px;
}
.gender-item.male::before { background: var(--grad-accent); box-shadow: 0 0 10px var(--glow); }
.gender-item.female::before { background: var(--grad-cyan); box-shadow: 0 0 10px var(--glow-soft); }
.gender-item:hover { border-color: var(--border); background: rgba(0, 90, 140, 0.4); transform: translateX(2px); }
.gender-icon {
  width: 32px; height: 32px; display: flex; align-items: center; justify-content: center;
  border-radius: 50%; font-size: 18px; font-weight: bold;
}
.gender-item.male .gender-icon { background: var(--primary-soft); color: var(--primary); box-shadow: 0 0 15px var(--glow-soft); }
.gender-item.female .gender-icon { background: var(--secondary-soft); color: var(--secondary); box-shadow: 0 0 15px var(--glow-soft); }
.gender-info { flex: 1; min-width: 0; }
.gender-label { display: block; font-size: 12px; color: var(--text-dim); margin-bottom: 2px; }
.gender-value { display: block; font-size: 22px; font-weight: bolder; font-family: "等线", 'Consolas', monospace; }
.gender-item.male .gender-value { color: var(--primary); text-shadow: 0 0 10px var(--glow-soft); }
.gender-item.female .gender-value { color: var(--secondary); text-shadow: 0 0 10px var(--glow-soft); }
.gender-bar { flex: 1; height: 6px; background: rgba(11, 196, 233, 0.08); border-radius: 0; overflow: hidden; min-width: 40px; }
.gender-bar-fill { height: 100%; border-radius: 0; box-shadow: 0 0 10px currentColor; transition: width 0.5s ease; }
.gender-item.male .gender-bar-fill { background: var(--grad-accent); }
.gender-item.female .gender-bar-fill { background: var(--grad-cyan); }

/* 小卡片 - 参考 1.css 风格 */
.kpi-row-small { display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 10px; }
.kpi-card-sm {
  position: relative;
  padding: 12px 10px;
  background: rgba(0, 72, 115, 0.28);
  border: 1px solid var(--border-soft);
  border-radius: 0; text-align: center; transition: all 0.5s;
  overflow: hidden;
}
.kpi-card-sm::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: var(--grad-main);
  opacity: 0.8;
}
.kpi-card-sm:hover { border-color: var(--border); background: var(--primary-soft); transform: translateY(-2px); box-shadow: 0 4px 20px var(--glow-soft); }
.kpi-sm-label { font-size: 12px; color: var(--text-dim); margin-bottom: 6px; }
.kpi-sm-value { 
  font-size: 24px; font-weight: bolder; 
  color: #00a8d7;
  font-family: "等线", 'Consolas', monospace; 
  text-shadow: 0 0 15px var(--glow-soft); 
  margin-bottom: 8px; 
  letter-spacing: 1px;
}
.kpi-sm-bar { height: 4px; background: rgba(11, 196, 233, 0.08); border-radius: 0; overflow: hidden; }
.kpi-sm-bar-fill { height: 100%; background: var(--grad-accent); border-radius: 0; box-shadow: 0 0 8px var(--glow-soft); }

.chart-area { width: 100%; height: 100%; min-height: 0; position: relative; }
.chart-area-glow {
  position: absolute;
  inset: 0;
  pointer-events: none;
  border-radius: 4px;
  box-shadow: inset 0 0 30px rgba(0, 229, 255, 0.04);
  opacity: 0.8;
}
.chart-area::after {
  content: '';
  position: absolute;
  inset: 0;
  pointer-events: none;
  background-image:
    linear-gradient(rgba(0, 229, 255, 0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 229, 255, 0.04) 1px, transparent 1px);
  background-size: 30px 30px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 70%);
  pointer-events: none;
  border-radius: 4px;
}

/* 地图 - 增强版 */
.map-area {
  position: relative;
  display: flex; flex-direction: column;
  min-height: 0;
  background: 
    radial-gradient(ellipse at 50% 40%, rgba(0, 100, 150, 0.15) 0%, transparent 60%),
    radial-gradient(ellipse at center, rgba(11, 196, 233, 0.08) 0%, transparent 70%);
  border: 1px solid var(--border);
  border-radius: 4px;
  overflow: hidden;
  box-shadow: 
    0 4px 30px rgba(0, 0, 0, 0.5),
    0 0 30px rgba(11, 196, 233, 0.15),
    inset 0 1px 0 rgba(255, 255, 255, 0.03);
  animation: map-glow-breathe 5s ease-in-out infinite;
  padding: 10px;
}
@keyframes map-glow-breathe {
  0%, 100% { box-shadow: 0 0 50px rgba(11, 196, 233, 0.2), inset 0 0 40px rgba(11, 196, 233, 0.06); }
  50% { box-shadow: 0 0 80px rgba(11, 196, 233, 0.35), 0 0 120px rgba(0, 168, 215, 0.15), inset 0 0 60px rgba(11, 196, 233, 0.1); }
}
.map-area::after {
  content: '';
  position: absolute;
  inset: 0;
  background:
    radial-gradient(circle at 20% 30%, var(--primary) 0px, transparent 2px),
    radial-gradient(circle at 80% 20%, var(--secondary) 0px, transparent 2px),
    radial-gradient(circle at 60% 80%, var(--accent) 0px, transparent 2px),
    radial-gradient(circle at 40% 50%, var(--primary) 0px, transparent 2px),
    radial-gradient(circle at 90% 60%, var(--secondary) 0px, transparent 2px);
  background-size: 200px 200px, 250px 250px, 180px 180px, 220px 220px, 300px 300px;
  animation: particle-flow 8s linear infinite;
  opacity: 0.5;
  pointer-events: none;
}
@keyframes particle-flow {
  0% { transform: translateY(0); }
  100% { transform: translateY(-200px); }
}
.map-canvas {
  width: 100%;
  height: 100%;
  position: relative;
  z-index: 2;
}
.map-loading {
  position: absolute;
  inset: 0;
  z-index: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 16px;
  background: var(--bg-card);
}
.map-loading-text { font-size: 13px; color: var(--text-dim); text-shadow: 0 0 10px var(--glow-soft); }
.map-deco { position: absolute; inset: 0; pointer-events: none; z-index: 3; }
.map-deco-ready .map-grid { opacity: 0.5; }
.map-deco-ready .map-corner { opacity: 1; }
.map-radar {
  position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%);
  width: 200px; height: 200px;
  border: 1px dashed rgba(0, 229, 255, 0.5);
  border-radius: 50%;
  animation: radar-spin 8s linear infinite;
  box-shadow: 0 0 30px rgba(0, 229, 255, 0.15);
}
.map-radar::before {
  content: ''; position: absolute; top: 50%; left: 50%;
  width: 50%; height: 1px;
  background: linear-gradient(90deg, var(--primary), transparent);
  transform-origin: left center;
  box-shadow: 0 0 10px var(--primary), 0 0 20px var(--glow);
}
@keyframes radar-spin { 0% { transform: translate(-50%, -50%) rotate(0); } 100% { transform: translate(-50%, -50%) rotate(360deg); } }

.map-radar-sweep {
  position: absolute;
  top: 50%; left: 50%;
  width: 120%; height: 120%;
  transform: translate(-50%, -50%);
  pointer-events: none;
  background: conic-gradient(from 0deg, transparent 0deg, #0bc4e9 30deg, transparent 60deg, transparent 180deg, #00a8d7 210deg, transparent 240deg);
  border-radius: 50%;
  opacity: 0.25;
  animation: radar-sweep-rotate 6s linear infinite;
  mix-blend-mode: screen;
}
@keyframes radar-sweep-rotate {
  from { transform: translate(-50%, -50%) rotate(0deg); }
  to { transform: translate(-50%, -50%) rotate(360deg); }
}

.map-pulse-ring {
  position: absolute;
  top: 50%; left: 50%;
  width: 100px; height: 100px;
  transform: translate(-50%, -50%);
  border: 2px solid var(--primary);
  border-radius: 50%;
  opacity: 0;
  pointer-events: none;
  animation: pulse-ring-expand 4s ease-out infinite;
}
.map-pulse-ring:nth-child(2) { animation-delay: -1.3s; border-color: var(--secondary); }
.map-pulse-ring:nth-child(3) { animation-delay: -2.6s; border-color: var(--accent); }
@keyframes pulse-ring-expand {
  0% { width: 50px; height: 50px; opacity: 0.8; }
  100% { width: 600px; height: 600px; opacity: 0; }
}

.map-grid {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(11, 196, 233, 0.06) 1px, transparent 1px),
    linear-gradient(90deg, rgba(11, 196, 233, 0.06) 1px, transparent 1px);
  background-size: 40px 40px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
}
.map-corner { position: absolute; width: 24px; height: 24px; border: 2px solid var(--primary); box-shadow: 0 0 12px var(--glow), 0 0 24px var(--glow-soft); transition: all 0.3s; }
.mc-tl { top: 12px; left: 12px; border-right: none; border-bottom: none; }
.mc-tr { top: 12px; right: 12px; border-left: none; border-bottom: none; }
.mc-bl { bottom: 12px; left: 12px; border-right: none; border-top: none; }
.mc-br { bottom: 12px; right: 12px; border-left: none; border-top: none; }

.map-data-strip {
  position: absolute; bottom: 0; left: 230px; right: 0;
  display: flex; justify-content: flex-end; gap: 20px;
  padding: 10px 16px;
  background: linear-gradient(180deg, transparent, rgba(6, 16, 28, 0.95));
  border-top: 1px solid var(--border-soft);
  font-size: 11px; color: var(--text-dim);
  font-family: 'Consolas', monospace;
  z-index: 5;
}
.map-data-strip b { color: var(--primary); font-weight: 700; text-shadow: 0 0 10px var(--glow-soft); }
.map-drill-tip {
  color: #ffffff;
  font-weight: 700;
  text-shadow: 0 0 10px rgba(11, 196, 233, 0.8), 0 0 20px rgba(11, 196, 233, 0.4);
  background: linear-gradient(90deg, rgba(11, 196, 233, 0.3), rgba(0, 168, 215, 0.3));
  border: 1px solid rgba(11, 196, 233, 0.6);
  padding: 4px 12px;
  border-radius: 2px;
  letter-spacing: 1px;
  animation: tip-pulse 2s ease-in-out infinite;
  box-shadow: 0 0 15px rgba(11, 196, 233, 0.4), inset 0 0 10px rgba(11, 196, 233, 0.1);
  cursor: pointer;
}
@keyframes tip-pulse {
  0%, 100% { 
    box-shadow: 0 0 15px rgba(11, 196, 233, 0.4), inset 0 0 10px rgba(11, 196, 233, 0.1);
    text-shadow: 0 0 10px rgba(11, 196, 233, 0.8), 0 0 20px rgba(11, 196, 233, 0.4);
  }
  50% { 
    box-shadow: 0 0 25px rgba(11, 196, 233, 0.7), inset 0 0 15px rgba(11, 196, 233, 0.2);
    text-shadow: 0 0 15px rgba(11, 196, 233, 1), 0 0 30px rgba(11, 196, 233, 0.6);
  }
}

.map-legend {
  position: absolute;
  bottom: 16px;
  left: 16px;
  z-index: 10;
  padding: 14px 18px;
  background: linear-gradient(180deg, rgba(6, 16, 28, 0.95) 0%, rgba(4, 10, 20, 0.92) 100%);
  border: 1px solid rgba(11, 196, 233, 0.5);
  border-radius: 0;
  box-shadow: 
    0 4px 20px rgba(0, 0, 0, 0.5),
    0 0 30px rgba(11, 196, 233, 0.15),
    inset 0 1px 0 rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(8px);
  display: flex;
  flex-direction: column;
  gap: 10px;
  min-width: 200px;
  pointer-events: auto;
}
.map-legend::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, #0bc4e9, #7dd87d, #f5c542, #e8554f);
  box-shadow: 0 0 10px rgba(11, 196, 233, 0.5);
}
.legend-title { 
  font-size: 13px; 
  color: #ffffff; 
  text-shadow: 0 0 10px rgba(11, 196, 233, 0.6); 
  font-weight: 700;
  letter-spacing: 1px;
  padding-bottom: 8px;
  border-bottom: 1px solid rgba(11, 196, 233, 0.2);
  font-family: "微软雅黑", sans-serif;
}
.legend-gradient-bar {
  width: 100%;
  height: 12px;
  border-radius: 2px;
  background: linear-gradient(90deg, 
    #0bc4e9 0%, #0bc4e9 48%, 
    #7dd87d 48%, #7dd87d 55%, 
    #f5c542 55%, #f5c542 60%, 
    #e8554f 60%, #e8554f 100%);
  box-shadow: 0 0 10px rgba(11, 196, 233, 0.3), inset 0 0 5px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(11, 196, 233, 0.3);
}
.legend-scale {
  display: flex;
  justify-content: space-between;
  font-size: 10px;
  color: #8adeff;
  margin-top: -2px;
  font-family: 'Consolas', monospace;
}
.legend-items { display: flex; flex-direction: column; gap: 6px; margin-top: 4px; }
.legend-item { 
  display: flex; 
  align-items: center; 
  gap: 10px; 
  font-size: 12px; 
  color: #8adeff; 
}
.legend-bar { 
  width: 20px; 
  height: 10px; 
  border-radius: 2px;
  box-shadow: 0 0 8px currentColor;
  flex-shrink: 0;
}
.legend-bar.bar-0 { background: #0bc4e9; color: #0bc4e9; }
.legend-bar.bar-1 { background: #0bc4e9; color: #0bc4e9; }
.legend-bar.bar-2 { background: #7dd87d; color: #7dd87d; }
.legend-bar.bar-3 { background: #f5c542; color: #f5c542; }
.legend-bar.bar-4 { background: #e8554f; color: #e8554f; }

.gauge-row { display: flex; gap: 14px; height: 100%; min-height: 0; }
.gauge-item { flex: 1; min-height: 0; }

/* 滚动条 - 增强版 */
::-webkit-scrollbar { width: 6px; height: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: var(--border); border-radius: 4px; }
::-webkit-scrollbar-thumb:hover { background: var(--primary); box-shadow: 0 0 8px var(--glow); }

/* 响应式 */
@media (max-width: 1500px) {
  .national-title { font-size: 24px; letter-spacing: 3px; }
  .kpi-lg-value { font-size: 24px; }
  .national-body { grid-template-columns: 26% 1fr 26%; padding: 8px 14px 14px; }
}
@media (max-width: 1200px) {
  .app-root { height: auto; min-height: 100vh; overflow-y: auto; }
  .national-body { grid-template-columns: 1fr; }
  .left-col, .right-col { flex-direction: row; flex-wrap: wrap; }
  .left-col .tech-panel, .right-col .tech-panel { flex: 1 1 300px; }
}
@media (max-width: 768px) {
  .national-title { font-size: 18px; letter-spacing: 2px; }
  .kpi-lg-value { font-size: 20px; }
  .national-header { padding: 10px 16px 6px; }
  .national-tabs { padding: 6px 12px; }
  .tech-tab { padding: 6px 14px; font-size: 12px; }
  .national-body { padding: 6px 12px 12px; gap: 8px; }
  .left-col, .right-col { flex-direction: column; }
  .header-deco { display: none; }
  .tech-panel-top-border { display: none; }
}

/* 额外的粒子动画层 */
.bg-particles-float {
  position: absolute; inset: 0; z-index: 1; pointer-events: none;
  background-image:
    radial-gradient(1px 1px at 15% 25%, rgba(11, 196, 233, 0.5) 50%, transparent 100%),
    radial-gradient(1px 1px at 45% 65%, rgba(0, 168, 215, 0.4) 50%, transparent 100%),
    radial-gradient(1px 1px at 75% 15%, rgba(89, 235, 232, 0.35) 50%, transparent 100%),
    radial-gradient(1px 1px at 85% 55%, rgba(14, 252, 255, 0.45) 50%, transparent 100%);
  background-size: 350px 350px, 420px 420px, 380px 380px, 450px 450px;
  animation: float-up 18s linear infinite;
  opacity: 0.5;
}
@keyframes float-up {
  0% { transform: translateY(0); }
  100% { transform: translateY(-100%); }
}

/* 浅色模式粒子效果禁用 */
.app-root.theme-light .bg-particles-float { display: none; }

/* 浅色模式面板增强 */
.app-root.theme-light .tech-panel {
  background: rgba(255, 255, 255, 0.98);
  border: 1px solid rgba(8, 145, 178, 0.12);
  box-shadow: 0 4px 20px rgba(15, 23, 42, 0.04);
}
.app-root.theme-light .tech-panel:hover {
  border-color: rgba(8, 145, 178, 0.25);
  box-shadow: 0 8px 28px rgba(8, 145, 178, 0.08);
}
.app-root.theme-light .tech-panel-header::after { display: none; }
.app-root.theme-light .panel-title { text-shadow: none; }

/* 浅色模式KPI卡片 */
.app-root.theme-light .kpi-card-lg {
  background: linear-gradient(135deg, rgba(8, 145, 178, 0.04), rgba(0, 168, 215, 0.02));
  border: 1px solid rgba(8, 145, 178, 0.1);
}
.app-root.theme-light .kpi-card-lg:hover {
  border-color: rgba(8, 145, 178, 0.2);
  box-shadow: 0 4px 16px rgba(8, 145, 178, 0.08);
}
.app-root.theme-light .kpi-card-sm {
  background: linear-gradient(135deg, rgba(8, 145, 178, 0.02), transparent);
  border: 1px solid rgba(8, 145, 178, 0.08);
}
.app-root.theme-light .kpi-card-sm:hover {
  border-color: rgba(8, 145, 178, 0.18);
  box-shadow: 0 4px 12px rgba(8, 145, 178, 0.06);
}
.app-root.theme-light .gender-item {
  background: rgba(8, 145, 178, 0.02);
  border: 1px solid rgba(8, 145, 178, 0.08);
}
.app-root.theme-light .gender-item:hover {
  background: rgba(8, 145, 178, 0.04);
  border-color: rgba(8, 145, 178, 0.15);
}

/* 浅色模式地图 */
.app-root.theme-light .map-area {
  background: linear-gradient(180deg, rgba(255,255,255,0.7), rgba(238,241,247,0.9));
  border: 1px solid rgba(8, 145, 178, 0.15);
  animation: none;
  box-shadow: 0 4px 24px rgba(15, 23, 42, 0.05);
}
.app-root.theme-light .map-area::after { display: none; }
.app-root.theme-light .map-grid {
  background-image:
    linear-gradient(rgba(8, 145, 178, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(8, 145, 178, 0.03) 1px, transparent 1px);
  opacity: 0.5;
}
.app-root.theme-light .map-corner {
  border-color: rgba(8, 145, 178, 0.4);
  box-shadow: none;
}
.app-root.theme-light .map-data-strip {
  background: linear-gradient(180deg, transparent, rgba(241, 245, 249, 0.98));
  border-top: 1px solid rgba(8, 145, 178, 0.1);
}
.app-root.theme-light .map-data-strip b { text-shadow: none; }
.app-root.theme-light .map-radar {
  border-color: rgba(8, 145, 178, 0.3);
  box-shadow: none;
}
.app-root.theme-light .map-radar::before {
  box-shadow: none;
}

/* 浅色模式标签页 */
.app-root.theme-light .tech-tab:hover {
  box-shadow: 0 4px 12px rgba(8, 145, 178, 0.1);
}
.app-root.theme-light .tech-tab.active {
  box-shadow: 0 4px 16px rgba(8, 145, 178, 0.2);
}

/* 浅色模式图例 */
.app-root.theme-light .map-legend {
  background: rgba(255, 255, 255, 0.98);
  border: 1px solid rgba(8, 145, 178, 0.15);
  box-shadow: 0 4px 20px rgba(15, 23, 42, 0.08);
}
.app-root.theme-light .legend-title {
  background: none;
  -webkit-text-fill-color: var(--text);
  color: var(--text);
  text-shadow: none;
  border-bottom-color: rgba(8, 145, 178, 0.15);
}
.app-root.theme-light .legend-gradient-bar {
  box-shadow: 0 0 4px rgba(8, 145, 178, 0.2);
  border-color: rgba(8, 145, 178, 0.2);
}
.app-root.theme-light .legend-scale {
  color: var(--text-dim);
}
.app-root.theme-light .legend-item {
  color: var(--text-dim);
}
</style>
