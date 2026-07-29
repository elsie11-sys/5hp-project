<script setup lang="ts">
import { computed, ref } from 'vue';

import { useEcharts } from '@vben/plugins/echarts';

defineProps<{
  type?: 'line' | 'bar' | 'progress';
}>();

const chartRef = ref<HTMLElement | null>(null);
const { renderEcharts } = useEcharts(chartRef);

const chartOption = computed(() => ({
  tooltip: {
    trigger: 'axis',
  },
  legend: {
    data: ['近视率', '视力不良率'],
    bottom: 0,
  },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '15%',
    top: '10%',
    containLabel: true,
  },
  xAxis: {
    type: 'category',
    boundaryGap: false,
    data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
  },
  yAxis: {
    type: 'value',
    axisLabel: {
      formatter: '{value}%',
    },
  },
  series: [
    {
      name: '近视率',
      type: 'line',
      smooth: true,
      data: [28.5, 29.1, 29.8, 30.2, 30.8, 31.5, 30.2, 29.5, 31.2, 32.1, 32.8, 33.1],
      areaStyle: {
        color: 'rgba(59, 130, 246, 0.2)',
      },
      lineStyle: {
        color: '#3b82f6',
      },
      itemStyle: {
        color: '#3b82f6',
      },
    },
    {
      name: '视力不良率',
      type: 'line',
      smooth: true,
      data: [32.1, 32.5, 33.0, 33.4, 33.8, 34.2, 33.5, 32.8, 34.0, 34.8, 35.2, 35.5],
      areaStyle: {
        color: 'rgba(239, 68, 68, 0.2)',
      },
      lineStyle: {
        color: '#ef4444',
      },
      itemStyle: {
        color: '#ef4444',
      },
    },
  ],
}));

renderEcharts(chartOption.value);
</script>

<template>
  <div ref="chartRef" class="w-full h-80"></div>
</template>
