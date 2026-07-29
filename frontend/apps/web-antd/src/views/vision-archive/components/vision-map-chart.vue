<script setup lang="ts">
import { computed, ref } from 'vue';

import { useEcharts } from '@vben/plugins/echarts';

const chartRef = ref<HTMLElement | null>(null);
const { renderEcharts } = useEcharts(chartRef);

// 模拟各省数据
const provinceData = [
  { name: '北京', value: 45.2 },
  { name: '上海', value: 47.8 },
  { name: '广东', value: 42.1 },
  { name: '江苏', value: 38.5 },
  { name: '浙江', value: 40.3 },
  { name: '四川', value: 35.6 },
  { name: '河南', value: 32.8 },
  { name: '山东', value: 36.4 },
  { name: '湖北', value: 34.2 },
  { name: '湖南', value: 33.9 },
  { name: '安徽', value: 31.5 },
  { name: '福建', value: 37.2 },
  { name: '江西', value: 30.8 },
  { name: '重庆', value: 39.1 },
  { name: '陕西', value: 34.5 },
];

const chartOption = computed(() => ({
  tooltip: {
    trigger: 'item',
    formatter: '{b}: {c}%',
  },
  visualMap: {
    min: 25,
    max: 50,
    left: 'left',
    top: 'bottom',
    text: ['高', '低'],
    calculable: true,
    inRange: {
      color: ['#e0f2fe', '#0ea5e9', '#0284c7', '#0369a1'],
    },
  },
  series: [
    {
      name: '视力不良率',
      type: 'pie',
      radius: ['35%', '60%'],
      center: ['50%', '50%'],
      data: provinceData.sort((a, b) => b.value - a.value),
      emphasis: {
        itemStyle: {
          shadowBlur: 10,
          shadowOffsetX: 0,
          shadowColor: 'rgba(0, 0, 0, 0.5)',
        },
      },
      label: {
        formatter: '{b}: {c}%',
        fontSize: 10,
      },
    },
  ],
}));

renderEcharts(chartOption.value);
</script>

<template>
  <div ref="chartRef" class="w-full h-80"></div>
</template>
