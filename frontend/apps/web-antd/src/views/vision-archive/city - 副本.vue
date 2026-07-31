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
        <span class="title-text">{{ currentCity }}中小学生健康监测大屏</span>
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
    <main class="national-body">
      <aside class="left-col">
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">筛选条件</span>
          </header>
          <div class="tech-panel-body">
            <div class="filter-bar">
              <div class="filter-item">
                <label>年份：</label>
                <div class="select-box">
                  <select v-model="currentFilter.year">
                    <option value="2026">2026</option>
                    <option value="2025">2025</option>
                    <option value="2024">2024</option>
                  </select>
                  <span class="arrow-down">▼</span>
                </div>
              </div>
              <div class="filter-item">
                <label>区县：</label>
                <div class="select-box">
                  <select v-model="currentFilter.district">
                    <option value="">全部</option>
                    <option v-for="d in districtList" :key="d" :value="d">{{ d }}</option>
                  </select>
                  <span class="arrow-down">▼</span>
                </div>
              </div>
              <div class="btn-group">
                <button class="btn-search" @click="handleSearch">
                  <span class="icon">🔍</span> 搜索
                </button>
                <button class="btn-reset" @click="handleReset">
                  <span class="icon">↺</span> 重置
                </button>
              </div>
            </div>
          </div>
        </section>
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
                  <div class="kpi-lg-label">{{ currentCity }}{{ currentMetric.name }}率</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ currentCityData.rate }}</span><span class="unit">%</span>
                  </div>
                  <div :class="['kpi-lg-trend', currentCityData.trend > 0 ? 'up' : 'down']">
                    <span class="trend-arrow">{{ currentCityData.trend > 0 ? '▲' : '▼' }}</span>
                    {{ Math.abs(currentCityData.trend) }}% 同比
                  </div>
                </div>
                <div class="kpi-card-lg">
                  <div class="kpi-card-deco"></div>
                  <div class="kpi-card-hex"><span></span><span></span><span></span></div>
                  <div class="kpi-lg-label">学生总数（千）</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ currentCityData.students }}</span>
                  </div>
                  <div class="kpi-lg-trend down">覆盖 {{ currentCityData.districts }} 区县</div>
                </div>
              </div>
              <div class="kpi-row-gender">
                <div class="gender-item male">
                  <span class="gender-icon">♂</span>
                  <div class="gender-info">
                    <span class="gender-label">男生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ currentCityData.maleRate }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: currentCityData.maleRate + '%' }"></div></div>
                </div>
                <div class="gender-item female">
                  <span class="gender-icon">♀</span>
                  <div class="gender-info">
                    <span class="gender-label">女生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ currentCityData.femaleRate }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: currentCityData.femaleRate + '%' }"></div></div>
                </div>
              </div>
              <div class="kpi-row-small">
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">小学{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentCityData.primary }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentCityData.primary + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">初中{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentCityData.junior }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentCityData.junior + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">高中{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentCityData.senior }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentCityData.senior + '%' }"></div></div>
                </div>
              </div>
            </div>
          </div>
        </section>
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">区县{{ currentMetric.name }}率排名</span>
            <span class="panel-tag">TOP {{ districtList.length > 0 ? Math.min(10, districtList.length) : 10 }}</span>
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
                <span class="alert-msg">{{ currentCityData.topDistrict }}{{ currentMetric.name }}率达{{ currentCityData.topRate }}%，高于全市平均水平</span>
              </div>
              <div class="alert-item warning">
                <span class="alert-icon">🟡</span>
                <span class="alert-msg">3个街道{{ currentMetric.name }}监测覆盖率低于40%，需加强推进</span>
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
              <span>经度：<b>{{ currentCityCenter[0].toFixed(1) }}°E</b></span>
              <span>纬度：<b>{{ currentCityCenter[1].toFixed(1) }}°N</b></span>
              <span>缩放：<b>{{ mapZoom.toFixed(2) }}x</b></span>
              <span v-if="mapLoaded" class="map-drill-tip">{{ currentCity }}地图</span>
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

const CITY_MAP_CONFIG = {
  '110101': { name: '东城区', short: '东城', center: [116.42, 39.93], zoom: 2.5 },
  '110102': { name: '西城区', short: '西城', center: [116.36, 39.92], zoom: 2.5 },
  '110105': { name: '朝阳区', short: '朝阳', center: [116.45, 39.92], zoom: 2.3 },
  '110108': { name: '海淀区', short: '海淀', center: [116.30, 39.98], zoom: 2.3 },
  '110106': { name: '丰台区', short: '丰台', center: [116.28, 39.86], zoom: 2.3 },
  '110107': { name: '石景山区', short: '石景山', center: [116.22, 39.90], zoom: 2.3 },
  '110112': { name: '通州区', short: '通州', center: [116.65, 39.91], zoom: 2.3 },
  '110114': { name: '昌平区', short: '昌平', center: [116.23, 40.22], zoom: 2.0 },
  '110113': { name: '顺义区', short: '顺义', center: [116.65, 40.08], zoom: 2.0 },
  '110115': { name: '大兴区', short: '大兴', center: [116.34, 39.73], zoom: 2.3 },
  '120101': { name: '和平区', short: '和平', center: [117.21, 39.12], zoom: 2.5 },
  '120102': { name: '河东区', short: '河东', center: [117.24, 39.13], zoom: 2.5 },
  '120103': { name: '河西区', short: '河西', center: [117.22, 39.10], zoom: 2.5 },
  '120104': { name: '南开区', short: '南开', center: [117.16, 39.13], zoom: 2.5 },
  '120105': { name: '河北区', short: '河北', center: [117.20, 39.14], zoom: 2.5 },
  '120106': { name: '红桥区', short: '红桥', center: [117.14, 39.17], zoom: 2.5 },
  '370100': { name: '济南市', short: '济南', center: [117.12, 36.65], zoom: 1.8 },
  '370200': { name: '青岛市', short: '青岛', center: [120.38, 36.07], zoom: 1.8 },
  '370300': { name: '淄博市', short: '淄博', center: [118.05, 36.81], zoom: 1.8 },
  '370400': { name: '枣庄市', short: '枣庄', center: [117.32, 34.81], zoom: 1.8 },
  '370500': { name: '东营市', short: '东营', center: [118.67, 37.43], zoom: 1.8 },
  '370600': { name: '烟台市', short: '烟台', center: [121.45, 37.46], zoom: 1.8 },
  '370700': { name: '潍坊市', short: '潍坊', center: [119.10, 36.70], zoom: 1.8 },
  '370800': { name: '济宁市', short: '济宁', center: [116.58, 35.42], zoom: 1.8 },
  '370900': { name: '泰安市', short: '泰安', center: [117.09, 36.19], zoom: 1.8 },
  '371000': { name: '威海市', short: '威海', center: [122.12, 37.51], zoom: 1.8 },
  '371100': { name: '日照市', short: '日照', center: [119.53, 35.42], zoom: 1.8 },
  '371300': { name: '临沂市', short: '临沂', center: [118.35, 35.10], zoom: 1.8 },
  '371400': { name: '德州市', short: '德州', center: [116.36, 37.44], zoom: 1.8 },
  '371500': { name: '聊城市', short: '聊城', center: [115.98, 36.45], zoom: 1.8 },
  '371600': { name: '滨州市', short: '滨州', center: [117.97, 37.38], zoom: 1.8 },
  '371700': { name: '菏泽市', short: '菏泽', center: [115.48, 35.23], zoom: 1.8 },
  '130100': { name: '石家庄市', short: '石家庄', center: [114.51, 38.04], zoom: 1.8 },
  '130200': { name: '唐山市', short: '唐山', center: [118.18, 39.63], zoom: 1.8 },
  '140100': { name: '太原市', short: '太原', center: [112.55, 37.87], zoom: 1.8 },
  '150100': { name: '呼和浩特市', short: '呼和浩特', center: [111.75, 40.84], zoom: 1.8 },
  '210100': { name: '沈阳市', short: '沈阳', center: [123.43, 41.80], zoom: 1.8 },
  '210200': { name: '大连市', short: '大连', center: [121.62, 38.92], zoom: 1.8 },
  '220100': { name: '长春市', short: '长春', center: [125.32, 43.82], zoom: 1.8 },
  '230100': { name: '哈尔滨市', short: '哈尔滨', center: [126.53, 45.80], zoom: 1.8 },
  '310101': { name: '黄浦区', short: '黄浦', center: [121.49, 31.23], zoom: 2.5 },
  '310104': { name: '徐汇区', short: '徐汇', center: [121.44, 31.18], zoom: 2.3 },
  '310105': { name: '长宁区', short: '长宁', center: [121.42, 31.22], zoom: 2.3 },
  '310106': { name: '静安区', short: '静安', center: [121.45, 31.23], zoom: 2.3 },
  '310115': { name: '浦东新区', short: '浦东', center: [121.54, 31.22], zoom: 2.0 },
  '310112': { name: '闵行区', short: '闵行', center: [121.38, 31.11], zoom: 2.0 },
  '320100': { name: '南京市', short: '南京', center: [118.78, 32.04], zoom: 1.8 },
  '320200': { name: '无锡市', short: '无锡', center: [120.31, 31.59], zoom: 1.8 },
  '320500': { name: '苏州市', short: '苏州', center: [120.58, 31.30], zoom: 1.8 },
  '330100': { name: '杭州市', short: '杭州', center: [120.15, 30.28], zoom: 1.8 },
  '330200': { name: '宁波市', short: '宁波', center: [121.55, 29.87], zoom: 1.8 },
  '340100': { name: '合肥市', short: '合肥', center: [117.28, 31.86], zoom: 1.8 },
  '350100': { name: '福州市', short: '福州', center: [119.30, 26.08], zoom: 1.8 },
  '350200': { name: '厦门市', short: '厦门', center: [118.09, 24.48], zoom: 1.8 },
  '360100': { name: '南昌市', short: '南昌', center: [115.89, 28.68], zoom: 1.8 },
  '410100': { name: '郑州市', short: '郑州', center: [113.65, 34.76], zoom: 1.8 },
  '410300': { name: '洛阳市', short: '洛阳', center: [112.45, 34.62], zoom: 1.8 },
  '420100': { name: '武汉市', short: '武汉', center: [114.31, 30.52], zoom: 1.8 },
  '430100': { name: '长沙市', short: '长沙', center: [112.98, 28.19], zoom: 1.8 },
  '440100': { name: '广州市', short: '广州', center: [113.27, 23.13], zoom: 1.7 },
  '440300': { name: '深圳市', short: '深圳', center: [114.06, 22.55], zoom: 1.8 },
  '440400': { name: '珠海市', short: '珠海', center: [113.58, 22.27], zoom: 1.8 },
  '440600': { name: '佛山市', short: '佛山', center: [113.12, 23.02], zoom: 1.8 },
  '450100': { name: '南宁市', short: '南宁', center: [108.37, 22.82], zoom: 1.8 },
  '460100': { name: '海口市', short: '海口', center: [110.33, 20.03], zoom: 1.8 },
  '500103': { name: '渝中区', short: '渝中', center: [106.55, 29.56], zoom: 2.3 },
  '510100': { name: '成都市', short: '成都', center: [104.07, 30.67], zoom: 1.6 },
  '520100': { name: '贵阳市', short: '贵阳', center: [106.63, 26.65], zoom: 1.8 },
  '530100': { name: '昆明市', short: '昆明', center: [102.71, 25.04], zoom: 1.8 },
  '610100': { name: '西安市', short: '西安', center: [108.94, 34.26], zoom: 1.8 },
  '620100': { name: '兰州市', short: '兰州', center: [103.82, 36.06], zoom: 1.8 },
  '630100': { name: '西宁市', short: '西宁', center: [101.78, 36.62], zoom: 1.8 },
  '640100': { name: '银川市', short: '银川', center: [106.23, 38.48], zoom: 1.8 },
  '650100': { name: '乌鲁木齐市', short: '乌鲁木齐', center: [87.62, 43.79], zoom: 1.8 },
  '540100': { name: '拉萨市', short: '拉萨', center: [91.11, 29.97], zoom: 1.8 },
};

