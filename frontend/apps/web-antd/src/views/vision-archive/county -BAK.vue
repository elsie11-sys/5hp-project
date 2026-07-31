<template>
  <div class="app-root" :class="{ 'theme-light': isLight }">
    <div class="bg-decor" aria-hidden="true">
      <div class="bg-grid"></div>
      <div class="bg-glow bg-glow-1"></div>
      <div class="bg-glow bg-glow-2"></div>
      <div class="bg-glow bg-glow-3"></div>
      <div class="bg-stars"></div>
    </div>
    <div class="scan-line"></div>
    <div class="scan-line-2"></div>

    <div class="tech-topbar">
      <div class="tech-corner tech-corner-l"><span class="tc-dot"></span></div>
      <div class="tech-line"><span class="line-pulse line-pulse-1"></span><span class="line-pulse line-pulse-2"></span></div>
      <div class="tech-dots">
        <span></span><span></span><span></span><span></span><span></span><span></span><span></span>
      </div>
      <div class="tech-line"><span class="line-pulse line-pulse-3"></span><span class="line-pulse line-pulse-4"></span></div>
      <div class="tech-corner tech-corner-r"><span class="tc-dot"></span></div>
    </div>
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
    <nav class="national-tabs">
      <button v-for="t in tabs" :key="t.key"
        :class="['tech-tab', { active: activeTab === t.key }]"
        @click="switchTab(t.key)">
        <span class="tech-tab-icon">{{ t.icon }}</span>
        <span class="tech-tab-label">{{ t.label }}</span>
        <span class="tech-tab-glow"></span>
      </button>
    </nav>

    <div class="top-filter-bar">
      <div class="filter-select" :class="{ 'has-value': currentFilter.province, 'is-open': openDropdown === 'province' }">
        <div class="select-trigger" @click.stop="toggleDropdown('province')">
          <span class="select-value" :class="{ placeholder: !currentFilter.province }">
            {{ getSelectedName(provinceList, currentFilter.province) || '请选择省份' }}
          </span>
          <span v-if="currentFilter.province" class="clear-icon" @click.stop="clearProvince">✕</span>
          <span class="arrow-down" :class="{ rotated: openDropdown === 'province' }">▼</span>
        </div>
        <div v-if="openDropdown === 'province'" class="select-dropdown">
          <div class="dropdown-header">选择省份</div>
          <div class="dropdown-list">
            <div v-for="p in provinceList" :key="p.code"
              class="dropdown-item" :class="{ selected: currentFilter.province === p.code }"
              @click.stop="selectOption('province', p.code)">
              {{ p.name }}
              <span v-if="currentFilter.province === p.code" class="check-mark">✓</span>
            </div>
          </div>
        </div>
      </div>

      <div class="filter-select" :class="{ 'has-value': currentFilter.city, 'is-open': openDropdown === 'city', 'disabled': !currentFilter.province }">
        <div class="select-trigger" :class="{ disabled: !currentFilter.province }" @click.stop="toggleDropdown('city')">
          <span class="select-value" :class="{ placeholder: !currentFilter.city }">
            {{ getSelectedName(cityOptions, currentFilter.city) || '请选择城市' }}
          </span>
          <span v-if="currentFilter.city" class="clear-icon" @click.stop="clearCity">✕</span>
          <span class="arrow-down" :class="{ rotated: openDropdown === 'city' }">▼</span>
        </div>
        <div v-if="openDropdown === 'city'" class="select-dropdown">
          <div class="dropdown-header">选择城市</div>
          <div class="dropdown-list">
            <div v-for="c in cityOptions" :key="c.code"
              class="dropdown-item" :class="{ selected: currentFilter.city === c.code }"
              @click.stop="selectOption('city', c.code)">
              {{ c.name }}
              <span v-if="currentFilter.city === c.code" class="check-mark">✓</span>
            </div>
            <div v-if="cityOptions.length === 0" class="dropdown-empty">请先选择省份</div>
          </div>
        </div>
      </div>

      <div class="filter-select" :class="{ 'has-value': currentFilter.district, 'is-open': openDropdown === 'district', 'disabled': !currentFilter.city }">
        <div class="select-trigger" :class="{ disabled: !currentFilter.city }" @click.stop="toggleDropdown('district')">
          <span class="select-value" :class="{ placeholder: !currentFilter.district }">
            {{ getSelectedName(districtOptions, currentFilter.district) || '请选择区县' }}
          </span>
          <span v-if="currentFilter.district" class="clear-icon" @click.stop="clearDistrict">✕</span>
          <span class="arrow-down" :class="{ rotated: openDropdown === 'district' }">▼</span>
        </div>
        <div v-if="openDropdown === 'district'" class="select-dropdown">
          <div class="dropdown-header">选择区县</div>
          <div class="dropdown-list">
            <div v-for="d in districtOptions" :key="d.code"
              class="dropdown-item" :class="{ selected: currentFilter.district === d.code }"
              @click.stop="selectOption('district', d.code)">
              {{ d.name }}
              <span v-if="currentFilter.district === d.code" class="check-mark">✓</span>
            </div>
            <div v-if="districtOptions.length === 0" class="dropdown-empty">请先选择城市</div>
          </div>
        </div>
      </div>

      <div class="filter-select" :class="{ 'has-value': currentFilter.school, 'is-open': openDropdown === 'school', 'disabled': !currentFilter.city }">
        <div class="select-trigger" :class="{ disabled: !currentFilter.city }" @click.stop="toggleDropdown('school')">
          <span class="select-value" :class="{ placeholder: !currentFilter.school }">
            {{ getSelectedName(schoolOptions, currentFilter.school) || '请选择学校' }}
          </span>
          <span v-if="currentFilter.school" class="clear-icon" @click.stop="clearSchool">✕</span>
          <span class="arrow-down" :class="{ rotated: openDropdown === 'school' }">▼</span>
        </div>
        <div v-if="openDropdown === 'school'" class="select-dropdown">
          <div class="dropdown-header">选择学校</div>
          <div class="dropdown-list">
            <div v-for="s in schoolOptions" :key="s.code"
              class="dropdown-item" :class="{ selected: currentFilter.school === s.code }"
              @click.stop="selectOption('school', s.code)">
              {{ s.name }}
              <span v-if="currentFilter.school === s.code" class="check-mark">✓</span>
            </div>
            <div v-if="schoolOptions.length === 0" class="dropdown-empty">请先选择区县</div>
          </div>
        </div>
      </div>

      <button class="btn-search-top" @click="handleSearch">
        <span class="icon">🔍</span> 搜索
      </button>
      <button class="btn-reset-top" @click="handleReset">
        <span class="icon">↺</span> 重置
      </button>
    </div>

    <main class="national-body">
      <aside class="left-col">
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">{{ currentMetric.name }}核心数据</span>
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
            <span class="panel-tag">TOP {{ schoolList.length > 0 ? Math.min(10, schoolList.length) : 10 }}</span>
          </header>
          <div class="tech-panel-body">
            <div ref="rankingChartRef" class="chart-area"></div>
          </div>
        </section>
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">预警提醒</span>
          </header>
          <div class="tech-panel-body">
            <div class="alert-list">
              <div class="alert-item danger">
                <span class="alert-icon">🔴</span>
                <span class="alert-msg">{{ currentCountyData.topSchool }}{{ currentMetric.name }}率达{{ currentCountyData.topRate }}%，高于全区平均水平</span>
              </div>
              <div class="alert-item warning">
                <span class="alert-icon">🟡</span>
                <span class="alert-msg">3所学校{{ currentMetric.name }}监测覆盖率低于40%，需加强推进</span>
              </div>
              <div class="alert-item info">
                <span class="alert-icon">🔵</span>
                <span class="alert-msg">本月{{ currentMetric.name }}检测完成率85.2%，较上月提升3.8%</span>
              </div>
            </div>
          </div>
        </section>
      </aside>
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
        </div>
        <div class="map-legend">
          <div class="legend-title">{{ currentMetric.name }}率分布</div>
          <div class="legend-items">
            <div class="legend-item"><span class="legend-dot" style="--lgc:1"></span><span>较低</span></div>
            <div class="legend-item"><span class="legend-dot" style="--lgc:2"></span><span>中等</span></div>
            <div class="legend-item"><span class="legend-dot" style="--lgc:3"></span><span>较高</span></div>
            <div class="legend-item"><span class="legend-dot" style="--lgc:4"></span><span>高发</span></div>
          </div>
        </div>
      </section>
      <aside class="right-col">
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">年龄/性别{{ currentMetric.name }}率对比</span>
          </header>
          <div class="tech-panel-body">
            <div ref="ageGenderChartRef" class="chart-area"></div>
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
            <span class="panel-title">{{ currentMetric.name }}变化趋势</span>
          </header>
          <div class="tech-panel-body">
            <div ref="trendChartRef" class="chart-area"></div>
          </div>
        </section>
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">{{ currentMetric.name }}防控效果</span>
          </header>
          <div class="tech-panel-body">
            <div ref="interventionChartRef" class="chart-area"></div>
          </div>
        </section>
      </aside>
    </main>

  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onBeforeUnmount, nextTick, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import * as echarts from 'echarts';

const router = useRouter();
const route = useRoute();

