<template>
  <div class="report-panel">
    <!-- ===== 筛选条件 ===== -->
    <div class="filter-section">
      <div class="filter-row">
        <div class="filter-group">
          <label class="filter-label">统计维度：</label>
          <select v-model="filter.dimension" class="filter-select">
            <option value="province">按省份</option>
            <option value="city">按城市</option>
            <option value="district">按区县</option>
            <option value="school">按学校</option>
            <option value="grade">按年级</option>
            <option value="class">按班级</option>
          </select>
        </div>
        <div class="filter-group">
          <label class="filter-label">时间范围：</label>
          <select v-model="filter.year" class="filter-select">
            <option value="2024">2024年</option>
            <option value="2025">2025年</option>
            <option value="2026">2026年</option>
          </select>
        </div>
        <div class="filter-group">
          <label class="filter-label">学段：</label>
          <select v-model="filter.stage" class="filter-select">
            <option value="all">全部学段</option>
            <option value="primary">小学</option>
            <option value="junior">初中</option>
            <option value="senior">高中</option>
          </select>
        </div>
        <div class="filter-group">
          <label class="filter-label">学制：</label>
          <select v-model="filter.system" class="filter-select">
            <option value="province">省级</option>
            <option value="city">市级</option>
            <option value="district">区县级</option>
            <option value="school">学校级</option>
            <option value="class">班级级</option>
          </select>
        </div>
        <div class="filter-actions">
          <button class="btn btn-primary" @click="generateReport">📊 生成报表</button>
          <button class="btn btn-default" @click="resetFilter">🔄 重置</button>
        </div>
      </div>
    </div>

    <!-- ===== 统计报表 ===== -->
    <div class="report-section">
      <div class="section-header">
        <h3>📋 {{ dimensionLabel }}超重/肥胖率统计报表</h3>
        <div class="header-actions">
          <button class="btn btn-sm btn-success" @click="exportReport">📥 导出Excel</button>
          <button class="btn btn-sm btn-info" @click="printReport">🖨️ 打印</button>
        </div>
      </div>

      <div class="table-wrapper">
        <table class="report-table">
          <thead>
            <tr>
              <th>#</th>
              <th>{{ dimensionLabel }}</th>
              <th>监测人数</th>
              <th>超重人数</th>
              <th>肥胖人数</th>
              <th>超重/肥胖率</th>
              <th>同比变化</th>
              <th>趋势</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in reportData" :key="index">
              <td>{{ index + 1 }}</td>
              <td class="dimension-name">{{ item.name }}</td>
              <td>{{ formatNumber(item.total) }}</td>
              <td>{{ formatNumber(item.overweight) }}</td>
              <td>{{ formatNumber(item.obese) }}</td>
              <td>
                <span class="rate-tag" :class="getRateClass(item.rate)">
                  {{ item.rate }}%
                </span>
              </td>
              <td :class="item.change > 0 ? 'change-up' : item.change < 0 ? 'change-down' : 'change-flat'">
                {{ item.change > 0 ? '+' : '' }}{{ item.change }}%
              </td>
              <td>
                <span class="trend-tag" :class="getTrendClass(item.trend)">
                  {{ item.trend === 'up' ? '⬆ 上升' : item.trend === 'down' ? '⬇ 下降' : '➡ 持平' }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="table-footer">
        <div class="summary-info">
          监测 <strong>{{ formatNumber(totalSummary.total) }}</strong> 人，
          超重 <strong>{{ formatNumber(totalSummary.overweight) }}</strong> 人，
          肥胖 <strong>{{ formatNumber(totalSummary.obese) }}</strong> 人
        </div>
        <div class="summary-rate">
          总体超重/肥胖率：<strong>{{ totalSummary.rate }}%</strong>
        </div>
      </div>
    </div>

    <!-- ===== 图表区域 ===== -->
    <div class="chart-section">
      <div class="chart-row">
        <div class="chart-card">
          <div class="chart-header">
            <h4>📊 各维度超重/肥胖率对比</h4>
          </div>
          <div ref="barChartRef" class="chart-container"></div>
        </div>
        <div class="chart-card">
          <div class="chart-header">
            <h4>📈 年度趋势预测</h4>
          </div>
          <div ref="trendChartRef" class="chart-container"></div>
        </div>
      </div>
      <div class="chart-row">
        <div class="chart-card full">
          <div class="chart-header">
            <h4>📉 各维度超重/肥胖率详细对比</h4>
          </div>
          <div ref="detailChartRef" class="chart-container"></div>
        </div>
      </div>
    </div>

    <!-- ===== 趋势预测说明 ===== -->
    <div class="trend-prediction">
      <div class="prediction-card">
        <div class="prediction-label">当前超重/肥胖率</div>
        <div class="prediction-value">{{ currentRate }}%</div>
      </div>
      <div class="prediction-card">
        <div class="prediction-label">预测下一年度超重/肥胖率</div>
        <div class="prediction-value">{{ predictedRate }}%</div>
      </div>
      <div class="prediction-card">
        <div class="prediction-label">趋势预测</div>
        <div class="prediction-value" :class="trendPrediction">
          {{ trendPrediction === 'up' ? '⬆ 上升趋势' : trendPrediction === 'down' ? '⬇ 下降趋势' : '➡ 持平趋势' }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onBeforeUnmount, nextTick } from 'vue';
import * as echarts from 'echarts';

// ================= 筛选条件 =================
const filter = reactive({
  dimension: 'province',
  year: '2026',
  stage: 'all',
  system: 'province'
});

// ================= 维度标签 =================
const dimensionLabel = computed(() => {
  const map = {
    province: '省份',
    city: '城市',
    district: '区县',
    school: '学校',
    grade: '年级',
    class: '班级'
  };
  return map[filter.dimension] || '维度';
});

// ================= 报表数据 =================
const reportData = ref([
  { name: '江苏省', total: 1250000, overweight: 325000, obese: 175000, rate: 40.0, change: 1.2, trend: 'up' },
  { name: '浙江省', total: 980000, overweight: 245000, obese: 137200, rate: 39.0, change: 0.8, trend: 'up' },
  { name: '广东省', total: 1560000, overweight: 436800, obese: 234000, rate: 43.0, change: 1.5, trend: 'up' },
  { name: '山东省', total: 1100000, overweight: 286000, obese: 154000, rate: 40.0, change: -0.3, trend: 'down' },
  { name: '河南省', total: 1080000, overweight: 270000, obese: 151200, rate: 39.0, change: -0.5, trend: 'down' },
  { name: '四川省', total: 950000, overweight: 237500, obese: 133000, rate: 39.0, change: 0.2, trend: 'up' },
  { name: '湖北省', total: 820000, overweight: 205000, obese: 114800, rate: 39.0, change: 0.1, trend: 'flat' },
  { name: '湖南省', total: 780000, overweight: 195000, obese: 109200, rate: 39.0, change: -0.2, trend: 'down' },
]);

// ================= 汇总数据 =================
const totalSummary = computed(() => {
  const total = reportData.value.reduce((sum, item) => sum + item.total, 0);
  const overweight = reportData.value.reduce((sum, item) => sum + item.overweight, 0);
  const obese = reportData.value.reduce((sum, item) => sum + item.obese, 0);
  const rate = total > 0 ? ((overweight + obese) / total * 100).toFixed(1) : 0;
  return { total, overweight, obese, rate };
});

// ================= 趋势预测 =================
const currentRate = ref(40.0);
const predictedRate = ref(42.5);
const trendPrediction = ref('up');

// ================= 图表引用 =================
const barChartRef = ref(null);
const trendChartRef = ref(null);
const detailChartRef = ref(null);

let barChart = null;
let trendChart = null;
let detailChart = null;

// ================= 辅助函数 =================
const formatNumber = (num) => {
  if (num >= 10000) {
    return (num / 10000).toFixed(1) + '万';
  }
  return num.toLocaleString();
};

const getRateClass = (rate) => {
  if (rate >= 45) return 'rate-high';
  if (rate >= 38) return 'rate-mid';
  return 'rate-low';
};

const getTrendClass = (trend) => {
  return `trend-${trend}`;
};

// ================= 操作函数 =================
const generateReport = () => {
  reportData.value = reportData.value.map(item => ({
    ...item,
    rate: (30 + Math.random() * 20).toFixed(1),
    change: +(Math.random() * 3 - 1.5).toFixed(1),
    trend: Math.random() > 0.6 ? 'up' : Math.random() > 0.3 ? 'down' : 'flat'
  }));
  initCharts();
};

const resetFilter = () => {
  filter.dimension = 'province';
  filter.year = '2026';
  filter.stage = 'all';
  filter.system = 'province';
  generateReport();
};

const exportReport = () => {
  alert('导出Excel功能开发中...');
};

const printReport = () => {
  window.print();
};

// ================= 初始化图表 =================
const initCharts = () => {
  nextTick(() => {
    setTimeout(() => {
      initBarChart();
      initTrendChart();
      initDetailChart();
    }, 100);
  });
};

const initBarChart = () => {
  if (!barChartRef.value) return;
  if (barChart) {
    barChart.dispose();
    barChart = null;
  }
  barChart = echarts.init(barChartRef.value);

  const names = reportData.value.map(item => item.name);
  const rates = reportData.value.map(item => parseFloat(item.rate));

  barChart.setOption({
    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'shadow' },
      formatter: function(params) {
        const p = params[0];
        return `<strong>${p.name}</strong><br/>超重/肥胖率：<strong>${p.value}%</strong>`;
      }
    },
    grid: {
      left: '6%',
      right: '6%',
      top: '10%',
      bottom: '18%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: names,
      axisLabel: {
        rotate: 0,
        fontSize: 12,
        fontWeight: 500,
        color: '#606266'
      },
      axisLine: { lineStyle: { color: '#dcdfe6' } }
    },
    yAxis: {
      type: 'value',
      name: '超重/肥胖率 (%)',
      nameTextStyle: {
        fontSize: 12,
        color: '#909399'
      },
      min: 0,
      max: 60,
      splitLine: {
        lineStyle: {
          color: '#f0f0f0',
          type: 'dashed'
        }
      },
      axisLabel: {
        fontSize: 12,
        color: '#606266'
      }
    },
    series: [{
      type: 'bar',
      data: rates,
      barWidth: '45%',
      itemStyle: {
        borderRadius: [4, 4, 0, 0],
        color: function(params) {
          const value = params.value;
          if (value >= 45) return '#ff4d4f';
          if (value >= 38) return '#faad14';
          return '#52c41a';
        }
      },
      label: {
        show: true,
        position: 'top',
        formatter: '{c}%',
        fontSize: 12,
        fontWeight: 600,
        color: '#303133'
      }
    }]
  });

  barChart.resize();
};