const DISTRICT_CODES_MAP = {
  '110101': { '东华门街道': '110101001', '景山街道': '110101002', '交道口街道': '110101003', '安定门街道': '110101004', '北新桥街道': '110101005', '东四十条街道': '110101006', '朝阳门街道': '110101007', '建国门街道': '110101008', '东直门街道': '110101009', '东花市街道': '110101010' },
  '110102': { '西长安街街道': '110102001', '新街口街道': '110102002', '月坛街道': '110102003', '展览路街道': '110102004', '德胜街道': '110102005', '什刹海街道': '110102006' },
  '110105': { '建外街道': '110105001', '朝外街道': '110105002', '呼家楼街道': '110105003', '三里屯街道': '110105004', '团结湖街道': '110105005', '六里屯街道': '110105006' },
  '110108': { '海淀街道': '110108001', '中关村街道': '110108002', '学院路街道': '110108003', '清河街道': '110108004', '西三旗街道': '110108005', '上地街道': '110108006' },
  '110106': { '丰台街道': '110106001', '卢沟桥街道': '110106002', '太平桥街道': '110106003', '大红门街道': '110106004', '南苑街道': '110106005' },
  '110107': { '八宝山街道': '110107001', '老山街道': '110107002', '八角街道': '110107003', '古城街道': '110107004', '苹果园街道': '110107005' },
  '110112': { '中仓街道': '110112001', '新华街道': '110112002', '北苑街道': '110112003', '玉桥街道': '110112004', '永顺镇': '110112005' },
  '110114': { '城北街道': '110114001', '城南街道': '110114002', '回龙观街道': '110114003', '天通苑北街道': '110114004' },
  '110113': { '胜利街道': '110113001', '光明街道': '110113002', '仁和镇': '110113003', '后沙峪镇': '110113004' },
  '110115': { '兴丰大街': '110115001', '林校路街道': '110115002', '清源街道': '110115003', '亦庄镇': '110115004' },
  '120101': { '劝业场街道': '120101001', '小白楼街道': '120101002', '南市街道': '120101003', '新兴街道': '120101004', '南营门街道': '120101005' },
  '120102': { '大王庄街道': '120102001', '大直沽街道': '120102002', '中山门街道': '120102003', '富民路街道': '120102004', '二号桥街道': '120102005' },
  '120103': { '大营门街道': '120103001', '下瓦房街道': '120103002', '桃园街道': '120103003', '挂甲寺街道': '120103004', '马场街道': '120103005' },
  '120104': { '长虹街道': '120104001', '鼓楼街道': '120104002', '兴南街道': '120104003', '广开街道': '120104004', '万兴街道': '120104005' },
  '120105': { '光复道街道': '120105001', '望海楼街道': '120105002', '鸿顺里街道': '120105003', '三马路街道': '120105004', '江都路街道': '120105005' },
  '120106': { '西于庄街道': '120106001', '咸阳北路街道': '120106002', '丁字沽街道': '120106003', '西沽街道': '120106004', '邵公庄街道': '120106005' },
  '370100': { '历下区': '370102', '市中区': '370103', '槐荫区': '370104', '天桥区': '370105', '历城区': '370112', '长清区': '370113' },
  '370200': { '市南区': '370202', '市北区': '370203', '黄岛区': '370211', '崂山区': '370212', '李沧区': '370213', '城阳区': '370214' },
  '370300': { '张店区': '370303', '淄川区': '370302', '博山区': '370304', '临淄区': '370305', '周村区': '370306' },
  '370400': { '市中区': '370402', '薛城区': '370403', '峄城区': '370404', '台儿庄区': '370405' },
  '370500': { '东营区': '370502', '河口区': '370503', '广饶县': '370523', '利津县': '370522' },
  '370600': { '芝罘区': '370602', '福山区': '370611', '牟平区': '370612', '莱山区': '370613' },
  '370700': { '潍城区': '370702', '坊子区': '370704', '奎文区': '370705', '寒亭区': '370703' },
  '370800': { '任城区': '370811', '兖州区': '370812', '邹城市': '370883', '曲阜市': '370881' },
  '370900': { '泰山区': '370902', '岱岳区': '370911', '新泰市': '370982', '肥城市': '370983' },
  '371000': { '环翠区': '371002', '文登区': '371003', '荣成市': '371082', '乳山市': '371083' },
  '371100': { '东港区': '371102', '岚山区': '371103', '莒县': '371122', '五莲县': '371125' },
  '371300': { '兰山区': '371302', '罗庄区': '371311', '河东区': '371312', '沂南县': '371321' },
  '371400': { '德城区': '371402', '陵城区': '371403', '乐陵市': '371481', '禹城市': '371482' },
  '371500': { '东昌府区': '371502', '茌平区': '371523', '临清市': '371581', '高唐县': '371526' },
  '371600': { '滨城区': '371602', '沾化区': '371603', '邹平市': '371681', '惠民县': '371621' },
  '371700': { '牡丹区': '371702', '定陶区': '371703', '曹县': '371721', '单县': '371722' },
  '130100': { '长安区': '130102', '桥西区': '130104', '新华区': '130105', '井陉矿区': '130107', '裕华区': '130108' },
  '130200': { '路南区': '130202', '路北区': '130203', '古冶区': '130204', '开平区': '130205', '丰南区': '130207' },
  '140100': { '小店区': '140105', '迎泽区': '140106', '杏花岭区': '140107', '尖草坪区': '140108', '万柏林区': '140109' },
  '150100': { '新城区': '150102', '回民区': '150103', '玉泉区': '150104', '赛罕区': '150105' },
  '210100': { '和平区': '210102', '沈河区': '210103', '大东区': '210104', '皇姑区': '210105', '铁西区': '210106' },
  '210200': { '中山区': '210202', '西岗区': '210203', '沙河口区': '210204', '甘井子区': '210211', '旅顺口区': '210212' },
  '220100': { '南关区': '220102', '宽城区': '220103', '朝阳区': '220104', '二道区': '220105', '绿园区': '220106' },
  '230100': { '道里区': '230102', '南岗区': '230103', '道外区': '230104', '平房区': '230108', '松北区': '230109' },
  '310101': { '南京东路街道': '310101001', '外滩街道': '310101002', '豫园街道': '310101003', '打浦桥街道': '310101004', '老西门街道': '310101005', '小东门街道': '310101006' },
  '310104': { '天平路街道': '310104001', '湖南路街道': '310104002', '斜土路街道': '310104003', '枫林路街道': '310104004', '田林街道': '310104005', '虹梅路街道': '310104006' },
  '310105': { '华阳路街道': '310105001', '江苏路街道': '310105002', '新华路街道': '310105003', '周家桥街道': '310105004', '天山路街道': '310105005' },
  '310106': { '江宁路街道': '310106001', '石门二路街道': '310106002', '南京西路街道': '310106003', '静安寺街道': '310106004', '曹家渡街道': '310106005' },
  '310115': { '陆家嘴街道': '310115001', '潍坊新村街道': '310115002', '陆家嘴金融贸易区': '310115003', '花木街道': '310115004', '金桥街道': '310115005' },
  '310112': { '江川路街道': '310112001', '古美路街道': '310112002', '新虹街道': '310112003', '浦锦街道': '310112004', '莘庄镇': '310112005' },
  '320100': { '玄武区': '320102', '秦淮区': '320104', '建邺区': '320105', '鼓楼区': '320106', '浦口区': '320111', '栖霞区': '320113', '雨花台区': '320114' },
  '320200': { '崇安区': '320202', '南长区': '320203', '北塘区': '320204', '滨湖区': '320211', '锡山区': '320205', '惠山区': '320206' },
  '320500': { '姑苏区': '320508', '虎丘区': '320505', '吴中区': '320506', '相城区': '320507', '吴江区': '320509' },
  '330100': { '上城区': '330102', '下城区': '330103', '江干区': '330104', '拱墅区': '330105', '西湖区': '330106', '滨江区': '330108' },
  '330200': { '海曙区': '330203', '江北区': '330205', '北仑区': '330206', '镇海区': '330211', '鄞州区': '330212' },
  '340100': { '瑶海区': '340102', '庐阳区': '340103', '蜀山区': '340104', '包河区': '340111', '高新区': '340191' },
  '350100': { '鼓楼区': '350102', '台江区': '350103', '仓山区': '350104', '马尾区': '350105', '晋安区': '350111' },
  '350200': { '思明区': '350203', '海沧区': '350205', '湖里区': '350206', '集美区': '350211', '同安区': '350212' },
  '360100': { '东湖区': '360102', '西湖区': '360103', '青云谱区': '360104', '青山湖区': '360111', '新建区': '360112' },
  '410100': { '中原区': '410102', '二七区': '410103', '管城回族区': '410104', '金水区': '410105', '上街区': '410106', '惠济区': '410108' },
  '410300': { '老城区': '410302', '西工区': '410303', '瀍河回族区': '410304', '涧西区': '410305', '洛龙区': '410307' },
  '420100': { '江岸区': '420102', '江汉区': '420103', '硚口区': '420104', '汉阳区': '420105', '武昌区': '420106', '洪山区': '420111' },
  '430100': { '芙蓉区': '430102', '天心区': '430103', '岳麓区': '430104', '开福区': '430105', '雨花区': '430111', '望城区': '430112' },
  '440100': { '越秀区': '440104', '海珠区': '440105', '荔湾区': '440103', '天河区': '440106', '白云区': '440111', '黄埔区': '440112' },
  '440300': { '福田区': '440304', '罗湖区': '440303', '南山区': '440305', '宝安区': '440306', '龙岗区': '440307', '盐田区': '440308' },
  '440400': { '香洲区': '440402', '斗门区': '440403', '金湾区': '440404' },
  '440600': { '禅城区': '440604', '南海区': '440605', '顺德区': '440606', '三水区': '440607', '高明区': '440608' },
  '450100': { '兴宁区': '450102', '青秀区': '450103', '江南区': '450105', '西乡塘区': '450107', '邕宁区': '450109' },
  '460100': { '秀英区': '460105', '龙华区': '460106', '琼山区': '460107', '美兰区': '460108' },
  '500103': { '七星岗街道': '500103001', '解放碑街道': '500103002', '两路口街道': '500103003', '上清寺街道': '500103004', '菜园坝街道': '500103005' },
  '510100': { '锦江区': '510104', '青羊区': '510105', '金牛区': '510106', '武侯区': '510107', '成华区': '510108', '龙泉驿区': '510112' },
  '520100': { '南明区': '520102', '云岩区': '520103', '花溪区': '520111', '乌当区': '520112', '白云区': '520113', '观山湖区': '520115' },
  '530100': { '五华区': '530102', '盘龙区': '530103', '官渡区': '530111', '西山区': '530112', '东川区': '530113' },
  '610100': { '新城区': '610102', '碑林区': '610103', '莲湖区': '610104', '灞桥区': '610111', '未央区': '610112', '雁塔区': '610113' },
  '620100': { '城关区': '620102', '七里河区': '620103', '西固区': '620104', '安宁区': '620105', '红古区': '620111' },
  '630100': { '城东区': '630102', '城中区': '630103', '城西区': '630104', '城北区': '630105' },
  '640100': { '兴庆区': '640104', '西夏区': '640105', '金凤区': '640106', '永宁县': '640121', '贺兰县': '640122' },
  '650100': { '天山区': '650102', '沙依巴克区': '650103', '新市区': '650104', '水磨沟区': '650105', '头屯河区': '650106' },
  '540100': { '城关区': '540102', '堆龙德庆区': '540103', '达孜区': '540104', '林周县': '540121' },
};

