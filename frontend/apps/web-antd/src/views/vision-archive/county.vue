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
        <span class="title-text">{{ currentCounty }}中小学生健康监测大屏</span>
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
            <span class="panel-title">{{ currentCounty }}{{ currentMetric.name }}核心数据</span>
            <span class="panel-tag">{{ currentMetric.name }}专题</span>
          </header>
          <div class="tech-panel-body">
            <div class="kpi-grid">
              <div class="kpi-row-large">
                <div class="kpi-card-lg">
                  <div class="kpi-card-deco"></div>
                  <div class="kpi-card-hex"><span></span><span></span><span></span></div>
                  <div class="kpi-lg-label">{{ currentCounty }}{{ currentMetric.name }}率</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ currentCountyData.rate }}</span><span class="unit">%</span>
                  </div>
                  <div :class="['kpi-lg-trend', currentCountyData.trend > 0 ? 'up' : 'down']">
                    <span class="trend-arrow">{{ currentCountyData.trend > 0 ? '▲' : '▼' }}</span>
                    {{ Math.abs(currentCountyData.trend) }}% 同比
                  </div>
                </div>
                <div class="kpi-card-lg">
                  <div class="kpi-card-deco"></div>
                  <div class="kpi-card-hex"><span></span><span></span><span></span></div>
                  <div class="kpi-lg-label">学生总数（千）</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ currentCountyData.students }}</span>
                  </div>
                  <div class="kpi-lg-trend down">覆盖 {{ currentCountyData.schools }} 所学校</div>
                </div>
              </div>

              <div class="kpi-row-gender">
                <div class="gender-item male">
                  <span class="gender-icon">♂</span>
                  <div class="gender-info">
                    <span class="gender-label">男生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ currentCountyData.maleRate }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: currentCountyData.maleRate + '%' }"></div></div>
                </div>
                <div class="gender-item female">
                  <span class="gender-icon">♀</span>
                  <div class="gender-info">
                    <span class="gender-label">女生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ currentCountyData.femaleRate }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: currentCountyData.femaleRate + '%' }"></div></div>
                </div>
              </div>

              <div class="kpi-row-small">
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">小学{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentCountyData.primary }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentCountyData.primary + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">初中{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentCountyData.junior }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentCountyData.junior + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">高中{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentCountyData.senior }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentCountyData.senior + '%' }"></div></div>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">学校{{ currentMetric.name }}率排名</span>
            <span class="panel-tag">TOP 10</span>
          </header>
          <div class="tech-panel-body">
            <div ref="schoolRankRef" class="chart-area"></div>
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
              <span>经度：<b>{{ currentCountyCenter[0].toFixed(1) }}°E</b></span>
              <span>纬度：<b>{{ currentCountyCenter[1].toFixed(1) }}°N</b></span>
              <span>缩放：<b>{{ mapZoom.toFixed(2) }}x</b></span>
              <span v-if="mapLoaded" class="map-drill-tip">{{ currentCounty }}地图</span>
              <span v-else>加载中…</span>
            </div>
          </div>
          <button class="map-back-btn" @click="goBack">
            <span class="back-icon">◀</span> 返回上级地图
          </button>
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
};

const getColors = () => (isLight.value ? LIGHT_COLOR : DARK_COLOR);

const makeGrad = (c1, c2, horizontal = true) =>
  new echarts.graphic.LinearGradient(0, 0, horizontal ? 1 : 0, horizontal ? 0 : 1, [
    { offset: 0, color: c1 },
    { offset: 1, color: c2 },
  ]);

// ============================================================
//  区县/学校 地图与数据配置
// ============================================================
const COUNTY_MAP_CONFIG = {
  '370102': { name: '历下区', short: '历下区', center: [117.14, 36.65], zoom: 2.3 },
  '370103': { name: '市中区', short: '市中区', center: [117.10, 36.63], zoom: 2.3 },
  '370104': { name: '槐荫区', short: '槐荫区', center: [116.98, 36.65], zoom: 2.3 },
  '370105': { name: '天桥区', short: '天桥区', center: [117.02, 36.68], zoom: 2.3 },
  '370112': { name: '历城区', short: '历城区', center: [117.14, 36.67], zoom: 2.2 },
  '370113': { name: '长清区', short: '长清区', center: [116.75, 36.55], zoom: 2.2 },
  '370702': { name: '潍城区', short: '潍城区', center: [119.16, 36.71], zoom: 2.3 },
  '370704': { name: '坊子区', short: '坊子区', center: [119.17, 36.62], zoom: 2.3 },
  '370705': { name: '奎文区', short: '奎文区', center: [119.12, 36.71], zoom: 2.3 },
  '370703': { name: '寒亭区', short: '寒亭区', center: [119.16, 36.78], zoom: 2.3 },
};

