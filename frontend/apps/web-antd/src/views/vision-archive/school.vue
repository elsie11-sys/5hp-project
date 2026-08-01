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

    <header class="national-header">
      <div class="header-deco header-deco-l">
        <span class="hd-line"></span>
        <span class="hd-dot"></span>
      </div>
      <h1 class="national-title">
        <span class="title-bracket">【</span>
        <span class="title-text">{{ schoolName }}健康监测看板</span>
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

    <main class="school-body">
      <aside class="left-col">
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">学校概览</span>
            <span class="panel-tag">{{ schoolLevel }}</span>
          </header>
          <div class="tech-panel-body">
            <div class="school-info">
              <div class="info-avatar">
                <span class="avatar-icon">🏫</span>
              </div>
              <div class="info-detail">
                <div class="info-name">{{ schoolName }}</div>
                <div class="info-code">编码：{{ routeCode }}</div>
                <div class="info-meta">
                  <span>👨‍🎓 {{ schoolData.students }}人</span>
                  <span>🏛️ {{ schoolData.classes }}个班级</span>
                  <span>📚 {{ schoolData.grades }}个年级</span>
                </div>
              </div>
            </div>
            <div class="divider"></div>
            <div class="teacher-row">
              <div class="teacher-item">
                <span class="teacher-avatar">👩‍🏫</span>
                <span class="teacher-name">健康教师</span>
                <span class="teacher-count">{{ schoolData.teachers }}人</span>
              </div>
              <div class="teacher-item">
                <span class="teacher-avatar">👨‍⚕️</span>
                <span class="teacher-name">校医</span>
                <span class="teacher-count">{{ schoolData.doctors }}人</span>
              </div>
            </div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">{{ currentMetric.name }}核心指标</span>
            <span class="panel-tag">{{ currentMetric.name }}专题</span>
          </header>
          <div class="tech-panel-body">
            <div class="kpi-grid">
              <div class="kpi-row-large">
                <div class="kpi-card-lg">
                  <div class="kpi-card-deco"></div>
                  <div class="kpi-card-hex"><span></span><span></span><span></span></div>
                  <div class="kpi-lg-label">{{ currentMetric.name }}率</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ schoolData.rate }}</span><span class="unit">%</span>
                  </div>
                  <div :class="['kpi-lg-trend', schoolData.trend > 0 ? 'up' : 'down']">
                    <span class="trend-arrow">{{ schoolData.trend > 0 ? '▲' : '▼' }}</span>
                    {{ Math.abs(schoolData.trend) }}% 同比
                  </div>
                </div>
                <div class="kpi-card-lg">
                  <div class="kpi-card-deco"></div>
                  <div class="kpi-card-hex"><span></span><span></span><span></span></div>
                  <div class="kpi-lg-label">全区{{ currentMetric.name }}率</div>
                  <div class="kpi-lg-value">
                    <span class="num">{{ schoolData.districtRate }}</span><span class="unit">%</span>
                  </div>
                  <div :class="['kpi-lg-trend', schoolData.rate > schoolData.districtRate ? 'up' : 'down']">
                    <span class="trend-arrow">{{ schoolData.rate > schoolData.districtRate ? '▲' : '▼' }}</span>
                    高于全区 {{ Math.abs(Math.round((schoolData.rate - schoolData.districtRate) * 10) / 10) }}%
                  </div>
                </div>
              </div>
              <div class="kpi-row-gender">
                <div class="gender-item male">
                  <span class="gender-icon">♂</span>
                  <div class="gender-info">
                    <span class="gender-label">男生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ schoolData.maleRate }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: schoolData.maleRate + '%' }"></div></div>
                </div>
                <div class="gender-item female">
                  <span class="gender-icon">♀</span>
                  <div class="gender-info">
                    <span class="gender-label">女生{{ currentMetric.name }}率</span>
                    <span class="gender-value">{{ schoolData.femaleRate }}%</span>
                  </div>
                  <div class="gender-bar"><div class="gender-bar-fill" :style="{ width: schoolData.femaleRate + '%' }"></div></div>
                </div>
              </div>
              <div class="kpi-row-small">
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">应测人数</div>
                  <div class="kpi-sm-value">{{ schoolData.shouldTest }}</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: schoolData.testCoverage + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">已测人数</div>
                  <div class="kpi-sm-value">{{ schoolData.tested }}</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: schoolData.testCoverage + '%' }"></div></div>
                </div>
                <div class="kpi-card-sm">
                  <div class="kpi-sm-label">覆盖率</div>
                  <div class="kpi-sm-value">{{ schoolData.testCoverage }}%</div>
                  <div class="kpi-sm-bar"><div class="kpi-sm-bar-fill" :style="{ width: schoolData.testCoverage + '%' }"></div></div>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">班级{{ currentMetric.name }}率排名</span>
            <span class="panel-tag">TOP {{ Math.min(10, classList.length) }}</span>
          </header>
          <div class="tech-panel-body">
            <div ref="classRankingRef" class="chart-area"></div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">预警提醒</span>
          </header>
          <div class="tech-panel-body">
            <div class="alert-list">
              <div v-for="(alert, i) in alertList" :key="i" :class="['alert-item', alert.level]">
                <span class="alert-icon">{{ alert.icon }}</span>
                <span class="alert-msg">{{ alert.message }}</span>
              </div>
            </div>
          </div>
        </section>
      </aside>

      <section class="center-col">
        <div class="center-top-bar">
          <div class="breadcrumb">
            <span class="bc-item" @click="goBack">◀ 返回县区级看板</span>
          </div>
          <div class="action-btns">
            <button class="btn-action" @click="viewProfile">查看完整档案</button>
          </div>
        </div>

        <section class="tech-panel main-chart-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">{{ currentMetric.name }}年级分布</span>
            <span class="panel-tag">全校 {{ currentMetric.name }}专题</span>
          </header>
          <div class="tech-panel-body chart-body">
            <div ref="gradeChartRef" class="chart-area"></div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">{{ currentMetric.name }}率趋势对比</span>
          </header>
          <div class="tech-panel-body chart-body">
            <div ref="trendChartRef" class="chart-area"></div>
          </div>
        </section>
      </section>

      <aside class="right-col">
        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">{{ currentMetric.name }}率与区域对比</span>
          </header>
          <div class="tech-panel-body">
            <div class="gauge-row">
              <div ref="schoolGaugeRef" class="gauge-item"></div>
              <div ref="districtGaugeRef" class="gauge-item"></div>
            </div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">{{ currentMetric.name }}率性别年级对比</span>
          </header>
          <div class="tech-panel-body">
            <div ref="genderGradeChartRef" class="chart-area"></div>
          </div>
        </section>

        <section class="tech-panel">
          <header class="tech-panel-header">
            <span class="panel-bullet"></span>
            <span class="panel-title">{{ currentMetric.name }}防控措施效果</span>
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
import { ref, computed, onMounted, onBeforeUnmount, nextTick, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import * as echarts from 'echarts';

const router = useRouter();
const route = useRoute();

const DARK_COLOR = {
  primary: '#0bc4e9', secondary: '#59ebe8', accent: '#00a8d7',
  warning: '#fbbf24', success: '#34d399', danger: '#fb7185',
  male: '#0bc4e9', female: '#59ebe8',
  text: '#e2e8f0', textDim: '#94a3b8',
  bg: 'rgba(6, 16, 28, 0.96)',
  splitLine: 'rgba(11, 196, 233, 0.12)',
  gaugeTrack: 'rgba(11, 196, 233, 0.12)', initialBar: '#475569',
};

const LIGHT_COLOR = {
  primary: '#0891b2', secondary: '#00a8d7', accent: '#0bc4e9',
  warning: '#d97706', success: '#059669', danger: '#dc2626',
  male: '#0891b2', female: '#00a8d7',
  text: '#1e293b', textDim: '#64748b',
  bg: 'rgba(255, 255, 255, 0.98)',
  splitLine: 'rgba(8, 145, 178, 0.1)',
  gaugeTrack: 'rgba(8, 145, 178, 0.08)', initialBar: '#cbd5e1',
};

const getColors = () => (isLight.value ? LIGHT_COLOR : DARK_COLOR);

const makeGrad = (c1, c2, horizontal = true) =>
  new echarts.graphic.LinearGradient(0, 0, horizontal ? 1 : 0, horizontal ? 0 : 1, [
    { offset: 0, color: c1 }, { offset: 1, color: c2 },
  ]);

const SCHOOL_CODES_MAP = {
  '370102001': '济南市历下区第一实验小学', '370102002': '济南市历下区实验中学',
  '370102003': '济南市历下区东方双语学校', '370102004': '济南市历下区第二实验小学',
  '370102005': '济南市历下区育英中学', '370102006': '济南市历下区汇文学校',
  '370102007': '济南市历下区燕翔小学', '370102008': '济南市历下区济南五中',
  '370102009': '济南市历下区解放路第一小学', '370102010': '济南市历下区山大路小学',
  '370102011': '济南市历下区文化东路小学', '370102012': '济南市历下区建筑新村学校',
  '370103001': '济南市市中区经五路小学', '370103002': '济南市市中区实验中学',
  '370103003': '济南市市中区胜利大街小学', '370103004': '济南市市中区济南中学',
  '370103005': '济南市市中区育英小学', '370103006': '济南市市中区十四中学',
  '370103007': '济南市市中区南上山街小学', '370103008': '济南市市中区七贤中学',
  '370103009': '济南市市中区舜耕小学', '370103010': '济南市市中区济南十六里河中学',
  '370103011': '济南市市中区七里山小学', '370103012': '济南市市中区党家镇中心小学',
  '370104001': '济南市槐荫区经十路小学', '370104002': '济南市槐荫区实验中学',
  '370104003': '济南市槐荫区南辛庄小学', '370104004': '济南市槐荫区济南中学分校',
  '370104005': '济南市槐荫区匡山小学', '370104006': '济南市槐荫区二十七中',
  '370104007': '济南市槐荫区张庄小学', '370104008': '济南市槐荫区二机床学校',
  '370104009': '济南市槐荫区古城中学', '370104010': '济南市槐荫区段店小学',
  '370105001': '济南市天桥区无影山小学', '370105002': '济南市天桥区实验中学',
  '370105003': '济南市天桥区工人新村小学', '370105004': '济南市天桥区十一中',
  '370105005': '济南市天桥区堤口路小学', '370105006': '济南市天桥区二十九中',
  '370105007': '济南市天桥区师范路小学', '370105008': '济南市天桥区双语实验学校',
  '370105009': '济南市天桥区北坦小学', '370105010': '济南市天桥区清河小学',
  '370112001': '济南市历城区洪家楼小学', '370112002': '济南市历城区实验中学',
  '370112003': '济南市历城区第二实验小学', '370112004': '济南市历城区历城二中',
  '370112005': '济南市历城区外国语学校', '370112006': '济南市历城区大辛庄小学',
  '370112007': '济南市历城区董家镇中心小学', '370112008': '济南市历城区华山中学',
  '370112009': '济南市历城区王舍人镇小学', '370112010': '济南市历城区仲宫镇中心小学',
  '370112011': '济南市历城区唐王镇小学', '370112012': '济南市历城区郭店镇中心学校',
  '370113001': '济南市长清区实验小学', '370113002': '济南市长清区实验中学',
  '370113003': '济南市长清区第一中学', '370113004': '济南市长清区石麟小学',
  '370113005': '济南市长清区第二中学', '370113006': '济南市长清区马山镇小学',
  '370113007': '济南市长清区双泉镇中心小学', '370113008': '济南市长清区张夏镇中学',
  '370113009': '济南市长清区万德镇中心学校', '370113010': '济南市长清区五峰山小学',
  '370702001': '潍坊市潍城区实验小学', '370702002': '潍坊市潍城区实验中学',
  '370702003': '潍坊市潍城区东风小学', '370702004': '潍坊市潍城区潍坊七中',
  '370702005': '潍坊市潍城区月河路小学', '370702006': '潍坊市潍城区三中',
  '370702007': '潍坊市潍城区永安路小学', '370702008': '潍坊市潍城区芙蓉小学',
  '370702009': '潍坊市潍城区健康街小学', '370702010': '潍坊市潍城区西关小学',
  '370704001': '潍坊市坊子区实验小学', '370704002': '潍坊市坊子区实验中学',
  '370704003': '潍坊市坊子区第二实验小学', '370704004': '潍坊市坊子区潍坊四中',
  '370704005': '潍坊市坊子区凤凰小学', '370704006': '潍坊市坊子区三马路小学',
  '370704007': '潍坊市坊子区九龙涧学校', '370704008': '潍坊市坊子区坊城小学',
  '370704009': '潍坊市坊子区坊安小学', '370704010': '潍坊市坊子区黄旗堡小学',
  '370705001': '潍坊市奎文区实验小学', '370705002': '潍坊市奎文区实验中学',
  '370705003': '潍坊市奎文区胜利东小学', '370705004': '潍坊市奎文区广文中学',
  '370705005': '潍坊市奎文区德信现代学校', '370705006': '潍坊市奎文区新华中学',
  '370705007': '潍坊市奎文区圣荣小学', '370705008': '潍坊市奎文区东关育才小学',
  '370705009': '潍坊市奎文区友谊小学', '370705010': '潍坊市奎文区北苑实验学校',
  '370703001': '潍坊市寒亭区实验小学', '370703002': '潍坊市寒亭区实验中学',
  '370703003': '潍坊市寒亭区第一中学', '370703004': '潍坊市寒亭区第二实验小学',
  '370703005': '潍坊市寒亭区潍坊一中', '370703006': '潍坊市寒亭区外国语学校',
  '370703007': '潍坊市寒亭区朱里镇中心小学', '370703008': '潍坊市寒亭区固堤镇小学',
  '370703009': '潍坊市寒亭区高里镇学校', '370703010': '潍坊市寒亭区央子镇中心小学',
};

const DISTRICT_SCHOOLS_MAP = {
  '370102': ['济南市历下区第一实验小学', '济南市历下区实验中学', '济南市历下区东方双语学校',
    '济南市历下区第二实验小学', '济南市历下区育英中学', '济南市历下区汇文学校',
    '济南市历下区燕翔小学', '济南市历下区济南五中', '济南市历下区解放路第一小学',
    '济南市历下区山大路小学', '济南市历下区文化东路小学', '济南市历下区建筑新村学校'],
  '370103': ['济南市市中区经五路小学', '济南市市中区实验中学', '济南市市中区胜利大街小学',
    '济南市市中区济南中学', '济南市市中区育英小学', '济南市市中区十四中学',
    '济南市市中区南上山街小学', '济南市市中区七贤中学', '济南市市中区舜耕小学',
    '济南市市中区济南十六里河中学', '济南市市中区七里山小学', '济南市市中区党家镇中心小学'],
  '370104': ['济南市槐荫区经十路小学', '济南市槐荫区实验中学', '济南市槐荫区南辛庄小学',
    '济南市槐荫区济南中学分校', '济南市槐荫区匡山小学', '济南市槐荫区二十七中',
    '济南市槐荫区张庄小学', '济南市槐荫区二机床学校', '济南市槐荫区古城中学',
    '济南市槐荫区段店小学'],
  '370105': ['济南市天桥区无影山小学', '济南市天桥区实验中学', '济南市天桥区工人新村小学',
    '济南市天桥区十一中', '济南市天桥区堤口路小学', '济南市天桥区二十九中',
    '济南市天桥区师范路小学', '济南市天桥区双语实验学校', '济南市天桥区北坦小学',
    '济南市天桥区清河小学'],
  '370112': ['济南市历城区洪家楼小学', '济南市历城区实验中学', '济南市历城区第二实验小学',
    '济南市历城区历城二中', '济南市历城区外国语学校', '济南市历城区大辛庄小学',
    '济南市历城区董家镇中心小学', '济南市历城区华山中学', '济南市历城区王舍人镇小学',
    '济南市历城区仲宫镇中心小学', '济南市历城区唐王镇小学', '济南市历城区郭店镇中心学校'],
  '370113': ['济南市长清区实验小学', '济南市长清区实验中学', '济南市长清区第一中学',
    '济南市长清区石麟小学', '济南市长清区第二中学', '济南市长清区马山镇小学',
    '济南市长清区双泉镇中心小学', '济南市长清区张夏镇中学', '济南市长清区万德镇中心学校',
    '济南市长清区五峰山小学'],
  '370702': ['潍坊市潍城区实验小学', '潍坊市潍城区实验中学', '潍坊市潍城区东风小学',
    '潍坊市潍城区潍坊七中', '潍坊市潍城区月河路小学', '潍坊市潍城区三中',
    '潍坊市潍城区永安路小学', '潍坊市潍城区芙蓉小学', '潍坊市潍城区健康街小学',
    '潍坊市潍城区西关小学'],
  '370704': ['潍坊市坊子区实验小学', '潍坊市坊子区实验中学', '潍坊市坊子区第二实验小学',
    '潍坊市坊子区潍坊四中', '潍坊市坊子区凤凰小学', '潍坊市坊子区三马路小学',
    '潍坊市坊子区九龙涧学校', '潍坊市坊子区坊城小学', '潍坊市坊子区坊安小学',
    '潍坊市坊子区黄旗堡小学'],
  '370705': ['潍坊市奎文区实验小学', '潍坊市奎文区实验中学', '潍坊市奎文区胜利东小学',
    '潍坊市奎文区广文中学', '潍坊市奎文区德信现代学校', '潍坊市奎文区新华中学',
    '潍坊市奎文区圣荣小学', '潍坊市奎文区东关育才小学', '潍坊市奎文区友谊小学',
    '潍坊市奎文区北苑实验学校'],
  '370703': ['潍坊市寒亭区实验小学', '潍坊市寒亭区实验中学', '潍坊市寒亭区第一中学',
    '潍坊市寒亭区第二实验小学', '潍坊市寒亭区潍坊一中', '潍坊市寒亭区外国语学校',
    '潍坊市寒亭区朱里镇中心小学', '潍坊市寒亭区固堤镇小学', '潍坊市寒亭区高里镇学校',
    '潍坊市寒亭区央子镇中心小学'],
};

const routeCode = computed(() => String(route.params.id || '370102001'));

const schoolName = computed(() => {
  return SCHOOL_CODES_MAP[routeCode.value] || '未知学校';
});

const schoolDistrictCode = computed(() => {
  return routeCode.value.slice(0, 6);
});

const schoolLevel = computed(() => {
  const name = schoolName.value;
  if (name.includes('小学')) return '小学';
  if (name.includes('中学')) return '中学';
  if (name.includes('实验')) return '实验学校';
  return '学校';
});

const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️' },
  { key: 'oral', label: '口腔健康', icon: '🦷' },
  { key: 'mental', label: '心理健康', icon: '🧠' },
  { key: 'weight', label: '健康体重', icon: '⚖️' },
  { key: 'bone', label: '骨骼健康', icon: '🦴' }
];