const initTrendChart = () => {
  if (!trendChartRef.value) return;
  if (trendChart) {
    trendChart.dispose();
    trendChart = null;
  }
  trendChart = echarts.init(trendChartRef.value);

  const years = ['2022年', '2023年', '2024年', '2025年', '2026年', '2027年(预测)'];
  const data = [36.5, 37.8, 38.5, 39.2, 40.0, 42.5];

  trendChart.setOption({
    tooltip: {
      trigger: 'axis',
      formatter: function(params) {
        const p = params[0];
        return `<strong>${p.name}</strong><br/>超重/肥胖率：<strong>${p.value}%</strong>`;
      }
    },
    grid: {
      left: '6%',
      right: '6%',
      top: '10%',
      bottom: '15%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: years,
      axisLabel: {
        fontSize: 12,
        fontWeight: 500,
        color: '#606266'
      },
      axisLine: { lineStyle: { color: '#dcdfe6' } }
    },
    yAxis: {
      type: 'value',
      name: '超重/肥胖率 (%)',
      nameTextStyle: {
        fontSize: 12,
        color: '#909399'
      },
      min: 30,
      max: 50,
      splitLine: {
        lineStyle: {
          color: '#f0f0f0',
          type: 'dashed'
        }
      },
      axisLabel: {
        fontSize: 12,
        color: '#606266'
      }
    },
    series: [{
      type: 'line',
      data: data,
      smooth: true,
      symbol: 'circle',
      symbolSize: 8,
      lineStyle: {
        width: 3,
        color: '#1890ff'
      },
      areaStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
          { offset: 0, color: 'rgba(24, 144, 255, 0.3)' },
          { offset: 1, color: 'rgba(24, 144, 255, 0.02)' }
        ])
      },
      itemStyle: {
        color: '#1890ff'
      },
      label: {
        show: true,
        formatter: '{c}%',
        fontSize: 12,
        fontWeight: 600,
        color: '#303133',
        position: 'top'
      },
      markLine: {
        data: [
          { yAxis: 40.0, name: '当前值' },
          { yAxis: 42.5, name: '预测值' }
        ],
        label: {
          formatter: function(params) {
            return params.name + ': ' + params.value + '%';
          },
          fontSize: 11,
          fontWeight: 600,
          color: '#606266'
        },
        lineStyle: {
          color: '#faad14',
          type: 'dashed',
          width: 2
        }
      }
    }]
  });

  trendChart.resize();
};

