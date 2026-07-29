<template>
  <div class="dashboard-wrapper" :class="{ 'theme-light': isLight }">
    <!-- 背景装饰 -->
    <div class="bg-decor" aria-hidden="true">
      <div class="bg-grid"></div>
      <div class="bg-glow bg-glow-1"></div>
      <div class="bg-glow bg-glow-2"></div>
    </div>
    <!-- 扫描线 -->
    <div class="scan-line"></div>

    <!-- ================= 页面标题 ================= -->
    <div class="page-header">
      <div class="header-deco header-deco-l">
        <span class="hd-line"></span>
        <span class="hd-dot"></span>
      </div>
      <div class="header-main">
        <h1 class="page-title">
          <span class="title-bracket">【</span>
          <span class="title-text">数据采集与审核</span>
          <span class="title-bracket">】</span>
          <span class="title-shine"></span>
        </h1>
        <div class="header-stats">
          <div class="stat-item">
            <span class="stat-label">采集总量</span>
            <span class="stat-value">{{ totalRecords }}</span>
          </div>
          <span class="stat-divider"></span>
          <div class="stat-item">
            <span class="stat-label">待审核</span>
            <span class="stat-value stat-warn">{{ totalPending }}</span>
          </div>
          <span class="stat-divider"></span>
          <div class="stat-item">
            <span class="stat-label">已审核</span>
            <span class="stat-value stat-ok">{{ totalApproved }}</span>
          </div>
        </div>
      </div>
      <div class="header-deco header-deco-r">
        <span class="hd-dot"></span>
        <span class="hd-line"></span>
      </div>
    </div>

    <!-- ================= Tab切换栏 ================= -->
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

    <!-- ================= Tab内容区 ================= -->
    <div class="tab-content">
      <!-- ===== 视力 Tab ===== -->
      <div v-show="activeTab === 'vision'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon blue">📊</div>
            <div class="stat-info">
              <div class="stat-label">待审核数据</div>
              <div class="stat-value">{{ visionStats.pending }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info">
              <div class="stat-label">已审核数据</div>
              <div class="stat-value">{{ visionStats.approved }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">⚠️</div>
            <div class="stat-info">
              <div class="stat-label">异常数据提醒</div>
              <div class="stat-value">{{ visionStats.abnormal }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon red">📋</div>
            <div class="stat-info">
              <div class="stat-label">待抽检数据</div>
              <div class="stat-value">{{ visionStats.spotCheck }}</div>
            </div>
          </div>
        </div>

        <div class="toolbar">
          <div class="toolbar-left">
            <button class="btn btn-primary" @click="handleSingleEntry">
              <span class="icon">📝</span> 单个录入
            </button>
            <button class="btn btn-success" @click="handleBatchImport">
              <span class="icon">📤</span> Excel批量导入
            </button>
            <button class="btn btn-info" @click="handleDeviceImport">
              <span class="icon">📱</span> 设备自动采集
            </button>
            <button class="btn btn-warning" @click="handleCheckData">
              <span class="icon">🔍</span> 数据校验
            </button>
          </div>
          <div class="toolbar-right">
            <span class="toolbar-tip">审核流程：班主任录入 → 校医审核 → 区县教育局抽检</span>
          </div>
        </div>

        <div class="flow-chart">
          <div class="flow-step">
            <div class="step-icon">👨‍🏫</div>
            <div class="step-label">班主任录入</div>
            <div class="step-status done">已完成 12条</div>
          </div>
          <div class="flow-arrow">→</div>
          <div class="flow-step">
            <div class="step-icon">👨‍⚕️</div>
            <div class="step-label">校医审核</div>
            <div class="step-status doing">进行中 8条</div>
          </div>
          <div class="flow-arrow">→</div>
          <div class="flow-step">
            <div class="step-icon">🏛️</div>
            <div class="step-label">区县教育局抽检</div>
            <div class="step-status pending">待抽检 5条</div>
          </div>
        </div>

        <div class="data-table-wrapper">
          <div class="table-header">
            <h3>📋 视力数据采集列表</h3>
            <div class="table-filter">
              <select v-model="visionFilter.status" class="filter-select">
                <option value="all">全部状态</option>
                <option value="pending">待审核</option>
                <option value="approved">已审核</option>
                <option value="abnormal">异常</option>
                <option value="spot">待抽检</option>
              </select>
              <input v-model="visionFilter.keyword" type="text" placeholder="搜索姓名/学号..." class="filter-input" />
              <button class="btn btn-sm btn-primary" @click="handleVisionFilter">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr>
                <th><input type="checkbox" /></th>
                <th>学号</th>
                <th>姓名</th>
                <th>年级</th>
                <th>班级</th>
                <th>左眼视力</th>
                <th>右眼视力</th>
                <th>视力等级</th>
                <th>录入人</th>
                <th>录入时间</th>
                <th>状态</th>
                <th>操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in filteredVisionData" :key="item.id">
                <td><input type="checkbox" /></td>
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.name }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.class }}</td>
                <td>{{ item.leftEye }}</td>
                <td>{{ item.rightEye }}</td>
                <td>
                  <span class="level-tag" :class="getLevelClass(item.level)">
                    {{ item.level }}
                  </span>
                </td>
                <td>{{ item.recorder }}</td>
                <td>{{ item.recordTime }}</td>
                <td>
                  <span class="status-tag" :class="getStatusClass(item.status)">
                    {{ getStatusText(item.status) }}
                  </span>
                </td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleApprove(item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleReject(item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleSpotCheck(item)" title="抽检">🔍</button>
                    <button class="btn-icon edit" @click="handleEdit(item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleDelete(item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ filteredVisionData.length }} 条</span>
            <div class="pagination-btns">
              <button class="page-btn">上一页</button>
              <button class="page-btn active">1</button>
              <button class="page-btn">2</button>
              <button class="page-btn">3</button>
              <button class="page-btn">下一页</button>
            </div>
          </div>
        </div>

        <div v-if="showAbnormalAlert" class="abnormal-alert">
          <div class="alert-header">
            <span class="alert-icon">⚠️</span>
            <span class="alert-title">异常数据提醒</span>
            <button class="alert-close" @click="showAbnormalAlert = false">✕</button>
          </div>
          <div class="alert-body">
            <div class="alert-item" v-for="item in abnormalList" :key="item.id">
              <span class="alert-name">{{ item.name }}</span>
              <span class="alert-detail">{{ item.detail }}</span>
              <span class="alert-suggest">{{ item.suggest }}</span>
            </div>
          </div>
        </div>

        <div v-if="showDuplicateAlert" class="duplicate-alert">
          <div class="alert-header">
            <span class="alert-icon">🔄</span>
            <span class="alert-title">重复数据自动去重提醒</span>
            <button class="alert-close" @click="showDuplicateAlert = false">✕</button>
          </div>
          <div class="alert-body">
            <div class="alert-item" v-for="item in duplicateList" :key="item.id">
              <span class="alert-name">{{ item.name }}</span>
              <span class="alert-detail">学号：{{ item.studentNo }}，重复录入 {{ item.count }} 次</span>
              <button class="btn btn-sm btn-danger" @click="handleMergeDuplicate(item)">合并去重</button>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 口腔 Tab ===== -->
      <div v-show="activeTab === 'oral'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon blue">📊</div>
            <div class="stat-info"><div class="stat-label">待审核数据</div><div class="stat-value">{{ oralStats.pending }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info"><div class="stat-label">已审核数据</div><div class="stat-value">{{ oralStats.approved }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">⚠️</div>
            <div class="stat-info"><div class="stat-label">异常数据提醒</div><div class="stat-value">{{ oralStats.abnormal }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon red">📋</div>
            <div class="stat-info"><div class="stat-label">待抽检数据</div><div class="stat-value">{{ oralStats.spotCheck }}</div></div>
          </div>
        </div>

        <div class="toolbar">
          <div class="toolbar-left">
            <button class="btn btn-primary" @click="handleOralSingleEntry">📝 单个录入</button>
            <button class="btn btn-success" @click="handleOralBatchImport">📤 Excel批量导入</button>
            <button class="btn btn-info" @click="handleOralDeviceImport">📱 设备自动采集</button>
            <button class="btn btn-warning" @click="handleOralCheckData">🔍 数据校验</button>
          </div>
          <div class="toolbar-right"><span class="toolbar-tip">审核流程：班主任录入 → 校医审核 → 区县教育局抽检</span></div>
        </div>

        <div class="flow-chart">
          <div class="flow-step"><div class="step-icon">👨‍🏫</div><div class="step-label">班主任录入</div><div class="step-status done">已完成 12条</div></div>
          <span class="flow-arrow">→</span>
          <div class="flow-step"><div class="step-icon">👨‍⚕️</div><div class="step-label">校医审核</div><div class="step-status doing">进行中 8条</div></div>
          <span class="flow-arrow">→</span>
          <div class="flow-step"><div class="step-icon">🏛️</div><div class="step-label">区县教育局抽检</div><div class="step-status pending">待抽检 5条</div></div>
        </div>

        <div class="data-table-wrapper">
          <div class="table-header">
            <h3>📋 口腔数据采集列表</h3>
            <div class="table-filter">
              <select v-model="oralFilter.status" class="filter-select">
                <option value="all">全部状态</option>
                <option value="pending">待审核</option>
                <option value="approved">已审核</option>
                <option value="abnormal">异常</option>
                <option value="spot">待抽检</option>
              </select>
              <input v-model="oralFilter.keyword" type="text" placeholder="搜索姓名/学号..." class="filter-input" />
              <button class="btn btn-sm btn-primary" @click="handleOralFilter">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr>
                <th><input type="checkbox" /></th>
                <th>学号</th><th>姓名</th><th>年级</th><th>班级</th>
                <th>乳牙龋</th><th>恒牙龋</th><th>替牙情况</th><th>颌面发育</th>
                <th>录入人</th><th>录入时间</th><th>状态</th><th>操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in filteredOralData" :key="item.id">
                <td><input type="checkbox" /></td>
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.name }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.class }}</td>
                <td>{{ item.decayedTeeth }}</td>
                <td>{{ item.permanentDecayed }}</td>
                <td>{{ item.toothStage }}</td>
                <td>{{ item.jawDevelopment }}</td>
                <td>{{ item.recorder }}</td>
                <td>{{ item.recordTime }}</td>
                <td><span class="status-tag" :class="getStatusClass(item.status)">{{ getStatusText(item.status) }}</span></td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleOralApprove(item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleOralReject(item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleOralSpotCheck(item)" title="抽检">🔍</button>
                    <button class="btn-icon edit" @click="handleOralEdit(item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleOralDelete(item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ filteredOralData.length }} 条</span>
            <div class="pagination-btns">
              <button class="page-btn">上一页</button>
              <button class="page-btn active">1</button>
              <button class="page-btn">2</button>
              <button class="page-btn">3</button>
              <button class="page-btn">下一页</button>
            </div>
          </div>
        </div>

        <div v-if="showOralAbnormalAlert" class="abnormal-alert">
          <div class="alert-header"><span class="alert-icon">⚠️</span><span class="alert-title">异常数据提醒</span><button class="alert-close" @click="showOralAbnormalAlert = false">✕</button></div>
          <div class="alert-body">
            <div class="alert-item" v-for="item in oralAbnormalList" :key="item.id">
              <span class="alert-name">{{ item.name }}</span>
              <span class="alert-detail">{{ item.detail }}</span>
              <span class="alert-suggest">{{ item.suggest }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 心理 Tab ===== -->
      <div v-show="activeTab === 'mental'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon blue">📊</div>
            <div class="stat-info"><div class="stat-label">待审核数据</div><div class="stat-value">{{ mentalStats.pending }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info"><div class="stat-label">已审核数据</div><div class="stat-value">{{ mentalStats.approved }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">⚠️</div>
            <div class="stat-info"><div class="stat-label">异常数据提醒</div><div class="stat-value">{{ mentalStats.abnormal }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon red">📋</div>
            <div class="stat-info"><div class="stat-label">待抽检数据</div><div class="stat-value">{{ mentalStats.spotCheck }}</div></div>
          </div>
        </div>

        <div class="toolbar">
          <div class="toolbar-left">
            <button class="btn btn-primary" @click="handleMentalSingleEntry">📝 单个录入</button>
            <button class="btn btn-success" @click="handleMentalBatchImport">📤 Excel批量导入</button>
            <button class="btn btn-info" @click="handleMentalDeviceImport">📱 设备自动采集</button>
            <button class="btn btn-warning" @click="handleMentalCheckData">🔍 数据校验</button>
          </div>
          <div class="toolbar-right"><span class="toolbar-tip">审核流程：班主任录入 → 心理老师审核 → 区县教育局抽检</span></div>
        </div>

        <div class="flow-chart">
          <div class="flow-step"><div class="step-icon">👨‍🏫</div><div class="step-label">班主任录入</div><div class="step-status done">已完成 12条</div></div>
          <span class="flow-arrow">→</span>
          <div class="flow-step"><div class="step-icon">🧠</div><div class="step-label">心理老师审核</div><div class="step-status doing">进行中 8条</div></div>
          <span class="flow-arrow">→</span>
          <div class="flow-step"><div class="step-icon">🏛️</div><div class="step-label">区县教育局抽检</div><div class="step-status pending">待抽检 5条</div></div>
        </div>

        <div class="data-table-wrapper">
          <div class="table-header">
            <h3>📋 心理数据采集列表</h3>
            <div class="table-filter">
              <select v-model="mentalFilter.status" class="filter-select">
                <option value="all">全部状态</option>
                <option value="pending">待审核</option>
                <option value="approved">已审核</option>
                <option value="abnormal">异常</option>
                <option value="spot">待抽检</option>
              </select>
              <input v-model="mentalFilter.keyword" type="text" placeholder="搜索姓名/学号..." class="filter-input" />
              <button class="btn btn-sm btn-primary" @click="handleMentalFilter">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr>
                <th><input type="checkbox" /></th>
                <th>学号</th><th>姓名</th><th>年级</th><th>班级</th>
                <th>焦虑评分</th><th>抑郁评分</th><th>学习焦虑</th><th>人际敏感</th>
                <th>录入人</th><th>录入时间</th><th>状态</th><th>操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in filteredMentalData" :key="item.id">
                <td><input type="checkbox" /></td>
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.name }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.class }}</td>
                <td>{{ item.anxietyScore }}</td>
                <td>{{ item.depressionScore }}</td>
                <td>{{ item.learningAnxiety }}</td>
                <td>{{ item.interpersonalSensitivity }}</td>
                <td>{{ item.recorder }}</td>
                <td>{{ item.recordTime }}</td>
                <td><span class="status-tag" :class="getStatusClass(item.status)">{{ getStatusText(item.status) }}</span></td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleMentalApprove(item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleMentalReject(item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleMentalSpotCheck(item)" title="抽检">🔍</button>
                    <button class="btn-icon edit" @click="handleMentalEdit(item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleMentalDelete(item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ filteredMentalData.length }} 条</span>
            <div class="pagination-btns">
              <button class="page-btn">上一页</button>
              <button class="page-btn active">1</button>
              <button class="page-btn">2</button>
              <button class="page-btn">3</button>
              <button class="page-btn">下一页</button>
            </div>
          </div>
        </div>

        <div v-if="showMentalAbnormalAlert" class="abnormal-alert">
          <div class="alert-header"><span class="alert-icon">⚠️</span><span class="alert-title">异常数据提醒</span><button class="alert-close" @click="showMentalAbnormalAlert = false">✕</button></div>
          <div class="alert-body">
            <div class="alert-item" v-for="item in mentalAbnormalList" :key="item.id">
              <span class="alert-name">{{ item.name }}</span>
              <span class="alert-detail">{{ item.detail }}</span>
              <span class="alert-suggest">{{ item.suggest }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 健康体重 Tab ===== -->
      <div v-show="activeTab === 'weight'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon blue">📊</div>
            <div class="stat-info"><div class="stat-label">待审核数据</div><div class="stat-value">{{ weightStats.pending }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info"><div class="stat-label">已审核数据</div><div class="stat-value">{{ weightStats.approved }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">⚠️</div>
            <div class="stat-info"><div class="stat-label">异常数据提醒</div><div class="stat-value">{{ weightStats.abnormal }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon red">📋</div>
            <div class="stat-info"><div class="stat-label">待抽检数据</div><div class="stat-value">{{ weightStats.spotCheck }}</div></div>
          </div>
        </div>

        <div class="toolbar">
          <div class="toolbar-left">
            <button class="btn btn-primary" @click="handleWeightSingleEntry">📝 单个录入</button>
            <button class="btn btn-success" @click="handleWeightBatchImport">📤 Excel批量导入</button>
            <button class="btn btn-info" @click="handleWeightDeviceImport">📱 设备自动采集</button>
            <button class="btn btn-warning" @click="handleWeightCheckData">🔍 数据校验</button>
          </div>
          <div class="toolbar-right"><span class="toolbar-tip">审核流程：体育老师录入 → 校医审核 → 区县教育局抽检</span></div>
        </div>

        <div class="flow-chart">
          <div class="flow-step"><div class="step-icon">🏃</div><div class="step-label">体育老师录入</div><div class="step-status done">已完成 10条</div></div>
          <span class="flow-arrow">→</span>
          <div class="flow-step"><div class="step-icon">👨‍⚕️</div><div class="step-label">校医审核</div><div class="step-status doing">进行中 6条</div></div>
          <span class="flow-arrow">→</span>
          <div class="flow-step"><div class="step-icon">🏛️</div><div class="step-label">区县教育局抽检</div><div class="step-status pending">待抽检 3条</div></div>
        </div>

        <div class="data-table-wrapper">
          <div class="table-header">
            <h3>📋 健康体重数据采集列表</h3>
            <div class="table-filter">
              <select v-model="weightFilter.status" class="filter-select">
                <option value="all">全部状态</option>
                <option value="pending">待审核</option>
                <option value="approved">已审核</option>
                <option value="abnormal">异常</option>
                <option value="spot">待抽检</option>
              </select>
              <input v-model="weightFilter.keyword" type="text" placeholder="搜索姓名/学号..." class="filter-input" />
              <button class="btn btn-sm btn-primary" @click="handleWeightFilter">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr>
                <th><input type="checkbox" /></th>
                <th>学号</th><th>姓名</th><th>年级</th><th>班级</th>
                <th>身高(cm)</th><th>体重(kg)</th><th>BMI</th><th>BMI等级</th>
                <th>腰围(cm)</th><th>臀围(cm)</th><th>腰臀比</th>
                <th>录入人</th><th>录入时间</th><th>状态</th><th>操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in filteredWeightData" :key="item.id">
                <td><input type="checkbox" /></td>
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.name }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.class }}</td>
                <td>{{ item.height }}</td>
                <td>{{ item.weight }}</td>
                <td>{{ item.bmi }}</td>
                <td>
                  <span class="level-tag" :class="getWeightLevelClass(item.bmiLevel)">{{ item.bmiLevel }}</span>
                </td>
                <td>{{ item.waist }}</td>
                <td>{{ item.hip }}</td>
                <td>{{ item.whr }}</td>
                <td>{{ item.recorder }}</td>
                <td>{{ item.recordTime }}</td>
                <td>
                  <span class="status-tag" :class="getStatusClass(item.status)">{{ getStatusText(item.status) }}</span>
                </td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleWeightApprove(item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleWeightReject(item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleWeightSpotCheck(item)" title="抽检">🔍</button>
                    <button class="btn-icon edit" @click="handleWeightEdit(item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleWeightDelete(item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ filteredWeightData.length }} 条</span>
            <div class="pagination-btns">
              <button class="page-btn">上一页</button>
              <button class="page-btn active">1</button>
              <button class="page-btn">2</button>
              <button class="page-btn">3</button>
              <button class="page-btn">下一页</button>
            </div>
          </div>
        </div>

        <div v-if="showWeightAbnormalAlert" class="abnormal-alert">
          <div class="alert-header"><span class="alert-icon">⚠️</span><span class="alert-title">异常数据提醒</span><button class="alert-close" @click="showWeightAbnormalAlert = false">✕</button></div>
          <div class="alert-body">
            <div class="alert-item" v-for="item in weightAbnormalList" :key="item.id">
              <span class="alert-name">{{ item.name }}</span>
              <span class="alert-detail">{{ item.detail }}</span>
              <span class="alert-suggest">{{ item.suggest }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 骨骼健康 Tab ===== -->
      <div v-show="activeTab === 'bone'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon blue">📊</div>
            <div class="stat-info"><div class="stat-label">待审核数据</div><div class="stat-value">{{ boneStats.pending }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info"><div class="stat-label">已审核数据</div><div class="stat-value">{{ boneStats.approved }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">⚠️</div>
            <div class="stat-info"><div class="stat-label">异常数据提醒</div><div class="stat-value">{{ boneStats.abnormal }}</div></div>
          </div>
          <div class="stat-card">
            <div class="stat-icon red">📋</div>
            <div class="stat-info"><div class="stat-label">待抽检数据</div><div class="stat-value">{{ boneStats.spotCheck }}</div></div>
          </div>
        </div>

        <div class="toolbar">
          <div class="toolbar-left">
            <button class="btn btn-primary" @click="handleBoneSingleEntry">📝 单个录入</button>
            <button class="btn btn-success" @click="handleBoneBatchImport">📤 Excel批量导入</button>
            <button class="btn btn-info" @click="handleBoneDeviceImport">📱 设备自动采集</button>
            <button class="btn btn-warning" @click="handleBoneCheckData">🔍 数据校验</button>
          </div>
          <div class="toolbar-right"><span class="toolbar-tip">审核流程：校医录入 → 专家审核 → 区县教育局抽检</span></div>
        </div>

        <div class="flow-chart">
          <div class="flow-step"><div class="step-icon">👨‍⚕️</div><div class="step-label">校医录入</div><div class="step-status done">已完成 8条</div></div>
          <span class="flow-arrow">→</span>
          <div class="flow-step"><div class="step-icon">🧑‍⚕️</div><div class="step-label">专家审核</div><div class="step-status doing">进行中 4条</div></div>
          <span class="flow-arrow">→</span>
          <div class="flow-step"><div class="step-icon">🏛️</div><div class="step-label">区县教育局抽检</div><div class="step-status pending">待抽检 2条</div></div>
        </div>

        <div class="data-table-wrapper">
          <div class="table-header">
            <h3>📋 骨骼健康数据采集列表</h3>
            <div class="table-filter">
              <select v-model="boneFilter.status" class="filter-select">
                <option value="all">全部状态</option>
                <option value="pending">待审核</option>
                <option value="approved">已审核</option>
                <option value="abnormal">异常</option>
                <option value="spot">待抽检</option>
              </select>
              <input v-model="boneFilter.keyword" type="text" placeholder="搜索姓名/学号..." class="filter-input" />
              <button class="btn btn-sm btn-primary" @click="handleBoneFilter">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr>
                <th><input type="checkbox" /></th>
                <th>学号</th><th>姓名</th><th>年级</th><th>班级</th>
                <th>骨密度</th><th>骨密度等级</th><th>骨龄</th>
                <th>维生素D</th><th>钙水平</th>
                <th>录入人</th><th>录入时间</th><th>状态</th><th>操作</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in filteredBoneData" :key="item.id">
                <td><input type="checkbox" /></td>
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.name }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.class }}</td>
                <td>{{ item.boneDensity }}</td>
                <td>
                  <span class="level-tag" :class="getBoneLevelClass(item.boneLevel)">{{ item.boneLevel }}</span>
                </td>
                <td>{{ item.boneAge }}</td>
                <td>
                  <span class="status-tag" :class="getVitaminClass(item.vitaminD)">{{ item.vitaminD }}</span>
                </td>
                <td>
                  <span class="status-tag" :class="getCalciumClass(item.calciumLevel)">{{ item.calciumLevel }}</span>
                </td>
                <td>{{ item.recorder }}</td>
                <td>{{ item.recordTime }}</td>
                <td>
                  <span class="status-tag" :class="getStatusClass(item.status)">{{ getStatusText(item.status) }}</span>
                </td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleBoneApprove(item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleBoneReject(item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleBoneSpotCheck(item)" title="抽检">🔍</button>
                    <button class="btn-icon edit" @click="handleBoneEdit(item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleBoneDelete(item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ filteredBoneData.length }} 条</span>
            <div class="pagination-btns">
              <button class="page-btn">上一页</button>
              <button class="page-btn active">1</button>
              <button class="page-btn">2</button>
              <button class="page-btn">3</button>
              <button class="page-btn">下一页</button>
            </div>
          </div>
        </div>

        <div v-if="showBoneAbnormalAlert" class="abnormal-alert">
          <div class="alert-header"><span class="alert-icon">⚠️</span><span class="alert-title">异常数据提醒</span><button class="alert-close" @click="showBoneAbnormalAlert = false">✕</button></div>
          <div class="alert-body">
            <div class="alert-item" v-for="item in boneAbnormalList" :key="item.id">
              <span class="alert-name">{{ item.name }}</span>
              <span class="alert-detail">{{ item.detail }}</span>
              <span class="alert-suggest">{{ item.suggest }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onBeforeUnmount } from 'vue';
import { message, Modal } from 'ant-design-vue';

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

// ================= Tab配置 =================
const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️', badge: '12' },
  { key: 'oral', label: '口腔健康', icon: '🦷', badge: '5' },
  { key: 'mental', label: '心理健康', icon: '🧠', badge: '8' },
  { key: 'weight', label: '健康体重', icon: '⚖️', badge: '7' },
  { key: 'bone', label: '骨骼健康', icon: '🦴', badge: '4' }
];
const activeTab = ref('vision');

const switchTab = (tabKey) => {
  activeTab.value = tabKey;
};

// ================= 统计数据 =================
const visionStats = reactive({ pending: 23, approved: 156, abnormal: 8, spotCheck: 5 });
const oralStats = reactive({ pending: 12, approved: 89, abnormal: 3, spotCheck: 2 });
const mentalStats = reactive({ pending: 15, approved: 72, abnormal: 5, spotCheck: 3 });
const weightStats = reactive({ pending: 18, approved: 95, abnormal: 6, spotCheck: 4 });
const boneStats = reactive({ pending: 10, approved: 52, abnormal: 4, spotCheck: 2 });

// ================= 汇总统计 =================
const totalRecords = computed(() => {
  return visionStats.approved + oralStats.approved + mentalStats.approved + weightStats.approved + boneStats.approved
    + visionStats.pending + oralStats.pending + mentalStats.pending + weightStats.pending + boneStats.pending;
});
const totalPending = computed(() => {
  return visionStats.pending + oralStats.pending + mentalStats.pending + weightStats.pending + boneStats.pending;
});
const totalApproved = computed(() => {
  return visionStats.approved + oralStats.approved + mentalStats.approved + weightStats.approved + boneStats.approved;
});

// ================= 视力数据 =================
const visionData = ref([
  { id: 1, studentNo: '2024001', name: '张小明', grade: '三年级', class: '二班', leftEye: '4.8', rightEye: '4.9', level: '轻度近视', recorder: '王老师', recordTime: '2026-06-28 14:30', status: 'pending' },
  { id: 2, studentNo: '2024002', name: '李小红', grade: '三年级', class: '二班', leftEye: '5.0', rightEye: '5.0', level: '正常', recorder: '王老师', recordTime: '2026-06-28 14:35', status: 'approved' },
  { id: 3, studentNo: '2024003', name: '王小刚', grade: '四年级', class: '一班', leftEye: '4.2', rightEye: '4.3', level: '高度近视', recorder: '张老师', recordTime: '2026-06-28 15:00', status: 'abnormal' },
  { id: 4, studentNo: '2024004', name: '赵小燕', grade: '四年级', class: '一班', leftEye: '4.6', rightEye: '4.7', level: '轻度近视', recorder: '张老师', recordTime: '2026-06-28 15:10', status: 'pending' },
  { id: 5, studentNo: '2024005', name: '刘小伟', grade: '五年级', class: '三班', leftEye: '4.9', rightEye: '4.8', level: '正常', recorder: '李老师', recordTime: '2026-06-28 15:30', status: 'approved' },
  { id: 6, studentNo: '2024006', name: '陈小丽', grade: '五年级', class: '三班', leftEye: '4.4', rightEye: '4.5', level: '中度近视', recorder: '李老师', recordTime: '2026-06-28 15:45', status: 'spot' },
  { id: 7, studentNo: '2024007', name: '周小杰', grade: '六年级', class: '一班', leftEye: '4.1', rightEye: '4.0', level: '高度近视', recorder: '赵老师', recordTime: '2026-06-28 16:00', status: 'abnormal' },
  { id: 8, studentNo: '2024008', name: '吴小芳', grade: '六年级', class: '一班', leftEye: '4.7', rightEye: '4.8', level: '轻度近视', recorder: '赵老师', recordTime: '2026-06-28 16:15', status: 'pending' },
]);

// ================= 口腔数据 =================
const oralData = ref([
  { id: 1, studentNo: '2024001', name: '张小明', grade: '三年级', class: '二班', decayedTeeth: 2, permanentDecayed: 1, toothStage: '替牙期', jawDevelopment: '正常', recorder: '王老师', recordTime: '2026-06-28 14:30', status: 'pending' },
  { id: 2, studentNo: '2024002', name: '李小红', grade: '三年级', class: '二班', decayedTeeth: 0, permanentDecayed: 0, toothStage: '替牙期', jawDevelopment: '正常', recorder: '王老师', recordTime: '2026-06-28 14:35', status: 'approved' },
  { id: 3, studentNo: '2024003', name: '王小刚', grade: '四年级', class: '一班', decayedTeeth: 5, permanentDecayed: 2, toothStage: '替牙期', jawDevelopment: '地包天', recorder: '张老师', recordTime: '2026-06-28 15:00', status: 'abnormal' },
  { id: 4, studentNo: '2024004', name: '赵小燕', grade: '四年级', class: '一班', decayedTeeth: 3, permanentDecayed: 1, toothStage: '替牙期', jawDevelopment: '正常', recorder: '张老师', recordTime: '2026-06-28 15:10', status: 'pending' },
  { id: 5, studentNo: '2024005', name: '刘小伟', grade: '五年级', class: '三班', decayedTeeth: 0, permanentDecayed: 0, toothStage: '恒牙期', jawDevelopment: '正常', recorder: '李老师', recordTime: '2026-06-28 15:30', status: 'approved' },
]);

// ================= 心理数据 =================
const mentalData = ref([
  { id: 1, studentNo: '2024001', name: '张小明', grade: '三年级', class: '二班', anxietyScore: 12, depressionScore: 8, learningAnxiety: '中度', interpersonalSensitivity: '轻度', recorder: '王老师', recordTime: '2026-06-28 14:30', status: 'pending' },
  { id: 2, studentNo: '2024002', name: '李小红', grade: '三年级', class: '二班', anxietyScore: 6, depressionScore: 5, learningAnxiety: '轻度', interpersonalSensitivity: '正常', recorder: '王老师', recordTime: '2026-06-28 14:35', status: 'approved' },
  { id: 3, studentNo: '2024003', name: '王小刚', grade: '四年级', class: '一班', anxietyScore: 18, depressionScore: 15, learningAnxiety: '重度', interpersonalSensitivity: '中度', recorder: '张老师', recordTime: '2026-06-28 15:00', status: 'abnormal' },
  { id: 4, studentNo: '2024004', name: '赵小燕', grade: '四年级', class: '一班', anxietyScore: 14, depressionScore: 10, learningAnxiety: '中度', interpersonalSensitivity: '中度', recorder: '张老师', recordTime: '2026-06-28 15:10', status: 'pending' },
  { id: 5, studentNo: '2024005', name: '刘小伟', grade: '五年级', class: '三班', anxietyScore: 8, depressionScore: 6, learningAnxiety: '轻度', interpersonalSensitivity: '正常', recorder: '李老师', recordTime: '2026-06-28 15:30', status: 'approved' },
]);

// ================= 健康体重数据 =================
const weightData = ref([
  { id: 1, studentNo: '2024001', name: '张小明', grade: '三年级', class: '二班', height: '132', weight: '28', bmi: '16.1', bmiLevel: '偏瘦', waist: '56', hip: '62', whr: '0.90', recorder: '王老师', recordTime: '2026-06-28 14:30', status: 'pending' },
  { id: 2, studentNo: '2024002', name: '李小红', grade: '三年级', class: '二班', height: '128', weight: '26', bmi: '15.9', bmiLevel: '偏瘦', waist: '54', hip: '60', whr: '0.90', recorder: '王老师', recordTime: '2026-06-28 14:35', status: 'approved' },
  { id: 3, studentNo: '2024003', name: '王小刚', grade: '四年级', class: '一班', height: '145', weight: '52', bmi: '24.7', bmiLevel: '超重', waist: '78', hip: '82', whr: '0.95', recorder: '张老师', recordTime: '2026-06-28 15:00', status: 'abnormal' },
  { id: 4, studentNo: '2024004', name: '赵小燕', grade: '四年级', class: '一班', height: '140', weight: '35', bmi: '17.9', bmiLevel: '正常', waist: '62', hip: '68', whr: '0.91', recorder: '张老师', recordTime: '2026-06-28 15:10', status: 'pending' },
  { id: 5, studentNo: '2024005', name: '刘小伟', grade: '五年级', class: '三班', height: '150', weight: '42', bmi: '18.7', bmiLevel: '正常', waist: '64', hip: '70', whr: '0.91', recorder: '李老师', recordTime: '2026-06-28 15:30', status: 'approved' },
]);

// ================= 骨骼健康数据 =================
const boneData = ref([
  { id: 1, studentNo: '2024001', name: '张小明', grade: '三年级', class: '二班', boneDensity: '0.85', boneLevel: '正常', boneAge: '8岁', vitaminD: '充足', calciumLevel: '正常', recorder: '王老师', recordTime: '2026-06-28 14:30', status: 'pending' },
  { id: 2, studentNo: '2024002', name: '李小红', grade: '三年级', class: '二班', boneDensity: '0.82', boneLevel: '正常', boneAge: '8岁', vitaminD: '充足', calciumLevel: '正常', recorder: '王老师', recordTime: '2026-06-28 14:35', status: 'approved' },
  { id: 3, studentNo: '2024003', name: '王小刚', grade: '四年级', class: '一班', boneDensity: '0.65', boneLevel: '偏低', boneAge: '9岁', vitaminD: '不足', calciumLevel: '偏低', recorder: '张老师', recordTime: '2026-06-28 15:00', status: 'abnormal' },
  { id: 4, studentNo: '2024004', name: '赵小燕', grade: '四年级', class: '一班', boneDensity: '0.78', boneLevel: '正常', boneAge: '9岁', vitaminD: '充足', calciumLevel: '正常', recorder: '张老师', recordTime: '2026-06-28 15:10', status: 'pending' },
  { id: 5, studentNo: '2024005', name: '刘小伟', grade: '五年级', class: '三班', boneDensity: '0.88', boneLevel: '正常', boneAge: '10岁', vitaminD: '充足', calciumLevel: '正常', recorder: '李老师', recordTime: '2026-06-28 15:30', status: 'approved' },
]);

// ================= 筛选条件 =================
const visionFilter = reactive({ status: 'all', keyword: '' });
const oralFilter = reactive({ status: 'all', keyword: '' });
const mentalFilter = reactive({ status: 'all', keyword: '' });
const weightFilter = reactive({ status: 'all', keyword: '' });
const boneFilter = reactive({ status: 'all', keyword: '' });

// ================= 异常/重复数据 =================
const abnormalList = ref([
  { id: 3, name: '王小刚', detail: '右眼视力4.3，低于正常值范围', suggest: '建议重新检测' },
  { id: 7, name: '周小杰', detail: '左眼视力4.1，低于正常值范围', suggest: '建议重新检测' },
]);
const oralAbnormalList = ref([
  { id: 3, name: '王小刚', detail: '乳牙龋5颗，超过风险阈值', suggest: '建议涂氟治疗' },
]);
const mentalAbnormalList = ref([
  { id: 3, name: '王小刚', detail: '焦虑评分18分，超过正常阈值', suggest: '建议心理老师干预' },
]);
const weightAbnormalList = ref([
  { id: 3, name: '王小刚', detail: 'BMI 24.7，属于超重范围', suggest: '建议调整饮食和运动' },
]);
const boneAbnormalList = ref([
  { id: 3, name: '王小刚', detail: '骨密度0.65，低于正常值范围', suggest: '建议补充钙和维生素D' },
]);
const duplicateList = ref([
  { id: 1, name: '张小明', studentNo: '2024001', count: 2 },
  { id: 2, name: '李小红', studentNo: '2024002', count: 3 },
]);

// ================= 弹窗控制 =================
const showAbnormalAlert = ref(true);
const showDuplicateAlert = ref(true);
const showOralAbnormalAlert = ref(true);
const showMentalAbnormalAlert = ref(true);
const showWeightAbnormalAlert = ref(true);
const showBoneAbnormalAlert = ref(true);

// ================= 计算属性 =================
const filteredVisionData = computed(() => {
  let data = visionData.value;
  if (visionFilter.status !== 'all') {
    data = data.filter(item => item.status === visionFilter.status);
  }
  if (visionFilter.keyword) {
    const keyword = visionFilter.keyword.toLowerCase();
    data = data.filter(item => item.name.includes(keyword) || item.studentNo.includes(keyword));
  }
  return data;
});

const filteredOralData = computed(() => {
  let data = oralData.value;
  if (oralFilter.status !== 'all') {
    data = data.filter(item => item.status === oralFilter.status);
  }
  if (oralFilter.keyword) {
    const keyword = oralFilter.keyword.toLowerCase();
    data = data.filter(item => item.name.includes(keyword) || item.studentNo.includes(keyword));
  }
  return data;
});

const filteredMentalData = computed(() => {
  let data = mentalData.value;
  if (mentalFilter.status !== 'all') {
    data = data.filter(item => item.status === mentalFilter.status);
  }
  if (mentalFilter.keyword) {
    const keyword = mentalFilter.keyword.toLowerCase();
    data = data.filter(item => item.name.includes(keyword) || item.studentNo.includes(keyword));
  }
  return data;
});

const filteredWeightData = computed(() => {
  let data = weightData.value;
  if (weightFilter.status !== 'all') {
    data = data.filter(item => item.status === weightFilter.status);
  }
  if (weightFilter.keyword) {
    const keyword = weightFilter.keyword.toLowerCase();
    data = data.filter(item => item.name.includes(keyword) || item.studentNo.includes(keyword));
  }
  return data;
});

const filteredBoneData = computed(() => {
  let data = boneData.value;
  if (boneFilter.status !== 'all') {
    data = data.filter(item => item.status === boneFilter.status);
  }
  if (boneFilter.keyword) {
    const keyword = boneFilter.keyword.toLowerCase();
    data = data.filter(item => item.name.includes(keyword) || item.studentNo.includes(keyword));
  }
  return data;
});

// ================= 辅助函数 =================
const getLevelClass = (level) => {
  const map = { '正常': 'level-normal', '轻度近视': 'level-low', '中度近视': 'level-mid', '高度近视': 'level-high' };
  return map[level] || '';
};

const getWeightLevelClass = (level) => {
  const map = { '偏瘦': 'level-low', '正常': 'level-normal', '超重': 'level-mid', '肥胖': 'level-high' };
  return map[level] || '';
};

const getBoneLevelClass = (level) => {
  const map = { '正常': 'level-normal', '偏低': 'level-low' };
  return map[level] || '';
};

const getVitaminClass = (level) => {
  const map = { '充足': 'status-approved', '良好': 'status-approved', '不足': 'status-abnormal' };
  return map[level] || '';
};

const getCalciumClass = (level) => {
  const map = { '正常': 'status-approved', '偏低': 'status-abnormal' };
  return map[level] || '';
};

const getStatusClass = (status) => {
  const map = { 'pending': 'status-pending', 'approved': 'status-approved', 'abnormal': 'status-abnormal', 'spot': 'status-spot' };
  return map[status] || '';
};

const getStatusText = (status) => {
  const map = { 'pending': '待审核', 'approved': '已审核', 'abnormal': '异常', 'spot': '待抽检' };
  return map[status] || status;
};

// ================= 视力操作 =================
const handleSingleEntry = () => {
  Modal.info({ title: '单个录入', content: '打开单个录入表单，支持手动输入学生视力数据', okText: '知道了' });
};
const handleBatchImport = () => {
  Modal.info({ title: 'Excel批量导入', content: '支持上传Excel文件，批量导入学生视力数据。支持 .xlsx, .xls 格式', okText: '知道了' });
};
const handleDeviceImport = () => {
  Modal.info({ title: '设备自动采集', content: '连接视力检测设备，自动采集学生视力数据并上传至系统', okText: '知道了' });
};
const handleCheckData = () => {
  message.success('数据校验完成！发现 3 条异常数据，2 条重复数据');
  showAbnormalAlert.value = true;
  showDuplicateAlert.value = true;
};
const handleApprove = (item) => {
  Modal.confirm({
    title: '审核通过', content: `确定要审核通过 ${item.name} 的视力数据吗？`,
    onOk: () => { item.status = 'approved'; message.success(`已审核通过 ${item.name} 的数据`); }
  });
};
const handleReject = (item) => {
  Modal.confirm({
    title: '驳回数据', content: `确定要驳回 ${item.name} 的视力数据吗？驳回后需要重新录入。`,
    onOk: () => { item.status = 'pending'; message.warning(`已驳回 ${item.name} 的数据`); }
  });
};
const handleSpotCheck = (item) => {
  Modal.info({ title: '抽检数据', content: `正在抽检 ${item.name} 的视力数据，请区县教育局审核确认。`, okText: '知道了' });
};
const handleEdit = (item) => {
  Modal.info({ title: '编辑数据', content: `编辑 ${item.name} 的视力数据`, okText: '知道了' });
};
const handleDelete = (item) => {
  Modal.confirm({
    title: '删除数据', content: `确定要删除 ${item.name} 的视力数据吗？`, okText: '确定', cancelText: '取消',
    onOk: () => {
      const index = visionData.value.findIndex(d => d.id === item.id);
      if (index > -1) { visionData.value.splice(index, 1); message.success('删除成功'); }
    }
  });
};
const handleMergeDuplicate = (item) => {
  Modal.confirm({
    title: '合并去重', content: `确定要合并 ${item.name}（学号：${item.studentNo}）的 ${item.count} 条重复数据吗？将保留最新的一条。`,
    onOk: () => {
      message.success(`已合并 ${item.name} 的重复数据`);
      const index = duplicateList.value.findIndex(d => d.id === item.id);
      if (index > -1) { duplicateList.value.splice(index, 1); }
      if (duplicateList.value.length === 0) { showDuplicateAlert.value = false; }
    }
  });
};
const handleVisionFilter = () => {
  message.success(`筛选条件：状态[${visionFilter.status}]，关键词[${visionFilter.keyword || '无'}]`);
};

// ================= 口腔操作 =================
const handleOralSingleEntry = () => {
  Modal.info({ title: '单个录入', content: '打开单个录入表单，支持手动输入学生口腔健康数据', okText: '知道了' });
};
const handleOralBatchImport = () => {
  Modal.info({ title: 'Excel批量导入', content: '支持上传Excel文件，批量导入学生口腔健康数据。支持 .xlsx, .xls 格式', okText: '知道了' });
};
const handleOralDeviceImport = () => {
  Modal.info({ title: '设备自动采集', content: '连接口腔检测设备，自动采集学生口腔数据并上传至系统', okText: '知道了' });
};
const handleOralCheckData = () => {
  message.success('数据校验完成！发现 2 条异常数据');
  showOralAbnormalAlert.value = true;
};
const handleOralApprove = (item) => {
  Modal.confirm({ title: '审核通过', content: `确定要审核通过 ${item.name} 的口腔数据吗？`,
    onOk: () => { item.status = 'approved'; message.success(`已审核通过 ${item.name} 的口腔数据`); } });
};
const handleOralReject = (item) => {
  Modal.confirm({ title: '驳回数据', content: `确定要驳回 ${item.name} 的口腔数据吗？驳回后需要重新录入。`,
    onOk: () => { item.status = 'pending'; message.warning(`已驳回 ${item.name} 的口腔数据`); } });
};
const handleOralSpotCheck = (item) => {
  Modal.info({ title: '抽检数据', content: `正在抽检 ${item.name} 的口腔数据，请区县教育局审核确认。`, okText: '知道了' });
};
const handleOralEdit = (item) => {
  Modal.info({ title: '编辑数据', content: `编辑 ${item.name} 的口腔数据`, okText: '知道了' });
};
const handleOralDelete = (item) => {
  Modal.confirm({ title: '删除数据', content: `确定要删除 ${item.name} 的口腔数据吗？`, okText: '确定', cancelText: '取消',
    onOk: () => {
      const index = oralData.value.findIndex(d => d.id === item.id);
      if (index > -1) { oralData.value.splice(index, 1); message.success('删除成功'); }
    }
  });
};
const handleOralFilter = () => {
  message.success(`筛选条件：状态[${oralFilter.status}]，关键词[${oralFilter.keyword || '无'}]`);
};

// ================= 心理操作 =================
const handleMentalSingleEntry = () => {
  Modal.info({ title: '单个录入', content: '打开单个录入表单，支持手动输入学生心理健康数据', okText: '知道了' });
};
const handleMentalBatchImport = () => {
  Modal.info({ title: 'Excel批量导入', content: '支持上传Excel文件，批量导入学生心理健康数据。支持 .xlsx, .xls 格式', okText: '知道了' });
};
const handleMentalDeviceImport = () => {
  Modal.info({ title: '设备自动采集', content: '连接心理测评设备，自动采集学生心理健康数据并上传至系统', okText: '知道了' });
};
const handleMentalCheckData = () => {
  message.success('数据校验完成！发现 2 条异常数据');
  showMentalAbnormalAlert.value = true;
};
const handleMentalApprove = (item) => {
  Modal.confirm({ title: '审核通过', content: `确定要审核通过 ${item.name} 的心理健康数据吗？`,
    onOk: () => { item.status = 'approved'; message.success(`已审核通过 ${item.name} 的心理健康数据`); } });
};
const handleMentalReject = (item) => {
  Modal.confirm({ title: '驳回数据', content: `确定要驳回 ${item.name} 的心理健康数据吗？驳回后需要重新录入。`,
    onOk: () => { item.status = 'pending'; message.warning(`已驳回 ${item.name} 的心理健康数据`); } });
};
const handleMentalSpotCheck = (item) => {
  Modal.info({ title: '抽检数据', content: `正在抽检 ${item.name} 的心理健康数据，请区县教育局审核确认。`, okText: '知道了' });
};
const handleMentalEdit = (item) => {
  Modal.info({ title: '编辑数据', content: `编辑 ${item.name} 的心理健康数据`, okText: '知道了' });
};
const handleMentalDelete = (item) => {
  Modal.confirm({ title: '删除数据', content: `确定要删除 ${item.name} 的心理健康数据吗？`, okText: '确定', cancelText: '取消',
    onOk: () => {
      const index = mentalData.value.findIndex(d => d.id === item.id);
      if (index > -1) { mentalData.value.splice(index, 1); message.success('删除成功'); }
    }
  });
};
const handleMentalFilter = () => {
  message.success(`筛选条件：状态[${mentalFilter.status}]，关键词[${mentalFilter.keyword || '无'}]`);
};

// ================= 健康体重操作 =================
const handleWeightSingleEntry = () => {
  Modal.info({ title: '单个录入', content: '打开单个录入表单，支持手动输入学生健康体重数据', okText: '知道了' });
};
const handleWeightBatchImport = () => {
  Modal.info({ title: 'Excel批量导入', content: '支持上传Excel文件，批量导入学生健康体重数据。支持 .xlsx, .xls 格式', okText: '知道了' });
};
const handleWeightDeviceImport = () => {
  Modal.info({ title: '设备自动采集', content: '连接身高体重测量设备，自动采集学生健康体重数据并上传至系统', okText: '知道了' });
};
const handleWeightCheckData = () => {
  message.success('数据校验完成！发现 2 条异常数据');
  showWeightAbnormalAlert.value = true;
};
const handleWeightApprove = (item) => {
  Modal.confirm({ title: '审核通过', content: `确定要审核通过 ${item.name} 的健康体重数据吗？`,
    onOk: () => { item.status = 'approved'; message.success(`已审核通过 ${item.name} 的健康体重数据`); } });
};
const handleWeightReject = (item) => {
  Modal.confirm({ title: '驳回数据', content: `确定要驳回 ${item.name} 的健康体重数据吗？驳回后需要重新录入。`,
    onOk: () => { item.status = 'pending'; message.warning(`已驳回 ${item.name} 的健康体重数据`); } });
};
const handleWeightSpotCheck = (item) => {
  Modal.info({ title: '抽检数据', content: `正在抽检 ${item.name} 的健康体重数据，请区县教育局审核确认。`, okText: '知道了' });
};
const handleWeightEdit = (item) => {
  Modal.info({ title: '编辑数据', content: `编辑 ${item.name} 的健康体重数据`, okText: '知道了' });
};
const handleWeightDelete = (item) => {
  Modal.confirm({ title: '删除数据', content: `确定要删除 ${item.name} 的健康体重数据吗？`, okText: '确定', cancelText: '取消',
    onOk: () => {
      const index = weightData.value.findIndex(d => d.id === item.id);
      if (index > -1) { weightData.value.splice(index, 1); message.success('删除成功'); }
    }
  });
};
const handleWeightFilter = () => {
  message.success(`筛选条件：状态[${weightFilter.status}]，关键词[${weightFilter.keyword || '无'}]`);
};

// ================= 骨骼健康操作 =================
const handleBoneSingleEntry = () => {
  Modal.info({ title: '单个录入', content: '打开单个录入表单，支持手动输入学生骨骼健康数据', okText: '知道了' });
};
const handleBoneBatchImport = () => {
  Modal.info({ title: 'Excel批量导入', content: '支持上传Excel文件，批量导入学生骨骼健康数据。支持 .xlsx, .xls 格式', okText: '知道了' });
};
const handleBoneDeviceImport = () => {
  Modal.info({ title: '设备自动采集', content: '连接骨密度检测设备，自动采集学生骨骼健康数据并上传至系统', okText: '知道了' });
};
const handleBoneCheckData = () => {
  message.success('数据校验完成！发现 2 条异常数据');
  showBoneAbnormalAlert.value = true;
};
const handleBoneApprove = (item) => {
  Modal.confirm({ title: '审核通过', content: `确定要审核通过 ${item.name} 的骨骼健康数据吗？`,
    onOk: () => { item.status = 'approved'; message.success(`已审核通过 ${item.name} 的骨骼健康数据`); } });
};
const handleBoneReject = (item) => {
  Modal.confirm({ title: '驳回数据', content: `确定要驳回 ${item.name} 的骨骼健康数据吗？驳回后需要重新录入。`,
    onOk: () => { item.status = 'pending'; message.warning(`已驳回 ${item.name} 的骨骼健康数据`); } });
};
const handleBoneSpotCheck = (item) => {
  Modal.info({ title: '抽检数据', content: `正在抽检 ${item.name} 的骨骼健康数据，请区县教育局审核确认。`, okText: '知道了' });
};
const handleBoneEdit = (item) => {
  Modal.info({ title: '编辑数据', content: `编辑 ${item.name} 的骨骼健康数据`, okText: '知道了' });
};
const handleBoneDelete = (item) => {
  Modal.confirm({ title: '删除数据', content: `确定要删除 ${item.name} 的骨骼健康数据吗？`, okText: '确定', cancelText: '取消',
    onOk: () => {
      const index = boneData.value.findIndex(d => d.id === item.id);
      if (index > -1) { boneData.value.splice(index, 1); message.success('删除成功'); }
    }
  });
};
const handleBoneFilter = () => {
  message.success(`筛选条件：状态[${boneFilter.status}]，关键词[${boneFilter.keyword || '无'}]`);
};

// ================= 生命周期 =================
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
</script>

<style scoped>
/* ===== CSS Variables (Dark Mode) ===== */
.dashboard-wrapper {
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

.dashboard-wrapper > * {
  position: relative;
  z-index: 2;
}

.dashboard-wrapper.theme-light {
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

/* ===== Scrollbar ===== */
.dashboard-wrapper ::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}
.dashboard-wrapper ::-webkit-scrollbar-track {
  background: transparent;
}
.dashboard-wrapper ::-webkit-scrollbar-thumb {
  background: var(--scrollbar-thumb);
  border-radius: 4px;
}
.dashboard-wrapper ::-webkit-scrollbar-thumb:hover {
  background: var(--border-strong);
}

/* ===== Layout ===== */
.dashboard-wrapper > * {
  max-width: 1600px;
  margin-left: auto;
  margin-right: auto;
  padding-left: 20px;
  padding-right: 20px;
}

/* ===== Page Header ===== */
.page-header {
  margin-bottom: 12px;
  position: relative;
  padding: 10px 16px;
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
.header-deco-r .hd-dot { animation-delay: -0.7s; }
@keyframes dot-blink {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.3; transform: scale(0.6); }
}
.header-main {
  display: flex;
  align-items: center;
  gap: 20px;
  flex: 1;
}
.page-title {
  font-size: 20px;
  font-weight: 700;
  color: var(--text-strong);
  margin: 0;
  letter-spacing: 2px;
  position: relative;
  display: flex;
  align-items: center;
  gap: 4px;
  flex-shrink: 0;
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
.header-stats {
  display: flex;
  align-items: center;
  gap: 12px;
}
.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1px;
}
.stat-label {
  font-size: 11px;
  color: var(--text-dim);
  letter-spacing: 0.5px;
}
.stat-value {
  font-size: 18px;
  font-weight: 700;
  color: var(--primary);
  font-family: 'Orbitron', 'Consolas', monospace;
  text-shadow: 0 0 10px var(--glow);
  letter-spacing: 1px;
}
.stat-value.stat-warn { color: var(--warning); text-shadow: 0 0 10px rgba(var(--warning), 0.4); }
.stat-value.stat-ok { color: var(--success); text-shadow: 0 0 10px rgba(var(--success), 0.4); }
.stat-divider {
  width: 1px;
  height: 24px;
  background: linear-gradient(180deg, transparent, var(--border-strong), transparent);
}

/* ===== Tab Bar ===== */
.tab-bar {
  display: flex;
  gap: 6px;
  margin-bottom: 10px;
  padding: 6px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  box-shadow: var(--shadow);
  backdrop-filter: blur(10px);
  position: relative;
}
.tab-bar::before {
  content: '';
  position: absolute;
  top: 0; left: 16px; right: 16px;
  height: 1px;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  opacity: 0.4;
}
.tab-item {
  flex: 1;
  text-align: center;
  padding: 10px 16px;
  cursor: pointer;
  font-weight: 500;
  font-size: 14px;
  letter-spacing: 0.5px;
  border-radius: 8px;
  transition: all 0.25s ease;
  border: 1px solid transparent;
  background: transparent;
  color: var(--text-dim);
  position: relative;
  overflow: hidden;
}
.tab-item:hover:not(.active) {
  color: var(--text);
  background: var(--bg-hover);
  border-color: var(--border-soft);
}
.tab-item.active {
  background: linear-gradient(135deg, var(--primary-bg), rgba(124, 58, 237, 0.15));
  color: var(--primary);
  border-color: var(--border-strong);
  box-shadow: 0 0 20px var(--glow), inset 0 0 0 1px rgba(56, 189, 248, 0.25);
}
.tab-item.active::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, var(--primary), var(--secondary));
}
.tab-item .tab-icon { font-size: 16px; }
.tab-label { margin-left: 4px; }
.tab-badge {
  position: absolute;
  top: 4px;
  right: 6px;
  min-width: 16px;
  height: 16px;
  padding: 0 4px;
  background: var(--danger);
  color: #fff;
  font-size: 10px;
  font-weight: 700;
  border-radius: 8px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 0 8px rgba(251, 113, 133, 0.5);
}

/* ===== Stats Cards ===== */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
  margin-bottom: 10px;
}
.stat-card {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 14px 16px;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  box-shadow: var(--shadow);
  backdrop-filter: blur(10px);
  display: flex;
  align-items: center;
  gap: 12px;
}
.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-card);
  border-color: var(--border-strong);
}
.stat-card::before {
  content: '';
  position: absolute;
  top: 0; left: 0;
  width: 3px;
  height: 100%;
  background: linear-gradient(180deg, var(--card-accent, var(--primary)), transparent);
  border-radius: 3px 0 0 3px;
}
.stat-icon {
  width: 44px;
  height: 44px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 22px;
  flex-shrink: 0;
}
.stat-icon.blue { background: linear-gradient(135deg, rgba(56, 189, 248, 0.2), rgba(56, 189, 248, 0.05)); color: #38bdf8; box-shadow: 0 0 15px rgba(56, 189, 248, 0.3); }
.stat-icon.green { background: linear-gradient(135deg, rgba(52, 211, 153, 0.2), rgba(52, 211, 153, 0.05)); color: #34d399; box-shadow: 0 0 15px rgba(52, 211, 153, 0.3); }
.stat-icon.orange { background: linear-gradient(135deg, rgba(251, 191, 36, 0.2), rgba(251, 191, 36, 0.05)); color: #fbbf24; box-shadow: 0 0 15px rgba(251, 191, 36, 0.3); }
.stat-icon.red { background: linear-gradient(135deg, rgba(251, 113, 133, 0.2), rgba(251, 113, 133, 0.05)); color: #fb7185; box-shadow: 0 0 15px rgba(251, 113, 133, 0.3); }
.stat-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}
.stat-card .stat-label {
  font-size: 12px;
  color: var(--text-dim);
  letter-spacing: 0.5px;
}
.stat-card .stat-value {
  font-size: 22px;
  font-weight: 700;
  color: var(--text-strong);
  font-family: 'Orbitron', 'Consolas', monospace;
  letter-spacing: 1px;
  text-shadow: none;
}

/* ===== Toolbar ===== */
.toolbar {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  margin-bottom: 10px;
  align-items: center;
}
.toolbar-left {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  flex: 1;
}
.toolbar-right {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}
.toolbar-tip {
  font-size: 12px;
  color: var(--text-dim);
  padding: 6px 12px;
  background: var(--primary-bg);
  border: 1px solid var(--border-soft);
  border-radius: 6px;
  letter-spacing: 0.3px;
}
.btn {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  padding: 6px 14px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  color: var(--text);
  border-radius: 6px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 500;
  letter-spacing: 0.3px;
  transition: all 0.2s ease;
  position: relative;
  overflow: hidden;
  font-family: inherit;
}
.btn:hover {
  border-color: var(--border-strong);
  color: var(--primary);
  background: var(--bg-hover);
  box-shadow: 0 0 12px var(--glow);
}
.btn .icon { font-size: 14px; }
.btn-primary {
  background: linear-gradient(135deg, var(--primary), #0284c7);
  border-color: var(--primary);
  color: #fff;
}
.btn-primary:hover {
  box-shadow: 0 0 20px var(--glow);
  transform: translateY(-1px);
  color: #fff;
}
.btn-success {
  background: linear-gradient(135deg, var(--success), #059669);
  border-color: var(--success);
  color: #fff;
}
.btn-success:hover {
  box-shadow: 0 0 20px rgba(52, 211, 153, 0.5);
  color: #fff;
}
.btn-info {
  background: linear-gradient(135deg, #a78bfa, #7c3aed);
  border-color: #a78bfa;
  color: #fff;
}
.btn-info:hover {
  box-shadow: 0 0 20px rgba(167, 139, 250, 0.5);
  color: #fff;
}
.btn-warning {
  background: linear-gradient(135deg, var(--warning), #d97706);
  border-color: var(--warning);
  color: #1f2937;
}
.btn-warning:hover {
  box-shadow: 0 0 20px rgba(251, 191, 36, 0.5);
  color: #1f2937;
}
.btn-danger {
  background: linear-gradient(135deg, var(--danger), #e11d48);
  border-color: var(--danger);
  color: #fff;
}
.btn-danger:hover {
  box-shadow: 0 0 20px rgba(251, 113, 133, 0.5);
  color: #fff;
}
.btn-sm {
  padding: 3px 10px;
  font-size: 12px;
}
.btn-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 6px;
  cursor: pointer;
  font-size: 13px;
  transition: all 0.2s ease;
  color: var(--text);
  padding: 0;
}
.btn-icon:hover {
  transform: translateY(-1px);
  box-shadow: 0 0 10px var(--glow);
}
.btn-icon.approve:hover { border-color: var(--success); color: var(--success); box-shadow: 0 0 10px rgba(52, 211, 153, 0.4); }
.btn-icon.reject:hover { border-color: var(--danger); color: var(--danger); box-shadow: 0 0 10px rgba(251, 113, 133, 0.4); }
.btn-icon.spot:hover { border-color: var(--primary); color: var(--primary); box-shadow: 0 0 10px var(--glow); }
.btn-icon.edit:hover { border-color: var(--warning); color: var(--warning); box-shadow: 0 0 10px rgba(251, 191, 36, 0.4); }
.btn-icon.delete:hover { border-color: var(--danger); color: var(--danger); box-shadow: 0 0 10px rgba(251, 113, 133, 0.4); }

/* ===== Flow Chart ===== */
.flow-chart {
  display: flex;
  align-items: center;
  gap: 0;
  padding: 16px 12px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  box-shadow: var(--shadow);
  overflow-x: auto;
  margin-bottom: 10px;
  backdrop-filter: blur(10px);
}
.flow-step {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
  min-width: 130px;
  position: relative;
  flex-shrink: 0;
}
.step-icon {
  width: 42px;
  height: 42px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  border: 2px solid var(--border);
  background: var(--bg-card);
  color: var(--text-muted);
  position: relative;
  z-index: 2;
  transition: all 0.3s ease;
}
.flow-step:has(.step-status.done) .step-icon {
  background: linear-gradient(135deg, var(--success), #059669);
  border-color: var(--success);
  box-shadow: 0 0 20px rgba(52, 211, 153, 0.5);
}
.flow-step:has(.step-status.doing) .step-icon {
  background: linear-gradient(135deg, var(--primary), #0284c7);
  border-color: var(--primary);
  box-shadow: 0 0 20px var(--glow);
  animation: pulse-glow 2s ease-in-out infinite;
}
@keyframes pulse-glow {
  0%, 100% { box-shadow: 0 0 20px var(--glow); }
  50% { box-shadow: 0 0 30px var(--glow), 0 0 40px var(--glow); }
}
.step-label {
  font-size: 12px;
  color: var(--text-dim);
  text-align: center;
}
.step-status {
  font-size: 11px;
  padding: 2px 8px;
  border-radius: 10px;
  background: var(--bg-hover);
  color: var(--text-muted);
  letter-spacing: 0.3px;
}
.step-status.done {
  background: rgba(52, 211, 153, 0.15);
  color: var(--success);
  border: 1px solid rgba(52, 211, 153, 0.3);
}
.step-status.doing {
  background: var(--primary-bg);
  color: var(--primary);
  border: 1px solid var(--border-strong);
}
.step-status.pending {
  background: rgba(251, 113, 133, 0.12);
  color: var(--danger);
  border: 1px solid rgba(251, 113, 133, 0.3);
}
.flow-arrow {
  flex: 1;
  min-width: 30px;
  text-align: center;
  color: var(--primary);
  font-size: 18px;
  font-weight: bold;
  opacity: 0.6;
  animation: arrow-pulse 2s ease-in-out infinite;
}
@keyframes arrow-pulse {
  0%, 100% { opacity: 0.4; }
  50% { opacity: 0.9; }
}

/* ===== Tab Content ===== */
.tab-content { margin-bottom: 12px; }
.tab-panel { animation: fade-in 0.3s ease; }
@keyframes fade-in {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

/* ===== Data Table ===== */
.data-table-wrapper {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  box-shadow: var(--shadow);
  overflow: hidden;
  margin-bottom: 10px;
  backdrop-filter: blur(10px);
  position: relative;
}
.data-table-wrapper::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 1px;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  opacity: 0.4;
}
.table-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 14px 16px;
  border-bottom: 1px solid var(--border);
  background: linear-gradient(180deg, rgba(56, 189, 248, 0.04), transparent);
}
.table-header h3 {
  margin: 0;
  font-size: 15px;
  font-weight: 600;
  color: var(--text-strong);
  letter-spacing: 0.5px;
}
.table-filter {
  display: flex;
  gap: 8px;
  align-items: center;
}
.data-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
}
.data-table thead {
  background: linear-gradient(180deg, rgba(56, 189, 248, 0.08), rgba(56, 189, 248, 0.02));
}
.data-table thead th {
  padding: 12px 14px;
  text-align: left;
  font-weight: 600;
  color: var(--primary);
  font-size: 12px;
  letter-spacing: 0.5px;
  border-bottom: 1px solid var(--border);
  position: relative;
  white-space: nowrap;
}
.data-table thead th::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(90deg, var(--primary), var(--secondary));
  opacity: 0.6;
}
.data-table tbody tr {
  transition: all 0.2s ease;
  border-bottom: 1px solid var(--border-soft);
}
.data-table tbody tr:last-child { border-bottom: none; }
.data-table tbody tr:hover {
  background: var(--bg-hover);
  box-shadow: inset 3px 0 0 var(--primary);
}
.data-table tbody tr:hover td { color: var(--text-strong); }
.data-table tbody td {
  padding: 12px 14px;
  color: var(--text);
  transition: color 0.2s ease;
  white-space: nowrap;
}
.student-no {
  font-family: 'Orbitron', 'Consolas', monospace;
  color: var(--primary);
  font-weight: 500;
  letter-spacing: 0.5px;
}
.action-btns {
  display: flex;
  gap: 5px;
  justify-content: flex-start;
}
.level-tag {
  display: inline-flex;
  padding: 2px 8px;
  border-radius: 10px;
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.3px;
}
.level-tag.excellent { background: rgba(52, 211, 153, 0.15); color: var(--success); }
.level-tag.good { background: var(--primary-bg); color: var(--primary); }
.level-tag.normal { background: rgba(251, 191, 36, 0.15); color: var(--warning); }
.level-tag.poor { background: rgba(251, 113, 133, 0.15); color: var(--danger); }
.table-pagination {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  border-top: 1px solid var(--border);
  color: var(--text-dim);
  font-size: 12px;
}
.pagination-btns {
  display: flex;
  gap: 5px;
}

/* ===== Filter Inputs ===== */
.filter-bar {
  display: flex;
  gap: 10px;
  margin-bottom: 10px;
  flex-wrap: wrap;
  align-items: center;
}
.filter-input {
  padding: 7px 12px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 6px;
  color: var(--text);
  font-size: 13px;
  transition: all 0.2s ease;
  width: 180px;
  outline: none;
}
.filter-input:focus {
  border-color: var(--primary);
  box-shadow: 0 0 0 2px var(--primary-bg);
  width: 220px;
}
.filter-select {
  padding: 7px 12px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 6px;
  color: var(--text);
  font-size: 13px;
  transition: all 0.2s ease;
  outline: none;
  cursor: pointer;
}
.filter-select:focus,
.filter-select:hover {
  border-color: var(--primary);
  box-shadow: 0 0 0 2px var(--primary-bg);
}

/* ===== Status Tags ===== */
.status-tag {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.5px;
  position: relative;
}
.status-tag::before {
  content: '';
  width: 5px;
  height: 5px;
  border-radius: 50%;
  background: currentColor;
}
.status-tag.approved {
  background: rgba(52, 211, 153, 0.15);
  color: var(--success);
  border: 1px solid rgba(52, 211, 153, 0.4);
}
.status-tag.pending {
  background: rgba(251, 191, 36, 0.15);
  color: var(--warning);
  border: 1px solid rgba(251, 191, 36, 0.4);
}
.status-tag.rejected {
  background: rgba(251, 113, 133, 0.15);
  color: var(--danger);
  border: 1px solid rgba(251, 113, 133, 0.4);
}
.status-tag.doing {
  background: var(--primary-bg);
  color: var(--primary);
  border: 1px solid var(--border-strong);
}

/* ===== Pagination ===== */
.page-btn {
  min-width: 32px;
  height: 32px;
  padding: 0 10px;
  background: var(--bg-card);
  border: 1px solid var(--border);
  color: var(--text);
  border-radius: 6px;
  cursor: pointer;
  font-size: 13px;
  transition: all 0.2s ease;
}
.page-btn:hover:not(:disabled) {
  border-color: var(--primary);
  color: var(--primary);
  box-shadow: 0 0 10px var(--glow);
}
.page-btn.active {
  background: linear-gradient(135deg, var(--primary), #0284c7);
  border-color: var(--primary);
  color: #fff;
  box-shadow: 0 0 12px var(--glow);
}
.page-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

/* ===== Alert Boxes ===== */
.abnormal-alert,
.duplicate-alert {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  margin-bottom: 10px;
  overflow: hidden;
  box-shadow: var(--shadow);
  backdrop-filter: blur(10px);
}
.alert-header {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: linear-gradient(90deg, rgba(251, 191, 36, 0.1), rgba(251, 113, 133, 0.05));
  border-bottom: 1px solid var(--border-soft);
}
.alert-icon {
  font-size: 18px;
}
.alert-title {
  font-weight: 600;
  font-size: 13px;
  color: var(--text-strong);
  letter-spacing: 0.5px;
  flex: 1;
}
.alert-close {
  background: transparent;
  border: none;
  color: var(--text-dim);
  cursor: pointer;
  font-size: 14px;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: all 0.2s;
  padding: 0;
}
.alert-close:hover {
  background: var(--bg-hover);
  color: var(--text);
}
.alert-body {
  padding: 12px 16px;
}
.alert-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 8px 12px;
  background: var(--bg-soft);
  border-radius: 6px;
  margin-bottom: 6px;
  border-left: 3px solid var(--warning);
  transition: all 0.2s ease;
}
.alert-item:last-child { margin-bottom: 0; }
.alert-item:hover {
  background: var(--bg-hover);
  transform: translateX(2px);
}
.alert-name {
  font-weight: 600;
  color: var(--text-strong);
  min-width: 70px;
}
.alert-detail {
  flex: 1;
  color: var(--text-dim);
  font-size: 12px;
}
.alert-suggest {
  color: var(--primary);
  font-size: 12px;
  padding: 2px 8px;
  background: var(--primary-bg);
  border-radius: 4px;
}

/* ===== Responsive ===== */
@media (max-width: 1200px) {
  .stats-grid { grid-template-columns: repeat(2, 1fr); }
}
@media (max-width: 768px) {
  .stats-grid { grid-template-columns: 1fr; }
  .page-header { flex-wrap: wrap; }
  .header-stats { margin-top: 8px; width: 100%; justify-content: center; }
  .filter-input { width: 140px; }
  .filter-input:focus { width: 160px; }
  .flow-chart { flex-direction: column; align-items: flex-start; }
  .flow-arrow { display: none; }
  .flow-step { flex-direction: row; gap: 10px; min-width: auto; }
  .table-header { flex-direction: column; align-items: flex-start; }
  .table-filter { width: 100%; }
}
</style>
