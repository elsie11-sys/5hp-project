<template>
  <div class="vision-statistics">
    <!-- 筛选栏 -->
    <div class="filter-bar">
      <div class="filter-row">
        <div class="filter-item">
          <label>统计维度：</label>
          <div class="select-box">
            <select v-model="filters.dimension">
              <option value="school">按学校</option>
              <option value="grade">按年级</option>
              <option value="class">按班级</option>
              <option value="region">按区域</option>
            </select>
            <span class="arrow-down">▼</span>
          </div>
        </div>
        <div class="filter-item">
          <label>时间范围：</label>
          <div class="select-box">
            <select v-model="filters.timeRange">
              <option value="2026">2026年</option>
              <option value="2025">2025年</option>
              <option value="2024">2024年</option>
            </select>
            <span class="arrow-down">▼</span>
          </div>
        </div>
        <div class="filter-item">
          <label>学段：</label>
          <div class="select-box">
            <select v-model="filters.grade">
              <option value="all">全部学段</option>
              <option value="primary">小学</option>
              <option value="junior">初中</option>
              <option value="senior">高中</option>
            </select>
            <span class="arrow-down">▼</span>
          </div>
        </div>
        <div class="btn-group">
          <button class="btn-search" @click="handleSearch">
            <span class="icon">🔍</span> 生成报表
          </button>
          <button class="btn-reset" @click="handleReset">
            <span class="icon">↺</span> 重置
          </button>
        </div>
      </div>
    </div>

    <!-- 五级统计导航 -->
    <div class="level-nav">
      <div class="level-item" :class="{ active: level === 'province' }" @click="level = 'province'">
        <span class="level-icon">🏛️</span> 省级
      </div>
      <span class="level-arrow">→</span>
      <div class="level-item" :class="{ active: level === 'city' }" @click="level = 'city'">
        <span class="level-icon">🏙️</span> 市级
      </div>
      <span class="level-arrow">→</span>
      <div class="level-item" :class="{ active: level === 'district' }" @click="level = 'district'">
        <span class="level-icon">🏘️</span> 区县级
      </div>
      <span class="level-arrow">→</span>
      <div class="level-item" :class="{ active: level === 'school' }" @click="level = 'school'">
        <span class="level-icon">🏫</span> 学校级
      </div>
      <span class="level-arrow">→</span>
      <div class="level-item" :class="{ active: level === 'class' }" @click="level = 'class'">
        <span class="level-icon">📚</span> 班级级
      </div>
    </div>

    <!-- 报表内容 -->
    <div class="report-content">
      <div class="report-table-wrapper">
        <div class="report-header">
          <h3>📊 {{ levelTitle }}近视率统计报表</h3>
          <div class="report-actions">
            <button class="btn btn-success btn-sm" @click="handleExportExcel">📊 导出Excel</button>
            <button class="btn btn-danger btn-sm" @click="handleExportPDF">📄 导出PDF</button>
          </div>
        </div>
        <table class="report-table">
          <thead>
            <tr>
              <th>#</th>
              <th>{{ levelColumnName }}</th>
              <th>监测人数</th>
              <th>近视人数</th>
              <th>近视率</th>
              <th>同比变化</th>
              <th>趋势</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in reportData" :key="index">
              <td>{{ index + 1 }}</td>
              <td>{{ item.name }}</td>
              <td>{{ item.total }}</td>
              <td>{{ item.myopia }}</td>
              <td><span class="rate-value" :class="getRateClass(item.rate)">{{ item.rate }}%</span></td>
              <td :class="item.yoy > 0 ? 'trend-up' : 'trend-down'">{{ item.yoy > 0 ? '+' : '' }}{{ item.yoy }}%</td>
              <td><span class="trend-icon">{{ item.trend === 'up' ? '📈' : '📉' }}</span></td>
            </tr>
          </tbody>
        </table>
        <div class="report-summary">
          <span>合计：监测 {{ summary.totalStudents }} 人，近视 {{ summary.totalMyopia }} 人</span>
          <span class="summary-rate">总体近视率：<strong>{{ summary.overallRate }}%</strong></span>
        </div>
      </div>

      <!-- 趋势预测 -->
      <div class="trend-predict-wrapper">
        <div class="predict-header">
          <h3>🔮 趋势预测</h3>
          <span class="predict-badge">基于历史数据</span>
        </div>
        <div ref="predictChartRef" class="predict-chart"></div>
        <div class="predict-info">
          <div class="predict-item">
            <span class="predict-label">当前近视率</span>
            <span class="predict-value">{{ predictData.currentRate }}%</span>
          </div>
          <div class="predict-item">
            <span class="predict-label">预测下季度</span>
            <span class="predict-value">{{ predictData.nextQuarter }}%</span>
          </div>
          <div class="predict-item">
            <span class="predict-label">预测下一年</span>
            <span class="predict-value">{{ predictData.nextYear }}%</span>
          </div>
          <div class="predict-item">
            <span class="predict-label">趋势判断</span>
            <span class="predict-trend" :class="predictData.trend">
              {{ predictData.trend === 'up' ? '⚠️ 上升趋势' : '✅ 下降趋势' }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onBeforeUnmount, nextTick } from 'vue';
