<script lang="ts" setup>
import { ref, reactive } from 'vue';

// ✅ 删除报错的 import { useVbenAntdv } from '#/adapter';
// Vben 5 的组件是全局注册的，不需要额外导入适配器。

const formState = reactive({
  reportType: 'monthly',
  year: '2024',
  month: '12',
  region: '',
  format: 'pdf',
});

// 1. 定义用于 <VbenForm> 的表单配置
const formConfig = {
  wrapperCol: { span: 24 },
  commonConfig: {
    componentProps: {
      style: { width: '100%' }
    }
  },
  fields: [
    {
      label: '报表类型',
      fieldName: 'reportType',
      component: 'Select',
      componentProps: {
        options: [
          { label: '日报', value: 'daily' },
          { label: '周报', value: 'weekly' },
          { label: '月报', value: 'monthly' },
          { label: '季报', value: 'quarterly' },
          { label: '年报', value: 'yearly' },
        ]
      }
    },
    {
      label: '统计年度',
      fieldName: 'year',
      component: 'Select',
      componentProps: {
        options: [
          { label: '2024年', value: '2024' },
          { label: '2023年', value: '2023' }
        ]
      }
    },
    {
      label: '统计周期',
      fieldName: 'month',
      component: 'Select',
      componentProps: {
        options: Array.from({ length: 12 }, (_, i) => ({ label: `${i + 1}月`, value: String(i + 1) }))
      }
    },
    {
      label: '区域选择',
      fieldName: 'region',
      component: 'TreeSelect',
      componentProps: {
        placeholder: '请选择区域',
        treeData: [
          { 
            label: '全国', 
            value: '', 
            children: [
              { label: '广东省', value: 'gd' },
              { label: '浙江省', value: 'zj' },
              { label: '江苏省', value: 'js' },
            ]
          }
        ]
      }
    },
    {
      label: '导出格式',
      fieldName: 'format',
      component: 'Select',
      componentProps: {
        options: [
          { label: 'PDF', value: 'pdf' },
          { label: 'Excel', value: 'excel' },
          { label: 'Word', value: 'word' }
        ]
      }
    }
  ]
};

const reportTemplates = ref([
  {
    id: 1,
    name: '视力筛查汇总表',
    description: '统计各地区、学校视力筛查总体情况',
    type: 'monthly',
  },
  {
    id: 2,
    name: '视力不良率分析表',
    description: '分析视力不良率变化趋势及影响因素',
    type: 'quarterly',
  },
  {
    id: 3,
    name: '重点关注学生清单',
    description: '导出视力异常需重点关注的学生列表',
    type: 'weekly',
  },
  {
    id: 4,
    name: '学校年度视力报告',
    description: '各学校年度视力健康状况综合报告',
    type: 'yearly',
  },
  {
    id: 5,
    name: '区域对比分析表',
    description: '不同区域视力健康指标对比分析',
    type: 'monthly',
  },
  {
    id: 6,
    name: '视力改善追踪表',
    description: '跟踪学生视力改善情况',
    type: 'quarterly',
  },
]);

// 历史报表的表格列
const historyColumns = [
  { title: '报表名称', dataIndex: 'name' },
  { title: '生成时间', dataIndex: 'createTime' },
  { title: '生成人', dataIndex: 'creator' },
  { title: '格式', dataIndex: 'format' },
  { 
    title: '状态', 
    dataIndex: 'status',
  },
  { 
    title: '操作', 
    dataIndex: 'action',
    width: 200
  }
];

// 历史数据（假数据）
const historyData = ref([
  { name: '月报-2024-12', createTime: '2024-12-31 18:00', creator: '管理员', format: 'PDF', status: '已完成' },
  { name: '月报-2024-11', createTime: '2024-11-30 18:00', creator: '管理员', format: 'Excel', status: '已完成' },
]);

const generateReport = () => {
  console.log('生成报表:', formState);
};

const previewReport = (id: number) => {
  console.log('预览报表:', id);
};
</script>

<template>
  <div class="p-5">
    <!-- 1. 报表生成配置 (使用 VbenForm 替代 a-select) -->
    <div class="bg-white dark:bg-gray-800 rounded-lg p-6 mb-4 shadow-sm">
      <h3 class="text-lg font-medium text-gray-700 dark:text-gray-200 mb-4">
        报表生成
      </h3>
      
      <VbenForm 
        :config="formConfig" 
        v-model:model-value="formState"
        class="mb-4"
      />
      
      <div class="flex justify-end">
        <a-button type="primary" @click="generateReport">
          生成报表
        </a-button>
      </div>
    </div>

    <!-- 2. 报表模板 -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-700 dark:text-gray-200">
          报表模板
        </h3>
      </div>
      <div class="p-4">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div
            v-for="template in reportTemplates"
            :key="template.id"
            class="border border-gray-200 dark:border-gray-700 rounded-lg p-4 hover:border-primary cursor-pointer transition"
            @click="previewReport(template.id)"
          >
            <div class="flex items-start justify-between">
              <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
                <span class="text-xl">📊</span>
              </div>
              <a-tag v-if="template.type === 'monthly'" color="blue">月度</a-tag>
              <a-tag v-else-if="template.type === 'yearly'" color="purple">年度</a-tag>
              <a-tag v-else color="green">周期</a-tag>
            </div>
            <div class="mt-3">
              <div class="font-medium text-gray-800 dark:text-gray-200">
                {{ template.name }}
              </div>
              <div class="text-sm text-gray-500 mt-1">
                {{ template.description }}
              </div>
            </div>
            <div class="mt-4 flex gap-2">
              <a-button size="small" type="primary">预览</a-button>
              <a-button size="small">下载</a-button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 3. 历史报表 (使用 VbenTable 替代 a-table) -->
    <div class="mt-5 bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-700 dark:text-gray-200">
          历史报表
        </h3>
      </div>
      <div class="p-4">
        <VbenTable 
          :columns="historyColumns" 
          :data-source="historyData" 
          :pagination="{ total: 20 }"
        >
          <template #status="{ row }">
            <a-tag color="green">{{ row.status }}</a-tag>
          </template>

          <template #action="{ row }">
            <a-space>
              <a-button type="link" size="small">预览</a-button>
              <a-button type="link" size="small">下载</a-button>
              <a-button type="link" size="small" danger>删除</a-button>
            </a-space>
          </template>
        </VbenTable>
      </div>
    </div>
  </div>
</template>