const DISTRICT_SCHOOLS_MAP = {
  '370102': [
    '济南市历下区第一实验小学', '济南市历下区实验中学', '济南市历下区东方双语学校',
    '济南市历下区第二实验小学', '济南市历下区育英中学', '济南市历下区汇文学校',
    '济南市历下区燕翔小学', '济南市历下区济南五中', '济南市历下区解放路第一小学',
    '济南市历下区山大路小学', '济南市历下区文化东路小学', '济南市历下区建筑新村学校'
  ],
  '370103': [
    '济南市市中区经五路小学', '济南市市中区实验中学', '济南市市中区胜利大街小学',
    '济南市市中区济南中学', '济南市市中区育英小学', '济南市市中区十四中学',
    '济南市市中区南上山街小学', '济南市市中区七贤中学', '济南市市中区舜耕小学',
    '济南市市中区济南十六里河中学', '济南市市中区七里山小学', '济南市市中区党家镇中心小学'
  ],
  '370104': [
    '济南市槐荫区经十路小学', '济南市槐荫区实验中学', '济南市槐荫区南辛庄小学',
    '济南市槐荫区济南中学分校', '济南市槐荫区匡山小学', '济南市槐荫区二十七中',
    '济南市槐荫区张庄小学', '济南市槐荫区二机床学校', '济南市槐荫区古城中学',
    '济南市槐荫区段店小学'
  ],
  '370105': [
    '济南市天桥区无影山小学', '济南市天桥区实验中学', '济南市天桥区工人新村小学',
    '济南市天桥区十一中', '济南市天桥区堤口路小学', '济南市天桥区二十九中',
    '济南市天桥区师范路小学', '济南市天桥区双语实验学校', '济南市天桥区北坦小学',
    '济南市天桥区清河小学'
  ],
  '370112': [
    '济南市历城区洪家楼小学', '济南市历城区实验中学', '济南市历城区第二实验小学',
    '济南市历城区历城二中', '济南市历城区外国语学校', '济南市历城区大辛庄小学',
    '济南市历城区董家镇中心小学', '济南市历城区华山中学', '济南市历城区王舍人镇小学',
    '济南市历城区仲宫镇中心小学', '济南市历城区唐王镇小学', '济南市历城区郭店镇中心学校'
  ],
  '370113': [
    '济南市长清区实验小学', '济南市长清区实验中学', '济南市长清区第一中学',
    '济南市长清区石麟小学', '济南市长清区第二中学', '济南市长清区马山镇小学',
    '济南市长清区双泉镇中心小学', '济南市长清区张夏镇中学', '济南市长清区万德镇中心学校',
    '济南市长清区五峰山小学'
  ],
  '370702': [
    '潍坊市潍城区实验小学', '潍坊市潍城区实验中学', '潍坊市潍城区东风小学',
    '潍坊市潍城区潍坊七中', '潍坊市潍城区月河路小学', '潍坊市潍城区三中',
    '潍坊市潍城区永安路小学', '潍坊市潍城区芙蓉小学', '潍坊市潍城区健康街小学',
    '潍坊市潍城区西关小学'
  ],
  '370704': [
    '潍坊市坊子区实验小学', '潍坊市坊子区实验中学', '潍坊市坊子区第二实验小学',
    '潍坊市坊子区潍坊四中', '潍坊市坊子区凤凰小学', '潍坊市坊子区三马路小学',
    '潍坊市坊子区九龙涧学校', '潍坊市坊子区坊城小学', '潍坊市坊子区坊安小学',
    '潍坊市坊子区黄旗堡小学'
  ],
  '370705': [
    '潍坊市奎文区实验小学', '潍坊市奎文区实验中学', '潍坊市奎文区胜利东小学',
    '潍坊市奎文区广文中学', '潍坊市奎文区德信现代学校', '潍坊市奎文区新华中学',
    '潍坊市奎文区圣荣小学', '潍坊市奎文区东关育才小学', '潍坊市奎文区友谊小学',
    '潍坊市奎文区北苑实验学校'
  ],
  '370703': [
    '潍坊市寒亭区实验小学', '潍坊市寒亭区实验中学', '潍坊市寒亭区第一中学',
    '潍坊市寒亭区第二实验小学', '潍坊市寒亭区潍坊一中', '潍坊市寒亭区外国语学校',
    '潍坊市寒亭区朱里镇中心小学', '潍坊市寒亭区固堤镇小学', '潍坊市寒亭区高里镇学校',
    '潍坊市寒亭区央子镇中心小学'
  ],
};

