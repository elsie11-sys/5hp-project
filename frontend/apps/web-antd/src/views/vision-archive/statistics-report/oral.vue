<template>
  <div class="oral-statistics">
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
              <option value="primary_low">小学低年级</option>
              <option value="primary_mid">小学中年级</option>
              <option value="primary_high">小学高年级</option>
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
      <div 
        class="level-item" 
        :class="{ active: level === 'province' }" 
        @click="level = 'province'"
      >
        <span class="level-icon">🏛️</span>
        <span class="level-label">省级</span>
      </div>
      <span class="level-arrow">→</span>
      <div 
        class="level-item" 
        :class="{ active: level === 'city' }" 
        @click="level = 'city'"
      >
        <span class="level-icon">🏙️</span>
        <span class="level-label">市级</span>
      </div>
      <span class="level-arrow">→</span>
      <div 
        class="level-item" 
        :class="{ active: level === 'district' }" 
        @click="level = 'district'"
      >
        <span class="level-icon">🏘️</span>
        <span class="level-label">区县级</span>
      </div>
      <span class="level-arrow">→</span>
      <div 
        class="level-item" 
        :class="{ active: level === 'school' }" 
        @click="level = 'school'"
      >
        <span class="level-icon">🏫</span>
        <span class="level-label">学校级</span>
      </div>
      <span class="level-arrow">→</span>
      <div 
        class="level-item" 
        :class="{ active: level === 'class' }" 
        @click="level = 'class'"
      >
        <span class="level-icon">📚</span>
        <span class="level-label">班级级</span>
      </div>
    </div>

    <!-- 报表内容 -->
    <div class="report-content">
      <!-- 左侧：报表表格 -->
      <div class="report-table-wrapper">
        <div class="report-header">
          <h3>📊 {{ levelTitle }}龋患率统计报表</h3>
          <div class="report-actions">
            <button class="btn btn-success btn-sm" @click="handleExportExcel">
              <span class="icon">📊</span> 导出Excel
            </button>
            <button class="btn btn-danger btn-sm" @click="handleExportPDF">
              <span class="icon">📄</span> 导出PDF
            </button>
            <button class="btn btn-info btn-sm" @click="handlePreview">
              <span class="icon">👁️</span> 预览
            </button>
          </div>
        </div>

        <!-- 表格 -->
        <div class="table-scroll">
          <table class="report-table">
            <thead>
              <tr>
                <th class="col-index">#</th>
                <th class="col-name">{{ levelColumnName }}</th>
                <th class="col-number">监测人数</th>
                <th class="col-number">乳牙龋人数</th>
                <th class="col-number">恒牙龋人数</th>
                <th class="col-rate">乳牙龋率</th>
                <th class="col-rate">恒牙龋率</th>
                <th class="col-rate">涂氟覆盖率</th>
                <th class="col-trend">趋势</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in reportData" :key="index">
                <td class="col-index">{{ index + 1 }}</td>
                <td class="col-name">{{ item.name }}</td>
                <td class="col-number">{{ item.total.toLocaleString() }}</td>
                <td class="col-number">{{ item.decayedTeeth.toLocaleString() }}</td>
                <td class="col-number">{{ item.permanentDecayed.toLocaleString() }}</td>
                <td class="col-rate">
                  <span class="rate-value" :class="getRateClass(item.decayedRate)">
                    {{ item.decayedRate }}%
                  </span>
                </td>
                <td class="col-rate">
                  <span class="rate-value" :class="getRateClass(item.permanentRate)">
                    {{ item.permanentRate }}%
                  </span>
                </td>
                <td class="col-rate">
                  <span class="coverage-value">{{ item.coverage }}%</span>
                </td>
                <td class="col-trend">
                  <span class="trend-badge" :class="item.trend">
                    {{ item.trend === 'up' ? '📈 上升' : '📉 下降' }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- 报表汇总 -->
        <div class="report-summary">
          <div class="summary-left">
            <span class="summary-label">合计：</span>
            <span class="summary-text">监测 <strong>{{ summary.totalStudents }}</strong> 人</span>
            <span class="summary-divider">|</span>
            <span class="summary-text">乳牙龋 <strong>{{ summary.totalDecayed }}</strong> 人</span>
            <span class="summary-divider">|</span>
            <span class="summary-text">恒牙龋 <strong>{{ summary.totalPermanent }}</strong> 人</span>
          </div>
          <div class="summary-right">
            <span class="summary-label">总体龋患率：</span>
            <span class="summary-rate">{{ summary.overallRate }}%</span>
          </div>
        </div>
      </div>

      <!-- 右侧：趋势预测 -->
      <div class="trend-predict-wrapper">
        <div class="predict-header">
          <h3>🔮 趋势预测</h3>
          <span class="predict-badge">基于历史数据</span>
        </div>
        <div ref="predictChartRef" class="predict-chart"></div>
        <div class="predict-info">
          <div class="predict-item">
            <span class="predict-label">当前龋患率</span>
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

    <!-- 预览弹窗 -->
    <div v-if="showPreview" class="preview-modal" @click.self="showPreview = false">
      <div class="preview-content">
        <div class="preview-header">
          <h3>📄 报表预览</h3>
          <button class="preview-close" @click="showPreview = false">✕</button>
        </div>
        <div class="preview-body">
          <div class="preview-title">{{ levelTitle }}龋患率统计报表</div>
          <div class="preview-date">生成时间：{{ new Date().toLocaleString() }}</div>
          <table class="preview-table">
            <thead>
              <tr>
                <th>#</th>
                <th>{{ levelColumnName }}</th>
                <th>监测人数</th>
                <th>乳牙龋率</th>
                <th>恒牙龋率</th>
                <th>涂氟覆盖率</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in reportData" :key="index">
                <td>{{ index + 1 }}</td>
                <td>{{ item.name }}</td>
                <td>{{ item.total.toLocaleString() }}</td>
                <td>{{ item.decayedRate }}%</td>
                <td>{{ item.permanentRate }}%</td>
                <td>{{ item.coverage }}%</td>
              </tr>
            </tbody>
          </table>
          <div class="preview-summary">
            合计：监测 {{ summary.totalStudents }} 人，总体龋患率 {{ summary.overallRate }}%
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onBeforeUnmount } from 'vue';
import { message } from 'ant-design-vue';
import * as echarts from 'echarts';