const routeCode = computed(() => String(route.params.code || '370100'));
const cityNameFromGeo = ref('');
const currentCityConfig = computed(() => {
  if (CITY_MAP_CONFIG[routeCode.value]) return CITY_MAP_CONFIG[routeCode.value];
  const code = routeCode.value;
  const short = cityNameFromGeo.value || code;
  return { name: short, short, center: [116.40, 39.90], zoom: 1.8 };
});
const currentCity = computed(() => currentCityConfig.value.short);
const currentCityCenter = computed(() => currentCityConfig.value.center);

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

const currentMetric = computed(() => metricConfig[activeTab.value]);

const districtList = ref([]);
const currentFilter = reactive({ year: '2026', district: '' });

const computeCityMetricData = () => {
  const m = metricConfig[activeTab.value];
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rate = Math.round((m.baseRate + (seed % 12 - 6)) * 10) / 10;
  const students = Math.round(500 + seed * 5);
  const districts = districtList.value.length || 8;
  const maleRate = Math.round(rate * m.maleFactor * 10) / 10;
  const femaleRate = Math.round(rate * m.femaleFactor * 10) / 10;
  const primary = Math.round(rate * m.primaryFactor * 10) / 10;
  const junior = Math.round(rate * m.juniorFactor * 10) / 10;
  const senior = Math.round(rate * m.seniorFactor * 10) / 10;
  const topDistrict = districtList.value[0] || '最高';
  const topRate = Math.round((rate + 4 + (seed % 6)) * 10) / 10;
  return { rate, students, districts, trend: m.trend, maleRate, femaleRate, primary, junior, senior, topDistrict, topRate };
};

const currentCityData = computed(() => computeCityMetricData());

