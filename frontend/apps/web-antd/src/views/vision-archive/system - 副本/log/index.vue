<template>
  <div class="log-manage">
    <!-- 筛选栏 -->
    <div class="filter-bar">
      <div class="filter-row">
        <div class="filter-item">
          <label>操作类型：</label>
          <select v-model="logFilters.type">
            <option value="all">全部</option>
            <option value="login">登录</option>
            <option value="create">新增</option>
            <option value="edit">编辑</option>
            <option value="delete">删除</option>
            <option value="export">导出</option>
            <option value="import">导入</option>
          </select>
        </div>
        <div class="filter-item">
          <label>时间范围：</label>
          <input type="date" v-model="logFilters.startDate" />
          <span>至</span>
          <input type="date" v-model="logFilters.endDate" />
        </div>
        <div class="filter-item">
          <label>操作人：</label>
          <input type="text" v-model="logFilters.operator" placeholder="请输入操作人" />
        </div>
        <div class="btn-group">
          <button class="btn btn-primary btn-sm" @click="handleLogSearch">查询</button>
          <button class="btn btn-default btn-sm" @click="handleLogReset">重置</button>
          <button class="btn btn-success btn-sm" @click="handleExportLog">导出日志</button>
        </div>
      </div>
    </div>

    <!-- 日志表格 -->
    <div class="table-wrapper">
      <table class="data-table">
        <thead>
          <tr>
            <th>日志ID</th>
            <th>操作人</th>
            <th>操作类型</th>
            <th>操作模块</th>
            <th>操作内容</th>
            <th>IP地址</th>
            <th>操作时间</th>
            <th>状态</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="log in logs" :key="log.id">
            <td class="log-id">{{ log.id }}</td>
            <td>{{ log.operator }}</td>
            <td><span class="log-type" :class="log.typeClass">{{ log.typeText }}</span></td>
            <td>{{ log.module }}</td>
            <td class="log-content">{{ log.content }}</td>
            <td>{{ log.ip }}</td>
            <td>{{ log.time }}</td>
            <td><span class="status-tag" :class="log.statusClass">{{ log.statusText }}</span></td>
          </tr>
        </tbody>
      </table>
      <div class="table-pagination">
        <span>共 2,847 条</span>
        <div class="pagination-btns">
          <button class="page-btn">上一页</button>
          <button class="page-btn active">1</button>
          <button class="page-btn">2</button>
          <button class="page-btn">3</button>
          <button class="page-btn">下一页</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive } from 'vue';
import { message } from 'ant-design-vue';

const logFilters = reactive({
  type: 'all',
  startDate: '',
  endDate: '',
  operator: ''
});

const logs = [
  { id: 'L001', operator: '张伟', type: 'login', typeClass: 'log-login', typeText: '登录', module: '系统管理', content: '用户登录系统', ip: '192.168.1.100', time: '2026-07-06 14:30:25', status: 'success', statusText: '成功', statusClass: 'status-success' },
  { id: 'L002', operator: '李明', type: 'create', typeClass: 'log-create', typeText: '新增', module: '学生档案', content: '新增学生档案：张小明（2024001）', ip: '192.168.1.101', time: '2026-07-06 14:25:10', status: 'success', statusText: '成功', statusClass: 'status-success' },
  { id: 'L003', operator: '王芳', type: 'edit', typeClass: 'log-edit', typeText: '编辑', module: '数据采集', content: '审核通过：李小红（2024002）的视力数据', ip: '192.168.1.102', time: '2026-07-06 14:20:05', status: 'success', statusText: '成功', statusClass: 'status-success' },
  { id: 'L004', operator: 'admin', type: 'delete', typeClass: 'log-delete', typeText: '删除', module: '用户管理', content: '删除用户：testuser（U010）', ip: '192.168.1.1', time: '2026-07-06 14:15:30', status: 'success', statusText: '成功', statusClass: 'status-success' },
  { id: 'L005', operator: '陈晓', type: 'export', typeClass: 'log-export', typeText: '导出', module: '统计分析', content: '导出2026年春季学期近视率报表', ip: '192.168.1.103', time: '2026-07-06 14:10:45', status: 'success', statusText: '成功', statusClass: 'status-success' },
  { id: 'L006', operator: '张伟', type: 'login', typeClass: 'log-login', typeText: '登录', module: '系统管理', content: '用户登录失败-密码错误', ip: '192.168.1.100', time: '2026-07-06 13:50:12', status: 'failed', statusText: '失败', statusClass: 'status-failed' },
];