const DARK_COLOR = {
  primary: '#38bdf8', secondary: '#a78bfa', accent: '#f472b6',
  warning: '#fbbf24', success: '#34d399', danger: '#fb7185',
  male: '#38bdf8', female: '#a78bfa',
  text: '#e2e8f0', textDim: '#94a3b8',
  bg: 'rgba(10, 22, 50, 0.96)', mapArea: 'rgba(15, 30, 65, 0.85)',
  mapBorder: 'rgba(56, 189, 248, 0.75)', mapLabel: '#e2e8f0',
  splitLine: 'rgba(56, 189, 248, 0.12)',
  gaugeTrack: 'rgba(56, 189, 248, 0.12)', initialBar: '#475569',
  mapColors: ['#0891b2', '#38bdf8', '#a78bfa', '#f472b6'],
};

const LIGHT_COLOR = {
  primary: '#0891b2', secondary: '#7c3aed', accent: '#db2777',
  warning: '#d97706', success: '#059669', danger: '#dc2626',
  male: '#0891b2', female: '#7c3aed',
  text: '#1e293b', textDim: '#64748b',
  bg: 'rgba(255, 255, 255, 0.98)', mapArea: '#f1f5f9',
  mapBorder: 'rgba(8, 145, 178, 0.35)', mapLabel: '#1e293b',
  splitLine: 'rgba(8, 145, 178, 0.1)',
  gaugeTrack: 'rgba(8, 145, 178, 0.08)', initialBar: '#cbd5e1',
  mapColors: ['#67e8f9', '#0891b2', '#7c3aed', '#db2777'],
};

const getColors = () => (isLight.value ? LIGHT_COLOR : DARK_COLOR);

const makeGrad = (c1, c2, horizontal = true) =>
  new echarts.graphic.LinearGradient(0, 0, horizontal ? 1 : 0, horizontal ? 0 : 1, [
    { offset: 0, color: c1 }, { offset: 1, color: c2 },
  ]);

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

const PROVINCE_CODES = { '山东省': '370000' };

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

const PROVINCE_NAMES = Object.fromEntries(Object.entries(PROVINCE_CODES).map(([n, c]) => [c, n]));
const CITY_NAMES = {};
Object.entries(PROVINCE_CITY_MAP).forEach(([_, cities]) => {
  cities.forEach(c => { CITY_NAMES[c.code] = c.name; });
});
const DISTRICT_NAMES = {};
Object.entries(CITY_DISTRICT_MAP).forEach(([_, districts]) => {
  if (Array.isArray(districts)) districts.forEach(d => { DISTRICT_NAMES[d.code] = d.name; });
});

const routeCode = computed(() => String(route.params.code || '370102'));
const currentCountyConfig = computed(() => {
  if (COUNTY_MAP_CONFIG[routeCode.value]) return COUNTY_MAP_CONFIG[routeCode.value];
  const code = routeCode.value;
  return { name: code, short: code, center: [117.12, 36.65], zoom: 2.0 };
});
const currentCounty = computed(() => currentCountyConfig.value.short);
const currentCountyCenter = computed(() => currentCountyConfig.value.center);

const schoolList = computed(() => {
  const districtCode = currentFilter.district || routeCode.value;
  const list = DISTRICT_SCHOOLS_MAP[districtCode] || DISTRICT_SCHOOLS_MAP[routeCode.value] || [];
  if (currentFilter.school) {
    const schoolName = Object.keys(SCHOOL_CODES_MAP).find(k => SCHOOL_CODES_MAP[k] === currentFilter.school);
    if (schoolName) return [schoolName];
  }
  return list;
});

const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️' },
  { key: 'oral', label: '口腔健康', icon: '🦷' },
  { key: 'mental', label: '心理健康', icon: '🧠' },
  { key: 'weight', label: '健康体重', icon: '⚖️' },
  { key: 'bone', label: '骨骼健康', icon: '🦴' }
];

const metricConfig = {
  vision: { name: '近视', baseRate: 55, trend: 2.0, maleFactor: 0.95, femaleFactor: 1.05, primaryFactor: 0.6, juniorFactor: 1.0, seniorFactor: 1.25 },
  oral: { name: '龋齿', baseRate: 40, trend: -1.0, maleFactor: 0.96, femaleFactor: 1.05, primaryFactor: 1.24, juniorFactor: 0.9, seniorFactor: 0.67 },
  mental: { name: '心理预警', baseRate: 16, trend: -0.6, maleFactor: 0.92, femaleFactor: 1.1, primaryFactor: 0.67, juniorFactor: 1.22, seniorFactor: 1.56 },
  weight: { name: '超重/肥胖', baseRate: 22, trend: 1.2, maleFactor: 1.12, femaleFactor: 0.88, primaryFactor: 0.63, juniorFactor: 1.17, seniorFactor: 1.33 },
  bone: { name: '骨密度偏低', baseRate: 14, trend: -0.4, maleFactor: 0.91, femaleFactor: 1.11, primaryFactor: 0.75, juniorFactor: 1.13, seniorFactor: 1.38 }
};

const activeTab = ref('vision');
const isLight = ref(false);
const currentDate = ref('');
const currentTime = ref('');
const mapLoaded = ref(false);
const mapZoom = ref(2.0);

const currentMetric = computed(() => metricConfig[activeTab.value]);

const currentFilter = reactive({ province: '', city: '', district: '', school: '' });

const openDropdown = ref(null);

const provinceList = computed(() => {
  return Object.entries(PROVINCE_CODES).map(([name, code]) => ({
    code, name,
  }));
});

const cityOptions = computed(() => PROVINCE_CITY_MAP[currentFilter.province] || []);

const districtOptions = computed(() => CITY_DISTRICT_MAP[currentFilter.city] || []);

const schoolOptions = computed(() => {
  const districtCode = currentFilter.district || routeCode.value;
  const list = DISTRICT_SCHOOLS_MAP[districtCode] || [];
  return list.map(name => ({ code: SCHOOL_CODES_MAP[name] || name, name }));
});

const getSelectedName = (list, value) => {
  if (!value) return '';
  const item = list.find(i => i.value === value || i.code === value);
  return item ? item.name : '';
};

const toggleDropdown = (key) => {
  openDropdown.value = openDropdown.value === key ? null : key;
};

const selectOption = (key, value) => {
  if (key === 'province') {
    currentFilter.province = value;
    currentFilter.city = '';
    currentFilter.district = '';
    currentFilter.school = '';
  } else if (key === 'city') {
    currentFilter.city = value;
    currentFilter.district = '';
    currentFilter.school = '';
  } else if (key === 'district') {
    currentFilter.district = value;
    currentFilter.school = '';
  } else if (key === 'school') {
    currentFilter.school = value;
    const code = String(value);
    if (code.length >= 6) {
      const provCode = code.substring(0, 2) + '0000';
      const cityCode = code.substring(0, 4) + '00';
      const distCode = code.substring(0, 6);
      if (PROVINCE_CITY_MAP[provCode]) {
        currentFilter.province = provCode;
        if (PROVINCE_CITY_MAP[provCode].some(c => c.code === cityCode)) {
          currentFilter.city = cityCode;
          if (CITY_DISTRICT_MAP[cityCode] && CITY_DISTRICT_MAP[cityCode].some(d => d.code === distCode)) {
            currentFilter.district = distCode;
          }
        }
      }
    }
  }
  openDropdown.value = null;
  renderAllCharts();
};

const clearSchool = () => { currentFilter.school = ''; };
const clearProvince = () => {
  currentFilter.province = '';
  currentFilter.city = '';
  currentFilter.district = '';
  currentFilter.school = '';
};
const clearCity = () => {
  currentFilter.city = '';
  currentFilter.district = '';
  currentFilter.school = '';
};
const clearDistrict = () => {
  currentFilter.district = '';
  currentFilter.school = '';
};

const handleDropdownClickOutside = (e) => {
  if (!e.target.closest('.filter-select')) {
    openDropdown.value = null;
  }
};

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

const rankingChartRef = ref(null);
const ageGenderChartRef = ref(null);
const urbanGaugeRef = ref(null);
const ruralGaugeRef = ref(null);
const trendChartRef = ref(null);
const interventionChartRef = ref(null);
const mapRef = ref(null);

const chartInstances = {};
let timer = null;
let themeObserver = null;

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

const updateDateTime = () => {
  const now = new Date();
  currentDate.value = now.toLocaleDateString('zh-CN', { year: 'numeric', month: 'long', day: 'numeric', weekday: 'short' });
  currentTime.value = now.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
};

const switchTab = (tabKey) => {
  activeTab.value = tabKey;
  setTimeout(() => renderAllCharts(), 100);
};

const handleSearch = () => { openDropdown.value = null; renderAllCharts(); };
const handleReset = () => {
  currentFilter.province = '';
  currentFilter.city = '';
  currentFilter.district = '';
  currentFilter.school = '';
  openDropdown.value = null;
};

const goBack = () => {
  const code = routeCode.value;
  const cityCode = code.slice(0, 4) + '00';
  router.push({ path: `/vision/city/${cityCode}`, query: { tab: activeTab.value } });
};