const SCHOOL_CODES_MAP = {
  '济南市历下区第一实验小学': '370102001', '济南市历下区实验中学': '370102002',
  '济南市历下区东方双语学校': '370102003', '济南市历下区第二实验小学': '370102004',
  '济南市历下区育英中学': '370102005', '济南市历下区汇文学校': '370102006',
  '济南市历下区燕翔小学': '370102007', '济南市历下区济南五中': '370102008',
  '济南市历下区解放路第一小学': '370102009', '济南市历下区山大路小学': '370102010',
  '济南市历下区文化东路小学': '370102011', '济南市历下区建筑新村学校': '370102012',
  '济南市市中区经五路小学': '370103001', '济南市市中区实验中学': '370103002',
  '济南市市中区胜利大街小学': '370103003', '济南市市中区济南中学': '370103004',
  '济南市市中区育英小学': '370103005', '济南市市中区十四中学': '370103006',
  '济南市市中区南上山街小学': '370103007', '济南市市中区七贤中学': '370103008',
  '济南市市中区舜耕小学': '370103009', '济南市市中区济南十六里河中学': '370103010',
  '济南市市中区七里山小学': '370103011', '济南市市中区党家镇中心小学': '370103012',
  '济南市槐荫区经十路小学': '370104001', '济南市槐荫区实验中学': '370104002',
  '济南市槐荫区南辛庄小学': '370104003', '济南市槐荫区济南中学分校': '370104004',
  '济南市槐荫区匡山小学': '370104005', '济南市槐荫区二十七中': '370104006',
  '济南市槐荫区张庄小学': '370104007', '济南市槐荫区二机床学校': '370104008',
  '济南市槐荫区古城中学': '370104009', '济南市槐荫区段店小学': '370104010',
  '济南市天桥区无影山小学': '370105001', '济南市天桥区实验中学': '370105002',
  '济南市天桥区工人新村小学': '370105003', '济南市天桥区十一中': '370105004',
  '济南市天桥区堤口路小学': '370105005', '济南市天桥区二十九中': '370105006',
  '济南市天桥区师范路小学': '370105007', '济南市天桥区双语实验学校': '370105008',
  '济南市天桥区北坦小学': '370105009', '济南市天桥区清河小学': '370105010',
  '济南市历城区洪家楼小学': '370112001', '济南市历城区实验中学': '370112002',
  '济南市历城区第二实验小学': '370112003', '济南市历城区历城二中': '370112004',
  '济南市历城区外国语学校': '370112005', '济南市历城区大辛庄小学': '370112006',
  '济南市历城区董家镇中心小学': '370112007', '济南市历城区华山中学': '370112008',
  '济南市历城区王舍人镇小学': '370112009', '济南市历城区仲宫镇中心小学': '370112010',
  '济南市历城区唐王镇小学': '370112011', '济南市历城区郭店镇中心学校': '370112012',
  '济南市长清区实验小学': '370113001', '济南市长清区实验中学': '370113002',
  '济南市长清区第一中学': '370113003', '济南市长清区石麟小学': '370113004',
  '济南市长清区第二中学': '370113005', '济南市长清区马山镇小学': '370113006',
  '济南市长清区双泉镇中心小学': '370113007', '济南市长清区张夏镇中学': '370113008',
  '济南市长清区万德镇中心学校': '370113009', '济南市长清区五峰山小学': '370113010',
  '潍坊市潍城区实验小学': '370702001', '潍坊市潍城区实验中学': '370702002',
  '潍坊市潍城区东风小学': '370702003', '潍坊市潍城区潍坊七中': '370702004',
  '潍坊市潍城区月河路小学': '370702005', '潍坊市潍城区三中': '370702006',
  '潍坊市潍城区永安路小学': '370702007', '潍坊市潍城区芙蓉小学': '370702008',
  '潍坊市潍城区健康街小学': '370702009', '潍坊市潍城区西关小学': '370702010',
  '潍坊市坊子区实验小学': '370704001', '潍坊市坊子区实验中学': '370704002',
  '潍坊市坊子区第二实验小学': '370704003', '潍坊市坊子区潍坊四中': '370704004',
  '潍坊市坊子区凤凰小学': '370704005', '潍坊市坊子区三马路小学': '370704006',
  '潍坊市坊子区九龙涧学校': '370704007', '潍坊市坊子区坊城小学': '370704008',
  '潍坊市坊子区坊安小学': '370704009', '潍坊市坊子区黄旗堡小学': '370704010',
  '潍坊市奎文区实验小学': '370705001', '潍坊市奎文区实验中学': '370705002',
  '潍坊市奎文区胜利东小学': '370705003', '潍坊市奎文区广文中学': '370705004',
  '潍坊市奎文区德信现代学校': '370705005', '潍坊市奎文区新华中学': '370705006',
  '潍坊市奎文区圣荣小学': '370705007', '潍坊市奎文区东关育才小学': '370705008',
  '潍坊市奎文区友谊小学': '370705009', '潍坊市奎文区北苑实验学校': '370705010',
  '潍坊市寒亭区实验小学': '370703001', '潍坊市寒亭区实验中学': '370703002',
  '潍坊市寒亭区第一中学': '370703003', '潍坊市寒亭区第二实验小学': '370703004',
  '潍坊市寒亭区潍坊一中': '370703005', '潍坊市寒亭区外国语学校': '370703006',
  '潍坊市寒亭区朱里镇中心小学': '370703007', '潍坊市寒亭区固堤镇小学': '370703008',
  '潍坊市寒亭区高里镇学校': '370703009', '潍坊市寒亭区央子镇中心小学': '370703010',
};