import * as echarts from 'echarts';

// ================= 筛选条件 =================
const filters = reactive({
  dimension: 'school',
  timeRange: '2026',
  grade: 'all'
});

const level = ref('province');

// ================= 报表数据 =================
const reportData = ref([
  { name: '江苏省', total: 1250000, myopia: 670000, rate: 53.6, yoy: 1.2, trend: 'up' },
  { name: '浙江省', total: 980000, myopia: 525280, rate: 53.6, yoy: 0.8, trend: 'up' },
  { name: '广东省', total: 1560000, myopia: 876720, rate: 56.2, yoy: 1.5, trend: 'up' },
  { name: '山东省', total: 1100000, myopia: 566500, rate: 51.5, yoy: -0.3, trend: 'down' },
  { name: '河南省', total: 1080000, myopia: 541080, rate: 50.1, yoy: -0.5, trend: 'down' },
]);

// ================= 计算属性 =================
const levelTitle = computed(() => ({ province: '省级', city: '市级', district: '区县级', school: '学校级', class: '班级级' }[level.value] || '省级'));
const levelColumnName = computed(() => ({ province: '省份', city: '城市', district: '区县', school: '学校名称', class: '班级名称' }[level.value] || '省份'));

const summary = computed(() => {
  const total = reportData.value.reduce((s, i) => s + i.total, 0);
  const myopia = reportData.value.reduce((s, i) => s + i.myopia, 0);
  return { totalStudents: total.toLocaleString(), totalMyopia: myopia.toLocaleString(), overallRate: (myopia / total * 100).toFixed(1) };
});

const predictData = ref({ currentRate: 53.6, nextQuarter: 54.2, nextYear: 55.8, trend: 'up' });
const predictChartRef = ref<HTMLElement | null>(null);
let predictChart: echarts.ECharts | null = null;

// ================= 辅助函数 =================
const getRateClass = (rate: number) => {
  if (rate >= 55) return 'rate-high';
  if (rate >= 50) return 'rate-mid';
  return 'rate-low';
};

// ================= 操作函数 =================
const handleSearch = () => {
  console.log('生成报表:', filters);
  reportData.value = reportData.value.map(item => ({
    ...item,
    rate: +(item.rate + (Math.random() - 0.5) * 0.8).toFixed(1),
    yoy: +((Math.random() - 0.3) * 2).toFixed(1),
    trend: Math.random() > 0.5 ? 'up' : 'down'
  }));
  nextTick(() => initPredictChart());
};

const handleReset = () => {
  filters.dimension = 'school';
  filters.timeRange = '2026';
  filters.grade = 'all';
  level.value = 'province';
};

const handleExportExcel = () => {
  console.log('导出Excel');
};

const handleExportPDF = () => {
  console.log('导出PDF');
};

// ================= 趋势预测图表 =================
const initPredictChart = () => {
  if (!predictChartRef.value) return;
  if (predictChart && !predictChart.isDisposed()) predictChart.dispose();
  
  predictChart = echarts.init(predictChartRef.value);
  const years = ['2022', '2023', '2024', '2025', '2026', '2027Q1', '2027Q2', '2027Q3'];
  const actual = [48.5, 50.1, 51.6, 52.8, 53.6, null, null, null];
  const predicted = [null, null, null, null, 53.6, 54.0, 54.5, 55.0];
  
  predictChart.setOption({
    tooltip: { trigger: 'axis' },
    legend: { data: ['实际值', '预测值'], bottom: 0 },
    grid: { left: '3%', right: '4%', top: '8%', bottom: '25%', containLabel: true },
    xAxis: { type: 'category', data: years },
    yAxis: { type: 'value', name: '近视率 (%)', min: 45, max: 60 },
    series: [
      { name: '实际值', type: 'line', data: actual, smooth: true, lineStyle: { width: 3, color: '#1890ff' }, itemStyle: { color: '#1890ff' }, symbolSize: 8 },
      { name: '预测值', type: 'line', data: predicted, smooth: true, lineStyle: { width: 3, color: '#faad14', type: 'dashed' }, itemStyle: { color: '#faad14' }, symbolSize: 8 }
    ]
  });
  
  const resizeHandler = () => { if (predictChart && !predictChart.isDisposed()) predictChart.resize(); };
  window.addEventListener('resize', resizeHandler);
};

onMounted(() => { setTimeout(initPredictChart, 300); });
onBeforeUnmount(() => { if (predictChart && !predictChart.isDisposed()) predictChart.dispose(); });
</script>

<style scoped>
.vision-statistics { width: 100%; }

