<template>
  <div class="statistics-report" :class="{ 'theme-light': isLight }">
    <!-- 背景装饰 -->
    <div class="bg-decor" aria-hidden="true">
      <div class="bg-grid"></div>
      <div class="bg-glow bg-glow-1"></div>
      <div class="bg-glow bg-glow-2"></div>
    </div>

    <!-- 扫描线 -->
    <div class="scan-line"></div>

    <div class="page-container">
      <!-- 页面标题 -->
      <div class="page-header">
        <div class="header-deco header-deco-l">
          <span class="hd-line"></span>
          <span class="hd-dot"></span>
        </div>
        <div class="header-main">
          <h1 class="page-title">
            <span class="title-bracket">【</span>
            <span class="title-text">📊 统计分析与报表</span>
            <span class="title-bracket">】</span>
            <span class="title-shine"></span>
          </h1>
        </div>
        <div class="header-deco header-deco-r">
          <span class="hd-dot"></span>
          <span class="hd-line"></span>
        </div>
      </div>

      <!-- Tab切换栏 -->
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
          <span v-if="tab.badge" class="tab-badge">{{ tab.badge }}</span>
        </div>
      </div>

      <!-- Tab内容区 -->
      <div class="tab-content">
        <!-- 视力健康 -->
        <div v-show="activeTab === 'vision'" class="tab-panel">
          <VisionReport />
        </div>

        <!-- 口腔健康 -->
        <div v-show="activeTab === 'oral'" class="tab-panel">
          <OralReport />
        </div>

        <!-- 心理健康 -->
        <div v-show="activeTab === 'mental'" class="tab-panel">
          <MentalReport />
        </div>

        <!-- 健康体重 -->
        <div v-show="activeTab === 'weight'" class="tab-panel">
          <WeightReport />
        </div>

        <!-- 骨骼健康 -->
        <div v-show="activeTab === 'bone'" class="tab-panel">
          <BoneReport />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue';
import VisionReport from './vision.vue';
import OralReport from './oral.vue';
import MentalReport from './mental.vue';
import WeightReport from './weight.vue';
import BoneReport from './bone.vue';

// ================= Theme Detection =================
const isLight = ref(false);
let themeObserver = null;

const applySystemTheme = () => {
  isLight.value = !document.documentElement.classList.contains('dark');
};

const observeTheme = () => {
  themeObserver = new MutationObserver(() => {
    isLight.value = !document.documentElement.classList.contains('dark');
  });
  themeObserver.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
};

onMounted(() => {
  applySystemTheme();
  observeTheme();
});

onBeforeUnmount(() => {
  if (themeObserver) {
    themeObserver.disconnect();
    themeObserver = null;
  }
});

// ================= Tab配置 =================
const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️', badge: '' },
  { key: 'oral', label: '口腔健康', icon: '🦷', badge: '' },
  { key: 'mental', label: '心理健康', icon: '🧠', badge: '' },
  { key: 'weight', label: '健康体重', icon: '⚖️', badge: '' },
  { key: 'bone', label: '骨骼健康', icon: '🦴', badge: '' }
];

const activeTab = ref('vision');

// ================= Tab切换 =================
const switchTab = (tabKey) => {
  activeTab.value = tabKey;
};
</script>

<style scoped>
/* ===== CSS Variables ===== */
.statistics-report {
  /* Dark mode - sci-fi tech blue */
  --bg-page: #060d1f;
  --bg-card: rgba(10, 22, 50, 0.82);
  --bg-soft: rgba(56, 189, 248, 0.04);
  --bg-hover: rgba(56, 189, 248, 0.08);
  --border: rgba(56, 189, 248, 0.22);
  --border-strong: rgba(56, 189, 248, 0.45);
  --border-soft: rgba(56, 189, 248, 0.12);
  --text: #e2e8f0;
  --text-strong: #f1f5f9;
  --text-dim: #94a3b8;
  --text-muted: #64748b;
  --primary: #38bdf8;
  --primary-hover: #7dd3fc;
  --primary-bg: rgba(56, 189, 248, 0.12);
  --secondary: #a78bfa;
  --accent: #f472b6;
  --success: #34d399;
  --warning: #fbbf24;
  --danger: #fb7185;
  --glow: rgba(56, 189, 248, 0.45);
  --shadow: 0 4px 24px rgba(0, 0, 0, 0.35), 0 0 0 1px rgba(56, 189, 248, 0.06);
  --shadow-card: 0 8px 32px rgba(0, 0, 0, 0.4), 0 0 0 1px rgba(56, 189, 248, 0.08);
  --scrollbar-thumb: rgba(56, 189, 248, 0.3);

  min-height: 100vh;
  width: 100%;
  position: relative;
  background: var(--bg-page);
  background-size: cover;
  background-attachment: fixed;
  color: var(--text);
  font-family: 'Inter', 'PingFang SC', 'Microsoft YaHei', system-ui, sans-serif;
}