const initDetailChart = () => {
  if (!detailChartRef.value) return;
  if (detailChart) {
    detailChart.dispose();
    detailChart = null;
  }
  detailChart = echarts.init(detailChartRef.value);

  const names = reportData.value.map(item => item.name);
  const overweightRates = reportData.value.map(item => +(item.overweight / item.total * 100).toFixed(1));
  const obeseRates = reportData.value.map(item => +(item.obese / item.total * 100).toFixed(1));

  detailChart.setOption({
    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'shadow' },
      formatter: function(params) {
        let result = `<strong>${params[0].name}</strong><br/>`;
        params.forEach(p => {
          result += `${p.marker} ${p.seriesName}：<strong>${p.value}%</strong><br/>`;
        });
        return result;
      }
    },
    legend: {
      data: ['超重率', '肥胖率'],
      bottom: 0,
      left: 'center',
      icon: 'roundRect',
      itemWidth: 20,
      itemHeight: 12,
      textStyle: {
        fontSize: 13,
        color: '#606266',
        fontWeight: 500
      }
    },
    grid: {
      left: '6%',
      right: '6%',
      top: '10%',
      bottom: '25%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: names,
      axisLabel: {
        rotate: 0,
        fontSize: 12,
        fontWeight: 500,
        color: '#606266'
      },
      axisLine: { lineStyle: { color: '#dcdfe6' } }
    },
    yAxis: {
      type: 'value',
      name: '比率 (%)',
      nameTextStyle: {
        fontSize: 12,
        color: '#909399'
      },
      min: 0,
      max: 35,
      splitLine: {
        lineStyle: {
          color: '#f0f0f0',
          type: 'dashed'
        }
      },
      axisLabel: {
        fontSize: 12,
        color: '#606266'
      }
    },
    series: [
      {
        name: '超重率',
        type: 'bar',
        data: overweightRates,
        barWidth: '28%',
        barGap: '10%',
        itemStyle: {
          color: '#faad14',
          borderRadius: [4, 4, 0, 0]
        },
        label: {
          show: true,
          position: 'top',
          formatter: '{c}%',
          fontSize: 11,
          fontWeight: 600,
          color: '#faad14'
        }
      },
      {
        name: '肥胖率',
        type: 'bar',
        data: obeseRates,
        barWidth: '28%',
        barGap: '10%',
        itemStyle: {
          color: '#ff4d4f',
          borderRadius: [4, 4, 0, 0]
        },
        label: {
          show: true,
          position: 'top',
          formatter: '{c}%',
          fontSize: 11,
          fontWeight: 600,
          color: '#ff4d4f'
        }
      }
    ]
  });

  detailChart.resize();
};

