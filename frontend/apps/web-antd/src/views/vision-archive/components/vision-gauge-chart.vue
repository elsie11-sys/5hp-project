<script setup lang="ts">
import { computed, ref } from 'vue';

import { useEcharts } from '@vben/plugins/echarts';

const chartRef = ref<HTMLElement | null>(null);
const { renderEcharts } = useEcharts(chartRef);

const chartOption = computed(() => ({
  series: [
    {
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
          color: [
            [0.3, '#22c55e'],
            [0.6, '#eab308'],
            [1, '#ef4444'],
          ],
        },
      },
      pointer: {
        icon: 'path://M12.8,0.7l12,40.1H0.7L12.8,0.7z',
        length: '12%',
        width: 20,
        offsetCenter: [0, '-60%'],
        itemStyle: {
          color: 'auto',
        },
      },
      axisTick: {
        length: 12,
        lineStyle: {
          color: 'auto',
          width: 2,
        },
      },
      splitLine: {
        length: 20,
        lineStyle: {
          color: 'auto',
          width: 5,
        },
      },
      axisLabel: {
        color: '#464646',
        fontSize: 12,
        distance: -60,
      },
      title: {
        offsetCenter: [0, '-10%'],
        fontSize: 14,
      },
      detail: {
        fontSize: 28,
        offsetCenter: [0, '-35%'],
        valueAnimation: true,
        formatter: (value: number) => {
          if (value >= 60) return '异常';
          if (value >= 30) return '预警';
          return '达标';
        },
        color: 'auto',
      },
      data: [
        {
          value: 33.08,
          name: '视力不良率',
        },
      ],
    },
  ],
}));

renderEcharts(chartOption.value);
</script>

<template>
  <div ref="chartRef" class="w-full h-72"></div>
</template>