const filters = reactive({ 
  dimension: 'school', 
  timeRange: '2026', 
  grade: 'all' 
});

const level = ref('province');
const showPreview = ref(false);

const reportData = ref([
  { name: '江苏省', total: 1250000, decayedTeeth: 445000, permanentDecayed: 361250, decayedRate: 35.6, permanentRate: 28.9, coverage: 68.5, trend: 'down' },
  { name: '浙江省', total: 980000, decayedTeeth: 333200, permanentDecayed: 274400, decayedRate: 34.0, permanentRate: 28.0, coverage: 70.0, trend: 'down' },
  { name: '广东省', total: 1560000, decayedTeeth: 624000, permanentDecayed: 468000, decayedRate: 40.0, permanentRate: 30.0, coverage: 65.0, trend: 'up' },
  { name: '山东省', total: 1100000, decayedTeeth: 418000, permanentDecayed: 330000, decayedRate: 38.0, permanentRate: 30.0, coverage: 62.0, trend: 'down' },
  { name: '河南省', total: 1080000, decayedTeeth: 410400, permanentDecayed: 302400, decayedRate: 38.0, permanentRate: 28.0, coverage: 60.0, trend: 'down' },
]);

const levelTitle = computed(() => {
  const map: Record<string, string> = { 
    province: '省级', 
    city: '市级', 
    district: '区县级', 
    school: '学校级', 
    class: '班级级' 
  };
  return map[level.value] || '省级';
});

const levelColumnName = computed(() => {
  const map: Record<string, string> = { 
    province: '省份', 
    city: '城市', 
    district: '区县', 
    school: '学校名称', 
    class: '班级名称' 
  };
  return map[level.value] || '省份';
});

const summary = computed(() => {
  const total = reportData.value.reduce((s, i) => s + i.total, 0);
  const decayed = reportData.value.reduce((s, i) => s + i.decayedTeeth, 0);
  const permanent = reportData.value.reduce((s, i) => s + i.permanentDecayed, 0);
  return { 
    totalStudents: total.toLocaleString(), 
    totalDecayed: decayed.toLocaleString(), 
    totalPermanent: permanent.toLocaleString(), 
    overallRate: ((decayed + permanent) / total * 100).toFixed(1) 
  };
});

const predictData = ref({ 
  currentRate: 42.3, 
  nextQuarter: 41.8, 
  nextYear: 40.2, 
  trend: 'down' 
});