// ================= 窗口自适应 =================
const handleResize = () => {
  if (barChart && !barChart.isDisposed()) barChart.resize();
  if (trendChart && !trendChart.isDisposed()) trendChart.resize();
  if (detailChart && !detailChart.isDisposed()) detailChart.resize();
};

// ================= 生命周期 =================
onMounted(() => {
  setTimeout(() => {
    initCharts();
  }, 300);
  window.addEventListener('resize', handleResize);
});

onBeforeUnmount(() => {
  window.removeEventListener('resize', handleResize);
  if (barChart && !barChart.isDisposed()) barChart.dispose();
  if (trendChart && !trendChart.isDisposed()) trendChart.dispose();
  if (detailChart && !detailChart.isDisposed()) detailChart.dispose();
});
</script>

<style scoped>
.report-panel {
  background: #fff;
  border-radius: 10px;
  padding: 20px;
  border: 1px solid #ebeef5;
}

/* ===== 筛选条件 ===== */
.filter-section {
  background: #f5f7fa;
  border-radius: 8px;
  padding: 16px 20px;
  margin-bottom: 20px;
}
.filter-row {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 12px;
}
.filter-group {
  display: flex;
  align-items: center;
  gap: 6px;
}
.filter-label {
  font-size: 13px;
  color: #606266;
  font-weight: 500;
  white-space: nowrap;
}
.filter-select {
  padding: 6px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 13px;
  outline: none;
  background: #fff;
  color: #303133;
  min-width: 100px;
}
.filter-select:focus { border-color: #409eff; }
.filter-actions {
  display: flex;
  gap: 8px;
  margin-left: auto;
}

/* ===== 按钮 ===== */
.btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 8px 18px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s;
  font-weight: 500;
}
.btn-primary { background: #409eff; color: #fff; }
.btn-primary:hover { background: #66b1ff; }
.btn-success { background: #67c23a; color: #fff; }
.btn-success:hover { background: #85ce61; }
.btn-info { background: #909399; color: #fff; }
.btn-info:hover { background: #a6a9ad; }
.btn-default { background: #fff; color: #606266; border: 1px solid #dcdfe6; }
.btn-default:hover { border-color: #409eff; color: #409eff; }
.btn-sm { padding: 4px 12px; font-size: 12px; }

/* ===== 报表 ===== */
.report-section {
  margin-bottom: 20px;
}
.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  flex-wrap: wrap;
  gap: 8px;
}
.section-header h3 {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin: 0;
}
.header-actions { display: flex; gap: 8px; }

.table-wrapper {
  overflow-x: auto;
  border: 1px solid #ebeef5;
  border-radius: 8px;
}

.report-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
  min-width: 700px;
}
.report-table th {
  background: #f5f7fa;
  padding: 10px 14px;
  text-align: left;
  font-weight: 600;
  color: #606266;
  border-bottom: 2px solid #e8ecf1;
  white-space: nowrap;
}
.report-table td {
  padding: 10px 14px;
  border-bottom: 1px solid #ebeef5;
  color: #303133;
}
.report-table tr:last-child td { border-bottom: none; }
.report-table tr:hover td { background: #f5f7fa; }
.dimension-name {
  font-weight: 500;
  color: #303133;
}

.rate-tag {
  display: inline-block;
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
}
.rate-high { background: #fce4ec; color: #ff4d4f; }
.rate-mid { background: #fff3e0; color: #faad14; }
.rate-low { background: #e8f5e9; color: #52c41a; }

.change-up { color: #ff4d4f; font-weight: 600; }
.change-down { color: #52c41a; font-weight: 600; }
.change-flat { color: #faad14; font-weight: 600; }

.trend-tag { font-weight: 600; }
.trend-up { color: #ff4d4f; }
.trend-down { color: #52c41a; }
.trend-flat { color: #faad14; }

.table-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px 0;
  margin-top: 4px;
  font-size: 14px;
  color: #606266;
  flex-wrap: wrap;
  gap: 8px;
  border-top: 2px solid #e8ecf1;
}
.table-footer strong { color: #303133; }
.summary-rate strong {
  color: #409eff;
  font-size: 18px;
}

/* ===== 图表 ===== */
.chart-section {
  margin-bottom: 20px;
}
.chart-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 16px;
}
.chart-card {
  background: #fafbfc;
  border-radius: 8px;
  padding: 16px;
  border: 1px solid #ebeef5;
}
.chart-card.full { grid-column: 1 / -1; }
.chart-header h4 {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
  margin: 0 0 12px 0;
}
.chart-container { width: 100%; height: 280px; }

/* ===== 趋势预测 ===== */
.trend-prediction {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
}
.prediction-card {
  background: #fafbfc;
  border-radius: 8px;
  padding: 16px 20px;
  text-align: center;
  border: 1px solid #ebeef5;
}
.prediction-label {
  font-size: 13px;
  color: #909399;
  margin-bottom: 4px;
}
.prediction-value {
  font-size: 24px;
  font-weight: 700;
  color: #303133;
}
.prediction-value.up { color: #ff4d4f; }
.prediction-value.down { color: #52c41a; }
.prediction-value.flat { color: #faad14; }

/* ===== 响应式 ===== */
@media (max-width: 1200px) {
  .chart-row { grid-template-columns: 1fr; }
  .trend-prediction { grid-template-columns: 1fr; }
}
@media (max-width: 768px) {
  .filter-row { flex-direction: column; align-items: stretch; }
  .filter-actions { margin-left: 0; justify-content: flex-start; }
  .filter-group { flex-wrap: wrap; }
  .section-header { flex-direction: column; align-items: stretch; }
  .table-footer { flex-direction: column; align-items: center; text-align: center; }
  .chart-container { height: 220px; }
  .prediction-value { font-size: 20px; }
}
</style>