const handleLogSearch = () => message.success('日志查询中...');
const handleLogReset = () => {
  logFilters.type = 'all';
  logFilters.startDate = '';
  logFilters.endDate = '';
  logFilters.operator = '';
  message.success('已重置筛选条件');
};
const handleExportLog = () => message.success('正在导出日志文件...');
</script>

<style scoped>
.log-manage { width: 100%; }

.filter-bar {
  background: #fff;
  padding: 14px 20px;
  border-radius: 10px;
  margin-bottom: 16px;
  border: 1px solid #ebeef5;
}
.filter-row {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}
.filter-item {
  display: flex;
  align-items: center;
  gap: 6px;
}
.filter-item label {
  font-size: 13px;
  color: #606266;
  font-weight: 500;
  white-space: nowrap;
}
.filter-item select, .filter-item input {
  padding: 6px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 13px;
  outline: none;
}
.filter-item select:focus, .filter-item input:focus { border-color: #409eff; }
.filter-item input[type="date"] { width: 140px; }
.btn-group { display: flex; gap: 8px; margin-left: auto; }

.btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 6px 14px;
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
.btn-success { background: #67c23a; color: #fff; }
.btn-success:hover { background: #85ce61; }
.btn-default { background: #fff; border: 1px solid #dcdfe6; color: #606266; }
.btn-default:hover { color: #409eff; border-color: #409eff; }

.table-wrapper {
  background: #fff;
  border-radius: 10px;
  padding: 16px 20px;
  border: 1px solid #ebeef5;
  overflow-x: auto;
}
.data-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
  min-width: 900px;
}
.data-table th {
  background: #f5f7fa;
  padding: 10px 12px;
  text-align: left;
  font-weight: 600;
  color: #606266;
  border-bottom: 2px solid #e8ecf1;
  white-space: nowrap;
}
.data-table td {
  padding: 10px 12px;
  border-bottom: 1px solid #ebeef5;
  color: #303133;
}
.data-table tr:hover td { background: #f5f9ff; }

.log-id { font-family: monospace; color: #409eff; font-weight: 500; }
.log-content { max-width: 200px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }

.log-type { padding: 2px 10px; border-radius: 12px; font-size: 11px; }
.log-login { background: #e8f0fe; color: #409eff; }
.log-create { background: #e8f5e9; color: #52c41a; }
.log-edit { background: #fff3e0; color: #e6a23c; }
.log-delete { background: #fce4ec; color: #f56c6c; }
.log-export { background: #f3e5f5; color: #9b59b6; }

.status-tag { padding: 2px 10px; border-radius: 12px; font-size: 11px; }
.status-success { background: #e8f5e9; color: #52c41a; }
.status-failed { background: #fce4ec; color: #f56c6c; }

.table-pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 12px;
  border-top: 1px solid #ebeef5;
  margin-top: 4px;
  font-size: 13px;
  color: #606266;
}
.pagination-btns { display: flex; gap: 4px; }
.page-btn {
  padding: 4px 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  background: #fff;
  cursor: pointer;
  font-size: 13px;
  transition: all 0.2s;
}
.page-btn:hover { color: #409eff; border-color: #409eff; }
.page-btn.active { background: #409eff; color: #fff; border-color: #409eff; }

@media (max-width: 768px) {
  .filter-row { flex-direction: column; align-items: stretch; }
  .filter-item { width: 100%; }
  .filter-item input[type="date"] { width: 100%; }
  .btn-group { margin-left: 0; justify-content: center; }
}
</style>
