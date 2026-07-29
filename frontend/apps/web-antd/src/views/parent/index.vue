<!-- apps/web-antd/src/views/parent/index.vue -->
<template>
  <div class="parent-app">
    <!-- ================= 顶部导航 ================= -->
    <div class="app-header">
      <div class="header-left">
        <span class="logo-icon">👨‍👩‍👧</span>
        <span class="logo-text">健康档案</span>
      </div>
      <div class="header-right">
        <span class="user-name">{{ parentInfo.name }}</span>
        <span class="user-avatar">👤</span>
      </div>
    </div>

    <!-- ================= 孩子切换 ================= -->
    <div class="child-switcher">
      <div 
        class="child-item" 
        v-for="child in children" 
        :key="child.id"
        :class="{ active: activeChildId === child.id }"
        @click="switchChild(child.id)"
      >
        <span class="child-avatar">{{ child.avatar }}</span>
        <span class="child-name">{{ child.name }}</span>
        <span class="child-school">{{ child.school }}</span>
        <span v-if="child.hasWarning" class="warning-dot">⚠️</span>
      </div>
    </div>

    <!-- ================= Tab切换 ================= -->
    <div class="tab-bar">
      <div 
        class="tab-item" 
        v-for="tab in tabs" 
        :key="tab.key"
        :class="{ active: activeTab === tab.key }"
        @click="switchTab(tab.key)"
      >
        <span class="tab-icon">{{ tab.icon }}</span>
        <span class="tab-label">{{ tab.label }}</span>
      </div>
    </div>

    <!-- ================= Tab内容 ================= -->
    <div class="tab-content">
      <!-- ===== 视力 Tab ===== -->
      <div v-show="activeTab === 'vision'" class="tab-panel">
        <div class="health-card">
          <div class="card-header">
            <span class="card-title">👁️ 最新视力检测</span>
            <span class="card-date">{{ currentChild?.vision?.lastCheck || '暂无数据' }}</span>
          </div>
          <div class="vision-result">
            <div class="result-item">
              <span class="result-label">左眼视力</span>
              <span class="result-value" :class="getVisionLevel(currentChild?.vision?.leftEye || 0)">
                {{ currentChild?.vision?.leftEye || '--' }}
              </span>
            </div>
            <div class="result-item">
              <span class="result-label">右眼视力</span>
              <span class="result-value" :class="getVisionLevel(currentChild?.vision?.rightEye || 0)">
                {{ currentChild?.vision?.rightEye || '--' }}
              </span>
            </div>
            <div class="result-item">
              <span class="result-label">视力等级</span>
              <span class="result-value level-tag" :class="getLevelClass(currentChild?.vision?.level || '')">
                {{ currentChild?.vision?.level || '未检测' }}
              </span>
            </div>
          </div>
          <div class="card-actions">
            <button class="btn btn-primary btn-sm" @click="handleViewReport('vision')">查看AI报告</button>
            <button class="btn btn-info btn-sm" @click="handleEditData('vision')">编辑</button>
          </div>
        </div>

        <div class="history-section">
          <h4>📊 视力变化趋势</h4>
          <div ref="visionTrendChartRef" class="chart-container"></div>
        </div>

        <div class="history-section">
          <h4>📋 历史检测记录</h4>
          <div class="history-list">
            <div class="history-item" v-for="record in currentChild?.vision?.history || []" :key="record.date">
              <span class="history-date">{{ record.date }}</span>
              <span class="history-data">左: {{ record.left }} | 右: {{ record.right }}</span>
              <span class="history-level" :class="getLevelClass(record.level)">{{ record.level }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 口腔 Tab ===== -->
      <div v-show="activeTab === 'oral'" class="tab-panel">
        <div class="health-card">
          <div class="card-header">
            <span class="card-title">🦷 最新口腔检测</span>
            <span class="card-date">{{ currentChild?.oral?.lastCheck || '暂无数据' }}</span>
          </div>
          <div class="oral-result">
            <div class="result-item">
              <span class="result-label">乳牙龋数</span>
              <span class="result-value" :class="currentChild?.oral?.decayedTeeth > 0 ? 'warning' : 'normal'">
                {{ currentChild?.oral?.decayedTeeth || 0 }} 颗
              </span>
            </div>
            <div class="result-item">
              <span class="result-label">恒牙龋数</span>
              <span class="result-value" :class="currentChild?.oral?.permanentDecayed > 0 ? 'warning' : 'normal'">
                {{ currentChild?.oral?.permanentDecayed || 0 }} 颗
              </span>
            </div>
            <div class="result-item">
              <span class="result-label">替牙情况</span>
              <span class="result-value">{{ currentChild?.oral?.toothStage || '未检测' }}</span>
            </div>
            <div class="result-item">
              <span class="result-label">颌面发育</span>
              <span class="result-value" :class="currentChild?.oral?.jawDevelopment === '正常' ? 'normal' : 'warning'">
                {{ currentChild?.oral?.jawDevelopment || '未检测' }}
              </span>
            </div>
          </div>
          <div class="card-actions">
            <button class="btn btn-primary btn-sm" @click="handleViewReport('oral')">查看AI报告</button>
            <button class="btn btn-info btn-sm" @click="handleEditData('oral')">编辑</button>
          </div>
        </div>

        <div class="history-section">
          <h4>📋 干预轨迹</h4>
          <div class="intervention-list">
            <div class="intervention-item" v-for="item in currentChild?.oral?.interventions || []" :key="item.date">
              <span class="intervention-icon">{{ getInterventionIcon(item.type) }}</span>
              <span class="intervention-name">{{ item.type }}</span>
              <span class="intervention-date">{{ item.date }}</span>
              <span class="intervention-status" :class="item.status === '已完成' ? 'done' : 'pending'">
                {{ item.status }}
              </span>
            </div>
          </div>
        </div>

        <div class="reminder-section">
          <h4>⏰ 服务提醒</h4>
          <div class="reminder-item" v-for="reminder in currentChild?.oral?.reminders || []" :key="reminder.id">
            <span class="reminder-icon">{{ reminder.icon }}</span>
            <span class="reminder-text">{{ reminder.text }}</span>
            <span class="reminder-date">{{ reminder.date }}</span>
          </div>
        </div>
      </div>

      <!-- ===== 心理 Tab ===== -->
      <div v-show="activeTab === 'mental'" class="tab-panel">
        <div class="health-card">
          <div class="card-header">
            <span class="card-title">🧠 心理健康评估</span>
            <span class="card-date">{{ currentChild?.mental?.lastCheck || '暂无数据' }}</span>
          </div>
          <div class="mental-result">
            <div class="result-item">
              <span class="result-label">心理状态</span>
              <span class="result-value" :class="getMentalStatusClass(currentChild?.mental?.status || '')">
                {{ currentChild?.mental?.status || '未评估' }}
              </span>
            </div>
            <div class="result-item">
              <span class="result-label">风险等级</span>
              <span class="result-value level-tag" :class="getRiskLevelClass(currentChild?.mental?.riskLevel || '')">
                {{ currentChild?.mental?.riskLevel || '未评估' }}
              </span>
            </div>
            <div class="result-item">
              <span class="result-label">主要问题</span>
              <span class="result-value">{{ currentChild?.mental?.mainIssue || '无' }}</span>
            </div>
          </div>
          <div class="card-actions">
            <button class="btn btn-primary btn-sm" @click="handleViewReport('mental')">查看报告</button>
            <button class="btn btn-info btn-sm" @click="handleEditData('mental')">编辑</button>
          </div>
        </div>

        <div v-if="currentChild?.mental?.riskLevel && currentChild?.mental?.riskLevel !== '低风险'" class="risk-alert">
          <div class="risk-header">
            <span class="risk-icon">⚠️</span>
            <span class="risk-title">风险提醒</span>
          </div>
          <div class="risk-body">
            <p><strong>风险类型：</strong>{{ currentChild?.mental?.riskType || '需关注' }}</p>
            <p><strong>专业建议：</strong>{{ currentChild?.mental?.suggestion || '建议咨询心理老师' }}</p>
            <p><strong>干预方案：</strong>{{ currentChild?.mental?.intervention || '已制定个性化方案' }}</p>
          </div>
        </div>

        <div class="history-section">
          <h4>📋 历史评估记录</h4>
          <div class="history-list">
            <div class="history-item" v-for="record in currentChild?.mental?.history || []" :key="record.date">
              <span class="history-date">{{ record.date }}</span>
              <span class="history-data">{{ record.status }}</span>
              <span class="history-level" :class="getRiskLevelClass(record.riskLevel)">{{ record.riskLevel }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 健康体重 Tab ===== -->
      <div v-show="activeTab === 'weight'" class="tab-panel">
        <div class="health-card">
          <div class="card-header">
            <span class="card-title">⚖️ 最新体重评估</span>
            <span class="card-date">{{ currentChild?.weight?.lastCheck || '暂无数据' }}</span>
          </div>
          <div class="weight-result">
            <div class="result-item">
              <span class="result-label">身高</span>
              <span class="result-value">{{ currentChild?.weight?.height || '--' }} cm</span>
            </div>
            <div class="result-item">
              <span class="result-label">体重</span>
              <span class="result-value">{{ currentChild?.weight?.weight || '--' }} kg</span>
            </div>
            <div class="result-item">
              <span class="result-label">BMI</span>
              <span class="result-value" :class="getBMIClass(currentChild?.weight?.bmi || 0)">
                {{ currentChild?.weight?.bmi || '--' }}
              </span>
            </div>
            <div class="result-item">
              <span class="result-label">体重状态</span>
              <span class="result-value level-tag" :class="getWeightStatusClass(currentChild?.weight?.status || '')">
                {{ currentChild?.weight?.status || '未评估' }}
              </span>
            </div>
          </div>
          <div class="card-actions">
            <button class="btn btn-primary btn-sm" @click="handleViewReport('weight')">查看报告</button>
            <button class="btn btn-info btn-sm" @click="handleEditData('weight')">编辑</button>
          </div>
        </div>

        <div class="history-section">
          <h4>📊 身高体重变化趋势</h4>
          <div ref="weightTrendChartRef" class="chart-container"></div>
        </div>

        <div class="history-section">
          <h4>📋 历史记录</h4>
          <div class="history-list">
            <div class="history-item" v-for="record in currentChild?.weight?.history || []" :key="record.date">
              <span class="history-date">{{ record.date }}</span>
              <span class="history-data">身高: {{ record.height }}cm | 体重: {{ record.weight }}kg</span>
              <span class="history-level" :class="getWeightStatusClass(record.status)">{{ record.status }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 骨骼健康 Tab ===== -->
      <div v-show="activeTab === 'bone'" class="tab-panel">
        <div class="health-card">
          <div class="card-header">
            <span class="card-title">🦴 骨骼健康评估</span>
            <span class="card-date">{{ currentChild?.bone?.lastCheck || '暂无数据' }}</span>
          </div>
          <div class="bone-result">
            <div class="result-item">
              <span class="result-label">骨密度</span>
              <span class="result-value" :class="getBoneDensityClass(currentChild?.bone?.density || '')">
                {{ currentChild?.bone?.density || '未检测' }}
              </span>
            </div>
            <div class="result-item">
              <span class="result-label">骨龄</span>
              <span class="result-value">{{ currentChild?.bone?.boneAge || '--' }} 岁</span>
            </div>
            <div class="result-item">
              <span class="result-label">身高预测</span>
              <span class="result-value">{{ currentChild?.bone?.heightPrediction || '--' }} cm</span>
            </div>
            <div class="result-item">
              <span class="result-label">骨骼状态</span>
              <span class="result-value level-tag" :class="getBoneStatusClass(currentChild?.bone?.status || '')">
                {{ currentChild?.bone?.status || '未评估' }}
              </span>
            </div>
          </div>
          <div class="card-actions">
            <button class="btn btn-primary btn-sm" @click="handleViewReport('bone')">查看报告</button>
            <button class="btn btn-info btn-sm" @click="handleEditData('bone')">编辑</button>
          </div>
        </div>

        <div v-if="currentChild?.bone?.suggestions && currentChild?.bone?.suggestions.length > 0" class="suggestion-section">
          <h4>💡 改善建议</h4>
          <div class="suggestion-item" v-for="(s, idx) in currentChild?.bone?.suggestions" :key="idx">
            <span class="suggestion-icon">✅</span>
            <span class="suggestion-text">{{ s }}</span>
          </div>
        </div>

        <div class="history-section">
          <h4>📋 历史评估记录</h4>
          <div class="history-list">
            <div class="history-item" v-for="record in currentChild?.bone?.history || []" :key="record.date">
              <span class="history-date">{{ record.date }}</span>
              <span class="history-data">骨密度: {{ record.density }}</span>
              <span class="history-level" :class="getBoneStatusClass(record.status)">{{ record.status }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ================= 底部导航 ================= -->
    <div class="bottom-nav">
      <div class="nav-item" @click="handlePrivacy">
        <span class="nav-icon">🔒</span>
        <span class="nav-label">隐私保护</span>
      </div>
      <div class="nav-item" @click="handleKnowledge">
        <span class="nav-icon">📚</span>
        <span class="nav-label">健康科普</span>
      </div>
      <div class="nav-item" @click="handleFeedback">
        <span class="nav-icon">💬</span>
        <span class="nav-label">反馈</span>
      </div>
    </div>

    <!-- ===== 编辑弹窗 ===== -->
    <Modal
      v-model:open="showEditModal"
      :title="editModalTitle"
      :footer="null"
      width="520px"
      @cancel="closeEditModal"
    >
      <div class="modal-form">
        <div class="form-group" v-for="field in editFields" :key="field.key">
          <label class="form-label">{{ field.label }}</label>
          <input 
            v-if="field.type === 'text' || field.type === 'number'"
            v-model="editFormData[field.key]" 
            :type="field.type"
            class="form-input"
          />
          <select v-else-if="field.type === 'select'" v-model="editFormData[field.key]" class="form-select">
            <option v-for="opt in field.options" :key="opt" :value="opt">{{ opt }}</option>
          </select>
        </div>
        <div class="form-actions">
          <button class="btn btn-default" @click="closeEditModal">取消</button>
          <button class="btn btn-primary" @click="submitEditData">保存</button>
        </div>
      </div>
    </Modal>

    <!-- ===== 隐私保护弹窗 ===== -->
    <Modal
      v-model:open="showPrivacyModal"
      title="🔒 隐私保护"
      :footer="null"
      width="480px"
      @cancel="showPrivacyModal = false"
    >
      <div class="privacy-content">
        <div class="privacy-item">
          <span class="privacy-icon">👁️</span>
          <div>
            <h4>仅查看自己孩子数据</h4>
            <p>您只能查看已绑定孩子的健康数据，无权查看其他学生信息</p>
          </div>
        </div>
        <div class="privacy-item">
          <span class="privacy-icon">🔐</span>
          <div>
            <h4>数据加密传输</h4>
            <p>所有数据传输采用HTTPS加密，确保信息安全</p>
          </div>
        </div>
        <div class="privacy-item">
          <span class="privacy-icon">📋</span>
          <div>
            <h4>操作留痕</h4>
            <p>所有查看和编辑操作均有日志记录，可追溯</p>
          </div>
        </div>
      </div>
    </Modal>

    <!-- ===== 健康科普弹窗 ===== -->
    <Modal
      v-model:open="showKnowledgeModal"
      title="📚 健康科普"
      :footer="null"
      width="520px"
      @cancel="showKnowledgeModal = false"
    >
      <div class="knowledge-content">
        <div class="knowledge-item" v-for="item in knowledgeList" :key="item.id">
          <span class="knowledge-icon">{{ item.icon }}</span>
          <div>
            <h4>{{ item.title }}</h4>
            <p>{{ item.content }}</p>
          </div>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onBeforeUnmount, nextTick } from 'vue';
import { message, Modal } from 'ant-design-vue';
import * as echarts from 'echarts';

// ================= 家长信息 =================
const parentInfo = ref({
  id: 'P001',
  name: '张明',
  phone: '138****8888'
});

// ================= 孩子数据（增加体重和骨骼数据） =================
const children = ref([
  {
    id: 1,
    name: '张小萌',
    avatar: '👧',
    school: '南京市第一中学',
    grade: '三年级',
    class: '二班',
    hasWarning: true,
    vision: {
      leftEye: 4.8,
      rightEye: 4.9,
      level: '轻度近视',
      lastCheck: '2026-07-10',
      history: [
        { date: '2026-07-10', left: 4.8, right: 4.9, level: '轻度近视' },
        { date: '2026-05-15', left: 4.9, right: 5.0, level: '正常' },
        { date: '2026-03-20', left: 5.0, right: 5.0, level: '正常' }
      ]
    },
    oral: {
      decayedTeeth: 2,
      permanentDecayed: 0,
      toothStage: '替牙期',
      jawDevelopment: '正常',
      lastCheck: '2026-07-08',
      interventions: [
        { type: '涂氟', date: '2026-06-15', status: '已完成' },
        { type: '窝沟封闭', date: '2026-07-05', status: '已完成' }
      ],
      reminders: [
        { id: 1, icon: '🦷', text: '下次涂氟时间：2026-10-15', date: '2026-10-15' },
        { id: 2, icon: '🏫', text: '学校口腔复查通知：2026-09-01', date: '2026-09-01' }
      ]
    },
    mental: {
      status: '需关注',
      riskLevel: '中风险',
      mainIssue: '学习焦虑',
      riskType: '学习焦虑-中度',
      suggestion: '建议适当减压，合理安排学习时间',
      intervention: '已制定个性化心理疏导方案',
      lastCheck: '2026-07-06',
      history: [
        { date: '2026-07-06', status: '需关注', riskLevel: '中风险' },
        { date: '2026-05-20', status: '稳定', riskLevel: '低风险' }
      ]
    },
    weight: {
      height: 138,
      weight: 32,
      bmi: 16.8,
      status: '正常',
      lastCheck: '2026-07-10',
      history: [
        { date: '2026-07-10', height: 138, weight: 32, status: '正常' },
        { date: '2026-05-15', height: 136, weight: 30.5, status: '正常' },
        { date: '2026-03-20', height: 134, weight: 29, status: '正常' }
      ]
    },
    bone: {
      density: '正常',
      boneAge: 9.5,
      heightPrediction: 165,
      status: '发育正常',
      lastCheck: '2026-07-09',
      suggestions: [
        '多参加户外运动，每天至少1小时',
        '保证充足钙质摄入，多喝牛奶',
        '保持良好坐姿站姿'
      ],
      history: [
        { date: '2026-07-09', density: '正常', status: '发育正常' },
        { date: '2026-05-15', density: '正常', status: '发育正常' }
      ]
    }
  },
  {
    id: 2,
    name: '张小阳',
    avatar: '👦',
    school: '南京市第一中学',
    grade: '一年级',
    class: '一班',
    hasWarning: false,
    vision: {
      leftEye: 5.0,
      rightEye: 5.0,
      level: '正常',
      lastCheck: '2026-07-09',
      history: [
        { date: '2026-07-09', left: 5.0, right: 5.0, level: '正常' }
      ]
    },
    oral: {
      decayedTeeth: 0,
      permanentDecayed: 0,
      toothStage: '乳牙期',
      jawDevelopment: '正常',
      lastCheck: '2026-07-07',
      interventions: [],
      reminders: [
        { id: 1, icon: '🦷', text: '建议涂氟保护牙齿', date: '2026-08-01' }
      ]
    },
    mental: {
      status: '稳定',
      riskLevel: '低风险',
      mainIssue: '无',
      riskType: '',
      suggestion: '',
      intervention: '',
      lastCheck: '2026-07-05',
      history: [
        { date: '2026-07-05', status: '稳定', riskLevel: '低风险' }
      ]
    },
    weight: {
      height: 120,
      weight: 22,
      bmi: 15.3,
      status: '正常',
      lastCheck: '2026-07-08',
      history: [
        { date: '2026-07-08', height: 120, weight: 22, status: '正常' }
      ]
    },
    bone: {
      density: '正常',
      boneAge: 7,
      heightPrediction: 175,
      status: '发育正常',
      lastCheck: '2026-07-07',
      suggestions: [
        '多参加户外运动',
        '保证充足钙质摄入'
      ],
      history: [
        { date: '2026-07-07', density: '正常', status: '发育正常' }
      ]
    }
  }
]);

const activeChildId = ref(1);
const activeTab = ref('vision');

// ================= 当前孩子 =================
const currentChild = computed(() => children.value.find(c => c.id === activeChildId.value));

// ================= Tab配置（增加体重和骨骼） =================
const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️' },
  { key: 'oral', label: '口腔健康', icon: '🦷' },
  { key: 'mental', label: '心理健康', icon: '🧠' },
  { key: 'weight', label: '健康体重', icon: '⚖️' },
  { key: 'bone', label: '骨骼健康', icon: '🦴' }
];

// ================= 图表引用 =================
const visionTrendChartRef = ref<HTMLElement | null>(null);
const weightTrendChartRef = ref<HTMLElement | null>(null);
let trendChart: echarts.ECharts | null = null;
let weightChart: echarts.ECharts | null = null;

// ================= 编辑弹窗 =================
const showEditModal = ref(false);
const editModalTitle = ref('');
const editTabType = ref('');
const editFields = ref<any[]>([]);
const editFormData = reactive<Record<string, any>>({});

// ================= 科普弹窗 =================
const showKnowledgeModal = ref(false);
const showPrivacyModal = ref(false);

// ================= 健康科普数据（增加体重和骨骼） =================
const knowledgeList = [
  { id: 1, icon: '👁️', title: '儿童近视防控', content: '每天户外活动2小时，保持正确读写姿势，每半年进行一次视力检查' },
  { id: 2, icon: '🦷', title: '口腔护理指南', content: '早晚刷牙各3分钟，使用含氟牙膏，每半年进行一次口腔检查' },
  { id: 3, icon: '🧠', title: '心理健康小贴士', content: '关注孩子情绪变化，建立良好沟通，保持规律作息和适度运动' },
  { id: 4, icon: '⚖️', title: '健康体重管理', content: '保持均衡饮食，减少高糖高脂食物，每天至少1小时户外运动' },
  { id: 5, icon: '🦴', title: '骨骼健康指南', content: '保证充足钙质和维生素D摄入，多参加跳跃类运动，保持良好姿势' }
];

// ================= 切换孩子 =================
const switchChild = (childId: number) => {
  activeChildId.value = childId;
  nextTick(() => {
    initCharts();
  });
};

// ================= 切换Tab =================
const switchTab = (tabKey: string) => {
  activeTab.value = tabKey;
  nextTick(() => {
    initCharts();
  });
};

// ================= 获取视力等级样式 =================
const getVisionLevel = (value: number) => {
  if (value >= 5.0) return 'normal';
  if (value >= 4.8) return 'warning';
  return 'danger';
};

const getLevelClass = (level: string) => {
  const map: Record<string, string> = {
    '正常': 'level-normal',
    '轻度近视': 'level-low',
    '中度近视': 'level-mid',
    '高度近视': 'level-high',
    '低风险': 'level-normal',
    '中风险': 'level-mid',
    '高风险': 'level-high',
    '正常体重': 'level-normal',
    '超重': 'level-mid',
    '肥胖': 'level-high',
    '偏瘦': 'level-low',
    '发育正常': 'level-normal',
    '发育偏慢': 'level-low'
  };
  return map[level] || '';
};

const getMentalStatusClass = (status: string) => {
  const map: Record<string, string> = {
    '稳定': 'status-stable',
    '需关注': 'status-warning',
    '高风险': 'status-danger'
  };
  return map[status] || '';
};

const getRiskLevelClass = (level: string) => {
  const map: Record<string, string> = {
    '低风险': 'level-normal',
    '中风险': 'level-mid',
    '高风险': 'level-high'
  };
  return map[level] || '';
};

// ================= 体重相关样式 =================
const getBMIClass = (bmi: number) => {
  if (bmi < 14) return 'danger';
  if (bmi < 15.5) return 'warning';
  if (bmi < 20) return 'normal';
  if (bmi < 22) return 'warning';
  return 'danger';
};

const getWeightStatusClass = (status: string) => {
  const map: Record<string, string> = {
    '正常': 'level-normal',
    '正常体重': 'level-normal',
    '超重': 'level-mid',
    '肥胖': 'level-high',
    '偏瘦': 'level-low',
    '轻度肥胖': 'level-mid',
    '中度肥胖': 'level-high'
  };
  return map[status] || '';
};

// ================= 骨骼相关样式 =================
const getBoneDensityClass = (density: string) => {
  const map: Record<string, string> = {
    '正常': 'normal',
    '偏低': 'warning',
    '低': 'danger'
  };
  return map[density] || '';
};

const getBoneStatusClass = (status: string) => {
  const map: Record<string, string> = {
    '发育正常': 'level-normal',
    '发育良好': 'level-normal',
    '发育偏慢': 'level-low',
    '需关注': 'level-mid',
    '发育异常': 'level-high'
  };
  return map[status] || '';
};

const getInterventionIcon = (type: string) => {
  const map: Record<string, string> = {
    '涂氟': '🦷',
    '窝沟封闭': '🔒',
    '复查': '🏥',
    '充填治疗': '💉'
  };
  return map[type] || '📋';
};

// ================= 查看报告 =================
const handleViewReport = (type: string) => {
  const name = currentChild.value?.name || '孩子';
  const map: Record<string, string> = {
    'vision': '视力',
    'oral': '口腔',
    'mental': '心理健康',
    'weight': '健康体重',
    'bone': '骨骼健康'
  };
  Modal.info({
    title: `📄 ${map[type] || ''}检测报告`,
    content: `${name}的${map[type] || ''}检测报告已生成，请查看详细数据。`,
    okText: '知道了'
  });
};

// ================= 编辑数据 =================
const handleEditData = (type: string) => {
  const child = currentChild.value;
  if (!child) return;
  
  editTabType.value = type;
  
  if (type === 'vision') {
    editModalTitle.value = `编辑${child.name}的视力数据`;
    editFields.value = [
      { key: 'leftEye', label: '左眼视力', type: 'number' },
      { key: 'rightEye', label: '右眼视力', type: 'number' },
      { key: 'level', label: '视力等级', type: 'select', options: ['正常', '轻度近视', '中度近视', '高度近视'] }
    ];
    editFormData.leftEye = child.vision.leftEye;
    editFormData.rightEye = child.vision.rightEye;
    editFormData.level = child.vision.level;
  } else if (type === 'oral') {
    editModalTitle.value = `编辑${child.name}的口腔数据`;
    editFields.value = [
      { key: 'decayedTeeth', label: '乳牙龋数', type: 'number' },
      { key: 'permanentDecayed', label: '恒牙龋数', type: 'number' },
      { key: 'toothStage', label: '替牙情况', type: 'select', options: ['乳牙期', '替牙期', '恒牙期'] }
    ];
    editFormData.decayedTeeth = child.oral.decayedTeeth;
    editFormData.permanentDecayed = child.oral.permanentDecayed;
    editFormData.toothStage = child.oral.toothStage;
  } else if (type === 'mental') {
    editModalTitle.value = `编辑${child.name}的心理健康数据`;
    editFields.value = [
      { key: 'status', label: '心理状态', type: 'select', options: ['稳定', '需关注', '高风险'] },
      { key: 'riskLevel', label: '风险等级', type: 'select', options: ['低风险', '中风险', '高风险'] },
      { key: 'mainIssue', label: '主要问题', type: 'text' }
    ];
    editFormData.status = child.mental.status;
    editFormData.riskLevel = child.mental.riskLevel;
    editFormData.mainIssue = child.mental.mainIssue;
  } else if (type === 'weight') {
    editModalTitle.value = `编辑${child.name}的体重数据`;
    editFields.value = [
      { key: 'height', label: '身高(cm)', type: 'number' },
      { key: 'weight', label: '体重(kg)', type: 'number' },
      { key: 'status', label: '体重状态', type: 'select', options: ['正常体重', '超重', '肥胖', '偏瘦'] }
    ];
    editFormData.height = child.weight.height;
    editFormData.weight = child.weight.weight;
    editFormData.status = child.weight.status;
  } else if (type === 'bone') {
    editModalTitle.value = `编辑${child.name}的骨骼健康数据`;
    editFields.value = [
      { key: 'density', label: '骨密度', type: 'select', options: ['正常', '偏低', '低'] },
      { key: 'boneAge', label: '骨龄(岁)', type: 'number' },
      { key: 'status', label: '骨骼状态', type: 'select', options: ['发育正常', '发育偏慢', '需关注'] }
    ];
    editFormData.density = child.bone.density;
    editFormData.boneAge = child.bone.boneAge;
    editFormData.status = child.bone.status;
  }
  
  showEditModal.value = true;
};

const closeEditModal = () => {
  showEditModal.value = false;
};

const submitEditData = () => {
  const child = currentChild.value;
  if (!child) return;
  
  const type = editTabType.value;
  
  if (type === 'vision') {
    child.vision.leftEye = editFormData.leftEye;
    child.vision.rightEye = editFormData.rightEye;
    child.vision.level = editFormData.level;
    child.vision.lastCheck = new Date().toISOString().slice(0, 10);
    child.vision.history.unshift({
      date: child.vision.lastCheck,
      left: editFormData.leftEye,
      right: editFormData.rightEye,
      level: editFormData.level
    });
  } else if (type === 'oral') {
    child.oral.decayedTeeth = editFormData.decayedTeeth;
    child.oral.permanentDecayed = editFormData.permanentDecayed;
    child.oral.toothStage = editFormData.toothStage;
    child.oral.lastCheck = new Date().toISOString().slice(0, 10);
  } else if (type === 'mental') {
    child.mental.status = editFormData.status;
    child.mental.riskLevel = editFormData.riskLevel;
    child.mental.mainIssue = editFormData.mainIssue;
    child.mental.lastCheck = new Date().toISOString().slice(0, 10);
    child.mental.history.unshift({
      date: child.mental.lastCheck,
      status: editFormData.status,
      riskLevel: editFormData.riskLevel
    });
  } else if (type === 'weight') {
    const height = editFormData.height;
    const weight = editFormData.weight;
    child.weight.height = height;
    child.weight.weight = weight;
    child.weight.bmi = Math.round((weight / ((height/100) ** 2)) * 10) / 10;
    child.weight.status = editFormData.status;
    child.weight.lastCheck = new Date().toISOString().slice(0, 10);
    child.weight.history.unshift({
      date: child.weight.lastCheck,
      height: height,
      weight: weight,
      status: editFormData.status
    });
  } else if (type === 'bone') {
    child.bone.density = editFormData.density;
    child.bone.boneAge = editFormData.boneAge;
    child.bone.status = editFormData.status;
    child.bone.lastCheck = new Date().toISOString().slice(0, 10);
    child.bone.history.unshift({
      date: child.bone.lastCheck,
      density: editFormData.density,
      status: editFormData.status
    });
  }
  
  message.success('数据已更新');
  closeEditModal();
  nextTick(() => {
    initCharts();
  });
};

// ================= 图表初始化 =================
const initCharts = () => {
  // 视力趋势图
  if (visionTrendChartRef.value) {
    if (trendChart && !trendChart.isDisposed()) {
      trendChart.dispose();
    }
    trendChart = echarts.init(visionTrendChartRef.value);
    const history = currentChild.value?.vision?.history || [];
    const dates = history.map(h => h.date).reverse();
    const leftData = history.map(h => h.left).reverse();
    const rightData = history.map(h => h.right).reverse();
    
    trendChart.setOption({
      tooltip: { trigger: 'axis' },
      legend: { data: ['左眼', '右眼'], bottom: 0 },
      grid: { left: '3%', right: '4%', top: '10%', bottom: '25%', containLabel: true },
      xAxis: { type: 'category', data: dates },
      yAxis: { type: 'value', name: '视力', min: 4.0, max: 5.2 },
      series: [
        { name: '左眼', type: 'line', data: leftData, smooth: true, lineStyle: { width: 2, color: '#1890ff' }, itemStyle: { color: '#1890ff' }, symbolSize: 6 },
        { name: '右眼', type: 'line', data: rightData, smooth: true, lineStyle: { width: 2, color: '#52c41a' }, itemStyle: { color: '#52c41a' }, symbolSize: 6 }
      ]
    });
  }

  // 体重趋势图
  if (weightTrendChartRef.value) {
    if (weightChart && !weightChart.isDisposed()) {
      weightChart.dispose();
    }
    weightChart = echarts.init(weightTrendChartRef.value);
    const history = currentChild.value?.weight?.history || [];
    const dates = history.map(h => h.date).reverse();
    const heightData = history.map(h => h.height).reverse();
    const weightData = history.map(h => h.weight).reverse();
    
    weightChart.setOption({
      tooltip: { trigger: 'axis' },
      legend: { data: ['身高(cm)', '体重(kg)'], bottom: 0 },
      grid: { left: '3%', right: '4%', top: '10%', bottom: '25%', containLabel: true },
      xAxis: { type: 'category', data: dates },
      yAxis: [
        { type: 'value', name: '身高(cm)', min: 80, max: 170 },
        { type: 'value', name: '体重(kg)', min: 0, max: 60 }
      ],
      series: [
        { name: '身高(cm)', type: 'line', data: heightData, smooth: true, lineStyle: { width: 2, color: '#1890ff' }, itemStyle: { color: '#1890ff' }, symbolSize: 6 },
        { name: '体重(kg)', type: 'line', yAxisIndex: 1, data: weightData, smooth: true, lineStyle: { width: 2, color: '#faad14' }, itemStyle: { color: '#faad14' }, symbolSize: 6 }
      ]
    });
  }
};

// ================= 底部导航 =================
const handlePrivacy = () => {
  showPrivacyModal.value = true;
};

const handleKnowledge = () => {
  showKnowledgeModal.value = true;
};

const handleFeedback = () => {
  Modal.info({
    title: '💬 反馈',
    content: '如有任何问题或建议，请联系学校老师或拨打客服热线：400-888-8888',
    okText: '知道了'
  });
};

// ================= 生命周期 =================
onMounted(() => {
  setTimeout(() => {
    initCharts();
  }, 300);
});

onBeforeUnmount(() => {
  if (trendChart && !trendChart.isDisposed()) {
    trendChart.dispose();
  }
  if (weightChart && !weightChart.isDisposed()) {
    weightChart.dispose();
  }
});
</script>

<style scoped>
/* ================= 全局 ================= */
.parent-app {
  max-width: 480px;
  margin: 0 auto;
  background: #f5f7fa;
  min-height: 100vh;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
  padding-bottom: 60px;
  position: relative;
}

/* ================= 顶部导航 ================= */
.app-header {
  background: linear-gradient(135deg, #1a3a5c, #2d6a9f);
  padding: 16px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  color: #fff;
  position: sticky;
  top: 0;
  z-index: 100;
}
.header-left {
  display: flex;
  align-items: center;
  gap: 10px;
}
.logo-icon { font-size: 24px; }
.logo-text { font-size: 18px; font-weight: 700; }
.header-right {
  display: flex;
  align-items: center;
  gap: 8px;
}
.user-name { font-size: 14px; opacity: 0.9; }
.user-avatar { font-size: 28px; }

/* ===== 孩子切换 ===== */
.child-switcher {
  display: flex;
  gap: 8px;
  padding: 12px 16px;
  background: #fff;
  border-bottom: 1px solid #ebeef5;
  overflow-x: auto;
}
.child-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  border-radius: 20px;
  background: #f5f7fa;
  cursor: pointer;
  transition: all 0.3s;
  white-space: nowrap;
  flex-shrink: 0;
  border: 2px solid transparent;
}
.child-item.active {
  background: #e8f4fd;
  border-color: #409eff;
}
.child-item .child-avatar { font-size: 18px; }
.child-item .child-name { font-weight: 600; color: #303133; }
.child-item .child-school { font-size: 12px; color: #909399; }
.child-item .warning-dot { font-size: 14px; margin-left: 4px; }

/* ===== Tab切换 ===== */
.tab-bar {
  display: flex;
  background: #fff;
  padding: 0 8px;
  border-bottom: 1px solid #ebeef5;
  overflow-x: auto;
}
.tab-item {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 10px 12px;
  cursor: pointer;
  transition: all 0.3s;
  color: #909399;
  border-bottom: 3px solid transparent;
  font-size: 13px;
}
.tab-item.active {
  color: #409eff;
  border-bottom-color: #409eff;
}
.tab-item .tab-icon { font-size: 16px; }
.tab-item .tab-label { font-weight: 500; }

/* ===== Tab内容 ===== */
.tab-content { padding: 12px 16px; }
.tab-panel { animation: fadeIn 0.3s ease; }
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

/* ===== 健康卡片 ===== */
.health-card {
  background: #fff;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  border: 1px solid #ebeef5;
  margin-bottom: 16px;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}
.card-title { font-size: 16px; font-weight: 600; color: #303133; }
.card-date { font-size: 12px; color: #909399; }

/* ===== 检测结果 ===== */
.vision-result, .oral-result, .mental-result, .weight-result, .bone-result {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}
.result-item {
  background: #f5f7fa;
  padding: 10px 12px;
  border-radius: 8px;
  text-align: center;
}
.result-item .result-label {
  display: block;
  font-size: 12px;
  color: #909399;
  margin-bottom: 4px;
}
.result-item .result-value {
  font-size: 18px;
  font-weight: 600;
  color: #303133;
}
.result-item .result-value.normal { color: #52c41a; }
.result-item .result-value.warning { color: #faad14; }
.result-item .result-value.danger { color: #ff4d4f; }

.level-tag {
  display: inline-block;
  padding: 2px 12px;
  border-radius: 12px;
  font-size: 14px;
}
.level-normal { background: #e8f5e9; color: #52c41a; }
.level-low { background: #fff3e0; color: #faad14; }
.level-mid { background: #fce4ec; color: #ff7a45; }
.level-high { background: #fce4ec; color: #ff4d4f; }

.status-stable { color: #52c41a; }
.status-warning { color: #faad14; }
.status-danger { color: #ff4d4f; }

.card-actions {
  display: flex;
  gap: 8px;
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px solid #f0f0f0;
}

/* ===== 历史记录 ===== */
.history-section {
  background: #fff;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  border: 1px solid #ebeef5;
  margin-bottom: 12px;
}
.history-section h4 {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
  margin: 0 0 12px 0;
}

.history-list {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.history-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: #f5f7fa;
  border-radius: 6px;
  font-size: 13px;
}
.history-date { color: #909399; }
.history-data { color: #303133; }
.history-level { font-size: 12px; padding: 1px 8px; border-radius: 10px; }

/* ===== 干预轨迹 ===== */
.intervention-list {
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.intervention-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 12px;
  background: #f5f7fa;
  border-radius: 6px;
  font-size: 13px;
}
.intervention-icon { font-size: 16px; }
.intervention-name { flex: 1; color: #303133; }
.intervention-date { color: #909399; font-size: 12px; }
.intervention-status { font-size: 11px; padding: 1px 8px; border-radius: 10px; }
.intervention-status.done { background: #e8f5e9; color: #52c41a; }
.intervention-status.pending { background: #fff3e0; color: #faad14; }

/* ===== 服务提醒 ===== */
.reminder-section {
  background: #fff;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  border: 1px solid #ebeef5;
}
.reminder-section h4 {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
  margin: 0 0 12px 0;
}
.reminder-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 12px;
  background: #f5f7fa;
  border-radius: 6px;
  font-size: 13px;
  margin-bottom: 6px;
}
.reminder-item:last-child { margin-bottom: 0; }
.reminder-icon { font-size: 16px; }
.reminder-text { flex: 1; color: #303133; }
.reminder-date { color: #909399; font-size: 12px; }

/* ===== 风险提醒 ===== */
.risk-alert {
  background: #fff3e0;
  border-radius: 12px;
  padding: 16px;
  border-left: 4px solid #faad14;
  margin-bottom: 16px;
}
.risk-header {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;
}
.risk-icon { font-size: 20px; }
.risk-title { font-weight: 600; color: #e6a23c; }
.risk-body p { margin: 4px 0; font-size: 14px; color: #606266; }
.risk-body p strong { color: #303133; }

/* ===== 改善建议 ===== */
.suggestion-section {
  background: #fff;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  border: 1px solid #ebeef5;
  margin-bottom: 12px;
}
.suggestion-section h4 {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
  margin: 0 0 10px 0;
}
.suggestion-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 6px 0;
  font-size: 13px;
  color: #606266;
}
.suggestion-icon { font-size: 16px; }
.suggestion-text { flex: 1; }

/* ===== 图表 ===== */
.chart-container { width: 100%; height: 180px; }

/* ===== 底部导航 ===== */
.bottom-nav {
  position: fixed;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  max-width: 480px;
  width: 100%;
  background: #fff;
  display: flex;
  border-top: 1px solid #ebeef5;
  padding: 8px 0;
  z-index: 100;
}
.nav-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2px;
  cursor: pointer;
  color: #909399;
  transition: color 0.3s;
  padding: 4px 0;
}
.nav-item:hover { color: #409eff; }
.nav-icon { font-size: 20px; }
.nav-label { font-size: 11px; }

/* ===== 弹窗表单 ===== */
.modal-form { padding: 8px 0; }
.form-group { margin-bottom: 16px; }
.form-label {
  display: block;
  font-size: 14px;
  color: #303133;
  font-weight: 500;
  margin-bottom: 4px;
}
.form-input, .form-select {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 14px;
  outline: none;
  background: #fff;
  transition: border-color 0.3s;
}
.form-input:focus, .form-select:focus {
  border-color: #409eff;
  box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.1);
}
.form-select {
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23606666' d='M6 8L1 3h10z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 12px center;
}
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding-top: 16px;
  border-top: 1px solid #ebeef5;
  margin-top: 4px;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 6px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 500;
  transition: all 0.3s;
}
.btn-sm { padding: 4px 12px; font-size: 12px; }
.btn-primary { background: #409eff; color: #fff; }
.btn-primary:hover { background: #66b1ff; }
.btn-info { background: #909399; color: #fff; }
.btn-info:hover { background: #a6a9ad; }
.btn-default { background: #fff; border: 1px solid #dcdfe6; color: #606266; }
.btn-default:hover { color: #409eff; border-color: #409eff; }

/* ===== 隐私保护 ===== */
.privacy-content { padding: 8px 0; }
.privacy-item {
  display: flex;
  gap: 12px;
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
}
.privacy-item:last-child { border-bottom: none; }
.privacy-icon { font-size: 24px; flex-shrink: 0; }
.privacy-item h4 { font-size: 14px; font-weight: 600; color: #303133; margin: 0 0 4px; }
.privacy-item p { font-size: 13px; color: #909399; margin: 0; }

/* ===== 健康科普 ===== */
.knowledge-content { padding: 8px 0; }
.knowledge-item {
  display: flex;
  gap: 12px;
  padding: 12px 0;
  border-bottom: 1px solid #f0f0f0;
}
.knowledge-item:last-child { border-bottom: none; }
.knowledge-icon { font-size: 28px; flex-shrink: 0; }
.knowledge-item h4 { font-size: 14px; font-weight: 600; color: #303133; margin: 0 0 4px; }
.knowledge-item p { font-size: 13px; color: #606266; margin: 0; }

/* ===== 响应式 ===== */
@media (max-width: 480px) {
  .parent-app { max-width: 100%; }
  .vision-result, .oral-result, .mental-result, .weight-result, .bone-result {
    grid-template-columns: 1fr 1fr;
  }
  .child-item .child-school { display: none; }
  .tab-item { font-size: 12px; padding: 8px 10px; }
  .tab-item .tab-label { display: none; }
}
</style>
