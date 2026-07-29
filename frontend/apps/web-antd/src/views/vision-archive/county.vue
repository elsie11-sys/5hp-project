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

<script setup lang="ts">
</script>

<style scoped src="../css/county.css"></style>