const drillToSchool = (name) => {
  const code = SCHOOL_CODES_MAP[name];
  if (code) router.push({ path: `/vision/school/${code}`, query: { tab: activeTab.value } });
  else console.warn('未找到学校编码:', name);
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
  const minV = Math.min(...values);
  const maxV = Math.max(...values);
  const cfg = currentCountyConfig.value;

  const scatterData = data.map(d => ({
    name: d.name,
    value: [...(SCHOOL_COORDS_MAP[d.name] || cfg.center), d.value],
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
            <div style="color:${c.accent};font-size:11px;margin-top:4px">点击查看详情 →</div>`;
        }
        if (p.seriesType === 'lines') {
          return `<div style="font-weight:600">${p.name}</div>
            <div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>
            <div style="color:${c.textDim};font-size:11px;margin-top:4px">点击查看详情 →</div>`;
        }
        return `<div style="font-weight:600">${p.name}</div>`;
      },
    },
    visualMap: {
      show: true,
      orient: 'horizontal',
      right: 16,
      bottom: 10,
      itemWidth: 10,
      itemHeight: 80,
      textStyle: { color: c.textDim, fontSize: 10 },
      min: minV, max: maxV,
      inRange: { color: c.mapColors },
      text: ['高', '低'],
      calculable: false,
    },
    geo: {
      map: 'county',
      roam: true,
      selectedMode: false,
      layoutCenter: ['50%', '50%'],
      layoutSize: '95%',
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
          areaColor: isLight.value ? 'rgba(241, 245, 249, 0.85)' : 'rgba(56,189,248,0.55)',
          borderColor: isLight.value ? 'rgba(8,145,178,0.3)' : c.primary,
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
        symbolSize: (val) => Math.max(10, Math.min(18, (val[2] || 0) / maxV * 18 + 6)),
        showEffectOn: 'render',
        rippleEffect: {
          brushType: 'stroke',
          scale: 4,
          period: 3,
        },
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
          color: c.primary,
          shadowBlur: 15,
          shadowColor: c.primary,
          borderColor: isLight.value ? '#fff' : c.bg,
          borderWidth: 2,
        },
        emphasis: {
          scale: 1.4,
          itemStyle: { color: c.accent, shadowBlur: 25, shadowColor: c.accent, borderWidth: 3 },
          label: { fontSize: 12, fontWeight: 700, color: c.accent },
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
        return `<div style="font-weight:600">${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>`;
      },
    },
    grid: { left: 10, right: 55, top: 10, bottom: 10, containLabel: true },
    xAxis: { type: 'value', show: false, max: maxRate + 5 },
    yAxis: {
      type: 'category', data: schools, inverse: true,
      axisLine: { show: false }, axisTick: { show: false },
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
          shadowBlur: isLight.value ? 4 : 8, shadowColor: `${c.primary}44`,
        },
      })),
      barWidth: 10,
      label: { show: true, position: 'right', color: c.primary, fontSize: 10, fontWeight: 'bold', formatter: '{c}%' },
    }],
  };
};

const getAgeGenderOption = () => {
  const c = getColors();
  const grades = ['小学低年级', '小学中年级', '小学高年级', '初中', '高中'];
  const d = {
    grades,
    male: grades.map((_, i) => Math.round(22 + i * 7 + (Math.random() * 4 - 2))),
    female: grades.map((_, i) => Math.round(20 + i * 8 + (Math.random() * 4 - 2)))
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
    legend: { data: ['男生', '女生'], textStyle: { color: c.text, fontSize: 11 }, right: 10, top: 2, itemWidth: 12, itemHeight: 12 },
    grid: { left: 40, right: 25, top: 30, bottom: 20, containLabel: true },
    xAxis: {
      type: 'value', min: -100, max: 100, splitNumber: 5,
      axisLine: { show: false }, axisTick: { show: false },
      splitLine: { lineStyle: { color: c.splitLine } },
      axisLabel: { color: c.textDim, fontSize: 10, formatter: (v) => Math.abs(v) + '' },
    },
    yAxis: {
      type: 'category', data: d.grades,
      axisLine: { show: false }, axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold' },
    },
    series: [
      {
        name: '男生', type: 'bar', stack: 'total', data: d.male.map((v) => -v), barWidth: 14,
        itemStyle: { color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'), borderRadius: [4, 0, 0, 4] },
        label: { show: true, position: 'left', color: c.text, fontSize: 10, fontWeight: 'bold', formatter: (p) => Math.abs(p.value) + '%' },
      },
      {
        name: '女生', type: 'bar', stack: 'total', data: d.female, barWidth: 14,
        itemStyle: { color: makeGrad(c.secondary, isLight.value ? '#c4b5fd' : '#7c3aed'), borderRadius: [0, 4, 4, 0] },
        label: { show: true, position: 'right', color: c.text, fontSize: 10, fontWeight: 'bold', formatter: '{c}%' },
      },
    ],
  };
};

const getGaugeOption = (value, name, color) => {
  const c = getColors();
  return {
    backgroundColor: 'transparent',
    series: [{
      type: 'gauge', startAngle: 200, endAngle: -20, min: 0, max: 100,
      radius: '92%', center: ['50%', '58%'],
      progress: {
        show: true, width: 10, roundCap: true,
        itemStyle: {
          color: makeGrad(color, color === c.primary ? c.secondary : c.accent),
          shadowBlur: isLight.value ? 6 : 12, shadowColor: `${color}88`,
        },
      },
      axisLine: { lineStyle: { width: 10, color: [[1, c.gaugeTrack]] } },
      pointer: { show: false }, axisTick: { show: false }, splitLine: { show: false },
      axisLabel: { show: false }, anchor: { show: false },
      detail: {
        valueAnimation: true, formatter: '{value}%', color, fontSize: 22, fontWeight: 'bold',
        offsetCenter: [0, '15%'], textShadowBlur: isLight.value ? 0 : 8, textShadowColor: color,
      },
      title: { show: true, offsetCenter: [0, '70%'], color: c.textDim, fontSize: 12 },
      data: [{ value, name }],
    }],
  };
};

const getTrendOption = () => {
  const c = getColors();
  const months = ['1月', '2月', '3月', '4月', '5月', '6月'];
  const baseRate = currentCountyData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rateData = months.map((_, i) => Math.round((baseRate + i * 0.7 + ((seed + i * 3) % 4 - 2)) * 10) / 10);
  const targetData = months.map((_, i) => Math.round((baseRate + 2 - i * 0.3) * 10) / 10);

  return {
    backgroundColor: 'transparent',
    tooltip: { ...getTooltip('axis') },
    legend: { show: false },
    grid: { left: 40, right: 20, top: 10, bottom: 30, containLabel: true },
    xAxis: {
      type: 'category', data: months,
      axisLine: { lineStyle: { color: c.splitLine } }, axisTick: { show: false },
      axisLabel: { color: c.textDim, fontSize: 11 },
    },
    yAxis: {
      type: 'value', axisLine: { show: false }, axisTick: { show: false },
      axisLabel: { color: c.textDim, fontSize: 11 },
      splitLine: { lineStyle: { color: c.splitLine } },
    },
    series: [
      {
        name: '实际' + currentMetric.value.name + '率', type: 'line', data: rateData,
        smooth: true, symbol: 'circle', symbolSize: 8,
        lineStyle: { width: 3, color: c.primary, shadowBlur: 8, shadowColor: `${c.primary}55` },
        itemStyle: { color: c.primary, borderColor: c.bg, borderWidth: 2 },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: `${c.primary}55` }, { offset: 1, color: `${c.primary}00` },
          ]),
        },
      },
      {
        name: '目标线', type: 'line', data: targetData,
        smooth: true, symbol: 'none',
        lineStyle: { width: 2, color: c.secondary, type: 'dashed' },
      },
    ],
  };
};

const getInterventionOption = () => {
  const c = getColors();
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const base = currentCountyData.value.rate;
  const data = [
    { name: '示范区A', initial: Math.round((base + 6 + (seed % 5)) * 10) / 10, final: Math.round((base + 1 - (seed % 3)) * 10) / 10, change: -Math.round((6 + (seed % 3)) * 10) / 10 },
    { name: '示范区B', initial: Math.round((base + 10 + (seed % 4)) * 10) / 10, final: Math.round((base + 2 - (seed % 4)) * 10) / 10, change: -Math.round((8 + (seed % 2)) * 10) / 10 },
    { name: '示范区C', initial: Math.round((base + 4 + (seed % 3)) * 10) / 10, final: Math.round((base + 1 - (seed % 2)) * 10) / 10, change: -Math.round((4 + (seed % 4)) * 10) / 10 },
    { name: '示范区D', initial: Math.round((base + 8 + (seed % 2)) * 10) / 10, final: Math.round((base + 2 - (seed % 3)) * 10) / 10, change: -Math.round((7 + (seed % 2)) * 10) / 10 },
  ];
  const names = data.map(d => d.name);
  const initials = data.map(d => d.initial);
  const finals = data.map(d => d.final);
  const changes = data.map(d => d.change);

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        let html = '';
        params.forEach((p) => { html += `<div>${p.marker}${p.seriesName}：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>`; });
        return html;
      },
    },
    legend: { show: false },
    grid: { left: 80, right: 60, top: 8, bottom: 8, containLabel: true },
    xAxis: { type: 'value', show: false, max: Math.ceil(Math.max(...initials) * 1.2) },
    yAxis: { type: 'category', data: names, axisLine: { show: false }, axisTick: { show: false }, axisLabel: { color: c.text, fontSize: 11 } },
    series: [
      {
        name: '期初', type: 'bar', data: initials, barWidth: 9, barGap: '30%',
        itemStyle: { color: c.initialBar, borderRadius: [0, 3, 3, 0] },
        label: { show: true, position: 'right', color: c.textDim, fontSize: 10, formatter: '{c}%', distance: 4 },
      },
      {
        name: '期末', type: 'bar', data: finals, barWidth: 9,
        itemStyle: {
          color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'),
          borderRadius: [0, 3, 3, 0],
          shadowBlur: isLight.value ? 4 : 8, shadowColor: `${c.primary}55`,
        },
        label: {
          show: true, position: 'right', color: c.primary, fontSize: 10, fontWeight: 'bold',
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

const renderAllCharts = () => {
  disposeAll();
  requestAnimationFrame(() => {
    nextTick(() => {
      const d = currentCountyData.value;
      const ur = { urban: Math.round(d.rate * 0.9 * 10) / 10, rural: Math.round(d.rate * 1.2 * 10) / 10 };
      initChart(rankingChartRef, 'ranking', getRankingOption(), (params) => {
        if (params?.name) drillToSchool(params.name);
      });
      initChart(ageGenderChartRef, 'ageGender', getAgeGenderOption());
      initChart(urbanGaugeRef, 'urbanGauge', getGaugeOption(ur.urban, `城区${currentMetric.value.name}率`, getColors().primary));
      initChart(ruralGaugeRef, 'ruralGauge', getGaugeOption(ur.rural, `县乡${currentMetric.value.name}率`, getColors().secondary));
      initChart(trendChartRef, 'trend', getTrendOption());
      initChart(interventionChartRef, 'intervention', getInterventionOption());
      if (mapLoaded.value) {
        initChart(mapRef, 'map', getMapOption(), (params) => {
          if (params?.name) drillToSchool(params.name);
        });
      }
    });
  });
};

const loadMap = async () => {
  const code = routeCode.value;
  const cfg = currentCountyConfig.value;
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
  document.addEventListener('click', handleDropdownClickOutside);
  setTimeout(() => resizeAll(), 500);
  setTimeout(() => resizeAll(), 1500);
});

onBeforeUnmount(() => {
  if (timer) clearInterval(timer);
  window.removeEventListener('resize', resizeAll);
  document.removeEventListener('click', handleDropdownClickOutside);
  if (themeObserver) { themeObserver.disconnect(); themeObserver = null; }
  disposeAll();
});

watch(activeTab, () => setTimeout(() => renderAllCharts(), 100));
watch(isLight, () => setTimeout(() => renderAllCharts(), 100));
watch(() => route.query.tab, (tab) => {
  if (tab && ['vision','oral','mental','weight','bone'].includes(tab)) {
    activeTab.value = tab;
    setTimeout(() => renderAllCharts(), 100);
  }
}, { immediate: true });
watch(() => route.params.code, () => {
  mapLoaded.value = false;
  setTimeout(() => loadMap(), 100);
});
</script>

<style scoped>
.app-root {
  position: relative;
  height: 100vh;
  padding: 0;
  background: var(--bg);
  color: var(--text);
  font-family: -apple-system, BlinkMacSystemFont, 'PingFang SC', 'Microsoft YaHei', sans-serif;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  --primary: #00d4ff;
  --primary-soft: rgba(0, 212, 255, 0.15);
  --primary-glow: rgba(0, 212, 255, 0.5);
  --secondary: #a855f7;
  --secondary-soft: rgba(168, 85, 247, 0.15);
  --secondary-glow: rgba(168, 85, 247, 0.45);
  --accent: #f472b6;
  --accent-glow: rgba(244, 114, 182, 0.45);
  --warning: #fbbf24;
  --success: #34d399;
  --danger: #fb7185;
  --glow-danger: rgba(251, 113, 133, 0.35);
  --bg: #030818;
  --bg-deep: #020612;
  --bg-card: rgba(8, 18, 45, 0.88);
  --bg-card-hover: rgba(12, 26, 58, 0.95);
  --bg-soft: rgba(0, 212, 255, 0.05);
  --border: rgba(0, 212, 255, 0.28);
  --border-hover: rgba(0, 212, 255, 0.55);
  --border-soft: rgba(0, 212, 255, 0.12);
  --border-purple: rgba(168, 85, 247, 0.35);
  --text: #e2e8f0;
  --text-dim: #94a3b8;
  --text-muted: #64748b;
  --text-dimmer: #7dd3fc;
  --glow: rgba(0, 212, 255, 0.45);
  --glow-soft: rgba(0, 212, 255, 0.15);
  --glow-strong: rgba(0, 212, 255, 0.7);
  --glow-purple: rgba(168, 85, 247, 0.45);
  --shadow-card: 0 4px 28px rgba(0, 0, 0, 0.45), 0 0 0 1px rgba(0, 212, 255, 0.08);
  --shadow-hover: 0 8px 36px rgba(0, 212, 255, 0.2);
  --shadow-purple: 0 8px 36px rgba(168, 85, 247, 0.18);
  --grad-main: linear-gradient(135deg, #00d4ff 0%, #a855f7 100%);
  --grad-cyan: linear-gradient(135deg, #00d4ff 0%, #0891b2 100%);
  --grad-purple: linear-gradient(135deg, #a855f7 0%, #7c3aed 100%);
  --grad-pink: linear-gradient(135deg, #f472b6 0%, #db2777 100%);
}
.app-root.theme-light {
  --primary: #0891b2;
  --primary-soft: rgba(8, 145, 178, 0.12);
  --primary-glow: rgba(8, 145, 178, 0.35);
  --secondary: #7c3aed;
  --secondary-soft: rgba(124, 58, 237, 0.12);
  --secondary-glow: rgba(124, 58, 237, 0.3);
  --accent: #db2777;
  --accent-glow: rgba(219, 39, 119, 0.3);
  --danger: #dc2626;
  --glow-danger: rgba(220, 38, 38, 0.25);
  --bg: #f8fafc;
  --bg-deep: #f1f5f9;
  --bg-card: rgba(255, 255, 255, 0.98);
  --bg-card-hover: rgba(255, 255, 255, 1);
  --bg-soft: rgba(8, 145, 178, 0.04);
  --border: rgba(8, 145, 178, 0.2);
  --border-hover: rgba(8, 145, 178, 0.38);
  --border-soft: rgba(8, 145, 178, 0.12);
  --border-purple: rgba(124, 58, 237, 0.22);
  --text: #1e293b;
  --text-dim: #64748b;
  --text-muted: #94a3b8;
  --text-dimmer: #475569;
  --glow: rgba(8, 145, 178, 0.15);
  --glow-soft: rgba(8, 145, 178, 0.08);
  --glow-strong: rgba(8, 145, 178, 0.25);
  --glow-purple: rgba(124, 58, 237, 0.15);
  --shadow-card: 0 4px 24px rgba(15, 23, 42, 0.08), 0 0 0 1px rgba(8, 145, 178, 0.06);
  --shadow-hover: 0 8px 32px rgba(8, 145, 178, 0.14);
  --shadow-purple: 0 8px 32px rgba(124, 58, 237, 0.12);
  --grad-main: linear-gradient(135deg, #0891b2 0%, #7c3aed 100%);
  --grad-cyan: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
  --grad-purple: linear-gradient(135deg, #7c3aed 0%, #6d28d9 100%);
  --grad-pink: linear-gradient(135deg, #db2777 0%, #be185d 100%);
}
.app-root *, .app-root *::before, .app-root *::after { box-sizing: border-box; }
.app-root {
  width: 100%;
  height: 100vh;
  position: relative;
  overflow: hidden;
  background: var(--bg);
  color: var(--text);
  font-family: 'Inter', 'PingFang SC', 'Microsoft YaHei', system-ui, sans-serif;
  display: flex;
  flex-direction: column;
}
.app-root.theme-light .scan-line,
.app-root.theme-light .scan-line-2,
.app-root.theme-light .bg-stars,
.app-root.theme-light .scan-particle { display: none; }
.app-root.theme-light .bg-glow { opacity: 0.08; filter: blur(100px); }
.app-root.theme-light .bg-grid { opacity: 0.4; }
.app-root.theme-light .tech-panel::before,
.app-root.theme-light .tech-panel::after { opacity: 0.5; box-shadow: none; }
.app-root.theme-light .tech-panel-corner { opacity: 0.6; }
.app-root.theme-light .title-text {
  background: linear-gradient(180deg, #1e293b 0%, #0891b2 55%, #7c3aed 100%);
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
  box-shadow: 0 4px 16px rgba(124, 58, 237, 0.28);
}
.app-root.theme-light .map-area {
  background: linear-gradient(180deg, rgba(255,255,255,0.6), rgba(238,241,247,0.85));
  box-shadow: var(--shadow-card);
}
.app-root.theme-light .map-deco-ready .map-grid { opacity: 0.3; }
.app-root.theme-light .tech-panel {
  box-shadow: 0 2px 16px rgba(15, 23, 42, 0.06), 0 0 0 1px rgba(8, 145, 178, 0.06);
}
.app-root.theme-light .kpi-card-lg,
.app-root.theme-light .kpi-card-sm {
  box-shadow: 0 2px 12px rgba(15, 23, 42, 0.04);
}
.app-root.theme-light .alert-item:hover {
  box-shadow: 0 4px 12px rgba(15, 23, 42, 0.06);
}

.bg-decor { position: absolute; inset: 0; pointer-events: none; z-index: 0; overflow: hidden; }
.bg-grid {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(0, 212, 255, 0.08) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.08) 1px, transparent 1px);
  background-size: 60px 60px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  -webkit-mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  animation: grid-drift 30s linear infinite;
}
@keyframes grid-drift { 0% { background-position: 0 0; } 100% { background-position: 60px 60px; } }
.bg-glow { position: absolute; border-radius: 50%; filter: blur(140px); animation: glow-float 20s ease-in-out infinite; }
.bg-glow-1 { width: 750px; height: 750px; background: #00d4ff; top: -200px; left: -150px; opacity: 0.25; }
.bg-glow-2 { width: 850px; height: 850px; background: #a855f7; bottom: -250px; right: -150px; opacity: 0.2; animation-delay: -7s; }
.bg-glow-3 { width: 600px; height: 600px; background: var(--accent); top: 35%; left: 45%; opacity: 0.1; animation-delay: -14s; }
@keyframes glow-float { 0%, 100% { transform: translate(0, 0) scale(1); } 33% { transform: translate(80px, -50px) scale(1.08); } 66% { transform: translate(-50px, 60px) scale(0.92); } }
.bg-stars { position: absolute; inset: 0; }
.bg-stars::before, .bg-stars::after {
  content: ''; position: absolute; inset: 0;
  background-image:
    radial-gradient(1px 1px at 20% 30%, var(--primary) 60%, transparent 100%),
    radial-gradient(1px 1px at 60% 70%, var(--secondary) 60%, transparent 100%),
    radial-gradient(1px 1px at 80% 10%, var(--text) 60%, transparent 100%),
    radial-gradient(1.5px 1.5px at 40% 80%, var(--primary) 60%, transparent 100%),
    radial-gradient(1px 1px at 90% 50%, var(--secondary) 60%, transparent 100%),
    radial-gradient(1px 1px at 15% 85%, var(--accent) 50%, transparent 100%),
    radial-gradient(1px 1px at 75% 25%, var(--primary) 50%, transparent 100%);
  background-size: 350px 350px;
  animation: star-twinkle 4s ease-in-out infinite alternate;
}
.bg-stars::after { animation-delay: 2s; opacity: 0.6; }
@keyframes star-twinkle { 0% { opacity: 0.3; } 100% { opacity: 0.85; } }

.scan-particle {
  position: absolute;
  width: 2px; height: 2px;
  background: var(--primary);
  border-radius: 50%;
  box-shadow: 0 0 6px var(--primary);
  pointer-events: none;
  animation: particle-float 12s linear infinite;
}
.scan-particle:nth-child(1) { top: 10%; left: 15%; animation-delay: 0s; }
.scan-particle:nth-child(2) { top: 30%; left: 80%; animation-delay: -2s; }
.scan-particle:nth-child(3) { top: 60%; left: 25%; animation-delay: -4s; }
.scan-particle:nth-child(4) { top: 80%; left: 70%; animation-delay: -6s; }
.scan-particle:nth-child(5) { top: 45%; left: 50%; animation-delay: -8s; }
@keyframes particle-float {
  0% { transform: translateY(0) translateX(0); opacity: 0; }
  10% { opacity: 0.8; }
  50% { transform: translateY(-30px) translateX(15px); opacity: 0.6; }
  90% { opacity: 0.8; }
  100% { transform: translateY(-60px) translateX(-10px); opacity: 0; }
}

.scan-line {
  position: absolute; left: 0; right: 0; top: 0; height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  box-shadow: 0 0 20px var(--primary), 0 0 40px var(--primary), 0 0 60px var(--glow-soft);
  animation: scan-move 7s linear infinite;
  z-index: 1; pointer-events: none;
}
.scan-line-2 {
  position: absolute; left: 0; right: 0; top: 0; height: 120px;
  background: linear-gradient(180deg, transparent, rgba(0, 212, 255, 0.1), transparent);
  animation: scan-move 7s linear infinite;
  animation-delay: 1.5s;
  z-index: 1; pointer-events: none;
}
@keyframes scan-move {
  0% { transform: translateY(-100px); }
  100% { transform: translateY(100vh); }
}

.tech-topbar {
  position: relative; z-index: 5;
  display: flex; align-items: center; justify-content: space-between;
  padding: 2px 24px;
  background: linear-gradient(90deg, transparent, rgba(0,212,255,0.04), transparent);
  border-bottom: 1px solid var(--border-soft);
}
.tech-corner {
  width: 24px; height: 24px;
  border: 1.5px solid var(--primary);
  position: relative; box-shadow: 0 0 8px var(--glow);
}
.tech-corner-l { border-right: none; border-bottom: none; }
.tech-corner-r { border-left: none; border-bottom: none; }
.tc-dot {
  position: absolute; width: 5px; height: 5px;
  background: var(--primary); border-radius: 50%;
  box-shadow: 0 0 8px var(--primary);
  animation: dot-blink 1.5s ease-in-out infinite;
}
.tech-corner-l .tc-dot { right: -2.5px; bottom: -2.5px; }
.tech-corner-r .tc-dot { left: -2.5px; bottom: -2.5px; }
@keyframes dot-blink { 0%, 100% { opacity: 1; transform: scale(1); } 50% { opacity: 0.4; transform: scale(0.7); } }

.tech-line { flex: 1; height: 1px; background: linear-gradient(90deg, transparent, var(--primary), transparent); margin: 0 8px; position: relative; }
.line-pulse {
  position: absolute; top: 50%; width: 60px; height: 2px;
  background: var(--primary); transform: translateY(-50%);
  box-shadow: 0 0 10px var(--primary);
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
.tech-dots span { width: 4px; height: 4px; background: var(--primary); border-radius: 50%; box-shadow: 0 0 4px var(--primary); animation: dot-blink 1.2s ease-in-out infinite; }
.tech-dots span:nth-child(2) { animation-delay: 0.15s; }
.tech-dots span:nth-child(3) { animation-delay: 0.3s; }
.tech-dots span:nth-child(4) { animation-delay: 0.45s; }
.tech-dots span:nth-child(5) { animation-delay: 0.6s; }
.tech-dots span:nth-child(6) { animation-delay: 0.75s; }
.tech-dots span:nth-child(7) { animation-delay: 0.9s; }

.national-header {
  position: relative; z-index: 5;
  display: flex; align-items: center; justify-content: space-between;
  padding: 0 32px; flex-shrink: 0;
}
.header-deco { display: flex; align-items: center; gap: 8px; flex: 1; }
.header-deco-l { justify-content: flex-end; padding-right: 24px; }
.header-deco-r { padding-left: 24px; }
.hd-line { width: 100px; height: 1px; background: linear-gradient(90deg, transparent, var(--primary)); box-shadow: 0 0 6px var(--glow-soft); }
.hd-dot { width: 8px; height: 8px; background: var(--primary); border-radius: 50%; box-shadow: 0 0 12px var(--primary); animation: hd-dot-pulse 2s ease-in-out infinite; }
@keyframes hd-dot-pulse { 0%, 100% { opacity: 1; transform: scale(1); } 50% { opacity: 0.6; transform: scale(0.85); } }

.national-title {
  position: relative; display: inline-flex; align-items: center;
  font-size: 20px; font-weight: 700; letter-spacing: 4px;
  margin: 0; white-space: nowrap;
}
.title-bracket { 
  color: var(--secondary); 
  font-weight: 700; 
  text-shadow: 0 0 16px var(--secondary-glow), 0 0 30px var(--glow-purple); 
  font-size: 26px;
  animation: bracket-glow 3s ease-in-out infinite;
}
@keyframes bracket-glow {
  0%, 100% { filter: brightness(1); }
  50% { filter: brightness(1.3); }
}
.title-text {
  background: linear-gradient(90deg, #ffffff 0%, #00d4ff 25%, #a855f7 60%, #ffffff 100%);
  background-size: 200% 100%;
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
  background-clip: text;
  filter: drop-shadow(0 0 16px var(--glow)) drop-shadow(0 0 30px var(--glow-soft));
  margin: 0 14px;
  animation: title-gradient-shift 5s ease-in-out infinite;
}
@keyframes title-gradient-shift {
  0%, 100% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
}
.title-shine {
  position: absolute; top: 0; left: -50%; width: 30%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.5), transparent);
  animation: shine-move 3.5s ease-in-out infinite;
  pointer-events: none;
  filter: blur(1px);
}
@keyframes shine-move {
  0%, 100% { left: -50%; }
  50% { left: 120%; }
}

.national-time { display: flex; flex-direction: column; align-items: flex-end; gap: 2px; }
.time-date { font-size: 12px; color: var(--text-dimmer); letter-spacing: 1px; text-shadow: 0 0 4px var(--glow-soft); }
.time-clock { display: flex; align-items: center; gap: 1px; }
.tc-digit {
  font-size: 20px; font-weight: 700; color: var(--primary);
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  text-shadow: 0 0 14px var(--glow), 0 0 4px var(--primary), 0 0 30px var(--glow-soft);
  letter-spacing: 2px;
  animation: clock-glow-pulse 2s ease-in-out infinite;
}
@keyframes clock-glow-pulse {
  0%, 100% { text-shadow: 0 0 14px var(--glow), 0 0 4px var(--primary); }
  50% { text-shadow: 0 0 20px var(--glow), 0 0 8px var(--primary), 0 0 40px var(--glow-soft); }
}

.national-tabs {
  position: relative; z-index: 5;
  display: flex; align-items: center; justify-content: center;
  gap: 3px; padding: 0 20px; flex-shrink: 0;
}
.tech-tab {
  position: relative;
  display: flex; align-items: center; gap: 4px;
  padding: 3px 12px;
  background: var(--bg-soft);
  border: 1px solid var(--border-soft);
  border-radius: 16px;
  color: var(--text-dim);
  font-size: 12px; font-weight: 500;
  font-family: inherit; cursor: pointer;
  transition: all 0.3s; overflow: hidden;
}
.tech-tab:hover { background: var(--primary-soft); color: var(--primary); border-color: var(--border); }
.tech-tab-icon { font-size: 13px; }
.tech-tab.active {
  background: var(--grad-main);
  border-color: transparent; color: #fff;
  box-shadow: 0 0 16px var(--glow), 0 0 24px var(--glow-purple);
  text-shadow: 0 0 6px rgba(255,255,255,0.5);
}
.tech-tab.active::before {
  content: ''; position: absolute; top: 0; left: 0; right: 0; height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), var(--primary), transparent);
  background-size: 200% 100%;
  animation: tab-border-move 3s linear infinite;
}
.tech-tab.active::after {
  content: ''; position: absolute; bottom: 0; left: 0; right: 0; height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), var(--primary), transparent);
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

.national-body {
  position: relative; z-index: 3;
  flex: 1;
  display: grid;
  grid-template-columns: 24% 1fr 24%;
  gap: 4px;
  padding: 0 8px 2px;
  min-height: 0;
  height: 0;
}
.left-col, .right-col, .map-col { display: flex; flex-direction: column; gap: 3px; min-height: 0; height: 100%; justify-content: flex-start; }
.map-col { gap: 4px; }
.map-col .map-area { flex: 2; min-height: 0; }
.map-col .map-legend { flex-shrink: 0; }
.left-col .tech-panel:nth-child(1) { flex: 0 1 auto; }
.left-col .tech-panel:nth-child(2) { flex: 0 1 auto; }
.left-col .tech-panel:nth-child(3) { flex: 0 1 auto; }
.right-col .tech-panel:nth-child(1) { flex: 0 1 auto; }
.right-col .tech-panel:nth-child(2) { flex: 0 1 auto; }
.right-col .tech-panel:nth-child(3) { flex: 0 1 auto; }
.right-col .tech-panel:nth-child(4) { flex: 0 1 auto; }

.tech-panel {
  position: relative;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 3px 6px;
  display: flex; flex-direction: column;
  box-shadow: 0 0 20px rgba(0, 212, 255, 0.12), inset 0 0 20px rgba(0, 212, 255, 0.03);
  flex: 0 1 auto;
  min-height: 0;
  overflow: visible;
  transition: transform 0.3s, box-shadow 0.3s;
}
.tech-panel:hover {
  transform: translateY(-1px);
  box-shadow: 0 0 40px rgba(0, 212, 255, 0.22), inset 0 0 30px rgba(0, 212, 255, 0.06);
}
.tech-panel::before {
  content: ''; position: absolute; top: -1px; left: -1px; right: -1px; height: 2px;
  background: linear-gradient(90deg, var(--primary) 0%, transparent 30%, transparent 70%, var(--secondary) 100%);
  background-size: 300% 100%;
  border-radius: 6px 6px 0 0;
  animation: border-shine 4s linear infinite, border-pulse 3s ease-in-out infinite;
  box-shadow: 0 0 10px var(--glow-soft);
}
@keyframes border-shine { 0% { background-position: -100% 0; } 100% { background-position: 200% 0; } }
@keyframes border-pulse { 0%, 100% { opacity: 0.85; } 50% { opacity: 1; } }
.tech-panel::after {
  content: ''; position: absolute; top: 0; left: 0; width: 16px; height: 16px;
  border-top: 2px solid var(--primary); border-left: 2px solid var(--primary);
  border-radius: 6px 0 0 0;
  box-shadow: 0 0 12px var(--glow), 0 0 20px var(--glow-soft);
}
.tech-panel-corner {
  position: absolute;
  width: 16px; height: 16px;
  pointer-events: none;
}
.tech-panel-corner::before,
.tech-panel-corner::after {
  content: ''; position: absolute;
  background: var(--primary);
  box-shadow: 0 0 10px var(--glow);
}
.tech-panel-corner.tl { top: 0; left: 0; }
.tech-panel-corner.tl::before { top: 0; left: 0; width: 16px; height: 2px; }
.tech-panel-corner.tl::after { top: 0; left: 0; width: 2px; height: 16px; }
.tech-panel-corner.tr { top: 0; right: 0; }
.tech-panel-corner.tr::before { top: 0; right: 0; width: 16px; height: 2px; }
.tech-panel-corner.tr::after { top: 0; right: 0; width: 2px; height: 16px; }
.tech-panel-corner.bl { bottom: 0; left: 0; }
.tech-panel-corner.bl::before { bottom: 0; left: 0; width: 16px; height: 2px; }
.tech-panel-corner.bl::after { bottom: 0; left: 0; width: 2px; height: 16px; }
.tech-panel-corner.br { bottom: 0; right: 0; }
.tech-panel-corner.br::before { bottom: 0; right: 0; width: 16px; height: 2px; }
.tech-panel-corner.br::after { bottom: 0; right: 0; width: 2px; height: 16px; }

.tech-panel-header {
  display: flex; align-items: center; gap: 6px;
  margin-bottom: 1px; position: relative;
  padding-bottom: 1px;
}
.tech-panel-header::after {
  content: '';
  position: absolute;
  bottom: 0; left: 0;
  width: 100%; height: 1px;
  background: linear-gradient(90deg, var(--primary) 0%, var(--secondary) 50%, transparent 100%);
  background-size: 200% 100%;
  animation: header-line-flow 3s linear infinite;
  box-shadow: 0 0 4px var(--glow-soft);
}
@keyframes header-line-flow {
  0% { background-position: -100% 0; }
  100% { background-position: 100% 0; }
}
.panel-bullet {
  width: 8px; height: 8px;
  background: var(--grad-main);
  border-radius: 2px;
  box-shadow: 0 0 8px var(--glow), 0 0 14px var(--glow-soft);
  animation: bullet-pulse 2s ease-in-out infinite;
}
@keyframes bullet-pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}
.panel-title {
  font-size: 12px; font-weight: 600;
  color: var(--text);
  letter-spacing: 1px;
  text-shadow: 0 0 6px var(--glow-soft);
}
.panel-tag {
  margin-left: auto;
  font-size: 9px;
  padding: 1px 6px;
  background: var(--primary-soft);
  border: 1px solid var(--border-soft);
  color: var(--primary);
  border-radius: 3px;
  letter-spacing: 0.5px;
  box-shadow: 0 0 8px var(--glow-soft);
}
.tech-panel-body {
  flex: 1; min-height: 0; position: relative;
}

.top-filter-bar {
  position: relative; z-index: 6;
  display: flex; align-items: center; justify-content: center;
  gap: 6px; padding: 0 16px 2px; flex-shrink: 0;
}
.filter-select {
  position: relative; display: inline-flex; align-items: stretch;
  flex: 0 0 auto;
  width: 100px;
  min-width: 100px;
  max-width: 100px;
}
.filter-select::before {
  content: ''; position: absolute; inset: 0;
  border-radius: 4px; padding: 1px;
  background: linear-gradient(135deg, rgba(0,212,255,0.6), rgba(168,85,247,0.6));
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
  font-size: 12px;
  padding: 3px 24px 3px 8px;
  font-family: inherit;
  cursor: pointer;
  outline: none;
  transition: all 0.3s;
  box-shadow: inset 0 0 12px rgba(0, 212, 255, 0.08);
  width: 100%;
  display: flex;
  align-items: center;
  gap: 4px;
  user-select: none;
  overflow: hidden;
}
.select-trigger:hover,
.select-trigger:focus {
  border-color: var(--border-hover);
  color: var(--text);
  box-shadow: 0 0 14px var(--glow-soft), inset 0 0 12px rgba(0, 212, 255, 0.12);
}
.filter-select.has-value .select-trigger { color: var(--text); }
.select-trigger.placeholder { color: var(--text-muted); }

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

.filter-select.disabled {
  opacity: 0.45;
  pointer-events: none;
}
.filter-select.disabled .select-trigger {
  opacity: 0.5;
  cursor: not-allowed;
  background: var(--bg-soft);
}
.filter-select.disabled .select-trigger:hover {
  border-color: var(--border);
  box-shadow: inset 0 0 12px rgba(0, 212, 255, 0.05);
  color: var(--text-muted);
}
.filter-select.disabled::before { opacity: 0 !important; }

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
  display: inline-flex; align-items: center; gap: 4px;
  padding: 3px 10px;
  border: 1px solid var(--border);
  border-radius: 4px;
  background: var(--bg-soft);
  color: var(--text-dim);
  font-size: 12px;
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
.kpi-grid {
  display: flex;
  flex-direction: column;
  gap: 4px;
  flex: 1;
  min-height: 0;
  overflow: visible;
}
.kpi-row-large {
  display: flex; gap: 4px; flex: 0 0 auto;
}
.kpi-row-large > .kpi-card-lg { flex: 1; }
.kpi-card-lg {
  position: relative;
  background: linear-gradient(135deg, var(--bg-soft) 0%, rgba(0, 212, 255, 0.03) 100%);
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 4px 6px;
  overflow: hidden;
  transition: all 0.3s;
  box-shadow: 0 0 12px rgba(0, 212, 255, 0.08);
}
.kpi-card-lg:hover { 
  border-color: var(--border-hover); 
  box-shadow: 0 4px 20px rgba(0, 212, 255, 0.2); 
  transform: translateY(-2px); 
}
.kpi-card-deco {
  position: absolute; top: 0; right: 0;
  width: 0; height: 0;
  border-style: solid;
}
.kpi-row-large > .kpi-card-lg:nth-child(1) .kpi-card-deco { border-width: 0 30px 30px 0; border-color: transparent var(--primary) transparent transparent; filter: drop-shadow(0 0 4px var(--glow)); }
.kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-card-deco { border-width: 0 30px 30px 0; border-color: transparent var(--secondary) transparent transparent; filter: drop-shadow(0 0 4px var(--glow-purple)); }
.kpi-card-hex {
  position: absolute; top: 6px; right: 8px;
  display: flex; gap: 2px;
}
.kpi-card-hex span {
  width: 5px; height: 5px;
  background: var(--primary);
  opacity: 0.7;
  clip-path: polygon(50% 0%, 100% 25%, 100% 75%, 50% 100%, 0% 75%, 0% 25%);
  animation: hex-pulse 2s ease-in-out infinite;
}
.kpi-card-hex span:nth-child(2) { animation-delay: -0.4s; }
.kpi-card-hex span:nth-child(3) { animation-delay: -0.8s; }
@keyframes hex-pulse {
  0%, 100% { opacity: 0.4; }
  50% { opacity: 0.9; }
}
.kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-card-hex span { background: var(--secondary); }
.kpi-lg-label {
  font-size: 10px; color: var(--text-dim);
  margin-bottom: 2px; letter-spacing: 0.5px;
}
.kpi-lg-value {
  font-size: 18px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  letter-spacing: 2px;
  margin-bottom: 2px;
  animation: value-glow-pulse 3s ease-in-out infinite;
}
@keyframes value-glow-pulse {
  0%, 100% { text-shadow: 0 0 10px var(--glow); }
  50% { text-shadow: 0 0 20px var(--glow), 0 0 30px var(--glow-soft); }
}
.kpi-lg-value .num { color: var(--primary); text-shadow: 0 0 14px var(--glow), 0 0 30px var(--glow-soft); }
.kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-lg-value .num { color: var(--secondary); text-shadow: 0 0 14px var(--glow-purple), 0 0 30px rgba(168, 85, 247, 0.2); }
.kpi-lg-value .unit { font-size: 13px; color: var(--text-dim); margin-left: 4px; font-weight: 400; }
.kpi-lg-trend {
  display: flex; align-items: center; gap: 3px;
  font-size: 11px;
}
.kpi-lg-trend.up { color: var(--success); text-shadow: 0 0 6px rgba(52, 211, 153, 0.3); }
.kpi-lg-trend.down { color: var(--text-dim); }
.kpi-lg-trend .trend-arrow { font-weight: 700; }

.kpi-row-gender {
  display: flex; gap: 4px;
  flex: 0 0 auto;
}
.gender-item {
  flex: 1;
  display: flex; align-items: center; gap: 6px;
  padding: 3px 6px;
  background: linear-gradient(135deg, var(--bg-soft) 0%, rgba(0, 212, 255, 0.02) 100%);
  border: 1px solid var(--border);
  border-radius: 5px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s;
}
.gender-item:hover { transform: translateX(2px); box-shadow: 0 4px 12px rgba(0, 212, 255, 0.12); }
.gender-item.male { border-left: 2px solid var(--primary); box-shadow: -2px 0 8px var(--glow-soft); }
.gender-item.female { border-left: 2px solid var(--secondary); box-shadow: -2px 0 8px var(--glow-purple); }
.gender-icon {
  width: 22px; height: 22px;
  border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  font-size: 13px; font-weight: 700;
  flex-shrink: 0;
}
.gender-item.male .gender-icon {
  background: var(--primary-soft);
  color: var(--primary);
  box-shadow: 0 0 8px var(--glow-soft);
}
.gender-item.female .gender-icon {
  background: var(--secondary-soft);
  color: var(--secondary);
  box-shadow: 0 0 8px var(--glow-purple);
}
.gender-info { flex: 1; min-width: 0; }
.gender-label {
  display: block;
  font-size: 10px; color: var(--text-dim);
  margin-bottom: 2px;
  letter-spacing: 0.5px;
}
.gender-value {
  font-size: 16px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  letter-spacing: 1px;
}
.gender-item.male .gender-value { color: var(--primary); text-shadow: 0 0 8px var(--glow); }
.gender-item.female .gender-value { color: var(--secondary); text-shadow: 0 0 8px var(--glow-purple); }
.gender-bar {
  position: absolute; left: 0; right: 0; bottom: 0;
  height: 2px;
  background: var(--border-soft);
  overflow: hidden;
}
.gender-bar-fill {
  height: 100%;
  transition: width 0.8s ease-out;
}
.gender-item.male .gender-bar-fill { background: var(--grad-main); box-shadow: 0 0 6px var(--glow); }
.gender-item.female .gender-bar-fill { background: var(--grad-purple); box-shadow: 0 0 6px var(--glow-purple); }

.kpi-row-small {
  display: flex; gap: 4px;
  flex: 0 0 auto;
}
.kpi-card-sm {
  flex: 1;
  padding: 3px 5px;
  background: linear-gradient(135deg, var(--bg-soft) 0%, rgba(244, 114, 182, 0.02) 100%);
  border: 1px solid var(--border);
  border-radius: 5px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s;
  box-shadow: 0 0 8px rgba(244, 114, 182, 0.06);
}
.kpi-card-sm:hover { border-color: var(--border-hover); transform: translateY(-1px); box-shadow: 0 4px 12px rgba(244, 114, 182, 0.12); }
.kpi-sm-label {
  font-size: 10px; color: var(--text-dim);
  margin-bottom: 3px;
  letter-spacing: 0.5px;
}
.kpi-sm-value {
  font-size: 17px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  color: var(--accent);
  text-shadow: 0 0 10px var(--accent-glow);
  letter-spacing: 1px;
  animation: sm-value-pulse 3s ease-in-out infinite;
}
@keyframes sm-value-pulse {
  0%, 100% { text-shadow: 0 0 8px var(--accent-glow); }
  50% { text-shadow: 0 0 14px var(--accent-glow), 0 0 22px rgba(244, 114, 182, 0.15); }
}
.kpi-sm-bar {
  margin-top: 4px;
  height: 2px;
  background: var(--border-soft);
  border-radius: 2px;
  overflow: hidden;
}
.kpi-sm-bar-fill {
  height: 100%;
  background: var(--grad-pink);
  box-shadow: 0 0 6px var(--accent-glow);
  transition: width 0.8s ease-out;
}

.chart-area {
  width: 100%; height: 150px; min-height: 130px;
  position: relative;
  background: radial-gradient(ellipse at center, rgba(0, 212, 255, 0.02) 0%, transparent 70%);
  border-radius: 4px;
}
.chart-area::before {
  content: '';
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(0, 212, 255, 0.02) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.02) 1px, transparent 1px);
  background-size: 30px 30px;
  pointer-events: none;
  border-radius: 4px;
}

.map-canvas {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
}
.map-loading {
  position: absolute; inset: 0;
  display: flex; flex-direction: column; align-items: center; justify-content: center;
  gap: 16px;
  z-index: 5;
}
.map-radar {
  width: 120px; height: 120px;
  border: 2px solid var(--primary);
  border-radius: 50%;
  position: relative;
  animation: radar-rotate 3s linear infinite;
  box-shadow: 0 0 40px var(--glow), 0 0 80px var(--glow-soft);
}
.map-radar::before {
  content: ''; position: absolute; inset: -12px;
  border: 1px solid var(--primary);
  border-radius: 50%;
  opacity: 0.5;
  animation: radar-pulse 2s ease-out infinite;
}
.map-radar::after {
  content: ''; position: absolute; inset: -25px;
  border: 1px dashed var(--primary);
  border-radius: 50%;
  opacity: 0.25;
}
@keyframes radar-rotate { 0% { transform: rotate(0); } 100% { transform: rotate(360deg); } }
@keyframes radar-pulse { 0% { transform: scale(0.8); opacity: 0.8; } 100% { transform: scale(1.3); opacity: 0; } }
.map-loading-text {
  font-size: 14px; color: var(--primary);
  letter-spacing: 3px;
  text-shadow: 0 0 10px var(--glow);
  animation: loading-text-pulse 1.5s ease-in-out infinite;
}
@keyframes loading-text-pulse {
  0%, 100% { opacity: 0.8; }
  50% { opacity: 1; }
}
.map-back-btn {
  position: absolute;
  left: 16px;
  bottom: 16px;
  z-index: 10;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 8px 18px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 6px;
  color: var(--text);
  font-size: 13px;
  font-family: inherit;
  cursor: pointer;
  transition: all 0.3s;
  backdrop-filter: blur(8px);
}
.map-back-btn:hover {
  border-color: var(--primary);
  color: var(--primary);
  box-shadow: 0 0 16px var(--glow-soft);
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
.map-deco {
  position: absolute; inset: 0;
  pointer-events: none;
  opacity: 0;
  transition: opacity 0.8s ease-out;
}
.map-deco-ready { opacity: 1; }
.map-grid {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(0, 212, 255, 0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.04) 1px, transparent 1px);
  background-size: 50px 50px;
}
.map-corner {
  position: absolute; width: 26px; height: 26px;
}
.map-corner::before, .map-corner::after {
  content: ''; position: absolute;
  background: var(--primary);
  box-shadow: 0 0 12px var(--glow);
}
.mc-tl { top: 12px; left: 12px; }
.mc-tl::before { top: 0; left: 0; width: 22px; height: 2px; }
.mc-tl::after { top: 0; left: 0; width: 2px; height: 22px; }
.mc-tr { top: 12px; right: 12px; }
.mc-tr::before { top: 0; right: 0; width: 22px; height: 2px; }
.mc-tr::after { top: 0; right: 0; width: 2px; height: 22px; }
.mc-bl { bottom: 12px; left: 12px; }
.mc-bl::before { bottom: 0; left: 0; width: 22px; height: 2px; }
.mc-bl::after { bottom: 0; left: 0; width: 2px; height: 22px; }
.mc-br { bottom: 12px; right: 12px; }
.mc-br::before { bottom: 0; right: 0; width: 22px; height: 2px; }
.mc-br::after { bottom: 0; right: 0; width: 2px; height: 22px; }
.map-data-strip {
  position: absolute; bottom: 10px; left: 50%;
  transform: translateX(-50%);
  display: flex; gap: 18px;
  padding: 8px 18px;
  background: rgba(0, 20, 50, 0.75);
  border: 1px solid var(--border);
  border-radius: 6px;
  font-size: 11px;
  color: var(--text-dim);
  backdrop-filter: blur(6px);
  box-shadow: 0 0 20px rgba(0, 212, 255, 0.1);
}
.map-data-strip b { color: var(--primary); font-weight: 600; font-family: 'Consolas', monospace; text-shadow: 0 0 6px var(--glow-soft); }
.map-drill-tip { color: var(--accent); text-shadow: 0 0 6px var(--accent-glow); }

.map-area {
  position: relative;
  display: flex; flex-direction: column;
  background:
    radial-gradient(ellipse at center, rgba(0, 212, 255, 0.04) 0%, transparent 70%),
    linear-gradient(135deg, var(--bg-card), var(--bg-deep));
  border: 1px solid var(--border);
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 0 50px rgba(0, 212, 255, 0.12), inset 0 0 50px rgba(0, 212, 255, 0.03);
  animation: map-glow-breathe 5s ease-in-out infinite;
  min-height: 0;
}
@keyframes map-glow-breathe {
  0%, 100% { box-shadow: 0 0 50px rgba(0, 212, 255, 0.12), inset 0 0 50px rgba(0, 212, 255, 0.03); }
  50% { box-shadow: 0 0 70px rgba(0, 212, 255, 0.22), inset 0 0 70px rgba(0, 212, 255, 0.06); }
}
.map-area::before {
  content: ''; position: absolute; inset: 0;
  background:
    linear-gradient(rgba(0, 212, 255, 0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.05) 1px, transparent 1px);
  background-size: 40px 40px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 75%);
  -webkit-mask-image: radial-gradient(ellipse at center, black 0%, transparent 75%);
  pointer-events: none;
}

.map-legend {
  display: flex; align-items: center; justify-content: space-between;
  padding: 6px 12px;
  background: linear-gradient(135deg, var(--bg-soft) 0%, rgba(0, 212, 255, 0.02) 100%);
  border: 1px solid var(--border);
  border-radius: 6px;
  box-shadow: 0 0 15px rgba(0, 212, 255, 0.08);
}
.legend-title {
  font-size: 12px; color: var(--text-dim);
  letter-spacing: 1px;
}
.legend-items {
  display: flex; gap: 12px;
}
.legend-item {
  display: flex; align-items: center; gap: 5px;
  font-size: 11px; color: var(--text-dim);
}
.legend-dot {
  width: 12px; height: 12px; border-radius: 3px;
  background: var(--primary);
}
.legend-item:nth-child(1) .legend-dot { background: var(--primary); box-shadow: 0 0 10px var(--glow); }
.legend-item:nth-child(2) .legend-dot { background: var(--primary-glow, var(--primary)); box-shadow: 0 0 10px var(--glow-soft); }
.legend-item:nth-child(3) .legend-dot { background: var(--secondary); box-shadow: 0 0 10px var(--glow-purple); }
.legend-item:nth-child(4) .legend-dot { background: var(--accent); box-shadow: 0 0 10px var(--accent-glow, var(--glow-soft)); }

.gauge-row {
  display: flex; gap: 10px; height: 130px; min-height: 110px;
}
.gauge-item {
  flex: 1; min-height: 110px;
  background: radial-gradient(ellipse at center, rgba(0, 212, 255, 0.03) 0%, transparent 70%);
  border-radius: 6px;
}

.alert-list {
  display: flex; flex-direction: column; gap: 4px;
  max-height: 80px;
  overflow-y: auto;
  padding-right: 4px;
}
.alert-item {
  display: flex; align-items: center; gap: 6px;
  padding: 4px 8px;
  background: linear-gradient(135deg, var(--bg-soft) 0%, rgba(0, 212, 255, 0.02) 100%);
  border: 1px solid var(--border);
  border-radius: 5px;
  transition: all 0.3s;
  cursor: pointer;
}
.alert-item:hover { 
  background: var(--bg-card); 
  transform: translateX(2px); 
  box-shadow: 0 4px 12px rgba(0, 212, 255, 0.12);
}
.alert-item.danger { border-left: 2px solid #ef4444; box-shadow: -2px 0 8px rgba(239, 68, 68, 0.2); }
.alert-item.warning { border-left: 2px solid var(--warning); box-shadow: -2px 0 8px rgba(251, 191, 36, 0.2); }
.alert-item.info { border-left: 2px solid var(--primary); box-shadow: -2px 0 8px var(--glow-soft); }
.alert-icon { font-size: 12px; flex-shrink: 0; }
.alert-msg {
  font-size: 10px; color: var(--text);
  line-height: 1.3;
}

.tech-tab-label { font-size: 12px; }

::-webkit-scrollbar { width: 6px; height: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: var(--border); border-radius: 3px; }
::-webkit-scrollbar-thumb:hover { background: var(--primary); }

@media (max-width: 1500px) {
  .national-body {
    grid-template-columns: 26% 1fr 26%;
  }
  .national-title { font-size: 26px; }
  .kpi-card-lg-value { font-size: 24px; }
}

@media (max-width: 1400px) {
  .national-body {
    grid-template-columns: 26% 1fr 26%;
  }
  .national-title { font-size: 24px; }
  .kpi-card-lg-value { font-size: 22px; }
  .chart-area { min-height: 120px; }
}

@media (max-width: 1200px) {
  .app-root { height: auto; min-height: 100vh; overflow-y: auto; }
  .national-body {
    grid-template-columns: 1fr;
  }
  .national-title { font-size: 22px; }
  .kpi-grid { grid-template-columns: 1fr 1fr; }
  .map-area { min-height: 320px; }
}

@media (max-width: 1024px) {
  .national-body {
    grid-template-columns: 1fr;
    grid-template-rows: auto auto auto;
  }
  .left-col, .right-col { flex-direction: row; flex-wrap: wrap; }
  .tech-panel { flex: 1; min-width: 280px; }
  .map-area { min-height: 340px; }
  .national-title { font-size: 22px; letter-spacing: 2px; }
  .title-bracket { font-size: 24px; }
  .time-clock { display: none; }
}

@media (max-width: 768px) {
  .app-root { padding: 0; }
  .national-header { flex-direction: column; gap: 8px; padding: 12px 16px; }
  .header-deco { display: none; }
  .national-tabs { flex-wrap: wrap; gap: 6px; }
  .tech-tab { padding: 6px 14px; font-size: 12px; }
  .national-body { padding: 8px 12px 16px; gap: 8px; }
  .tech-panel { padding: 10px; min-width: 100%; }
  .kpi-grid { grid-template-columns: 1fr 1fr; gap: 6px; }
  .map-area { min-height: 280px; }
  .map-corner { display: none; }
}

@media (max-width: 480px) {
  .national-title { font-size: 18px; letter-spacing: 1px; }
  .title-bracket { display: none; }
  .kpi-grid { grid-template-columns: 1fr; }
  .kpi-card-lg { padding: 10px; }
  .kpi-card-lg-value { font-size: 20px; }
  .map-area { min-height: 240px; }
  .map-corner { display: none; }
}
</style>