const metricConfig = {
  vision: { name: '近视', baseRate: 55, trend: 2.0, maleFactor: 0.95, femaleFactor: 1.05 },
  oral: { name: '龋齿', baseRate: 40, trend: -1.0, maleFactor: 0.96, femaleFactor: 1.05 },
  mental: { name: '心理预警', baseRate: 16, trend: -0.6, maleFactor: 0.92, femaleFactor: 1.1 },
  weight: { name: '超重/肥胖', baseRate: 22, trend: 1.2, maleFactor: 1.12, femaleFactor: 0.88 },
  bone: { name: '骨密度偏低', baseRate: 14, trend: -0.4, maleFactor: 0.91, femaleFactor: 1.11 }
};

const activeTab = ref('vision');
const isLight = ref(false);
const currentDate = ref('');
const currentTime = ref('');

const currentMetric = computed(() => metricConfig[activeTab.value]);

const seed = computed(() => {
  const code = routeCode.value;
  let s = 0;
  for (let i = 0; i < code.length; i++) s += code.charCodeAt(i);
  return s;
});

const schoolData = computed(() => {
  const m = currentMetric.value;
  const s = seed.value;
  const rate = Math.round((m.baseRate + (s % 15 - 7)) * 10) / 10;
  const districtRate = Math.round((m.baseRate + ((s + 3) % 10 - 5)) * 10) / 10;
  const maleRate = Math.round(rate * m.maleFactor * 10) / 10;
  const femaleRate = Math.round(rate * m.femaleFactor * 10) / 10;
  const students = Math.round(600 + (s % 12) * 80);
  const teachers = Math.round(20 + (s % 8));
  const doctors = Math.round(2 + (s % 3));
  const classes = Math.round(12 + (s % 6) * 2);
  const grades = schoolLevel.value === '小学' ? 6 : schoolLevel.value === '中学' ? 3 : 6;
  const shouldTest = students;
  const testCoverage = Math.round((75 + (s % 20)) * 10) / 10;
  const tested = Math.round(shouldTest * testCoverage / 100);
  return {
    rate, districtRate, maleRate, femaleRate,
    trend: m.trend + (s % 3 - 1),
    students, teachers, doctors, classes, grades,
    shouldTest, tested, testCoverage,
  };
});