const predictChartRef = ref<HTMLElement | null>(null);
let predictChart: echarts.ECharts | null = null;

const getRateClass = (rate: number) => {
  if (rate >= 35) return 'rate-high';
  if (rate >= 25) return 'rate-mid';
  return 'rate-low';
};

const handleSearch = () => { 
  message.success('报表生成中...'); 
};

const handleReset = () => { 
  filters.dimension = 'school'; 
  filters.timeRange = '2026'; 
  filters.grade = 'all'; 
  level.value = 'province'; 
  message.success('已重置筛选条件');
};

const handleExportExcel = () => { 
  message.success('正在导出 Excel...'); 
  setTimeout(() => message.success('Excel 文件导出成功！'), 1500); 
};

const handleExportPDF = () => { 
  message.success('正在导出 PDF...'); 
  setTimeout(() => message.success('PDF 文件导出成功！'), 1500); 
};

const handlePreview = () => { 
  showPreview.value = true; 
};

const initPredictChart = () => {
  if (!predictChartRef.value) return;
  if (predictChart && !predictChart.isDisposed()) predictChart.dispose();
  
  predictChart = echarts.init(predictChartRef.value);
  const years = ['2022', '2023', '2024', '2025', '2026', '2027Q1', '2027Q2', '2027Q3'];
  const actual = [45.5, 44.8, 43.6, 42.8, 42.3, null, null, null];
  const predicted = [null, null, null, null, 42.3, 41.8, 41.2, 40.2];
  
  predictChart.setOption({
    tooltip: { 
      trigger: 'axis',
      formatter: function(params: any) {
        let result = `<strong>${params[0].axisValue}</strong><br/>`;
        params.forEach((p: any) => {
          if (p.value !== null) {
            result += `${p.marker} ${p.seriesName}: ${p.value}%<br/>`;
          }
        });
        return result;
      }
    },
    legend: { 
      data: ['实际值', '预测值'], 
      bottom: 0,
      itemWidth: 14,
      itemHeight: 14
    },
    grid: { 
      left: '3%', 
      right: '4%', 
      top: '8%', 
      bottom: '25%', 
      containLabel: true 
    },
    xAxis: { 
      type: 'category', 
      data: years,
      axisLabel: { fontSize: 11 }
    },
    yAxis: { 
      type: 'value', 
      name: '龋患率(%)', 
      min: 35, 
      max: 50,
      nameTextStyle: { fontSize: 12 }
    },
    series: [
      { 
        name: '实际值', 
        type: 'line', 
        data: actual, 
        smooth: true, 
        lineStyle: { width: 3, color: '#409eff' }, 
        itemStyle: { color: '#409eff' },
        symbolSize: 8,
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: 'rgba(64, 158, 255, 0.3)' },
            { offset: 1, color: 'rgba(64, 158, 255, 0.02)' }
          ])
        }
      },
      { 
        name: '预测值', 
        type: 'line', 
        data: predicted, 
        smooth: true, 
        lineStyle: { width: 3, color: '#faad14', type: 'dashed' }, 
        itemStyle: { color: '#faad14' },
        symbolSize: 8,
        symbol: 'diamond'
      }
    ]
  });
  
  const resizeHandler = () => {
    if (predictChart && !predictChart.isDisposed()) {
      predictChart.resize();
    }
  };
  window.addEventListener('resize', resizeHandler);
};

onMounted(() => { 
  setTimeout(initPredictChart, 300); 
});

onBeforeUnmount(() => { 
  if (predictChart && !predictChart.isDisposed()) predictChart.dispose(); 
});
</script>

<style scoped>
.oral-statistics { 
  width: 100%; 
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
}

