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
        <span class="title-text">{{ currentProvince }}中小学生健康监测大屏</span>
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
                <label>地区：</label>
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
                  <div class="kpi-lg-label">{{ currentProvince }}{{ currentMetric.name }}率</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ currentProvinceData.rate }}</span><span class="unit">%</span>
                  </div>
                  <div :class="['kpi-lg-trend', currentProvinceData.trend > 0 ? 'up' : 'down']">
                    <span class="trend-arrow">{{ currentProvinceData.trend > 0 ? '▲' : '▼' }}</span>
                    {{ Math.abs(currentProvinceData.trend) }}% 同比
                  </div>
                </div>
                <div class="kpi-card-lg">
                  <div class="kpi-card-deco"></div>
                  <div class="kpi-card-hex"><span></span><span></span><span></span></div>
                  <div class="kpi-lg-label">学生总数（万）</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ currentProvinceData.students }}</span>
                  </div>
                  <div class="kpi-lg-trend down">覆盖 {{ currentProvinceData.cities }} 地市</div>
                </div>
              </div>
              <div class="kpi-row-gender">
                <div class="gender-item male">
                  <span class="gender-icon">♂</span>
                  <div class="gender-info">
                    <span class="gender-label">男生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ currentProvinceData.maleRate }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: currentProvinceData.maleRate + '%' }"></div></div>
                </div>
                <div class="gender-item female">
                  <span class="gender-icon">♀</span>
                  <div class="gender-info">
                    <span class="gender-label">女生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ currentProvinceData.femaleRate }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: currentProvinceData.femaleRate + '%' }"></div></div>
                </div>
              </div>
              <div class="kpi-row-small">
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">小学{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentProvinceData.primary }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentProvinceData.primary + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">初中{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentProvinceData.junior }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentProvinceData.junior + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">高中{{ currentMetric.name }}率</div>
                  <div class="kpi-sm-value">{{ currentProvinceData.senior }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: currentProvinceData.senior + '%' }"></div></div>
                </div>
              </div>
            </div>
          </div>
        </section>
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">地市{{ currentMetric.name }}率排名</span>
            <span class="panel-tag">TOP {{ cityList.length > 0 ? Math.min(10, cityList.length) : 10 }}</span>
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
                <span class="alert-msg">{{ currentProvinceData.topCity }}{{ currentMetric.name }}率达{{ currentProvinceData.topRate }}%，高于全省平均水平</span>
              </div>
              <div class="alert-item warning">
                <span class="alert-icon">🟡</span>
                <span class="alert-msg">3个区县{{ currentMetric.name }}监测覆盖率低于40%，需加强推进</span>
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
              <span>经度：<b>{{ currentProvinceCenter[0].toFixed(1) }}°E</b></span>
              <span>纬度：<b>{{ currentProvinceCenter[1].toFixed(1) }}°N</b></span>
              <span>缩放：<b>{{ mapZoom.toFixed(2) }}x</b></span>
              <span v-if="mapLoaded" class="map-drill-tip">{{ currentProvince }}地图</span>
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
  primary: '#38bdf8',
  secondary: '#a78bfa',
  accent: '#f472b6',
  warning: '#fbbf24',
  success: '#34d399',
  danger: '#fb7185',
  male: '#38bdf8',
  female: '#a78bfa',
  text: '#e2e8f0',
  textDim: '#94a3b8',
  bg: 'rgba(10, 22, 50, 0.96)',
  mapArea: 'rgba(15, 30, 65, 0.85)',
  mapBorder: 'rgba(56, 189, 248, 0.75)',
  mapLabel: '#e2e8f0',
  splitLine: 'rgba(56, 189, 248, 0.12)',
  gaugeTrack: 'rgba(56, 189, 248, 0.12)',
  initialBar: '#475569',
  mapColors: ['#0891b2', '#38bdf8', '#a78bfa', '#f472b6'],
};

const LIGHT_COLOR = {
  primary: '#0891b2',
  secondary: '#7c3aed',
  accent: '#db2777',
  warning: '#d97706',
  success: '#059669',
  danger: '#dc2626',
  male: '#0891b2',
  female: '#7c3aed',
  text: '#1e293b',
  textDim: '#64748b',
  bg: 'rgba(255, 255, 255, 0.98)',
  mapArea: '#f1f5f9',
  mapBorder: 'rgba(8, 145, 178, 0.35)',
  mapLabel: '#1e293b',
  splitLine: 'rgba(8, 145, 178, 0.1)',
  gaugeTrack: 'rgba(8, 145, 178, 0.08)',
  initialBar: '#cbd5e1',
  mapColors: ['#67e8f9', '#0891b2', '#7c3aed', '#db2777'],
};

const getColors = () => (isLight.value ? LIGHT_COLOR : DARK_COLOR);

const makeGrad = (c1, c2, horizontal = true) =>
  new echarts.graphic.LinearGradient(0, 0, horizontal ? 1 : 0, horizontal ? 0 : 1, [
    { offset: 0, color: c1 },
    { offset: 1, color: c2 },
  ]);

const PROVINCE_MAP_CONFIG = {
  '110000': { name: '北京市', short: '北京', center: [116.4, 39.9], zoom: 1.6 },
  '120000': { name: '天津市', short: '天津', center: [117.2, 39.1], zoom: 1.8 },
  '130000': { name: '河北省', short: '河北', center: [114.5, 38.0], zoom: 1.8 },
  '140000': { name: '山西省', short: '山西', center: [112.5, 37.8], zoom: 1.8 },
  '150000': { name: '内蒙古自治区', short: '内蒙古', center: [111.7, 40.8], zoom: 1.4 },
  '210000': { name: '辽宁省', short: '辽宁', center: [123.4, 41.8], zoom: 1.7 },
  '220000': { name: '吉林省', short: '吉林', center: [125.3, 43.9], zoom: 1.6 },
  '230000': { name: '黑龙江省', short: '黑龙江', center: [126.6, 45.7], zoom: 1.5 },
  '310000': { name: '上海市', short: '上海', center: [121.5, 31.2], zoom: 2.0 },
  '320000': { name: '江苏省', short: '江苏', center: [118.8, 32.0], zoom: 1.8 },
  '330000': { name: '浙江省', short: '浙江', center: [120.2, 29.2], zoom: 1.8 },
  '340000': { name: '安徽省', short: '安徽', center: [117.3, 31.8], zoom: 1.8 },
  '350000': { name: '福建省', short: '福建', center: [119.3, 26.1], zoom: 1.8 },
  '360000': { name: '江西省', short: '江西', center: [115.9, 28.7], zoom: 1.8 },
  '370000': { name: '山东省', short: '山东', center: [118.0, 36.5], zoom: 1.8 },
  '410000': { name: '河南省', short: '河南', center: [113.6, 34.8], zoom: 1.8 },
  '420000': { name: '湖北省', short: '湖北', center: [112.3, 31.0], zoom: 1.8 },
  '430000': { name: '湖南省', short: '湖南', center: [112.0, 27.6], zoom: 1.8 },
  '440000': { name: '广东省', short: '广东', center: [113.5, 23.8], zoom: 1.8 },
  '450000': { name: '广西壮族自治区', short: '广西', center: [108.3, 22.8], zoom: 1.7 },
  '460000': { name: '海南省', short: '海南', center: [110.3, 20.0], zoom: 1.8 },
  '500000': { name: '重庆市', short: '重庆', center: [106.5, 29.5], zoom: 1.9 },
  '510000': { name: '四川省', short: '四川', center: [104.0, 30.6], zoom: 1.6 },
  '520000': { name: '贵州省', short: '贵州', center: [106.7, 26.6], zoom: 1.8 },
  '530000': { name: '云南省', short: '云南', center: [102.7, 25.0], zoom: 1.7 },
  '540000': { name: '西藏自治区', short: '西藏', center: [91.1, 29.7], zoom: 1.2 },
  '610000': { name: '陕西省', short: '陕西', center: [108.9, 34.3], zoom: 1.8 },
  '620000': { name: '甘肃省', short: '甘肃', center: [103.8, 36.1], zoom: 1.4 },
  '630000': { name: '青海省', short: '青海', center: [101.8, 36.6], zoom: 1.4 },
  '640000': { name: '宁夏回族自治区', short: '宁夏', center: [106.3, 38.5], zoom: 1.7 },
  '650000': { name: '新疆维吾尔自治区', short: '新疆', center: [87.6, 43.8], zoom: 1.2 },
};

const CITY_CODES_MAP = {
  '110000': { '东城区': '110101', '西城区': '110102', '朝阳区': '110105', '海淀区': '110108', '丰台区': '110106', '石景山区': '110107', '通州区': '110112', '昌平区': '110114', '顺义区': '110113', '大兴区': '110115', '和平区': '110116', '东城区': '110101' },
  '120000': { '和平区': '120101', '河东区': '120102', '河西区': '120103', '南开区': '120104', '河北区': '120105', '红桥区': '120106', '东丽区': '120110', '西青区': '120111', '津南区': '120112', '北辰区': '120113' },
  '130000': { '石家庄市': '130100', '唐山市': '130200', '秦皇岛市': '130300', '邯郸市': '130400', '邢台市': '130500', '保定市': '130600', '张家口市': '130700', '承德市': '130800', '沧州市': '130900', '廊坊市': '131000', '衡水市': '131100' },
  '140000': { '太原市': '140100', '大同市': '140200', '阳泉市': '140300', '长治市': '140400', '晋城市': '140500', '朔州市': '140600', '晋中市': '140700', '运城市': '140800', '忻州市': '140900', '临汾市': '141000', '吕梁市': '141100' },
  '150000': { '呼和浩特市': '150100', '包头市': '150200', '乌海市': '150300', '赤峰市': '150400', '通辽市': '150500', '鄂尔多斯市': '150600', '呼伦贝尔市': '150700', '巴彦淖尔市': '150800', '乌兰察布市': '150900', '兴安盟': '152200' },
  '210000': { '沈阳市': '210100', '大连市': '210200', '鞍山市': '210300', '抚顺市': '210400', '本溪市': '210500', '丹东市': '210600', '锦州市': '210700', '营口市': '210800', '阜新市': '210900', '辽阳市': '211000', '盘锦市': '211100', '铁岭市': '211200', '朝阳市': '211300', '葫芦岛市': '211400' },
  '220000': { '长春市': '220100', '吉林市': '220200', '四平市': '220300', '辽源市': '220400', '通化市': '220500', '白山市': '220600', '松原市': '220700', '白城市': '220800' },
  '230000': { '哈尔滨市': '230100', '齐齐哈尔市': '230200', '鸡西市': '230300', '鹤岗市': '230400', '双鸭山市': '230500', '大庆市': '230600', '伊春市': '230700', '佳木斯市': '230800', '七台河市': '230900', '牡丹江市': '231000' },
  '310000': { '黄浦区': '310101', '徐汇区': '310104', '长宁区': '310105', '静安区': '310106', '普陀区': '310107', '虹口区': '310109', '杨浦区': '310110', '浦东新区': '310115', '闵行区': '310112', '宝山区': '310113', '嘉定区': '310114', '金山区': '310116', '松江区': '310117', '青浦区': '310118', '奉贤区': '310120', '崇明区': '310151' },
  '320000': { '南京市': '320100', '无锡市': '320200', '徐州市': '320300', '常州市': '320400', '苏州市': '320500', '南通市': '320600', '连云港市': '320700', '淮安市': '320800', '盐城市': '320900', '扬州市': '321000', '镇江市': '321100', '泰州市': '321200', '宿迁市': '321300' },
  '330000': { '杭州市': '330100', '宁波市': '330200', '温州市': '330300', '嘉兴市': '330400', '湖州市': '330500', '绍兴市': '330600', '金华市': '330700', '衢州市': '330800', '舟山市': '330900', '台州市': '331000', '丽水市': '331100' },
  '340000': { '合肥市': '340100', '芜湖市': '340200', '蚌埠市': '340300', '淮南市': '340400', '马鞍山市': '340500', '淮北市': '340600', '铜陵市': '340700', '安庆市': '340800', '黄山市': '341000', '滁州市': '341100', '阜阳市': '341200', '宿州市': '341300', '六安市': '341500' },
  '350000': { '福州市': '350100', '厦门市': '350200', '莆田市': '350300', '三明市': '350400', '泉州市': '350500', '漳州市': '350600', '南平市': '350700', '龙岩市': '350800', '宁德市': '350900' },
  '360000': { '南昌市': '360100', '景德镇市': '360200', '萍乡市': '360300', '九江市': '360400', '新余市': '360500', '鹰潭市': '360600', '赣州市': '360700', '吉安市': '360800', '宜春市': '360900', '抚州市': '361000', '上饶市': '361100' },
  '370000': { '济南市': '370100', '青岛市': '370200', '淄博市': '370300', '枣庄市': '370400', '东营市': '370500', '烟台市': '370600', '潍坊市': '370700', '济宁市': '370800', '泰安市': '370900', '威海市': '371000', '日照市': '371100', '临沂市': '371300', '德州市': '371400', '聊城市': '371500', '滨州市': '371600', '菏泽市': '371700' },
  '410000': { '郑州市': '410100', '开封市': '410200', '洛阳市': '410300', '平顶山市': '410400', '安阳市': '410500', '鹤壁市': '410600', '新乡市': '410700', '焦作市': '410800', '濮阳市': '410900', '许昌市': '411000', '漯河市': '411100', '三门峡市': '411200', '南阳市': '411300', '商丘市': '411400', '信阳市': '411500', '周口市': '411600', '驻马店市': '411700' },
  '420000': { '武汉市': '420100', '黄石市': '420200', '十堰市': '420300', '宜昌市': '420500', '襄阳市': '420600', '鄂州市': '420700', '荆门市': '420800', '孝感市': '420900', '荆州市': '421000', '黄冈市': '421100', '咸宁市': '421200', '随州市': '421300' },
  '430000': { '长沙市': '430100', '株洲市': '430200', '湘潭市': '430300', '衡阳市': '430400', '邵阳市': '430500', '岳阳市': '430600', '常德市': '430700', '张家界市': '430800', '益阳市': '430900', '郴州市': '431000', '永州市': '431100', '怀化市': '431200', '娄底市': '431300' },
  '440000': { '广州市': '440100', '深圳市': '440300', '珠海市': '440400', '汕头市': '440500', '佛山市': '440600', '韶关市': '440200', '湛江市': '440800', '肇庆市': '441200', '江门市': '440700', '茂名市': '440900', '惠州市': '441300', '梅州市': '441400', '汕尾市': '441500', '河源市': '441600', '阳江市': '441700', '清远市': '441800', '东莞市': '441900', '中山市': '442000', '潮州市': '445100', '揭阳市': '445200' },
  '450000': { '南宁市': '450100', '柳州市': '450200', '桂林市': '450300', '梧州市': '450400', '北海市': '450500', '防城港市': '450600', '钦州市': '450700', '贵港市': '450800', '玉林市': '450900', '百色市': '451000', '贺州市': '451100', '河池市': '451200', '来宾市': '451300', '崇左市': '451400' },
  '460000': { '海口市': '460100', '三亚市': '460200', '三沙市': '460300', '儋州市': '460400' },
  '500000': { '渝中区': '500103', '江北区': '500105', '沙坪坝区': '500106', '九龙坡区': '500107', '南岸区': '500108', '北碚区': '500109', '万州区': '500101', '涪陵区': '500102', '渝北区': '500112', '巴南区': '500113' },
  '510000': { '成都市': '510100', '自贡市': '510300', '攀枝花市': '510400', '泸州市': '510500', '德阳市': '510600', '绵阳市': '510700', '广元市': '510800', '遂宁市': '510900', '内江市': '511000', '乐山市': '511100', '南充市': '511300', '眉山市': '511400' },
  '520000': { '贵阳市': '520100', '六盘水市': '520200', '遵义市': '520300', '安顺市': '520400', '毕节市': '520500', '铜仁市': '520600' },
  '530000': { '昆明市': '530100', '曲靖市': '530300', '玉溪市': '530400', '保山市': '530500', '昭通市': '530600', '丽江市': '530700', '普洱市': '530800', '临沧市': '530900' },
  '540000': { '拉萨市': '540100', '日喀则市': '540200', '昌都市': '540300', '林芝市': '540400', '山南市': '540500', '那曲市': '540600' },
  '610000': { '西安市': '610100', '铜川市': '610200', '宝鸡市': '610300', '咸阳市': '610400', '渭南市': '610500', '延安市': '610600', '汉中市': '610700', '榆林市': '610800', '安康市': '610900', '商洛市': '611000' },
  '620000': { '兰州市': '620100', '嘉峪关市': '620200', '金昌市': '620300', '白银市': '620400', '天水市': '620500', '武威市': '620600', '张掖市': '620700', '平凉市': '620800', '酒泉市': '620900', '庆阳市': '621000', '定西市': '621100', '陇南市': '621200' },
  '630000': { '西宁市': '630100', '海东市': '630200' },
  '640000': { '银川市': '640100', '石嘴山市': '640200', '吴忠市': '640300', '固原市': '640400', '中卫市': '640500' },
  '650000': { '乌鲁木齐市': '650100', '克拉玛依市': '650200', '吐鲁番市': '650400', '哈密市': '650500' },
};