// 学校坐标：以区县中心为圆心，按角度分散生成
const SCHOOL_COORDS_MAP = {};
const _countyCenters = {
  '370102': [117.14, 36.65], '370103': [117.10, 36.63], '370104': [116.98, 36.65],
  '370105': [117.02, 36.68], '370112': [117.14, 36.67], '370113': [116.75, 36.55],
  '370702': [119.16, 36.71], '370704': [119.17, 36.62], '370705': [119.12, 36.71],
  '370703': [119.16, 36.78],
};
Object.entries(DISTRICT_SCHOOLS_MAP).forEach(([code, schools]) => {
  const center = _countyCenters[code] || [117.12, 36.65];
  schools.forEach((name, i) => {
    const angle = (i / schools.length) * Math.PI * 2;
    const radius = 0.04 + (i % 3) * 0.015;
    const lng = Math.round((center[0] + Math.cos(angle) * radius) * 1000) / 1000;
    const lat = Math.round((center[1] + Math.sin(angle) * radius) * 1000) / 1000;
    SCHOOL_COORDS_MAP[name] = [lng, lat];
  });
});

// ============================================================
//  顶部筛选（省/市/区县/学校）数据源
// ============================================================
const PROVINCE_CODES = {
  '北京': '110000', '天津': '120000', '河北': '130000', '山西': '140000', '内蒙古': '150000',
  '辽宁': '210000', '吉林': '220000', '黑龙江': '230000', '上海': '310000', '江苏': '320000',
  '浙江': '330000', '安徽': '340000', '福建': '350000', '江西': '360000', '山东': '370000',
  '河南': '410000', '湖北': '420000', '湖南': '430000', '广东': '440000', '广西': '450000',
  '海南': '460000', '重庆': '500000', '四川': '510000', '贵州': '520000', '云南': '530000',
  '西藏': '540000', '陕西': '610000', '甘肃': '620000', '青海': '630000', '宁夏': '640000',
  '新疆': '650000', '台湾': '710000', '香港': '810000', '澳门': '820000',
};

const PROVINCE_CITY_MAP = {
  '370000': [
    { code: '370100', name: '济南市' },
    { code: '370700', name: '潍坊市' },
  ],
};

const CITY_DISTRICT_MAP = {
  '370100': [
    { code: '370102', name: '历下区' },
    { code: '370103', name: '市中区' },
    { code: '370104', name: '槐荫区' },
    { code: '370105', name: '天桥区' },
    { code: '370112', name: '历城区' },
    { code: '370113', name: '长清区' },
  ],
  '370700': [
    { code: '370702', name: '潍城区' },
    { code: '370704', name: '坊子区' },
    { code: '370705', name: '奎文区' },
    { code: '370703', name: '寒亭区' },
  ],
};

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

const selectOption = (key, value) => {
  if (key === 'province') {
    filter.province = value;
    filter.city = '';
    filter.district = '';
    filter.school = '';
  } else if (key === 'city') {
    filter.city = value;
    filter.district = '';
    filter.school = '';
  } else if (key === 'district') {
    filter.district = value;
    filter.school = '';
  } else if (key === 'school') {
    filter.school = value;
    const code = String(value);
    if (code.length >= 6) {
      const provCode = code.substring(0, 2) + '0000';
      const cityCode = code.substring(0, 4) + '00';
      const distCode = code.substring(0, 6);
      if (PROVINCE_CITY_MAP[provCode]) {
        filter.province = provCode;
        if (PROVINCE_CITY_MAP[provCode].some(c => c.code === cityCode)) {
          filter.city = cityCode;
          if (CITY_DISTRICT_MAP[cityCode] && CITY_DISTRICT_MAP[cityCode].some(d => d.code === distCode)) {
            filter.district = distCode;
          }
        }
      }
    }
  }
  openDropdown.value = null;
  renderAllCharts();
};

