<template>
  <div class="org-manage">
    <!-- 机构统计 -->
    <div class="org-stats">
      <div class="stat-item" v-for="stat in orgStatsData" :key="stat.label">
        <span class="stat-label">{{ stat.label }}</span>
        <span class="stat-value" :class="getStatClass(stat.label)">{{ stat.value }}</span>
      </div>
    </div>

    <!-- 搜索筛选栏 -->
    <div class="search-bar">
      <div class="search-item">
        <span class="search-label">部门</span>
        <input
          v-model="searchName"
          type="text"
          placeholder="请输入部门名称"
          class="search-input"
          @keyup.enter="handleSearch"
        />
      </div>
      <div class="search-item">
        <span class="search-label">状态</span>
        <select v-model="searchStatus" class="search-select">
          <option value="">部门状态</option>
          <option value="1">正常</option>
          <option value="0">停用</option>
        </select>
      </div>
      <div class="search-actions">
        <button class="btn btn-primary" @click="handleSearch">
          <span class="icon">🔍</span> 搜索
        </button>
        <button class="btn btn-default" @click="handleReset">
          <span class="icon">↻</span> 重置
        </button>
      </div>
    </div>

    <!-- 操作工具栏 -->
    <div class="toolbar">
      <button class="btn btn-success" @click="handleAddOrg">
        <span class="icon">➕</span> 新增
      </button>
      <button class="btn btn-danger" @click="handleBatchDelete">
        <span class="icon">🗑️</span> 删除
      </button>
      <div class="toolbar-right">
        <button class="btn-icon" @click="handleRefresh" title="刷新">
          <span>🔄</span>
        </button>
      </div>
    </div>

    <!-- 数据表格（树形） -->
    <div class="table-wrapper" v-loading="loading">
      <table class="data-table tree-table">
        <colgroup>
          <col class="col-check" />
          <col class="col-name" />
          <col class="col-sort" />
          <col class="col-status" />
          <col class="col-time" />
          <col class="col-action" />
        </colgroup>
        <thead>
          <tr>
            <th class="col-check">
              <label class="custom-checkbox-wrap" :title="isAllSelected ? '取消全选' : '全选'">
                <input type="checkbox" :checked="isAllSelected" :indeterminate="isIndeterminate" @change="toggleSelectAll" />
                <span class="custom-checkbox">
                  <svg v-if="isAllSelected" class="check-icon" viewBox="0 0 16 16">
                    <path d="M3.5 8.5l3 3 6-6" fill="none" stroke="#fff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                  <svg v-else-if="isIndeterminate" class="check-icon" viewBox="0 0 16 16">
                    <path d="M4 8h8" fill="none" stroke="#fff" stroke-width="2" stroke-linecap="round"/>
                  </svg>
                </span>
              </label>
            </th>
            <th class="col-name">
              <div class="col-name-header">
                <span>部门名称</span>
                <span class="expand-all-btn" @click="toggleExpandAll">
                  {{ expandedIds.size > 0 ? '收起全部' : '展开全部' }}
                </span>
              </div>
            </th>
            <th class="col-sort">排序</th>
            <th class="col-status">状态</th>
            <th class="col-time">创建时间</th>
            <th class="col-action">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(row, index) in tableRows"
            :key="row.org.id"
            class="tree-row"
            :class="`level-${row.depth}`"
            v-show="row.visible"
          >
            <td class="col-check">
              <label class="custom-checkbox-wrap">
                <input type="checkbox" :checked="selectedIds.includes(row.org.id)" @change="toggleSelect(row.org.id)" />
                <span class="custom-checkbox">
                  <svg v-if="selectedIds.includes(row.org.id)" class="check-icon" viewBox="0 0 16 16">
                    <path d="M3.5 8.5l3 3 6-6" fill="none" stroke="#fff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                </span>
              </label>
            </td>
            <td class="col-name">
              <div class="org-name-cell" :style="{ paddingLeft: row.depth * 28 + 'px' }">
                <button
                  v-if="row.hasChildren"
                  class="expand-icon"
                  :class="{ 'is-expanded': row.isExpanded }"
                  @click="toggleExpand(row.org.id)"
                >
                  <svg viewBox="0 0 16 16" class="expand-svg">
                    <path d="M6 4l4 4-4 4" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                </button>
                <span v-else class="expand-icon empty"></span>
                <span v-if="row.org.level" class="level-tag" :class="getLevelClass(row.org.level)">{{ row.org.level }}</span>
                <span class="org-name" :class="{ 'has-children': row.hasChildren }">{{ row.org.name }}</span>
              </div>
            </td>
            <td class="col-sort">{{ row.org.sort }}</td>
            <td class="col-status">
              <span class="status-tag" :style="{ color: row.org.statusColor }">● {{ row.org.statusText }}</span>
            </td>
            <td class="col-time">{{ formatTime(row.org.createdAt) }}</td>
            <td class="col-action">
              <div class="action-btns">
                <button class="action-btn edit" @click="handleEditOrg(row.org)">
                  <span>✏️</span> 修改
                </button>
                <span class="action-divider">|</span>
                <button class="action-btn add" @click="handleAddChild(row.org)">
                  <span>➕</span> 新增
                </button>
                <span class="action-divider">|</span>
                <button class="action-btn delete" @click="handleDeleteOrg(row.org)">
                  <span>🗑️</span> 删除
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="!tableRows || tableRows.length === 0">
            <td colspan="6" class="empty-row">
              <div class="empty-state">
                <span class="empty-icon">📋</span>
                <span>暂无部门数据</span>
                <span class="empty-hint">点击"新增"添加第一个部门</span>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- 底部信息 -->
      <div class="table-footer">
        <span class="total-text">
          筛选结果：<b>{{ filteredCount }}</b> 条 / 总数：<b>{{ totalOrgCount }}</b> 条
        </span>
      </div>
    </div>

    <!-- ===== 新增部门弹窗 ===== -->
    <Modal
      v-model:open="showAddModal"
      title="新增部门"
      :footer="null"
      width="520px"
      @cancel="closeAddModal"
    >
      <div class="modal-form">
        <div class="form-group">
          <label class="form-label required">部门名称</label>
          <input v-model="addForm.name" type="text" placeholder="请输入部门名称" class="form-input" />
        </div>
        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">部门级别</label>
            <select v-model="addForm.level" class="form-select" @change="onAddLevelChange">
              <option value="">请选择级别</option>
              <option value="国家级">国家级</option>
              <option value="省级">省级</option>
              <option value="市级">市级</option>
              <option value="区县级">区县级</option>
              <option value="学校级">学校级</option>
            </select>
          </div>
          <div class="form-group half">
            <label class="form-label">部门编码</label>
            <input v-model="addForm.code" type="text" placeholder="自动生成" class="form-input" disabled />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group half">
            <label class="form-label">上级部门</label>
            <TreeSelect
              v-model:value="addForm.parentId"
              :tree-data="treeSelectData"
              :placeholder="'无（顶级部门）'"
              :allow-clear="true"
              :tree-expand-all="true"
              :show-search="true"
              :tree-node-filter-prop="['title']"
              class="form-tree-select"
              @change="(val: any) => { addForm.parentId = val === undefined || val === null ? null : val }"
            />
          </div>
          <div class="form-group half">
            <label class="form-label required">排序</label>
            <input v-model.number="addForm.sort" type="number" placeholder="请输入排序号" class="form-input" />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">状态</label>
            <select v-model="addForm.status" class="form-select">
              <option :value="1">正常</option>
              <option :value="0">停用</option>
            </select>
          </div>
        </div>
        <div class="form-actions">
          <button class="btn btn-default" @click="closeAddModal">取消</button>
          <button class="btn btn-primary" @click="submitAddOrg" :disabled="submitting">
            {{ submitting ? '提交中...' : '确定' }}
          </button>
        </div>
      </div>
    </Modal>

    <!-- ===== 编辑部门弹窗 ===== -->
    <Modal
      v-model:open="showEditModal"
      title="编辑部门"
      :footer="null"
      width="520px"
      @cancel="closeEditModal"
    >
      <div class="modal-form">
        <div class="form-group">
          <label class="form-label">部门ID</label>
          <input v-model="editForm.id" type="text" class="form-input" disabled />
        </div>
        <div class="form-group">
          <label class="form-label required">部门名称</label>
          <input v-model="editForm.name" type="text" placeholder="请输入部门名称" class="form-input" />
        </div>
        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">部门级别</label>
            <select v-model="editForm.level" class="form-select">
              <option value="">请选择级别</option>
              <option value="国家级">国家级</option>
              <option value="省级">省级</option>
              <option value="市级">市级</option>
              <option value="区县级">区县级</option>
              <option value="学校级">学校级</option>
            </select>
          </div>
          <div class="form-group half">
            <label class="form-label">部门编码</label>
            <input v-model="editForm.code" type="text" class="form-input" disabled />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group half">
            <label class="form-label">上级部门</label>
            <TreeSelect
              v-model:value="editForm.parentId"
              :tree-data="treeSelectData"
              :placeholder="'无（顶级部门）'"
              :allow-clear="true"
              :tree-expand-all="true"
              :show-search="true"
              :tree-node-filter-prop="['title']"
              :disabled-keys="[editForm.id]"
              class="form-tree-select"
              @change="(val: any) => { editForm.parentId = val === undefined || val === null ? null : val }"
            />
          </div>
          <div class="form-group half">
            <label class="form-label required">排序</label>
            <input v-model.number="editForm.sort" type="number" placeholder="请输入排序号" class="form-input" />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">状态</label>
            <select v-model="editForm.status" class="form-select">
              <option :value="1">正常</option>
              <option :value="0">停用</option>
            </select>
          </div>
        </div>
        <div class="form-actions">
          <button class="btn btn-default" @click="closeEditModal">取消</button>
          <button class="btn btn-primary" @click="submitEditOrg" :disabled="submitting">
            {{ submitting ? '提交中...' : '确定' }}
          </button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { Modal, TreeSelect } from 'ant-design-vue';