const classList = computed(() => {
  const s = seed.value;
  const rates = [];
  for (let i = 1; i <= 10; i++) {
    const r = Math.round((schoolData.value.rate + (15 - i * 2) + ((s + i * 7) % 6 - 3)) * 10) / 10;
    rates.push({ name: `${i}班`, value: r });
  }
  return rates.sort((a, b) => b.value - a.value);
});

const alertList = computed(() => {
  const d = schoolData.value;
  const m = currentMetric.value;
  const list = [];
  if (d.rate > d.districtRate + 5) {
    list.push({ level: 'danger', icon: '🔴', message: `${m.name}率高于全区平均水平${Math.round((d.rate - d.districtRate) * 10) / 10}%，需重点关注` });
  }
  if (d.testCoverage < 85) {
    list.push({ level: 'warning', icon: '🟡', message: `检测覆盖率${d.testCoverage}%，低于85%目标线，需加强推进` });
  }
  list.push({ level: 'info', icon: '🔵', message: `本月${m.name}检测完成率${d.testCoverage}%，较上月提升${(d.testCoverage % 3).toFixed(1)}%` });
  if (d.rate < d.districtRate) {
    list.push({ level: 'success', icon: '🟢', message: `${m.name}率低于全区平均水平，防控效果良好` });
  }
  return list;
});