const rankingChartRef = ref(null);
const ageGenderChartRef = ref(null);
const urbanGaugeRef = ref(null);
const ruralGaugeRef = ref(null);
const trendChartRef = ref(null);
const interventionChartRef = ref(null);
const mapRef = ref(null);

const chartInstances = {};
const activeTab = ref('vision');
const isLight = ref(false);
const currentDate = ref('');
const currentTime = ref('');
const mapLoaded = ref(false);
const mapZoom = ref(1.8);
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

const handleSearch = () => { renderAllCharts(); };
const handleReset = () => { currentFilter.year = '2026'; currentFilter.district = ''; };

const goBack = () => {
  const code = routeCode.value;
  const provinceCode = code.slice(0, 2) + '0000';
  router.push({ path: `/vision/province/${provinceCode}`, query: { tab: activeTab.value } });
};

const drillToDistrict = (name) => {
  const codes = DISTRICT_CODES_MAP[routeCode.value] || {};
  let code = codes[name];
  if (!code) {
    const suffixes = ['区', '县', '市'];
    for (const s of suffixes) {
      const stripped = name.replace(s, '');
      if (codes[stripped]) { code = codes[stripped]; break; }
    }
  }
  if (code) router.push({ path: `/vision/county/${code}`, query: { tab: activeTab.value } });
  else console.warn('未找到区县编码:', name);
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

const getCityMapData = () => {
  const districts = districtList.value;
  if (districts.length === 0) return [];
  const baseRate = currentCityData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  return districts.map((name, i) => ({
    name,
    value: Math.round((baseRate + ((seed + i * 5) % 18 - 9)) * 10) / 10
  }));
};

const getMapOption = () => {
  if (!mapLoaded.value) return {};
  const c = getColors();
  const data = getCityMapData();
  if (data.length === 0) return {};
  const values = data.map(d => d.value);
  const minV = Math.min(...values);
  const maxV = Math.max(...values);
  const cfg = currentCityConfig.value;

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('item'),
      formatter: (p) => {
        return `<div style="font-weight:600">${p.name}</div>
          <div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value || 0}%</span></div>
          <div style="color:${c.textDim};font-size:11px;margin-top:4px">点击查看区县详情 →</div>`;
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
    series: [{
      type: 'map',
      map: 'city',
      roam: true,
      selectedMode: false,
      zoom: cfg.zoom,
      center: cfg.center,
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
      data,
    }],
  };
};

const getRankingOption = () => {
  const c = getColors();
  const districts = districtList.value.length ? districtList.value.slice(0, 10) : ['暂无数据'];
  const baseRate = currentCityData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rates = districts.map((_, i) => Math.round((baseRate + (12 - i * 1.2) + ((seed + i * 3) % 5 - 2)) * 10) / 10);
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
    grid: { left: 10, right: 50, top: 5, bottom: 5, containLabel: true },
    xAxis: { type: 'value', show: false, max: maxRate + 5 },
    yAxis: {
      type: 'category', data: districts, inverse: true,
      axisLine: { show: false }, axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold' },
    },
    series: [{
      name: currentMetric.value.name + '率',
      type: 'bar',
      data: rates.map(v => ({
        value: v,
        itemStyle: {
          color: makeGrad(c.primary, c.secondary),
          borderRadius: [0, 4, 4, 0],
          shadowBlur: isLight.value ? 4 : 8, shadowColor: `${c.primary}44`,
        },
      })),
      barWidth: 12,
      label: { show: true, position: 'right', color: c.primary, fontSize: 10, fontWeight: 'bold', formatter: '{c}%' },
    }],
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
  const baseRate = currentCityData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rateData = months.map((_, i) => Math.round((baseRate + i * 0.7 + ((seed + i * 3) % 4 - 2)) * 10) / 10);
  const targetData = months.map((_, i) => Math.round((baseRate + 2 - i * 0.3) * 10) / 10);

  return {
    backgroundColor: 'transparent',
    tooltip: { ...getTooltip('axis') },
    legend: { show: false },
    grid: { left: 40, right: 20, top: 10, bottom: 30 },
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

const getAgeGenderOption = () => {
  const c = getColors();
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const base = currentCityData.value.rate;
  const maleData = [
    { value: Math.round((base * 0.75 + (seed % 3)) * 10) / 10, name: '7-10岁' },
    { value: Math.round((base * 0.9 + (seed % 4)) * 10) / 10, name: '11-14岁' },
    { value: Math.round((base * 1.1 + (seed % 2)) * 10) / 10, name: '15-18岁' },
    { value: Math.round((base * 0.85 + (seed % 5)) * 10) / 10, name: '19-22岁' },
  ];
  const femaleData = [
    { value: Math.round((base * 0.82 + (seed % 3)) * 10) / 10, name: '7-10岁' },
    { value: Math.round((base * 1.05 + (seed % 4)) * 10) / 10, name: '11-14岁' },
    { value: Math.round((base * 1.18 + (seed % 2)) * 10) / 10, name: '15-18岁' },
    { value: Math.round((base * 0.92 + (seed % 5)) * 10) / 10, name: '19-22岁' },
  ];
  const maxV = Math.max(...maleData.map(d => d.value), ...femaleData.map(d => d.value));

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        const p = params[0];
        const gender = p.seriesName === '男生' ? '男' : '女';
        return `<div style="font-weight:600">${gender}生 ${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${p.color};font-weight:bold">${p.value}%</span></div>`;
      },
    },
    legend: {
      show: true,
      top: 0, right: 0,
      textStyle: { color: c.textDim, fontSize: 10 },
      itemWidth: 12, itemHeight: 8,
    },
    grid: { left: 30, right: 10, top: 24, bottom: 5, containLabel: true },
    xAxis: {
      type: 'value', show: false,
      max: Math.ceil(maxV + 5),
    },
    yAxis: {
      type: 'category',
      data: maleData.map(d => d.name),
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: c.textDim, fontSize: 10 },
      inverse: true,
    },
    series: [
      {
        name: '男生',
        type: 'bar',
        data: maleData.map(d => ({
          value: d.value,
          itemStyle: {
            color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'),
            borderRadius: [0, 3, 3, 0],
            shadowBlur: isLight.value ? 4 : 8,
            shadowColor: `${c.primary}55`,
          },
        })),
        barWidth: 8,
        label: {
          show: true, position: 'right',
          color: c.primary, fontSize: 9, fontWeight: 'bold',
          formatter: '{c}%',
        },
        z: 2,
      },
      {
        name: '女生',
        type: 'bar',
        data: femaleData.map(d => ({
          value: d.value,
          itemStyle: {
            color: makeGrad(c.secondary, isLight.value ? '#c4b5fd' : '#7c3aed'),
            borderRadius: [0, 3, 3, 0],
            shadowBlur: isLight.value ? 4 : 8,
            shadowColor: `${c.secondary}55`,
          },
        })),
        barWidth: 8,
        barGap: '30%',
        label: {
          show: true, position: 'right',
          color: c.secondary, fontSize: 9, fontWeight: 'bold',
          formatter: '{c}%',
        },
        z: 1,
      },
    ],
  };
};

const getInterventionOption = () => {
  const c = getColors();
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const base = currentCityData.value.rate;
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
    grid: { left: 80, right: 60, top: 8, bottom: 8 },
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
      const d = currentCityData.value;
      const ur = { urban: Math.round(d.rate * 0.9 * 10) / 10, rural: Math.round(d.rate * 1.2 * 10) / 10 };
      initChart(rankingChartRef, 'ranking', getRankingOption(), (params) => {
        if (params?.name) drillToDistrict(params.name);
      });
      initChart(ageGenderChartRef, 'ageGender', getAgeGenderOption());
      initChart(urbanGaugeRef, 'urbanGauge', getGaugeOption(ur.urban, `城区${currentMetric.value.name}率`, getColors().primary));
      initChart(ruralGaugeRef, 'ruralGauge', getGaugeOption(ur.rural, `县乡${currentMetric.value.name}率`, getColors().secondary));
      initChart(trendChartRef, 'trend', getTrendOption());
      initChart(interventionChartRef, 'intervention', getInterventionOption());
      if (mapLoaded.value) {
        initChart(mapRef, 'map', getMapOption(), (params) => {
          if (params?.name) drillToDistrict(params.name);
        });
      }
    });
  });
};

