<script setup lang="ts">
import { computed, ref } from 'vue';

import { useEcharts } from '@vben/plugins/echarts';

const chartRef = ref<HTMLElement | null>(null);
const { renderEcharts } = useEcharts(chartRef);

const chartOption = computed(() => ({
  tooltip: {
    trigger: 'axis',
    axisPointer: {
      type: 'shadow',
    },
  },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '3%',
    top: '3%',
    containLabel: true,
  },
  xAxis: {
    type: 'category',
    data: ['一年级', '二年级', '三年级', '四年级', '五年级', '六年级', '初一', '初二', '初三', '高一', '高二', '高三'],
    axisLabel: {
      rotate: 45,
      fontSize: 10,
    },
  },
  yAxis: {
    type: 'value',
    axisLabel: {
      formatter: '{value}%',
    },
  },
  series: [
    {
      name: '视力不良率',
      type: 'bar',
      data: [22.5, 25.8, 30.2, 35.6, 40.1, 45.2, 48.5, 52.3, 58.6, 62.5, 68.2, 72.8],
      itemStyle: {
        color: (params: any) => {
          const colorList = ['#22c55e', '#84cc16', '#eab308', '#f97316', '#ef4444', '#dc2626', '#b91c1c', '#991b1b', '#7f1d1d', '#650000', '#4a0000', '#330000'];
          return colorList[params.dataIndex];
        },
      },
    },
  ],
}));

renderEcharts(chartOption.value);
</script>

<template>
  <div ref="chartRef" class="w-full h-72"></div>
</template>
