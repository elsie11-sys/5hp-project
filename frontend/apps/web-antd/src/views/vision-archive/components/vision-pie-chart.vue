<script setup lang="ts">
import { computed, ref } from 'vue';

import { useEcharts } from '@vben/plugins/echarts';

const chartRef = ref<HTMLElement | null>(null);
const { renderEcharts } = useEcharts(chartRef);

const chartOption = computed(() => ({
  tooltip: {
    trigger: 'item',
    formatter: '{b}: {c}%',
  },
  legend: {
    orient: 'vertical',
    left: 'left',
    bottom: 0,
  },
  color: ['#22c55e', '#84cc16', '#eab308', '#f97316', '#ef4444'],
  series: [
    {
      name: '视力等级',
      type: 'pie',
      radius: '65%',
      center: ['50%', '45%'],
      data: [
        { value: 35, name: '正常' },
        { value: 25, name: '轻度近视' },
        { value: 20, name: '中度近视' },
        { value: 12, name: '高度近视' },
        { value: 8, name: '重度近视' },
      ],
      emphasis: {
        itemStyle: {
          shadowBlur: 10,
          shadowOffsetX: 0,
          shadowColor: 'rgba(0, 0, 0, 0.5)',
        },
      },
      label: {
        show: true,
        formatter: '{b}\n{c}%',
      },
    },
  ],
}));

renderEcharts(chartOption.value);
</script>

<template>
  <div ref="chartRef" class="w-full h-72"></div>
</template>