/* ===== 筛选栏 ===== */
.filter-bar {
  background: #fff;
  padding: 16px 24px;
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
  width: 150px;
}
.select-box select {
  width: 100%;
  padding: 8px 32px 8px 14px;
  border: 1px solid #dcdfe6;
  border-radius: 6px;
  appearance: none;
  outline: none;
  color: #303133;
  cursor: pointer;
  background: #fff;
  font-size: 14px;
  transition: border-color 0.3s;
}
.select-box select:hover { border-color: #409eff; }
.select-box select:focus { border-color: #409eff; box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.1); }
.select-box .arrow-down {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 10px;
  color: #c0c4cc;
  pointer-events: none;
}
.btn-group {
  display: flex;
  gap: 10px;
  margin-left: auto;
}
.btn-search, .btn-reset {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 22px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.3s;
}
.btn-search {
  background: #67c23a;
  color: white;
}
.btn-search:hover { background: #5daf34; }
.btn-search .icon { font-size: 14px; }
.btn-reset {
  background: #fff;
  border: 1px solid #dcdfe6;
  color: #606266;
}
.btn-reset:hover { 
  color: #409eff; 
  border-color: #c6e2ff; 
  background: #ecf5ff; 
}
.btn-reset .icon { font-size: 14px; }

/* ===== 五级统计导航 ===== */
.level-nav {
  display: flex;
  align-items: center;
  gap: 6px;
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
  padding: 6px 18px;
  border-radius: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  color: #606266;
  font-size: 13px;
  font-weight: 500;
}
.level-item:hover { 
  background: #f5f7fa; 
  color: #409eff; 
}
.level-item.active { 
  background: linear-gradient(135deg, #e8f4fd, #d4e8f7);
  color: #409eff; 
  font-weight: 600;
  box-shadow: 0 2px 8px rgba(64, 158, 255, 0.12);
}
.level-item .level-icon { font-size: 16px; }
.level-item .level-label { font-size: 13px; }
.level-arrow { color: #c0c4cc; font-size: 14px; }

/* ===== 报表内容 ===== */
.report-content { 
  display: grid; 
  grid-template-columns: 1fr 360px; 
  gap: 20px; 
}

/* ===== 报表表格 ===== */
.report-table-wrapper {
  background: #fff;
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid #ebeef5;
}
.report-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
  flex-wrap: wrap;
  gap: 12px;
}
.report-header h3 {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin: 0;
}
.report-actions { 
  display: flex; 
  gap: 8px; 
}
.btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 6px 14px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 500;
  transition: all 0.3s;
}
.btn-sm { padding: 5px 12px; font-size: 12px; }
.btn-success { background: #67c23a; color: #fff; }
.btn-success:hover { background: #85ce61; }
.btn-danger { background: #f56c6c; color: #fff; }
.btn-danger:hover { background: #f78989; }
.btn-info { background: #409eff; color: #fff; }
.btn-info:hover { background: #66b1ff; }
.btn .icon { font-size: 13px; }

/* 表格滚动 */
.table-scroll { 
  overflow-x: auto; 
  margin-bottom: 12px;
}

/* ===== 表格样式 ===== */
.report-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
  min-width: 800px;
}
.report-table thead th {
  background: #f5f7fa;
  padding: 10px 14px;
  text-align: center;
  font-weight: 600;
  color: #606266;
  border-bottom: 2px solid #e8ecf1;
  white-space: nowrap;
  font-size: 12px;
  letter-spacing: 0.3px;
}
.report-table tbody td {
  padding: 10px 14px;
  text-align: center;
  border-bottom: 1px solid #f0f0f0;
  color: #303133;
  vertical-align: middle;
}
.report-table tbody tr:hover td { 
  background: #f5f9ff; 
}
.report-table tbody tr:last-child td {
  border-bottom: none;
}

/* 列宽控制 */
.col-index { width: 44px; }
.col-name { text-align: left; font-weight: 500; color: #303133; }
.col-number { font-variant-numeric: tabular-nums; }
.col-rate { font-weight: 500; }
.col-trend { width: 80px; }

/* 数值样式 */
.rate-value {
  display: inline-block;
  padding: 2px 10px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 13px;
  min-width: 48px;
}
.rate-high { 
  color: #ff4d4f; 
  background: #fce4ec; 
}
.rate-mid { 
  color: #faad14; 
  background: #fff3e0; 
}
.rate-low { 
  color: #52c41a; 
  background: #e8f5e9; 
}

.coverage-value {
  font-weight: 600;
  color: #409eff;
}

/* 趋势徽章 */
.trend-badge {
  display: inline-block;
  padding: 2px 10px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}
.trend-badge.up {
  color: #ff4d4f;
  background: #fce4ec;
}
.trend-badge.down {
  color: #52c41a;
  background: #e8f5e9;
}

/* ===== 报表汇总 ===== */
.report-summary {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 4px 0;
  border-top: 2px solid #e8ecf1;
  margin-top: 4px;
  flex-wrap: wrap;
  gap: 8px;
}
.summary-left {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #606266;
}
.summary-label { color: #909399; }
.summary-text strong { color: #303133; }
.summary-divider { color: #dcdfe6; }
.summary-right {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
}
.summary-rate {
  font-size: 18px;
  font-weight: 700;
  color: #409eff;
}

/* ===== 趋势预测 ===== */
.trend-predict-wrapper {
  background: #fff;
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid #ebeef5;
}
.predict-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 14px;
}
.predict-header h3 {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin: 0;
}
.predict-badge {
  font-size: 11px;
  background: #e8f0fe;
  color: #409eff;
  padding: 2px 12px;
  border-radius: 12px;
  font-weight: 500;
}
.predict-chart { 
  width: 100%; 
  height: 200px; 
}
.predict-info {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px;
  margin-top: 14px;
  padding-top: 14px;
  border-top: 1px solid #ebeef5;
}
.predict-item {
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 6px 8px;
  border-radius: 6px;
  background: #f5f7fa;
}
.predict-label {
  font-size: 11px;
  color: #909399;
  letter-spacing: 0.3px;
}
.predict-value {
  font-size: 18px;
  font-weight: 700;
  color: #303133;
}
.predict-trend {
  font-size: 13px;
  font-weight: 600;
  padding: 2px 10px;
  border-radius: 12px;
  display: inline-block;
}
.predict-trend.up { 
  color: #ff4d4f; 
  background: #fce4ec;
}
.predict-trend.down { 
  color: #52c41a; 
  background: #e8f5e9;
}

/* ===== 预览弹窗 ===== */
.preview-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
}
.preview-content {
  background: #fff;
  border-radius: 12px;
  width: 820px;
  max-width: 95%;
  max-height: 90%;
  overflow: hidden;
  box-shadow: 0 8px 40px rgba(0,0,0,0.2);
}
.preview-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px;
  border-bottom: 1px solid #e8ecf1;
}
.preview-header h3 {
  font-size: 18px;
  font-weight: 600;
  color: #303133;
  margin: 0;
}
.preview-close {
  background: none;
  border: none;
  font-size: 22px;
  color: #c0c4cc;
  cursor: pointer;
  padding: 0 4px;
}
.preview-close:hover { color: #606266; }
.preview-body {
  padding: 24px;
  max-height: 500px;
  overflow-y: auto;
}
.preview-title {
  font-size: 20px;
  font-weight: 700;
  text-align: center;
  margin-bottom: 6px;
}
.preview-date {
  text-align: center;
  color: #909399;
  font-size: 13px;
  margin-bottom: 16px;
}
.preview-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
}
.preview-table th {
  background: #f5f7fa;
  padding: 8px 12px;
  text-align: center;
  font-weight: 600;
  border: 1px solid #e8ecf1;
}
.preview-table td {
  padding: 8px 12px;
  text-align: center;
  border: 1px solid #e8ecf1;
}
.preview-summary {
  margin-top: 14px;
  padding: 12px 16px;
  background: #f5f7fa;
  border-radius: 6px;
  text-align: center;
  font-size: 14px;
  font-weight: 500;
  color: #303133;
}

/* ===== 响应式 ===== */
@media (max-width: 1200px) { 
  .report-content { grid-template-columns: 1fr; } 
}
@media (max-width: 768px) {
  .filter-row { 
    flex-direction: column; 
    align-items: stretch; 
  }
  .select-box { width: 100%; }
  .btn-group { 
    margin-left: 0; 
    justify-content: center; 
  }
  .level-nav { 
    gap: 4px; 
    padding: 10px 12px; 
  }
  .level-item { 
    padding: 4px 12px; 
    font-size: 12px; 
  }
  .level-arrow { font-size: 10px; }
  .report-header { 
    flex-direction: column; 
    align-items: stretch; 
  }
  .report-actions { 
    justify-content: center; 
    flex-wrap: wrap;
  }
  .report-table-wrapper { 
    padding: 16px; 
  }
  .report-table { 
    font-size: 12px; 
    min-width: 700px;
  }
  .report-summary { 
    flex-direction: column; 
    align-items: stretch;
    text-align: center;
  }
  .summary-left { 
    flex-wrap: wrap; 
    justify-content: center;
  }
  .preview-content { width: 98%; }
  .predict-info { grid-template-columns: 1fr 1fr; }
}
</style>