import { useOrgManage } from './useOrgManage';

const {
  // 状态
  searchName, searchStatus, selectedIds, loading, submitting,
  apiConnected,
  // 树形
  expandedIds,
  // 弹窗
  showAddModal, showEditModal,
  // 数据与表单
  addForm, editForm,
  filteredTree, flatOrgList,
  tableRows, treeSelectData,
  totalCount, totalOrgCount, filteredCount,
  isAllSelected, isIndeterminate,
  orgStatsData,
  // 方法
  toggleExpand, isExpanded, toggleExpandAll,
  handleSearch, handleReset, handleAddOrg, submitAddOrg,
  handleEditOrg, submitEditOrg, handleDeleteOrg, handleBatchDelete,
  handleRefresh, toggleSelectAll, handleAddChild, onAddLevelChange,
  closeAddModal, closeEditModal,
} = useOrgManage();

// 格式化时间
const formatTime = (time?: string) => {
  if (!time) return '-';
  const date = new Date(time);
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} ${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
};

// 获取级别样式
const getLevelClass = (level: string) => {
  const map: Record<string, string> = {
    '国家级': 'level-national',
    '省级': 'level-province',
    '市级': 'level-city',
    '区县级': 'level-district',
    '学校级': 'level-school',
  };
  return map[level] || '';
};