.statistics-report > * {
  position: relative;
  z-index: 2;
}

.statistics-report.theme-light {
  /* Light mode - light sci-fi with cyan accents */
  --bg-page: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 40%, #f0f7ff 100%);
  --bg-card: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  --bg-soft: rgba(8, 145, 178, 0.04);
  --bg-hover: rgba(8, 145, 178, 0.07);
  --border: rgba(8, 145, 178, 0.2);
  --border-strong: rgba(8, 145, 178, 0.38);
  --border-soft: rgba(8, 145, 178, 0.12);
  --text: #334155;
  --text-strong: #0f172a;
  --text-dim: #64748b;
  --text-muted: #94a3b8;
  --primary: #0891b2;
  --primary-hover: #0e7490;
  --primary-bg: rgba(8, 145, 178, 0.1);
  --secondary: #7c3aed;
  --accent: #db2777;
  --success: #16a34a;
  --warning: #d97706;
  --danger: #dc2626;
  --glow: rgba(8, 145, 178, 0.3);
  --shadow: 0 4px 20px rgba(15, 23, 42, 0.06), 0 0 0 1px rgba(8, 145, 178, 0.1);
  --shadow-card: 0 8px 28px rgba(15, 23, 42, 0.08), 0 0 0 1px rgba(8, 145, 178, 0.08);
  --scrollbar-thumb: rgba(8, 145, 178, 0.35);
}

/* ===== Background Decorations ===== */
.bg-decor {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
  overflow: hidden;
}

.bg-grid {
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(0, 212, 255, 0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(0, 212, 255, 0.05) 1px, transparent 1px);
  background-size: 60px 60px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  -webkit-mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  animation: grid-drift 30s linear infinite;
}

@keyframes grid-drift {
  0% { background-position: 0 0; }
  100% { background-position: 60px 60px; }
}

.bg-glow {
  position: absolute;
  border-radius: 50%;
  filter: blur(120px);
  animation: glow-float 18s ease-in-out infinite;
}

.bg-glow-1 {
  width: 700px;
  height: 700px;
  background: var(--primary);
  top: -200px;
  left: -150px;
  opacity: 0.18;
}

.bg-glow-2 {
  width: 800px;
  height: 800px;
  background: var(--secondary);
  bottom: -250px;
  right: -150px;
  opacity: 0.12;
  animation-delay: -6s;
}

@keyframes glow-float {
  0%, 100% { transform: translate(0, 0) scale(1); }
  33% { transform: translate(60px, -40px) scale(1.08); }
  66% { transform: translate(-40px, 50px) scale(0.96); }
}

/* ===== Scan Line ===== */
.scan-line {
  position: absolute;
  left: 0;
  right: 0;
  top: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  animation: scan-move 10s linear infinite;
  z-index: 1;
  pointer-events: none;
  opacity: 0.5;
}

@keyframes scan-move {
  0% { transform: translateY(-100px); opacity: 0; }
  10% { opacity: 0.5; }
  90% { opacity: 0.5; }
  100% { transform: translateY(100vh); opacity: 0; }
}

/* ===== Layout ===== */
.page-container {
  max-width: 1600px;
  margin: 0 auto;
  padding: 20px;
}

/* ===== Page Header ===== */
.page-header {
  margin-bottom: 16px;
  position: relative;
  padding: 12px 16px;
  background: linear-gradient(90deg, var(--primary-bg) 0%, transparent 70%);
  border: 1px solid var(--border-soft);
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 14px;
  overflow: hidden;
}

.page-header::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), var(--primary), transparent);
  border-radius: 8px 8px 0 0;
  opacity: 0.9;
}

.page-header::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 120px;
  height: 2px;
  background: linear-gradient(90deg, var(--primary), var(--secondary), transparent);
  border-radius: 2px;
}

