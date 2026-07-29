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
          <span class="title-text">预警与干预管理</span>
          <span class="title-bracket">】</span>
          <span class="title-shine"></span>
        </h1>
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
            <div class="stat-icon red">🏫</div>
            <div class="stat-info">
              <div class="stat-label">高近视率预警学校</div>
              <div class="stat-value">{{ visionStats.warningSchools }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">🆕</div>
            <div class="stat-info">
              <div class="stat-label">新发近视预警学生</div>
              <div class="stat-value">{{ visionStats.newCases }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon blue">📋</div>
            <div class="stat-info">
              <div class="stat-label">进行中干预方案</div>
              <div class="stat-value">{{ visionStats.activeInterventions }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info">
              <div class="stat-label">干预有效改善</div>
              <div class="stat-value">{{ visionStats.improved }}%</div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>🚨 预警列表</h3>
          <div class="section-actions">
            <button class="btn btn-sm btn-primary" @click="handleRefresh">🔄 刷新</button>
          </div>
        </div>

        <div class="alert-list">
          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🏫</span>
              <span class="alert-group-title">高近视率学校预警（近视率 ≥ 60%）</span>
              <span class="alert-group-count">{{ warningSchools.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="school in warningSchools" :key="school.id" class="alert-item warning">
                <div class="alert-info">
                  <span class="alert-name">{{ school.name }}</span>
                  <span class="alert-detail">近视率 {{ school.rate }}%</span>
                  <span class="alert-detail">监测人数 {{ school.total }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleViewSchool(school)">查看详情</button>
                  <button class="btn btn-sm btn-success" @click="handleCreateIntervention(school)">制定方案</button>
                </div>
              </div>
            </div>
          </div>

          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🆕</span>
              <span class="alert-group-title">新发近视学生实时提醒</span>
              <span class="alert-group-count new">{{ newCaseStudents.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="student in newCaseStudents" :key="student.id" class="alert-item info">
                <div class="alert-info">
                  <span class="alert-name">{{ student.name }}</span>
                  <span class="alert-detail">学号 {{ student.studentNo }}</span>
                  <span class="alert-detail">年级 {{ student.grade }}</span>
                  <span class="alert-detail">班级 {{ student.class }}</span>
                  <span class="alert-detail">发现时间 {{ student.time }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleViewStudent(student)">查看档案</button>
                  <button class="btn btn-sm btn-warning" @click="handleTrackStudent(student)">跟踪记录</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📋 干预方案管理</h3>
          <div class="section-actions">
            <button class="btn btn-sm btn-success" @click="handleAddIntervention">➕ 新增方案</button>
          </div>
        </div>

        <div class="intervention-list">
          <div v-for="intervention in interventions" :key="intervention.id" class="intervention-card">
            <div class="intervention-header">
              <div class="intervention-title">
                <span class="intervention-icon">{{ intervention.icon }}</span>
                <span class="intervention-name">{{ intervention.name }}</span>
                <span class="intervention-status" :class="intervention.statusClass">{{ intervention.status }}</span>
              </div>
              <div class="intervention-school">{{ intervention.school }}</div>
            </div>
            <div class="intervention-body">
              <div class="intervention-info">
                <span>开始：{{ intervention.startDate }}</span>
                <span>预计：{{ intervention.endDate }}</span>
                <span>参与：{{ intervention.participants }} 人</span>
              </div>
              <div class="intervention-progress">
                <span class="progress-label">效果跟踪</span>
                <div class="progress-bar">
                  <div class="progress-fill" :style="{ width: intervention.progress + '%' }"></div>
                </div>
                <span class="progress-value">{{ intervention.progress }}%</span>
              </div>
            </div>
            <div class="intervention-actions">
              <button class="btn btn-sm btn-info" @click="handleViewIntervention(intervention)">查看详情</button>
              <button class="btn btn-sm btn-primary" @click="handleTrackIntervention(intervention)">跟踪记录</button>
              <button class="btn btn-sm btn-success" @click="handleEvaluateIntervention(intervention)">效果评估</button>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📊 试点学校调控前后数据对比</h3>
        </div>

        <div class="compare-chart-wrapper">
          <div class="chart-card">
            <div ref="compareChartRef" class="chart-container"></div>
          </div>
          <div class="compare-summary">
            <div class="compare-item">
              <span class="compare-label">试点学校平均近视率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 56.8%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 48.2%</span>
              </div>
              <span class="compare-change good">下降 8.6% ✅</span>
            </div>
            <div class="compare-item">
              <span class="compare-label">非试点学校平均近视率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 53.2%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 54.1%</span>
              </div>
              <span class="compare-change bad">上升 0.9% ⚠️</span>
            </div>
          </div>
        </div>

        <div v-if="showEvaluateModal" class="modal-overlay" @click.self="showEvaluateModal = false">
          <div class="modal-content">
            <div class="modal-header">
              <h3>📊 干预效果评估</h3>
              <button class="modal-close" @click="showEvaluateModal = false">✕</button>
            </div>
            <div class="modal-body">
              <div class="evaluate-grid">
                <div class="evaluate-item">
                  <span class="evaluate-label">干预前近视率</span>
                  <span class="evaluate-value">{{ evaluateData.beforeRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">干预后近视率</span>
                  <span class="evaluate-value">{{ evaluateData.afterRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">改善幅度</span>
                  <span class="evaluate-value" :class="evaluateData.change > 0 ? 'improve' : 'worsen'">
                    {{ evaluateData.change > 0 ? '⬆' : '⬇' }} {{ Math.abs(evaluateData.change) }}%
                  </span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">效果评价</span>
                  <span class="evaluate-value" :class="evaluateData.ratingClass">
                    {{ evaluateData.rating }}
                  </span>
                </div>
              </div>
              <div ref="evaluateChartRef" class="evaluate-chart"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 口腔 Tab ===== -->
      <div v-show="activeTab === 'oral'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon red">🏫</div>
            <div class="stat-info">
              <div class="stat-label">高龋风险预警学校</div>
              <div class="stat-value">{{ oralStats.warningSchools }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">🆕</div>
            <div class="stat-info">
              <div class="stat-label">新发龋齿预警学生</div>
              <div class="stat-value">{{ oralStats.newCases }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon blue">📋</div>
            <div class="stat-info">
              <div class="stat-label">进行中干预方案</div>
              <div class="stat-value">{{ oralStats.activeInterventions }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info">
              <div class="stat-label">干预有效改善</div>
              <div class="stat-value">{{ oralStats.improved }}%</div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>🚨 预警列表</h3>
        </div>

        <div class="alert-list">
          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🏫</span>
              <span class="alert-group-title">高龋风险学校预警（龋患率 ≥ 50%）</span>
              <span class="alert-group-count">{{ oralWarningSchools.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="school in oralWarningSchools" :key="school.id" class="alert-item warning">
                <div class="alert-info">
                  <span class="alert-name">{{ school.name }}</span>
                  <span class="alert-detail">龋患率 {{ school.rate }}%</span>
                  <span class="alert-detail">监测人数 {{ school.total }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleOralViewSchool(school)">查看详情</button>
                  <button class="btn btn-sm btn-success" @click="handleOralCreateIntervention(school)">制定方案</button>
                </div>
              </div>
            </div>
          </div>

          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🆕</span>
              <span class="alert-group-title">新发龋齿学生实时提醒</span>
              <span class="alert-group-count new">{{ oralNewCaseStudents.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="student in oralNewCaseStudents" :key="student.id" class="alert-item info">
                <div class="alert-info">
                  <span class="alert-name">{{ student.name }}</span>
                  <span class="alert-detail">学号 {{ student.studentNo }}</span>
                  <span class="alert-detail">年级 {{ student.grade }}</span>
                  <span class="alert-detail">发现时间 {{ student.time }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleOralViewStudent(student)">查看档案</button>
                  <button class="btn btn-sm btn-warning" @click="handleOralTrackStudent(student)">跟踪记录</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📋 干预方案管理</h3>
          <div class="section-actions">
            <button class="btn btn-sm btn-success" @click="handleOralAddIntervention">➕ 新增方案</button>
          </div>
        </div>

        <div class="intervention-list">
          <div v-for="intervention in oralInterventions" :key="intervention.id" class="intervention-card">
            <div class="intervention-header">
              <div class="intervention-title">
                <span class="intervention-icon">{{ intervention.icon }}</span>
                <span class="intervention-name">{{ intervention.name }}</span>
                <span class="intervention-status" :class="intervention.statusClass">{{ intervention.status }}</span>
              </div>
              <div class="intervention-school">{{ intervention.school }}</div>
            </div>
            <div class="intervention-body">
              <div class="intervention-info">
                <span>开始：{{ intervention.startDate }}</span>
                <span>预计：{{ intervention.endDate }}</span>
                <span>参与：{{ intervention.participants }} 人</span>
              </div>
              <div class="intervention-progress">
                <span class="progress-label">效果跟踪</span>
                <div class="progress-bar">
                  <div class="progress-fill" :style="{ width: intervention.progress + '%' }"></div>
                </div>
                <span class="progress-value">{{ intervention.progress }}%</span>
              </div>
            </div>
            <div class="intervention-actions">
              <button class="btn btn-sm btn-info" @click="handleOralViewIntervention(intervention)">查看详情</button>
              <button class="btn btn-sm btn-primary" @click="handleOralTrackIntervention(intervention)">跟踪记录</button>
              <button class="btn btn-sm btn-success" @click="handleOralEvaluateIntervention(intervention)">效果评估</button>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📊 试点学校调控前后数据对比</h3>
        </div>

        <div class="compare-chart-wrapper">
          <div class="chart-card">
            <div ref="oralCompareChartRef" class="chart-container"></div>
          </div>
          <div class="compare-summary">
            <div class="compare-item">
              <span class="compare-label">试点学校平均龋患率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 52.8%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 44.2%</span>
              </div>
              <span class="compare-change good">下降 8.6% ✅</span>
            </div>
            <div class="compare-item">
              <span class="compare-label">非试点学校平均龋患率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 48.5%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 49.2%</span>
              </div>
              <span class="compare-change bad">上升 0.7% ⚠️</span>
            </div>
          </div>
        </div>

        <div v-if="showOralEvaluateModal" class="modal-overlay" @click.self="showOralEvaluateModal = false">
          <div class="modal-content">
            <div class="modal-header">
              <h3>📊 干预效果评估</h3>
              <button class="modal-close" @click="showOralEvaluateModal = false">✕</button>
            </div>
            <div class="modal-body">
              <div class="evaluate-grid">
                <div class="evaluate-item">
                  <span class="evaluate-label">干预前龋患率</span>
                  <span class="evaluate-value">{{ oralEvaluateData.beforeRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">干预后龋患率</span>
                  <span class="evaluate-value">{{ oralEvaluateData.afterRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">改善幅度</span>
                  <span class="evaluate-value" :class="oralEvaluateData.change > 0 ? 'improve' : 'worsen'">
                    {{ oralEvaluateData.change > 0 ? '⬆' : '⬇' }} {{ Math.abs(oralEvaluateData.change) }}%
                  </span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">效果评价</span>
                  <span class="evaluate-value" :class="oralEvaluateData.ratingClass">
                    {{ oralEvaluateData.rating }}
                  </span>
                </div>
              </div>
              <div ref="oralEvaluateChartRef" class="evaluate-chart"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 心理 Tab ===== -->
      <div v-show="activeTab === 'mental'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon red">🏫</div>
            <div class="stat-info">
              <div class="stat-label">高预警率学校</div>
              <div class="stat-value">{{ mentalStats.warningSchools }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">🆕</div>
            <div class="stat-info">
              <div class="stat-label">新发心理预警学生</div>
              <div class="stat-value">{{ mentalStats.newCases }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon blue">📋</div>
            <div class="stat-info">
              <div class="stat-label">进行中干预方案</div>
              <div class="stat-value">{{ mentalStats.activeInterventions }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info">
              <div class="stat-label">干预有效改善</div>
              <div class="stat-value">{{ mentalStats.improved }}%</div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>🚨 预警列表</h3>
        </div>

        <div class="alert-list">
          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🏫</span>
              <span class="alert-group-title">高预警率学校预警（预警率 ≥ 25%）</span>
              <span class="alert-group-count">{{ mentalWarningSchools.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="school in mentalWarningSchools" :key="school.id" class="alert-item warning">
                <div class="alert-info">
                  <span class="alert-name">{{ school.name }}</span>
                  <span class="alert-detail">预警率 {{ school.rate }}%</span>
                  <span class="alert-detail">监测人数 {{ school.total }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleMentalViewSchool(school)">查看详情</button>
                  <button class="btn btn-sm btn-success" @click="handleMentalCreateIntervention(school)">制定方案</button>
                </div>
              </div>
            </div>
          </div>

          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🆕</span>
              <span class="alert-group-title">新发心理预警学生实时提醒</span>
              <span class="alert-group-count new">{{ mentalNewCaseStudents.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="student in mentalNewCaseStudents" :key="student.id" class="alert-item info">
                <div class="alert-info">
                  <span class="alert-name">{{ student.name }}</span>
                  <span class="alert-detail">学号 {{ student.studentNo }}</span>
                  <span class="alert-detail">年级 {{ student.grade }}</span>
                  <span class="alert-detail">班级 {{ student.class }}</span>
                  <span class="alert-detail">发现时间 {{ student.time }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleMentalViewStudent(student)">查看档案</button>
                  <button class="btn btn-sm btn-warning" @click="handleMentalTrackStudent(student)">跟踪记录</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📋 干预方案管理</h3>
          <div class="section-actions">
            <button class="btn btn-sm btn-success" @click="handleMentalAddIntervention">➕ 新增方案</button>
          </div>
        </div>

        <div class="intervention-list">
          <div v-for="intervention in mentalInterventions" :key="intervention.id" class="intervention-card">
            <div class="intervention-header">
              <div class="intervention-title">
                <span class="intervention-icon">{{ intervention.icon }}</span>
                <span class="intervention-name">{{ intervention.name }}</span>
                <span class="intervention-status" :class="intervention.statusClass">{{ intervention.status }}</span>
              </div>
              <div class="intervention-school">{{ intervention.school }}</div>
            </div>
            <div class="intervention-body">
              <div class="intervention-info">
                <span>开始：{{ intervention.startDate }}</span>
                <span>预计：{{ intervention.endDate }}</span>
                <span>参与：{{ intervention.participants }} 人</span>
              </div>
              <div class="intervention-progress">
                <span class="progress-label">效果跟踪</span>
                <div class="progress-bar">
                  <div class="progress-fill" :style="{ width: intervention.progress + '%' }"></div>
                </div>
                <span class="progress-value">{{ intervention.progress }}%</span>
              </div>
            </div>
            <div class="intervention-actions">
              <button class="btn btn-sm btn-info" @click="handleMentalViewIntervention(intervention)">查看详情</button>
              <button class="btn btn-sm btn-primary" @click="handleMentalTrackIntervention(intervention)">跟踪记录</button>
              <button class="btn btn-sm btn-success" @click="handleMentalEvaluateIntervention(intervention)">效果评估</button>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📊 试点学校调控前后数据对比</h3>
        </div>

        <div class="compare-chart-wrapper">
          <div class="chart-card">
            <div ref="mentalCompareChartRef" class="chart-container"></div>
          </div>
          <div class="compare-summary">
            <div class="compare-item">
              <span class="compare-label">试点学校平均预警率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 28.5%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 20.2%</span>
              </div>
              <span class="compare-change good">下降 8.3% ✅</span>
            </div>
            <div class="compare-item">
              <span class="compare-label">非试点学校平均预警率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 24.5%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 25.2%</span>
              </div>
              <span class="compare-change bad">上升 0.7% ⚠️</span>
            </div>
          </div>
        </div>

        <div v-if="showMentalEvaluateModal" class="modal-overlay" @click.self="showMentalEvaluateModal = false">
          <div class="modal-content">
            <div class="modal-header">
              <h3>📊 干预效果评估</h3>
              <button class="modal-close" @click="showMentalEvaluateModal = false">✕</button>
            </div>
            <div class="modal-body">
              <div class="evaluate-grid">
                <div class="evaluate-item">
                  <span class="evaluate-label">干预前预警率</span>
                  <span class="evaluate-value">{{ mentalEvaluateData.beforeRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">干预后预警率</span>
                  <span class="evaluate-value">{{ mentalEvaluateData.afterRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">改善幅度</span>
                  <span class="evaluate-value" :class="mentalEvaluateData.change > 0 ? 'improve' : 'worsen'">
                    {{ mentalEvaluateData.change > 0 ? '⬆' : '⬇' }} {{ Math.abs(mentalEvaluateData.change) }}%
                  </span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">效果评价</span>
                  <span class="evaluate-value" :class="mentalEvaluateData.ratingClass">
                    {{ mentalEvaluateData.rating }}
                  </span>
                </div>
              </div>
              <div ref="mentalEvaluateChartRef" class="evaluate-chart"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 健康体重 Tab ===== -->
      <div v-show="activeTab === 'weight'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon red">🏫</div>
            <div class="stat-info">
              <div class="stat-label">高体重问题预警学校</div>
              <div class="stat-value">{{ weightStats.warningSchools }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">🆕</div>
            <div class="stat-info">
              <div class="stat-label">新发体重预警学生</div>
              <div class="stat-value">{{ weightStats.newCases }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon blue">📋</div>
            <div class="stat-info">
              <div class="stat-label">进行中干预方案</div>
              <div class="stat-value">{{ weightStats.activeInterventions }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info">
              <div class="stat-label">干预有效改善</div>
              <div class="stat-value">{{ weightStats.improved }}%</div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>🚨 预警列表</h3>
        </div>

        <div class="alert-list">
          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🏫</span>
              <span class="alert-group-title">高体重问题学校预警（超重/肥胖率 ≥ 35%）</span>
              <span class="alert-group-count">{{ weightWarningSchools.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="school in weightWarningSchools" :key="school.id" class="alert-item warning">
                <div class="alert-info">
                  <span class="alert-name">{{ school.name }}</span>
                  <span class="alert-detail">超重/肥胖率 {{ school.rate }}%</span>
                  <span class="alert-detail">监测人数 {{ school.total }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleWeightViewSchool(school)">查看详情</button>
                  <button class="btn btn-sm btn-success" @click="handleWeightCreateIntervention(school)">制定方案</button>
                </div>
              </div>
            </div>
          </div>

          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🆕</span>
              <span class="alert-group-title">新发体重问题学生实时提醒</span>
              <span class="alert-group-count new">{{ weightNewCaseStudents.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="student in weightNewCaseStudents" :key="student.id" class="alert-item info">
                <div class="alert-info">
                  <span class="alert-name">{{ student.name }}</span>
                  <span class="alert-detail">学号 {{ student.studentNo }}</span>
                  <span class="alert-detail">年级 {{ student.grade }}</span>
                  <span class="alert-detail">班级 {{ student.class }}</span>
                  <span class="alert-detail">问题类型 {{ student.issueType }}</span>
                  <span class="alert-detail">发现时间 {{ student.time }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleWeightViewStudent(student)">查看档案</button>
                  <button class="btn btn-sm btn-warning" @click="handleWeightTrackStudent(student)">跟踪记录</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📋 干预方案管理</h3>
          <div class="section-actions">
            <button class="btn btn-sm btn-success" @click="handleWeightAddIntervention">➕ 新增方案</button>
          </div>
        </div>

        <div class="intervention-list">
          <div v-for="intervention in weightInterventions" :key="intervention.id" class="intervention-card">
            <div class="intervention-header">
              <div class="intervention-title">
                <span class="intervention-icon">{{ intervention.icon }}</span>
                <span class="intervention-name">{{ intervention.name }}</span>
                <span class="intervention-status" :class="intervention.statusClass">{{ intervention.status }}</span>
              </div>
              <div class="intervention-school">{{ intervention.school }}</div>
            </div>
            <div class="intervention-body">
              <div class="intervention-info">
                <span>开始：{{ intervention.startDate }}</span>
                <span>预计：{{ intervention.endDate }}</span>
                <span>参与：{{ intervention.participants }} 人</span>
              </div>
              <div class="intervention-progress">
                <span class="progress-label">效果跟踪</span>
                <div class="progress-bar">
                  <div class="progress-fill" :style="{ width: intervention.progress + '%' }"></div>
                </div>
                <span class="progress-value">{{ intervention.progress }}%</span>
              </div>
            </div>
            <div class="intervention-actions">
              <button class="btn btn-sm btn-info" @click="handleWeightViewIntervention(intervention)">查看详情</button>
              <button class="btn btn-sm btn-primary" @click="handleWeightTrackIntervention(intervention)">跟踪记录</button>
              <button class="btn btn-sm btn-success" @click="handleWeightEvaluateIntervention(intervention)">效果评估</button>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📊 试点学校调控前后数据对比</h3>
        </div>

        <div class="compare-chart-wrapper">
          <div class="chart-card">
            <div ref="weightCompareChartRef" class="chart-container"></div>
          </div>
          <div class="compare-summary">
            <div class="compare-item">
              <span class="compare-label">试点学校平均超重/肥胖率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 42.5%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 35.2%</span>
              </div>
              <span class="compare-change good">下降 7.3% ✅</span>
            </div>
            <div class="compare-item">
              <span class="compare-label">非试点学校平均超重/肥胖率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 38.5%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 39.2%</span>
              </div>
              <span class="compare-change bad">上升 0.7% ⚠️</span>
            </div>
          </div>
        </div>

        <div v-if="showWeightEvaluateModal" class="modal-overlay" @click.self="showWeightEvaluateModal = false">
          <div class="modal-content">
            <div class="modal-header">
              <h3>📊 干预效果评估</h3>
              <button class="modal-close" @click="showWeightEvaluateModal = false">✕</button>
            </div>
            <div class="modal-body">
              <div class="evaluate-grid">
                <div class="evaluate-item">
                  <span class="evaluate-label">干预前超重/肥胖率</span>
                  <span class="evaluate-value">{{ weightEvaluateData.beforeRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">干预后超重/肥胖率</span>
                  <span class="evaluate-value">{{ weightEvaluateData.afterRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">改善幅度</span>
                  <span class="evaluate-value" :class="weightEvaluateData.change > 0 ? 'improve' : 'worsen'">
                    {{ weightEvaluateData.change > 0 ? '⬆' : '⬇' }} {{ Math.abs(weightEvaluateData.change) }}%
                  </span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">效果评价</span>
                  <span class="evaluate-value" :class="weightEvaluateData.ratingClass">
                    {{ weightEvaluateData.rating }}
                  </span>
                </div>
              </div>
              <div ref="weightEvaluateChartRef" class="evaluate-chart"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- ===== 骨骼健康 Tab ===== -->
      <div v-show="activeTab === 'bone'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon red">🏫</div>
            <div class="stat-info">
              <div class="stat-label">高风险骨骼健康预警学校</div>
              <div class="stat-value">{{ boneStats.warningSchools }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon orange">🆕</div>
            <div class="stat-info">
              <div class="stat-label">新发骨骼健康预警学生</div>
              <div class="stat-value">{{ boneStats.newCases }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon blue">📋</div>
            <div class="stat-info">
              <div class="stat-label">进行中干预方案</div>
              <div class="stat-value">{{ boneStats.activeInterventions }}</div>
            </div>
          </div>
          <div class="stat-card">
            <div class="stat-icon green">✅</div>
            <div class="stat-info">
              <div class="stat-label">干预有效改善</div>
              <div class="stat-value">{{ boneStats.improved }}%</div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>🚨 预警列表</h3>
        </div>

        <div class="alert-list">
          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🏫</span>
              <span class="alert-group-title">高风险骨骼健康学校预警（骨密度偏低率 ≥ 20%）</span>
              <span class="alert-group-count">{{ boneWarningSchools.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="school in boneWarningSchools" :key="school.id" class="alert-item warning">
                <div class="alert-info">
                  <span class="alert-name">{{ school.name }}</span>
                  <span class="alert-detail">骨密度偏低率 {{ school.rate }}%</span>
                  <span class="alert-detail">监测人数 {{ school.total }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleBoneViewSchool(school)">查看详情</button>
                  <button class="btn btn-sm btn-success" @click="handleBoneCreateIntervention(school)">制定方案</button>
                </div>
              </div>
            </div>
          </div>

          <div class="alert-group">
            <div class="alert-group-header">
              <span class="alert-group-icon">🆕</span>
              <span class="alert-group-title">新发骨骼健康预警学生实时提醒</span>
              <span class="alert-group-count new">{{ boneNewCaseStudents.length }}</span>
            </div>
            <div class="alert-items">
              <div v-for="student in boneNewCaseStudents" :key="student.id" class="alert-item info">
                <div class="alert-info">
                  <span class="alert-name">{{ student.name }}</span>
                  <span class="alert-detail">学号 {{ student.studentNo }}</span>
                  <span class="alert-detail">年级 {{ student.grade }}</span>
                  <span class="alert-detail">班级 {{ student.class }}</span>
                  <span class="alert-detail">预警类型 {{ student.issueType }}</span>
                  <span class="alert-detail">发现时间 {{ student.time }}</span>
                </div>
                <div class="alert-actions">
                  <button class="btn btn-sm btn-primary" @click="handleBoneViewStudent(student)">查看档案</button>
                  <button class="btn btn-sm btn-warning" @click="handleBoneTrackStudent(student)">跟踪记录</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📋 干预方案管理</h3>
          <div class="section-actions">
            <button class="btn btn-sm btn-success" @click="handleBoneAddIntervention">➕ 新增方案</button>
          </div>
        </div>

        <div class="intervention-list">
          <div v-for="intervention in boneInterventions" :key="intervention.id" class="intervention-card">
            <div class="intervention-header">
              <div class="intervention-title">
                <span class="intervention-icon">{{ intervention.icon }}</span>
                <span class="intervention-name">{{ intervention.name }}</span>
                <span class="intervention-status" :class="intervention.statusClass">{{ intervention.status }}</span>
              </div>
              <div class="intervention-school">{{ intervention.school }}</div>
            </div>
            <div class="intervention-body">
              <div class="intervention-info">
                <span>开始：{{ intervention.startDate }}</span>
                <span>预计：{{ intervention.endDate }}</span>
                <span>参与：{{ intervention.participants }} 人</span>
              </div>
              <div class="intervention-progress">
                <span class="progress-label">效果跟踪</span>
                <div class="progress-bar">
                  <div class="progress-fill" :style="{ width: intervention.progress + '%' }"></div>
                </div>
                <span class="progress-value">{{ intervention.progress }}%</span>
              </div>
            </div>
            <div class="intervention-actions">
              <button class="btn btn-sm btn-info" @click="handleBoneViewIntervention(intervention)">查看详情</button>
              <button class="btn btn-sm btn-primary" @click="handleBoneTrackIntervention(intervention)">跟踪记录</button>
              <button class="btn btn-sm btn-success" @click="handleBoneEvaluateIntervention(intervention)">效果评估</button>
            </div>
          </div>
        </div>

        <div class="section-title">
          <h3>📊 试点学校调控前后数据对比</h3>
        </div>

        <div class="compare-chart-wrapper">
          <div class="chart-card">
            <div ref="boneCompareChartRef" class="chart-container"></div>
          </div>
          <div class="compare-summary">
            <div class="compare-item">
              <span class="compare-label">试点学校平均骨密度偏低率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 24.5%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 18.2%</span>
              </div>
              <span class="compare-change good">下降 6.3% ✅</span>
            </div>
            <div class="compare-item">
              <span class="compare-label">非试点学校平均骨密度偏低率</span>
              <div class="compare-values">
                <span class="compare-before">干预前 21.5%</span>
                <span class="compare-arrow">→</span>
                <span class="compare-after">干预后 22.1%</span>
              </div>
              <span class="compare-change bad">上升 0.6% ⚠️</span>
            </div>
          </div>
        </div>

        <div v-if="showBoneEvaluateModal" class="modal-overlay" @click.self="showBoneEvaluateModal = false">
          <div class="modal-content">
            <div class="modal-header">
              <h3>📊 干预效果评估</h3>
              <button class="modal-close" @click="showBoneEvaluateModal = false">✕</button>
            </div>
            <div class="modal-body">
              <div class="evaluate-grid">
                <div class="evaluate-item">
                  <span class="evaluate-label">干预前骨密度偏低率</span>
                  <span class="evaluate-value">{{ boneEvaluateData.beforeRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">干预后骨密度偏低率</span>
                  <span class="evaluate-value">{{ boneEvaluateData.afterRate }}%</span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">改善幅度</span>
                  <span class="evaluate-value" :class="boneEvaluateData.change > 0 ? 'improve' : 'worsen'">
                    {{ boneEvaluateData.change > 0 ? '⬆' : '⬇' }} {{ Math.abs(boneEvaluateData.change) }}%
                  </span>
                </div>
                <div class="evaluate-item">
                  <span class="evaluate-label">效果评价</span>
                  <span class="evaluate-value" :class="boneEvaluateData.ratingClass">
                    {{ boneEvaluateData.rating }}
                  </span>
                </div>
              </div>
              <div ref="boneEvaluateChartRef" class="evaluate-chart"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, onBeforeUnmount, nextTick } from 'vue';
import { message, Modal } from 'ant-design-vue';
import * as echarts from 'echarts';

// ================= Theme Detection =================
const isLight = ref(false);
let themeObserver: MutationObserver | null = null;

const applySystemTheme = () => {
  isLight.value = !document.documentElement.classList.contains('dark');
};

const observeTheme = () => {
  themeObserver = new MutationObserver(() => {
    isLight.value = !document.documentElement.classList.contains('dark');
  });
  themeObserver.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
};

// ================= ECharts Theme =================
const getEchartsTheme = () => {
  if (isLight.value) {
    return {
      text: '#334155', textDim: '#64748b',
      axisLine: 'rgba(8, 145, 178, 0.3)', splitLine: 'rgba(8, 145, 178, 0.1)',
      tooltipBg: 'rgba(255,255,255,0.95)', tooltipBorder: 'rgba(8, 145, 178, 0.2)',
      primary: '#0891b2', secondary: '#7c3aed',
      success: '#16a34a', warning: '#d97706', danger: '#dc2626'
    };
  }
  return {
    text: '#e2e8f0', textDim: '#94a3b8',
    axisLine: 'rgba(56, 189, 248, 0.25)', splitLine: 'rgba(56, 189, 248, 0.08)',
    tooltipBg: 'rgba(10, 22, 50, 0.95)', tooltipBorder: 'rgba(56, 189, 248, 0.3)',
    primary: '#38bdf8', secondary: '#a78bfa',
    success: '#34d399', warning: '#fbbf24', danger: '#fb7185'
  };
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

// ================= Tab切换 =================
const switchTab = (tabKey: string) => {
  activeTab.value = tabKey;
  nextTick(() => {
    if (tabKey === 'vision') {
      initCompareChart();
    } else if (tabKey === 'oral') {
      initOralCompareChart();
    } else if (tabKey === 'mental') {
      initMentalCompareChart();
    } else if (tabKey === 'weight') {
      initWeightCompareChart();
    } else if (tabKey === 'bone') {
      initBoneCompareChart();
    }
  });
};

// ================= 视力统计数据 =================
const visionStats = reactive({
  warningSchools: 8,
  newCases: 23,
  activeInterventions: 15,
  improved: 72.5
});

// ================= 口腔统计数据 =================
const oralStats = reactive({
  warningSchools: 4,
  newCases: 12,
  activeInterventions: 8,
  improved: 68.0
});

// ================= 心理统计数据 =================
const mentalStats = reactive({
  warningSchools: 6,
  newCases: 18,
  activeInterventions: 10,
  improved: 65.5
});

// ================= 健康体重统计数据 =================
const weightStats = reactive({
  warningSchools: 5,
  newCases: 15,
  activeInterventions: 9,
  improved: 62.0
});

// ================= 骨骼健康统计数据 =================
const boneStats = reactive({
  warningSchools: 3,
  newCases: 8,
  activeInterventions: 6,
  improved: 58.5
});

// ================= 视力预警学校数据 =================
const warningSchools = ref([
  { id: 1, name: '第一中学', rate: 65.2, total: 1200 },
  { id: 2, name: '育才小学', rate: 62.8, total: 980 },
  { id: 3, name: '实验中学', rate: 61.5, total: 1100 },
  { id: 4, name: '红星小学', rate: 60.3, total: 850 },
]);

// ================= 视力新发近视学生数据 =================
const newCaseStudents = ref([
  { id: 1, name: '张小明', studentNo: '2024001', grade: '三年级', class: '二班', time: '2026-06-28 14:30' },
  { id: 2, name: '李小红', studentNo: '2024005', grade: '五年级', class: '三班', time: '2026-06-28 15:20' },
  { id: 3, name: '王小刚', studentNo: '2024003', grade: '四年级', class: '一班', time: '2026-06-27 16:00' },
]);

// ================= 视力干预方案数据 =================
const interventions = ref([
  {
    id: 1,
    icon: '👁️',
    name: '视力健康干预计划',
    school: '第一中学',
    status: '进行中',
    statusClass: 'status-active',
    startDate: '2026-03-01',
    endDate: '2026-08-31',
    participants: 156,
    progress: 65
  },
  {
    id: 2,
    icon: '👁️',
    name: '近视防控试点方案',
    school: '育才小学',
    status: '已完成',
    statusClass: 'status-completed',
    startDate: '2025-09-01',
    endDate: '2026-06-30',
    participants: 89,
    progress: 100
  },
  {
    id: 3,
    icon: '👁️',
    name: '视力提升专项计划',
    school: '实验中学',
    status: '待启动',
    statusClass: 'status-pending',
    startDate: '2026-07-01',
    endDate: '2026-12-31',
    participants: 120,
    progress: 0
  },
]);

// ================= 口腔预警学校数据 =================
const oralWarningSchools = ref([
  { id: 1, name: '第一中学', rate: 55.2, total: 1200 },
  { id: 2, name: '育才小学', rate: 52.8, total: 980 },
  { id: 3, name: '红星中学', rate: 50.5, total: 1100 },
]);

// ================= 口腔新发龋齿学生数据 =================
const oralNewCaseStudents = ref([
  { id: 1, name: '张小明', studentNo: '2024001', grade: '三年级', time: '2026-06-28 14:30' },
  { id: 2, name: '李小红', studentNo: '2024005', grade: '五年级', time: '2026-06-28 15:20' },
]);

// ================= 口腔干预方案数据 =================
const oralInterventions = ref([
  {
    id: 1,
    icon: '🦷',
    name: '口腔健康干预计划',
    school: '第一中学',
    status: '进行中',
    statusClass: 'status-active',
    startDate: '2026-03-01',
    endDate: '2026-08-31',
    participants: 156,
    progress: 65
  },
  {
    id: 2,
    icon: '🦷',
    name: '龋齿防控试点方案',
    school: '育才小学',
    status: '已完成',
    statusClass: 'status-completed',
    startDate: '2025-09-01',
    endDate: '2026-06-30',
    participants: 89,
    progress: 100
  },
]);

// ================= 心理预警学校数据 =================
const mentalWarningSchools = ref([
  { id: 1, name: '第一中学', rate: 28.5, total: 1200 },
  { id: 2, name: '育才小学', rate: 26.2, total: 980 },
  { id: 3, name: '实验中学', rate: 24.8, total: 1100 },
  { id: 4, name: '红星小学', rate: 22.5, total: 850 },
]);

// ================= 心理新发预警学生数据 =================
const mentalNewCaseStudents = ref([
  { id: 1, name: '张小明', studentNo: '2024001', grade: '三年级', class: '二班', time: '2026-06-28 14:30' },
  { id: 2, name: '李小红', studentNo: '2024005', grade: '五年级', class: '三班', time: '2026-06-28 15:20' },
  { id: 3, name: '王小刚', studentNo: '2024003', grade: '四年级', class: '一班', time: '2026-06-27 16:00' },
]);

// ================= 心理干预方案数据 =================
const mentalInterventions = ref([
  {
    id: 1,
    icon: '🧠',
    name: '心理健康干预计划',
    school: '第一中学',
    status: '进行中',
    statusClass: 'status-active',
    startDate: '2026-03-01',
    endDate: '2026-08-31',
    participants: 120,
    progress: 60
  },
  {
    id: 2,
    icon: '🧠',
    name: '心理疏导试点方案',
    school: '育才小学',
    status: '已完成',
    statusClass: 'status-completed',
    startDate: '2025-09-01',
    endDate: '2026-06-30',
    participants: 78,
    progress: 100
  },
  {
    id: 3,
    icon: '🧠',
    name: '心理健康提升计划',
    school: '实验中学',
    status: '待启动',
    statusClass: 'status-pending',
    startDate: '2026-07-01',
    endDate: '2026-12-31',
    participants: 95,
    progress: 0
  },
]);

// ================= 健康体重预警学校数据 =================
const weightWarningSchools = ref([
  { id: 1, name: '第一中学', rate: 42.5, total: 1200 },
  { id: 2, name: '育才小学', rate: 40.2, total: 980 },
  { id: 3, name: '实验中学', rate: 38.5, total: 1100 },
]);

// ================= 健康体重新发预警学生数据 =================
const weightNewCaseStudents = ref([
  { id: 1, name: '张小明', studentNo: '2024001', grade: '三年级', class: '二班', issueType: '超重', time: '2026-06-28 14:30' },
  { id: 2, name: '李小红', studentNo: '2024005', grade: '五年级', class: '三班', issueType: '肥胖', time: '2026-06-28 15:20' },
  { id: 3, name: '王小刚', studentNo: '2024003', grade: '四年级', class: '一班', issueType: '超重', time: '2026-06-27 16:00' },
]);

// ================= 健康体重干预方案数据 =================
const weightInterventions = ref([
  {
    id: 1,
    icon: '⚖️',
    name: '体重管理干预计划',
    school: '第一中学',
    status: '进行中',
    statusClass: 'status-active',
    startDate: '2026-03-01',
    endDate: '2026-08-31',
    participants: 120,
    progress: 55
  },
  {
    id: 2,
    icon: '⚖️',
    name: '健康体重试点方案',
    school: '育才小学',
    status: '已完成',
    statusClass: 'status-completed',
    startDate: '2025-09-01',
    endDate: '2026-06-30',
    participants: 67,
    progress: 100
  },
]);

// ================= 骨骼健康预警学校数据 =================
const boneWarningSchools = ref([
  { id: 1, name: '第一中学', rate: 24.5, total: 1200 },
  { id: 2, name: '育才小学', rate: 22.8, total: 980 },
]);

// ================= 骨骼健康新发预警学生数据 =================
const boneNewCaseStudents = ref([
  { id: 1, name: '张小明', studentNo: '2024001', grade: '三年级', class: '二班', issueType: '骨密度偏低', time: '2026-06-28 14:30' },
  { id: 2, name: '李小红', studentNo: '2024005', grade: '五年级', class: '三班', issueType: '钙摄入不足', time: '2026-06-28 15:20' },
]);

// ================= 骨骼健康干预方案数据 =================
const boneInterventions = ref([
  {
    id: 1,
    icon: '🦴',
    name: '骨骼健康干预计划',
    school: '第一中学',
    status: '进行中',
    statusClass: 'status-active',
    startDate: '2026-03-01',
    endDate: '2026-08-31',
    participants: 85,
    progress: 50
  },
  {
    id: 2,
    icon: '🦴',
    name: '骨密度提升试点方案',
    school: '育才小学',
    status: '已完成',
    statusClass: 'status-completed',
    startDate: '2025-09-01',
    endDate: '2026-06-30',
    participants: 52,
    progress: 100
  },
]);

// ================= 视力效果评估数据 =================
const evaluateData = reactive({
  beforeRate: 56.8,
  afterRate: 48.2,
  change: 8.6,
  rating: '显著改善',
  ratingClass: 'rating-excellent'
});

// ================= 口腔效果评估数据 =================
const oralEvaluateData = reactive({
  beforeRate: 52.8,
  afterRate: 44.2,
  change: 8.6,
  rating: '显著改善',
  ratingClass: 'rating-excellent'
});

// ================= 心理效果评估数据 =================
const mentalEvaluateData = reactive({
  beforeRate: 28.5,
  afterRate: 20.2,
  change: 8.3,
  rating: '显著改善',
  ratingClass: 'rating-excellent'
});

// ================= 健康体重效果评估数据 =================
const weightEvaluateData = reactive({
  beforeRate: 42.5,
  afterRate: 35.2,
  change: 7.3,
  rating: '显著改善',
  ratingClass: 'rating-excellent'
});

// ================= 骨骼健康效果评估数据 =================
const boneEvaluateData = reactive({
  beforeRate: 24.5,
  afterRate: 18.2,
  change: 6.3,
  rating: '显著改善',
  ratingClass: 'rating-excellent'
});

// ================= 弹窗控制 =================
const showEvaluateModal = ref(false);
const showOralEvaluateModal = ref(false);
const showMentalEvaluateModal = ref(false);
const showWeightEvaluateModal = ref(false);
const showBoneEvaluateModal = ref(false);

// ================= 图表引用 =================
const compareChartRef = ref<HTMLElement | null>(null);
const evaluateChartRef = ref<HTMLElement | null>(null);
const oralCompareChartRef = ref<HTMLElement | null>(null);
const oralEvaluateChartRef = ref<HTMLElement | null>(null);
const mentalCompareChartRef = ref<HTMLElement | null>(null);
const mentalEvaluateChartRef = ref<HTMLElement | null>(null);
const weightCompareChartRef = ref<HTMLElement | null>(null);
const weightEvaluateChartRef = ref<HTMLElement | null>(null);
const boneCompareChartRef = ref<HTMLElement | null>(null);
const boneEvaluateChartRef = ref<HTMLElement | null>(null);

// ================= 图表实例存储 =================
let chartInstances: echarts.ECharts[] = [];

const initChartInstance = (ref: typeof compareChartRef, option: echarts.EChartsOption) => {
  if (!ref.value) return;
  const existing = chartInstances.find(c => c.getDom() === ref.value);
  if (existing) {
    existing.dispose();
    chartInstances = chartInstances.filter(c => c !== existing);
  }
  const chart = echarts.init(ref.value);
  chart.setOption(option);
  chartInstances.push(chart);
  return chart;
};

const disposeAllCharts = () => {
  chartInstances.forEach(c => c.dispose());
  chartInstances = [];
};

// ================= 视力对比图 =================
const initCompareChart = () => {
  if (!compareChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: {
      trigger: 'axis',
      backgroundColor: t.tooltipBg,
      borderColor: t.tooltipBorder,
      textStyle: { color: t.text }
    },
    legend: {
      data: ['试点学校', '非试点学校'],
      textStyle: { color: t.textDim },
      top: 0,
      right: 10
    },
    grid: { left: 50, right: 20, top: 40, bottom: 30 },
    xAxis: {
      type: 'category',
      data: ['干预前', '干预后'],
      axisLine: { lineStyle: { color: t.axisLine } },
      axisLabel: { color: t.textDim }
    },
    yAxis: {
      type: 'value',
      name: '近视率(%)',
      nameTextStyle: { color: t.textDim },
      axisLine: { lineStyle: { color: t.axisLine } },
      splitLine: { lineStyle: { color: t.splitLine } },
      axisLabel: { color: t.textDim }
    },
    series: [
      {
        name: '试点学校',
        type: 'bar',
        data: [56.8, 48.2],
        itemStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: t.primary },
            { offset: 1, color: t.secondary }
          ]),
          borderRadius: [4, 4, 0, 0]
        },
        barWidth: 30
      },
      {
        name: '非试点学校',
        type: 'bar',
        data: [53.2, 54.1],
        itemStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: t.warning },
            { offset: 1, color: t.danger }
          ]),
          borderRadius: [4, 4, 0, 0]
        },
        barWidth: 30
      }
    ]
  };
  initChartInstance(compareChartRef, option);
};

// ================= 视力评估图 =================
const initEvaluateChart = () => {
  if (!evaluateChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: {
      trigger: 'axis',
      backgroundColor: t.tooltipBg,
      borderColor: t.tooltipBorder,
      textStyle: { color: t.text }
    },
    grid: { left: 50, right: 20, top: 20, bottom: 30 },
    xAxis: {
      type: 'category',
      data: ['干预前', '干预后'],
      axisLine: { lineStyle: { color: t.axisLine } },
      axisLabel: { color: t.textDim }
    },
    yAxis: {
      type: 'value',
      name: '近视率(%)',
      nameTextStyle: { color: t.textDim },
      axisLine: { lineStyle: { color: t.axisLine } },
      splitLine: { lineStyle: { color: t.splitLine } },
      axisLabel: { color: t.textDim }
    },
    series: [{
      type: 'line',
      data: [evaluateData.beforeRate, evaluateData.afterRate],
      smooth: true,
      lineStyle: { color: t.primary, width: 3 },
      itemStyle: { color: t.primary },
      areaStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
          { offset: 0, color: 'rgba(56, 189, 248, 0.35)' },
          { offset: 1, color: 'rgba(56, 189, 248, 0.02)' }
        ])
      }
    }]
  };
  initChartInstance(evaluateChartRef, option);
};

// ================= 口腔对比图 =================
const initOralCompareChart = () => {
  if (!oralCompareChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: {
      trigger: 'axis',
      backgroundColor: t.tooltipBg,
      borderColor: t.tooltipBorder,
      textStyle: { color: t.text }
    },
    legend: {
      data: ['试点学校', '非试点学校'],
      textStyle: { color: t.textDim },
      top: 0, right: 10
    },
    grid: { left: 50, right: 20, top: 40, bottom: 30 },
    xAxis: {
      type: 'category', data: ['干预前', '干预后'],
      axisLine: { lineStyle: { color: t.axisLine } },
      axisLabel: { color: t.textDim }
    },
    yAxis: {
      type: 'value', name: '龋患率(%)',
      nameTextStyle: { color: t.textDim },
      axisLine: { lineStyle: { color: t.axisLine } },
      splitLine: { lineStyle: { color: t.splitLine } },
      axisLabel: { color: t.textDim }
    },
    series: [
      {
        name: '试点学校', type: 'bar', data: [52.8, 44.2],
        itemStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: t.primary }, { offset: 1, color: t.secondary }
          ]),
          borderRadius: [4, 4, 0, 0]
        },
        barWidth: 30
      },
      {
        name: '非试点学校', type: 'bar', data: [48.5, 49.2],
        itemStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            { offset: 0, color: t.warning }, { offset: 1, color: t.danger }
          ]),
          borderRadius: [4, 4, 0, 0]
        },
        barWidth: 30
      }
    ]
  };
  initChartInstance(oralCompareChartRef, option);
};

// ================= 口腔评估图 =================
const initOralEvaluateChart = () => {
  if (!oralEvaluateChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: { trigger: 'axis', backgroundColor: t.tooltipBg, borderColor: t.tooltipBorder, textStyle: { color: t.text } },
    grid: { left: 50, right: 20, top: 20, bottom: 30 },
    xAxis: { type: 'category', data: ['干预前', '干预后'], axisLine: { lineStyle: { color: t.axisLine } }, axisLabel: { color: t.textDim } },
    yAxis: { type: 'value', name: '龋患率(%)', nameTextStyle: { color: t.textDim }, axisLine: { lineStyle: { color: t.axisLine } }, splitLine: { lineStyle: { color: t.splitLine } }, axisLabel: { color: t.textDim } },
    series: [{
      type: 'line', data: [oralEvaluateData.beforeRate, oralEvaluateData.afterRate],
      smooth: true, lineStyle: { color: t.primary, width: 3 }, itemStyle: { color: t.primary },
      areaStyle: { color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
        { offset: 0, color: 'rgba(56, 189, 248, 0.35)' }, { offset: 1, color: 'rgba(56, 189, 248, 0.02)' }
      ]) }
    }]
  };
  initChartInstance(oralEvaluateChartRef, option);
};

// ================= 心理对比图 =================
const initMentalCompareChart = () => {
  if (!mentalCompareChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: { trigger: 'axis', backgroundColor: t.tooltipBg, borderColor: t.tooltipBorder, textStyle: { color: t.text } },
    legend: { data: ['试点学校', '非试点学校'], textStyle: { color: t.textDim }, top: 0, right: 10 },
    grid: { left: 50, right: 20, top: 40, bottom: 30 },
    xAxis: { type: 'category', data: ['干预前', '干预后'], axisLine: { lineStyle: { color: t.axisLine } }, axisLabel: { color: t.textDim } },
    yAxis: { type: 'value', name: '预警率(%)', nameTextStyle: { color: t.textDim }, axisLine: { lineStyle: { color: t.axisLine } }, splitLine: { lineStyle: { color: t.splitLine } }, axisLabel: { color: t.textDim } },
    series: [
      { name: '试点学校', type: 'bar', data: [28.5, 20.2], itemStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{ offset: 0, color: t.primary }, { offset: 1, color: t.secondary }]),
        borderRadius: [4, 4, 0, 0]
      }, barWidth: 30 },
      { name: '非试点学校', type: 'bar', data: [24.5, 25.2], itemStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{ offset: 0, color: t.warning }, { offset: 1, color: t.danger }]),
        borderRadius: [4, 4, 0, 0]
      }, barWidth: 30 }
    ]
  };
  initChartInstance(mentalCompareChartRef, option);
};

// ================= 心理评估图 =================
const initMentalEvaluateChart = () => {
  if (!mentalEvaluateChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: { trigger: 'axis', backgroundColor: t.tooltipBg, borderColor: t.tooltipBorder, textStyle: { color: t.text } },
    grid: { left: 50, right: 20, top: 20, bottom: 30 },
    xAxis: { type: 'category', data: ['干预前', '干预后'], axisLine: { lineStyle: { color: t.axisLine } }, axisLabel: { color: t.textDim } },
    yAxis: { type: 'value', name: '预警率(%)', nameTextStyle: { color: t.textDim }, axisLine: { lineStyle: { color: t.axisLine } }, splitLine: { lineStyle: { color: t.splitLine } }, axisLabel: { color: t.textDim } },
    series: [{
      type: 'line', data: [mentalEvaluateData.beforeRate, mentalEvaluateData.afterRate],
      smooth: true, lineStyle: { color: t.primary, width: 3 }, itemStyle: { color: t.primary },
      areaStyle: { color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
        { offset: 0, color: 'rgba(56, 189, 248, 0.35)' }, { offset: 1, color: 'rgba(56, 189, 248, 0.02)' }
      ]) }
    }]
  };
  initChartInstance(mentalEvaluateChartRef, option);
};

// ================= 体重对比图 =================
const initWeightCompareChart = () => {
  if (!weightCompareChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: { trigger: 'axis', backgroundColor: t.tooltipBg, borderColor: t.tooltipBorder, textStyle: { color: t.text } },
    legend: { data: ['试点学校', '非试点学校'], textStyle: { color: t.textDim }, top: 0, right: 10 },
    grid: { left: 50, right: 20, top: 40, bottom: 30 },
    xAxis: { type: 'category', data: ['干预前', '干预后'], axisLine: { lineStyle: { color: t.axisLine } }, axisLabel: { color: t.textDim } },
    yAxis: { type: 'value', name: '超重/肥胖率(%)', nameTextStyle: { color: t.textDim }, axisLine: { lineStyle: { color: t.axisLine } }, splitLine: { lineStyle: { color: t.splitLine } }, axisLabel: { color: t.textDim } },
    series: [
      { name: '试点学校', type: 'bar', data: [42.5, 35.2], itemStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{ offset: 0, color: t.primary }, { offset: 1, color: t.secondary }]),
        borderRadius: [4, 4, 0, 0]
      }, barWidth: 30 },
      { name: '非试点学校', type: 'bar', data: [38.5, 39.2], itemStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{ offset: 0, color: t.warning }, { offset: 1, color: t.danger }]),
        borderRadius: [4, 4, 0, 0]
      }, barWidth: 30 }
    ]
  };
  initChartInstance(weightCompareChartRef, option);
};

// ================= 体重评估图 =================
const initWeightEvaluateChart = () => {
  if (!weightEvaluateChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: { trigger: 'axis', backgroundColor: t.tooltipBg, borderColor: t.tooltipBorder, textStyle: { color: t.text } },
    grid: { left: 50, right: 20, top: 20, bottom: 30 },
    xAxis: { type: 'category', data: ['干预前', '干预后'], axisLine: { lineStyle: { color: t.axisLine } }, axisLabel: { color: t.textDim } },
    yAxis: { type: 'value', name: '超重/肥胖率(%)', nameTextStyle: { color: t.textDim }, axisLine: { lineStyle: { color: t.axisLine } }, splitLine: { lineStyle: { color: t.splitLine } }, axisLabel: { color: t.textDim } },
    series: [{
      type: 'line', data: [weightEvaluateData.beforeRate, weightEvaluateData.afterRate],
      smooth: true, lineStyle: { color: t.primary, width: 3 }, itemStyle: { color: t.primary },
      areaStyle: { color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
        { offset: 0, color: 'rgba(56, 189, 248, 0.35)' }, { offset: 1, color: 'rgba(56, 189, 248, 0.02)' }
      ]) }
    }]
  };
  initChartInstance(weightEvaluateChartRef, option);
};

// ================= 骨骼对比图 =================
const initBoneCompareChart = () => {
  if (!boneCompareChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: { trigger: 'axis', backgroundColor: t.tooltipBg, borderColor: t.tooltipBorder, textStyle: { color: t.text } },
    legend: { data: ['试点学校', '非试点学校'], textStyle: { color: t.textDim }, top: 0, right: 10 },
    grid: { left: 50, right: 20, top: 40, bottom: 30 },
    xAxis: { type: 'category', data: ['干预前', '干预后'], axisLine: { lineStyle: { color: t.axisLine } }, axisLabel: { color: t.textDim } },
    yAxis: { type: 'value', name: '骨密度偏低率(%)', nameTextStyle: { color: t.textDim }, axisLine: { lineStyle: { color: t.axisLine } }, splitLine: { lineStyle: { color: t.splitLine } }, axisLabel: { color: t.textDim } },
    series: [
      { name: '试点学校', type: 'bar', data: [24.5, 18.2], itemStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{ offset: 0, color: t.primary }, { offset: 1, color: t.secondary }]),
        borderRadius: [4, 4, 0, 0]
      }, barWidth: 30 },
      { name: '非试点学校', type: 'bar', data: [21.5, 22.1], itemStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [{ offset: 0, color: t.warning }, { offset: 1, color: t.danger }]),
        borderRadius: [4, 4, 0, 0]
      }, barWidth: 30 }
    ]
  };
  initChartInstance(boneCompareChartRef, option);
};

// ================= 骨骼评估图 =================
const initBoneEvaluateChart = () => {
  if (!boneEvaluateChartRef.value) return;
  const t = getEchartsTheme();
  const option: echarts.EChartsOption = {
    tooltip: { trigger: 'axis', backgroundColor: t.tooltipBg, borderColor: t.tooltipBorder, textStyle: { color: t.text } },
    grid: { left: 50, right: 20, top: 20, bottom: 30 },
    xAxis: { type: 'category', data: ['干预前', '干预后'], axisLine: { lineStyle: { color: t.axisLine } }, axisLabel: { color: t.textDim } },
    yAxis: { type: 'value', name: '骨密度偏低率(%)', nameTextStyle: { color: t.textDim }, axisLine: { lineStyle: { color: t.axisLine } }, splitLine: { lineStyle: { color: t.splitLine } }, axisLabel: { color: t.textDim } },
    series: [{
      type: 'line', data: [boneEvaluateData.beforeRate, boneEvaluateData.afterRate],
      smooth: true, lineStyle: { color: t.primary, width: 3 }, itemStyle: { color: t.primary },
      areaStyle: { color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
        { offset: 0, color: 'rgba(56, 189, 248, 0.35)' }, { offset: 1, color: 'rgba(56, 189, 248, 0.02)' }
      ]) }
    }]
  };
  initChartInstance(boneEvaluateChartRef, option);
};

// ================= 处理函数 =================
const handleRefresh = () => {
  message.success('数据已刷新');
};

const handleViewSchool = (school: any) => {
  message.info(`查看学校详情: ${school.name}`);
};

const handleCreateIntervention = (school: any) => {
  message.info(`为 ${school.name} 制定干预方案`);
};

const handleViewStudent = (student: any) => {
  message.info(`查看学生档案: ${student.name}`);
};

const handleTrackStudent = (student: any) => {
  message.info(`跟踪记录: ${student.name}`);
};

const handleAddIntervention = () => {
  message.info('新增干预方案');
};

const handleViewIntervention = (intervention: any) => {
  message.info(`查看干预方案详情: ${intervention.name}`);
};

const handleTrackIntervention = (intervention: any) => {
  message.info(`跟踪记录: ${intervention.name}`);
};

const handleEvaluateIntervention = (intervention: any) => {
  showEvaluateModal.value = true;
  nextTick(() => { initEvaluateChart(); });
};

const handleOralViewSchool = (school: any) => { message.info(`查看学校详情: ${school.name}`); };
const handleOralCreateIntervention = (school: any) => { message.info(`为 ${school.name} 制定干预方案`); };
const handleOralViewStudent = (student: any) => { message.info(`查看学生档案: ${student.name}`); };
const handleOralTrackStudent = (student: any) => { message.info(`跟踪记录: ${student.name}`); };
const handleOralAddIntervention = () => { message.info('新增干预方案'); };
const handleOralViewIntervention = (intervention: any) => { message.info(`查看干预方案详情: ${intervention.name}`); };
const handleOralTrackIntervention = (intervention: any) => { message.info(`跟踪记录: ${intervention.name}`); };
const handleOralEvaluateIntervention = (_intervention: any) => {
  showOralEvaluateModal.value = true;
  nextTick(() => { initOralEvaluateChart(); });
};

const handleMentalViewSchool = (school: any) => { message.info(`查看学校详情: ${school.name}`); };
const handleMentalCreateIntervention = (school: any) => { message.info(`为 ${school.name} 制定干预方案`); };
const handleMentalViewStudent = (student: any) => { message.info(`查看学生档案: ${student.name}`); };
const handleMentalTrackStudent = (student: any) => { message.info(`跟踪记录: ${student.name}`); };
const handleMentalAddIntervention = () => { message.info('新增干预方案'); };
const handleMentalViewIntervention = (intervention: any) => { message.info(`查看干预方案详情: ${intervention.name}`); };
const handleMentalTrackIntervention = (intervention: any) => { message.info(`跟踪记录: ${intervention.name}`); };
const handleMentalEvaluateIntervention = (_intervention: any) => {
  showMentalEvaluateModal.value = true;
  nextTick(() => { initMentalEvaluateChart(); });
};

const handleWeightViewSchool = (school: any) => { message.info(`查看学校详情: ${school.name}`); };
const handleWeightCreateIntervention = (school: any) => { message.info(`为 ${school.name} 制定干预方案`); };
const handleWeightViewStudent = (student: any) => { message.info(`查看学生档案: ${student.name}`); };
const handleWeightTrackStudent = (student: any) => { message.info(`跟踪记录: ${student.name}`); };
const handleWeightAddIntervention = () => { message.info('新增干预方案'); };
const handleWeightViewIntervention = (intervention: any) => { message.info(`查看干预方案详情: ${intervention.name}`); };
const handleWeightTrackIntervention = (intervention: any) => { message.info(`跟踪记录: ${intervention.name}`); };
const handleWeightEvaluateIntervention = (_intervention: any) => {
  showWeightEvaluateModal.value = true;
  nextTick(() => { initWeightEvaluateChart(); });
};

const handleBoneViewSchool = (school: any) => { message.info(`查看学校详情: ${school.name}`); };
const handleBoneCreateIntervention = (school: any) => { message.info(`为 ${school.name} 制定干预方案`); };
const handleBoneViewStudent = (student: any) => { message.info(`查看学生档案: ${student.name}`); };
const handleBoneTrackStudent = (student: any) => { message.info(`跟踪记录: ${student.name}`); };
const handleBoneAddIntervention = () => { message.info('新增干预方案'); };
const handleBoneViewIntervention = (intervention: any) => { message.info(`查看干预方案详情: ${intervention.name}`); };
const handleBoneTrackIntervention = (intervention: any) => { message.info(`跟踪记录: ${intervention.name}`); };
const handleBoneEvaluateIntervention = (_intervention: any) => {
  showBoneEvaluateModal.value = true;
  nextTick(() => { initBoneEvaluateChart(); });
};

// ================= 生命周期 =================
onMounted(() => {
  applySystemTheme();
  observeTheme();
  nextTick(() => {
    initCompareChart();
  });
  window.addEventListener('resize', () => {
    chartInstances.forEach(c => c.resize());
  });
});

onBeforeUnmount(() => {
  if (themeObserver) {
    themeObserver.disconnect();
    themeObserver = null;
  }
  disposeAllCharts();
});
</script>

<style scoped>
/* ===== CSS Variables ===== */
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

/* ===== Page Header ===== */
.page-header {
  max-width: 1600px;
  margin: 0 auto 12px;
  padding: 10px 16px;
  background: linear-gradient(90deg, var(--primary-bg) 0%, transparent 70%);
  border: 1px solid var(--border-soft);
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 14px;
  overflow: hidden;
  position: relative;
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

/* ===== Tab Bar ===== */
.tab-bar {
  max-width: 1600px;
  margin: 0 auto;
  display: flex;
  gap: 4px;
  padding: 4px;
  background: var(--bg-card);
  border: 1px solid var(--border-soft);
  border-radius: 10px;
  box-shadow: var(--shadow);
  margin-bottom: 16px;
  position: relative;
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
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  padding: 12px 16px;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  color: var(--text-dim);
  font-size: 14px;
  font-weight: 500;
  border: 1px solid transparent;
  position: relative;
  background: transparent;
}

.tab-item:hover {
  background: var(--bg-hover);
  color: var(--text);
  border-color: var(--border-soft);
}

.tab-item.active {
  background: var(--primary-bg);
  color: var(--primary);
  border-color: var(--border);
  box-shadow: 0 0 16px var(--primary-bg), inset 0 0 20px rgba(56, 189, 248, 0.05);
}

.tab-item.active::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 20%;
  right: 20%;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), transparent);
  border-radius: 2px;
}

.tab-icon {
  font-size: 18px;
}

.tab-label {
  letter-spacing: 0.5px;
}

.tab-badge {
  background: var(--danger);
  color: #fff;
  font-size: 11px;
  font-weight: 600;
  padding: 2px 8px;
  border-radius: 10px;
  min-width: 22px;
  text-align: center;
  box-shadow: 0 0 8px rgba(239, 68, 68, 0.4);
}

.tab-item.active .tab-badge {
  background: var(--primary);
  box-shadow: 0 0 8px var(--primary-bg);
}

/* ===== Tab Content ===== */
.tab-content {
  max-width: 1600px;
  margin: 0 auto;
}

.tab-panel {
  animation: fadeInTab 0.3s ease;
}

@keyframes fadeInTab {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

/* ===== Stats Grid ===== */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 20px;
}

.stat-card {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 16px 20px;
  display: flex;
  align-items: center;
  gap: 14px;
  box-shadow: var(--shadow);
  backdrop-filter: blur(8px);
  position: relative;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.stat-card::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.7;
  transition: opacity 0.4s ease;
}

.stat-card:hover {
  box-shadow: var(--shadow-card);
  border-color: var(--border-strong);
  transform: translateY(-2px);
}

.stat-card:hover::before {
  opacity: 1;
}

.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 22px;
  flex-shrink: 0;
  position: relative;
}

.stat-icon.red {
  background: rgba(239, 68, 68, 0.15);
  border: 1px solid rgba(239, 68, 68, 0.3);
  box-shadow: 0 0 12px rgba(239, 68, 68, 0.15);
}

.stat-icon.orange {
  background: rgba(245, 158, 11, 0.15);
  border: 1px solid rgba(245, 158, 11, 0.3);
  box-shadow: 0 0 12px rgba(245, 158, 11, 0.15);
}

.stat-icon.blue {
  background: rgba(56, 189, 248, 0.15);
  border: 1px solid rgba(56, 189, 248, 0.3);
  box-shadow: 0 0 12px rgba(56, 189, 248, 0.15);
}

.stat-icon.green {
  background: rgba(34, 197, 94, 0.15);
  border: 1px solid rgba(34, 197, 94, 0.3);
  box-shadow: 0 0 12px rgba(34, 197, 94, 0.15);
}

.stat-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.stat-label {
  font-size: 12px;
  color: var(--text-dim);
  letter-spacing: 0.5px;
}

.stat-value {
  font-size: 26px;
  font-weight: 700;
  color: var(--text-strong);
  font-family: 'Orbitron', 'Consolas', monospace;
  letter-spacing: 1px;
  text-shadow: 0 0 12px var(--glow);
}

/* ===== Section Title ===== */
.section-title {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 14px;
  padding-bottom: 10px;
  border-bottom: 1px solid var(--border-soft);
  position: relative;
}

.section-title::after {
  content: '';
  position: absolute;
  bottom: -1px;
  left: 0;
  width: 100px;
  height: 2px;
  background: linear-gradient(90deg, var(--primary), transparent);
  border-radius: 2px;
}

.section-title h3 {
  font-size: 16px;
  font-weight: 600;
  color: var(--text-strong);
  margin: 0;
  letter-spacing: 0.5px;
}

.section-actions {
  display: flex;
  gap: 10px;
}

/* ===== Buttons ===== */
.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
  padding: 7px 16px;
  border-radius: 6px;
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid transparent;
  white-space: nowrap;
}

.btn-sm {
  padding: 5px 12px;
  font-size: 12px;
  border-radius: 5px;
}

.btn-primary {
  background: var(--primary);
  color: #fff;
  border-color: var(--primary);
}

.btn-primary:hover {
  background: var(--primary-hover);
  border-color: var(--primary-hover);
  box-shadow: 0 0 16px var(--primary-bg);
  transform: translateY(-1px);
}

.btn-success {
  background: var(--success);
  color: #fff;
  border-color: var(--success);
}

.btn-success:hover {
  opacity: 0.9;
  box-shadow: 0 0 12px rgba(34, 197, 94, 0.3);
  transform: translateY(-1px);
}

.btn-warning {
  background: var(--warning);
  color: #0f172a;
  border-color: var(--warning);
}

.btn-warning:hover {
  opacity: 0.9;
  box-shadow: 0 0 12px rgba(245, 158, 11, 0.3);
  transform: translateY(-1px);
}

.btn-info {
  background: var(--bg-card);
  color: var(--primary);
  border-color: var(--border);
}

.btn-info:hover {
  background: var(--primary-bg);
  border-color: var(--primary);
  transform: translateY(-1px);
}

/* ===== Alert List ===== */
.alert-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 24px;
}

.alert-group {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  overflow: hidden;
  box-shadow: var(--shadow);
  position: relative;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.alert-group::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.7;
}

.alert-group:hover {
  box-shadow: var(--shadow-card);
  border-color: var(--border-strong);
}

.alert-group-header {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 16px;
  background: var(--primary-bg);
  border-bottom: 1px solid var(--border-soft);
}

.alert-group-icon {
  font-size: 16px;
}

.alert-group-title {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-strong);
  flex: 1;
}

.alert-group-count {
  background: var(--danger);
  color: #fff;
  font-size: 12px;
  font-weight: 600;
  padding: 2px 10px;
  border-radius: 10px;
  box-shadow: 0 0 8px rgba(239, 68, 68, 0.3);
}

.alert-group-count.new {
  background: var(--warning);
  color: #0f172a;
  box-shadow: 0 0 8px rgba(245, 158, 11, 0.3);
}

.alert-items {
  padding: 8px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.alert-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 12px 14px;
  border-radius: 8px;
  background: var(--bg-soft);
  border: 1px solid var(--border-soft);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
}

.alert-item::before {
  content: '';
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 3px;
  height: 60%;
  border-radius: 0 3px 3px 0;
}

.alert-item.warning::before {
  background: var(--danger);
  box-shadow: 0 0 8px var(--danger);
}

.alert-item.info::before {
  background: var(--warning);
  box-shadow: 0 0 8px var(--warning);
}

.alert-item:hover {
  background: var(--bg-hover);
  border-color: var(--border);
  transform: translateX(4px);
}

.alert-info {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
  flex: 1;
}

.alert-name {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-strong);
}

.alert-detail {
  font-size: 12px;
  color: var(--text-dim);
  padding: 2px 8px;
  background: var(--bg-card);
  border: 1px solid var(--border-soft);
  border-radius: 4px;
}

.alert-actions {
  display: flex;
  gap: 8px;
  flex-shrink: 0;
}

/* ===== Intervention List ===== */
.intervention-list {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 14px;
  margin-bottom: 24px;
}

.intervention-card {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 16px;
  box-shadow: var(--shadow);
  position: relative;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.intervention-card::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.7;
}

.intervention-card:hover {
  box-shadow: var(--shadow-card);
  border-color: var(--border-strong);
  transform: translateY(-2px);
}

.intervention-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 12px;
}

.intervention-title {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.intervention-icon {
  font-size: 20px;
}

.intervention-name {
  font-size: 15px;
  font-weight: 600;
  color: var(--text-strong);
}

.intervention-status {
  font-size: 11px;
  font-weight: 600;
  padding: 2px 10px;
  border-radius: 10px;
}

.status-active {
  background: rgba(56, 189, 248, 0.15);
  color: var(--primary);
  border: 1px solid rgba(56, 189, 248, 0.3);
}

.status-completed {
  background: rgba(34, 197, 94, 0.15);
  color: var(--success);
  border: 1px solid rgba(34, 197, 94, 0.3);
}

.status-pending {
  background: rgba(148, 163, 184, 0.15);
  color: var(--text-dim);
  border: 1px solid rgba(148, 163, 184, 0.3);
}

.intervention-school {
  font-size: 12px;
  color: var(--text-muted);
  white-space: nowrap;
}

.intervention-body {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.intervention-info {
  display: flex;
  flex-wrap: wrap;
  gap: 14px;
  font-size: 12px;
  color: var(--text-dim);
}

.intervention-progress {
  display: flex;
  align-items: center;
  gap: 10px;
}

.progress-label {
  font-size: 12px;
  color: var(--text-dim);
  white-space: nowrap;
}

.progress-bar {
  flex: 1;
  height: 6px;
  background: var(--bg-soft);
  border-radius: 3px;
  overflow: hidden;
  border: 1px solid var(--border-soft);
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--primary), var(--secondary));
  border-radius: 3px;
  transition: width 0.5s ease;
  box-shadow: 0 0 10px var(--primary-bg);
}

.progress-value {
  font-size: 13px;
  font-weight: 600;
  color: var(--primary);
  font-family: 'Orbitron', 'Consolas', monospace;
  min-width: 40px;
  text-align: right;
}

.intervention-actions {
  display: flex;
  gap: 8px;
  margin-top: 14px;
  padding-top: 12px;
  border-top: 1px solid var(--border-soft);
}

/* ===== Compare Chart ===== */
.compare-chart-wrapper {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 16px;
  margin-bottom: 24px;
}

.chart-card {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 16px;
  box-shadow: var(--shadow);
  position: relative;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.chart-card::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.7;
}

.chart-card:hover {
  box-shadow: var(--shadow-card);
  border-color: var(--border-strong);
}

.chart-container {
  width: 100%;
  height: 300px;
}

.compare-summary {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.compare-item {
  background: var(--bg-card);
  border: 1px solid var(--border);
  border-radius: 10px;
  padding: 14px 16px;
  box-shadow: var(--shadow);
  position: relative;
}

.compare-item::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), transparent);
  border-radius: 10px 10px 0 0;
  opacity: 0.5;
}

.compare-label {
  font-size: 12px;
  color: var(--text-dim);
  display: block;
  margin-bottom: 8px;
}

.compare-values {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 6px;
  flex-wrap: wrap;
}

.compare-before {
  font-size: 13px;
  color: var(--text-muted);
  text-decoration: line-through;
}

.compare-arrow {
  color: var(--primary);
  font-weight: bold;
}

.compare-after {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-strong);
}

.compare-change {
  font-size: 12px;
  font-weight: 600;
}

.compare-change.good {
  color: var(--success);
}

.compare-change.bad {
  color: var(--danger);
}

/* ===== Modal ===== */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(4px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  animation: modalFadeIn 0.2s ease;
}

@keyframes modalFadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-content {
  background: var(--bg-card);
  border: 1px solid var(--border-strong);
  border-radius: 12px;
  box-shadow: 0 0 40px var(--primary-bg), var(--shadow-card);
  backdrop-filter: blur(12px);
  width: 90%;
  max-width: 600px;
  max-height: 85vh;
  overflow: hidden;
  animation: modalSlideIn 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

@keyframes modalSlideIn {
  from { opacity: 0; transform: translateY(-20px) scale(0.96); }
  to { opacity: 1; transform: translateY(0) scale(1); }
}

.modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  border-bottom: 1px solid var(--border);
  background: var(--primary-bg);
}

.modal-header h3 {
  font-size: 16px;
  font-weight: 600;
  color: var(--text-strong);
  margin: 0;
}

.modal-close {
  width: 32px;
  height: 32px;
  border-radius: 6px;
  border: 1px solid var(--border);
  background: transparent;
  color: var(--text-dim);
  font-size: 14px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}

.modal-close:hover {
  background: var(--danger);
  color: #fff;
  border-color: var(--danger);
}

.modal-body {
  padding: 20px;
  overflow-y: auto;
}

.evaluate-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 14px;
  margin-bottom: 16px;
}

.evaluate-item {
  background: var(--bg-soft);
  border: 1px solid var(--border-soft);
  border-radius: 8px;
  padding: 12px 14px;
}

.evaluate-label {
  font-size: 12px;
  color: var(--text-dim);
  display: block;
  margin-bottom: 6px;
}

.evaluate-value {
  font-size: 20px;
  font-weight: 700;
  color: var(--text-strong);
  font-family: 'Orbitron', 'Consolas', monospace;
}

.evaluate-value.improve {
  color: var(--success);
}

.evaluate-value.worsen {
  color: var(--danger);
}

.rating-excellent {
  color: var(--success) !important;
}

.rating-good {
  color: var(--primary) !important;
}

.rating-fair {
  color: var(--warning) !important;
}

.evaluate-chart {
  width: 100%;
  height: 220px;
}

/* ===== Responsive ===== */
@media (max-width: 1200px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  .compare-chart-wrapper {
    grid-template-columns: 1fr;
  }
  .intervention-list {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .page-header {
    padding: 8px 12px;
    gap: 8px;
  }
  .page-title {
    font-size: 17px;
    letter-spacing: 1px;
  }
  .header-deco {
    display: none;
  }
  .stats-grid {
    grid-template-columns: 1fr;
  }
  .tab-item {
    padding: 10px 8px;
    font-size: 12px;
  }
  .tab-label {
    display: none;
  }
  .evaluate-grid {
    grid-template-columns: 1fr;
  }
  .alert-item {
    flex-direction: column;
    align-items: flex-start;
  }
  .alert-actions {
    width: 100%;
    justify-content: flex-end;
  }
}

/* ===== Light Mode Overrides ===== */
.dashboard-wrapper.theme-light .scan-line {
  opacity: 0.12;
  height: 1px;
}

.dashboard-wrapper.theme-light .bg-glow {
  opacity: 0.1;
  filter: blur(80px);
}

.dashboard-wrapper.theme-light .bg-grid {
  opacity: 0.4;
  background-image:
    linear-gradient(rgba(8, 145, 178, 0.06) 1px, transparent 1px),
    linear-gradient(90deg, rgba(8, 145, 178, 0.06) 1px, transparent 1px);
}

.dashboard-wrapper.theme-light .page-header {
  background: linear-gradient(90deg, rgba(8, 145, 178, 0.08), transparent 65%);
  border-color: rgba(8, 145, 178, 0.2);
}

.dashboard-wrapper.theme-light .page-header::before {
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, #0891b2, transparent);
}

.dashboard-wrapper.theme-light .title-text {
  background: linear-gradient(180deg, #1e293b 0%, #0891b2 100%);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  filter: drop-shadow(0 0 10px rgba(8, 145, 178, 0.18));
}

.dashboard-wrapper.theme-light .title-bracket {
  color: #0891b2;
  opacity: 0.6;
}

.dashboard-wrapper.theme-light .stat-value {
  color: #0891b2;
  text-shadow: 0 0 8px rgba(8, 145, 178, 0.3);
}

.dashboard-wrapper.theme-light .tab-bar,
.dashboard-wrapper.theme-light .stat-card,
.dashboard-wrapper.theme-light .alert-group,
.dashboard-wrapper.theme-light .intervention-card,
.dashboard-wrapper.theme-light .chart-card,
.dashboard-wrapper.theme-light .compare-item {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  box-shadow: 0 2px 8px rgba(15, 23, 42, 0.05), 0 1px 2px rgba(15, 23, 42, 0.04);
  border-color: rgba(8, 145, 178, 0.22);
  backdrop-filter: none;
}

.dashboard-wrapper.theme-light .tab-bar::before,
.dashboard-wrapper.theme-light .stat-card::before,
.dashboard-wrapper.theme-light .alert-group::before,
.dashboard-wrapper.theme-light .intervention-card::before,
.dashboard-wrapper.theme-light .chart-card::before,
.dashboard-wrapper.theme-light .compare-item::before {
  opacity: 0.65;
  background: linear-gradient(90deg, transparent, #0891b2, #7c3aed, transparent);
}

.dashboard-wrapper.theme-light .tab-item:hover {
  background: rgba(8, 145, 178, 0.05);
}

.dashboard-wrapper.theme-light .tab-item.active {
  background: rgba(8, 145, 178, 0.1);
  color: #0891b2;
  border-color: rgba(8, 145, 178, 0.25);
  box-shadow: none;
}

.dashboard-wrapper.theme-light .alert-item {
  background: linear-gradient(180deg, #fafcff 0%, #f5f9ff 100%);
  border-color: rgba(8, 145, 178, 0.15);
}

.dashboard-wrapper.theme-light .alert-item:hover {
  background: rgba(8, 145, 178, 0.05);
  border-color: rgba(8, 145, 178, 0.3);
}

.dashboard-wrapper.theme-light .alert-detail {
  background: #f8fafc;
  border-color: rgba(8, 145, 178, 0.15);
  color: #475569;
}

.dashboard-wrapper.theme-light .btn-primary {
  background: linear-gradient(135deg, #0891b2 0%, #0e7490 100%);
  border-color: #0891b2;
  color: #fff;
  box-shadow: 0 2px 8px rgba(8, 145, 178, 0.3);
}

.dashboard-wrapper.theme-light .btn-primary:hover {
  background: linear-gradient(135deg, #0e7490 0%, #155e75 100%);
  box-shadow: 0 4px 12px rgba(8, 145, 178, 0.4);
}

.dashboard-wrapper.theme-light .btn-success {
  background: linear-gradient(135deg, #16a34a 0%, #15803d 100%);
  border-color: #16a34a;
  color: #fff;
}

.dashboard-wrapper.theme-light .btn-warning {
  background: linear-gradient(135deg, #d97706 0%, #b45309 100%);
  border-color: #d97706;
  color: #fff;
}

.dashboard-wrapper.theme-light .btn-info {
  background: #fff;
  border-color: rgba(8, 145, 178, 0.25);
  color: #0891b2;
}

.dashboard-wrapper.theme-light .btn-info:hover {
  background: rgba(8, 145, 178, 0.06);
  border-color: #0891b2;
}

.dashboard-wrapper.theme-light .progress-bar {
  background: #f1f5f9;
  border-color: rgba(8, 145, 178, 0.15);
}

.dashboard-wrapper.theme-light .modal-overlay {
  background: rgba(15, 23, 42, 0.45);
}

.dashboard-wrapper.theme-light .modal-content {
  background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
  border: 1px solid rgba(8, 145, 178, 0.2);
  box-shadow: 0 20px 60px rgba(15, 23, 42, 0.12), 0 0 0 1px rgba(8, 145, 178, 0.08);
  backdrop-filter: none;
}

.dashboard-wrapper.theme-light .modal-header {
  background: linear-gradient(180deg, #f0f9ff 0%, #ffffff 100%);
  border-bottom-color: rgba(8, 145, 178, 0.12);
}

.dashboard-wrapper.theme-light .modal-header h3 {
  color: #0f172a;
}

.dashboard-wrapper.theme-light .evaluate-item {
  background: linear-gradient(180deg, #fafcff 0%, #f5f9ff 100%);
  border-color: rgba(8, 145, 178, 0.15);
}

.dashboard-wrapper.theme-light .evaluate-value {
  color: #0f172a;
}

.dashboard-wrapper.theme-light .stat-icon.red {
  background: rgba(220, 38, 38, 0.08);
  border-color: rgba(220, 38, 38, 0.25);
}

.dashboard-wrapper.theme-light .stat-icon.orange {
  background: rgba(217, 119, 6, 0.08);
  border-color: rgba(217, 119, 6, 0.25);
}

.dashboard-wrapper.theme-light .stat-icon.blue {
  background: rgba(8, 145, 178, 0.08);
  border-color: rgba(8, 145, 178, 0.25);
}

.dashboard-wrapper.theme-light .stat-icon.green {
  background: rgba(22, 163, 74, 0.08);
  border-color: rgba(22, 163, 74, 0.25);
}

.dashboard-wrapper.theme-light .status-active {
  background: rgba(8, 145, 178, 0.08);
  color: #0891b2;
  border-color: rgba(8, 145, 178, 0.25);
}

.dashboard-wrapper.theme-light .status-completed {
  background: rgba(22, 163, 74, 0.08);
  color: #16a34a;
  border-color: rgba(22, 163, 74, 0.25);
}

.dashboard-wrapper.theme-light .progress-fill {
  background: linear-gradient(90deg, #0891b2, #7c3aed);
  box-shadow: none;
}

.dashboard-wrapper.theme-light .progress-value {
  color: #0891b2;
}

.dashboard-wrapper.theme-light .hd-line {
  background: linear-gradient(90deg, #0891b2, transparent);
}

.dashboard-wrapper.theme-light .hd-line::after {
  background: #0891b2;
  box-shadow: 0 0 4px rgba(8, 145, 178, 0.45);
}

.dashboard-wrapper.theme-light .hd-dot {
  background: #0891b2;
  box-shadow: 0 0 6px rgba(8, 145, 178, 0.45);
}

.dashboard-wrapper.theme-light ::-webkit-scrollbar-thumb {
  background: linear-gradient(180deg, rgba(8, 145, 178, 0.4) 0%, rgba(8, 145, 178, 0.25) 100%);
}

.dashboard-wrapper.theme-light ::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(180deg, rgba(8, 145, 178, 0.55) 0%, rgba(8, 145, 178, 0.4) 100%);
}
</style>