const getSelectedName = (list, code) => {
  if (!code) return '';
  const found = list.find(item => item.code === code);
  return found ? found.name : '';
};

const clearSchool = () => { filter.school = ''; };
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

const cityOptions = computed(() => PROVINCE_CITY_MAP[filter.province] || []);

const districtOptions = computed(() => CITY_DISTRICT_MAP[filter.city] || []);

const schoolOptions = computed(() => {
  const districtCode = filter.district || routeCode.value;
  const list = DISTRICT_SCHOOLS_MAP[districtCode] || [];
  return list.map(name => ({ code: SCHOOL_CODES_MAP[name] || name, name }));
});

const handleSearch = () => {
  openDropdown.value = null;
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
  openDropdown.value = null;
};

// ============================================================
//  路由驱动的区县配置 + 学校列表
// ============================================================
const routeCode = computed(() => String(route.params.code || '370102'));
const currentCountyConfig = computed(() => {
  if (COUNTY_MAP_CONFIG[routeCode.value]) return COUNTY_MAP_CONFIG[routeCode.value];
  const code = routeCode.value;
  return { name: code, short: code, center: [117.12, 36.65], zoom: 2.0 };
});
const currentCounty = computed(() => currentCountyConfig.value.short);
const currentCountyCenter = computed(() => currentCountyConfig.value.center);

// 根据路由参数同步筛选条件（从 city.vue 地图下钻进入时生效）
// 区县码前两位 + '0000' 为省份码，前四位 + '00' 为城市码
const syncFilterFromRoute = () => {
  const code = routeCode.value;
  if (code && code.length >= 6) {
    const provinceCode = code.slice(0, 2) + '0000';
    const cityCode = code.slice(0, 4) + '00';
    if (Object.values(PROVINCE_CODES).includes(provinceCode)) {
      filter.province = provinceCode;
      filter.city = cityCode;
      filter.district = code;
      filter.school = '';
    }
  }
};

const schoolList = computed(() => {
  const districtCode = filter.district || routeCode.value;
  const list = DISTRICT_SCHOOLS_MAP[districtCode] || DISTRICT_SCHOOLS_MAP[routeCode.value] || [];
  if (filter.school) {
    const schoolName = Object.keys(SCHOOL_CODES_MAP).find(k => SCHOOL_CODES_MAP[k] === filter.school);
    if (schoolName) return [schoolName];
  }
  return list;
});

// ============================================================
//  状态
// ============================================================
const activeTab = ref('vision');
const mapLoaded = ref(false);
const currentDate = ref('');
const currentTime = ref('');
const isLight = ref(false);
const mapZoom = ref(1.0);

const schoolRankRef = ref(null);
const ageGenderRef = ref(null);
const urbanGaugeRef = ref(null);
const ruralGaugeRef = ref(null);
const interventionRef = ref(null);
const mapRef = ref(null);

const chartInstances = {};
let timer = null;
let themeObserver = null;

const metricConfig = {
  vision: { name: '近视', baseRate: 55, trend: 2.0, maleFactor: 0.95, femaleFactor: 1.05, primaryFactor: 0.6, juniorFactor: 1.0, seniorFactor: 1.25 },
  oral: { name: '龋齿', baseRate: 40, trend: -1.0, maleFactor: 0.96, femaleFactor: 1.05, primaryFactor: 1.24, juniorFactor: 0.9, seniorFactor: 0.67 },
  mental: { name: '心理预警', baseRate: 16, trend: -0.6, maleFactor: 0.92, femaleFactor: 1.1, primaryFactor: 0.67, juniorFactor: 1.22, seniorFactor: 1.56 },
  weight: { name: '超重/肥胖', baseRate: 22, trend: 1.2, maleFactor: 1.12, femaleFactor: 0.88, primaryFactor: 0.63, juniorFactor: 1.17, seniorFactor: 1.33 },
  bone: { name: '骨密度偏低', baseRate: 14, trend: -0.4, maleFactor: 0.91, femaleFactor: 1.11, primaryFactor: 0.75, juniorFactor: 1.13, seniorFactor: 1.38 }
};

const currentMetric = computed(() => metricConfig[activeTab.value]);