.header-deco {
  display: flex;
  flex-direction: column;
  gap: 3px;
  flex-shrink: 0;
}

.header-deco-l { align-items: flex-start; }
.header-deco-r { align-items: flex-end; }

.hd-line {
  width: 30px;
  height: 2px;
  background: linear-gradient(90deg, var(--primary), transparent);
  position: relative;
}

.header-deco-r .hd-line {
  background: linear-gradient(270deg, var(--primary), transparent);
}

.hd-line::after {
  content: '';
  position: absolute;
  right: 0;
  top: -2px;
  width: 2px;
  height: 5px;
  background: var(--primary);
  box-shadow: 0 0 6px var(--primary);
}

.hd-dot {
  width: 5px;
  height: 5px;
  background: var(--primary);
  border-radius: 50%;
  box-shadow: 0 0 6px var(--primary);
  animation: dot-blink 2s ease-in-out infinite;
}

.header-deco-r .hd-dot {
  animation-delay: -0.7s;
}

@keyframes dot-blink {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.3; transform: scale(0.6); }
}

.header-main {
  display: flex;
  align-items: center;
  flex: 1;
}

.page-title {
  font-size: 22px;
  font-weight: 700;
  color: var(--text-strong);
  margin: 0;
  letter-spacing: 2px;
  position: relative;
  display: flex;
  align-items: center;
  gap: 4px;
}

.title-bracket {
  color: var(--primary);
  font-weight: 400;
  opacity: 0.7;
}

.title-text {
  background: linear-gradient(180deg, #f1f5f9 0%, var(--primary) 100%);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 8px var(--glow));
}

.title-shine {
  position: absolute;
  top: 0;
  right: 0;
  width: 20px;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  animation: shine-sweep 3s ease-in-out infinite;
}

@keyframes shine-sweep {
  0% { transform: translateX(-100%); }
  50% { transform: translateX(200%); }
  100% { transform: translateX(200%); }
}

/* ===== Tab Bar ===== */
.tab-bar {
  display: flex;
  gap: 6px;
  background: var(--bg-card);
  backdrop-filter: blur(8px);
  padding: 10px 14px;
  border-radius: 10px;
  border: 1px solid var(--border);
  box-shadow: var(--shadow);
  margin-bottom: 20px;
  flex-wrap: wrap;
  position: relative;
  transition: box-shadow 0.4s cubic-bezier(0.4, 0, 0.2, 1), border-color 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.tab-bar::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.7;
}

.tab-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 22px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  color: var(--text-dim);
  font-size: 14px;
  font-weight: 500;
  position: relative;
  overflow: hidden;
  border: 1px solid transparent;
}

.tab-item::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, var(--primary-bg), transparent);
  opacity: 0;
  transition: opacity 0.3s ease;
  border-radius: 6px;
}

.tab-item:hover {
  color: var(--primary);
  border-color: var(--border);
  background: var(--bg-hover);
  transform: translateY(-1px);
}

.tab-item:hover::before {
  opacity: 1;
}

.tab-item.active {
  background: linear-gradient(135deg, var(--primary-bg), rgba(167, 139, 250, 0.12));
  color: var(--primary);
  border-color: var(--border-strong);
  box-shadow: 0 0 16px var(--glow), inset 0 0 12px rgba(56, 189, 248, 0.08);
  text-shadow: 0 0 8px var(--glow);
}

.tab-item.active::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 60%;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 2px;
}

.tab-item .tab-icon {
  font-size: 18px;
  filter: drop-shadow(0 0 4px rgba(56, 189, 248, 0.3));
}

.tab-item .tab-label {
  font-weight: 600;
  letter-spacing: 0.5px;
}