const classRankingRef = ref(null);
const gradeChartRef = ref(null);
const trendChartRef = ref(null);
const schoolGaugeRef = ref(null);
const districtGaugeRef = ref(null);
const genderGradeChartRef = ref(null);
const interventionChartRef = ref(null);

const chartInstances = {};
let timer = null;
let themeObserver = null;

// 可视化大屏始终使用深色主题，不受系统深浅色模式影响
const applySystemTheme = () => {
  isLight.value = false;
  setTimeout(() => renderAllCharts(), 300);
};

const observeTheme = () => {
  // 不再监听系统主题变化，保持深色背景不受系统颜色影响
  themeObserver = null;
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

const goBack = () => {
  const code = schoolDistrictCode.value;
  router.push({ path: `/vision/county/${code}`, query: { tab: activeTab.value } });
};

const viewProfile = () => {
  router.push({ path: `/student/profile`, query: { school: routeCode.value, tab: activeTab.value } });
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

const getClassRankingOption = () => {
  const c = getColors();
  const data = classList.value;
  const maxRate = Math.max(...data.map(d => d.value));
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
      type: 'category', data: data.map(d => d.name), inverse: true,
      axisLine: { show: false }, axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold' },
    },
    series: [{
      name: currentMetric.value.name + '率',
      type: 'bar',
      data: data.map(d => ({
        value: d.value,
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

const mFactorCalc = (isMale, i) => {
  const base = isMale ? currentMetric.value.maleFactor : currentMetric.value.femaleFactor;
  return base + (i % 3 - 1) * 0.05;
};

const getGradeOption = () => {
  const c = getColors();
  const d = schoolData.value;
  const grades = schoolLevel.value === '小学'
    ? ['一年级', '二年级', '三年级', '四年级', '五年级', '六年级']
    : ['初一', '初二', '初三', '高一', '高二', '高三'];
  const maleData = grades.map((_, i) => Math.round((d.rate + (i * 3 % 12 - 6)) * mFactorCalc(true, i) * 10) / 10);
  const femaleData = grades.map((_, i) => Math.round((d.rate + (i * 3 % 12 - 6)) * mFactorCalc(false, i) * 10) / 10);

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
      formatter: (params) => {
        const i = params[0].dataIndex;
        return `<div style="font-weight:600;margin-bottom:4px">${grades[i]}</div>
                <div>■男生：<span style="color:${c.primary};font-weight:bold">${maleData[i]}%</span></div>
                <div>■女生：<span style="color:${c.secondary};font-weight:bold">${femaleData[i]}%</span></div>`;
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
      type: 'category', data: grades,
      axisLine: { show: false }, axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold' },
    },
    series: [
      {
        name: '男生', type: 'bar', stack: 'total', data: maleData.map(v => -v), barWidth: 14,
        itemStyle: { color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'), borderRadius: [4, 0, 0, 4] },
        label: { show: true, position: 'left', color: c.text, fontSize: 10, fontWeight: 'bold', formatter: (p) => Math.abs(p.value) + '%' },
      },
      {
        name: '女生', type: 'bar', stack: 'total', data: femaleData, barWidth: 14,
        itemStyle: { color: makeGrad(c.secondary, isLight.value ? '#5eead4' : '#0e7490'), borderRadius: [0, 4, 4, 0] },
        label: { show: true, position: 'right', color: c.text, fontSize: 10, fontWeight: 'bold', formatter: '{c}%' },
      },
    ],
  };
};

const getGenderGradeOption = () => {
  const c = getColors();
  const grades = ['低年级', '中年级', '高年级'];
  const d = schoolData.value;
  const male = grades.map((_, i) => Math.round((d.maleRate + (i * 4 % 10 - 5)) * 10) / 10);
  const female = grades.map((_, i) => Math.round((d.femaleRate + (i * 5 % 10 - 5)) * 10) / 10);
  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
    },
    legend: { data: ['男生', '女生'], textStyle: { color: c.text, fontSize: 11 }, right: 10, top: 2, itemWidth: 12, itemHeight: 12 },
    grid: { left: 40, right: 25, top: 30, bottom: 20, containLabel: true },
    xAxis: {
      type: 'value', min: -80, max: 80, splitNumber: 4,
      axisLine: { show: false }, axisTick: { show: false },
      splitLine: { lineStyle: { color: c.splitLine } },
      axisLabel: { color: c.textDim, fontSize: 10, formatter: (v) => Math.abs(v) + '' },
    },
    yAxis: {
      type: 'category', data: grades,
      axisLine: { show: false }, axisTick: { show: false },
      axisLabel: { color: c.text, fontSize: 11, fontWeight: 'bold' },
    },
    series: [
      {
        name: '男生', type: 'bar', stack: 'g', data: male.map(v => -v), barWidth: 16,
        itemStyle: { color: makeGrad(c.primary, isLight.value ? '#67e8f9' : '#0891b2'), borderRadius: [4, 0, 0, 4] },
        label: { show: true, position: 'left', color: c.text, fontSize: 10, formatter: (p) => Math.abs(p.value) + '%' },
      },
      {
        name: '女生', type: 'bar', stack: 'g', data: female, barWidth: 16,
        itemStyle: { color: makeGrad(c.secondary, isLight.value ? '#5eead4' : '#0e7490'), borderRadius: [0, 4, 4, 0] },
        label: { show: true, position: 'right', color: c.text, fontSize: 10, formatter: '{c}%' },
      },
    ],
  };
};

const getTrendOption = () => {
  const c = getColors();
  const months = ['1月', '2月', '3月', '4月', '5月', '6月'];
  const d = schoolData.value;
  const schoolRate = months.map((_, i) => Math.round((d.rate + i * 0.5 + ((seed.value + i * 3) % 4 - 2)) * 10) / 10);
  const districtRate = months.map((_, i) => Math.round((d.districtRate + i * 0.3 + ((seed.value + i * 2) % 3 - 1)) * 10) / 10);
  const targetRate = months.map((_, i) => Math.round((d.rate - 2 + i * 0.2) * 10) / 10);

  return {
    backgroundColor: 'transparent',
    tooltip: { ...getTooltip('axis') },
    legend: { data: ['本校', '全区平均', '目标线'], textStyle: { color: c.text, fontSize: 11 }, right: 10, top: 2 },
    grid: { left: 40, right: 20, top: 30, bottom: 30, containLabel: true },
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
        name: '本校', type: 'line', data: schoolRate,
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
        name: '全区平均', type: 'line', data: districtRate,
        smooth: true, symbol: 'circle', symbolSize: 6,
        lineStyle: { width: 2, color: c.secondary },
        itemStyle: { color: c.secondary },
      },
      {
        name: '目标线', type: 'line', data: targetRate,
        smooth: true, symbol: 'none',
        lineStyle: { width: 2, color: c.accent, type: 'dashed' },
      },
    ],
  };
};

const getGaugeOption = (value, name, color) => {
  return {
    backgroundColor: 'transparent',
    series: [{
      type: 'gauge', startAngle: 200, endAngle: -20, min: 0, max: 100,
      radius: '92%', center: ['50%', '58%'],
      progress: {
        show: true, width: 10, roundCap: true,
        itemStyle: {
          color: makeGrad(color, color === getColors().primary ? getColors().secondary : getColors().accent),
          shadowBlur: isLight.value ? 6 : 12, shadowColor: `${color}88`,
        },
      },
      axisLine: { lineStyle: { width: 10, color: [[1, getColors().gaugeTrack]] } },
      pointer: { show: false }, axisTick: { show: false }, splitLine: { show: false },
      axisLabel: { show: false }, anchor: { show: false },
      detail: {
        valueAnimation: true, formatter: '{value}%', color, fontSize: 22, fontWeight: 'bold',
        offsetCenter: [0, '15%'], textShadowBlur: isLight.value ? 0 : 8, textShadowColor: color,
      },
      title: { show: true, offsetCenter: [0, '70%'], color: getColors().textDim, fontSize: 12 },
      data: [{ value, name }],
    }],
  };
};

const getInterventionOption = () => {
  const c = getColors();
  const s = seed.value;
  const d = schoolData.value;
  const data = [
    { name: '健康讲座', initial: Math.round((d.rate + 5 + (s % 4)) * 10) / 10, final: Math.round((d.rate + 1 - (s % 2)) * 10) / 10 },
    { name: '课间操', initial: Math.round((d.rate + 8 + (s % 3)) * 10) / 10, final: Math.round((d.rate + 2 - (s % 3)) * 10) / 10 },
    { name: '体检监测', initial: Math.round((d.rate + 3 + (s % 5)) * 10) / 10, final: Math.round((d.rate + 1 - (s % 2)) * 10) / 10 },
    { name: '家校联动', initial: Math.round((d.rate + 6 + (s % 2)) * 10) / 10, final: Math.round((d.rate + 1 - (s % 3)) * 10) / 10 },
  ];
  const names = data.map(d => d.name);
  const initials = data.map(d => d.initial);
  const finals = data.map(d => d.final);

  return {
    backgroundColor: 'transparent',
    tooltip: {
      ...getTooltip('axis'),
      axisPointer: { type: 'shadow' },
    },
    legend: { show: false },
    grid: { left: 70, right: 50, top: 8, bottom: 8, containLabel: true },
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
        label: { show: true, position: 'right', color: c.primary, fontSize: 10, fontWeight: 'bold', formatter: '{c}%' },
      },
    ],
  };
};

const renderAllCharts = () => {
  disposeAll();
  requestAnimationFrame(() => {
    nextTick(() => {
      const d = schoolData.value;
      initChart(classRankingRef, 'classRanking', getClassRankingOption());
      initChart(gradeChartRef, 'grade', getGradeOption());
      initChart(trendChartRef, 'trend', getTrendOption());
      initChart(schoolGaugeRef, 'schoolGauge', getGaugeOption(d.rate, `本校${currentMetric.value.name}率`, getColors().primary));
      initChart(districtGaugeRef, 'districtGauge', getGaugeOption(d.districtRate, `全区${currentMetric.value.name}率`, getColors().secondary));
      initChart(genderGradeChartRef, 'genderGrade', getGenderGradeOption());
      initChart(interventionChartRef, 'intervention', getInterventionOption());
    });
  });
};

const resizeAll = () => {
  Object.values(chartInstances).forEach(c => { if (c && !c.isDisposed()) c.resize(); });
};

onMounted(() => {
  applySystemTheme();
  observeTheme();
  updateDateTime();
  timer = setInterval(updateDateTime, 1000);
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
  if (tab && ['vision', 'oral', 'mental', 'weight', 'bone'].includes(tab)) {
    activeTab.value = tab;
    setTimeout(() => renderAllCharts(), 100);
  }
}, { immediate: true });
watch(() => route.params.id, () => {
  setTimeout(() => renderAllCharts(), 100);
});
</script>

<style scoped>
/* ============================================================
   CSS 变量 - 深色主题（青色科技风格，对齐 province.vue）
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

/* 全局背景 */
.app-root.theme-light .scan-line,
.app-root.theme-light .scan-line-2,
.app-root.theme-light .bg-stars { display: none; }
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
.app-root.theme-light .tech-tab.active { box-shadow: 0 4px 16px rgba(8, 145, 178, 0.2); }
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

/* 扫描线 */
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

/* 顶部光带 */
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

/* Header */
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

/* Tabs */
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
.tech-tab-label { font-size: 13px; }

/* 主体 */
.school-body {
  position: relative; z-index: 3;
  flex: 1;
  display: grid;
  grid-template-columns: 22% 1fr 22%;
  gap: 14px;
  padding: 8px 20px 16px;
  min-height: 0;
  height: 0;
}
.left-col, .right-col, .center-col { display: flex; flex-direction: column; gap: 12px; min-height: 0; height: 100%; }
.center-col { gap: 12px; }
.center-col .tech-panel { flex: 1; min-height: 0; }

.center-top-bar {
  display: flex; align-items: center; justify-content: space-between;
  padding: 6px 4px; flex-shrink: 0;
}
.breadcrumb { display: flex; align-items: center; gap: 8px; }
.bc-item {
  display: inline-flex; align-items: center; gap: 8px;
  font-size: 13px; color: var(--text); cursor: pointer;
  padding: 7px 16px; border-radius: 4px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  font-family: inherit;
  transition: all 0.3s;
  backdrop-filter: blur(8px);
  box-shadow: 0 0 12px var(--glow-soft);
}
.bc-item:hover { border-color: var(--primary); color: var(--primary); box-shadow: 0 0 18px var(--glow); transform: translateX(-2px); }
.action-btns { display: flex; gap: 8px; }
.btn-action {
  display: inline-flex; align-items: center; gap: 6px;
  padding: 7px 18px; font-size: 13px; font-weight: 500;
  background: var(--grad-main); border: none; border-radius: 4px;
  color: #fff; cursor: pointer; transition: all 0.3s;
  font-family: inherit;
  box-shadow: 0 0 18px var(--glow), 0 4px 14px rgba(0, 168, 215, 0.25);
  text-shadow: 0 0 6px rgba(255,255,255,0.4);
}
.btn-action:hover { filter: brightness(1.1); box-shadow: 0 0 25px var(--glow-strong), 0 6px 20px rgba(0, 168, 215, 0.35); }

/* 科技面板 */
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
}
.tech-panel::before {
  content: '';
  position: absolute;
  width: 80%; height: 100%;
  bottom: -1px; top: -1px; left: 10%;
  border-bottom: 1px solid var(--border-inner);
  border-top: 1px solid var(--border-inner);
  transition: all 0.5s;
  pointer-events: none;
}
.tech-panel::after {
  content: '';
  position: absolute;
  width: 100%; height: 80%;
  left: -1px; right: -1px; top: 10%;
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
  bottom: -1px; left: 0; right: 0; height: 1px;
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

/* 学校概览 */
.school-info { display: flex; align-items: center; gap: 12px; padding: 10px; background: rgba(0, 72, 115, 0.28); border: 1px solid var(--border-soft); border-radius: 0; }
.info-avatar {
  width: 52px; height: 52px; border-radius: 50%;
  background: var(--grad-main);
  display: flex; align-items: center; justify-content: center;
  flex-shrink: 0; box-shadow: 0 0 15px var(--glow);
}
.avatar-icon { font-size: 24px; }
.info-detail { flex: 1; min-width: 0; }
.info-name { font-size: 15px; font-weight: 700; color: var(--text); margin-bottom: 2px; text-shadow: 0 0 8px var(--glow-soft); }
.info-code { font-size: 11px; color: var(--text-dim); font-family: 'Consolas', monospace; margin-bottom: 4px; }
.info-meta { display: flex; gap: 8px; font-size: 11px; color: var(--text-dim); flex-wrap: wrap; }
.info-meta span { white-space: nowrap; }
.divider { height: 1px; background: linear-gradient(90deg, transparent, var(--border-inner), transparent); margin: 10px 0; }
.teacher-row { display: flex; gap: 10px; padding: 4px; }
.teacher-item { flex: 1; display: flex; flex-direction: column; align-items: center; gap: 4px; padding: 10px; background: rgba(0, 72, 115, 0.28); border: 1px solid var(--border-soft); border-radius: 0; transition: all 0.5s; }
.teacher-item:hover { border-color: var(--border); background: rgba(0, 90, 140, 0.4); transform: translateY(-2px); box-shadow: 0 4px 20px var(--glow-soft); }
.teacher-avatar { font-size: 20px; }
.teacher-name { font-size: 11px; color: var(--text-dim); }
.teacher-count { font-size: 16px; font-weight: bolder; color: var(--primary); font-family: "等线", 'Consolas', monospace; text-shadow: 0 0 10px var(--glow-soft); }

/* KPI */
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
  content: ''; position: absolute; top: 0; left: 0;
  width: 4px; height: 100%;
  background: var(--grad-accent);
  box-shadow: 0 0 10px var(--glow);
}
.kpi-card-lg::after {
  content: ''; position: absolute; bottom: 0; left: 0; right: 0; height: 1px;
  background: linear-gradient(90deg, var(--primary), transparent 60%);
}
.kpi-card-lg:hover { transform: translateY(-2px); border-color: var(--border-hover); box-shadow: 0 4px 20px var(--glow-soft), 0 0 30px var(--glow); }
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
  font-size: 14px; color: var(--text-dim);
  margin-left: 4px; text-shadow: 0 0 8px var(--glow-soft);
}
.kpi-lg-trend { font-size: 10px; padding: 2px 10px; border-radius: 2px; z-index: 1; display: flex; align-items: center; gap: 3px; }
.kpi-lg-trend.up { color: var(--danger); background: rgba(244, 63, 94, 0.2); border: 1px solid rgba(244, 63, 94, 0.5); box-shadow: 0 0 10px rgba(244, 63, 94, 0.3); }
.kpi-lg-trend.down { color: var(--success); background: rgba(16, 185, 129, 0.2); border: 1px solid rgba(16, 185, 129, 0.5); box-shadow: 0 0 10px rgba(16, 185, 129, 0.3); }
.trend-arrow { font-size: 9px; margin-right: 2px; }