const computeCountyMetricData = () => {
  const m = metricConfig[activeTab.value];
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rate = Math.round((m.baseRate + (seed % 12 - 6)) * 10) / 10;
  const students = Math.round(30 + seed * 3);
  const schools = schoolList.value.length || 10;
  const maleRate = Math.round(rate * m.maleFactor * 10) / 10;
  const femaleRate = Math.round(rate * m.femaleFactor * 10) / 10;
  const primary = Math.round(rate * m.primaryFactor * 10) / 10;
  const junior = Math.round(rate * m.juniorFactor * 10) / 10;
  const senior = Math.round(rate * m.seniorFactor * 10) / 10;
  const sortedSchools = [...schoolList.value].sort((a, b) => {
    const ra = Math.round((rate + (schoolList.value.indexOf(a) % 8 - 4)) * 10) / 10;
    const rb = Math.round((rate + (schoolList.value.indexOf(b) % 8 - 4)) * 10) / 10;
    return rb - ra;
  });
  const topSchool = sortedSchools[0] || '最高';
  const topRate = Math.round((rate + 4 + (seed % 6)) * 10) / 10;
  return { rate, students, schools, trend: m.trend, maleRate, femaleRate, primary, junior, senior, topSchool, topRate };
};

const currentCountyData = computed(() => computeCountyMetricData());

// ============================================================
//  方法
// ============================================================
// 可视化大屏始终使用深色主题，不受系统深浅色模式影响
const applySystemTheme = () => {
  isLight.value = false;
  setTimeout(() => renderAllCharts(), 300);
};

const observeTheme = () => {
  // 不再监听系统主题变化，保持深色背景不受系统颜色影响
  themeObserver = null;
};

const drillToSchool = (name) => {
  const code = SCHOOL_CODES_MAP[name];
  if (code) router.push({ path: `/vision/school/${code}`, query: { tab: activeTab.value } });
  else console.warn('未找到学校编码:', name);
};

const goBack = () => {
  const code = routeCode.value;
  const cityCode = code.slice(0, 4) + '00';
  router.push({ path: `/vision/city/${cityCode}`, query: { tab: activeTab.value } });
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
      : 'box-shadow: 0 0 20px rgba(11,196,233,0.35); border-radius: 6px; backdrop-filter: blur(8px);',
  };
};

// 学校排名（左栏）
const getRankingOption = () => {
  const c = getColors();
  const schools = schoolList.value.length ? schoolList.value.slice(0, 10) : ['暂无数据'];
  const baseRate = currentCountyData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rates = schools.map((_, i) => Math.round((baseRate + (12 - i * 1.2) + ((seed + i * 3) % 5 - 2)) * 10) / 10);
  const maxRate = Math.max(...rates);

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        const p = params[0];
        return `<div style="font-weight:600">${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div><div style="font-size:11px;color:${c.textDim};margin-top:4px">点击查看学校详情</div>`;
      },
    },
    grid: { left: 10, right: 50, top: 5, bottom: 5, containLabel: true },
    xAxis: { type: 'value', show: false, max: maxRate + 5 },
    yAxis: {
      type: 'category',
      data: schools,
      inverse: true,
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold', interval: 0 },
    },
    series: [{
      name: currentMetric.value.name + '率',
      type: 'bar',
      barGap: '30%',
      barCategoryGap: '40%',
      data: rates.map(v => ({
        value: v,
        itemStyle: {
          color: makeGrad(c.primary, c.secondary),
          borderRadius: [0, 4, 4, 0],
          shadowBlur: isLight.value ? 4 : 8,
          shadowColor: `${c.primary}44`,
        },
      })),
      barWidth: 12,
      label: {
        show: true,
        position: 'right',
        color: c.primary,
        fontSize: 10,
        fontWeight: 'bold',
        formatter: '{c}%',
      },
    }],
  };
};