// 获取统计样式
const getStatClass = (label: string) => {
  const map: Record<string, string> = {
    '机构总数': 'stat-total',
    '国家级': 'stat-national',
    '省级': 'stat-province',
    '市级': 'stat-city',
    '区县级': 'stat-district',
    '学校级': 'stat-school',
  };
  return map[label] || '';
};

// 切换选择
const toggleSelect = (id: number) => {
  const index = selectedIds.value.indexOf(id);
  if (index > -1) {
    selectedIds.value.splice(index, 1);
  } else {
    selectedIds.value.push(id);
  }
};
</script>

<style scoped>
.org-manage {
  width: 100%;
  padding: 0;
}

/* ===== 机构统计 ===== */
.org-stats {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  gap: 12px;
  margin-bottom: 12px;
  background: #fff;
  border-radius: 8px;
  padding: 16px 20px;
  border: 1px solid #ebeef5;
}

.stat-item {
  text-align: center;
  padding: 8px 0;
  border-right: 1px solid #f0f0f0;
}

.stat-item:last-child {
  border-right: none;
}

.stat-label {
  display: block;
  font-size: 12px;
  color: #909399;
  margin-bottom: 6px;
}

.stat-value {
  font-size: 24px;
  font-weight: 700;
  color: #303133;
}

