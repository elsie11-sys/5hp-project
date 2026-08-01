<template>
  <div class="user-manage">
    <!-- 操作栏 -->
    <div class="toolbar">
      <button class="btn btn-primary" @click="handleAddUser">
        <span class="icon">➕</span> 新增用户
      </button>
      <!-- 修复：直接控制 showImportModal，移除不存在的 handleImportUser -->
      <button class="btn btn-success" @click="showImportModal = true">
        <span class="icon">📤</span> 批量导入
      </button>
      <button class="btn btn-danger" @click="handleBatchDelete">
        <span class="icon">🗑️</span> 批量删除
      </button>
      <div class="search-box">
        <input 
          type="text" 
          v-model="userSearch" 
          placeholder="搜索用户名/姓名..." 
          @keyup.enter="handleUserSearch"
        />
        <button class="btn btn-sm btn-primary" @click="handleUserSearch">搜索</button>
      </div>
    </div>

    <!-- 用户表格 -->
    <div class="table-wrapper">
      <table class="data-table">
        <thead>
          <tr>
            <th><input type="checkbox" @change="toggleSelectAll" :checked="isAllSelected" /></th>
            <th>用户ID</th>
            <th>用户名</th>
            <th>姓名</th>
            <th>角色</th>
            <th>所属机构</th>
            <th>手机号</th>
            <th>邮箱</th>
            <th>状态</th>
            <th>最后登录</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <!-- 优化：增加 || [] 兜底，防止 paginatedUsers 为 undefined 时页面白屏 -->
          <tr v-for="user in (paginatedUsers || [])" :key="user.id">
            <td><input type="checkbox" v-model="selectedIds" :value="user.id" /></td>
            <td class="user-id">{{ user.id }}</td>
            <td class="username">{{ user.username }}</td>
            <td>{{ user.name }}</td>
            <td><span class="role-tag" :class="user.roleClass">{{ user.role }}</span></td>
            <td>{{ user.org }}</td>
            <td>{{ user.phone }}</td>
            <td>{{ user.email }}</td>
            <td><span class="status-tag" :class="user.statusClass">{{ user.statusText }}</span></td>
            <td>{{ user.lastLogin }}</td>
            <td>
              <div class="action-btns">
                <button class="btn-icon edit" @click="handleEditUser(user)" title="编辑">✏️</button>
                <button class="btn-icon reset" @click="handleResetPassword(user)" title="重置密码">🔑</button>
                <button class="btn-icon delete" @click="handleDeleteUser(user)" title="删除">🗑️</button>
              </div>
            </td>
          </tr>
          <!-- 增加空数据提示 -->
          <tr v-if="!paginatedUsers || paginatedUsers.length === 0">
            <td colspan="11" style="text-align: center; padding: 40px 0; color: #999;">暂无数据</td>
          </tr>
        </tbody>
      </table>
      
      <div class="table-pagination">
        <!-- 核心修复：将 filteredUsers.length 改为 totalUsers (总条数) -->
        <span>共 {{ totalUsers }} 条</span>
        <div class="pagination-btns">
          <button class="page-btn" @click="prevPage" :disabled="currentPage === 1">上一页</button>
          <!-- 优化：增加 || [] 兜底 -->
          <button 
            v-for="page in (pageNumbers || [])" 
            :key="page"
            class="page-btn" 
            :class="{ active: currentPage === page }"
            @click="goToPage(page)"
          >
            {{ page }}
          </button>
          <button class="page-btn" @click="nextPage" :disabled="currentPage === totalPages">下一页</button>
        </div>
      </div>
    </div>

    <!-- ===== 新增用户弹窗 ===== -->
    <Modal v-model:open="showAddModal" title="新增用户" :footer="null" width="560px" @cancel="closeAddModal">
      <div class="modal-form">
        <div class="form-group">
          <label class="form-label required">用户名</label>
          <input v-model="addForm.username" type="text" placeholder="请输入用户名" class="form-input" />
        </div>
        
        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">姓名</label>
            <input v-model="addForm.name" type="text" placeholder="请输入姓名" class="form-input" />
          </div>
          <div class="form-group half">
            <label class="form-label required">用户性别</label>
            <select v-model="addForm.gender" class="form-select">
              <option value="">请选择性别</option>
              <option value="male">男</option>
              <option value="female">女</option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label class="form-label required">角色</label>
          <select v-model="addForm.role" class="form-select">
            <option value="">请选择角色</option>
            <option value="国家管理员">国家管理员</option>
            <option value="省级管理员">省级管理员</option>
            <option value="市级管理员">市级管理员</option>
            <option value="县级管理员">县级管理员</option>
            <option value="学校管理员">学校管理员</option>
            <option value="校医">校医</option>
            <option value="班主任">班主任</option>
          </select>
        </div>

        <div class="form-group">
          <label class="form-label required">所属机构</label>
          <select v-model="addForm.org" class="form-select">
            <option value="">请选择所属机构</option>
            <option value="国家教育部">国家教育部</option>
            <option value="江苏省教育厅">江苏省教育厅</option>
            <option value="浙江省教育厅">浙江省教育厅</option>
            <option value="南京市教育局">南京市教育局</option>
            <option value="苏州市教育局">苏州市教育局</option>
            <option value="鼓楼区教育局">鼓楼区教育局</option>
            <option value="南京市第一中学">南京市第一中学</option>
            <option value="南京市金陵中学">南京市金陵中学</option>
          </select>
        </div>

        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">手机号</label>
            <input v-model="addForm.phone" type="text" placeholder="请输入手机号" class="form-input" />
          </div>
          <div class="form-group half">
            <label class="form-label required">邮箱</label>
            <input v-model="addForm.email" type="text" placeholder="请输入邮箱" class="form-input" />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">用户密码</label>
            <input v-model="addForm.password" type="password" placeholder="请输入初始密码" class="form-input" />
          </div>
          <div class="form-group half">
            <label class="form-label required">状态</label>
            <select v-model="addForm.status" class="form-select">
              <option value="active">启用</option>
              <option value="inactive">禁用</option>
            </select>
          </div>
        </div>

        <div class="form-actions">
          <button class="btn btn-default" @click="closeAddModal">取消</button>
          <!-- 优化：增加 submitting 防重复点击 -->
          <button class="btn btn-primary" @click="submitAddUser" :disabled="submitting">
            {{ submitting ? '提交中...' : '确定' }}
          </button>
        </div>
      </div>
    </Modal>

    <!-- ===== 编辑用户弹窗 ===== -->
    <Modal v-model:open="showEditModal" title="编辑用户" :footer="null" width="560px" @cancel="closeEditModal">
      <div class="modal-form">
        <div class="form-group">
          <label class="form-label">用户ID</label>
          <input v-model="editForm.id" type="text" class="form-input" disabled />
        </div>
        <div class="form-group">
          <label class="form-label required">用户名</label>
          <input v-model="editForm.username" type="text" placeholder="请输入用户名" class="form-input" />
        </div>
        
        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">姓名</label>
            <input v-model="editForm.name" type="text" placeholder="请输入姓名" class="form-input" />
          </div>
          <div class="form-group half">
            <label class="form-label required">用户性别</label>
            <select v-model="editForm.gender" class="form-select">
              <option value="">请选择性别</option>
              <option value="male">男</option>
              <option value="female">女</option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label class="form-label required">角色</label>
          <select v-model="editForm.role" class="form-select">
            <option value="">请选择角色</option>
            <option value="国家管理员">国家管理员</option>
            <option value="省级管理员">省级管理员</option>
            <option value="市级管理员">市级管理员</option>
            <option value="县级管理员">县级管理员</option>
            <option value="学校管理员">学校管理员</option>
            <option value="校医">校医</option>
            <option value="班主任">班主任</option>
          </select>
        </div>
        <div class="form-group">
          <label class="form-label required">所属机构</label>
          <select v-model="editForm.org" class="form-select">
            <option value="">请选择所属机构</option>
            <option value="国家教育部">国家教育部</option>
            <option value="江苏省教育厅">江苏省教育厅</option>
            <option value="浙江省教育厅">浙江省教育厅</option>
            <option value="南京市教育局">南京市教育局</option>
            <option value="苏州市教育局">苏州市教育局</option>
            <option value="鼓楼区教育局">鼓楼区教育局</option>
            <option value="南京市第一中学">南京市第一中学</option>
            <option value="南京市金陵中学">南京市金陵中学</option>
          </select>
        </div>
        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">手机号</label>
            <input v-model="editForm.phone" type="text" placeholder="请输入手机号" class="form-input" />
          </div>
          <div class="form-group half">
            <label class="form-label required">邮箱</label>
            <input v-model="editForm.email" type="text" placeholder="请输入邮箱" class="form-input" />
          </div>
        </div>
        <div class="form-group">
          <label class="form-label required">状态</label>
          <select v-model="editForm.status" class="form-select">
            <option value="active">启用</option>
            <option value="inactive">禁用</option>
          </select>
        </div>
        <div class="form-actions">
          <button class="btn btn-default" @click="closeEditModal">取消</button>
          <button class="btn btn-primary" @click="submitEditUser" :disabled="submitting">
            {{ submitting ? '提交中...' : '确定' }}
          </button>
        </div>
      </div>
    </Modal>

    <!-- ===== 批量导入弹窗 ===== -->
    <Modal v-model:open="showImportModal" title="批量导入用户" :footer="null" width="560px" @cancel="closeImportModal">
      <div class="import-content">
        <div class="import-tip">
          <div class="tip-icon">📋</div>
          <div class="tip-text">
            <p><strong>导入说明：</strong></p>
            <p>1. 请下载导入模板，按照模板格式填写用户信息</p>
            <p>2. 支持 .xlsx, .xls 格式文件</p>
            <p>3. 文件大小不超过 5MB</p>
          </div>
        </div>
        <div class="import-actions">
          <button class="btn btn-info" @click="downloadTemplate">
            <span class="icon">📥</span> 下载模板
          </button>
          <div class="upload-wrapper">
            <input type="file" accept=".xlsx,.xls" @change="handleFileUpload" id="fileInput" style="display: none;" />
            <label for="fileInput" class="btn btn-success">
              <span class="icon">📤</span> 选择文件
            </label>
            <span v-if="uploadFileName" class="file-name">{{ uploadFileName }}</span>
          </div>
        </div>
        <div class="import-actions">
          <!-- 修复：将 !uploadFile 改为 !uploadFileName，因为 hook 中未暴露 uploadFile -->
          <button class="btn btn-primary" @click="submitImport" :disabled="!uploadFileName || submitting">
            {{ submitting ? '导入中...' : '✅ 导入' }}
          </button>
          <button class="btn btn-default" @click="closeImportModal">取消</button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { Modal } from 'ant-design-vue';
import { useUserManage } from './useUserManage';

// 重新对齐解构变量，确保与 useUserManage.ts 完全一致，移除不存在的变量
const {
  // 状态
  userSearch, selectedIds, currentPage, totalUsers, loading, submitting,
  showAddModal, showEditModal, showImportModal, uploadFileName,
  // 数据与表单
  addForm, editForm, isAllSelected, paginatedUsers, pageNumbers, totalPages,
  // 核心方法
  goToPage, handleUserSearch, handleAddUser, submitAddUser,
  handleEditUser, submitEditUser, handleDeleteUser, handleBatchDelete,
  handleResetPassword, handleFileUpload, submitImport, toggleSelectAll,
  // 弹窗控制
  closeAddModal, closeEditModal, closeImportModal,
  // 其他
  downloadTemplate, prevPage, nextPage
} = useUserManage();
</script>

<style scoped src="./UserManage.css"></style>