const getAgeGenderOption = () => {
  const c = getColors();
  const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
  const baseRate = currentCountyData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const d = {
    grades,
    male: grades.map((_, i) => Math.round((baseRate * 0.95 + i * 5 + ((seed + i * 3) % 6 - 3)) * 10) / 10),
    female: grades.map((_, i) => Math.round((baseRate * 1.05 + i * 6 + ((seed + i * 2) % 5 - 2)) * 10) / 10)
  };
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
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const base = currentCountyData.value.rate;
  const data = [
    { name: '示范区A', initial: Math.round((base + 8 + (seed % 5)) * 10) / 10, final: Math.round((base + 1 - (seed % 3)) * 10) / 10, change: -Math.round((7 + (seed % 3)) * 10) / 10 },
    { name: '示范区B', initial: Math.round((base + 12 + (seed % 4)) * 10) / 10, final: Math.round((base + 3 - (seed % 4)) * 10) / 10, change: -Math.round((9 + (seed % 2)) * 10) / 10 },
    { name: '示范区C', initial: Math.round((base + 5 + (seed % 3)) * 10) / 10, final: Math.round((base + 1 - (seed % 2)) * 10) / 10, change: -Math.round((5 + (seed % 4)) * 10) / 10 },
    { name: '示范区D', initial: Math.round((base + 10 + (seed % 2)) * 10) / 10, final: Math.round((base + 2 - (seed % 3)) * 10) / 10, change: -Math.round((8 + (seed % 2)) * 10) / 10 },
  ];
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

// 区县地图：学校散点 + 中心连线，散点按 4 段分级着色（与图例一致）
const getMapOption = () => {
  if (!mapLoaded.value) return {};
  const c = getColors();

  const allSchools = schoolList.value.length > 0
    ? schoolList.value
    : (DISTRICT_SCHOOLS_MAP[routeCode.value] || []);

  const baseRate = currentCountyData.value?.rate || 50;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const data = allSchools.map((name, i) => ({
    name,
    value: Math.round((baseRate + ((seed + i * 5) % 18 - 9)) * 10) / 10
  }));

  if (data.length === 0) return {};

  const values = data.map(d => d.value);
  const maxV = Math.max(...values);
  const cfg = currentCountyConfig.value;

  const colorForRate = (v) => {
    const cols = c.mapColors;
    if (v < 48) return cols[0];
    if (v < 55) return cols[1];
    if (v < 60) return cols[2];
    return cols[3];
  };

  const scatterData = data.map(d => ({
    name: d.name,
    value: [...(SCHOOL_COORDS_MAP[d.name] || cfg.center), d.value],
    itemStyle: { color: colorForRate(d.value) },
  }));

  const lineData = data.map(d => ({
    coords: [cfg.center, (SCHOOL_COORDS_MAP[d.name] || cfg.center)],
    value: d.value,
    name: d.name,
  }));

  const arrowSymbol = 'path://M2,-10 L6,-2 L2,-2 L2,10 L-2,10 L-2,-2 L-6,-2 Z';

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('item'),
      formatter: (p) => {
        if (p.seriesType === 'effectScatter' || p.seriesType === 'scatter') {
          const v = p.value;
          return `<div style="font-weight:600">${p.name}</div>
            <div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${v[2]}%</span></div>
            <div style="color:${c.textDim};font-size:11px;margin-top:4px">📍 坐标：${v[0].toFixed(2)}°E, ${v[1].toFixed(2)}°N</div>
            <div style="color:${c.accent};font-size:11px;margin-top:4px">点击查看学校详情 →</div>`;
        }
        if (p.seriesType === 'lines') {
          return `<div style="font-weight:600">${p.name}</div>
            <div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>
            <div style="color:${c.textDim};font-size:11px;margin-top:4px">点击查看学校详情 →</div>`;
        }
        return `<div style="font-weight:600">${p.name}</div>`;
      },
    },
    geo: {
      map: 'county',
      roam: true,
      selectedMode: false,
      layoutCenter: ['50%', '50%'],
      layoutSize: '92%',
      zoom: 1,
      label: {
        show: true,
        color: c.mapLabel,
        fontSize: 10,
        fontWeight: '500',
        textShadowColor: isLight.value ? 'rgba(0,0,0,0.6)' : 'rgba(0,0,0,0.8)',
        textShadowBlur: 4,
      },
      itemStyle: {
        borderColor: c.mapBorder,
        borderWidth: 1.2,
        areaColor: c.mapArea,
        shadowBlur: isLight.value ? 6 : 12,
        shadowColor: `${c.primary}44`,
      },
      emphasis: {
        label: { color: '#ffffff', fontSize: 12, fontWeight: 700 },
        itemStyle: {
          areaColor: isLight.value ? 'rgba(241, 245, 249, 0.85)' : 'rgba(11, 196, 233, 0.35)',
          borderColor: isLight.value ? 'rgba(8, 145, 178, 0.3)' : c.primary,
          borderWidth: 2,
          shadowBlur: 20,
          shadowColor: `${c.primary}aa`,
        },
      },
    },
    series: [
      {
        type: 'lines',
        coordinateSystem: 'geo',
        zlevel: 1,
        effect: {
          show: true,
          period: 5,
          trailLength: 0.3,
          symbol: 'arrow',
          symbolSize: 6,
          color: c.primary,
        },
        lineStyle: {
          color: c.primary,
          width: 1,
          opacity: 0.4,
          curveness: 0.15,
        },
        data: lineData,
      },
      {
        type: 'effectScatter',
        coordinateSystem: 'geo',
        data: scatterData,
        symbolSize: (val) => Math.max(10, Math.min(18, (val[2] || 0) / Math.max(maxV, 1) * 18 + 6)),
        showEffectOn: 'render',
        rippleEffect: { brushType: 'stroke', scale: 4, period: 3 },
        label: {
          show: true,
          formatter: '{b}',
          position: 'right',
          distance: 6,
          color: c.text,
          fontSize: 10,
          fontWeight: '500',
          textShadowColor: isLight.value ? 'rgba(255,255,255,0.95)' : 'rgba(0,0,0,0.85)',
          textShadowBlur: 4,
        },
        itemStyle: {
          shadowBlur: 15,
          shadowColor: c.primary,
          borderColor: isLight.value ? '#fff' : c.bg,
          borderWidth: 2,
        },
        emphasis: {
          scale: 1.4,
          itemStyle: { shadowBlur: 25, borderWidth: 3 },
          label: { fontSize: 12, fontWeight: 700 },
        },
        zlevel: 3,
      },
      {
        type: 'scatter',
        coordinateSystem: 'geo',
        data: scatterData,
        symbol: arrowSymbol,
        symbolSize: 10,
        symbolRotate: 0,
        silent: false,
        tooltip: { show: false },
        itemStyle: {
          color: c.accent,
          shadowBlur: 8,
          shadowColor: c.accent,
        },
        emphasis: {
          scale: 1.5,
          itemStyle: { color: '#fff', shadowBlur: 15 },
        },
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
      const d = currentCountyData.value;
      const ur = { urban: Math.round((d.rate + 3) * 10) / 10, rural: Math.round((d.rate - 5) * 10) / 10 };
      initChart(schoolRankRef, 'schoolRank', getRankingOption(), (params) => {
        if (params?.name) drillToSchool(params.name);
      });
      initChart(ageGenderRef, 'ageGender', getAgeGenderOption());
      initChart(urbanGaugeRef, 'urbanGauge', getGaugeOption(ur.urban, `城区${currentMetric.value.name}率`, c.primary));
      initChart(ruralGaugeRef, 'ruralGauge', getGaugeOption(ur.rural, `县乡${currentMetric.value.name}率`, c.secondary));
      initChart(interventionRef, 'intervention', getInterventionOption());
      if (mapLoaded.value) {
        initChart(mapRef, 'map', getMapOption(), (params) => {
          if (params?.name) drillToSchool(params.name);
        });
        // 地图漫游时同步缩放显示
        const mapChart = chartInstances['map'];
        if (mapChart) {
          mapChart.on('georoam', () => {
            const option = mapChart.getOption();
            if (option && option.geo && option.geo[0] && option.geo[0].zoom != null) {
              mapZoom.value = option.geo[0].zoom;
            }
          });
        }
      }
    });
  });
};