const currentCityCodes = computed(() => {
  return CITY_CODES_MAP[routeCode.value] || {};
});

const drillToCity = (name) => {
  const codes = currentCityCodes.value;
  let code = codes[name];
  if (!code) {
    const suffixes = ['市', '区', '县', '盟', '自治州', '地区'];
    for (const s of suffixes) {
      const stripped = name.replace(s, '');
      if (codes[stripped]) { code = codes[stripped]; break; }
    }
  }
  if (code) router.push({ path: `/vision/city/${code}`, query: { tab: activeTab.value } });
  else console.warn('未找到城市编码:', name);
};

const activeTab = ref('vision');
const isLight = ref(false);
const currentDate = ref('');
const currentTime = ref('');
const mapLoaded = ref(false);
const mapZoom = ref(1.8);
let timer = null;
let themeObserver = null;

const routeCode = computed(() => String(route.params.code || '320000'));
const currentProvinceConfig = computed(() => {
  return PROVINCE_MAP_CONFIG[routeCode.value] || PROVINCE_MAP_CONFIG['320000'];
});
const currentProvince = computed(() => currentProvinceConfig.value.short);
const currentProvinceCenter = computed(() => currentProvinceConfig.value.center);

const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️' },
  { key: 'oral', label: '口腔健康', icon: '🦷' },
  { key: 'mental', label: '心理健康', icon: '🧠' },
  { key: 'weight', label: '健康体重', icon: '⚖️' },
  { key: 'bone', label: '骨骼健康', icon: '🦴' }
];

const metricConfig = {
  vision: { name: '近视', baseRate: 58, trend: 2.5, maleFactor: 0.95, femaleFactor: 1.05, primaryFactor: 0.6, juniorFactor: 1.0, seniorFactor: 1.25 },
  oral: { name: '龋齿', baseRate: 42, trend: -1.2, maleFactor: 0.96, femaleFactor: 1.05, primaryFactor: 1.24, juniorFactor: 0.9, seniorFactor: 0.67 },
  mental: { name: '心理预警', baseRate: 18, trend: -0.8, maleFactor: 0.92, femaleFactor: 1.1, primaryFactor: 0.67, juniorFactor: 1.22, seniorFactor: 1.56 },
  weight: { name: '超重/肥胖', baseRate: 24, trend: 1.5, maleFactor: 1.12, femaleFactor: 0.88, primaryFactor: 0.63, juniorFactor: 1.17, seniorFactor: 1.33 },
  bone: { name: '骨密度偏低', baseRate: 16, trend: -0.5, maleFactor: 0.91, femaleFactor: 1.11, primaryFactor: 0.75, juniorFactor: 1.13, seniorFactor: 1.38 }
};

const currentMetric = computed(() => metricConfig[activeTab.value]);

const cityList = ref([]);

const districtList = computed(() => cityList.value.slice(0, 6));

const currentFilter = reactive({
  year: '2026',
  district: ''
});

const computeProvinceMetricData = () => {
  const m = metricConfig[activeTab.value];
  const cfg = currentProvinceConfig.value;
  const code = routeCode.value;
  const seed = parseInt(code.slice(-2), 10) || 13;
  const rate = Math.round((m.baseRate + (seed % 15 - 7)) * 10) / 10;
  const students = Math.round(800 + seed * 3 + cfg.center[0] % 50);
  const cities = cityList.value.length || 11;
  const maleRate = Math.round(rate * m.maleFactor * 10) / 10;
  const femaleRate = Math.round(rate * m.femaleFactor * 10) / 10;
  const primary = Math.round(rate * m.primaryFactor * 10) / 10;
  const junior = Math.round(rate * m.juniorFactor * 10) / 10;
  const senior = Math.round(rate * m.seniorFactor * 10) / 10;
  const topCity = cityList.value[0] || '最高';
  const topRate = Math.round((rate + 5 + (seed % 8)) * 10) / 10;
  return { rate, students, cities, trend: m.trend, maleRate, femaleRate, primary, junior, senior, topCity, topRate };
};

const currentProvinceData = computed(() => computeProvinceMetricData());

const rankingChartRef = ref(null);
const ageGenderChartRef = ref(null);
const urbanGaugeRef = ref(null);
const ruralGaugeRef = ref(null);
const trendChartRef = ref(null);
const interventionChartRef = ref(null);
const mapRef = ref(null);

const chartInstances = {};

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

const handleSearch = () => {
  renderAllCharts();
};

const handleReset = () => {
  currentFilter.year = '2026';
  currentFilter.district = '';
};

const goBack = () => {
  router.push({ path: '/vision/national', query: { tab: activeTab.value } });
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

const getProvinceMapData = () => {
  const cities = cityList.value;
  if (cities.length === 0) return [];
  const baseRate = currentProvinceData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  return cities.map((name, i) => ({
    name: name,
    value: Math.round((baseRate + ((seed + i * 7) % 20 - 10)) * 10) / 10
  }));
};

const getMapOption = () => {
  if (!mapLoaded.value) return {};
  const c = getColors();
  const data = getProvinceMapData();
  if (data.length === 0) return {};
  const values = data.map(d => d.value);
  const minV = Math.min(...values);
  const maxV = Math.max(...values);
  const cfg = currentProvinceConfig.value;

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('item'),
      formatter: (p) => {
        const canDrill = !!currentCityCodes.value[p.name];
        return `<div style="font-weight:600">${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value || 0}%</span></div>${canDrill ? '<div style="font-size:11px;color:var(--text-dim);margin-top:4px">👆 点击查看市级详情</div>' : ''}`;
      },
    },
    visualMap: { show: false, min: minV, max: maxV, inRange: { color: c.mapColors } },
    series: [{
      type: 'map',
      map: 'province',
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
          borderColor: isLight.value ? 'rgba(8,145,178,0.3)' : c.primary, borderWidth: 2, shadowBlur: 20, shadowColor: `${c.primary}aa`,
        },
      },
      data,
    }],
  };
};

const getRankingOption = () => {
  const c = getColors();
  const cities = cityList.value.length ? cityList.value.slice(0, 10) : ['暂无数据'];
  const baseRate = currentProvinceData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rates = cities.map((_, i) => Math.round((baseRate + (15 - i * 1.5) + ((seed + i * 3) % 6 - 3)) * 10) / 10);
  const maxRate = Math.max(...rates);

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        const p = params[0];
        return `<div style="font-weight:600">${p.name}</div><div>${currentMetric.value.name}率：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div><div style="font-size:11px;color:var(--text-dim);margin-top:4px">👆 点击查看市级详情</div>`;
      },
    },
    grid: { left: 10, right: 50, top: 5, bottom: 5, containLabel: true },
    xAxis: { type: 'value', show: false, max: maxRate + 5 },
    yAxis: {
      type: 'category',
      data: cities,
      inverse: true,
      axisLine: { show: false },
      axisTick: { show: false },
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
  const d = {
    grades,
    male: grades.map((_, i) => Math.round(25 + i * 8 + (Math.random() * 4 - 2))),
    female: grades.map((_, i) => Math.round(22 + i * 9 + (Math.random() * 4 - 2)))
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
        itemStyle: { color: makeGrad(c.secondary, isLight.value ? '#c4b5fd' : '#7c3aed'), borderRadius: [0, 4, 4, 0] },
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

const getTrendOption = () => {
  const c = getColors();
  const months = ['1月', '2月', '3月', '4月', '5月', '6月'];
  const baseRate = currentProvinceData.value.rate;
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const rateData = months.map((_, i) => Math.round((baseRate + i * 0.8 + ((seed + i * 3) % 4 - 2)) * 10) / 10);
  const targetData = months.map((_, i) => Math.round((baseRate + 2 - i * 0.3) * 10) / 10);

  return {
    backgroundColor: 'transparent',
    tooltip: { ...getTooltip('axis') },
    legend: { show: false },
    grid: { left: 40, right: 20, top: 10, bottom: 30 },
    xAxis: {
      type: 'category',
      data: months,
      axisLine: { lineStyle: { color: c.splitLine } },
      axisTick: { show: false },
      axisLabel: { color: c.textDim, fontSize: 11 },
    },
    yAxis: {
      type: 'value',
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { color: c.textDim, fontSize: 11 },
      splitLine: { lineStyle: { color: c.splitLine } },
    },
    series: [
      {
        name: '实际' + currentMetric.value.name + '率',
        type: 'line',
        data: rateData,
        smooth: true,
        symbol: 'circle',
        symbolSize: 8,
        lineStyle: { width: 3, color: c.primary, shadowBlur: 8, shadowColor: `${c.primary}55` },
        itemStyle: { color: c.primary, borderColor: c.bg, borderWidth: 2 },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: `${c.primary}55` },
            { offset: 1, color: `${c.primary}00` },
          ]),
        },
      },
      {
        name: '目标线',
        type: 'line',
        data: targetData,
        smooth: true,
        symbol: 'none',
        lineStyle: { width: 2, color: c.secondary, type: 'dashed' },
      },
    ],
  };
};