const loadMap = async () => {
  const code = routeCode.value;
  const cfg = currentCityConfig.value;
  const urls = [
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}_full.json`,
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}.json`,
  ];
  for (const url of urls) {
    try {
      const res = await fetch(url);
      if (!res.ok) continue;
      const geoJson = await res.json();
      echarts.registerMap('city', geoJson);
      const features = geoJson.features || [];
      if (features.length > 0) {
        const list = [];
        features.forEach(f => {
          const props = f.properties || {};
          const fullName = props.name || props.NAME || props.NL_NAME_1 || '';
          if (fullName && fullName.length > 0 && fullName.length < 15) {
            list.push(fullName);
          }
        });
        if (list.length >= 2) {
          districtList.value = list.slice(0, 30);
        } else {
          districtList.value = [];
        }
        const parentProps = geoJson.properties || {};
        const parentName = parentProps.name || parentProps.NAME || '';
        if (parentName && !CITY_MAP_CONFIG[code]) {
          cityNameFromGeo.value = parentName;
        }
      } else {
        districtList.value = [];
      }
      mapLoaded.value = true;
      mapZoom.value = cfg.zoom;
      setTimeout(() => renderAllCharts(), 200);
      return;
    } catch (e) {
      console.warn('地图加载失败', url, e);
    }
  }
  districtList.value = [];
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
  setTimeout(() => resizeAll(), 500);
  setTimeout(() => resizeAll(), 1500);
});

onBeforeUnmount(() => {
  if (timer) clearInterval(timer);
  window.removeEventListener('resize', resizeAll);
  if (themeObserver) { themeObserver.disconnect(); themeObserver = null; }
  disposeAll();
});