.tab-item .tab-badge {
  background: linear-gradient(135deg, var(--danger), #f97316);
  color: #fff;
  font-size: 11px;
  padding: 2px 8px;
  border-radius: 10px;
  margin-left: 4px;
  font-weight: 600;
  box-shadow: 0 0 10px rgba(251, 113, 133, 0.5);
  animation: badge-pulse 2s ease-in-out infinite;
}

@keyframes badge-pulse {
  0%, 100% { box-shadow: 0 0 10px rgba(251, 113, 133, 0.5); }
  50% { box-shadow: 0 0 16px rgba(251, 113, 133, 0.8); }
}

/* ===== Tab Content ===== */
.tab-content {
  min-height: 500px;
  position: relative;
}

.tab-panel {
  animation: fadeIn 0.4s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

/* ===== Scrollbar ===== */
.statistics-report ::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

.statistics-report ::-webkit-scrollbar-track {
  background: transparent;
}

.statistics-report ::-webkit-scrollbar-thumb {
  background: var(--scrollbar-thumb);
  border-radius: 4px;
}

.statistics-report ::-webkit-scrollbar-thumb:hover {
  background: var(--border-strong);
}

/* ===== Light Theme Overrides ===== */
.statistics-report.theme-light .bg-grid {
  background-image:
    linear-gradient(rgba(8, 145, 178, 0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(8, 145, 178, 0.04) 1px, transparent 1px);
}

.statistics-report.theme-light .bg-glow-1 {
  opacity: 0.12;
}

.statistics-report.theme-light .bg-glow-2 {
  opacity: 0.08;
}

.statistics-report.theme-light .scan-line {
  opacity: 0.3;
}

.statistics-report.theme-light .page-header {
  background: linear-gradient(90deg, rgba(8, 145, 178, 0.06) 0%, transparent 70%);
  border-color: rgba(8, 145, 178, 0.18);
}

.statistics-report.theme-light .page-header::before {
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, transparent);
}

.statistics-report.theme-light .page-header::after {
  background: linear-gradient(90deg, #0891b2, #7c3aed, transparent);
}

.statistics-report.theme-light .hd-line {
  background: linear-gradient(90deg, #0891b2, transparent);
}

.statistics-report.theme-light .hd-line::after {
  background: #0891b2;
  box-shadow: 0 0 4px rgba(8, 145, 178, 0.45);
}

.statistics-report.theme-light .hd-dot {
  background: #0891b2;
  box-shadow: 0 0 6px rgba(8, 145, 178, 0.45);
}

.statistics-report.theme-light .title-text {
  background: linear-gradient(180deg, #1e293b 0%, #0891b2 100%);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 10px rgba(8, 145, 178, 0.18));
}

.statistics-report.theme-light .title-bracket {
  color: #0891b2;
  opacity: 0.6;
}

.statistics-report.theme-light .title-shine {
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.7), transparent);
}

.statistics-report.theme-light .tab-bar {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border-color: rgba(8, 145, 178, 0.2);
  box-shadow: 0 2px 8px rgba(15, 23, 42, 0.05), 0 1px 2px rgba(15, 23, 42, 0.04);
  backdrop-filter: none;
}

.statistics-report.theme-light .tab-bar::before {
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, transparent);
  opacity: 0.65;
}

.statistics-report.theme-light .tab-item {
  color: #64748b;
}

.statistics-report.theme-light .tab-item:hover {
  color: #0891b2;
  background: rgba(8, 145, 178, 0.05);
  border-color: rgba(8, 145, 178, 0.2);
}

.statistics-report.theme-light .tab-item.active {
  background: linear-gradient(135deg, rgba(8, 145, 178, 0.1), rgba(124, 58, 237, 0.06));
  color: #0891b2;
  border-color: rgba(8, 145, 178, 0.35);
  box-shadow: 0 2px 12px rgba(8, 145, 178, 0.15);
  text-shadow: none;
}

.statistics-report.theme-light .tab-item.active::after {
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, transparent);
}

.statistics-report.theme-light .tab-item .tab-icon {
  filter: none;
}

.statistics-report.theme-light .tab-item .tab-badge {
  box-shadow: 0 0 6px rgba(220, 38, 38, 0.35);
}

/* ===== Responsive ===== */
@media (max-width: 768px) {
  .page-container {
    padding: 12px;
  }

  .page-header {
    padding: 10px 12px;
  }

  .page-title {
    font-size: 18px;
    letter-spacing: 1px;
  }

  .header-deco {
    display: none;
  }

  .tab-bar {
    padding: 8px;
    gap: 4px;
  }

  .tab-item {
    padding: 8px 14px;
    font-size: 13px;
    gap: 6px;
  }

  .tab-item .tab-icon {
    font-size: 16px;
  }

  .bg-glow-1,
  .bg-glow-2 {
    width: 400px;
    height: 400px;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 16px;
  }

  .tab-item {
    padding: 7px 10px;
    font-size: 12px;
  }

  .tab-item .tab-label {
    display: none;
  }

  .tab-item .tab-icon {
    font-size: 18px;
  }
}
</style>