const getInterventionOption = () => {
  const c = getColors();
  const seed = parseInt(routeCode.value.slice(-2), 10) || 13;
  const base = currentProvinceData.value.rate;
  const data = [
    { name: '示范区A', initial: Math.round((base + 8 + (seed % 5)) * 10) / 10, final: Math.round((base + 1 - (seed % 3)) * 10) / 10, change: -Math.round((7 + (seed % 3)) * 10) / 10 },
    { name: '示范区B', initial: Math.round((base + 12 + (seed % 4)) * 10) / 10, final: Math.round((base + 3 - (seed % 4)) * 10) / 10, change: -Math.round((9 + (seed % 2)) * 10) / 10 },
    { name: '示范区C', initial: Math.round((base + 5 + (seed % 3)) * 10) / 10, final: Math.round((base + 1 - (seed % 2)) * 10) / 10, change: -Math.round((5 + (seed % 4)) * 10) / 10 },
    { name: '示范区D', initial: Math.round((base + 10 + (seed % 2)) * 10) / 10, final: Math.round((base + 2 - (seed % 3)) * 10) / 10, change: -Math.round((8 + (seed % 2)) * 10) / 10 },
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
        params.forEach((p) => {
          html += `<div>${p.marker}${p.seriesName}：<span style="color:${c.primary};font-weight:bold">${p.value}%</span></div>`;
        });
        return html;
      },
    },
    legend: { show: false },
    grid: { left: 80, right: 60, top: 8, bottom: 8 },
    xAxis: { type: 'value', show: false, max: Math.ceil(Math.max(...initials) * 1.2) },
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

const renderAllCharts = () => {
  disposeAll();
  requestAnimationFrame(() => {
    nextTick(() => {
      const d = currentProvinceData.value;
      const ur = { urban: Math.round(d.rate * 0.9 * 10) / 10, rural: Math.round(d.rate * 1.2 * 10) / 10 };
      initChart(rankingChartRef, 'ranking', getRankingOption(), (params) => {
        if (params?.name) drillToCity(params.name);
      });
      initChart(ageGenderChartRef, 'ageGender', getAgeGenderOption());
      initChart(urbanGaugeRef, 'urbanGauge', getGaugeOption(ur.urban, `城区${currentMetric.value.name}率`, getColors().primary));
      initChart(ruralGaugeRef, 'ruralGauge', getGaugeOption(ur.rural, `县乡${currentMetric.value.name}率`, getColors().secondary));
      initChart(trendChartRef, 'trend', getTrendOption());
      initChart(interventionChartRef, 'intervention', getInterventionOption());
      if (mapLoaded.value) {
        initChart(mapRef, 'map', getMapOption(), (params) => {
          if (params?.name) drillToCity(params.name);
        });
      }
    });
  });
};

const loadMap = async () => {
  const code = routeCode.value;
  const cfg = currentProvinceConfig.value;
  const urls = [
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}_full.json`,
    `https://fastly.jsdelivr.net/npm/echarts@4.9.0/map/json/province/${cfg.short}.json`,
    `https://geo.datav.aliyun.com/areas_v3/bound/${code}.json`,
  ];
  for (const url of urls) {
    try {
      const res = await fetch(url);
      if (!res.ok) continue;
      const geoJson = await res.json();
      echarts.registerMap('province', geoJson);
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
        if (list.length >= 3) {
          cityList.value = list.slice(0, 30);
        } else {
          cityList.value = [];
        }
      } else {
        cityList.value = [];
      }
      mapLoaded.value = true;
      mapZoom.value = cfg.zoom;
      setTimeout(() => renderAllCharts(), 200);
      return;
    } catch (e) {
      console.warn('地图加载失败', url, e);
    }
  }
  cityList.value = [];
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
  cityList.value = [];
  setTimeout(() => loadMap(), 100);
});
</script>