/* 性别 */
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
  content: ''; position: absolute; left: 0; top: 0; bottom: 0; width: 4px;
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

/* 小卡片 */
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
  content: ''; position: absolute; top: 0; left: 0; right: 0; height: 2px;
  background: var(--grad-main); opacity: 0.8;
}
.kpi-card-sm:hover { border-color: var(--border); background: var(--primary-soft); transform: translateY(-2px); box-shadow: 0 4px 20px var(--glow-soft); }
.kpi-sm-label { font-size: 12px; color: var(--text-dim); margin-bottom: 6px; }
.kpi-sm-value {
  font-size: 24px; font-weight: bolder; color: #00a8d7;
  font-family: "等线", 'Consolas', monospace;
  text-shadow: 0 0 15px var(--glow-soft);
  margin-bottom: 8px; letter-spacing: 1px;
}
.kpi-sm-bar { height: 4px; background: rgba(11, 196, 233, 0.08); border-radius: 0; overflow: hidden; }
.kpi-sm-bar-fill { height: 100%; background: var(--grad-accent); border-radius: 0; box-shadow: 0 0 8px var(--glow-soft); }

.chart-area { width: 100%; height: 100%; min-height: 0; position: relative; }
.chart-area::after {
  content: ''; position: absolute; inset: 0; pointer-events: none;
  background-image:
    linear-gradient(rgba(0, 229, 255, 0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 229, 255, 0.04) 1px, transparent 1px);
  background-size: 30px 30px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 70%);
  border-radius: 4px;
}
.chart-body { flex: 1; min-height: 0; height: 0; }

