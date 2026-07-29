<script setup lang="ts">
/**
 * 动态配置卡片组件
 * 支持根据配置动态渲染不同类型的图表和数据
 */
import { computed, ref, watch } from 'vue';

import { useEcharts } from '@vben/plugins/echarts';

// 卡片配置接口
export interface CardConfig {
  id: string;
  title: string;
  type: 'line' | 'bar' | 'pie' | 'gauge' | 'table' | 'stat';
  dataSource: 'api' | 'static';
  apiUrl?: string;
  staticData?: any;
  refreshInterval?: number;
  colSpan?: 1 | 2 | 3 | 4;
  options?: Record<string, any>;
}

const props = withDefaults(defineProps<{
  config: CardConfig;
  loading?: boolean;
}>(), {
  loading: false,
});

const emit = defineEmits<{
  (e: 'refresh', id: string): void;
  (e: 'click', id: string): void;
}>();

const chartRef = ref<HTMLElement | null>(null);
const { renderEcharts } = useEcharts(chartRef);

const chartOption = computed(() => {
  if (!props.config.staticData) return {};

  const data = props.config.staticData;
  const type = props.config.type;

  switch (type) {
    case 'line':
      return {
        tooltip: { trigger: 'axis' },
        grid: { left: '3%', right: '4%', bottom: '3%', top: '10%', containLabel: true },
        xAxis: { type: 'category', data: data.xAxis || [] },
        yAxis: { type: 'value' },
        series: data.series || [],
        ...props.config.options,
      };

    case 'bar':
      return {
        tooltip: { trigger: 'axis' },
        grid: { left: '3%', right: '4%', bottom: '3%', top: '3%', containLabel: true },
        xAxis: { type: 'category', data: data.categories || [] },
        yAxis: { type: 'value' },
        series: data.series || [],
        ...props.config.options,
      };

    case 'pie':
      return {
        tooltip: { trigger: 'item', formatter: '{b}: {c}%' },
        legend: { bottom: 0 },
        series: [{
          type: 'pie',
          radius: '60%',
          center: ['50%', '45%'],
          data: data.items || [],
          label: { show: true, formatter: '{b}\n{c}%' },
        }],
        ...props.config.options,
      };

    case 'gauge':
      return {
        series: [{
          type: 'gauge',
          startAngle: 180,
          endAngle: 0,
          center: ['50%', '75%'],
          radius: '90%',
          min: 0,
          max: 100,
          splitNumber: 4,
          axisLine: {
            lineStyle: {
              width: 20,
              color: [[0.3, '#22c55e'], [0.6, '#eab308'], [1, '#ef4444']],
            },
          },
          data: data.items || [],
          ...props.config.options,
        }],
      };

    default:
      return {};
  }
});

watch(chartOption, (opt) => {
  if (chartRef.value && props.config.type !== 'table' && props.config.type !== 'stat') {
    renderEcharts(opt);
  }
}, { immediate: true });

const handleRefresh = () => {
  emit('refresh', props.config.id);
};

const handleClick = () => {
  emit('click', props.config.id);
};

// 计算列宽
const colSpanClass = computed(() => {
  const span = props.config.colSpan || 1;
  return `col-span-1 md:col-span-${span}`;
});
</script>

<template>
  <div
    :class="colSpanClass"
    class="bg-white dark:bg-gray-800 rounded-lg shadow hover:shadow-lg transition-shadow cursor-pointer"
    @click="handleClick"
  >
    <!-- 表头 -->
    <div class="p-4 border-b border-gray-200 dark:border-gray-700 flex justify-between items-center">
      <h3 class="font-medium text-gray-700 dark:text-gray-200">{{ config.title }}</h3>
      <a-button type="text" size="small" @click.stop="handleRefresh">
        <template #icon>
          <span :class="{ 'animate-spin': loading }">🔄</span>
        </template>
      </a-button>
    </div>

    <!-- 内容区域 -->
    <div class="p-4">
      <!-- 统计卡片 -->
      <template v-if="config.type === 'stat'">
        <div class="text-center">
          <div class="text-3xl font-bold text-primary">
            {{ config.staticData?.value || 0 }}
          </div>
          <div class="text-sm text-gray-500 mt-1">
            {{ config.staticData?.label || '' }}
          </div>
          <div v-if="config.staticData?.trend" class="text-sm mt-2" :class="config.staticData.trend > 0 ? 'text-red-500' : 'text-green-500'">
            {{ config.staticData.trend > 0 ? '↑' : '↓' }} {{ Math.abs(config.staticData.trend) }}%
          </div>
        </div>
      </template>

      <!-- 表格卡片 -->
      <template v-else-if="config.type === 'table'">
        <table class="w-full text-sm">
          <thead>
            <tr>
              <th v-for="col in config.staticData?.columns || []" :key="col" class="text-left py-2 text-gray-500">
                {{ col }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(row, idx) in config.staticData?.rows || []" :key="idx" class="border-t">
              <td v-for="(cell, cidx) in row" :key="cidx" class="py-2">
                {{ cell }}
              </td>
            </tr>
          </tbody>
        </table>
      </template>

      <!-- 图表卡片 -->
      <template v-else>
        <div v-if="loading" class="h-72 flex items-center justify-center">
          <a-spin />
        </div>
        <div v-else ref="chartRef" class="w-full" :style="{ height: config.type === 'gauge' ? '200px' : '280px' }"></div>
      </template>
    </div>
  </div>
</template>