watch(activeTab, () => setTimeout(() => renderAllCharts(), 100));
watch(isLight, () => setTimeout(() => renderAllCharts(), 100));
watch(() => route.query.tab, (tab) => {
  if (tab && typeof tab === 'string' && ['vision','oral','mental','weight','bone'].includes(tab)) {
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
  --particle-count: 30;
  --particle-size: 2px;
  --particle-opacity: 0.6;
  --glow-intensity: 0.45;
  --border-glow: 0 0 12px var(--glow), 0 0 24px var(--glow-soft);
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
  --particle-count: 0;
  --particle-size: 0;
  --particle-opacity: 0;
  --glow-intensity: 0;
  --border-glow: none;
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
.app-root.theme-light .bg-stars { display: none; }
.app-root.theme-light .bg-decor::before,
.app-root.theme-light .bg-decor::after { display: none; }
.app-root.theme-light .bg-glow { opacity: 0.05; filter: blur(80px); mix-blend-mode: multiply; }
.app-root.theme-light .bg-grid { opacity: 0.25; }
.app-root.theme-light .tech-panel::before {
  background: linear-gradient(90deg, var(--primary) 0%, transparent 25%, transparent 75%, var(--secondary) 100%);
  background-size: 300% 100%;
  box-shadow: none;
  opacity: 0.7;
  animation: border-shine 4s linear infinite;
}
.app-root.theme-light .tech-panel::after {
  box-shadow: none;
  border-top-color: var(--primary);
  border-left-color: var(--primary);
  opacity: 0.8;
}
.app-root.theme-light .tech-panel > .corner-tr,
.app-root.theme-light .tech-panel > .corner-br,
.app-root.theme-light .tech-panel > .corner-bl {
  box-shadow: none;
  opacity: 0.8;
}
.app-root.theme-light .title-text {
  background: linear-gradient(180deg, #1e293b 0%, #0891b2 55%, #7c3aed 100%);
  -webkit-background-clip: text;
  background-clip: text;
  filter: none;
}
.app-root.theme-light .title-bracket {
  text-shadow: none;
  animation: none;
  filter: none;
}
.app-root.theme-light .tc-digit { text-shadow: none; }
.app-root.theme-light .kpi-lg-value { animation: none; }
.app-root.theme-light .kpi-lg-value .num { text-shadow: none; }
.app-root.theme-light .kpi-sm-value { animation: none; text-shadow: none; }
.app-root.theme-light .gender-item.male .gender-value { text-shadow: none; }
.app-root.theme-light .gender-item.female .gender-value { text-shadow: none; }
.app-root.theme-light .gender-item.male .gender-icon { box-shadow: none; }
.app-root.theme-light .gender-item.female .gender-icon { box-shadow: none; }
.app-root.theme-light .tech-tab.active {
  box-shadow: 0 4px 16px rgba(124, 58, 237, 0.28);
}
.app-root.theme-light .map-area {
  background: linear-gradient(180deg, rgba(255,255,255,0.8), rgba(238,241,247,0.95));
  box-shadow: 0 4px 24px rgba(15, 23, 42, 0.06);
  animation: none;
}
.app-root.theme-light .map-area::before { opacity: 0.3; }
.app-root.theme-light .map-deco-ready .map-grid { opacity: 0.2; }
.app-root.theme-light .map-corner::before,
.app-root.theme-light .map-corner::after {
  box-shadow: none;
}
.app-root.theme-light .tech-panel {
  background: #ffffff;
  border: 1px solid rgba(8, 145, 178, 0.12);
  box-shadow: 0 2px 12px rgba(15, 23, 42, 0.04), 0 0 0 1px rgba(8, 145, 178, 0.04);
}
.app-root.theme-light .tech-panel:hover {
  box-shadow: 0 6px 24px rgba(15, 23, 42, 0.08);
}
.app-root.theme-light .tech-panel-inner-border { display: none; }
.app-root.theme-light .kpi-card-lg,
.app-root.theme-light .kpi-card-sm {
  background: linear-gradient(135deg, rgba(8, 145, 178, 0.04) 0%, #ffffff 50%, rgba(124, 58, 237, 0.03) 100%);
  border: 1px solid rgba(8, 145, 178, 0.1);
  box-shadow: 0 2px 10px rgba(15, 23, 42, 0.04);
}
.app-root.theme-light .kpi-card-lg:hover,
.app-root.theme-light .kpi-card-sm:hover {
  box-shadow: 0 4px 16px rgba(15, 23, 42, 0.06);
}
.app-root.theme-light .btn-search {
  background: linear-gradient(135deg, #0891b2, #7c3aed);
  box-shadow: 0 4px 14px rgba(8, 145, 178, 0.3);
}
.app-root.theme-light .btn-search:hover {
  box-shadow: 0 6px 20px rgba(8, 145, 178, 0.4);
}
.app-root.theme-light .btn-reset {
  background: #ffffff;
  border: 1px solid rgba(8, 145, 178, 0.3);
  color: #0891b2;
}
.app-root.theme-light .btn-reset:hover {
  background: rgba(8, 145, 178, 0.06);
  box-shadow: 0 4px 12px rgba(8, 145, 178, 0.15);
}
.app-root.theme-light .alert-item {
  background: #ffffff;
  border: 1px solid rgba(8, 145, 178, 0.1);
}
.app-root.theme-light .alert-item:hover {
  background: #ffffff;
  box-shadow: 0 4px 12px rgba(15, 23, 42, 0.06);
}
.app-root.theme-light .alert-item.danger { border-left: 3px solid #ef4444; box-shadow: -3px 0 10px rgba(239, 68, 68, 0.15); }
.app-root.theme-light .alert-item.warning { border-left: 3px solid #d97706; box-shadow: -3px 0 10px rgba(217, 119, 6, 0.15); }
.app-root.theme-light .alert-item.info { border-left: 3px solid #0891b2; box-shadow: -3px 0 10px rgba(8, 145, 178, 0.15); }
.app-root.theme-light .gender-item.male {
  background: linear-gradient(135deg, rgba(8, 145, 178, 0.04) 0%, #ffffff 100%);
  border: 1px solid rgba(8, 145, 178, 0.1);
  border-left-color: #0891b2;
  box-shadow: -3px 0 12px rgba(8, 145, 178, 0.12);
}
.app-root.theme-light .gender-item.female {
  background: linear-gradient(135deg, rgba(124, 58, 237, 0.04) 0%, #ffffff 100%);
  border: 1px solid rgba(124, 58, 237, 0.1);
  border-left-color: #7c3aed;
  box-shadow: -3px 0 12px rgba(124, 58, 237, 0.1);
}
.app-root.theme-light .chart-area {
  background: radial-gradient(ellipse at center, rgba(8, 145, 178, 0.02) 0%, transparent 70%);
  box-shadow: none;
}
.app-root.theme-light .chart-area::before {
  background-image:
    linear-gradient(rgba(8, 145, 178, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(8, 145, 178, 0.03) 1px, transparent 1px);
  opacity: 0.4;
}
.app-root.theme-light .gauge-item {
  background: radial-gradient(ellipse at center, rgba(8, 145, 178, 0.03) 0%, transparent 70%);
  box-shadow: none;
}
.app-root.theme-light .select-box select {
  background: #ffffff;
  border: 1px solid rgba(8, 145, 178, 0.2);
  color: #1e293b;
  box-shadow: none;
}
.app-root.theme-light .select-box select:hover,
.app-root.theme-light .select-box select:focus {
  border-color: #0891b2;
  box-shadow: 0 0 0 2px rgba(8, 145, 178, 0.12);
}
.app-root.theme-light .panel-bullet {
  box-shadow: none;
}
.app-root.theme-light .panel-tag {
  background: rgba(8, 145, 178, 0.06);
  border-color: rgba(8, 145, 178, 0.15);
  color: #0891b2;
  box-shadow: none;
}
.app-root.theme-light .legend-dot {
  box-shadow: none;
}

.bg-decor { position: absolute; inset: 0; pointer-events: none; z-index: 0; overflow: hidden; }
.bg-decor::before {
  content: '';
  position: absolute; inset: 0;
  background-image:
    radial-gradient(circle, rgba(0, 212, 255, 0.6) 1px, transparent 1px),
    radial-gradient(circle, rgba(168, 85, 247, 0.5) 1px, transparent 1px),
    radial-gradient(circle, rgba(255, 255, 255, 0.4) 1px, transparent 1px);
  background-size: 280px 280px, 350px 350px, 420px 420px;
  background-position: 0 0, 140px 140px, 210px 210px;
  animation: particle-float 22s linear infinite;
  opacity: 0.55;
}
.bg-decor::after {
  content: '';
  position: absolute; inset: 0;
  background-image:
    radial-gradient(circle, rgba(0, 212, 255, 0.4) 1px, transparent 1px),
    radial-gradient(circle, rgba(168, 85, 247, 0.35) 1px, transparent 1px);
  background-size: 500px 500px, 600px 600px;
  background-position: 250px 250px, 100px 100px;
  animation: particle-float 35s linear infinite reverse;
  opacity: 0.4;
}
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
@keyframes particle-float {
  0% { transform: translateY(0) translateX(0); opacity: 0.4; }
  25% { transform: translateY(-80px) translateX(20px); opacity: 0.7; }
  50% { transform: translateY(-160px) translateX(-15px); opacity: 0.5; }
  75% { transform: translateY(-240px) translateX(10px); opacity: 0.6; }
  100% { transform: translateY(-320px) translateX(0); opacity: 0; }
}
@keyframes grid-drift { 0% { background-position: 0 0; } 100% { background-position: 60px 60px; } }
.bg-glow { position: absolute; border-radius: 50%; filter: blur(140px); animation: glow-float 20s ease-in-out infinite; mix-blend-mode: screen; }
.bg-glow-1 { width: 750px; height: 750px; background: #00d4ff; top: -200px; left: -150px; opacity: 0.25; box-shadow: 0 0 200px rgba(0, 212, 255, 0.4); }
.bg-glow-2 { width: 850px; height: 850px; background: #a855f7; bottom: -250px; right: -150px; opacity: 0.2; animation-delay: -7s; box-shadow: 0 0 200px rgba(168, 85, 247, 0.35); }
.bg-glow-3 { width: 600px; height: 600px; background: var(--accent); top: 35%; left: 45%; opacity: 0.1; animation-delay: -14s; box-shadow: 0 0 150px rgba(244, 114, 182, 0.2); }
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
  padding: 4px 24px;
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
  padding: 8px 32px 4px; flex-shrink: 0;
}
.header-deco { display: flex; align-items: center; gap: 8px; flex: 1; }
.header-deco-l { justify-content: flex-end; padding-right: 24px; }
.header-deco-r { padding-left: 24px; }
.hd-line { width: 100px; height: 1px; background: linear-gradient(90deg, transparent, var(--primary)); box-shadow: 0 0 6px var(--glow-soft); }
.hd-dot { width: 8px; height: 8px; background: var(--primary); border-radius: 50%; box-shadow: 0 0 12px var(--primary); animation: hd-dot-pulse 2s ease-in-out infinite; }
@keyframes hd-dot-pulse { 0%, 100% { opacity: 1; transform: scale(1); } 50% { opacity: 0.6; transform: scale(0.85); } }

.national-title {
  position: relative; display: inline-flex; align-items: center;
  font-size: 28px; font-weight: 700; letter-spacing: 6px;
  margin: 0; white-space: nowrap;
}
.title-bracket {
  color: var(--secondary);
  font-weight: 700;
  text-shadow: 0 0 16px var(--secondary-glow), 0 0 30px var(--glow-purple);
  font-size: 32px;
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
  filter: drop-shadow(0 0 20px var(--glow)) drop-shadow(0 0 40px var(--glow-soft));
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
  gap: 4px; padding: 4px 20px; flex-shrink: 0;
}
.tech-tab {
  position: relative;
  display: flex; align-items: center; gap: 8px;
  padding: 6px 18px;
  background: var(--bg-soft);
  border: 1px solid var(--border-soft);
  border-radius: 20px;
  color: var(--text-dim);
  font-size: 13px; font-weight: 500;
  font-family: inherit; cursor: pointer;
  transition: all 0.3s; overflow: hidden;
}
.tech-tab:hover { background: var(--primary-soft); color: var(--primary); border-color: var(--border); }
.tech-tab-icon { font-size: 14px; }
.tech-tab.active {
  background: var(--grad-main);
  border-color: transparent; color: #fff;
  box-shadow: 0 0 20px var(--glow), 0 0 30px var(--glow-purple);
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
.tech-tab-label { font-size: 13px; }

.national-body {
  position: relative; z-index: 3;
  flex: 1;
  display: grid;
  grid-template-columns: 18% 1fr 18%;
  gap: 8px;
  padding: 4px 16px 12px;
  min-height: 0;
  height: 0;
}
.left-col, .right-col, .map-col { display: flex; flex-direction: column; gap: 8px; min-height: 0; height: 100%; }
.map-col { gap: 6px; }
.map-col .map-area { flex: 2; min-height: 0; }
.map-col .map-legend { flex-shrink: 0; }

.tech-panel {
  position: relative;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 8px 10px;
  display: flex; flex-direction: column;
  box-shadow: 0 4px 28px rgba(0, 0, 0, 0.45), 0 0 0 1px rgba(0, 212, 255, 0.08), inset 0 0 25px rgba(0, 212, 255, 0.04);
  flex: 1;
  min-height: 0;
  overflow: hidden;
  transition: transform 0.3s, box-shadow 0.3s;
}
.tech-panel:hover {
  transform: translateY(-1px);
  box-shadow: 0 8px 40px rgba(0, 212, 255, 0.25), 0 0 0 1px rgba(0, 212, 255, 0.15), inset 0 0 30px rgba(0, 212, 255, 0.06);
}
.tech-panel::before {
  content: ''; position: absolute; top: -1px; left: -1px; right: -1px; height: 2px;
  background: linear-gradient(90deg, var(--primary) 0%, transparent 25%, transparent 75%, var(--secondary) 100%);
  background-size: 300% 100%;
  border-radius: 6px 6px 0 0;
  animation: border-shine 4s linear infinite, border-pulse 3s ease-in-out infinite;
  box-shadow: 0 0 12px var(--glow), 0 0 24px var(--glow-soft);
  z-index: 2;
}
@keyframes border-shine { 0% { background-position: -100% 0; } 100% { background-position: 200% 0; } }
@keyframes border-pulse { 0%, 100% { opacity: 0.85; } 50% { opacity: 1; } }
.tech-panel::after {
  content: ''; position: absolute; top: 0; left: 0; width: 16px; height: 16px;
  border-top: 2px solid var(--primary); border-left: 2px solid var(--primary);
  border-radius: 6px 0 0 0;
  box-shadow: 0 0 12px var(--glow), 0 0 20px var(--glow-soft);
}
.tech-panel > .corner-br {
  position: absolute; bottom: 0; right: 0; width: 16px; height: 16px;
  border-bottom: 2px solid var(--secondary); border-right: 2px solid var(--secondary);
  border-radius: 0 0 6px 0;
  box-shadow: 0 0 12px var(--glow-purple), 0 0 20px rgba(168, 85, 247, 0.2);
}
.tech-panel > .corner-tr {
  position: absolute; top: 0; right: 0; width: 16px; height: 16px;
  border-top: 2px solid var(--primary); border-right: 2px solid var(--primary);
  border-radius: 0 6px 0 0;
  box-shadow: 0 0 12px var(--glow), 0 0 20px var(--glow-soft);
}
.tech-panel > .corner-bl {
  position: absolute; bottom: 0; left: 0; width: 16px; height: 16px;
  border-bottom: 2px solid var(--secondary); border-left: 2px solid var(--secondary);
  border-radius: 0 0 0 6px;
  box-shadow: 0 0 12px var(--glow-purple), 0 0 20px rgba(168, 85, 247, 0.2);
}
.tech-panel-inner-border {
  position: absolute; inset: 1px; border-radius: 5px; pointer-events: none;
  border: 1px solid rgba(0, 212, 255, 0.08);
  box-shadow: inset 0 0 20px rgba(0, 212, 255, 0.03);
}

.tech-panel-header {
  display: flex; align-items: center; gap: 8px;
  margin-bottom: 8px; position: relative;
  padding-bottom: 6px;
}
.tech-panel-header::after {
  content: '';
  position: absolute;
  bottom: 0; left: 0;
  width: 100%; height: 1px;
  background: linear-gradient(90deg, var(--primary) 0%, var(--secondary) 50%, transparent 100%);
  background-size: 200% 100%;
  animation: header-line-flow 3s linear infinite;
  box-shadow: 0 0 6px var(--glow-soft);
}
@keyframes header-line-flow {
  0% { background-position: -100% 0; }
  100% { background-position: 100% 0; }
}
.panel-bullet {
  width: 10px; height: 10px;
  background: var(--grad-main);
  border-radius: 2px;
  box-shadow: 0 0 12px var(--glow), 0 0 24px var(--glow-soft);
  animation: bullet-pulse 2s ease-in-out infinite;
}
@keyframes bullet-pulse {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.75; transform: scale(0.9); }
}
.panel-title {
  font-size: 14px; font-weight: 600;
  color: var(--text);
  letter-spacing: 1px;
  text-shadow: 0 0 8px var(--glow-soft);
}
.panel-tag {
  margin-left: auto;
  font-size: 10px;
  padding: 3px 10px;
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

.filter-bar {
  display: flex; flex-direction: column; gap: 10px;
}
.filter-item {
  display: flex; align-items: center; gap: 10px;
  flex-wrap: wrap;
}
.filter-item label {
  font-size: 12px; color: var(--text-dim); flex-shrink: 0;
  letter-spacing: 0.5px;
}
.select-box {
  position: relative; display: inline-flex; align-items: center;
}
.select-box::before {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: 4px;
  padding: 1px;
  background: linear-gradient(135deg, rgba(0,212,255,0.6), rgba(168,85,247,0.6));
  -webkit-mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
  -webkit-mask-composite: xor;
          mask-composite: exclude;
  pointer-events: none;
  transition: opacity 0.3s;
  opacity: 0;
}
.select-box:hover::before { opacity: 1; }
.select-box select {
  appearance: none; -webkit-appearance: none;
  background: var(--bg-soft);
  border: 1px solid var(--border);
  border-radius: 4px;
  color: var(--text);
  font-size: 12px;
  padding: 5px 26px 5px 10px;
  font-family: inherit;
  cursor: pointer;
  outline: none;
  transition: all 0.3s;
  box-shadow: inset 0 0 12px rgba(0, 212, 255, 0.08);
}
.select-box select:hover, .select-box select:focus {
  border-color: var(--border-hover);
  box-shadow: 0 0 14px var(--glow-soft), inset 0 0 12px rgba(0, 212, 255, 0.12);
}
.select-box .arrow-down {
  position: absolute; right: 10px; top: 50%;
  transform: translateY(-50%);
  color: var(--primary); font-size: 8px;
  pointer-events: none;
  text-shadow: 0 0 4px var(--glow);
}
.btn-group { display: flex; gap: 8px; margin-left: auto; }
.btn-search, .btn-reset {
  display: inline-flex; align-items: center; gap: 4px;
  padding: 5px 12px;
  border: 1px solid var(--border);
  border-radius: 4px;
  background: var(--bg-soft);
  color: var(--text-dim);
  font-size: 12px;
  font-family: inherit;
  cursor: pointer;
  transition: all 0.3s;
}
.btn-search:hover, .btn-reset:hover {
  border-color: var(--border-hover);
  color: var(--primary);
  background: var(--primary-soft);
  box-shadow: 0 0 12px var(--glow-soft);
}
.btn-search {
  background: var(--grad-main);
  border-color: transparent;
  color: #fff;
  box-shadow: 0 0 18px var(--glow), 0 4px 14px rgba(0, 212, 255, 0.25);
  text-shadow: 0 0 6px rgba(255,255,255,0.4);
}
.btn-search:hover {
  filter: brightness(1.1);
  box-shadow: 0 0 25px var(--glow-strong), 0 6px 20px rgba(0, 212, 255, 0.35);
}
.btn-search .icon, .btn-reset .icon { font-size: 12px; }

.kpi-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
  height: 100%;
  min-height: 0;
  overflow: hidden;
}
.kpi-row-large {
  display: flex; gap: 10px;
}
.kpi-row-large > .kpi-card-lg { flex: 1; }
.kpi-card-lg {
  position: relative;
  background: linear-gradient(135deg, var(--bg-soft) 0%, rgba(0, 212, 255, 0.05) 50%, rgba(168, 85, 247, 0.03) 100%);
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 12px 14px;
  overflow: hidden;
  transition: all 0.3s;
  box-shadow: 0 0 20px rgba(0, 212, 255, 0.1), inset 0 0 20px rgba(0, 212, 255, 0.02);
}
.kpi-card-lg:hover {
  border-color: var(--border-hover);
  box-shadow: 0 6px 30px rgba(0, 212, 255, 0.25), inset 0 0 25px rgba(0, 212, 255, 0.04);
  transform: translateY(-2px);
}
.kpi-card-deco {
  position: absolute; top: 0; right: 0;
  width: 0; height: 0;
  border-style: solid;
}
.kpi-row-large > .kpi-card-lg:nth-child(1) .kpi-card-deco { border-width: 0 45px 45px 0; border-color: transparent var(--primary) transparent transparent; filter: drop-shadow(0 0 6px var(--glow)); }
.kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-card-deco { border-width: 0 45px 45px 0; border-color: transparent var(--secondary) transparent transparent; filter: drop-shadow(0 0 6px var(--glow-purple)); }
.kpi-card-hex {
  position: absolute; top: 10px; right: 10px;
  display: flex; gap: 3px;
}
.kpi-card-hex span {
  width: 7px; height: 7px;
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
  font-size: 12px; color: var(--text-dim);
  margin-bottom: 8px; letter-spacing: 0.5px;
}
.kpi-lg-value {
  font-size: 28px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  letter-spacing: 3px;
  margin-bottom: 6px;
  animation: value-glow-pulse 3s ease-in-out infinite;
}
@keyframes value-glow-pulse {
  0%, 100% { text-shadow: 0 0 14px var(--glow), 0 0 28px var(--glow-soft); }
  50% { text-shadow: 0 0 28px var(--glow), 0 0 50px var(--glow-soft), 0 0 70px rgba(0, 212, 255, 0.15); }
}
.kpi-lg-value .num { color: var(--primary); text-shadow: 0 0 22px var(--glow), 0 0 45px var(--glow-soft); }
.kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-lg-value .num { color: var(--secondary); text-shadow: 0 0 22px var(--glow-purple), 0 0 45px rgba(168, 85, 247, 0.25); }
.kpi-lg-value .unit { font-size: 16px; color: var(--text-dim); margin-left: 5px; font-weight: 400; }
.kpi-lg-trend {
  display: flex; align-items: center; gap: 4px;
  font-size: 12px;
}
.kpi-lg-trend.up { color: var(--success); text-shadow: 0 0 6px rgba(52, 211, 153, 0.3); }
.kpi-lg-trend.down { color: var(--text-dim); }
.kpi-lg-trend .trend-arrow { font-weight: 700; }

.kpi-row-gender {
  display: flex; gap: 10px;
  margin-top: 10px;
}
.gender-item {
  flex: 1;
  display: flex; align-items: center; gap: 10px;
  padding: 10px 14px;
  background: linear-gradient(135deg, var(--bg-soft) 0%, rgba(0, 212, 255, 0.02) 100%);
  border: 1px solid var(--border);
  border-radius: 6px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s;
}
.gender-item:hover { transform: translateX(3px); box-shadow: 0 4px 16px rgba(0, 212, 255, 0.12); }
.gender-item.male { border-left: 3px solid var(--primary); box-shadow: -4px 0 16px var(--glow-soft), inset 0 0 20px rgba(0, 212, 255, 0.03); }
.gender-item.female { border-left: 3px solid var(--secondary); box-shadow: -4px 0 16px var(--glow-purple), inset 0 0 20px rgba(168, 85, 247, 0.03); }
.gender-icon {
  width: 30px; height: 30px;
  border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  font-size: 17px; font-weight: 700;
  flex-shrink: 0;
}
.gender-item.male .gender-icon {
  background: var(--primary-soft);
  color: var(--primary);
  box-shadow: 0 0 12px var(--glow-soft);
}
.gender-item.female .gender-icon {
  background: var(--secondary-soft);
  color: var(--secondary);
  box-shadow: 0 0 12px var(--glow-purple);
}
.gender-info { flex: 1; min-width: 0; }
.gender-label {
  display: block;
  font-size: 11px; color: var(--text-dim);
  margin-bottom: 3px;
  letter-spacing: 0.5px;
}
.gender-value {
  font-size: 18px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  letter-spacing: 2px;
}
.gender-item.male .gender-value { color: var(--primary); text-shadow: 0 0 10px var(--glow); }
.gender-item.female .gender-value { color: var(--secondary); text-shadow: 0 0 10px var(--glow-purple); }
.gender-bar {
  position: absolute; left: 0; right: 0; bottom: 0;
  height: 3px;
  background: var(--border-soft);
  overflow: hidden;
}
.gender-bar-fill {
  height: 100%;
  transition: width 0.8s ease-out;
}
.gender-item.male .gender-bar-fill { background: var(--grad-main); box-shadow: 0 0 8px var(--glow); }
.gender-item.female .gender-bar-fill { background: var(--grad-purple); box-shadow: 0 0 8px var(--glow-purple); }

.kpi-row-small {
  display: flex; gap: 8px;
  margin-top: 10px;
}
.kpi-card-sm {
  flex: 1;
  padding: 10px 12px;
  background: linear-gradient(135deg, var(--bg-soft) 0%, rgba(244, 114, 182, 0.03) 50%, rgba(0, 212, 255, 0.02) 100%);
  border: 1px solid var(--border);
  border-radius: 6px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s;
  box-shadow: 0 0 15px rgba(244, 114, 182, 0.08);
}
.kpi-card-sm:hover { border-color: var(--border-hover); transform: translateY(-2px); box-shadow: 0 6px 20px rgba(244, 114, 182, 0.15); }
.kpi-sm-label {
  font-size: 11px; color: var(--text-dim);
  margin-bottom: 5px;
  letter-spacing: 0.5px;
}
.kpi-sm-value {
  font-size: 20px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  color: var(--accent);
  text-shadow: 0 0 12px var(--accent-glow);
  letter-spacing: 2px;
  animation: sm-value-pulse 3s ease-in-out infinite;
}
@keyframes sm-value-pulse {
  0%, 100% { text-shadow: 0 0 10px var(--accent-glow); }
  50% { text-shadow: 0 0 18px var(--accent-glow), 0 0 30px rgba(244, 114, 182, 0.15); }
}
.kpi-sm-bar {
  margin-top: 6px;
  height: 3px;
  background: var(--border-soft);
  border-radius: 2px;
  overflow: hidden;
}
.kpi-sm-bar-fill {
  height: 100%;
  background: var(--grad-pink);
  box-shadow: 0 0 8px var(--accent-glow);
  transition: width 0.8s ease-out;
}

.chart-area {
  width: 100%; height: 100%; min-height: 0;
  position: relative;
  background: radial-gradient(ellipse at center, rgba(0, 212, 255, 0.03) 0%, transparent 70%);
  border-radius: 4px;
  box-shadow: inset 0 0 30px rgba(0, 212, 255, 0.03);
}
.chart-area::before {
  content: '';
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(0, 212, 255, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.03) 1px, transparent 1px);
  background-size: 30px 30px;
  pointer-events: none;
  border-radius: 4px;
  opacity: 0.6;
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
  position: absolute; width: 28px; height: 28px;
}
.map-corner::before, .map-corner::after {
  content: ''; position: absolute;
  background: var(--primary);
  box-shadow: 0 0 16px var(--glow), 0 0 30px var(--glow-soft);
}
.mc-tl { top: 12px; left: 12px; }
.mc-tl::before { top: 0; left: 0; width: 24px; height: 2px; }
.mc-tl::after { top: 0; left: 0; width: 2px; height: 24px; }
.mc-tr { top: 12px; right: 12px; }
.mc-tr::before { top: 0; right: 0; width: 24px; height: 2px; }
.mc-tr::after { top: 0; right: 0; width: 2px; height: 24px; }
.mc-bl { bottom: 12px; left: 12px; }
.mc-bl::before { bottom: 0; left: 0; width: 24px; height: 2px; }
.mc-bl::after { bottom: 0; left: 0; width: 2px; height: 24px; }
.mc-br { bottom: 12px; right: 12px; }
.mc-br::before { bottom: 0; right: 0; width: 24px; height: 2px; }
.mc-br::after { bottom: 0; right: 0; width: 2px; height: 24px; }
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
    radial-gradient(ellipse at center, rgba(0, 212, 255, 0.05) 0%, transparent 70%),
    linear-gradient(135deg, var(--bg-card), var(--bg-deep));
  border: 1px solid var(--border);
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 0 60px rgba(0, 212, 255, 0.15), inset 0 0 60px rgba(0, 212, 255, 0.04);
  animation: map-glow-breathe 5s ease-in-out infinite;
  min-height: 0;
}
@keyframes map-glow-breathe {
  0%, 100% { box-shadow: 0 0 60px rgba(0, 212, 255, 0.15), inset 0 0 60px rgba(0, 212, 255, 0.04); }
  50% { box-shadow: 0 0 85px rgba(0, 212, 255, 0.28), inset 0 0 85px rgba(0, 212, 255, 0.07); }
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
  padding: 10px 14px;
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
  display: flex; gap: 14px;
}
.legend-item {
  display: flex; align-items: center; gap: 6px;
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
  display: flex; gap: 10px; height: 100%; min-height: 0;
}
.gauge-item {
  flex: 1; min-height: 0;
  background: radial-gradient(ellipse at center, rgba(0, 212, 255, 0.04) 0%, transparent 70%);
  border-radius: 6px;
  box-shadow: inset 0 0 20px rgba(0, 212, 255, 0.03);
}

.alert-list {
  display: flex; flex-direction: column; gap: 6px;
  max-height: 120px;
  overflow-y: auto;
  padding-right: 6px;
}
.alert-item {
  display: flex; align-items: center; gap: 8px;
  padding: 8px 12px;
  background: linear-gradient(135deg, var(--bg-soft) 0%, rgba(0, 212, 255, 0.02) 100%);
  border: 1px solid var(--border);
  border-radius: 6px;
  transition: all 0.3s;
  cursor: pointer;
  animation: alert-slide-in 0.4s ease-out;
}
@keyframes alert-slide-in {
  from { opacity: 0; transform: translateX(-10px); }
  to { opacity: 1; transform: translateX(0); }
}
.alert-item:hover {
  background: var(--bg-card);
  transform: translateX(3px);
  box-shadow: 0 4px 16px rgba(0, 212, 255, 0.12);
}
.alert-item.danger { border-left: 3px solid #ef4444; box-shadow: -4px 0 14px rgba(239, 68, 68, 0.25), inset 0 0 16px rgba(239, 68, 68, 0.05); }
.alert-item.warning { border-left: 3px solid var(--warning); box-shadow: -4px 0 14px rgba(251, 191, 36, 0.25), inset 0 0 16px rgba(251, 191, 36, 0.05); }
.alert-item.info { border-left: 3px solid var(--primary); box-shadow: -4px 0 14px var(--glow-soft), inset 0 0 16px rgba(0, 212, 255, 0.05); }
.alert-icon { font-size: 14px; flex-shrink: 0; }
.alert-msg {
  font-size: 11px; color: var(--text);
  line-height: 1.4;
}

::-webkit-scrollbar { width: 6px; height: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: var(--border); border-radius: 3px; }
::-webkit-scrollbar-thumb:hover { background: var(--primary); }

@media (max-width: 1500px) {
  .national-body {
    grid-template-columns: 24% 1fr 24%;
  }
  .national-title { font-size: 28px; }
}

@media (max-width: 1400px) {
  .national-body {
    grid-template-columns: 24% 1fr 24%;
  }
  .national-title { font-size: 26px; }
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
  .map-area { min-height: 240px; }
  .map-corner { display: none; }
}
</style>