/* 筛选栏 */
.filter-bar {
  background: #fff;
  padding: 15px 20px;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.05);
  margin-bottom: 20px;
}
.filter-row {
  display: flex;
  align-items: center;
  gap: 20px;
  flex-wrap: wrap;
}
.filter-item {
  display: flex;
  align-items: center;
  gap: 8px;
}
.filter-item label {
  font-size: 14px;
  color: #606266;
  font-weight: 500;
  white-space: nowrap;
}
.select-box {
  position: relative;
  width: 140px;
}
.select-box select {
  width: 100%;
  padding: 8px 30px 8px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  appearance: none;
  outline: none;
  color: #606266;
  cursor: pointer;
  background: #fff;
}
.select-box .arrow-down {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 10px;
  color: #c0c4cc;
  pointer-events: none;
}
.btn-group { display: flex; gap: 10px; margin-left: auto; }
.btn-search, .btn-reset {
  display: flex;
  align-items: center;
  gap: 5px;
  padding: 8px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s;
}
.btn-search { background: #67c23a; color: white; }
.btn-search:hover { background: #5daf34; }
.btn-reset { background: #fff; border: 1px solid #dcdfe6; color: #606266; }
.btn-reset:hover { color: #409eff; border-color: #c6e2ff; background: #ecf5ff; }

.btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 6px 14px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  transition: all 0.3s;
}
.btn-sm { padding: 4px 12px; font-size: 12px; }
.btn-success { background: #67c23a; color: #fff; }
.btn-success:hover { background: #85ce61; }
.btn-danger { background: #f56c6c; color: #fff; }
.btn-danger:hover { background: #f78989; }

/* 五级导航 */
.level-nav {
  display: flex;
  align-items: center;
  gap: 8px;
  background: #fff;
  padding: 12px 24px;
  border-radius: 8px;
  margin-bottom: 20px;
  border: 1px solid #ebeef5;
  flex-wrap: wrap;
  justify-content: center;
}
.level-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 16px;
  border-radius: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #606266;
  font-size: 13px;
}
.level-item:hover { background: #f5f7fa; color: #409eff; }
.level-item.active { background: #e8f4fd; color: #409eff; font-weight: 600; }
.level-item .level-icon { font-size: 16px; }
.level-arrow { color: #c0c4cc; font-size: 14px; }

/* 报表内容 */
.report-content {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 20px;
}
.report-table-wrapper {
  background: #fff;
  border-radius: 10px;
  padding: 16px 20px;
  border: 1px solid #ebeef5;
}
.report-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  flex-wrap: wrap;
  gap: 10px;
}
.report-header h3 { font-size: 16px; font-weight: 600; color: #303133; margin: 0; }
.report-actions { display: flex; gap: 8px; }

.report-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
}
.report-table th {
  background: #f5f7fa;
  padding: 10px 12px;
  text-align: left;
  font-weight: 600;
  color: #606266;
  border-bottom: 2px solid #e8ecf1;
}
.report-table td {
  padding: 10px 12px;
  border-bottom: 1px solid #ebeef5;
  color: #303133;
}
.rate-value { font-weight: 600; padding: 2px 8px; border-radius: 4px; }
.rate-high { color: #ff4d4f; background: #fce4ec; }
.rate-mid { color: #faad14; background: #fff3e0; }
.rate-low { color: #52c41a; background: #e8f5e9; }
.trend-up { color: #ff4d4f; }
.trend-down { color: #52c41a; }

.report-summary {
  display: flex;
  justify-content: space-between;
  padding: 12px 4px 0;
  border-top: 2px solid #e8ecf1;
  margin-top: 4px;
  font-size: 13px;
  color: #606266;
}
.summary-rate strong { color: #303133; font-size: 16px; }

/* 趋势预测 */
.trend-predict-wrapper {
  background: #fff;
  border-radius: 10px;
  padding: 16px 20px;
  border: 1px solid #ebeef5;
}
.predict-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}
.predict-header h3 { font-size: 16px; font-weight: 600; color: #303133; margin: 0; }
.predict-badge { font-size: 11px; background: #e8f0fe; color: #409eff; padding: 2px 12px; border-radius: 12px; }
.predict-chart { width: 100%; height: 200px; }
.predict-info {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px;
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px solid #ebeef5;
}
.predict-item { display: flex; flex-direction: column; gap: 2px; }
.predict-label { font-size: 12px; color: #909399; }
.predict-value { font-size: 18px; font-weight: 700; color: #303133; }
.predict-trend { font-size: 14px; font-weight: 600; }
.predict-trend.up { color: #ff4d4f; }
.predict-trend.down { color: #52c41a; }

@media (max-width: 1200px) {
  .report-content { grid-template-columns: 1fr; }
}
@media (max-width: 768px) {
  .filter-row { flex-direction: column; align-items: stretch; }
  .select-box { width: 100%; }
  .btn-group { margin-left: 0; justify-content: center; }
  .level-nav { gap: 4px; padding: 10px 12px; }
  .level-item { padding: 4px 10px; font-size: 12px; }
  .report-header { flex-direction: column; align-items: stretch; }
  .report-actions { justify-content: center; }
  .report-table-wrapper { overflow-x: auto; }
}
</style>