<style scoped>
.app-root {
  position: relative;
  padding: 0;
  background: var(--bg);
  color: var(--text);
  font-family: -apple-system, BlinkMacSystemFont, 'PingFang SC', 'Microsoft YaHei', sans-serif;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  --primary: #00d4ff;
  --primary-soft: rgba(0, 212, 255, 0.15);
  --primary-glow: rgba(0, 212, 255, 0.55);
  --secondary: #a855f7;
  --secondary-soft: rgba(168, 85, 247, 0.15);
  --secondary-glow: rgba(168, 85, 247, 0.5);
  --accent: #f472b6;
  --accent-glow: rgba(244, 114, 182, 0.45);
  --warning: #fbbf24;
  --success: #34d399;
  --danger: #fb7185;

  --bg: #030818;
  --bg-deep: #020614;
  --bg-card: rgba(8, 18, 44, 0.88);
  --bg-card-hover: rgba(12, 26, 58, 0.95);
  --bg-soft: rgba(0, 212, 255, 0.05);

  --border: rgba(0, 212, 255, 0.28);
  --border-hover: rgba(0, 212, 255, 0.5);
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

  --shadow-card: 0 4px 28px rgba(0, 0, 0, 0.5), 0 0 0 1px rgba(0, 212, 255, 0.08);
  --shadow-hover: 0 8px 36px rgba(0, 212, 255, 0.2);
  --shadow-purple: 0 8px 28px rgba(168, 85, 247, 0.18);

  --grad-main: linear-gradient(135deg, #00d4ff 0%, #a855f7 100%);
  --grad-cyan: linear-gradient(135deg, #00d4ff 0%, #0891b2 100%);
  --grad-purple: linear-gradient(135deg, #a855f7 0%, #7c3aed 100%);
  --grad-pink: linear-gradient(135deg, #f472b6 0%, #db2777 100%);
}

.app-root.theme-light {
  --primary: #0891b2;
  --primary-soft: rgba(8, 145, 178, 0.1);
  --primary-glow: rgba(8, 145, 178, 0.3);
  --secondary: #7c3aed;
  --secondary-soft: rgba(124, 58, 237, 0.1);
  --secondary-glow: rgba(124, 58, 237, 0.25);
  --accent: #db2777;
  --accent-glow: rgba(219, 39, 119, 0.25);
  --bg: #f8fafc;
  --bg-deep: #f1f5f9;
  --bg-card: rgba(255, 255, 255, 0.98);
  --bg-card-hover: rgba(255, 255, 255, 1);
  --bg-soft: rgba(8, 145, 178, 0.04);
  --border: rgba(8, 145, 178, 0.15);
  --border-hover: rgba(8, 145, 178, 0.3);
  --border-soft: rgba(8, 145, 178, 0.08);
  --border-purple: rgba(124, 58, 237, 0.18);
  --text: #1e293b;
  --text-dim: #64748b;
  --text-muted: #94a3b8;
  --text-dimmer: #475569;
  --glow: rgba(8, 145, 178, 0.18);
  --glow-soft: rgba(8, 145, 178, 0.08);
  --glow-strong: rgba(8, 145, 178, 0.28);
  --glow-purple: rgba(124, 58, 237, 0.18);
  --shadow-card: 0 4px 24px rgba(15, 23, 42, 0.06), 0 0 0 1px rgba(8, 145, 178, 0.06);
  --shadow-hover: 0 10px 32px rgba(8, 145, 178, 0.12);
  --shadow-purple: 0 8px 28px rgba(124, 58, 237, 0.1);
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

.app-root.theme-light {
  --primary: #0891b2;
  --primary-soft: rgba(8, 145, 178, 0.1);
  --primary-glow: rgba(8, 145, 178, 0.3);
  --secondary: #7c3aed;
  --secondary-soft: rgba(124, 58, 237, 0.1);
  --secondary-glow: rgba(124, 58, 237, 0.25);
  --accent: #db2777;
  --accent-glow: rgba(219, 39, 119, 0.25);
  --bg: #f8fafc;
  --bg-deep: #f1f5f9;
  --bg-card: rgba(255, 255, 255, 0.98);
  --bg-card-hover: rgba(255, 255, 255, 1);
  --bg-soft: rgba(8, 145, 178, 0.04);
  --border: rgba(8, 145, 178, 0.15);
  --border-hover: rgba(8, 145, 178, 0.3);
  --border-soft: rgba(8, 145, 178, 0.08);
  --border-purple: rgba(124, 58, 237, 0.18);
  --text: #1e293b;
  --text-dim: #64748b;
  --text-muted: #94a3b8;
  --text-dimmer: #475569;
  --glow: rgba(8, 145, 178, 0.18);
  --glow-soft: rgba(8, 145, 178, 0.08);
  --glow-strong: rgba(8, 145, 178, 0.28);
  --glow-purple: rgba(124, 58, 237, 0.18);
  --shadow-card: 0 4px 24px rgba(15, 23, 42, 0.06), 0 0 0 1px rgba(8, 145, 178, 0.06);
  --shadow-hover: 0 10px 32px rgba(8, 145, 178, 0.12);
  --shadow-purple: 0 8px 28px rgba(124, 58, 237, 0.1);
  --grad-main: linear-gradient(135deg, #0891b2 0%, #7c3aed 100%);
  --grad-cyan: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
  --grad-purple: linear-gradient(135deg, #7c3aed 0%, #6d28d9 100%);
  --grad-pink: linear-gradient(135deg, #db2777 0%, #be185d 100%);
}

.app-root.theme-light .scan-line,
.app-root.theme-light .scan-line-2,
.app-root.theme-light .bg-stars,
.app-root.theme-light .bg-particles { display: none; }
.app-root.theme-light .bg-glow { opacity: 0.06; filter: blur(120px); }
.app-root.theme-light .bg-glow-1 { background: #0891b2; }
.app-root.theme-light .bg-glow-2 { background: #7c3aed; }
.app-root.theme-light .bg-grid { opacity: 0.2; }
.app-root.theme-light .tech-panel {
  background: rgba(255, 255, 255, 0.98);
  border: 1px solid rgba(8, 145, 178, 0.12);
  box-shadow: 0 4px 20px rgba(15, 23, 42, 0.06);
}
.app-root.theme-light .tech-panel::before {
  background: linear-gradient(90deg, #0891b2, transparent 30%, transparent 70%, #7c3aed);
  opacity: 0.6;
}
.app-root.theme-light .tech-panel::after {
  border-top-color: #0891b2;
  border-left-color: #0891b2;
  box-shadow: 0 0 8px rgba(8, 145, 178, 0.4);
}
.app-root.theme-light .panel-corner.br::before {
  border-bottom-color: #0891b2;
  border-right-color: #0891b2;
  box-shadow: 0 0 8px rgba(8, 145, 178, 0.4);
}
.app-root.theme-light .title-text {
  background: linear-gradient(90deg, #1e293b 0%, #0891b2 40%, #7c3aed 70%, #1e293b 100%);
  background-size: 200% 100%;
  -webkit-background-clip: text;
  background-clip: text;
  filter: none;
  animation: title-gradient-flow 5s ease-in-out infinite;
}
.app-root.theme-light .tc-digit { text-shadow: 0 0 8px rgba(8, 145, 178, 0.3); color: #0891b2; }
.app-root.theme-light .kpi-lg-value .num { color: #0891b2; text-shadow: none; }
.app-root.theme-light .kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-lg-value .num { color: #7c3aed; text-shadow: none; }
.app-root.theme-light .kpi-card-lg { background: linear-gradient(135deg, rgba(8, 145, 178, 0.04) 0%, rgba(255, 255, 255, 0.9) 100%); border-color: rgba(8, 145, 178, 0.1); }
.app-root.theme-light .kpi-sm-value,
.app-root.theme-light .gender-item.male .gender-value,
.app-root.theme-light .gender-item.female .gender-value { text-shadow: none; }
.app-root.theme-light .gender-item.male .gender-value { color: #0891b2; }
.app-root.theme-light .gender-item.female .gender-value { color: #7c3aed; }
.app-root.theme-light .gender-item { background: linear-gradient(135deg, rgba(8, 145, 178, 0.03), rgba(255, 255, 255, 0.9)); }
.app-root.theme-light .kpi-sm-value { color: #db2777; }
.app-root.theme-light .tech-tab.active {
  box-shadow: 0 4px 16px rgba(8, 145, 178, 0.28);
}
.app-root.theme-light .map-area {
  background: linear-gradient(180deg, rgba(255,255,255,0.6), rgba(238,241,247,0.9));
  box-shadow: 0 4px 24px rgba(15, 23, 42, 0.06);
  border-color: rgba(8, 145, 178, 0.12);
}
.app-root.theme-light .map-deco-ready .map-grid { opacity: 0.2; }
.app-root.theme-light .map-corner::before,
.app-root.theme-light .map-corner::after { background: #0891b2; box-shadow: 0 0 8px rgba(8, 145, 178, 0.4); }
.app-root.theme-light .gauge-value { color: #0891b2; text-shadow: none; }
.app-root.theme-light .chart-area::before { opacity: 0.3; }
.app-root.theme-light .alert-item { background: linear-gradient(135deg, rgba(8, 145, 178, 0.03), rgba(255, 255, 255, 0.9)); }
.app-root.theme-light .select-box select { background: linear-gradient(135deg, rgba(8, 145, 178, 0.04), rgba(255, 255, 255, 0.9)); border-color: rgba(8, 145, 178, 0.15); }
.app-root.theme-light .btn-search { box-shadow: 0 4px 14px rgba(8, 145, 178, 0.25); }
.app-root.theme-light .btn-search:hover { box-shadow: 0 6px 20px rgba(8, 145, 178, 0.35); }
.app-root.theme-light .btn-reset { background: linear-gradient(135deg, rgba(8, 145, 178, 0.03), rgba(255, 255, 255, 0.9)); }
.app-root.theme-light .kpi-card-sm { background: linear-gradient(135deg, rgba(219, 39, 119, 0.04), rgba(255, 255, 255, 0.9)); }
.app-root.theme-light .kpi-card-sm-value { color: #db2777; text-shadow: none; }
.app-root.theme-light .gender-item.male { background: linear-gradient(135deg, rgba(8, 145, 178, 0.06), rgba(255, 255, 255, 0.9)); }
.app-root.theme-light .gender-item.female { background: linear-gradient(135deg, rgba(124, 58, 237, 0.06), rgba(255, 255, 255, 0.9)); }
.app-root.theme-light .kpi-card-hex span { background: #0891b2; box-shadow: none; opacity: 0.5; }
.app-root.theme-light .kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-card-hex span { background: #7c3aed; }
.app-root.theme-light .map-data-strip { background: rgba(255, 255, 255, 0.9); border-color: rgba(8, 145, 178, 0.15); box-shadow: 0 4px 12px rgba(15, 23, 42, 0.08); }
.app-root.theme-light .map-data-strip b { color: #0891b2; text-shadow: none; }
.app-root.theme-light .panel-title::after { background: linear-gradient(90deg, #0891b2 0%, #7c3aed 50%, transparent 100%); }
.app-root.theme-light .panel-bullet { box-shadow: 0 0 8px rgba(8, 145, 178, 0.4); }
.app-root.theme-light .panel-tag { background: rgba(8, 145, 178, 0.06); border-color: rgba(8, 145, 178, 0.15); color: #0891b2; box-shadow: none; }
.app-root.theme-light .legend-dot { box-shadow: none; }
.app-root.theme-light .map-bars { display: none; }
.app-root.theme-light .gauge-item { background: linear-gradient(135deg, rgba(8, 145, 178, 0.04), rgba(255, 255, 255, 0.9)); box-shadow: inset 0 0 16px rgba(8, 145, 178, 0.03); }
.app-root.theme-light .gauge-item::before { background: linear-gradient(180deg, rgba(8, 145, 178, 0.05), transparent); }
.app-root.theme-light .kpi-card { background: linear-gradient(135deg, rgba(8, 145, 178, 0.04), rgba(255, 255, 255, 0.95)); border-color: rgba(8, 145, 178, 0.1); box-shadow: 0 2px 12px rgba(15, 23, 42, 0.04); }
.app-root.theme-light .kpi-card:hover { border-color: rgba(8, 145, 178, 0.25); box-shadow: 0 4px 20px rgba(8, 145, 178, 0.1); }
.app-root.theme-light .kpi-card .num-text,
.app-root.theme-light .kpi-card.hc-2 .num-text,
.app-root.theme-light .kpi-card.hc-3 .num-text,
.app-root.theme-light .kpi-card.hc-4 .num-text { text-shadow: none; }
.app-root.theme-light .kpi-card .num-text { color: #0891b2; }
.app-root.theme-light .kpi-card.hc-2 .num-text { color: #7c3aed; }
.app-root.theme-light .kpi-card.hc-3 .num-text { color: #db2777; }
.app-root.theme-light .kpi-card.hc-4 .num-text { color: #059669; }
.app-root.theme-light .kpi-card-icon { filter: none; }
.app-root.theme-light .kpi-card.hc-2 .kpi-card-icon { filter: none; }
.app-root.theme-light .kpi-card.hc-3 .kpi-card-icon { filter: none; }
.app-root.theme-light .kpi-card.hc-4 .kpi-card-icon { filter: none; }
.app-root.theme-light .map-pulse-ring { box-shadow: none; }
.app-root.theme-light .map-radar-sweep { opacity: 0.15; }
.app-root.theme-light .map-loading-text { color: #0891b2; text-shadow: none; }
.app-root.theme-light .map-data-strip { color: #0891b2; }
.app-root.theme-light .chart-loading-text { color: #0891b2; text-shadow: none; }
.app-root.theme-light .filter-chip { background: linear-gradient(135deg, rgba(8, 145, 178, 0.04), rgba(255, 255, 255, 0.9)); border-color: rgba(8, 145, 178, 0.12); color: #64748b; }
.app-root.theme-light .filter-chip:hover { border-color: rgba(8, 145, 178, 0.3); color: #0891b2; box-shadow: 0 0 8px rgba(8, 145, 178, 0.15); }
.app-root.theme-light .filter-chip.active { background: linear-gradient(135deg, #0891b2, #7c3aed); color: #fff; box-shadow: 0 2px 10px rgba(8, 145, 178, 0.3); }
.app-root.theme-light .map-stats .map-s-item { background: rgba(255, 255, 255, 0.95); border-color: rgba(8, 145, 178, 0.12); box-shadow: 0 4px 14px rgba(15, 23, 42, 0.05); }
.app-root.theme-light .map-s-item .map-s-value { text-shadow: none; }
.app-root.theme-light .map-s-item:nth-child(1) .map-s-value { color: #0891b2; }
.app-root.theme-light .map-s-item:nth-child(2) .map-s-value { color: #7c3aed; }
.app-root.theme-light .map-s-item:nth-child(3) .map-s-value { color: #db2777; }
.app-root.theme-light .map-s-icon { filter: none !important; }
.app-root.theme-light .tech-topbar { background: linear-gradient(90deg, transparent, rgba(8, 145, 178, 0.04), transparent); border-bottom-color: rgba(8, 145, 178, 0.12); }
.app-root.theme-light .tech-corner { border-color: #0891b2; box-shadow: 0 0 6px rgba(8, 145, 178, 0.3); }
.app-root.theme-light .tc-dot { background: #0891b2; box-shadow: 0 0 6px rgba(8, 145, 178, 0.3); }
.app-root.theme-light .tech-line { background: linear-gradient(90deg, transparent, #0891b2, transparent); }
.app-root.theme-light .tech-dots span { background: #0891b2; box-shadow: 0 0 4px rgba(8, 145, 178, 0.3); }
.app-root.theme-light .legend-items { color: #64748b; }
.app-root.theme-light .chart-area { box-shadow: none; }
.app-root.theme-light .map-area::after { display: none; }
.app-root.theme-light .kpi-card::before { filter: none; }
.app-root.theme-light .kpi-card-lg::before,
.app-root.theme-light .kpi-card-lg::after { background: rgba(8, 145, 178, 0.15); }
.app-root.theme-light .map-decoration .md-circle { border-color: #0891b2; box-shadow: 0 0 8px rgba(8, 145, 178, 0.3); background: rgba(8, 145, 178, 0.05); color: #0891b2; }
.app-root.theme-light .map-decoration .md-circle::before { border-color: rgba(8, 145, 178, 0.3); }
.app-root.theme-light .map-decoration .md-circle::after { border-color: #0891b2; opacity: 0.3; }
.app-root.theme-light .map-decoration .map-deco-text { color: #64748b; }
.app-root.theme-light .map-deco-ready .map-corner::before,
.app-root.theme-light .map-deco-ready .map-corner::after { background: #0891b2; box-shadow: 0 0 6px rgba(8, 145, 178, 0.3); }
.app-root.theme-light .alert-item.danger::before { background: #ef4444; box-shadow: 0 0 6px rgba(239, 68, 68, 0.3); }
.app-root.theme-light .alert-item.warning::before { background: #f59e0b; box-shadow: 0 0 6px rgba(245, 158, 11, 0.3); }
.app-root.theme-light .alert-item.info::before { background: #0891b2; box-shadow: 0 0 6px rgba(8, 145, 178, 0.3); }
.app-root.theme-light .alert-item:hover { background: rgba(255, 255, 255, 0.98); box-shadow: 0 4px 16px rgba(15, 23, 42, 0.06); }
.app-root.theme-light .map-legend { background: linear-gradient(135deg, rgba(8, 145, 178, 0.04), rgba(255, 255, 255, 0.9)); border-color: rgba(8, 145, 178, 0.1); box-shadow: 0 2px 12px rgba(15, 23, 42, 0.04); }
.app-root.theme-light .legend-title { color: #64748b; text-shadow: none; }
.app-root.theme-light .legend-items { color: #64748b; }
.app-root.theme-light .map-s-item::before { background: linear-gradient(180deg, #0891b2, #7c3aed); }
.app-root.theme-light .chart-area::before { background-image: linear-gradient(rgba(8, 145, 178, 0.04) 1px, transparent 1px), linear-gradient(90deg, rgba(8, 145, 178, 0.04) 1px, transparent 1px); }
.app-root.theme-light .btn-reset { box-shadow: 0 2px 8px rgba(15, 23, 42, 0.04); }
.app-root.theme-light .btn-reset:hover { box-shadow: 0 4px 14px rgba(8, 145, 178, 0.15); }
.app-root.theme-light .tech-panel::before { opacity: 0.4; }
.app-root.theme-light .tech-panel::after { border-top-color: #0891b2; border-left-color: #0891b2; box-shadow: 0 0 6px rgba(8, 145, 178, 0.3); }
.app-root.theme-light .panel-corner::before { border-bottom-color: #0891b2; border-right-color: #0891b2; box-shadow: 0 0 6px rgba(8, 145, 178, 0.3); }
.app-root.theme-light .tech-panel:hover { box-shadow: 0 8px 32px rgba(15, 23, 42, 0.08); }
.app-root.theme-light .radar-sweep { background: conic-gradient(from 0deg, transparent 0deg, rgba(8, 145, 178, 0.3) 20deg, rgba(8, 145, 178, 0.5) 30deg, transparent 50deg, transparent 180deg, rgba(124, 58, 237, 0.25) 200deg, rgba(124, 58, 237, 0.4) 210deg, transparent 230deg); }
.app-root.theme-light .radar-pulse-ring { border-color: #0891b2; opacity: 0.4; }
.app-root.theme-light .map-radar::before { border-color: rgba(8, 145, 178, 0.4); }
.app-root.theme-light .map-radar::after { border-color: rgba(8, 145, 178, 0.2); }

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
@keyframes grid-drift {
  0% { background-position: 0 0; }
  100% { background-position: 60px 60px; }
}

.bg-glow {
  position: absolute; border-radius: 50%; filter: blur(140px);
  animation: glow-float 18s ease-in-out infinite;
}
.bg-glow-1 { width: 750px; height: 750px; background: #00d4ff; top: -200px; left: -150px; opacity: 0.25; }
.bg-glow-2 { width: 850px; height: 850px; background: #a855f7; bottom: -250px; right: -150px; opacity: 0.2; animation-delay: -6s; }
.bg-glow-3 { width: 600px; height: 600px; background: #f472b6; top: 35%; left: 45%; opacity: 0.1; animation-delay: -12s; }
@keyframes glow-float {
  0%, 100% { transform: translate(0, 0) scale(1); }
  33% { transform: translate(80px, -50px) scale(1.1); }
  66% { transform: translate(-50px, 60px) scale(0.95); }
}

.bg-stars { position: absolute; inset: 0; }
.bg-stars::before, .bg-stars::after {
  content: ''; position: absolute; inset: 0;
  background-image:
    radial-gradient(1px 1px at 20% 30%, #00d4ff 50%, transparent 100%),
    radial-gradient(1px 1px at 60% 70%, #a855f7 50%, transparent 100%),
    radial-gradient(1px 1px at 80% 10%, #ffffff 50%, transparent 100%),
    radial-gradient(1.5px 1.5px at 40% 80%, #00d4ff 50%, transparent 100%),
    radial-gradient(1px 1px at 90% 50%, #a855f7 50%, transparent 100%),
    radial-gradient(1px 1px at 10% 90%, #00d4ff 50%, transparent 100%),
    radial-gradient(1px 1px at 50% 50%, #f472b6 50%, transparent 100%),
    radial-gradient(1.5px 1.5px at 30% 60%, #00d4ff 50%, transparent 100%);
  background-size: 350px 350px;
  animation: star-twinkle 4s ease-in-out infinite alternate;
}
.bg-stars::after {
  animation-delay: 2s;
  opacity: 0.6;
  background-image:
    radial-gradient(1px 1px at 15% 20%, #00d4ff 50%, transparent 100%),
    radial-gradient(1px 1px at 55% 65%, #a855f7 50%, transparent 100%),
    radial-gradient(1px 1px at 75% 15%, #ffffff 50%, transparent 100%),
    radial-gradient(1px 1px at 35% 75%, #00d4ff 50%, transparent 100%),
    radial-gradient(1.5px 1.5px at 85% 45%, #a855f7 50%, transparent 100%);
  background-size: 280px 280px;
}
@keyframes star-twinkle { 0% { opacity: 0.3; } 100% { opacity: 0.85; } }

.bg-particles {
  position: absolute; inset: 0;
  background-image:
    radial-gradient(2px 2px at 10% 20%, rgba(0, 212, 255, 0.7) 50%, transparent 100%),
    radial-gradient(2px 2px at 30% 80%, rgba(168, 85, 247, 0.6) 50%, transparent 100%),
    radial-gradient(1.5px 1.5px at 50% 40%, rgba(0, 212, 255, 0.5) 50%, transparent 100%),
    radial-gradient(2px 2px at 70% 60%, rgba(244, 114, 182, 0.6) 50%, transparent 100%),
    radial-gradient(1.5px 1.5px at 90% 30%, rgba(0, 212, 255, 0.5) 50%, transparent 100%),
    radial-gradient(1px 1px at 25% 55%, rgba(168, 85, 247, 0.5) 50%, transparent 100%),
    radial-gradient(1.5px 1.5px at 60% 15%, rgba(0, 212, 255, 0.6) 50%, transparent 100%),
    radial-gradient(1px 1px at 80% 85%, rgba(168, 85, 247, 0.5) 50%, transparent 100%);
  background-size: 400px 400px;
  animation: particle-drift 20s linear infinite;
  opacity: 0.5;
  pointer-events: none;
  z-index: 1;
}
@keyframes particle-drift {
  0% { transform: translateY(0); }
  100% { transform: translateY(-100px); }
}

.scan-line {
  position: absolute; left: 0; right: 0; top: 0; height: 2px;
  background: linear-gradient(90deg, transparent, #00d4ff, transparent);
  box-shadow: 0 0 24px #00d4ff, 0 0 48px rgba(0, 212, 255, 0.5);
  animation: scan-move 7s linear infinite;
  z-index: 1; pointer-events: none;
}
.scan-line-2 {
  position: absolute; left: 0; right: 0; top: 0; height: 100px;
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
  background: linear-gradient(90deg, transparent, rgba(0,212,255,0.06), transparent);
  border-bottom: 1px solid var(--border-soft);
}
.tech-corner {
  width: 28px; height: 28px;
  border: 2px solid #00d4ff;
  position: relative; box-shadow: 0 0 12px rgba(0, 212, 255, 0.7), inset 0 0 8px rgba(0, 212, 255, 0.2);
}
.tech-corner-l { border-right: none; border-bottom: none; }
.tech-corner-r { border-left: none; border-bottom: none; }
.tc-dot {
  position: absolute; width: 6px; height: 6px;
  background: #00d4ff; border-radius: 50%;
  box-shadow: 0 0 12px #00d4ff, 0 0 24px rgba(0, 212, 255, 0.5);
  animation: dot-blink 1.5s ease-in-out infinite;
}
.tech-corner-l .tc-dot { right: -2.5px; bottom: -2.5px; }
.tech-corner-r .tc-dot { left: -2.5px; bottom: -2.5px; }
@keyframes dot-blink { 0%, 100% { opacity: 1; transform: scale(1); } 50% { opacity: 0.4; transform: scale(0.7); } }

.tech-line { flex: 1; height: 1px; background: linear-gradient(90deg, transparent, #00d4ff, transparent); margin: 0 8px; position: relative; }
.line-pulse {
  position: absolute; top: 50%; width: 60px; height: 2px;
  background: #00d4ff; transform: translateY(-50%);
  box-shadow: 0 0 10px #00d4ff;
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
.tech-dots span { width: 4px; height: 4px; background: #00d4ff; border-radius: 50%; box-shadow: 0 0 8px #00d4ff; animation: dot-blink 1.2s ease-in-out infinite; }
.tech-dots span:nth-child(2) { animation-delay: 0.15s; }
.tech-dots span:nth-child(3) { animation-delay: 0.3s; }
.tech-dots span:nth-child(4) { animation-delay: 0.45s; }
.tech-dots span:nth-child(5) { animation-delay: 0.6s; }
.tech-dots span:nth-child(6) { animation-delay: 0.75s; }
.tech-dots span:nth-child(7) { animation-delay: 0.9s; }

.national-header {
  position: relative; z-index: 5;
  display: flex; align-items: center; justify-content: space-between;
  padding: 6px 32px 4px; flex-shrink: 0;
}
.header-deco { display: flex; align-items: center; gap: 6px; flex: 1; }
.header-deco-l { justify-content: flex-end; padding-right: 24px; }
.header-deco-r { padding-left: 24px; }
.hd-line { width: 80px; height: 2px; background: linear-gradient(90deg, transparent, #00d4ff); box-shadow: 0 0 8px rgba(0, 212, 255, 0.6); }
.hd-dot { width: 8px; height: 8px; background: #00d4ff; border-radius: 50%; box-shadow: 0 0 14px #00d4ff, 0 0 28px rgba(0, 212, 255, 0.4); }

.national-title {
  position: relative; display: inline-flex; align-items: center;
  font-size: 26px; font-weight: 700; letter-spacing: 4px;
  margin: 0; white-space: nowrap;
}
.title-bracket { color: #a855f7; font-weight: 700; text-shadow: 0 0 16px rgba(168, 85, 247, 0.7), 0 0 32px rgba(168, 85, 247, 0.3); }
.title-text {
  background: linear-gradient(90deg, #ffffff 0%, #00d4ff 40%, #a855f7 70%, #ffffff 100%);
  background-size: 200% 100%;
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
  background-clip: text;
  filter: drop-shadow(0 0 16px rgba(0, 212, 255, 0.6)) drop-shadow(0 0 32px rgba(168, 85, 247, 0.3));
  margin: 0 12px;
  animation: title-gradient-flow 5s ease-in-out infinite;
}
@keyframes title-gradient-flow {
  0%, 100% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
}
.title-shine {
  position: absolute; top: 0; left: -50%; width: 30%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.5), transparent);
  animation: shine-move 4s ease-in-out infinite;
  pointer-events: none;
  filter: blur(1px);
}
@keyframes shine-move {
  0%, 100% { left: -50%; }
  50% { left: 120%; }
}

.national-time { display: flex; flex-direction: column; align-items: flex-end; gap: 2px; }
.time-date { font-size: 12px; color: var(--text-dimmer); letter-spacing: 1px; text-shadow: 0 0 8px rgba(0, 212, 255, 0.3); }
.time-clock { display: flex; align-items: center; gap: 1px; }
.tc-digit {
  font-size: 18px; font-weight: 700; color: #00d4ff;
  font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
  text-shadow: 0 0 12px rgba(0, 212, 255, 0.8), 0 0 24px rgba(0, 212, 255, 0.4), 0 0 4px #00d4ff;
  letter-spacing: 1px;
  animation: time-glow-pulse 2s ease-in-out infinite;
}
@keyframes time-glow-pulse {
  0%, 100% { text-shadow: 0 0 12px rgba(0, 212, 255, 0.8), 0 0 24px rgba(0, 212, 255, 0.4), 0 0 4px #00d4ff; }
  50% { text-shadow: 0 0 16px rgba(0, 212, 255, 1), 0 0 32px rgba(0, 212, 255, 0.6), 0 0 6px #00d4ff; }
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
  box-shadow: 0 0 24px rgba(0, 212, 255, 0.5), 0 0 40px rgba(168, 85, 247, 0.3);
  text-shadow: 0 0 8px rgba(255,255,255,0.6);
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
  flex: 1;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 8px 10px;
  display: flex; flex-direction: column;
  box-shadow: 0 4px 30px rgba(0, 0, 0, 0.45), 0 0 0 1px rgba(0, 212, 255, 0.06), inset 0 0 30px rgba(0, 212, 255, 0.03);
  min-height: 0;
  overflow: hidden;
  transition: box-shadow 0.3s, transform 0.3s;
}
.tech-panel:hover {
  box-shadow: 0 8px 40px rgba(0, 0, 0, 0.55), 0 0 0 1px rgba(0, 212, 255, 0.1), inset 0 0 30px rgba(0, 212, 255, 0.04), 0 0 30px rgba(0, 212, 255, 0.1);
  transform: translateY(-1px);
}
.tech-panel::before {
  content: ''; position: absolute; top: -1px; left: -1px; right: -1px; height: 2px;
  background: linear-gradient(90deg, #00d4ff, transparent 30%, transparent 70%, #a855f7);
  background-size: 200% 100%;
  border-radius: 8px 8px 0 0;
  animation: border-shine 4s linear infinite;
}
@keyframes border-shine { 0% { background-position: -200% 0; } 100% { background-position: 200% 0; } }
@keyframes border-pulse { 0%, 100% { opacity: 0.85; } 50% { opacity: 1; } }
.tech-panel::after {
  content: ''; position: absolute; top: 0; left: 0; width: 18px; height: 18px;
  border-top: 2px solid #00d4ff; border-left: 2px solid #00d4ff;
  border-radius: 8px 0 0 0;
  box-shadow: 0 0 16px rgba(0, 212, 255, 0.9), 0 0 30px rgba(0, 212, 255, 0.4);
  animation: corner-glow-pulse 2s ease-in-out infinite;
}
@keyframes corner-glow-pulse {
  0%, 100% { box-shadow: 0 0 14px rgba(0, 212, 255, 0.85), 0 0 26px rgba(0, 212, 255, 0.35); }
  50% { box-shadow: 0 0 22px rgba(0, 212, 255, 1), 0 0 40px rgba(0, 212, 255, 0.55); }
}

.panel-corner {
  position: absolute; width: 18px; height: 18px; pointer-events: none;
}
.panel-corner.br { bottom: 0; right: 0; }
.panel-corner.br::before {
  content: ''; position: absolute; bottom: 0; right: 0;
  width: 18px; height: 18px;
  border-bottom: 2px solid #00d4ff; border-right: 2px solid #00d4ff;
  border-radius: 0 0 8px 0;
  box-shadow: 0 0 16px rgba(0, 212, 255, 0.9), 0 0 30px rgba(0, 212, 255, 0.4);
  animation: corner-pulse 2s ease-in-out infinite;
}
@keyframes corner-pulse {
  0%, 100% { box-shadow: 0 0 12px rgba(0, 212, 255, 0.8), 0 0 24px rgba(0, 212, 255, 0.3); }
  50% { box-shadow: 0 0 22px rgba(0, 212, 255, 1), 0 0 38px rgba(0, 212, 255, 0.5); }
}

.panel-title {
  display: flex; align-items: center; gap: 8px;
  margin-bottom: 6px;
  position: relative;
}
.panel-title::after {
  content: '';
  position: absolute;
  bottom: -4px; left: 0; right: 0; height: 1px;
  background: linear-gradient(90deg, #00d4ff 0%, #a855f7 50%, transparent 100%);
  background-size: 200% 100%;
  animation: panel-title-line 3s linear infinite;
  opacity: 0.7;
}
@keyframes panel-title-line {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}
.panel-title-bar {
  width: 3px; height: 16px;
  background: linear-gradient(180deg, #00d4ff, #a855f7);
  border-radius: 2px;
  box-shadow: 0 0 14px rgba(0, 212, 255, 0.8), 0 0 20px rgba(168, 85, 247, 0.4);
}
.panel-title-text {
  font-size: 14px; font-weight: 600; color: var(--text);
  letter-spacing: 1px;
  text-shadow: 0 0 8px rgba(0, 212, 255, 0.3);
}
.panel-title-deco {
  flex: 1; height: 1px;
  background: linear-gradient(90deg, var(--border), transparent);
  margin-left: 4px;
}

.kpi-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
  height: 100%;
  min-height: 0;
  overflow: hidden;
}
.kpi-card {
  position: relative;
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.06), rgba(8, 18, 44, 0.6));
  border: 1px solid var(--border-soft);
  border-radius: 6px;
  padding: 8px 10px;
  overflow: hidden;
  transition: all 0.3s;
  cursor: pointer;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.3);
}
.kpi-card:hover { border-color: var(--border); transform: translateY(-2px); box-shadow: 0 4px 24px rgba(0, 212, 255, 0.2); }
.kpi-card::before {
  content: ''; position: absolute; top: 0; right: 0;
  width: 0; height: 0;
  border-style: solid;
}
.kpi-card.hc-1::before { border-width: 0 28px 28px 0; border-color: transparent #00d4ff transparent transparent; filter: drop-shadow(0 0 6px rgba(0, 212, 255, 0.6)); }
.kpi-card.hc-2::before { border-width: 0 28px 28px 0; border-color: transparent #a855f7 transparent transparent; filter: drop-shadow(0 0 6px rgba(168, 85, 247, 0.6)); }
.kpi-card.hc-3::before { border-width: 0 28px 28px 0; border-color: transparent #f472b6 transparent transparent; filter: drop-shadow(0 0 6px rgba(244, 114, 182, 0.6)); }
.kpi-card.hc-4::before { border-width: 0 28px 28px 0; border-color: transparent #34d399 transparent transparent; filter: drop-shadow(0 0 6px rgba(52, 211, 153, 0.6)); }

.kpi-card::after {
  content: ''; position: absolute; top: 4px; right: 4px;
  width: 6px; height: 6px;
  border-top: 1px solid rgba(255,255,255,0.3);
  border-right: 1px solid rgba(255,255,255,0.3);
}
.kpi-card-icon {
  font-size: 16px; color: #00d4ff;
  margin-bottom: 4px;
  filter: drop-shadow(0 0 8px rgba(0, 212, 255, 0.7));
}
.kpi-card.hc-2 .kpi-card-icon { color: #a855f7; filter: drop-shadow(0 0 8px rgba(168, 85, 247, 0.7)); }
.kpi-card.hc-3 .kpi-card-icon { color: #f472b6; filter: drop-shadow(0 0 8px rgba(244, 114, 182, 0.7)); }
.kpi-card.hc-4 .kpi-card-icon { color: #34d399; filter: drop-shadow(0 0 8px rgba(52, 211, 153, 0.7)); }
.kpi-card-label {
  font-size: 11px; color: var(--text-dim);
  margin-bottom: 4px; letter-spacing: 0.5px;
}
.kpi-card-value {
  font-size: 18px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  letter-spacing: 1px;
  animation: value-glow-pulse 3s ease-in-out infinite;
}
@keyframes value-glow-pulse {
  0%, 100% { text-shadow: 0 0 14px rgba(0, 212, 255, 0.6), 0 0 28px rgba(0, 212, 255, 0.3); }
  50% { text-shadow: 0 0 22px rgba(0, 212, 255, 0.9), 0 0 44px rgba(0, 212, 255, 0.5); }
}
.kpi-card .num-text { color: #00d4ff; text-shadow: 0 0 14px rgba(0, 212, 255, 0.7), 0 0 28px rgba(0, 212, 255, 0.3); }
.kpi-card.hc-2 .num-text { color: #a855f7; text-shadow: 0 0 14px rgba(168, 85, 247, 0.7), 0 0 28px rgba(168, 85, 247, 0.3); }
.kpi-card.hc-3 .num-text { color: #f472b6; text-shadow: 0 0 14px rgba(244, 114, 182, 0.7), 0 0 28px rgba(244, 114, 182, 0.3); }
.kpi-card.hc-4 .num-text { color: #34d399; text-shadow: 0 0 14px rgba(52, 211, 153, 0.7), 0 0 28px rgba(52, 211, 153, 0.3); }
.kpi-card-unit { font-size: 12px; color: var(--text-dim); margin-left: 4px; font-weight: 400; }
.kpi-card-trend { display: flex; align-items: center; gap: 4px; margin-top: 4px; font-size: 11px; }
.kpi-card-trend .up { color: var(--success); }
.kpi-card-trend .down { color: var(--warning); }
.kpi-card-trend .arrow { font-weight: 700; }

.kpi-card-lg {
  position: relative;
  background: var(--bg-soft);
  border: 1px solid var(--border-soft);
  border-radius: 6px;
  padding: 10px 12px;
  overflow: hidden;
  transition: all 0.3s;
}
.kpi-card-lg:hover { border-color: var(--border); box-shadow: 0 4px 20px rgba(0, 212, 255, 0.15); }
.kpi-card-lg::before {
  content: ''; position: absolute; top: 0; right: 0;
  width: 0; height: 0;
  border-style: solid;
}
.kpi-card-lg.lc-1::before { border-width: 0 40px 40px 0; border-color: transparent var(--primary) transparent transparent; }
.kpi-card-lg.lc-2::before { border-width: 0 40px 40px 0; border-color: transparent var(--secondary) transparent transparent; }
.kpi-card-lg.lc-3::before { border-width: 0 40px 40px 0; border-color: transparent var(--accent) transparent transparent; }
.kpi-card-lg.lc-4::before { border-width: 0 40px 40px 0; border-color: transparent var(--warning) transparent transparent; }
.kpi-card-lg::after {
  content: ''; position: absolute; top: 4px; right: 4px;
  width: 8px; height: 8px;
  border-top: 1.5px solid rgba(255,255,255,0.3);
  border-right: 1.5px solid rgba(255,255,255,0.3);
}
.kpi-card-lg-icon {
  font-size: 18px;
  margin-bottom: 6px;
  filter: drop-shadow(0 0 10px rgba(0, 212, 255, 0.7));
}
.kpi-card-lg.lc-1 .kpi-card-lg-icon { color: #00d4ff; }
.kpi-card-lg.lc-2 .kpi-card-lg-icon { color: #a855f7; filter: drop-shadow(0 0 10px rgba(168, 85, 247, 0.7)); }
.kpi-card-lg.lc-3 .kpi-card-lg-icon { color: #f472b6; filter: drop-shadow(0 0 10px rgba(244, 114, 182, 0.7)); }
.kpi-card-lg.lc-4 .kpi-card-lg-icon { color: #34d399; filter: drop-shadow(0 0 10px rgba(52, 211, 153, 0.7)); }
.kpi-card-lg-label {
  font-size: 12px; color: var(--text-dim);
  margin-bottom: 6px; letter-spacing: 0.5px;
}
.kpi-card-lg-value {
  font-size: 24px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  letter-spacing: 2px;
}
.kpi-card-lg .num-text-lg { color: #00d4ff; text-shadow: 0 0 16px rgba(0, 212, 255, 0.7), 0 0 32px rgba(0, 212, 255, 0.3); }
.kpi-card-lg.lc-2 .num-text-lg { color: #a855f7; text-shadow: 0 0 16px rgba(168, 85, 247, 0.7), 0 0 32px rgba(168, 85, 247, 0.3); }
.kpi-card-lg.lc-3 .num-text-lg { color: #f472b6; text-shadow: 0 0 16px rgba(244, 114, 182, 0.7), 0 0 32px rgba(244, 114, 182, 0.3); }
.kpi-card-lg.lc-4 .num-text-lg { color: #34d399; text-shadow: 0 0 16px rgba(52, 211, 153, 0.7), 0 0 32px rgba(52, 211, 153, 0.3); }
.kpi-card-lg-unit { font-size: 13px; color: var(--text-dim); margin-left: 4px; font-weight: 400; }
.kpi-card-lg-trend { display: flex; align-items: center; gap: 6px; margin-top: 6px; font-size: 12px; }
.kpi-card-lg-trend .up { color: var(--success); }
.kpi-card-lg-trend .down { color: var(--warning); }
.kpi-card-lg-trend .arrow { font-weight: 700; }

.filter-bar {
  display: flex; flex-direction: column; gap: 10px;
}
.filter-row {
  display: flex; align-items: center; gap: 10px; flex-wrap: wrap;
}
.filter-label {
  font-size: 12px; color: var(--text-dim); flex-shrink: 0;
}
.filter-item {
  display: flex; align-items: center; gap: 8px;
  flex-wrap: wrap;
}
.filter-chip {
  padding: 4px 10px;
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.06), rgba(8, 18, 44, 0.5));
  border: 1px solid var(--border-soft);
  border-radius: 4px;
  color: var(--text-dim);
  font-size: 12px;
  cursor: pointer;
  transition: all 0.3s;
}
.filter-chip:hover { border-color: #00d4ff; color: #00d4ff; box-shadow: 0 0 10px rgba(0, 212, 255, 0.3); }
.filter-chip.active {
  background: var(--grad-main);
  border-color: transparent;
  color: #fff;
  box-shadow: 0 0 14px rgba(0, 212, 255, 0.5);
}

.chart-area {
  width: 100%; height: 100%; min-height: 0;
  position: relative;
  border-radius: 6px;
  background: radial-gradient(ellipse at center, rgba(0, 212, 255, 0.03) 0%, transparent 70%);
  box-shadow: inset 0 0 30px rgba(0, 212, 255, 0.03);
}
.chart-area::before {
  content: '';
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(0, 212, 255, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.03) 1px, transparent 1px);
  background-size: 30px 30px;
  border-radius: 6px;
  pointer-events: none;
  opacity: 0.5;
}

.chart-loading {
  position: absolute; top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  display: flex; flex-direction: column; align-items: center; gap: 8px;
  color: var(--primary);
}
.chart-loading-spinner {
  width: 32px; height: 32px;
  border: 2px solid var(--border);
  border-top-color: var(--primary);
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }
.chart-loading span { font-size: 12px; }

.chart-placeholder {
  flex: 1;
  display: flex; align-items: center; justify-content: center;
  min-height: 100px;
}
.chart-empty {
  display: flex; flex-direction: column; align-items: center; gap: 8px;
  color: var(--text-muted);
}
.chart-empty-icon { font-size: 32px; opacity: 0.6; }
.chart-empty span { font-size: 12px; }

.map-area {
  position: relative;
  display: flex; flex-direction: column;
  min-height: 0;
  background:
    radial-gradient(ellipse at center, rgba(0, 212, 255, 0.03) 0%, transparent 70%),
    linear-gradient(135deg, var(--bg-card), var(--bg-deep));
  border: 1px solid var(--border);
  border-radius: 6px;
  overflow: hidden;
  box-shadow: 0 0 50px rgba(0, 212, 255, 0.15), inset 0 0 50px rgba(0, 212, 255, 0.03);
  animation: map-glow-breathe 5s ease-in-out infinite;
}
@keyframes map-glow-breathe {
  0%, 100% { box-shadow: 0 0 50px rgba(0, 212, 255, 0.15), inset 0 0 50px rgba(0, 212, 255, 0.03); }
  50% { box-shadow: 0 0 80px rgba(0, 212, 255, 0.3), inset 0 0 80px rgba(0, 212, 255, 0.06); }
}
.map-area::before {
  content: ''; position: absolute; inset: 0;
  background:
    linear-gradient(rgba(0, 212, 255, 0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.04) 1px, transparent 1px);
  background-size: 40px 40px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 75%);
  -webkit-mask-image: radial-gradient(ellipse at center, black 0%, transparent 75%);
  pointer-events: none;
}
.map-area::after {
  content: '';
  position: absolute;
  inset: 0;
  background:
    radial-gradient(circle at 20% 30%, #00d4ff 0px, transparent 2px),
    radial-gradient(circle at 80% 20%, #a855f7 0px, transparent 2px),
    radial-gradient(circle at 60% 80%, #f472b6 0px, transparent 2px),
    radial-gradient(circle at 40% 60%, #00d4ff 0px, transparent 2px),
    radial-gradient(circle at 90% 70%, #a855f7 0px, transparent 2px);
  background-size: 200px 200px, 250px 250px, 180px 180px, 220px 220px, 300px 300px;
  animation: particle-flow 8s linear infinite;
  opacity: 0.4;
  pointer-events: none;
}
@keyframes particle-flow {
  0% { transform: translateY(0); }
  100% { transform: translateY(-200px); }
}
.map-decoration {
  position: absolute; pointer-events: none;
  display: flex; align-items: center; gap: 8px;
  opacity: 0.8;
}
.map-deco-tl { top: 12px; left: 12px; }
.map-deco-tr { top: 12px; right: 12px; }
.map-deco-bl { bottom: 12px; left: 12px; }
.map-deco-br { bottom: 12px; right: 12px; }
.md-circle {
  width: 34px; height: 34px; border-radius: 50%;
  border: 1.5px solid #00d4ff;
  position: relative;
  box-shadow: 0 0 16px rgba(0, 212, 255, 0.6), inset 0 0 12px rgba(0, 212, 255, 0.15);
  background: rgba(0, 212, 255, 0.08);
  display: flex; align-items: center; justify-content: center;
  color: #00d4ff; font-size: 14px;
}
.md-circle::before {
  content: ''; position: absolute; inset: 5px;
  border: 1px solid rgba(0, 212, 255, 0.5);
  border-radius: 50%;
}
.md-circle::after {
  content: ''; position: absolute; inset: -4px;
  border: 1px dashed #00d4ff;
  border-radius: 50%;
  opacity: 0.5; animation: spin 8s linear infinite;
}
.map-deco-text { font-size: 11px; color: var(--text-dim); letter-spacing: 1px; }

.map-stats {
  position: absolute; top: 12px; right: 12px;
  z-index: 10;
}
.map-s-item {
  position: relative;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 8px 14px;
  backdrop-filter: blur(8px);
  display: flex; align-items: center; gap: 8px;
  box-shadow: 0 0 20px rgba(0, 212, 255, 0.15);
}
.map-s-item::before {
  content: ''; position: absolute; left: 0; top: 20%; bottom: 20%;
  width: 2px; background: var(--grad-main);
  border-radius: 0 2px 2px 0;
}
.map-s-icon { font-size: 16px; }
.map-s-item:nth-child(1) .map-s-icon { color: #00d4ff; filter: drop-shadow(0 0 8px rgba(0, 212, 255, 0.6)); }
.map-s-item:nth-child(2) .map-s-icon { color: #a855f7; filter: drop-shadow(0 0 8px rgba(168, 85, 247, 0.6)); }
.map-s-item:nth-child(3) .map-s-icon { color: #f472b6; filter: drop-shadow(0 0 8px rgba(244, 114, 182, 0.6)); }
.map-s-label { font-size: 11px; color: var(--text-dim); }
.map-s-value {
  font-size: 16px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  letter-spacing: 1px;
}
.map-s-item:nth-child(1) .map-s-value { color: #00d4ff; text-shadow: 0 0 10px rgba(0, 212, 255, 0.6); }
.map-s-item:nth-child(2) .map-s-value { color: #a855f7; text-shadow: 0 0 10px rgba(168, 85, 247, 0.6); }
.map-s-item:nth-child(3) .map-s-value { color: #f472b6; text-shadow: 0 0 10px rgba(244, 114, 182, 0.6); }

.map-bars {
  position: absolute; bottom: 16px; left: 50%; transform: translateX(-50%);
  display: flex; gap: 6px;
  z-index: 10;
}
.map-bar {
  width: 3px; height: 20px;
  background: #00d4ff;
  border-radius: 2px;
  box-shadow: 0 0 10px rgba(0, 212, 255, 0.6);
  animation: bar-pulse 1.8s ease-in-out infinite;
}
.map-bar:nth-child(1) { animation-delay: 0s; height: 16px; }
.map-bar:nth-child(2) { animation-delay: 0.15s; height: 24px; }
.map-bar:nth-child(3) { animation-delay: 0.3s; height: 20px; }
.map-bar:nth-child(4) { animation-delay: 0.45s; height: 28px; }
.map-bar:nth-child(5) { animation-delay: 0.6s; height: 18px; }
.map-bar:nth-child(6) { animation-delay: 0.75s; height: 22px; }
.map-bar:nth-child(7) { animation-delay: 0.9s; height: 14px; }
.map-bar:nth-child(8) { animation-delay: 1.05s; height: 26px; }
.map-bar:nth-child(9) { animation-delay: 1.2s; height: 20px; }
.map-bar:nth-child(10) { animation-delay: 1.35s; height: 18px; }
@keyframes bar-pulse { 0%, 100% { opacity: 0.4; } 50% { opacity: 1; transform: scaleY(1.2); } }

.gauge-area {
  display: flex; justify-content: center; align-items: center;
  padding: 4px 0;
}
.gauge-wrapper {
  position: relative;
  width: 140px; height: 140px;
}
.gauge-center {
  position: absolute; inset: 0;
  display: flex; flex-direction: column; align-items: center; justify-content: center;
  pointer-events: none;
}
.gauge-value {
  font-size: 26px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  color: #00d4ff;
  text-shadow: 0 0 16px rgba(0, 212, 255, 0.8), 0 0 32px rgba(0, 212, 255, 0.4);
  animation: gauge-value-pulse 3s ease-in-out infinite;
}
@keyframes gauge-value-pulse {
  0%, 100% { text-shadow: 0 0 16px rgba(0, 212, 255, 0.8), 0 0 32px rgba(0, 212, 255, 0.4); }
  50% { text-shadow: 0 0 20px rgba(0, 212, 255, 1), 0 0 40px rgba(0, 212, 255, 0.6); }
}
.gauge-label {
  font-size: 11px; color: var(--text-dim);
  letter-spacing: 1px; margin-top: 2px;
  text-shadow: 0 0 6px rgba(0, 212, 255, 0.3);
}

.alert-list {
  display: flex; flex-direction: column; gap: 4px;
  max-height: 120px;
  overflow-y: auto;
  padding-right: 4px;
}
.alert-item {
  display: flex; align-items: center; gap: 6px;
  padding: 4px 8px;
  background: var(--bg-soft);
  border: 1px solid var(--border-soft);
  border-radius: 4px;
  border-left: 3px solid;
  transition: all 0.2s;
  cursor: pointer;
}
.alert-item:hover { background: var(--bg-card); transform: translateX(2px); }
.alert-item.al-critical { border-left-color: #ef4444; }
.alert-item.al-warning { border-left-color: var(--warning); }
.alert-item.al-info { border-left-color: var(--primary); }
.alert-item.al-success { border-left-color: var(--success); }
.alert-dot {
  width: 6px; height: 6px; border-radius: 50%;
  flex-shrink: 0;
}
.alert-item.al-critical .alert-dot { background: #ef4444; box-shadow: 0 0 8px #ef4444; animation: dot-blink 1s ease-in-out infinite; }
.alert-item.al-warning .alert-dot { background: #f59e0b; box-shadow: 0 0 10px rgba(245, 158, 11, 0.7); }
.alert-item.al-info .alert-dot { background: #00d4ff; box-shadow: 0 0 10px rgba(0, 212, 255, 0.7); }
.alert-item.al-success .alert-dot { background: #34d399; box-shadow: 0 0 10px rgba(52, 211, 153, 0.7); }
.alert-content { flex: 1; min-width: 0; }
.alert-title {
  font-size: 12px; color: var(--text);
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
}
.alert-time {
  font-size: 10px; color: var(--text-dimmer);
  margin-top: 2px;
}
.alert-level {
  font-size: 10px;
  padding: 2px 6px;
  border-radius: 2px;
  font-weight: 600;
  flex-shrink: 0;
}
.alert-item.al-critical .alert-level { background: rgba(239, 68, 68, 0.15); color: #ef4444; }
.alert-item.al-warning .alert-level { background: rgba(245, 158, 11, 0.15); color: var(--warning); }
.alert-item.al-info .alert-level { background: var(--primary-soft); color: var(--primary); }
.alert-item.al-success .alert-level { background: rgba(16, 185, 129, 0.15); color: var(--success); }

/* ------ Panel Header & Body ------ */
.tech-panel-header {
  display: flex; align-items: center; gap: 6px;
  margin-bottom: 6px; position: relative;
}
.panel-bullet {
  width: 10px; height: 10px;
  background: var(--grad-main);
  border-radius: 2px;
  box-shadow: 0 0 12px rgba(0, 212, 255, 0.8), 0 0 24px rgba(168, 85, 247, 0.4);
  animation: bullet-pulse 2s ease-in-out infinite;
}
@keyframes bullet-pulse {
  0%, 100% { box-shadow: 0 0 12px rgba(0, 212, 255, 0.8), 0 0 24px rgba(168, 85, 247, 0.4); }
  50% { box-shadow: 0 0 16px rgba(0, 212, 255, 1), 0 0 32px rgba(168, 85, 247, 0.6); }
}
.panel-tag {
  margin-left: auto;
  font-size: 10px;
  padding: 2px 8px;
  background: var(--primary-soft);
  border: 1px solid var(--border-soft);
  color: var(--primary);
  border-radius: 3px;
  letter-spacing: 0.5px;
  box-shadow: 0 0 8px rgba(0, 212, 255, 0.2);
}
.tech-panel-body {
  flex: 1; min-height: 0; position: relative;
}

/* ------ Filter Bar ------ */
.filter-item {
  display: flex; align-items: center; gap: 8px;
  flex-wrap: wrap;
}
.filter-item label {
  font-size: 12px; color: var(--text-dim); flex-shrink: 0;
}
.select-box {
  position: relative; display: inline-flex; align-items: center;
}
.select-box select {
  appearance: none; -webkit-appearance: none;
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.06), rgba(8, 18, 44, 0.5));
  border: 1px solid var(--border-soft);
  border-radius: 4px;
  color: var(--text);
  font-size: 12px;
  padding: 5px 24px 5px 10px;
  font-family: inherit;
  cursor: pointer;
  outline: none;
  transition: all 0.3s;
}
.select-box select:hover, .select-box select:focus {
  border-color: #00d4ff;
  box-shadow: 0 0 10px rgba(0, 212, 255, 0.25), inset 0 0 8px rgba(0, 212, 255, 0.05);
}
.select-box .arrow-down {
  position: absolute; right: 8px; top: 50%;
  transform: translateY(-50%);
  color: #00d4ff; font-size: 8px;
  pointer-events: none;
  text-shadow: 0 0 6px rgba(0, 212, 255, 0.6);
}
.btn-group { display: flex; gap: 6px; margin-left: auto; }
.btn-search, .btn-reset {
  display: inline-flex; align-items: center; gap: 4px;
  padding: 5px 12px;
  border: 1px solid var(--border-soft);
  border-radius: 4px;
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.05), rgba(8, 18, 44, 0.5));
  color: var(--text-dim);
  font-size: 12px;
  font-family: inherit;
  cursor: pointer;
  transition: all 0.3s;
}
.btn-search:hover, .btn-reset:hover {
  border-color: #00d4ff;
  color: #00d4ff;
  background: rgba(0, 212, 255, 0.1);
  box-shadow: 0 0 12px rgba(0, 212, 255, 0.3);
}
.btn-search {
  background: var(--grad-main);
  border-color: transparent;
  color: #fff;
  box-shadow: 0 0 14px rgba(0, 212, 255, 0.5), 0 0 28px rgba(168, 85, 247, 0.2);
}
.btn-search:hover { filter: brightness(1.15); box-shadow: 0 0 20px rgba(0, 212, 255, 0.7), 0 0 36px rgba(168, 85, 247, 0.3); }
.btn-search .icon, .btn-reset .icon { font-size: 12px; }

/* ------ KPI Large Cards ------ */
.kpi-row-large {
  display: flex; gap: 10px;
}
.kpi-row-large > .kpi-card-lg { flex: 1; }
.kpi-card-lg {
  position: relative;
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.08) 0%, rgba(8, 18, 44, 0.6) 100%);
  border: 1px solid var(--border-soft);
  border-radius: 8px;
  padding: 12px 14px;
  overflow: hidden;
  transition: all 0.3s;
}
.kpi-card-lg:hover { border-color: var(--border); box-shadow: 0 6px 30px rgba(0, 212, 255, 0.25), inset 0 0 20px rgba(0, 212, 255, 0.03); transform: translateY(-2px); }
.kpi-card-deco {
  position: absolute; top: 0; right: 0;
  width: 0; height: 0;
  border-style: solid;
}
.kpi-row-large > .kpi-card-lg:nth-child(1) .kpi-card-deco { border-width: 0 44px 44px 0; border-color: transparent #00d4ff transparent transparent; filter: drop-shadow(0 0 8px rgba(0, 212, 255, 0.6)); }
.kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-card-deco { border-width: 0 44px 44px 0; border-color: transparent #a855f7 transparent transparent; filter: drop-shadow(0 0 8px rgba(168, 85, 247, 0.6)); }
.kpi-card-hex {
  position: absolute; top: 8px; right: 8px;
  display: flex; gap: 2px;
}
.kpi-card-hex span {
  width: 6px; height: 6px;
  background: #00d4ff;
  opacity: 0.7;
  clip-path: polygon(50% 0%, 100% 25%, 100% 75%, 50% 100%, 0% 75%, 0% 25%);
  box-shadow: 0 0 6px rgba(0, 212, 255, 0.6);
}
.kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-card-hex span { background: #a855f7; box-shadow: 0 0 6px rgba(168, 85, 247, 0.6); }
.kpi-lg-label {
  font-size: 12px; color: var(--text-dim);
  margin-bottom: 6px; letter-spacing: 0.5px;
}
.kpi-lg-value {
  font-size: 26px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  letter-spacing: 2px;
  margin-bottom: 6px;
  animation: value-glow-pulse 3s ease-in-out infinite;
}
.kpi-lg-value .num { color: #00d4ff; text-shadow: 0 0 18px rgba(0, 212, 255, 0.8), 0 0 36px rgba(0, 212, 255, 0.4); }
.kpi-row-large > .kpi-card-lg:nth-child(2) .kpi-lg-value .num { color: #a855f7; text-shadow: 0 0 18px rgba(168, 85, 247, 0.8), 0 0 36px rgba(168, 85, 247, 0.4); }
.kpi-lg-value .unit { font-size: 14px; color: var(--text-dim); margin-left: 4px; font-weight: 400; }
.kpi-lg-trend {
  display: flex; align-items: center; gap: 4px;
  font-size: 12px;
}
.kpi-lg-trend.up { color: var(--success); }
.kpi-lg-trend.down { color: var(--text-dim); }
.kpi-lg-trend .trend-arrow { font-weight: 700; }

/* ------ Gender Row ------ */
.kpi-row-gender {
  display: flex; gap: 10px;
  margin-top: 10px;
}
.gender-item {
  flex: 1;
  display: flex; align-items: center; gap: 10px;
  padding: 10px 12px;
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.05) 0%, rgba(8, 18, 44, 0.5) 100%);
  border: 1px solid var(--border-soft);
  border-radius: 8px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s;
}
.gender-item:hover { border-color: var(--border); transform: translateY(-1px); box-shadow: 0 4px 16px rgba(0, 212, 255, 0.15); }
.gender-item.male { border-left: 3px solid #00d4ff; }
.gender-item.female { border-left: 3px solid #a855f7; }
.gender-icon {
  width: 30px; height: 30px;
  border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  font-size: 16px; font-weight: 700;
  flex-shrink: 0;
}
.gender-item.male .gender-icon {
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.2), rgba(0, 212, 255, 0.05));
  color: #00d4ff;
  box-shadow: 0 0 14px rgba(0, 212, 255, 0.5), inset 0 0 8px rgba(0, 212, 255, 0.2);
}
.gender-item.female .gender-icon {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.2), rgba(168, 85, 247, 0.05));
  color: #a855f7;
  box-shadow: 0 0 14px rgba(168, 85, 247, 0.5), inset 0 0 8px rgba(168, 85, 247, 0.2);
}
.gender-info { flex: 1; min-width: 0; }
.gender-label {
  display: block;
  font-size: 11px; color: var(--text-dim);
  margin-bottom: 2px;
}
.gender-value {
  font-size: 18px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  letter-spacing: 1px;
}
.gender-item.male .gender-value { color: #00d4ff; text-shadow: 0 0 12px rgba(0, 212, 255, 0.7); }
.gender-item.female .gender-value { color: #a855f7; text-shadow: 0 0 12px rgba(168, 85, 247, 0.7); }
.gender-bar {
  position: absolute; left: 0; right: 0; bottom: 0;
  height: 4px;
  background: var(--border-soft);
  overflow: hidden;
  border-radius: 0 0 8px 8px;
}
.gender-bar-fill {
  height: 100%;
  transition: width 0.8s ease-out;
  border-radius: 0 0 8px 8px;
}
.gender-item.male .gender-bar-fill { background: linear-gradient(90deg, #00d4ff, #0891b2); box-shadow: 0 0 8px rgba(0, 212, 255, 0.6); }
.gender-item.female .gender-bar-fill { background: linear-gradient(90deg, #a855f7, #7c3aed); box-shadow: 0 0 8px rgba(168, 85, 247, 0.6); }

/* ------ Small KPI Row ------ */
.kpi-row-small {
  display: flex; gap: 8px;
  margin-top: 10px;
}
.kpi-card-sm {
  flex: 1;
  padding: 10px 12px;
  background: linear-gradient(135deg, rgba(244, 114, 182, 0.06) 0%, rgba(8, 18, 44, 0.5) 100%);
  border: 1px solid var(--border-soft);
  border-radius: 8px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s;
}
.kpi-card-sm:hover { border-color: var(--border); transform: translateY(-2px); box-shadow: 0 4px 16px rgba(244, 114, 182, 0.15); }
.kpi-sm-label {
  font-size: 11px; color: var(--text-dim);
  margin-bottom: 4px;
}
.kpi-sm-value {
  font-size: 20px; font-weight: 700;
  font-family: 'Consolas', 'Monaco', monospace;
  color: #f472b6;
  text-shadow: 0 0 12px rgba(244, 114, 182, 0.7), 0 0 24px rgba(244, 114, 182, 0.3);
  letter-spacing: 1px;
  animation: value-glow-pulse 3s ease-in-out infinite;
}
.kpi-sm-bar {
  margin-top: 6px;
  height: 4px;
  background: var(--border-soft);
  border-radius: 2px;
  overflow: hidden;
}
.kpi-sm-bar-fill {
  height: 100%;
  background: linear-gradient(90deg, #f472b6, #db2777);
  box-shadow: 0 0 10px rgba(244, 114, 182, 0.7);
  transition: width 0.8s ease-out;
}

/* ------ Map Area ------ */
.map-canvas {
  position: absolute; inset: 0;
  width: 100%;
  height: 100%;
}
.map-loading {
  position: absolute; inset: 0;
  display: flex; flex-direction: column; align-items: center; justify-content: center;
  gap: 14px;
  z-index: 5;
}
.map-radar {
  width: 110px; height: 110px;
  border: 2px solid #00d4ff;
  border-radius: 50%;
  position: relative;
  animation: radar-rotate 3s linear infinite;
  box-shadow: 0 0 40px rgba(0, 212, 255, 0.6), 0 0 80px rgba(0, 212, 255, 0.3);
}
.map-radar::before {
  content: ''; position: absolute; inset: -10px;
  border: 1px solid #00d4ff;
  border-radius: 50%;
  opacity: 0.5;
  animation: radar-pulse 2s ease-out infinite;
}
.map-radar::after {
  content: ''; position: absolute; inset: -20px;
  border: 1px dashed #00d4ff;
  border-radius: 50%;
  opacity: 0.3;
}
@keyframes radar-rotate { 0% { transform: rotate(0); } 100% { transform: rotate(360deg); } }
@keyframes radar-pulse { 0% { transform: scale(0.8); opacity: 0.9; box-shadow: 0 0 20px rgba(0, 212, 255, 0.6); } 100% { transform: scale(1.4); opacity: 0; box-shadow: 0 0 50px rgba(0, 212, 255, 0); } }

.map-area {
  position: relative;
  display: flex; flex-direction: column;
  min-height: 0;
  background:
    radial-gradient(ellipse at center, rgba(0, 212, 255, 0.05) 0%, transparent 70%),
    linear-gradient(135deg, var(--bg-card), var(--bg-deep));
  border: 1px solid var(--border);
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 0 50px rgba(0, 212, 255, 0.12), inset 0 0 50px rgba(0, 212, 255, 0.03);
  animation: map-glow-breathe 5s ease-in-out infinite;
}
@keyframes map-glow-breathe {
  0%, 100% { box-shadow: 0 0 50px rgba(0, 212, 255, 0.12), inset 0 0 50px rgba(0, 212, 255, 0.03); }
  50% { box-shadow: 0 0 80px rgba(0, 212, 255, 0.25), inset 0 0 70px rgba(0, 212, 255, 0.06); }
}

.map-radar-sweep {
  position: absolute;
  top: 50%; left: 50%;
  width: 120%; height: 120%;
  transform: translate(-50%, -50%);
  pointer-events: none;
  background: conic-gradient(from 0deg, transparent 0deg, rgba(0, 212, 255, 0.6) 20deg, rgba(0, 212, 255, 0.9) 30deg, transparent 50deg, transparent 180deg, rgba(168, 85, 247, 0.5) 200deg, rgba(168, 85, 247, 0.8) 210deg, transparent 230deg);
  border-radius: 50%;
  opacity: 0.35;
  animation: radar-sweep-rotate 6s linear infinite;
  mix-blend-mode: screen;
  filter: blur(3px);
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
  border: 2px solid #00d4ff;
  border-radius: 50%;
  opacity: 0;
  pointer-events: none;
  animation: pulse-ring-expand 4s ease-out infinite;
  box-shadow: 0 0 20px rgba(0, 212, 255, 0.5), inset 0 0 20px rgba(0, 212, 255, 0.3);
}
.map-pulse-ring:nth-child(2) { animation-delay: -1.3s; border-color: #a855f7; box-shadow: 0 0 20px rgba(168, 85, 247, 0.5), inset 0 0 20px rgba(168, 85, 247, 0.3); }
.map-pulse-ring:nth-child(3) { animation-delay: -2.6s; border-color: #f472b6; box-shadow: 0 0 20px rgba(244, 114, 182, 0.5), inset 0 0 20px rgba(244, 114, 182, 0.3); }
@keyframes pulse-ring-expand {
  0% { width: 50px; height: 50px; opacity: 0.8; }
  100% { width: 600px; height: 600px; opacity: 0; }
}

.map-loading-text {
  font-size: 13px; color: #00d4ff;
  letter-spacing: 2px;
  text-shadow: 0 0 14px rgba(0, 212, 255, 0.9), 0 0 28px rgba(0, 212, 255, 0.5);
  animation: loading-text-pulse 1.5s ease-in-out infinite;
}
@keyframes loading-text-pulse {
  0%, 100% { opacity: 0.7; }
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
    linear-gradient(rgba(0, 212, 255, 0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.03) 1px, transparent 1px);
  background-size: 50px 50px;
}
.map-corner {
  position: absolute; width: 28px; height: 28px;
}
.map-corner::before, .map-corner::after {
  content: ''; position: absolute;
  background: #00d4ff;
  box-shadow: 0 0 12px rgba(0, 212, 255, 0.8), 0 0 24px rgba(0, 212, 255, 0.4);
}
.mc-tl { top: 10px; left: 10px; }
.mc-tl::before { top: 0; left: 0; width: 24px; height: 2px; }
.mc-tl::after { top: 0; left: 0; width: 2px; height: 24px; }
.mc-tr { top: 10px; right: 10px; }
.mc-tr::before { top: 0; right: 0; width: 24px; height: 2px; }
.mc-tr::after { top: 0; right: 0; width: 2px; height: 24px; }
.mc-bl { bottom: 10px; left: 10px; }
.mc-bl::before { bottom: 0; left: 0; width: 24px; height: 2px; }
.mc-bl::after { bottom: 0; left: 0; width: 2px; height: 24px; }
.mc-br { bottom: 10px; right: 10px; }
.mc-br::before { bottom: 0; right: 0; width: 24px; height: 2px; }
.mc-br::after { bottom: 0; right: 0; width: 2px; height: 24px; }
.map-data-strip {
  position: absolute; bottom: 8px; left: 50%;
  transform: translateX(-50%);
  display: flex; gap: 16px;
  padding: 6px 16px;
  background: rgba(0, 20, 50, 0.75);
  border: 1px solid var(--border);
  border-radius: 6px;
  font-size: 11px;
  color: var(--text-dim);
  backdrop-filter: blur(6px);
  box-shadow: 0 0 20px rgba(0, 212, 255, 0.15);
}
.map-data-strip b { color: #00d4ff; font-weight: 600; font-family: 'Consolas', monospace; text-shadow: 0 0 6px rgba(0, 212, 255, 0.5); }
.map-drill-tip { color: var(--accent); }

.map-legend {
  display: flex; align-items: center; justify-content: space-between;
  padding: 8px 12px;
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.04), rgba(8, 18, 44, 0.5));
  border: 1px solid var(--border-soft);
  border-radius: 6px;
  box-shadow: 0 0 16px rgba(0, 212, 255, 0.1);
}
.legend-title {
  font-size: 12px; color: var(--text-dim);
  text-shadow: 0 0 6px rgba(0, 212, 255, 0.3);
}
.legend-items {
  display: flex; gap: 12px;
}
.legend-item {
  display: flex; align-items: center; gap: 4px;
  font-size: 11px; color: var(--text-dim);
}
.legend-dot {
  width: 10px; height: 10px; border-radius: 2px;
  background: #00d4ff;
}
.legend-item:nth-child(1) .legend-dot { background: #00d4ff; box-shadow: 0 0 10px rgba(0, 212, 255, 0.6); }
.legend-item:nth-child(2) .legend-dot { background: rgba(0, 212, 255, 0.5); box-shadow: 0 0 10px rgba(0, 212, 255, 0.3); }
.legend-item:nth-child(3) .legend-dot { background: #a855f7; box-shadow: 0 0 10px rgba(168, 85, 247, 0.6); }
.legend-item:nth-child(4) .legend-dot { background: #f472b6; box-shadow: 0 0 10px rgba(244, 114, 182, 0.6); }

/* ------ Gauge Row ------ */
.gauge-row {
  display: flex; gap: 8px; height: 100%; min-height: 0;
}
.gauge-item {
  flex: 1; min-height: 0;
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.03), rgba(8, 18, 44, 0.5));
  border-radius: 6px;
  padding: 8px;
  position: relative;
  box-shadow: inset 0 0 20px rgba(0, 212, 255, 0.02);
}
.gauge-item::before {
  content: ''; position: absolute; inset: 0;
  border-radius: 6px;
  background: linear-gradient(180deg, rgba(0, 212, 255, 0.04), transparent);
  pointer-events: none;
}

/* ------ Alert Items (template variant) ------ */
.alert-item {
  display: flex; align-items: center; gap: 6px;
  padding: 5px 10px;
  background: linear-gradient(135deg, rgba(0, 212, 255, 0.04), rgba(8, 18, 44, 0.5));
  border: 1px solid var(--border-soft);
  border-radius: 6px;
  transition: all 0.3s;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}
.alert-item::before {
  content: '';
  position: absolute;
  left: 0; top: 0; bottom: 0;
  width: 3px;
}
.alert-item.danger { border-left: 3px solid #ef4444; }
.alert-item.danger::before { background: #ef4444; box-shadow: 0 0 10px #ef4444; }
.alert-item.warning { border-left: 3px solid var(--warning); }
.alert-item.warning::before { background: var(--warning); box-shadow: 0 0 10px var(--warning); }
.alert-item.info { border-left: 3px solid #00d4ff; }
.alert-item.info::before { background: #00d4ff; box-shadow: 0 0 10px #00d4ff; }
.alert-item:hover { background: var(--bg-card); transform: translateX(3px); box-shadow: 0 4px 16px rgba(0, 0, 0, 0.3); }
.alert-icon { font-size: 14px; flex-shrink: 0; filter: drop-shadow(0 0 4px rgba(0, 0, 0, 0.3)); }
.alert-msg {
  font-size: 11px; color: var(--text);
  line-height: 1.4;
}

/* ------ Tab Label ------ */
.tech-tab-label { font-size: 13px; }

/* Scrollbar */
::-webkit-scrollbar { width: 6px; height: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: var(--border); border-radius: 3px; }
::-webkit-scrollbar-thumb:hover { background: var(--primary); }

/* Responsive */
@media (max-width: 1500px) {
  .national-body {
    grid-template-columns: 24% 1fr 24%;
  }
  .national-title { font-size: 28px; }
  .kpi-card-lg-value { font-size: 24px; }
}

@media (max-width: 1200px) {
  .app-root { height: auto; min-height: 100vh; overflow-y: auto; }
  .national-body {
    grid-template-columns: 1fr;
  }
  .national-title { font-size: 24px; }
  .kpi-grid { grid-template-columns: 1fr 1fr; }
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
  .map-stats { position: relative; top: auto; right: auto; display: flex; gap: 6px; margin-bottom: 8px; }
  .map-s-item { padding: 4px 10px; }
  .map-deco-tl, .map-deco-tr, .map-deco-bl, .map-deco-br { display: none; }
}

@media (max-width: 480px) {
  .national-title { font-size: 18px; letter-spacing: 1px; }
  .title-bracket { display: none; }
  .kpi-grid { grid-template-columns: 1fr; }
  .kpi-card-lg { padding: 10px; }
  .kpi-card-lg-value { font-size: 20px; }
  .map-area { min-height: 240px; }
}
</style>