.gauge-row { display: flex; gap: 14px; height: 100%; min-height: 0; }
.gauge-item { flex: 1; min-height: 0; }

/* 预警提醒 */
.alert-list { display: flex; flex-direction: column; gap: 6px; max-height: 100%; overflow-y: auto; padding-right: 4px; }
.alert-item { display: flex; align-items: center; gap: 8px; padding: 8px 12px; background: rgba(0, 72, 115, 0.28); border: 1px solid var(--border-soft); border-radius: 0; transition: all 0.25s; cursor: pointer; position: relative; overflow: hidden; }
.alert-item::before { content: ''; position: absolute; left: 0; top: 0; bottom: 0; width: 3px; }
.alert-item.danger::before { background: #ef4444; box-shadow: 0 0 8px #ef4444; }
.alert-item.warning::before { background: var(--warning); box-shadow: 0 0 8px var(--warning); }
.alert-item.info::before { background: var(--primary); box-shadow: 0 0 8px var(--primary); }
.alert-item.success::before { background: var(--success); box-shadow: 0 0 8px var(--success); }
.alert-item:hover { background: rgba(0, 90, 140, 0.4); transform: translateX(2px); }
.alert-icon { font-size: 14px; flex-shrink: 0; }
.alert-msg { font-size: 11px; color: var(--text); line-height: 1.3; }

/* 滚动条 */
::-webkit-scrollbar { width: 6px; height: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: var(--border); border-radius: 4px; }
::-webkit-scrollbar-thumb:hover { background: var(--primary); box-shadow: 0 0 8px var(--glow); }

/* 浅色模式面板 */
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
.app-root.theme-light .school-info,
.app-root.theme-light .teacher-item,
.app-root.theme-light .alert-item {
  background: rgba(8, 145, 178, 0.02);
  border: 1px solid rgba(8, 145, 178, 0.08);
}
.app-root.theme-light .tech-tab:hover { box-shadow: 0 4px 12px rgba(8, 145, 178, 0.1); }
.app-root.theme-light .tech-tab.active { box-shadow: 0 4px 16px rgba(8, 145, 178, 0.2); }

/* 响应式 */
@media (max-width: 1500px) {
  .school-body { grid-template-columns: 26% 1fr 26%; padding: 8px 14px 14px; }
  .national-title { font-size: 24px; letter-spacing: 3px; }
  .kpi-lg-value { font-size: 24px; }
}
@media (max-width: 1400px) {
  .school-body { grid-template-columns: 26% 1fr 26%; }
  .national-title { font-size: 22px; }
  .chart-area { min-height: 120px; }
}
@media (max-width: 1200px) {
  .app-root { height: auto; min-height: 100vh; overflow-y: auto; }
  .school-body { grid-template-columns: 1fr; height: auto; }
  .left-col, .right-col, .center-col { flex-direction: row; flex-wrap: wrap; height: auto; }
  .tech-panel { flex: 1 1 300px; min-height: 200px; }
  .national-title { font-size: 18px; letter-spacing: 3px; }
}
</style>