.stat-total { color: #409eff; }
.stat-national { color: #f56c6c; }
.stat-province { color: #e6a23c; }
.stat-city { color: #409eff; }
.stat-district { color: #67c23a; }
.stat-school { color: #9b59b6; }

/* ===== 搜索筛选栏 ===== */
.search-bar {
  background: #fff;
  border-radius: 8px;
  padding: 16px 20px;
  margin-bottom: 12px;
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
  border: 1px solid #ebeef5;
}

.search-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.search-label {
  font-size: 14px;
  color: #606266;
  white-space: nowrap;
  min-width: 36px;
}

.search-input {
  padding: 7px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 14px;
  outline: none;
  width: 200px;
  transition: border-color 0.3s;
}

.search-input:focus {
  border-color: #409eff;
  box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.1);
}

.search-select {
  padding: 7px 32px 7px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 14px;
  outline: none;
  width: 160px;
  background-color: #fff;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23606666' d='M6 8L1 3h10z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 10px center;
  appearance: none;
  transition: border-color 0.3s;
}

.search-select:focus {
  border-color: #409eff;
  box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.1);
}

.search-actions {
  display: flex;
  gap: 8px;
  margin-left: auto;
}

/* ===== 按钮样式 ===== */
.btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 7px 16px;
  border: 1px solid transparent;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 400;
  transition: all 0.3s;
  background: #fff;
}

.btn .icon {
  font-size: 14px;
}

.btn-primary {
  background: #409eff;
  color: #fff;
  border-color: #409eff;
}

.btn-primary:hover {
  background: #66b1ff;
  border-color: #66b1ff;
}

.btn-success {
  background: #67c23a;
  color: #fff;
  border-color: #67c23a;
}

.btn-success:hover {
  background: #85ce61;
  border-color: #85ce61;
}

.btn-danger {
  background: #f56c6c;
  color: #fff;
  border-color: #f56c6c;
}

.btn-danger:hover {
  background: #f78989;
  border-color: #f78989;
}

.btn-default {
  background: #fff;
  border-color: #dcdfe6;
  color: #606266;
}

.btn-default:hover {
  color: #409eff;
  border-color: #c6e2ff;
  background: #ecf5ff;
}

.btn-icon {
  width: 32px;
  height: 32px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  cursor: pointer;
  background: #fff;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
}

.btn-icon:hover {
  color: #409eff;
  border-color: #c6e2ff;
  background: #ecf5ff;
}

/* ===== 工具栏 ===== */
.toolbar {
  background: #fff;
  border-radius: 8px;
  padding: 14px 20px;
  margin-bottom: 12px;
  display: flex;
  align-items: center;
  gap: 10px;
  border: 1px solid #ebeef5;
}

.toolbar-right {
  margin-left: auto;
  display: flex;
  gap: 8px;
}

/* ===== 数据表格 ===== */
.table-wrapper {
  background: #fff;
  border-radius: 8px;
  border: 1px solid #ebeef5;
  overflow: hidden;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
}

.data-table thead {
  background: #fafafa;
}

.data-table th {
  padding: 12px 16px;
  text-align: left;
  font-weight: 500;
  color: #606266;
  border-bottom: 1px solid #ebeef5;
  white-space: nowrap;
}

.data-table td {
  padding: 12px 16px;
  border-bottom: 1px solid #ebeef5;
  color: #606266;
}

.data-table tbody tr:hover {
  background: #f5f7fa;
}

.col-check {
  width: 52px;
  text-align: center;
  vertical-align: middle;
}

/* ===== 自定义复选框 ===== */
.custom-checkbox-wrap {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  width: 20px;
  height: 20px;
}

.custom-checkbox-wrap input[type="checkbox"] {
  position: absolute;
  opacity: 0;
  width: 100%;
  height: 100%;
  margin: 0;
  cursor: pointer;
  z-index: 2;
}

.custom-checkbox {
  position: relative;
  width: 18px;
  height: 18px;
  min-width: 18px;
  border: 1px solid #dcdfe6;
  border-radius: 3px;
  background: #fff;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  z-index: 1;
}

.custom-checkbox-wrap input[type="checkbox"]:hover + .custom-checkbox {
  border-color: #a8d58e;
}

.custom-checkbox-wrap input[type="checkbox"]:checked + .custom-checkbox,
.custom-checkbox-wrap input[type="checkbox"]:indeterminate + .custom-checkbox {
  background: #67c23a;
  border-color: #67c23a;
}

.custom-checkbox-wrap input[type="checkbox"]:focus-visible + .custom-checkbox {
  box-shadow: 0 0 0 3px rgba(103, 194, 58, 0.2);
}

.check-icon {
  width: 14px;
  height: 14px;
  color: #fff;
  transition: transform 0.15s ease;
}

.custom-checkbox-wrap input[type="checkbox"]:checked + .custom-checkbox .check-icon {
  transform: scale(1);
}

.col-name {
  min-width: 260px;
}

.col-sort {
  width: 80px;
  text-align: center;
}

.col-status {
  width: 120px;
}

.col-time {
  width: 180px;
  color: #909399;
}

.col-action {
  width: 260px;
}

/* 部门名称单元格 */
.org-name-cell {
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.2s ease;
}

.org-name-cell.indent-1 {
  padding-left: 32px;
  position: relative;
}

.org-name-cell.indent-1::before {
  content: '';
  position: absolute;
  left: 12px;
  top: 50%;
  width: 16px;
  height: 1px;
  background: #e4e7ed;
}

.org-name-cell.indent-2 {
  padding-left: 64px;
  position: relative;
}

.org-name-cell.indent-2::before {
  content: '';
  position: absolute;
  left: 44px;
  top: 50%;
  width: 16px;
  height: 1px;
  background: #e4e7ed;
}

.org-name-cell.indent-2::after {
  content: '';
  position: absolute;
  left: 12px;
  top: 0;
  width: 1px;
  height: 100%;
  background: #e4e7ed;
}

.org-name {
  font-weight: 500;
  color: #303133;
  transition: color 0.2s;
}

.org-name.has-children {
  font-weight: 600;
}

/* 展开图标 - 使用 SVG 箭头 */
.expand-icon {
  width: 18px;
  height: 18px;
  border: none;
  background: transparent;
  cursor: pointer;
  padding: 0;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  color: #909399;
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  border-radius: 4px;
  flex-shrink: 0;
}

.expand-icon:hover {
  background: #ecf5ff;
  color: #409eff;
}

.expand-icon.is-expanded {
  color: #409eff;
  transform: rotate(0deg);
}

.expand-icon.is-expanded .expand-svg {
  transform: rotate(90deg);
}

.expand-svg {
  width: 14px;
  height: 14px;
  transition: transform 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}

.expand-icon.empty {
  cursor: default;
  pointer-events: none;
}

.expand-icon.empty:hover {
  background: transparent;
}

/* 列头展开按钮 */
.col-name-header {
  display: flex;
  align-items: center;
  gap: 12px;
}

.expand-all-btn {
  font-size: 12px;
  font-weight: 400;
  color: #409eff;
  cursor: pointer;
  padding: 2px 8px;
  border-radius: 4px;
  transition: all 0.2s;
  background: #ecf5ff;
}

.expand-all-btn:hover {
  background: #409eff;
  color: #fff;
}

/* 树形表格行样式 */
.tree-table tbody tr {
  transition: background-color 0.2s;
}

.tree-table .level-1 td {
  background: #fafbfc;
}

.tree-table .level-2 td {
  background: #f5f7fa;
}

.tree-table .level-3 td {
  background: #f0f3f7;
}

.tree-table .level-4 td {
  background: #eaeff5;
}

.tree-table tbody tr:hover td {
  background: #ecf5ff !important;
}

/* 展开动画效果 */
.tree-table tbody tr {
  animation: slideDown 0.25s ease-out;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* 展开图标旋转动画 */
.expand-icon .expand-svg {
  transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.expand-icon.is-expanded .expand-svg {
  transform: rotate(90deg);
}

/* 级别标签 */
.level-tag {
  display: inline-block;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  white-space: nowrap;
}

.level-national {
  background: #fef0f0;
  color: #f56c6c;
  border: 1px solid #fbc4c4;
}

.level-province {
  background: #fdf6ec;
  color: #e6a23c;
  border: 1px solid #f5dab1;
}

.level-city {
  background: #ecf5ff;
  color: #409eff;
  border: 1px solid #b3d8ff;
}

.level-district {
  background: #f0f9eb;
  color: #67c23a;
  border: 1px solid #c2e7b0;
}

.level-school {
  background: #f3e8ff;
  color: #9b59b6;
  border: 1px solid #d8b4fe;
}

/* 状态标签 */
.status-tag {
  font-size: 13px;
  font-weight: 500;
}

/* 操作按钮 */
.action-btns {
  display: flex;
  align-items: center;
  gap: 4px;
}

.action-btn {
  display: inline-flex;
  align-items: center;
  gap: 3px;
  padding: 4px 8px;
  border: none;
  background: transparent;
  cursor: pointer;
  font-size: 13px;
  transition: all 0.2s;
  border-radius: 3px;
}

.action-btn.edit {
  color: #409eff;
}

.action-btn.edit:hover {
  background: #ecf5ff;
  color: #66b1ff;
}

.action-btn.add {
  color: #67c23a;
}

.action-btn.add:hover {
  background: #f0f9eb;
  color: #85ce61;
}

.action-btn.delete {
  color: #f56c6c;
}

.action-btn.delete:hover {
  background: #fef0f0;
  color: #f78989;
}

.action-divider {
  color: #dcdfe6;
  margin: 0 2px;
}

/* ===== 空状态 ===== */
.empty-row {
  padding: 0 !important;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  color: #c0c4cc;
}

.empty-icon {
  font-size: 48px;
  margin-bottom: 12px;
}

.empty-hint {
  font-size: 13px;
  color: #dcdfe6;
  margin-top: 4px;
}

/* ===== 底部信息 ===== */
.table-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px;
  border-top: 1px solid #ebeef5;
}

.total-text {
  color: #606266;
  font-size: 14px;
}

/* ===== 弹窗表单 ===== */
.modal-form {
  padding: 8px 0;
}

.form-group {
  margin-bottom: 16px;
}

.form-group.half {
  width: 48%;
}

.form-row {
  display: flex;
  justify-content: space-between;
  gap: 4%;
}

.form-label {
  display: block;
  font-size: 14px;
  color: #606266;
  font-weight: 500;
  margin-bottom: 6px;
}

.form-label.required::after {
  content: '*';
  color: #f56c6c;
  margin-left: 4px;
}

.form-input,
.form-select {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 14px;
  outline: none;
  transition: border-color 0.3s;
  background: #fff;
  box-sizing: border-box;
}

.form-input:focus,
.form-select:focus {
  border-color: #409eff;
  box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.1);
}

.form-input:disabled {
  background: #f5f7fa;
  color: #c0c4cc;
  cursor: not-allowed;
}

/* ===== TreeSelect 样式 ===== */
.form-tree-select {
  width: 100%;
}

.form-tree-select :deep(.ant-select-selector) {
  border: 1px solid #dcdfe6 !important;
  border-radius: 4px !important;
  padding: 0 11px !important;
  height: 36px !important;
  transition: border-color 0.3s !important;
}

.form-tree-select :deep(.ant-select-selector:hover) {
  border-color: #409eff !important;
}

.form-tree-select :deep(.ant-select-focused .ant-select-selector) {
  border-color: #409eff !important;
  box-shadow: 0 0 0 2px rgba(64, 158, 255, 0.1) !important;
}

.form-tree-select :deep(.ant-select-selection-item) {
  line-height: 34px !important;
  padding: 0 !important;
}

.form-tree-select :deep(.ant-select-selection-placeholder) {
  line-height: 34px !important;
  color: #c0c4cc !important;
}

.form-tree-select :deep(.ant-tree-select-dropdown) {
  border-radius: 6px !important;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1) !important;
}

.form-tree-select :deep(.ant-tree-node-content-wrapper) {
  border-radius: 4px !important;
}

.form-tree-select :deep(.ant-tree-node-content-wrapper:hover) {
  background: #f5f7fa !important;
}

.form-tree-select :deep(.ant-tree-node-selected .ant-tree-node-content-wrapper) {
  background: #e6f4ff !important;
  color: #409eff !important;
}

.form-select {
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23606666' d='M6 8L1 3h10z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 12px center;
  padding-right: 32px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding-top: 16px;
  border-top: 1px solid #ebeef5;
  margin-top: 4px;
}

/* ===== 响应式 ===== */
@media (max-width: 1200px) {
  .search-input {
    width: 160px;
  }

  .search-select {
    width: 140px;
  }

  .org-stats {
    grid-template-columns: repeat(3, 1fr);
  }

  .stat-item:nth-child(3n) {
    border-right: none;
  }
}

@media (max-width: 768px) {
  .search-bar,
  .toolbar {
    flex-direction: column;
    align-items: stretch;
  }

  .search-actions {
    margin-left: 0;
    justify-content: flex-start;
  }

  .search-input,
  .search-select {
    width: 100%;
  }

  .toolbar-right {
    margin-left: 0;
  }

  .org-stats {
    grid-template-columns: repeat(2, 1fr);
  }

  .stat-item {
    border-right: none;
  }

  .data-table {
    font-size: 13px;
  }

  .data-table th,
  .data-table td {
    padding: 10px 12px;
  }

  .col-time {
    display: none;
  }

  .action-btns {
    flex-wrap: wrap;
  }
}
</style>