const loadMap = async () => {
  const code = routeCode.value;
  const urls = [
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}_full.json`,
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}.json`,
  ];
  for (const url of urls) {
    try {
      const res = await fetch(url);
      if (!res.ok) continue;
      const geoJson = await res.json();
      echarts.registerMap('county', geoJson);
      mapLoaded.value = true;
      mapZoom.value = 1.0;
      setTimeout(() => renderAllCharts(), 200);
      return;
    } catch (e) {
      console.warn('地图加载失败', url, e);
    }
  }
  mapLoaded.value = true;
  setTimeout(() => renderAllCharts(), 200);
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
  syncFilterFromRoute();
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
watch(() => route.params.code, () => {
  mapLoaded.value = false;
  syncFilterFromRoute();
  setTimeout(() => loadMap(), 100);
});

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

.map-back-btn {
  position: absolute;
  left: 16px;
  top: 16px;
  z-index: 10;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 7px 16px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 4px;
  color: var(--text);
  font-size: 13px;
  font-family: inherit;
  cursor: pointer;
  transition: all 0.3s;
  backdrop-filter: blur(8px);
  box-shadow: 0 0 12px var(--glow-soft);
}
.map-back-btn:hover {
  border-color: var(--primary);
  color: var(--primary);
  box-shadow: 0 0 18px var(--glow);
  transform: translateX(-2px);
}
.map-back-btn .back-icon {
  font-size: 11px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: var(--primary-soft);
  color: var(--primary);
}
.app-root.theme-light .map-back-btn {
  background: rgba(255, 255, 255, 0.95);
  border-color: rgba(8, 145, 178, 0.15);
  box-shadow: 0 4px 12px rgba(15, 23, 42, 0.06);
}
.app-root.theme-light .map-back-btn:hover {
  border-color: var(--primary);
  color: var(--primary);
  box-shadow: 0 4px 16px rgba(8, 145, 178, 0.15);
}
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
