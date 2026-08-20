<template>
  <div class="dashboard-wrapper" :class="{ 'theme-light': isLight }">
    <div class="bg-decor" aria-hidden="true">
      <div class="bg-grid"></div>
      <div class="bg-glow bg-glow-1"></div>
      <div class="bg-glow bg-glow-2"></div>
    </div>
    <div class="scan-line"></div>

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

    <div class="tab-bar">
      <div
        v-for="tab in tabs"
        :key="tab.key"
        class="tab-item"
        :class="{ active: activeTab === tab.key }"
        @click="switchTab(tab.key)"
      >
        <span class="tab-icon">{{ tab.icon }}</span>
        <span class="tab-label">{{ tab.label }}</span>
      </div>
    </div>

    <div class="tab-content">
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
            <button class="btn btn-primary" @click="openEntryModal('vision')">📝 单个录入</button>
            <button class="btn btn-success" @click="openExcelModal('vision')">📤 Excel批量导入</button>
            <button class="btn btn-info" @click="openDeviceModal('vision')">📱 设备自动采集</button>
            <button class="btn btn-warning" @click="handleCheckData('vision')">🔍 数据校验</button>
          </div>
          <div class="toolbar-right">
            <span class="toolbar-tip">审核流程：班主任录入 → 校医审核 → 区县教育局抽检</span>
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
              <button class="btn btn-sm btn-primary" @click="loadVisionData(1)">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr>
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
              <tr v-if="loadingVision">
                <td colspan="11" style="text-align: center; padding: 24px; color: var(--text-dim);">加载中...</td>
              </tr>
              <tr v-else-if="visionData.length === 0">
                <td colspan="11" style="text-align: center; padding: 24px; color: var(--text-dim);">暂无数据，请录入或导入</td>
              </tr>
              <tr v-for="item in visionData" v-else :key="item.id">
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.studentName }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.className }}</td>
                <td>{{ item.leftEye }}</td>
                <td>{{ item.rightEye }}</td>
                <td><span class="level-tag" :class="getLevelClass(item.visionLevel)">{{ item.visionLevel }}</span></td>
                <td>{{ item.recorderName }}<span v-if="item.source === 'device'" class="source-tag" title="设备采集">📱</span></td>
                <td>{{ item.recordTime }}</td>
                <td><span class="status-tag" :class="getStatusClass(item.status)">{{ getStatusText(item.status) }}</span></td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleApprove('vision', item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleReject('vision', item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleSpotCheck('vision', item)" title="标记为待抽检">🔍</button>
                    <template v-if="item.status === 'spot'">
                      <button class="btn-icon approve" @click="handleSpotFinish('vision', item, 'approved')" title="完成抽检（通过）">✅</button>
                      <button class="btn-icon reject" @click="handleSpotFinish('vision', item, 'abnormal')" title="抽检不通过">❌</button>
                    </template>
                    <button class="btn-icon edit" @click="openEditModal('vision', item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleDelete('vision', item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ visionPagination.total }} 条 · 第 {{ visionPagination.page }} / {{ visionPagination.pageCount }} 页</span>
            <div class="pagination-btns">
              <button class="page-btn" :disabled="visionPagination.page <= 1" @click="loadVisionData(visionPagination.page - 1)">上一页</button>
              <button v-for="p in visionPagination.pageCount" :key="p" class="page-btn" :class="{ active: p === visionPagination.page }" @click="loadVisionData(p)">{{ p }}</button>
              <button class="page-btn" :disabled="visionPagination.page >= visionPagination.pageCount" @click="loadVisionData(visionPagination.page + 1)">下一页</button>
            </div>
          </div>
        </div>
      </div>

      <div v-show="activeTab === 'oral'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card"><div class="stat-icon blue">📊</div><div class="stat-info"><div class="stat-label">待审核数据</div><div class="stat-value">{{ oralStats.pending }}</div></div></div>
          <div class="stat-card"><div class="stat-icon green">✅</div><div class="stat-info"><div class="stat-label">已审核数据</div><div class="stat-value">{{ oralStats.approved }}</div></div></div>
          <div class="stat-card"><div class="stat-icon orange">⚠️</div><div class="stat-info"><div class="stat-label">异常数据提醒</div><div class="stat-value">{{ oralStats.abnormal }}</div></div></div>
          <div class="stat-card"><div class="stat-icon red">📋</div><div class="stat-info"><div class="stat-label">待抽检数据</div><div class="stat-value">{{ oralStats.spotCheck }}</div></div></div>
        </div>
        <div class="toolbar">
          <div class="toolbar-left">
            <button class="btn btn-primary" @click="openEntryModal('oral')">📝 单个录入</button>
            <button class="btn btn-success" @click="openExcelModal('oral')">📤 Excel批量导入</button>
            <button class="btn btn-info" @click="openDeviceModal('oral')">📱 设备自动采集</button>
            <button class="btn btn-warning" @click="handleCheckData('oral')">🔍 数据校验</button>
          </div>
          <div class="toolbar-right"><span class="toolbar-tip">审核流程：班主任录入 → 校医审核 → 区县教育局抽检</span></div>
        </div>
        <div class="data-table-wrapper">
          <div class="table-header">
            <h3>📋 口腔数据采集列表</h3>
            <div class="table-filter">
              <select v-model="oralFilter.status" class="filter-select">
                <option value="all">全部状态</option><option value="pending">待审核</option><option value="approved">已审核</option><option value="abnormal">异常</option><option value="spot">待抽检</option>
              </select>
              <input v-model="oralFilter.keyword" type="text" placeholder="搜索姓名/学号..." class="filter-input" />
              <button class="btn btn-sm btn-primary" @click="loadOralData(1)">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr><th>学号</th><th>姓名</th><th>年级</th><th>班级</th><th>乳牙龋</th><th>恒牙龋</th><th>替牙情况</th><th>颌面发育</th><th>录入人</th><th>录入时间</th><th>状态</th><th>操作</th></tr>
            </thead>
            <tbody>
              <tr v-if="loadingOral"><td colspan="12" style="text-align: center; padding: 24px; color: var(--text-dim);">加载中...</td></tr>
              <tr v-else-if="oralData.length === 0"><td colspan="12" style="text-align: center; padding: 24px; color: var(--text-dim);">暂无数据</td></tr>
              <tr v-for="item in oralData" v-else :key="item.id">
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.studentName }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.className }}</td>
                <td>{{ item.decayedBabyTeeth }}</td>
                <td>{{ item.decayedPermanentTeeth }}</td>
                <td>{{ item.toothStage }}</td>
                <td>{{ item.jawDevelopment }}</td>
                <td>{{ item.recorderName }}<span v-if="item.source === 'device'" class="source-tag" title="设备采集">📱</span></td>
                <td>{{ item.recordTime }}</td>
                <td><span class="status-tag" :class="getStatusClass(item.status)">{{ getStatusText(item.status) }}</span></td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleApprove('oral', item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleReject('oral', item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleSpotCheck('oral', item)" title="标记为待抽检">🔍</button>
                    <template v-if="item.status === 'spot'">
                      <button class="btn-icon approve" @click="handleSpotFinish('oral', item, 'approved')" title="完成抽检（通过）">✅</button>
                      <button class="btn-icon reject" @click="handleSpotFinish('oral', item, 'abnormal')" title="抽检不通过">❌</button>
                    </template>
                    <button class="btn-icon edit" @click="openEditModal('oral', item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleDelete('oral', item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ oralPagination.total }} 条 · 第 {{ oralPagination.page }} / {{ oralPagination.pageCount }} 页</span>
            <div class="pagination-btns">
              <button class="page-btn" :disabled="oralPagination.page <= 1" @click="loadOralData(oralPagination.page - 1)">上一页</button>
              <button v-for="p in oralPagination.pageCount" :key="p" class="page-btn" :class="{ active: p === oralPagination.page }" @click="loadOralData(p)">{{ p }}</button>
              <button class="page-btn" :disabled="oralPagination.page >= oralPagination.pageCount" @click="loadOralData(oralPagination.page + 1)">下一页</button>
            </div>
          </div>
        </div>
      </div>

      <div v-show="activeTab === 'mental'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card"><div class="stat-icon blue">📊</div><div class="stat-info"><div class="stat-label">待审核数据</div><div class="stat-value">{{ mentalStats.pending }}</div></div></div>
          <div class="stat-card"><div class="stat-icon green">✅</div><div class="stat-info"><div class="stat-label">已审核数据</div><div class="stat-value">{{ mentalStats.approved }}</div></div></div>
          <div class="stat-card"><div class="stat-icon orange">⚠️</div><div class="stat-info"><div class="stat-label">异常数据提醒</div><div class="stat-value">{{ mentalStats.abnormal }}</div></div></div>
          <div class="stat-card"><div class="stat-icon red">📋</div><div class="stat-info"><div class="stat-label">待抽检数据</div><div class="stat-value">{{ mentalStats.spotCheck }}</div></div></div>
        </div>
        <div class="toolbar">
          <div class="toolbar-left">
            <button class="btn btn-primary" @click="openEntryModal('mental')">📝 单个录入</button>
            <button class="btn btn-success" @click="openExcelModal('mental')">📤 Excel批量导入</button>
            <button class="btn btn-info" @click="openDeviceModal('mental')">📱 设备自动采集</button>
            <button class="btn btn-warning" @click="handleCheckData('mental')">🔍 数据校验</button>
          </div>
          <div class="toolbar-right"><span class="toolbar-tip">审核流程：班主任录入 → 心理老师审核 → 区县教育局抽检</span></div>
        </div>
        <div class="data-table-wrapper">
          <div class="table-header">
            <h3>📋 心理数据采集列表</h3>
            <div class="table-filter">
              <select v-model="mentalFilter.status" class="filter-select">
                <option value="all">全部状态</option><option value="pending">待审核</option><option value="approved">已审核</option><option value="abnormal">异常</option><option value="spot">待抽检</option>
              </select>
              <input v-model="mentalFilter.keyword" type="text" placeholder="搜索姓名/学号..." class="filter-input" />
              <button class="btn btn-sm btn-primary" @click="loadMentalData(1)">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr><th>学号</th><th>姓名</th><th>年级</th><th>班级</th><th>焦虑评分</th><th>抑郁评分</th><th>学习焦虑</th><th>人际敏感</th><th>录入人</th><th>录入时间</th><th>状态</th><th>操作</th></tr>
            </thead>
            <tbody>
              <tr v-if="loadingMental"><td colspan="12" style="text-align: center; padding: 24px; color: var(--text-dim);">加载中...</td></tr>
              <tr v-else-if="mentalData.length === 0"><td colspan="12" style="text-align: center; padding: 24px; color: var(--text-dim);">暂无数据</td></tr>
              <tr v-for="item in mentalData" v-else :key="item.id">
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.studentName }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.className }}</td>
                <td>{{ item.anxietyScore }}</td>
                <td>{{ item.depressionScore }}</td>
                <td>{{ item.learningAnxiety }}</td>
                <td>{{ item.interpersonalSensitivity }}</td>
                <td>{{ item.recorderName }}<span v-if="item.source === 'device'" class="source-tag" title="设备采集">📱</span></td>
                <td>{{ item.recordTime }}</td>
                <td><span class="status-tag" :class="getStatusClass(item.status)">{{ getStatusText(item.status) }}</span></td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleApprove('mental', item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleReject('mental', item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleSpotCheck('mental', item)" title="标记为待抽检">🔍</button>
                    <template v-if="item.status === 'spot'">
                      <button class="btn-icon approve" @click="handleSpotFinish('mental', item, 'approved')" title="完成抽检（通过）">✅</button>
                      <button class="btn-icon reject" @click="handleSpotFinish('mental', item, 'abnormal')" title="抽检不通过">❌</button>
                    </template>
                    <button class="btn-icon edit" @click="openEditModal('mental', item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleDelete('mental', item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ mentalPagination.total }} 条 · 第 {{ mentalPagination.page }} / {{ mentalPagination.pageCount }} 页</span>
            <div class="pagination-btns">
              <button class="page-btn" :disabled="mentalPagination.page <= 1" @click="loadMentalData(mentalPagination.page - 1)">上一页</button>
              <button v-for="p in mentalPagination.pageCount" :key="p" class="page-btn" :class="{ active: p === mentalPagination.page }" @click="loadMentalData(p)">{{ p }}</button>
              <button class="page-btn" :disabled="mentalPagination.page >= mentalPagination.pageCount" @click="loadMentalData(mentalPagination.page + 1)">下一页</button>
            </div>
          </div>
        </div>
      </div>

      <div v-show="activeTab === 'weight'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card"><div class="stat-icon blue">📊</div><div class="stat-info"><div class="stat-label">待审核数据</div><div class="stat-value">{{ weightStats.pending }}</div></div></div>
          <div class="stat-card"><div class="stat-icon green">✅</div><div class="stat-info"><div class="stat-label">已审核数据</div><div class="stat-value">{{ weightStats.approved }}</div></div></div>
          <div class="stat-card"><div class="stat-icon orange">⚠️</div><div class="stat-info"><div class="stat-label">异常数据提醒</div><div class="stat-value">{{ weightStats.abnormal }}</div></div></div>
          <div class="stat-card"><div class="stat-icon red">📋</div><div class="stat-info"><div class="stat-label">待抽检数据</div><div class="stat-value">{{ weightStats.spotCheck }}</div></div></div>
        </div>
        <div class="toolbar">
          <div class="toolbar-left">
            <button class="btn btn-primary" @click="openEntryModal('weight')">📝 单个录入</button>
            <button class="btn btn-success" @click="openExcelModal('weight')">📤 Excel批量导入</button>
            <button class="btn btn-info" @click="openDeviceModal('weight')">📱 设备自动采集</button>
            <button class="btn btn-warning" @click="handleCheckData('weight')">🔍 数据校验</button>
          </div>
          <div class="toolbar-right"><span class="toolbar-tip">审核流程：体育老师录入 → 校医审核 → 区县教育局抽检</span></div>
        </div>
        <div class="data-table-wrapper">
          <div class="table-header">
            <h3>📋 健康体重数据采集列表</h3>
            <div class="table-filter">
              <select v-model="weightFilter.status" class="filter-select">
                <option value="all">全部状态</option><option value="pending">待审核</option><option value="approved">已审核</option><option value="abnormal">异常</option><option value="spot">待抽检</option>
              </select>
              <input v-model="weightFilter.keyword" type="text" placeholder="搜索姓名/学号..." class="filter-input" />
              <button class="btn btn-sm btn-primary" @click="loadWeightData(1)">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr><th>学号</th><th>姓名</th><th>年级</th><th>班级</th><th>身高</th><th>体重</th><th>BMI</th><th>BMI等级</th><th>腰围</th><th>臀围</th><th>腰臀比</th><th>录入人</th><th>录入时间</th><th>状态</th><th>操作</th></tr>
            </thead>
            <tbody>
              <tr v-if="loadingWeight"><td colspan="15" style="text-align: center; padding: 24px; color: var(--text-dim);">加载中...</td></tr>
              <tr v-else-if="weightData.length === 0"><td colspan="15" style="text-align: center; padding: 24px; color: var(--text-dim);">暂无数据</td></tr>
              <tr v-for="item in weightData" v-else :key="item.id">
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.studentName }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.className }}</td>
                <td>{{ item.height }}</td>
                <td>{{ item.weight }}</td>
                <td>{{ item.bmi }}</td>
                <td><span class="level-tag" :class="getWeightLevelClass(item.bmiLevel)">{{ item.bmiLevel }}</span></td>
                <td>{{ item.waistCircumference }}</td>
                <td>{{ item.hipCircumference }}</td>
                <td>{{ item.whr }}</td>
                <td>{{ item.recorderName }}<span v-if="item.source === 'device'" class="source-tag" title="设备采集">📱</span></td>
                <td>{{ item.recordTime }}</td>
                <td><span class="status-tag" :class="getStatusClass(item.status)">{{ getStatusText(item.status) }}</span></td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleApprove('weight', item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleReject('weight', item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleSpotCheck('weight', item)" title="标记为待抽检">🔍</button>
                    <template v-if="item.status === 'spot'">
                      <button class="btn-icon approve" @click="handleSpotFinish('weight', item, 'approved')" title="完成抽检（通过）">✅</button>
                      <button class="btn-icon reject" @click="handleSpotFinish('weight', item, 'abnormal')" title="抽检不通过">❌</button>
                    </template>
                    <button class="btn-icon edit" @click="openEditModal('weight', item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleDelete('weight', item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ weightPagination.total }} 条 · 第 {{ weightPagination.page }} / {{ weightPagination.pageCount }} 页</span>
            <div class="pagination-btns">
              <button class="page-btn" :disabled="weightPagination.page <= 1" @click="loadWeightData(weightPagination.page - 1)">上一页</button>
              <button v-for="p in weightPagination.pageCount" :key="p" class="page-btn" :class="{ active: p === weightPagination.page }" @click="loadWeightData(p)">{{ p }}</button>
              <button class="page-btn" :disabled="weightPagination.page >= weightPagination.pageCount" @click="loadWeightData(weightPagination.page + 1)">下一页</button>
            </div>
          </div>
        </div>
      </div>

      <div v-show="activeTab === 'bone'" class="tab-panel">
        <div class="stats-grid">
          <div class="stat-card"><div class="stat-icon blue">📊</div><div class="stat-info"><div class="stat-label">待审核数据</div><div class="stat-value">{{ boneStats.pending }}</div></div></div>
          <div class="stat-card"><div class="stat-icon green">✅</div><div class="stat-info"><div class="stat-label">已审核数据</div><div class="stat-value">{{ boneStats.approved }}</div></div></div>
          <div class="stat-card"><div class="stat-icon orange">⚠️</div><div class="stat-info"><div class="stat-label">异常数据提醒</div><div class="stat-value">{{ boneStats.abnormal }}</div></div></div>
          <div class="stat-card"><div class="stat-icon red">📋</div><div class="stat-info"><div class="stat-label">待抽检数据</div><div class="stat-value">{{ boneStats.spotCheck }}</div></div></div>
        </div>
        <div class="toolbar">
          <div class="toolbar-left">
            <button class="btn btn-primary" @click="openEntryModal('bone')">📝 单个录入</button>
            <button class="btn btn-success" @click="openExcelModal('bone')">📤 Excel批量导入</button>
            <button class="btn btn-info" @click="openDeviceModal('bone')">📱 设备自动采集</button>
            <button class="btn btn-warning" @click="handleCheckData('bone')">🔍 数据校验</button>
          </div>
          <div class="toolbar-right"><span class="toolbar-tip">审核流程：校医录入 → 专家审核 → 区县教育局抽检</span></div>
        </div>
        <div class="data-table-wrapper">
          <div class="table-header">
            <h3>📋 骨骼健康数据采集列表</h3>
            <div class="table-filter">
              <select v-model="boneFilter.status" class="filter-select">
                <option value="all">全部状态</option><option value="pending">待审核</option><option value="approved">已审核</option><option value="abnormal">异常</option><option value="spot">待抽检</option>
              </select>
              <input v-model="boneFilter.keyword" type="text" placeholder="搜索姓名/学号..." class="filter-input" />
              <button class="btn btn-sm btn-primary" @click="loadBoneData(1)">筛选</button>
            </div>
          </div>
          <table class="data-table">
            <thead>
              <tr><th>学号</th><th>姓名</th><th>年级</th><th>班级</th><th>骨密度</th><th>骨密度等级</th><th>骨龄</th><th>维D</th><th>钙水平</th><th>录入人</th><th>录入时间</th><th>状态</th><th>操作</th></tr>
            </thead>
            <tbody>
              <tr v-if="loadingBone"><td colspan="13" style="text-align: center; padding: 24px; color: var(--text-dim);">加载中...</td></tr>
              <tr v-else-if="boneData.length === 0"><td colspan="13" style="text-align: center; padding: 24px; color: var(--text-dim);">暂无数据</td></tr>
              <tr v-for="item in boneData" v-else :key="item.id">
                <td class="student-no">{{ item.studentNo }}</td>
                <td>{{ item.studentName }}</td>
                <td>{{ item.grade }}</td>
                <td>{{ item.className }}</td>
                <td>{{ item.boneDensity }}</td>
                <td><span class="level-tag" :class="getBoneLevelClass(item.boneLevel)">{{ item.boneLevel }}</span></td>
                <td>{{ item.boneAge }}</td>
                <td><span class="status-tag" :class="getVitaminClass(item.vitaminD)">{{ item.vitaminD }}</span></td>
                <td><span class="status-tag" :class="getCalciumClass(item.calciumLevel)">{{ item.calciumLevel }}</span></td>
                <td>{{ item.recorderName }}<span v-if="item.source === 'device'" class="source-tag" title="设备采集">📱</span></td>
                <td>{{ item.recordTime }}</td>
                <td><span class="status-tag" :class="getStatusClass(item.status)">{{ getStatusText(item.status) }}</span></td>
                <td>
                  <div class="action-btns">
                    <button v-if="item.status === 'pending'" class="btn-icon approve" @click="handleApprove('bone', item)" title="审核通过">✅</button>
                    <button v-if="item.status === 'pending' || item.status === 'abnormal'" class="btn-icon reject" @click="handleReject('bone', item)" title="驳回">❌</button>
                    <button v-if="item.status === 'approved'" class="btn-icon spot" @click="handleSpotCheck('bone', item)" title="标记为待抽检">🔍</button>
                    <template v-if="item.status === 'spot'">
                      <button class="btn-icon approve" @click="handleSpotFinish('bone', item, 'approved')" title="完成抽检（通过）">✅</button>
                      <button class="btn-icon reject" @click="handleSpotFinish('bone', item, 'abnormal')" title="抽检不通过">❌</button>
                    </template>
                    <button class="btn-icon edit" @click="openEditModal('bone', item)" title="编辑">✏️</button>
                    <button class="btn-icon delete" @click="handleDelete('bone', item)" title="删除">🗑️</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="table-pagination">
            <span>共 {{ bonePagination.total }} 条 · 第 {{ bonePagination.page }} / {{ bonePagination.pageCount }} 页</span>
            <div class="pagination-btns">
              <button class="page-btn" :disabled="bonePagination.page <= 1" @click="loadBoneData(bonePagination.page - 1)">上一页</button>
              <button v-for="p in bonePagination.pageCount" :key="p" class="page-btn" :class="{ active: p === bonePagination.page }" @click="loadBoneData(p)">{{ p }}</button>
              <button class="page-btn" :disabled="bonePagination.page >= bonePagination.pageCount" @click="loadBoneData(bonePagination.page + 1)">下一页</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <Modal v-model:open="entryModalOpen" :title="(entryFormMode === 'edit' ? '编辑' : '单个录入') + ' - ' + entryDimensionLabel" :width="780" :confirmLoading="entrySubmitting" @ok="submitEntry" @cancel="resetEntryForm" okText="保存" cancelText="取消">
      <AForm layout="vertical" :model="entryForm">
        <ARow :gutter="12">
          <ACol :span="12">
            <AFormItem label="学号" required :validateStatus="studentNoStatus" :help="studentNoHelp">
              <AAutoComplete
                v-model:value="entryForm.studentNo"
                :options="studentOptions"
                :filter-option="false"
                placeholder="输入学号或姓名检索（从学生电子档案加载）"
                @search="onStudentSearch"
                @select="(val, opt) => onStudentSelect(val, opt, 'no')"
                @blur="onStudentNoBlur"
                allow-clear
              >
                <template #option="{ value: val, label, name, grade, className: cls }">
                  <div style="display:flex;justify-content:space-between;gap:8px;">
                    <span><b>{{ val }}</b> · {{ name }}</span>
                    <span style="color:#999;font-size:12px;">{{ grade }} {{ cls }}</span>
                  </div>
                </template>
              </AAutoComplete>
            </AFormItem>
          </ACol>
          <ACol :span="12">
            <AFormItem label="姓名" required :validateStatus="studentNameStatus" :help="studentNameHelp">
              <AAutoComplete
                v-model:value="entryForm.studentName"
                :options="studentOptions"
                :filter-option="false"
                placeholder="输入姓名或学号检索"
                @search="onStudentSearch"
                @select="(val, opt) => onStudentSelect(val, opt, 'name')"
                @blur="onStudentNameBlur"
                allow-clear
              >
                <template #option="{ value: val, label, studentNo, grade, className: cls }">
                  <div style="display:flex;justify-content:space-between;gap:8px;">
                    <span>{{ val }} · <b>{{ studentNo }}</b></span>
                    <span style="color:#999;font-size:12px;">{{ grade }} {{ cls }}</span>
                  </div>
                </template>
              </AAutoComplete>
            </AFormItem>
          </ACol>
          <ACol :span="12"><AFormItem label="年级"><AInput v-model:value="entryForm.grade" placeholder="如 七年级" /></AFormItem></ACol>
          <ACol :span="12"><AFormItem label="班级"><AInput v-model:value="entryForm.className" placeholder="如 七年级五班" /></AFormItem></ACol>
          <ACol :span="8"><AFormItem label="检查日期" required><ADatePicker v-model:value="entryForm.checkDate" value-format="YYYY-MM-DD" format="YYYY-MM-DD" style="width:100%" placeholder="选择检查日期" /></AFormItem></ACol>

          <template v-if="entryDimension === 'vision'">
            <ACol :span="8"><AFormItem label="视力等级"><ASelect v-model:value="entryForm.visionLevel" :options="VISION_LEVELS" /></AFormItem></ACol>
            <ACol :span="8" />
            <ACol :span="12"><AFormItem label="左眼视力" extra="裸眼视力 3.0–5.3，<3.0 填 0"><AInput v-model:value="entryForm.leftEye" placeholder="如 4.8（范围 3.0–5.3）" /></AFormItem></ACol>
            <ACol :span="12"><AFormItem label="右眼视力" extra="裸眼视力 3.0–5.3，<3.0 填 0"><AInput v-model:value="entryForm.rightEye" placeholder="如 4.9（范围 3.0–5.3）" /></AFormItem></ACol>
          </template>
          <template v-else-if="entryDimension === 'oral'">
            <ACol :span="8"><AFormItem label="乳牙龋" extra="龋齿颗数 0–20"><AInputNumber v-model:value="entryForm.decayedBabyTeeth" :min="0" :max="20" style="width:100%" placeholder="0–20 颗" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="恒牙龋" extra="龋齿颗数 0–32"><AInputNumber v-model:value="entryForm.decayedPermanentTeeth" :min="0" :max="32" style="width:100%" placeholder="0–32 颗" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="替牙情况" extra="WHO 标准：6–12 岁替牙期"><ASelect v-model:value="entryForm.toothStage" :options="ORAL_STAGES" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="颌面发育"><ASelect v-model:value="entryForm.jawDevelopment" :options="ORAL_JAW" /></AFormItem></ACol>
          </template>
          <template v-else-if="entryDimension === 'mental'">
            <ACol :span="8"><AFormItem label="焦虑评分" extra="0–10 分：≤3 正常 / 4–6 轻度 / 7–8 中度 / ≥9 重度"><AInputNumber v-model:value="entryForm.anxietyScore" :min="0" :max="10" style="width:100%" placeholder="0–10 分" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="抑郁评分" extra="0–10 分：≤3 正常 / 4–6 轻度 / 7–8 中度 / ≥9 重度"><AInputNumber v-model:value="entryForm.depressionScore" :min="0" :max="10" style="width:100%" placeholder="0–10 分" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="学习焦虑" extra="4 级：正常 / 轻度 / 中度 / 重度"><ASelect v-model:value="entryForm.learningAnxiety" :options="MENTAL_LEVELS" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="人际敏感" extra="4 级：正常 / 轻度 / 中度 / 重度"><ASelect v-model:value="entryForm.interpersonalSensitivity" :options="MENTAL_LEVELS" /></AFormItem></ACol>
          </template>
          <template v-else-if="entryDimension === 'weight'">
            <ACol :span="8"><AFormItem label="身高 (cm)" extra="中小学生范围 100–200 cm"><AInput v-model:value="entryForm.height" placeholder="如 152（100–200）" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="体重 (kg)" extra="中小学生范围 20–100 kg"><AInput v-model:value="entryForm.weight" placeholder="如 42（20–100）" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="BMI" extra="体重÷身高²，正常 14.8–22.1（七年级）"><AInput v-model:value="entryForm.bmi" placeholder="如 18.2（13–30）" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="BMI 等级" extra="按《国家学生体质健康标准》分正常/低体重/超重/肥胖"><ASelect v-model:value="entryForm.bmiLevel" :options="WEIGHT_LEVELS" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="腰围 (cm)" extra="7–18 岁参考：男 < 75 / 女 < 70 为正常"><AInput v-model:value="entryForm.waistCircumference" placeholder="如 62（40–150）" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="臀围 (cm)"><AInput v-model:value="entryForm.hipCircumference" placeholder="如 78（50–160）" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="腰臀比" extra="男 < 0.9 / 女 < 0.8 为正常中心型肥胖参考"><AInput v-model:value="entryForm.whr" placeholder="如 0.79（0.5–1.5）" /></AFormItem></ACol>
          </template>
          <template v-else-if="entryDimension === 'bone'">
            <ACol :span="8"><AFormItem label="骨密度 (g/cm²)" extra="参考范围 0.5–1.5；≥0.85 正常"><AInput v-model:value="entryForm.boneDensity" placeholder="如 0.85（0.5–1.5）" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="骨密度等级" extra="正常 / 骨量减少 / 骨质疏松"><ASelect v-model:value="entryForm.boneLevel" :options="BONE_LEVELS" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="骨龄" extra="与实际年龄相差 ±3 岁为正常"><AInput v-model:value="entryForm.boneAge" placeholder="如 12岁" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="维生素 D" extra="充足 ≥30 / 不足 20–30 / 缺乏 <20 ng/mL"><ASelect v-model:value="entryForm.vitaminD" :options="VITAMIN_LEVELS" /></AFormItem></ACol>
            <ACol :span="8"><AFormItem label="钙水平" extra="正常 2.1–2.6 mmol/L"><ASelect v-model:value="entryForm.calciumLevel" :options="CALCIUM_LEVELS" /></AFormItem></ACol>
          </template>

          <ACol :span="24"><AFormItem label="备注"><ATextarea v-model:value="entryForm.remark" :rows="2" /></AFormItem></ACol>
        </ARow>
      </AForm>
    </Modal>

    <Modal v-model:open="excelModalOpen" :title="'Excel 批量导入 - ' + entryDimensionLabel" :width="720" :confirmLoading="excelSubmitting" @ok="submitExcel" @cancel="onExcelCancel" okText="导入" cancelText="取消">
      <ASpace direction="vertical" style="width:100%">
        <AAlert :message="`表头要求：${excelHeaderTip}`" type="info" show-icon />
        <AUpload :before-upload="handleExcelFile" :show-upload-list="false" accept=".xlsx,.xls">
          <AButton>选择 Excel 文件</AButton>
        </AUpload>
        <div v-if="excelRows.length > 0">
          <p>已解析 {{ excelRows.length }} 条数据：</p>
          <ATable :columns="excelPreviewColumns" :data-source="excelRows.slice(0, 5)" :pagination="false" size="small" />
          <p v-if="excelRows.length > 5" style="color: var(--text-dim)">...仅预览前 5 条</p>
        </div>
      </ASpace>
    </Modal>

    <Modal v-model:open="deviceModalOpen" :title="'设备自动采集 - ' + entryDimensionLabel" :width="640" :confirmLoading="deviceSubmitting" @ok="submitDevice" @cancel="deviceForm.collectedAt = ''" okText="开始采集" cancelText="取消">
      <AAlert :message="`设备 SDK 通过 POST /api/device/v1/${entryDimension}/ingest 推流，请求头 X-Device-Key: 5hp-device-default-key。下方可触发模拟推流。`" type="info" show-icon style="margin-bottom: 12px" />
      <AForm layout="vertical" :model="deviceForm">
        <ARow :gutter="12">
          <ACol :span="12"><AFormItem label="设备 SN"><AInput v-model:value="deviceForm.deviceSn" /></AFormItem></ACol>
          <ACol :span="12"><AFormItem label="采集时间"><AInput v-model:value="deviceForm.collectedAt" placeholder="留空则取当前时间" /></AFormItem></ACol>
          <ACol :span="12"><AFormItem label="操作人姓名"><AInput v-model:value="deviceForm.recorderName" /></AFormItem></ACol>
          <ACol :span="12"><AFormItem label="操作人 ID"><AInputNumber v-model:value="deviceForm.recorderId" :min="0" style="width:100%" /></AFormItem></ACol>
          <ACol :span="24"><AFormItem label="模拟采集条数">
            <AInputNumber v-model:value="deviceForm.simulateCount" :min="1" :max="20" style="width:100%" />
            <small style="color: var(--text-dim)">为方便演示，会自动生成 N 条示例数据推送到设备采集接口</small>
          </AFormItem></ACol>
        </ARow>
      </AForm>
    </Modal>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onBeforeUnmount, watch, h } from 'vue';
import {
  message,
  Modal,
  Form as AForm,
  FormItem as AFormItem,
  Input as AInput,
  InputNumber as AInputNumber,
  Select as ASelect,
  Upload as AUpload,
  Table as ATable,
  Alert as AAlert,
  Space as ASpace,
  Row as ARow,
  Col as ACol,
  Button as AButton,
  DatePicker as ADatePicker,
  AutoComplete as AAutoComplete,
} from 'ant-design-vue';
import { Textarea as ATextarea } from 'ant-design-vue/es/input';
import { healthArchiveApi, collectDeviceApi, studentApi } from '#/api/vision-archive';
import * as XLSX from 'xlsx';

// ================= Theme =================
const isLight = ref(false);
let themeObserver = null;
const applySystemTheme = () => { isLight.value = !document.documentElement.classList.contains('dark'); };
const observeTheme = () => {
  themeObserver = new MutationObserver(() => { isLight.value = !document.documentElement.classList.contains('dark'); });
  themeObserver.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
};

// ================= Tab =================
const tabs = [
  { key: 'vision', label: '视力健康', icon: '👁️' },
  { key: 'oral', label: '口腔健康', icon: '🦷' },
  { key: 'mental', label: '心理健康', icon: '🧠' },
  { key: 'weight', label: '健康体重', icon: '⚖️' },
  { key: 'bone', label: '骨骼健康', icon: '🦴' },
];
const activeTab = ref('vision');
const switchTab = (k) => { activeTab.value = k; };

// ================= 5 维度数据 =================
const defaultStats = () => ({ total: 0, pending: 0, approved: 0, abnormal: 0, spotCheck: 0, deviceIngested: 0, excelImported: 0 });
const defaultPaging = () => ({ page: 1, pageSize: 10, total: 0, pageCount: 1 });

const visionStats = reactive(defaultStats());
const oralStats = reactive(defaultStats());
const mentalStats = reactive(defaultStats());
const weightStats = reactive(defaultStats());
const boneStats = reactive(defaultStats());

const visionData = ref([]); const loadingVision = ref(false);
const oralData = ref([]);   const loadingOral = ref(false);
const mentalData = ref([]); const loadingMental = ref(false);
const weightData = ref([]); const loadingWeight = ref(false);
const boneData = ref([]);   const loadingBone = ref(false);

const visionPagination = reactive(defaultPaging());
const oralPagination = reactive(defaultPaging());
const mentalPagination = reactive(defaultPaging());
const weightPagination = reactive(defaultPaging());
const bonePagination = reactive(defaultPaging());

const visionFilter = reactive({ status: 'all', keyword: '' });
const oralFilter = reactive({ status: 'all', keyword: '' });
const mentalFilter = reactive({ status: 'all', keyword: '' });
const weightFilter = reactive({ status: 'all', keyword: '' });
const boneFilter = reactive({ status: 'all', keyword: '' });

// ================= 汇总 =================
const totalRecords = computed(() => visionStats.total + oralStats.total + mentalStats.total + weightStats.total + boneStats.total);
const totalPending = computed(() => visionStats.pending + oralStats.pending + mentalStats.pending + weightStats.pending + boneStats.pending);
const totalApproved = computed(() => visionStats.approved + oralStats.approved + mentalStats.approved + weightStats.approved + boneStats.approved);

// ================= 辅助 =================
const VISION_LEVELS = [{ value: '正常', label: '正常' }, { value: '轻度近视', label: '轻度近视' }, { value: '中度近视', label: '中度近视' }, { value: '高度近视', label: '高度近视' }];
const ORAL_STAGES = [{ value: '乳牙期', label: '乳牙期' }, { value: '替牙期', label: '替牙期' }, { value: '恒牙期', label: '恒牙期' }];
const ORAL_JAW = [{ value: '正常', label: '正常' }, { value: '地包天', label: '地包天' }, { value: '龅牙', label: '龅牙' }, { value: '其他', label: '其他' }];
const MENTAL_LEVELS = [{ value: '正常', label: '正常' }, { value: '轻度', label: '轻度' }, { value: '中度', label: '中度' }, { value: '重度', label: '重度' }];
const WEIGHT_LEVELS = [{ value: '偏瘦', label: '偏瘦' }, { value: '正常', label: '正常' }, { value: '超重', label: '超重' }, { value: '肥胖', label: '肥胖' }];
const BONE_LEVELS = [{ value: '正常', label: '正常' }, { value: '偏低', label: '偏低' }, { value: '骨质疏松', label: '骨质疏松' }];
const VITAMIN_LEVELS = [{ value: '充足', label: '充足' }, { value: '良好', label: '良好' }, { value: '不足', label: '不足' }];
const CALCIUM_LEVELS = [{ value: '正常', label: '正常' }, { value: '偏低', label: '偏低' }];

const getLevelClass = (l) => ({ '正常': 'level-normal', '轻度近视': 'level-low', '中度近视': 'level-mid', '高度近视': 'level-high' })[l] || '';
const getWeightLevelClass = (l) => ({ '偏瘦': 'level-low', '正常': 'level-normal', '超重': 'level-mid', '肥胖': 'level-high' })[l] || '';
const getBoneLevelClass = (l) => ({ '正常': 'level-normal', '偏低': 'level-low', '骨质疏松': 'level-mid' })[l] || '';
const getVitaminClass = (l) => ({ '充足': 'status-approved', '良好': 'status-approved', '不足': 'status-abnormal' })[l] || '';
const getCalciumClass = (l) => ({ '正常': 'status-approved', '偏低': 'status-abnormal' })[l] || '';
const getStatusClass = (s) => ({ pending: 'status-pending', approved: 'status-approved', abnormal: 'status-abnormal', spot: 'status-spot' })[s] || '';
const getStatusText = (s) => ({ pending: '待审核', approved: '已审核', abnormal: '异常', spot: '待抽检' })[s] || s;
const dimensionLabel = (d) => ({ vision: '视力健康', oral: '口腔健康', mental: '心理健康', weight: '健康体重', bone: '骨骼健康' })[d] || '';

// ================= 加载 =================
function fillPaging(p, data) {
  p.total = data.total; p.page = data.page; p.pageSize = data.pageSize;
  p.pageCount = Math.max(1, Math.ceil(data.total / data.pageSize));
}
async function loadVisionData(page = 1) {
  loadingVision.value = true;
  try {
    const data = await healthArchiveApi.vision.getPage({ page, pageSize: 10, status: visionFilter.status, keyword: visionFilter.keyword });
    visionData.value = data.items;
    fillPaging(visionPagination, data);
    Object.assign(visionStats, await healthArchiveApi.vision.getStats());
  } catch (e) { message.error('加载视力数据失败: ' + e.message); }
  finally { loadingVision.value = false; }
}
async function loadOralData(page = 1) {
  loadingOral.value = true;
  try {
    const data = await healthArchiveApi.oral.getPage({ page, pageSize: 10, status: oralFilter.status, keyword: oralFilter.keyword });
    oralData.value = data.items;
    fillPaging(oralPagination, data);
    Object.assign(oralStats, await healthArchiveApi.oral.getStats());
  } catch (e) { message.error('加载口腔数据失败: ' + e.message); }
  finally { loadingOral.value = false; }
}
async function loadMentalData(page = 1) {
  loadingMental.value = true;
  try {
    const data = await healthArchiveApi.mental.getPage({ page, pageSize: 10, status: mentalFilter.status, keyword: mentalFilter.keyword });
    mentalData.value = data.items;
    fillPaging(mentalPagination, data);
    Object.assign(mentalStats, await healthArchiveApi.mental.getStats());
  } catch (e) { message.error('加载心理数据失败: ' + e.message); }
  finally { loadingMental.value = false; }
}
async function loadWeightData(page = 1) {
  loadingWeight.value = true;
  try {
    const data = await healthArchiveApi.weight.getPage({ page, pageSize: 10, status: weightFilter.status, keyword: weightFilter.keyword });
    weightData.value = data.items;
    fillPaging(weightPagination, data);
    Object.assign(weightStats, await healthArchiveApi.weight.getStats());
  } catch (e) { message.error('加载体重数据失败: ' + e.message); }
  finally { loadingWeight.value = false; }
}
async function loadBoneData(page = 1) {
  loadingBone.value = true;
  try {
    const data = await healthArchiveApi.bone.getPage({ page, pageSize: 10, status: boneFilter.status, keyword: boneFilter.keyword });
    boneData.value = data.items;
    fillPaging(bonePagination, data);
    Object.assign(boneStats, await healthArchiveApi.bone.getStats());
  } catch (e) { message.error('加载骨骼数据失败: ' + e.message); }
  finally { loadingBone.value = false; }
}
const loaders = { vision: loadVisionData, oral: loadOralData, mental: loadMentalData, weight: loadWeightData, bone: loadBoneData };
async function reloadAllTabs() { await Promise.all([loadVisionData(), loadOralData(), loadMentalData(), loadWeightData(), loadBoneData()]); }

watch(activeTab, async (k) => {
  if (k === 'vision' && visionData.value.length === 0) await loadVisionData(1);
  if (k === 'oral' && oralData.value.length === 0) await loadOralData(1);
  if (k === 'mental' && mentalData.value.length === 0) await loadMentalData(1);
  if (k === 'weight' && weightData.value.length === 0) await loadWeightData(1);
  if (k === 'bone' && boneData.value.length === 0) await loadBoneData(1);
});

// ================= 状态变更 =================
function getCurrentUser() {
  try {
    const u = JSON.parse(localStorage.getItem('userInfo') || 'null');
    if (u?.realName) return { id: u.id || 0, name: u.realName };
    if (u?.username) return { id: u.id || 0, name: u.username };
  } catch {}
  return { id: 0, name: '审核员' };
}
async function handleApprove(dim, item) {
  Modal.confirm({ title: '审核通过', content: `确定要审核通过 ${item.studentName} 的数据吗？`,
    onOk: async () => {
      try {
        const me = getCurrentUser();
        await healthArchiveApi[dim].changeStatus(item.id, { status: 'approved', reviewerId: me.id, reviewerName: me.name, reviewRemark: '审核通过' });
        message.success(`已审核通过 ${item.studentName} 的数据`);
        await loaders[dim]();
      } catch (e) { message.error('操作失败: ' + e.message); }
    } });
}
async function handleReject(dim, item) {
  Modal.confirm({ title: '驳回数据', content: `确定要驳回 ${item.studentName} 的数据吗？驳回后状态会变为待重新审核。`,
    onOk: async () => {
      try {
        const me = getCurrentUser();
        await healthArchiveApi[dim].changeStatus(item.id, { status: 'pending', reviewerId: me.id, reviewerName: me.name, reviewRemark: '数据异常，驳回重新审核' });
        message.warning(`已驳回 ${item.studentName} 的数据`);
        await loaders[dim]();
      } catch (e) { message.error('操作失败: ' + e.message); }
    } });
}
async function handleSpotCheck(dim, item) {
  Modal.confirm({ title: '标记为待抽检', content: `将 ${item.studentName} 的数据标记为"待抽检"，提交区县教育局抽检。`,
    onOk: async () => {
      try {
        const me = getCurrentUser();
        await healthArchiveApi[dim].changeStatus(item.id, { status: 'spot', reviewerId: me.id, reviewerName: me.name, reviewRemark: '已提交区县教育局抽检' });
        message.success(`已将 ${item.studentName} 标记为待抽检`);
        await loaders[dim]();
      } catch (e) { message.error('操作失败: ' + e.message); }
    } });
}
async function handleSpotFinish(dim, item, finalStatus) {
  const isPass = finalStatus === 'approved';
  Modal.confirm({
    title: isPass ? '完成抽检（通过）' : '抽检不通过',
    content: isPass
      ? `确认 ${item.studentName} 的数据抽检通过，状态变更为"已审核"。`
      : `确认 ${item.studentName} 的数据抽检不通过，状态变更为"异常"，需要重新处理。`,
    okText: '确定', cancelText: '取消', okType: isPass ? 'primary' : 'danger',
    onOk: async () => {
      try {
        const me = getCurrentUser();
        await healthArchiveApi[dim].changeStatus(item.id, {
          status: finalStatus,
          reviewerId: me.id,
          reviewerName: me.name,
          reviewRemark: isPass ? '抽检通过' : '抽检不通过',
        });
        message.success(isPass ? `已标记为抽检通过` : `已标记为异常`);
        await loaders[dim]();
      } catch (e) { message.error('操作失败: ' + e.message); }
    },
  });
}
async function handleDelete(dim, item) {
  Modal.confirm({ title: '删除数据', content: `确定要删除 ${item.studentName} 的数据吗？此操作不可恢复。`, okText: '确定删除', cancelText: '取消', okType: 'danger',
    onOk: async () => {
      try { await healthArchiveApi[dim].remove(item.id); message.success('删除成功'); await loaders[dim](); }
      catch (e) {
        // 兜底：后端挂掉时 axios 抛 Network Error，e.message 是空，要从 response.data.message 取
        const errMsg = e?.response?.data?.message || e?.response?.data?.error || e?.message || '网络异常，请确认后端服务是否启动';
        message.error('删除失败: ' + errMsg);
      }
    } });
}
function handleCheckData(dim) {
  const stats = { vision: visionStats, oral: oralStats, mental: mentalStats, weight: weightStats, bone: boneStats }[dim];
  message.success(`数据校验完成！发现 ${stats.abnormal} 条异常数据，${stats.pending} 条待审核数据`);
}

// ================= 单条录入 / 编辑 弹窗 =================
const entryModalOpen = ref(false);
const entryDimension = ref('vision');
const entryDimensionLabel = computed(() => dimensionLabel(entryDimension.value));
const entryFormMode = ref('create');
const entrySubmitting = ref(false);
const entryForm = reactive({});
let editingId = null;

// ================= 学生电子档案检索（学号/姓名 联动下拉） =================
const studentOptions = ref([]);   // AAutoComplete 的 options
const studentCache = new Map();   // studentNo -> StudentDto（命中后查学号用）
let studentSearchToken = 0;       // 防抖：用最新 token 覆盖旧请求
let studentSearchTimer = null;

const studentNoStatus = ref('');      // '' | 'error' | 'warning' | 'success'
const studentNoHelp = ref('');
const studentNameStatus = ref('');
const studentNameHelp = ref('');

function resetStudentValidation() {
  studentNoStatus.value = '';
  studentNoHelp.value = '';
  studentNameStatus.value = '';
  studentNameHelp.value = '';
}

async function onStudentSearch(text) {
  // 防抖 250ms
  if (studentSearchTimer) clearTimeout(studentSearchTimer);
  if (!text || !text.trim()) {
    studentOptions.value = [];
    return;
  }
  const myToken = ++studentSearchToken;
  studentSearchTimer = setTimeout(async () => {
    try {
      const r = await studentApi.getPagedList({ keyword: text.trim(), page: 1, pageSize: 20 });
      if (myToken !== studentSearchToken) return;  // 旧请求，丢弃
      const list = (r?.items || []).map((s) => {
        studentCache.set(s.studentNo, s);
        return {
          // 关键：AAutoComplete 的 value 必须出现在 options 里；
          // 我们把 value 设为学号或姓名（取决于当前在哪个输入框），label 是显示文本
          value: s.studentNo,
          label: `${s.studentNo} | ${s.name} | ${s.gradeYear || ''} ${s.className || ''}`.trim(),
          // 自定义字段：模板里用
          studentNo: s.studentNo,
          name: s.name,
          grade: s.gradeYear || '',
          className: s.className || '',
          id: s.id,
        };
      });
      studentOptions.value = list;
    } catch (e) {
      // 静默失败：检索失败不打扰用户
      studentOptions.value = [];
    }
  }, 250);
}

function onStudentSelect(val, opt, fromField) {
  // 从下拉选中：自动填学号/姓名/年级/班级
  // opt 的字段是 AAutoComplete 透传的（看 templates 里的 :value=val 也得看实际传）
  // AAutoComplete 的 select 回调：第一个参数是选中的 value（字符串），第二个是 option 对象
  // 我们从 studentCache 里查
  const cached = studentCache.get(val) || studentCache.get(opt?.studentNo);
  if (!cached) return;
  entryForm.studentNo = cached.studentNo;
  entryForm.studentName = cached.name;
  if (cached.gradeYear) entryForm.grade = cached.gradeYear;
  if (cached.className) entryForm.className = cached.className;
  entryForm.studentId = cached.id;
  // 验证通过
  studentNoStatus.value = 'success';
  studentNoHelp.value = '';
  studentNameStatus.value = 'success';
  studentNameHelp.value = '';
}

async function onStudentNoBlur() {
  const no = (entryForm.studentNo || '').trim();
  if (!no) { studentNoStatus.value = ''; studentNoHelp.value = ''; return; }
  // 编辑模式：学号没变，跳过
  if (entryFormMode.value === 'edit' && no === entryForm._origStudentNo) {
    studentNoStatus.value = 'success';
    studentNoHelp.value = '';
    return;
  }
  try {
    const s = await studentApi.getByNo(no);
    if (s && s.studentNo) {
      studentCache.set(s.studentNo, s);
      // 自动同步：学号匹配上 → 同步姓名/年级/班级
      entryForm.studentId = s.id;
      if (s.name && !entryForm.studentName) entryForm.studentName = s.name;
      if (s.gradeYear) entryForm.grade = s.gradeYear;
      if (s.className) entryForm.className = s.className;
      // 如果姓名已经填了，但跟档案对不上，提示
      if (entryForm.studentName && entryForm.studentName !== s.name) {
        studentNameStatus.value = 'warning';
        studentNameHelp.value = `档案中姓名为「${s.name}」，请确认是否一致`;
      } else {
        studentNameStatus.value = 'success';
        studentNameHelp.value = '';
      }
      studentNoStatus.value = 'success';
      studentNoHelp.value = '';
    }
  } catch (e) {
    // 404 或异常：学号不在档案中
    studentNoStatus.value = 'error';
    studentNoHelp.value = '⚠ 学号不在学生电子档案中，请先维护学生电子档案';
  }
}

function onStudentNameBlur() {
  // 姓名框失焦不主动校验（用户可能临时只填姓名）—— 提交时再校验
}

function openEntryModal(dim) {
  entryDimension.value = dim;
  entryFormMode.value = 'create';
  editingId = null;
  resetStudentValidation();
  studentOptions.value = [];
  const me = getCurrentUser();
  Object.assign(entryForm, {
    id: 0, studentId: 0, studentNo: '', studentName: '',
    grade: '七年级', className: '七年级五班', checkDate: new Date().toISOString().slice(0, 10),
    recorderId: me.id, recorderName: me.name, recordTime: '', source: 'manual', status: 'pending', remark: '',
    leftEye: '', rightEye: '', visionLevel: '正常',
    decayedBabyTeeth: 0, decayedPermanentTeeth: 0, toothStage: '替牙期', jawDevelopment: '正常',
    anxietyScore: 0, depressionScore: 0, learningAnxiety: '正常', interpersonalSensitivity: '正常',
    height: '', weight: '', bmi: '', bmiLevel: '正常', waistCircumference: '', hipCircumference: '', whr: '',
    boneDensity: '', boneLevel: '正常', boneAge: '', vitaminD: '充足', calciumLevel: '正常',
  });
  entryModalOpen.value = true;
}
function openEditModal(dim, item) {
  entryDimension.value = dim;
  entryFormMode.value = 'edit';
  editingId = item.id;
  Object.assign(entryForm, item, { _origStudentNo: item.studentNo });
  resetStudentValidation();
  studentOptions.value = [];
  entryModalOpen.value = true;
}
function resetEntryForm() { editingId = null; entryFormMode.value = 'create'; }

function buildPayload() {
  const me = getCurrentUser();
  const common = {
    id: editingId || 0,
    studentId: entryForm.studentId || 0,
    studentNo: entryForm.studentNo || '',
    studentName: entryForm.studentName || '',
    grade: entryForm.grade || '',
    className: entryForm.className || '',
    checkDate: entryForm.checkDate || new Date().toISOString().slice(0, 10),
    recorderId: me.id || 0,
    recorderName: me.name || '',
    recordTime: entryForm.recordTime || new Date().toISOString().slice(0, 19).replace('T', ' '),
    source: 'manual',
    status: entryForm.status || 'pending',
    remark: entryForm.remark || '',
  };
  if (entryDimension.value === 'vision') return { ...common, leftEye: entryForm.leftEye || '', rightEye: entryForm.rightEye || '', visionLevel: entryForm.visionLevel || '正常' };
  if (entryDimension.value === 'oral') return { ...common, decayedBabyTeeth: +entryForm.decayedBabyTeeth || 0, decayedPermanentTeeth: +entryForm.decayedPermanentTeeth || 0, toothStage: entryForm.toothStage || '', jawDevelopment: entryForm.jawDevelopment || '正常' };
  if (entryDimension.value === 'mental') return { ...common, anxietyScore: +entryForm.anxietyScore || 0, depressionScore: +entryForm.depressionScore || 0, learningAnxiety: entryForm.learningAnxiety || '正常', interpersonalSensitivity: entryForm.interpersonalSensitivity || '正常' };
  if (entryDimension.value === 'weight') return { ...common, height: String(entryForm.height || ''), weight: String(entryForm.weight || ''), bmi: String(entryForm.bmi || ''), bmiLevel: entryForm.bmiLevel || '正常', waistCircumference: String(entryForm.waistCircumference || ''), hipCircumference: String(entryForm.hipCircumference || ''), whr: String(entryForm.whr || '') };
  if (entryDimension.value === 'bone') return { ...common, boneDensity: String(entryForm.boneDensity || ''), boneLevel: entryForm.boneLevel || '正常', boneAge: String(entryForm.boneAge || ''), vitaminD: entryForm.vitaminD || '充足', calciumLevel: entryForm.calciumLevel || '正常' };
  return common;
}

async function submitEntry() {
  if (!entryForm.studentNo) { message.error('请填写学号'); return; }
  if (!entryForm.studentName) { message.error('请填写姓名'); return; }
  if (!entryForm.checkDate) { message.error('请填写检查日期'); return; }
  // 强校验：学号必须在学生电子档案中、姓名必须与档案一致
  try {
    const s = await studentApi.getByNo(entryForm.studentNo.trim());
    if (!s || !s.studentNo) {
      message.error('学号「' + entryForm.studentNo + '」不在学生电子档案中，请先维护学生电子档案');
      studentNoStatus.value = 'error';
      studentNoHelp.value = '⚠ 学号不在学生电子档案中，请先维护学生电子档案';
      return;
    }
    if (s.name && s.name.trim() && s.name.trim() !== entryForm.studentName.trim()) {
      message.error(`学号「${s.studentNo}」在学生电子档案中对应姓名为「${s.name}」，与录入姓名「${entryForm.studentName}」不一致`);
      studentNameStatus.value = 'error';
      studentNameHelp.value = `档案姓名为「${s.name}」，与录入不一致`;
      return;
    }
    // 把档案的 studentId 同步到表单
    entryForm.studentId = s.id;
  } catch (e) {
    message.error('校验学号失败：' + (e?.message || '请稍后再试'));
    return;
  }
  entrySubmitting.value = true;
  try {
    const payload = buildPayload();
    const api = healthArchiveApi[entryDimension.value];
    if (entryFormMode.value === 'edit') {
      await api.update(editingId, payload);
      message.success('更新成功');
    } else {
      await api.create(payload);
      message.success('录入成功');
    }
    entryModalOpen.value = false;
    await loaders[entryDimension.value]();
  } catch (e) { message.error('保存失败: ' + e.message); }
  finally { entrySubmitting.value = false; }
}

// ================= Excel 导入 弹窗 =================
const excelModalOpen = ref(false);
const excelSubmitting = ref(false);
const excelRows = ref([]);
const excelPreviewColumns = computed(() => {
  const validCol = {
    title: '校验',
    dataIndex: '_valid',
    width: 220,
    customRender: ({ record }) => record._valid
      ? h('span', { style: 'color:#52c41a' }, '✓ 通过')
      : h('span', { style: 'color:#ff4d4f' }, record._reason || '未通过'),
  };
  const d = entryDimension.value;
  if (d === 'vision') return [validCol, { title: '学号', dataIndex: 'studentNo' }, { title: '姓名', dataIndex: 'studentName' }, { title: '左眼', dataIndex: 'leftEye' }, { title: '右眼', dataIndex: 'rightEye' }, { title: '视力等级', dataIndex: 'visionLevel' }];
  if (d === 'oral') return [validCol, { title: '学号', dataIndex: 'studentNo' }, { title: '姓名', dataIndex: 'studentName' }, { title: '乳牙龋', dataIndex: 'decayedBabyTeeth' }, { title: '恒牙龋', dataIndex: 'decayedPermanentTeeth' }, { title: '替牙情况', dataIndex: 'toothStage' }, { title: '颌面发育', dataIndex: 'jawDevelopment' }];
  if (d === 'mental') return [validCol, { title: '学号', dataIndex: 'studentNo' }, { title: '姓名', dataIndex: 'studentName' }, { title: '焦虑', dataIndex: 'anxietyScore' }, { title: '抑郁', dataIndex: 'depressionScore' }, { title: '学习焦虑', dataIndex: 'learningAnxiety' }, { title: '人际敏感', dataIndex: 'interpersonalSensitivity' }];
  if (d === 'weight') return [validCol, { title: '学号', dataIndex: 'studentNo' }, { title: '姓名', dataIndex: 'studentName' }, { title: '身高', dataIndex: 'height' }, { title: '体重', dataIndex: 'weight' }, { title: 'BMI', dataIndex: 'bmi' }, { title: 'BMI等级', dataIndex: 'bmiLevel' }];
  if (d === 'bone') return [validCol, { title: '学号', dataIndex: 'studentNo' }, { title: '姓名', dataIndex: 'studentName' }, { title: '骨密度', dataIndex: 'boneDensity' }, { title: '骨密度等级', dataIndex: 'boneLevel' }, { title: '骨龄', dataIndex: 'boneAge' }, { title: '维D', dataIndex: 'vitaminD' }, { title: '钙水平', dataIndex: 'calciumLevel' }];
  return [validCol];
});

const excelHeaderTip = computed(() => {
  const d = entryDimension.value;
  if (d === 'vision') return '学号 | 姓名 | 年级 | 班级 | 检查日期(YYYY-MM-DD) | 左眼 | 右眼 | 视力等级';
  if (d === 'oral') return '学号 | 姓名 | 年级 | 班级 | 检查日期 | 乳牙龋 | 恒牙龋 | 替牙情况 | 颌面发育';
  if (d === 'mental') return '学号 | 姓名 | 年级 | 班级 | 检查日期 | 焦虑评分 | 抑郁评分 | 学习焦虑 | 人际敏感';
  if (d === 'weight') return '学号 | 姓名 | 年级 | 班级 | 检查日期 | 身高 | 体重 | BMI | BMI等级 | 腰围 | 臀围 | 腰臀比';
  if (d === 'bone') return '学号 | 姓名 | 年级 | 班级 | 检查日期 | 骨密度 | 骨密度等级 | 骨龄 | 维生素D | 钙水平';
  return '';
});

function openExcelModal(dim) {
  entryDimension.value = dim;
  excelRows.value = [];
  _studentArchiveCache = null;   // 重置缓存：每次打开重新拉
  excelModalOpen.value = true;
}
function onExcelCancel() {
  excelRows.value = [];
  _studentArchiveCache = null;
}

async function handleExcelFile(file) {
  try {
    const buffer = await file.arrayBuffer();
    const wb = XLSX.read(buffer, { type: 'array' });
    const ws = wb.Sheets[wb.SheetNames[0]];
    const rawRows = XLSX.utils.sheet_to_json(ws, { defval: '' });
    const rows = rawRows.map((r) => normalizeExcelRow(r, entryDimension.value));
    // 拉一次全量学生档案做大表校验（pageSize=1000 一般够用；超过会循环拉）
    const archiveMap = await loadAllStudentsForValidation();
    for (const row of rows) {
      const no = (row.studentNo || '').trim();
      if (!no) {
        row._valid = false;
        row._reason = '学号为空';
        continue;
      }
      const archived = archiveMap.get(no);
      if (!archived) {
        row._valid = false;
        row._reason = '学号不在学生电子档案中';
        row._reasonTip = '请先在【学生电子档案】中维护该学号';
        continue;
      }
      if (archived.name && row.studentName && archived.name.trim() !== row.studentName.trim()) {
        row._valid = false;
        row._reason = `姓名不匹配（档案中为「${archived.name}」）`;
        continue;
      }
      // 校验通过：同步 studentId / grade / className
      row.studentId = archived.id;
      if (archived.gradeYear) row.grade = archived.gradeYear;
      if (archived.className) row.className = archived.className;
      row._valid = true;
      row._reason = '';
    }
    excelRows.value = rows;
    const validCount = rows.filter((r) => r._valid).length;
    const invalidCount = rows.length - validCount;
    if (invalidCount > 0) {
      message.warning(`解析成功 ${rows.length} 条，其中 ${validCount} 条通过校验，${invalidCount} 条不通过（请查看下方表格）`);
    } else {
      message.success(`解析成功 ${rows.length} 条，全部通过校验`);
    }
  } catch (e) {
    message.error('Excel 解析失败: ' + e.message);
  }
  return false;
}

let _studentArchiveCache = null;
async function loadAllStudentsForValidation() {
  // 简单缓存：同一维度一次导入只拉一次
  if (_studentArchiveCache) return _studentArchiveCache;
  const map = new Map();   // studentNo -> StudentDto
  let page = 1;
  const pageSize = 1000;
  while (true) {
    const r = await studentApi.getPagedList({ page, pageSize, keyword: '' });
    const items = r?.items || [];
    for (const s of items) map.set(s.studentNo, s);
    if (items.length < pageSize) break;
    page++;
    if (page > 50) break;   // 安全保护
  }
  _studentArchiveCache = map;
  return map;
}

function normalizeExcelRow(r, dim) {
  const get = (keys) => {
    for (const k of keys) {
      for (const realKey of Object.keys(r)) {
        if (realKey.toLowerCase().trim() === k) return r[realKey];
      }
    }
    return '';
  };
  const base = {
    id: 0, studentId: 0,
    studentNo: String(get(['学号', 'studentno', 'student no'])),
    studentName: String(get(['姓名', 'name', 'studentname'])),
    grade: String(get(['年级', 'grade'])),
    className: String(get(['班级', 'class', 'classname'])),
    checkDate: String(get(['检查日期', 'checkdate', 'check date'])),
    source: 'excel', status: 'pending',
    recorderId: 0, recorderName: 'Excel导入',
  };
  if (dim === 'vision') return { ...base, leftEye: String(get(['左眼', 'lefteye', 'left eye'])), rightEye: String(get(['右眼', 'righteye', 'right eye'])), visionLevel: String(get(['视力等级', 'visionlevel', 'level'])) };
  if (dim === 'oral') return { ...base, decayedBabyTeeth: +get(['乳牙龋', 'decayedbabyteeth']) || 0, decayedPermanentTeeth: +get(['恒牙龋', 'decayedpermanentteeth']) || 0, toothStage: String(get(['替牙情况', 'toothstage'])), jawDevelopment: String(get(['颌面发育', 'jawdevelopment'])) };
  if (dim === 'mental') return { ...base, anxietyScore: +get(['焦虑评分', 'anxietyscore']) || 0, depressionScore: +get(['抑郁评分', 'depressionscore']) || 0, learningAnxiety: String(get(['学习焦虑', 'learninganxiety'])), interpersonalSensitivity: String(get(['人际敏感', 'interpersonalsensitivity'])) };
  if (dim === 'weight') return { ...base, height: String(get(['身高', 'height'])), weight: String(get(['体重', 'weight'])), bmi: String(get(['bmi'])), bmiLevel: String(get(['bmi等级', 'bmilevel'])), waistCircumference: String(get(['腰围', 'waist'])), hipCircumference: String(get(['臀围', 'hip'])), whr: String(get(['腰臀比', 'whr'])) };
  if (dim === 'bone') return { ...base, boneDensity: String(get(['骨密度', 'bonedensity'])), boneLevel: String(get(['骨密度等级', 'bonelevel'])), boneAge: String(get(['骨龄', 'boneage'])), vitaminD: String(get(['维生素d', 'vitamind'])), calciumLevel: String(get(['钙水平', 'calciumlevel'])) };
  return base;
}

async function submitExcel() {
  if (excelRows.value.length === 0) { message.error('请先选择 Excel 文件'); return; }
  const validRows = excelRows.value.filter((r) => r._valid);
  const invalidRows = excelRows.value.filter((r) => !r._valid);
  if (validRows.length === 0) {
    message.error('没有可导入的合法数据，请先修正 Excel 中的学号/姓名问题');
    return;
  }
  if (invalidRows.length > 0) {
    Modal.confirm({
      title: '存在未通过校验的数据',
      content: `共 ${excelRows.value.length} 条，其中 ${validRows.length} 条通过校验、${invalidRows.length} 条未通过（${invalidRows.map((r) => r._reason).filter((v, i, a) => a.indexOf(v) === i).slice(0, 3).join('；')}${invalidRows.length > 3 ? '…' : ''}）。\n\n是否仅导入通过校验的 ${validRows.length} 条？`,
      okText: `导入 ${validRows.length} 条`,
      cancelText: '取消',
      onOk: async () => { await doImport(validRows); },
    });
    return;
  }
  await doImport(validRows);
}

async function doImport(rows) {
  excelSubmitting.value = true;
  let success = 0, failed = 0;
  const errors = [];
  try {
    const api = healthArchiveApi[entryDimension.value];
    for (const row of rows) {
      try { await api.create(row); success++; } catch (e) { failed++; errors.push(`${row.studentNo || '?'}: ${e?.message || '提交失败'}`); }
    }
    if (failed === 0) {
      message.success(`导入完成：成功 ${success} 条`);
    } else {
      message.warning(`导入完成：成功 ${success} 条，失败 ${failed} 条`);
      console.warn('[excel import] errors:', errors);
    }
    excelModalOpen.value = false;
    excelRows.value = [];
    await loaders[entryDimension.value]();
  } finally { excelSubmitting.value = false; }
}

// ================= 设备采集 弹窗 =================
const deviceModalOpen = ref(false);
const deviceSubmitting = ref(false);
const deviceForm = reactive({ deviceSn: '', collectedAt: '', recorderId: 0, recorderName: '', simulateCount: 3 });

function openDeviceModal(dim) {
  entryDimension.value = dim;
  const me = getCurrentUser();
  Object.assign(deviceForm, { deviceSn: `SN-${dim.toUpperCase()}-${Date.now().toString().slice(-6)}`, collectedAt: '', recorderId: me.id, recorderName: me.name, simulateCount: 3 });
  deviceModalOpen.value = true;
}

function buildSimulatedItems() {
  const n = deviceForm.simulateCount;
  const items = [];
  for (let i = 0; i < n; i++) {
    const idx = (i % 6) + 1;
    const studentNo = '2022' + (idx < 10 ? '00' + idx : '0' + idx);
    const studentName = ['', '陈熙航', '席振轩', '丁书婉', '周梓萌', '朱宇土', '招崧熙'][idx];
    const base = {
      id: 0, studentId: idx, studentNo, studentName,
      grade: '七年级', className: '七年级五班',
      checkDate: new Date().toISOString().slice(0, 10),
      recorderId: deviceForm.recorderId || 0, recorderName: deviceForm.recorderName || '设备采集',
      source: 'device', deviceSn: deviceForm.deviceSn, status: 'pending',
    };
    if (entryDimension.value === 'vision') items.push({ ...base, leftEye: (4.5 + Math.random() * 0.5).toFixed(1), rightEye: (4.5 + Math.random() * 0.5).toFixed(1), visionLevel: '正常' });
    if (entryDimension.value === 'oral') items.push({ ...base, decayedBabyTeeth: 0, decayedPermanentTeeth: 0, toothStage: '替牙期', jawDevelopment: '正常' });
    if (entryDimension.value === 'mental') items.push({ ...base, anxietyScore: 5 + Math.floor(Math.random() * 5), depressionScore: 3 + Math.floor(Math.random() * 5), learningAnxiety: '正常', interpersonalSensitivity: '正常' });
    if (entryDimension.value === 'weight') {
      const h = 130 + idx * 5; const w = 40 + idx * 3;
      const bmi = (w / Math.pow(h / 100, 2)).toFixed(1);
      items.push({ ...base, height: String(h), weight: String(w), bmi, bmiLevel: bmi < 18.5 ? '偏瘦' : bmi > 24 ? '超重' : '正常', waistCircumference: '65', hipCircumference: '85', whr: '0.76' });
    }
    if (entryDimension.value === 'bone') items.push({ ...base, boneDensity: (0.7 + Math.random() * 0.2).toFixed(2), boneLevel: '正常', boneAge: '12岁', vitaminD: '充足', calciumLevel: '正常' });
  }
  return items;
}

async function submitDevice() {
  deviceSubmitting.value = true;
  try {
    const items = buildSimulatedItems();
    const payload = { deviceSn: deviceForm.deviceSn, collectedAt: deviceForm.collectedAt || new Date().toISOString(), recorderId: deviceForm.recorderId, recorderName: deviceForm.recorderName, items };
    const apiMap = { vision: collectDeviceApi.vision, oral: collectDeviceApi.oral, mental: collectDeviceApi.mental, weight: collectDeviceApi.weight, bone: collectDeviceApi.bone };
    const resp = await apiMap[entryDimension.value](payload);
    message.success(`设备采集完成：成功 ${resp.success} 条，失败 ${resp.failed} 条`);
    deviceModalOpen.value = false;
    await loaders[entryDimension.value]();
  } catch (e) { message.error('设备采集失败: ' + e.message); }
  finally { deviceSubmitting.value = false; }
}

// ================= 生命周期 =================
onMounted(async () => {
  applySystemTheme();
  observeTheme();
  await reloadAllTabs();
});
onBeforeUnmount(() => { if (themeObserver) { themeObserver.disconnect(); themeObserver = null; } });
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
.dashboard-wrapper > * { position: relative; z-index: 2; }
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
.bg-decor { position: absolute; inset: 0; pointer-events: none; z-index: 0; overflow: hidden; }
.bg-grid {
  position: absolute; inset: 0;
  background-image: linear-gradient(rgba(0, 212, 255, 0.05) 1px, transparent 1px), linear-gradient(90deg, rgba(0, 212, 255, 0.05) 1px, transparent 1px);
  background-size: 60px 60px;
  mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  -webkit-mask-image: radial-gradient(ellipse at center, black 0%, transparent 80%);
  animation: grid-drift 30s linear infinite;
}
@keyframes grid-drift { 0% { background-position: 0 0; } 100% { background-position: 60px 60px; } }
.bg-glow { position: absolute; border-radius: 50%; filter: blur(120px); animation: glow-float 18s ease-in-out infinite; }
.bg-glow-1 { width: 700px; height: 700px; background: var(--primary); top: -200px; left: -150px; opacity: 0.18; }
.bg-glow-2 { width: 800px; height: 800px; background: var(--secondary); bottom: -250px; right: -150px; opacity: 0.12; animation-delay: -6s; }
@keyframes glow-float { 0%, 100% { transform: translate(0, 0) scale(1); } 33% { transform: translate(60px, -40px) scale(1.08); } 66% { transform: translate(-40px, 50px) scale(0.96); } }
.scan-line { position: absolute; left: 0; right: 0; top: 0; height: 1px; background: linear-gradient(90deg, transparent, var(--primary), transparent); animation: scan-move 10s linear infinite; z-index: 1; pointer-events: none; opacity: 0.5; }
@keyframes scan-move { 0% { transform: translateY(-100px); opacity: 0; } 10% { opacity: 0.5; } 90% { opacity: 0.5; } 100% { transform: translateY(100vh); opacity: 0; } }
.dashboard-wrapper ::-webkit-scrollbar { width: 8px; height: 8px; }
.dashboard-wrapper ::-webkit-scrollbar-track { background: transparent; }
.dashboard-wrapper ::-webkit-scrollbar-thumb { background: var(--scrollbar-thumb); border-radius: 4px; }
.dashboard-wrapper ::-webkit-scrollbar-thumb:hover { background: var(--border-strong); }
.dashboard-wrapper > * { max-width: 1600px; margin-left: auto; margin-right: auto; padding-left: 20px; padding-right: 20px; }

.page-header { margin-bottom: 12px; position: relative; padding: 10px 16px; background: linear-gradient(90deg, var(--primary-bg) 0%, transparent 70%); border: 1px solid var(--border-soft); border-radius: 8px; display: flex; align-items: center; gap: 14px; overflow: hidden; }
.page-header::before { content: ''; position: absolute; top: 0; left: 0; right: 0; height: 2px; background: linear-gradient(90deg, transparent, var(--primary), var(--secondary), var(--primary), transparent); border-radius: 8px 8px 0 0; opacity: 0.9; }
.page-header::after { content: ''; position: absolute; bottom: 0; left: 0; width: 120px; height: 2px; background: linear-gradient(90deg, var(--primary), var(--secondary), transparent); border-radius: 2px; }
.header-deco { display: flex; flex-direction: column; gap: 3px; flex-shrink: 0; }
.header-deco-l { align-items: flex-start; } .header-deco-r { align-items: flex-end; }
.hd-line { width: 30px; height: 2px; background: linear-gradient(90deg, var(--primary), transparent); position: relative; }
.header-deco-r .hd-line { background: linear-gradient(270deg, var(--primary), transparent); }
.hd-line::after { content: ''; position: absolute; right: 0; top: -2px; width: 2px; height: 5px; background: var(--primary); box-shadow: 0 0 6px var(--primary); }
.hd-dot { width: 5px; height: 5px; background: var(--primary); border-radius: 50%; box-shadow: 0 0 6px var(--primary); animation: dot-blink 2s ease-in-out infinite; }
.header-deco-r .hd-dot { animation-delay: -0.7s; }
@keyframes dot-blink { 0%, 100% { opacity: 1; transform: scale(1); } 50% { opacity: 0.3; transform: scale(0.6); } }
.header-main { display: flex; align-items: center; gap: 20px; flex: 1; }
.page-title { font-size: 20px; font-weight: 700; color: var(--text-strong); margin: 0; letter-spacing: 2px; position: relative; display: flex; align-items: center; gap: 4px; flex-shrink: 0; }
.title-bracket { color: var(--primary); font-weight: 400; opacity: 0.7; }
.title-text { background: linear-gradient(180deg, #f1f5f9 0%, var(--primary) 100%); -webkit-background-clip: text; background-clip: text; -webkit-text-fill-color: transparent; filter: drop-shadow(0 0 8px var(--glow)); }
.title-shine { position: absolute; top: 0; right: 0; width: 20px; height: 100%; background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent); animation: shine-sweep 3s ease-in-out infinite; }
@keyframes shine-sweep { 0% { transform: translateX(-100%); } 50% { transform: translateX(200%); } 100% { transform: translateX(200%); } }
.header-stats { display: flex; align-items: center; gap: 12px; }
.stat-item { display: flex; flex-direction: column; align-items: center; gap: 1px; }
.stat-label { font-size: 11px; color: var(--text-dim); letter-spacing: 0.5px; }
.stat-value { font-size: 18px; font-weight: 700; color: var(--primary); font-family: 'Orbitron', 'Consolas', monospace; text-shadow: 0 0 10px var(--glow); letter-spacing: 1px; }
.stat-value.stat-warn { color: var(--warning); text-shadow: 0 0 10px rgba(var(--warning), 0.4); }
.stat-value.stat-ok { color: var(--success); text-shadow: 0 0 10px rgba(var(--success), 0.4); }
.stat-divider { width: 1px; height: 24px; background: linear-gradient(180deg, transparent, var(--border-strong), transparent); }

.tab-bar { display: flex; gap: 6px; margin-bottom: 10px; padding: 6px; background: var(--bg-card); border: 1px solid var(--border); border-radius: 10px; box-shadow: var(--shadow); backdrop-filter: blur(10px); position: relative; }
.tab-bar::before { content: ''; position: absolute; top: 0; left: 16px; right: 16px; height: 1px; background: linear-gradient(90deg, transparent, var(--primary), transparent); opacity: 0.4; }
.tab-item { flex: 1; text-align: center; padding: 10px 16px; cursor: pointer; font-weight: 500; font-size: 14px; letter-spacing: 0.5px; border-radius: 8px; transition: all 0.25s ease; border: 1px solid transparent; background: transparent; color: var(--text-dim); position: relative; overflow: hidden; }
.tab-item:hover:not(.active) { color: var(--text); background: var(--bg-hover); border-color: var(--border-soft); }
.tab-item.active { background: linear-gradient(135deg, var(--primary-bg), rgba(124, 58, 237, 0.15)); color: var(--primary); border-color: var(--border-strong); box-shadow: 0 0 20px var(--glow), inset 0 0 0 1px rgba(56, 189, 248, 0.25); }
.tab-item.active::before { content: ''; position: absolute; top: 0; left: 0; right: 0; height: 2px; background: linear-gradient(90deg, var(--primary), var(--secondary)); }
.tab-item .tab-icon { font-size: 16px; } .tab-label { margin-left: 4px; }

.stats-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 12px; margin-bottom: 10px; }
.stat-card { background: var(--bg-card); border: 1px solid var(--border); border-radius: 10px; padding: 14px 16px; position: relative; overflow: hidden; transition: all 0.3s ease; box-shadow: var(--shadow); backdrop-filter: blur(10px); display: flex; align-items: center; gap: 12px; }
.stat-card:hover { transform: translateY(-2px); box-shadow: var(--shadow-card); border-color: var(--border-strong); }
.stat-card::before { content: ''; position: absolute; top: 0; left: 0; width: 3px; height: 100%; background: linear-gradient(180deg, var(--card-accent, var(--primary)), transparent); border-radius: 3px 0 0 3px; }
.stat-icon { width: 44px; height: 44px; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-size: 22px; flex-shrink: 0; }
.stat-icon.blue { background: linear-gradient(135deg, rgba(56, 189, 248, 0.2), rgba(56, 189, 248, 0.05)); color: #38bdf8; box-shadow: 0 0 15px rgba(56, 189, 248, 0.3); }
.stat-icon.green { background: linear-gradient(135deg, rgba(52, 211, 153, 0.2), rgba(52, 211, 153, 0.05)); color: #34d399; box-shadow: 0 0 15px rgba(52, 211, 153, 0.3); }
.stat-icon.orange { background: linear-gradient(135deg, rgba(251, 191, 36, 0.2), rgba(251, 191, 36, 0.05)); color: #fbbf24; box-shadow: 0 0 15px rgba(251, 191, 36, 0.3); }
.stat-icon.red { background: linear-gradient(135deg, rgba(251, 113, 133, 0.2), rgba(251, 113, 133, 0.05)); color: #fb7185; box-shadow: 0 0 15px rgba(251, 113, 133, 0.3); }
.stat-info { display: flex; flex-direction: column; gap: 2px; }
.stat-card .stat-label { font-size: 12px; color: var(--text-dim); letter-spacing: 0.5px; }
.stat-card .stat-value { font-size: 22px; font-weight: 700; color: var(--text-strong); font-family: 'Orbitron', 'Consolas', monospace; letter-spacing: 1px; text-shadow: none; }

.toolbar { display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 10px; align-items: center; }
.toolbar-left { display: flex; gap: 8px; flex-wrap: wrap; flex: 1; }
.toolbar-right { display: flex; gap: 8px; flex-wrap: wrap; }
.toolbar-tip { font-size: 12px; color: var(--text-dim); padding: 6px 12px; background: var(--primary-bg); border: 1px solid var(--border-soft); border-radius: 6px; letter-spacing: 0.3px; }
.btn { display: inline-flex; align-items: center; gap: 5px; padding: 6px 14px; background: var(--bg-card); border: 1px solid var(--border); color: var(--text); border-radius: 6px; cursor: pointer; font-size: 13px; font-weight: 500; letter-spacing: 0.3px; transition: all 0.2s ease; position: relative; overflow: hidden; font-family: inherit; }
.btn:hover { border-color: var(--border-strong); color: var(--primary); background: var(--bg-hover); box-shadow: 0 0 12px var(--glow); }
.btn .icon { font-size: 14px; }
.btn-primary { background: linear-gradient(135deg, var(--primary), #0284c7); border-color: var(--primary); color: #fff; }
.btn-primary:hover { box-shadow: 0 0 20px var(--glow); transform: translateY(-1px); color: #fff; }
.btn-success { background: linear-gradient(135deg, var(--success), #059669); border-color: var(--success); color: #fff; }
.btn-success:hover { box-shadow: 0 0 20px rgba(52, 211, 153, 0.5); color: #fff; }
.btn-info { background: linear-gradient(135deg, #a78bfa, #7c3aed); border-color: #a78bfa; color: #fff; }
.btn-info:hover { box-shadow: 0 0 20px rgba(167, 139, 250, 0.5); color: #fff; }
.btn-warning { background: linear-gradient(135deg, var(--warning), #d97706); border-color: var(--warning); color: #1f2937; }
.btn-warning:hover { box-shadow: 0 0 20px rgba(251, 191, 36, 0.5); color: #1f2937; }
.btn-danger { background: linear-gradient(135deg, var(--danger), #e11d48); border-color: var(--danger); color: #fff; }
.btn-sm { padding: 3px 10px; font-size: 12px; }
.btn-icon { display: inline-flex; align-items: center; justify-content: center; width: 28px; height: 28px; background: var(--bg-card); border: 1px solid var(--border); border-radius: 6px; cursor: pointer; font-size: 13px; transition: all 0.2s ease; color: var(--text); padding: 0; }
.btn-icon:hover { transform: translateY(-1px); box-shadow: 0 0 10px var(--glow); }
.btn-icon.approve:hover { border-color: var(--success); color: var(--success); box-shadow: 0 0 10px rgba(52, 211, 153, 0.4); }
.btn-icon.reject:hover { border-color: var(--danger); color: var(--danger); box-shadow: 0 0 10px rgba(251, 113, 133, 0.4); }
.btn-icon.spot:hover { border-color: var(--primary); color: var(--primary); box-shadow: 0 0 10px var(--glow); }
.btn-icon.edit:hover { border-color: var(--warning); color: var(--warning); box-shadow: 0 0 10px rgba(251, 191, 36, 0.4); }
.btn-icon.delete:hover { border-color: var(--danger); color: var(--danger); box-shadow: 0 0 10px rgba(251, 113, 133, 0.4); }
.btn:disabled, .page-btn:disabled { opacity: 0.4; cursor: not-allowed; }

.source-tag { display: inline-block; margin-left: 4px; font-size: 11px; }

.data-table-wrapper { background: var(--bg-card); border: 1px solid var(--border); border-radius: 10px; overflow: hidden; box-shadow: var(--shadow-card); backdrop-filter: blur(10px); margin-bottom: 12px; }
.table-header { display: flex; align-items: center; justify-content: space-between; padding: 12px 16px; border-bottom: 1px solid var(--border-soft); }
.table-header h3 { margin: 0; font-size: 15px; color: var(--text-strong); letter-spacing: 0.5px; }
.table-filter { display: flex; gap: 8px; align-items: center; }
.filter-select, .filter-input { background: var(--bg-soft); border: 1px solid var(--border-soft); color: var(--text); padding: 4px 10px; border-radius: 6px; font-size: 12px; font-family: inherit; outline: none; transition: all 0.2s ease; }
.filter-input { min-width: 160px; }
.filter-select:focus, .filter-input:focus { border-color: var(--border-strong); box-shadow: 0 0 10px var(--glow); }
.data-table { width: 100%; border-collapse: collapse; font-size: 13px; }
.data-table th, .data-table td { padding: 10px 12px; text-align: left; border-bottom: 1px solid var(--border-soft); }
.data-table th { background: var(--bg-soft); color: var(--text-dim); font-weight: 500; font-size: 12px; letter-spacing: 0.3px; }
.data-table tbody tr { transition: background 0.2s ease; }
.data-table tbody tr:hover { background: var(--bg-hover); }
.student-no { color: var(--primary); font-weight: 500; }
.level-tag { display: inline-block; padding: 2px 8px; border-radius: 10px; font-size: 11px; font-weight: 500; }
.level-tag.level-normal { background: rgba(52, 211, 153, 0.15); color: var(--success); border: 1px solid rgba(52, 211, 153, 0.3); }
.level-tag.level-low { background: rgba(251, 191, 36, 0.15); color: var(--warning); border: 1px solid rgba(251, 191, 36, 0.3); }
.level-tag.level-mid { background: rgba(251, 113, 133, 0.15); color: var(--danger); border: 1px solid rgba(251, 113, 133, 0.3); }
.level-tag.level-high { background: rgba(251, 113, 133, 0.3); color: #fff; border: 1px solid rgba(251, 113, 133, 0.5); }
.status-tag { display: inline-block; padding: 2px 8px; border-radius: 10px; font-size: 11px; font-weight: 500; }
.status-tag.status-pending { background: rgba(251, 191, 36, 0.15); color: var(--warning); border: 1px solid rgba(251, 191, 36, 0.3); }
.status-tag.status-approved { background: rgba(52, 211, 153, 0.15); color: var(--success); border: 1px solid rgba(52, 211, 153, 0.3); }
.status-tag.status-abnormal { background: rgba(251, 113, 133, 0.15); color: var(--danger); border: 1px solid rgba(251, 113, 133, 0.3); }
.status-tag.status-spot { background: var(--primary-bg); color: var(--primary); border: 1px solid var(--border-strong); }
.action-btns { display: flex; gap: 4px; }
.table-pagination { display: flex; align-items: center; justify-content: space-between; padding: 10px 16px; border-top: 1px solid var(--border-soft); }
.pagination-btns { display: flex; gap: 4px; }
.page-btn { padding: 4px 10px; background: var(--bg-soft); border: 1px solid var(--border-soft); color: var(--text-dim); border-radius: 4px; cursor: pointer; font-size: 12px; transition: all 0.2s ease; font-family: inherit; }
.page-btn:hover:not(.active):not(:disabled) { color: var(--primary); border-color: var(--border-strong); }
.page-btn.active { background: var(--primary); color: #fff; border-color: var(--primary); }
</style>
