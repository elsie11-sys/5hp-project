<template>
  <div class="menu-manage">
    <!-- 搜索栏 -->
    <div class="search-bar">
      <div class="search-item">
        <label class="search-label">菜单名称</label>
        <input
          type="text"
          v-model="menuSearch"
          placeholder="请输入菜单名称"
          class="search-input"
          @keyup.enter="handleSearch"
        />
      </div>
      <div class="search-item">
        <label class="search-label">状态</label>
        <select v-model="statusFilter" class="search-select">
          <option value="">菜单状态</option>
          <option :value="1">正常</option>
          <option :value="0">停用</option>
        </select>
      </div>
      <div class="search-actions">
        <button class="btn btn-primary" @click="handleSearch">
          <span class="icon">🔍</span> 搜索
        </button>
        <button class="btn btn-default" @click="handleReset">
          <span class="icon">↺</span> 重置
        </button>
      </div>
    </div>

    <!-- 操作栏 -->
    <div class="toolbar">
      <div class="toolbar-left">
        <button class="btn btn-primary" @click="handleAddMenu(null)">
          <span class="icon">➕</span> 新增
        </button>
        <button class="btn btn-default" @click="toggleExpandAll">
          <span class="icon">{{ isAllExpanded ? '⤡' : '⤢' }}</span>
          {{ isAllExpanded ? '折叠全部' : '展开全部' }}
        </button>
      </div>
    </div>

    <!-- 菜单表格 -->
    <div class="table-wrapper">
      <table class="data-table">
        <thead>
          <tr>
            <th class="col-menu-name">菜单名称</th>
            <th class="col-icon">图标</th>
            <th class="col-sort">排序</th>
            <th class="col-permission">权限标识</th>
            <th class="col-component">组件路径</th>
            <th class="col-status">状态</th>
            <th class="col-created">创建时间</th>
            <th class="col-actions">操作</th>
          </tr>
        </thead>
        <tbody>
          <!-- 使用递归组件渲染树形结构 -->
          <menu-tree-node
            :nodes="filteredMenuTree"
            :level="0"
            :is-expanded="isExpanded"
            :toggle-expand="toggleExpand"
            @edit="handleEditMenu"
            @add="handleAddMenu"
            @delete="handleDeleteMenu"
          />

          <!-- 空状态 -->
          <tr v-if="filteredMenuTree.length === 0 && !loading">
            <td colspan="8" class="empty-cell">
              <div class="empty-state">
                <div class="empty-icon">📭</div>
                <div class="empty-text">暂无菜单数据</div>
                <div class="empty-desc">点击"新增"按钮创建第一个菜单</div>
              </div>
            </td>
          </tr>

          <!-- 加载状态 -->
          <tr v-if="loading">
            <td colspan="8" class="loading-cell">
              <div class="loading-state">
                <div class="loading-spinner"></div>
                <div class="loading-text">加载中...</div>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- ===== 新增菜单弹窗 ===== -->
    <Modal v-model:open="showAddModal" title="新增菜单" :footer="null" width="600px" @cancel="closeAddModal">
      <div class="modal-form">
        <div class="form-group">
          <label class="form-label required">菜单名称</label>
          <input
            v-model="addForm.name"
            type="text"
            placeholder="请输入菜单名称"
            class="form-input"
          />
        </div>

        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">菜单类型</label>
            <select v-model="addForm.type" class="form-select">
              <option :value="1">目录</option>
              <option :value="2">菜单</option>
              <option :value="3">按钮</option>
            </select>
          </div>
          <div class="form-group half">
            <label class="form-label required">状态</label>
            <select v-model="addForm.status" class="form-select">
              <option :value="1">正常</option>
              <option :value="0">停用</option>
            </select>
          </div>
        </div>

        <div class="form-row">
          <div class="form-group half">
            <label class="form-label">上级菜单</label>
            <select v-model="addForm.parentId" class="form-select">
              <option
                v-for="opt in parentMenuOptions"
                :key="opt.value ?? 'root'"
                :value="opt.value"
              >
                {{ opt.label }}
              </option>
            </select>
          </div>
          <div class="form-group half">
            <label class="form-label required">排序</label>
            <input
              v-model.number="addForm.sort"
              type="number"
              placeholder="请输入排序号"
              class="form-input"
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group half">
            <label class="form-label">图标</label>
            <select v-model="addForm.icon" class="form-select">
              <option value="">请选择图标</option>
              <option v-for="icon in ICON_OPTIONS" :key="icon.value" :value="icon.value">
                {{ icon.label }}
              </option>
            </select>
          </div>
          <div class="form-group half">
            <label class="form-label">权限标识</label>
            <input
              v-model="addForm.permission"
              type="text"
              placeholder="如：system:user:list"
              class="form-input"
            />
          </div>
        </div>

        <div class="form-group" v-if="addForm.type !== 3">
          <label class="form-label">组件路径</label>
          <input
            v-model="addForm.component"
            type="text"
            placeholder="如：system/user/index"
            class="form-input"
          />
        </div>

        <div class="form-actions">
          <button class="btn btn-default" @click="closeAddModal">取消</button>
          <button class="btn btn-primary" @click="submitAddMenu" :disabled="submitting">
            {{ submitting ? '提交中...' : '确定' }}
          </button>
        </div>
      </div>
    </Modal>

    <!-- ===== 编辑菜单弹窗 ===== -->
    <Modal v-model:open="showEditModal" title="编辑菜单" :footer="null" width="600px" @cancel="closeEditModal">
      <div class="modal-form">
        <div class="form-group">
          <label class="form-label required">菜单名称</label>
          <input
            v-model="editForm.name"
            type="text"
            placeholder="请输入菜单名称"
            class="form-input"
          />
        </div>

        <div class="form-row">
          <div class="form-group half">
            <label class="form-label required">菜单类型</label>
            <select v-model="editForm.type" class="form-select">
              <option :value="1">目录</option>
              <option :value="2">菜单</option>
              <option :value="3">按钮</option>
            </select>
          </div>
          <div class="form-group half">
            <label class="form-label required">状态</label>
            <select v-model="editForm.status" class="form-select">
              <option :value="1">正常</option>
              <option :value="0">停用</option>
            </select>
          </div>
        </div>

        <div class="form-row">
          <div class="form-group half">
            <label class="form-label">上级菜单</label>
            <select v-model="editForm.parentId" class="form-select">
              <option
                v-for="opt in parentMenuOptions"
                :key="opt.value ?? 'root'"
                :value="opt.value"
              >
                {{ opt.label }}
              </option>
            </select>
          </div>
          <div class="form-group half">
            <label class="form-label required">排序</label>
            <input
              v-model.number="editForm.sort"
              type="number"
              placeholder="请输入排序号"
              class="form-input"
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group half">
            <label class="form-label">图标</label>
            <select v-model="editForm.icon" class="form-select">
              <option value="">请选择图标</option>
              <option v-for="icon in ICON_OPTIONS" :key="icon.value" :value="icon.value">
                {{ icon.label }}
              </option>
            </select>
          </div>
          <div class="form-group half">
            <label class="form-label">权限标识</label>
            <input
              v-model="editForm.permission"
              type="text"
              placeholder="如：system:user:list"
              class="form-input"
            />
          </div>
        </div>

        <div class="form-group" v-if="editForm.type !== 3">
          <label class="form-label">组件路径</label>
          <input
            v-model="editForm.component"
            type="text"
            placeholder="如：system/user/index"
            class="form-input"
          />
        </div>

        <div class="form-actions">
          <button class="btn btn-default" @click="closeEditModal">取消</button>
          <button class="btn btn-primary" @click="submitEditMenu" :disabled="submitting">
            {{ submitting ? '提交中...' : '确定' }}
          </button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { Modal } from 'ant-design-vue';
import MenuTreeNode from './MenuTreeNode.vue';
import { useMenuManage } from './useMenuManage';

const {
  menuSearch,
  statusFilter,
  loading,
  submitting,
  showAddModal,
  showEditModal,
  filteredMenuTree,
  addForm,
  editForm,
  isAllExpanded,
  isExpanded,
  toggleExpand,
  parentMenuOptions,
  ICON_OPTIONS,
  handleSearch,
  handleReset,
  toggleExpandAll,
  handleAddMenu,
  submitAddMenu,
  handleEditMenu,
  submitEditMenu,
  handleDeleteMenu,
  closeAddModal,
  closeEditModal,
} = useMenuManage();
</script>

<style scoped src="./MenuManage.css"></style>
