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

<script setup lang="ts">

// 把你 country.ts 里的所有代码原封不动粘进来

import { ref, reactive, computed, onMounted, onBeforeUnmount, nextTick, watch } from 'vue'

import { useRouter, useRoute } from 'vue-router'

import * as echarts from 'echarts'


// ... 你那一大坨 DARK_COLOR / LIGHT_COLOR / getColors() 等


// 关键：把当前激活的色板暴露出去

const colors = computed(() => getColors())

</script>
<style scoped src="./css/province.css"></style>
