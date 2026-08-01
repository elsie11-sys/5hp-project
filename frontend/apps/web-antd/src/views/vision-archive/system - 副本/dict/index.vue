<template>
  <div class="dict-manage">
    <!-- 搜索区 -->
    <div class="search-bar">
      <div class="search-item">
        <span class="label">字典名称</span>
        <input v-model="dictName" type="text" placeholder="请输入字典名称" class="input" />
      </div>
      <div class="search-item">
        <span class="label">字典类型</span>
        <input v-model="dictType" type="text" placeholder="请输入字典类型" class="input" />
      </div>
      <div class="search-item">
        <span class="label">状态</span>
        <select v-model="statusFilter" class="input">
          <option :value="undefined">字典状态</option>
          <option :value="1">正常</option>
          <option :value="0">停用</option>
        </select>
      </div>
      <div class="search-item">
        <span class="label">归属类型</span>
        <select v-model="ownerTypeFilter" class="input">
          <option :value="undefined">全部</option>
          <option value="SYSTEM">系统</option>
          <option value="USER">用户</option>
        </select>
      </div>
      <div class="search-item">
        <span class="label">创建时间</span>
        <input
          v-model="startDate"
          type="date"
          class="input date-input"
        />
        <span class="tilde">~</span>
        <input
          v-model="endDate"
          type="date"
          class="input date-input"
        />
      </div>
      <div class="search-actions">
        <button class="btn btn-primary" @click="handleSearch">🔍 搜索</button>
        <button class="btn btn-default" @click="handleReset">↺ 重置</button>
      </div>
    </div>

    <!-- 操作栏 -->
    <div class="toolbar">
      <button class="btn btn-primary" @click="handleAdd">➕ 新增</button>
      <button
        class="btn btn-primary"
        :class="{ 'is-disabled': selectedIds.length !== 1 }"
        :disabled="selectedIds.length !== 1"
        @click="handleBatchEdit"
      >✏️ 修改</button>
      <button
        class="btn btn-danger"
        :class="{ 'is-disabled': selectedIds.length === 0 }"
        :disabled="selectedIds.length === 0"
        @click="handleBatchDelete"
      ><span style="color:#ff4d4f;">🗑️</span> 删除</button>
      <button class="btn btn-warning" @click="handleExport">⬇ 导出</button>
      <button class="btn btn-success" @click="handleRefreshCache">🔄 刷新缓存</button>
      <div class="spacer"></div>
      <button class="btn-icon-only" @click="fetchDicts" title="刷新">⟳</button>
      <button class="btn-icon-only" title="列设置">≡</button>
    </div>

    <!-- 表格 -->
    <div class="table-wrapper">
      <table class="data-table">
        <thead>
          <tr>
            <th class="col-check">
              <label class="custom-checkbox" :class="{ indeterminate: !isAllSelected && selectedIds.length > 0 }">
                <input
                  type="checkbox"
                  :checked="isAllSelected"
                  :indeterminate="!isAllSelected && selectedIds.length > 0"
                  @change="toggleSelectAll"
                />
                <span class="checkmark"></span>
              </label>
            </th>
            <th>字典编号</th>
            <th>字典名称</th>
            <th>字典类型</th>
            <th>归属类型</th>
            <th>状态</th>
            <th>备注</th>
            <th>创建时间</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in paginatedDicts" :key="item.id">
            <td class="col-check">
              <label class="custom-checkbox">
                <input type="checkbox" :value="item.id" v-model="selectedIds" />
                <span class="checkmark"></span>
              </label>
            </td>
            <td>{{ index + 1 }}</td>
            <td>{{ item.dictName }}</td>
            <td>
              <Popover
                trigger="hover"
                placement="right"
                :overlay-style="{ maxWidth: '480px' }"
                @openChange="(open) => {
                  if (open) loadHoverDictItems(item);
                  else clearHoverDictItems();
                }"
              >
                <template #content>
                  <div class="hover-dict-preview">
                    <div v-if="hoverDictLoading" class="hover-loading">加载中...</div>
                    <div v-else-if="hoverDictItems.length === 0" class="hover-empty">暂无字典项</div>
                    <template v-else>
                      <div class="hover-dict-title">字典项列表 <span class="hover-count">共 {{ hoverDictItems.length }} 条</span></div>
                      <table class="hover-table">
                        <thead>
                          <tr>
                            <th style="width: 40px;">序号</th>
                            <th>字典标签</th>
                            <th>字典键值</th>
                            <th style="width: 50px;">排序</th>
                            <th style="width: 55px;">状态</th>
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-for="(di, idx) in hoverDictItems" :key="'hover-' + item.id + '-' + idx">
                            <td>{{ idx + 1 }}</td>
                            <td>{{ di.itemLabel }}</td>
                            <td class="hover-value">{{ di.itemValue }}</td>
                            <td>{{ di.sortOrder }}</td>
                            <td>
                              <span
                                class="hover-status"
                                :class="di.status === 1 ? 'normal' : 'disabled'"
                              >
                                {{ di.status === 1 ? '正常' : '停用' }}
                              </span>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </template>
                  </div>
                </template>
                <code class="type-code">{{ item.dictType }}</code>
              </Popover>
            </td>
            <td>
              <span class="owner-tag" :class="item.ownerType === 'SYSTEM' ? 'system' : 'user'">
                {{ item.ownerType === 'SYSTEM' ? '系统' : '用户' }}
              </span>
            </td>
            <td>
              <span class="status-tag" :class="item.status === 1 ? 'normal' : 'disabled'">
                {{ item.status === 1 ? '正常' : '停用' }}
              </span>
            </td>
            <td>{{ item.remark || '-' }}</td>
            <td>{{ formatDate(item.createdAt) }}</td>
            <td class="col-action">
              <button class="link-btn link-btn-edit" @click="handleEdit(item)"><span class="icon">✏️</span>修改</button>
              <span class="divider">|</span>
              <button class="link-btn link-btn-delete" @click="handleDelete(item)"><span class="icon">🗑️</span>删除</button>
            </td>
          </tr>
          <tr v-if="!paginatedDicts || paginatedDicts.length === 0">
            <td colspan="9" class="empty">暂无数据</td>
          </tr>
        </tbody>
      </table>

      <!-- 分页 -->
      <div class="pagination">
        <span class="total">共 {{ totalDicts }} 条</span>
        <select v-model="pageSize" class="page-size" @change="handlePageSizeChange">
          <option :value="10">10 条/页</option>
          <option :value="20">20 条/页</option>
          <option :value="50">50 条/页</option>
        </select>
        <div class="page-btns">
          <button class="page-btn" @click="prevPage" :disabled="currentPage === 1">‹</button>
          <button
            v-for="p in pageNumbers"
            :key="p"
            class="page-btn"
            :class="{ active: currentPage === p }"
            @click="goToPage(p)"
          >
            {{ p }}
          </button>
          <button class="page-btn" @click="nextPage" :disabled="currentPage === totalPages">›</button>
        </div>
        <span class="jump">
          前往
          <input
            type="number"
            min="1"
            :max="totalPages"
            v-model.number="jumpPage"
            class="jump-input"
            @keyup.enter="handleJumpPage"
          />
          页
        </span>
      </div>
    </div>

    <!-- ===== 新增弹窗 ===== -->
    <Modal v-model:open="showAddModal" title="添加字典类型" :footer="null" width="960px" :bodyStyle="{ maxHeight: '75vh', overflow: 'hidden' }" @cancel="showAddModal = false">
      <div class="modal-form modal-form-scroll">
        <div class="form-section">
          <div class="form-group">
          <label class="form-label required">字典名称</label>
          <div class="form-control">
            <input v-model="addForm.dictName" type="text" placeholder="请输入字典名称" class="form-input" />
          </div>
        </div>
        <div class="form-group">
          <label class="form-label required">字典类型</label>
          <div class="form-control">
              <input v-model="addForm.dictType" type="text" placeholder="请输入字典类型" class="form-input" />
              <div class="form-validation-error" v-if="addSubmitted && addForm.dictType === ''">字典类型不能为空</div>
            </div>
        </div>
        <div class="form-group">
          <label class="form-label">状态</label>
          <div class="form-control">
            <div class="radio-group">
              <label class="radio-item" :class="{ active: addForm.status === 1 }">
                <input type="radio" :value="1" v-model="addForm.status" />
                <span class="radio-dot"></span>
                <span>正常</span>
              </label>
              <label class="radio-item" :class="{ active: addForm.status === 0 }">
                <input type="radio" :value="0" v-model="addForm.status" />
                <span class="radio-dot"></span>
                <span>停用</span>
              </label>
            </div>
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">归属类型</label>
          <div class="form-control">
            <div class="radio-group">
              <label class="radio-item" :class="{ active: addForm.ownerType === 'SYSTEM' }">
                <input type="radio" value="SYSTEM" v-model="addForm.ownerType" />
                <span class="radio-dot"></span>
                <span>系统</span>
              </label>
              <label class="radio-item" :class="{ active: addForm.ownerType === 'USER' }">
                <input type="radio" value="USER" v-model="addForm.ownerType" />
                <span class="radio-dot"></span>
                <span>用户</span>
              </label>
            </div>
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">备注</label>
          <div class="form-control">
            <textarea v-model="addForm.remark" placeholder="请输入内容" class="form-textarea" rows="3"></textarea>
          </div>
        </div>

        <!-- 字典项管理 -->
        <div class="dict-items-section">
          <div class="dict-items-header">
            <div class="dict-items-title-bar">
              <span class="dict-items-title">字典项列表</span>
              <span class="selected-count" v-if="selectedAddItemIds.length > 0">已选 {{ selectedAddItemIds.length }} 项</span>
            </div>
            <div class="dict-items-actions">
              <button class="btn btn-primary btn-sm" @click="handleAddItemSubmitAdd">➕ 添加</button>
              <button
                class="btn btn-danger btn-sm"
                :disabled="selectedAddItemIds.length === 0"
                @click="batchDeleteAddItems"
              >🗑️ 批量删除</button>
            </div>

          </div>
          <div class="dict-items-table-wrapper">
            <table class="dict-items-table">
              <thead>
                <tr>
                  <th class="col-check">
                    <label class="custom-checkbox" :class="{ indeterminate: !isAllAddItemsSelected && selectedAddItemIds.length > 0 }">
                      <input
                        type="checkbox"
                        :checked="isAllAddItemsSelected"
                        :indeterminate="!isAllAddItemsSelected && selectedAddItemIds.length > 0"
                        @change="toggleSelectAllAddItems"
                      />
                      <span class="checkmark"></span>
                    </label>
                  </th>
                  <th style="width:60px;">序号</th>
                  <th><span class="required-mark">*</span>字典标签</th>
                  <th><span class="required-mark">*</span>字典键值</th>
                  <th style="width:60px;"><span class="required-mark">*</span>排序</th>
                  <th style="width:80px;"><span class="required-mark">*</span>状态</th>
                  <th>备注</th>
                  <th style="width:140px;">操作</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in paginatedAddDictItems" :key="'add-' + addDictItems.indexOf(item)" :class="{ 'row-deleted': item.isDeleted }">
                  <td class="col-check">
                    <label class="custom-checkbox" :class="{ 'is-disabled': item.isDeleted }">
                      <input
                        type="checkbox"
                        :checked="selectedAddItemIds.includes(item.id ?? addDictItems.indexOf(item))"
                        :disabled="item.isDeleted"
                        @change="toggleSelectAddItem(item, addDictItems.indexOf(item))"
                      />
                      <span class="checkmark"></span>
                    </label>
                  </td>
                  <td>{{ addDictItems.indexOf(item) + 1 }}</td>
                  <td>
                    <input v-model="item.itemLabel" type="text" placeholder="标签" class="cell-input" :class="{ 'cell-input-error': addSubmitted && !item.itemLabel?.trim() }" />
                  </td>
                  <td>
                    <input v-model="item.itemValue" type="text" placeholder="键值" class="cell-input" :class="{ 'cell-input-error': addSubmitted && !item.itemValue?.trim() }" />
                  </td>
                  <td>
                    <input v-model.number="item.sortOrder" type="number" placeholder="0" class="cell-input cell-input-sm" :class="{ 'cell-input-error': addSubmitted && (item.sortOrder === null || item.sortOrder === undefined || isNaN(item.sortOrder)) }" />
                  </td>
                  <td>
                    <select v-model="item.status" class="cell-input cell-input-sm" @change="handleItemStatusChangeAdd(addDictItems.indexOf(item))">
                      <option :value="1">正常</option>
                      <option :value="0">停用</option>
                    </select>
                  </td>
                  <td>
                    <input v-model="item.remark" type="text" placeholder="备注" class="cell-input" />
                  </td>
                  <td class="col-actions">
                    <button class="link-btn link-btn-edit" @click="handleEditItem('add', addDictItems.indexOf(item))">
                      <span class="icon">✏️</span>修改
                    </button>
                    <button class="link-btn link-btn-delete" @click="handleRemoveItemAdd(addDictItems.indexOf(item))">
                      <span class="icon">🗑️</span>删除
                    </button>
                  </td>
                </tr>
                <tr v-if="activeAddDictItems.length === 0">
                  <td colspan="8" class="empty">暂无字典项，点击「添加」按钮新增</td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="dict-items-footer" v-if="activeAddDictItems.length > 0">
            <span class="total-label">共 {{ activeAddDictItems.length }} 条</span>
            <div class="item-pagination">
              <button class="page-btn" @click="goAddItemPage(addItemCurrentPage - 1)" :disabled="addItemCurrentPage <= 1">‹</button>
              <button
                v-for="p in addItemPageNumbers"
                :key="'add-p-' + p"
                class="page-btn"
                :class="{ active: addItemCurrentPage === p }"
                @click="goAddItemPage(p)"
              >
                {{ p }}
              </button>
              <button class="page-btn" @click="goAddItemPage(addItemCurrentPage + 1)" :disabled="addItemCurrentPage >= addItemTotalPages">›</button>
              <span class="jump">
                前往
                <input type="number" min="1" :max="addItemTotalPages" v-model.number="addItemJumpPage" class="jump-input" @keyup.enter="handleAddItemJump" />
                页
              </span>
            </div>
          </div>
        </div>
        </div>

        <div class="form-actions">
          <button class="btn btn-primary" @click="submitAdd" :disabled="submitting">
            {{ submitting ? '提交中...' : '确定' }}
          </button>
          <button class="btn btn-default" @click="showAddModal = false">取消</button>
        </div>
      </div>
    </Modal>

    <!-- ===== 编辑弹窗 ===== -->
    <Modal v-model:open="showEditModal" title="编辑字典" :footer="null" width="960px" :bodyStyle="{ maxHeight: '75vh', overflow: 'hidden' }" @cancel="showEditModal = false">
      <div class="modal-form modal-form-scroll">
        <div class="form-section">
          <div class="form-group">
          <label class="form-label required">字典名称</label>
          <div class="form-control">
            <input v-model="editForm.dictName" type="text" placeholder="请输入字典名称" class="form-input" />
          </div>
        </div>
        <div class="form-group">
          <label class="form-label required">字典类型</label>
          <div class="form-control">
            <input
              v-model="editForm.dictType"
              type="text"
              placeholder="请输入字典类型"
              class="form-input"
              :disabled="editForm.ownerType === 'SYSTEM'"
            />
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">状态</label>
          <div class="form-control">
            <div class="radio-group">
              <label class="radio-item" :class="{ active: editForm.status === 1 }">
                <input type="radio" :value="1" v-model="editForm.status" />
                <span class="radio-dot"></span>
                <span>正常</span>
              </label>
              <label class="radio-item" :class="{ active: editForm.status === 0 }">
                <input type="radio" :value="0" v-model="editForm.status" />
                <span class="radio-dot"></span>
                <span>停用</span>
              </label>
            </div>
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">归属类型</label>
          <div class="form-control">
            <div class="radio-group">
              <label class="radio-item" :class="{ active: editForm.ownerType === 'SYSTEM' }" :style="{ opacity: editForm.ownerType === 'SYSTEM' ? 1 : 0.5 }">
                <input type="radio" value="SYSTEM" v-model="editForm.ownerType" :disabled="editForm.ownerType !== 'SYSTEM'" />
                <span class="radio-dot"></span>
                <span>系统</span>
              </label>
              <label class="radio-item" :class="{ active: editForm.ownerType === 'USER' }">
                <input type="radio" value="USER" v-model="editForm.ownerType" />
                <span class="radio-dot"></span>
                <span>用户</span>
              </label>
            </div>
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">备注</label>
          <div class="form-control">
            <textarea v-model="editForm.remark" placeholder="请输入内容" class="form-textarea" rows="3"></textarea>
          </div>
        </div>

        <!-- 字典项管理 -->
        <div class="dict-items-section">
          <div class="dict-items-header">
            <div class="dict-items-title-bar">
              <span class="dict-items-title">字典项列表</span>
              <span class="selected-count" v-if="selectedEditItemIds.length > 0">已选 {{ selectedEditItemIds.length }} 项</span>
            </div>
            <div class="dict-items-actions">
              <button class="btn btn-primary btn-sm" @click="handleAddItemSubmitEdit">➕ 添加</button>
              <button
                class="btn btn-danger btn-sm"
                :disabled="selectedEditItemIds.length === 0"
                @click="batchDeleteEditItems"
              >🗑️ 批量删除</button>
            </div>
          </div>
          <div class="dict-items-table-wrapper">
            <table class="dict-items-table">
              <thead>
                <tr>
                  <th class="col-check">
                    <label class="custom-checkbox" :class="{ indeterminate: !isAllEditItemsSelected && selectedEditItemIds.length > 0 }">
                      <input
                        type="checkbox"
                        :checked="isAllEditItemsSelected"
                        :indeterminate="!isAllEditItemsSelected && selectedEditItemIds.length > 0"
                        @change="toggleSelectAllEditItems"
                      />
                      <span class="checkmark"></span>
                    </label>
                  </th>
                  <th style="width:60px;">序号</th>
                  <th><span class="required-mark">*</span>字典标签</th>
                  <th><span class="required-mark">*</span>字典键值</th>
                  <th style="width:60px;"><span class="required-mark">*</span>排序</th>
                  <th style="width:80px;"><span class="required-mark">*</span>状态</th>
                  <th>备注</th>
                  <th style="width:140px;">操作</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in paginatedEditDictItems" :key="'edit-' + editDictItems.indexOf(item)" :class="{ 'row-deleted': item.isDeleted }">
                  <td class="col-check">
                    <label class="custom-checkbox" :class="{ 'is-disabled': item.isDeleted }">
                      <input
                        type="checkbox"
                        :checked="selectedEditItemIds.includes(item.id ?? editDictItems.indexOf(item))"
                        :disabled="item.isDeleted"
                        @change="toggleSelectEditItem(item, editDictItems.indexOf(item))"
                      />
                      <span class="checkmark"></span>
                    </label>
                  </td>
                  <td>{{ editDictItems.indexOf(item) + 1 }}</td>
                  <td>
                    <input v-model="item.itemLabel" type="text" placeholder="标签" class="cell-input" :class="{ 'cell-input-error': editSubmitted && !item.itemLabel?.trim() }" />
                  </td>
                  <td>
                    <input v-model="item.itemValue" type="text" placeholder="键值" class="cell-input" :class="{ 'cell-input-error': editSubmitted && !item.itemValue?.trim() }" />
                  </td>
                  <td>
                    <input v-model.number="item.sortOrder" type="number" placeholder="0" class="cell-input cell-input-sm" :class="{ 'cell-input-error': editSubmitted && (item.sortOrder === null || item.sortOrder === undefined || isNaN(item.sortOrder)) }" />
                  </td>
                  <td>
                    <select v-model="item.status" class="cell-input cell-input-sm" @change="handleItemStatusChangeEdit(editDictItems.indexOf(item))">
                      <option :value="1">正常</option>
                      <option :value="0">停用</option>
                    </select>
                  </td>
                  <td>
                    <input v-model="item.remark" type="text" placeholder="备注" class="cell-input" />
                  </td>
                  <td class="col-actions">
                    <button class="link-btn link-btn-edit" @click="handleEditItem('edit', editDictItems.indexOf(item))">
                      <span class="icon">✏️</span>修改
                    </button>
                    <button class="link-btn link-btn-delete" @click="handleRemoveItemEdit(editDictItems.indexOf(item))">
                      <span class="icon">🗑️</span>删除
                    </button>
                  </td>
                </tr>
                <tr v-if="activeEditDictItems.length === 0">
                  <td colspan="8" class="empty">暂无字典项，点击「添加」按钮新增</td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="dict-items-footer" v-if="activeEditDictItems.length > 0">
            <span class="total-label">共 {{ activeEditDictItems.length }} 条</span>
            <div class="item-pagination">
              <button class="page-btn" @click="goEditItemPage(editItemCurrentPage - 1)" :disabled="editItemCurrentPage <= 1">‹</button>
              <button
                v-for="p in editItemPageNumbers"
                :key="'edit-p-' + p"
                class="page-btn"
                :class="{ active: editItemCurrentPage === p }"
                @click="goEditItemPage(p)"
              >
                {{ p }}
              </button>
              <button class="page-btn" @click="goEditItemPage(editItemCurrentPage + 1)" :disabled="editItemCurrentPage >= editItemTotalPages">›</button>
              <span class="jump">
                前往
                <input type="number" min="1" :max="editItemTotalPages" v-model.number="editItemJumpPage" class="jump-input" @keyup.enter="handleEditItemJump" />
                页
              </span>
            </div>
          </div>
        </div>
        </div>

        <div class="form-actions">
          <button class="btn btn-primary" @click="submitEdit" :disabled="submitting">
            {{ submitting ? '提交中...' : '确定' }}
          </button>
          <button class="btn btn-default" @click="showEditModal = false">取消</button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { Modal, Popover } from 'ant-design-vue';
import { useDictManage } from './useDictManage';
import type { DictItemDto } from './useDictManage';

const {
  dictName,
  dictType,
  statusFilter,
  ownerTypeFilter,
  startDate,
  endDate,
  currentPage,
  pageSize,
  totalDicts,
  submitting,
  selectedIds,
  showAddModal,
  showEditModal,
  addSubmitted,
  editSubmitted,
  addForm,
  editForm,
  addDictItems,
  editDictItems,
  paginatedDicts,
  isAllSelected,
  totalPages,
  fetchDicts,
  handleSearch,
  handleReset,
  goToPage,
  prevPage,
  nextPage,
  handleAdd,
  submitAdd,
  handleEdit,
  submitEdit,
  handleDelete,
  handleBatchDelete,
  handleExport,
  handleRefreshCache,
  toggleSelectAll,
  formatDate,
  handleAddItemSubmitAdd,
  handleAddItemSubmitEdit,
  handleRemoveItemAdd,
  handleRemoveItemEdit,
  selectedAddItemIds,
  selectedEditItemIds,
  editingAddItemIndex,
  editingEditItemIndex,
  isAllAddItemsSelected,
  isAllEditItemsSelected,
  toggleSelectAllAddItems,
  toggleSelectAllEditItems,
  toggleSelectAddItem,
  toggleSelectEditItem,
  batchDeleteAddItems,
  batchDeleteEditItems,
  handleEditItem,
  handleSaveItem,
  handleCancelEditItem,
  itemPageSize,
  addItemCurrentPage,
  editItemCurrentPage,
  activeAddDictItems,
  activeEditDictItems,
  addItemTotalPages,
  editItemTotalPages,
  addItemPageNumbers,
  editItemPageNumbers,
  paginatedAddDictItems,
  paginatedEditDictItems,
  goAddItemPage,
  goEditItemPage,
  hoverDictItems,
  hoverDictLoading,
  hoverDictId,
  loadHoverDictItems,
  clearHoverDictItems,
  handleItemStatusChangeAdd,
  handleItemStatusChangeEdit,
} = useDictManage();

const jumpPage = ref<number>(1);
const addItemJumpPage = ref<number>(1);
const editItemJumpPage = ref<number>(1);

const handleAddItemJump = () => {
  if (addItemJumpPage.value >= 1 && addItemJumpPage.value <= addItemTotalPages.value) {
    addItemCurrentPage.value = addItemJumpPage.value;
  }
};

const handleEditItemJump = () => {
  if (editItemJumpPage.value >= 1 && editItemJumpPage.value <= editItemTotalPages.value) {
    editItemCurrentPage.value = editItemJumpPage.value;
  }
};

const pageNumbers = computed(() => {
  const total = totalPages.value;
  const current = currentPage.value;
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
  const pages: number[] = [];
  pages.push(1);
  let start = Math.max(2, current - 1);
  let end = Math.min(total - 1, current + 1);
  if (current <= 4) {
    start = 2;
    end = Math.min(total - 1, 5);
  } else if (current >= total - 3) {
    start = Math.max(2, total - 4);
    end = total - 1;
  }
  for (let i = start; i <= end; i++) pages.push(i);
  pages.push(total);
  return pages;
});

const handlePageSizeChange = () => {
  currentPage.value = 1;
  fetchDicts();
};

const handleJumpPage = () => {
  if (jumpPage.value >= 1 && jumpPage.value <= totalPages.value) {
    currentPage.value = jumpPage.value;
    fetchDicts();
  }
};

const handleBatchEdit = () => {
  if (selectedIds.value.length === 0) {
    return;
  }
  const first = paginatedDicts.value.find((d) => d.id === selectedIds.value[0]);
  if (first) handleEdit(first);
};
</script>

<style scoped>
.dict-manage {
  width: 100%;
  padding: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'PingFang SC', 'Microsoft YaHei', 'Helvetica Neue', Helvetica, Arial, sans-serif;
  color: #303133;
  font-size: 14px;
  line-height: 1.5;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.breadcrumb {
  margin-bottom: 12px;
  font-size: 13px;
  color: #606266;
  line-height: 1.5;
}
.breadcrumb .sep {
  margin: 0 6px;
  color: #c0c4cc;
}
.breadcrumb .current {
  color: #303133;
  font-weight: 500;
}

.search-bar {
  background: #fff;
  border: 1px solid #ebeef5;
  border-radius: 4px;
  padding: 16px 20px 8px;
  margin-bottom: 12px;
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  align-items: center;
}
.search-item {
  display: flex;
  align-items: center;
  gap: 8px;
}
.search-item .label {
  color: #606266;
  font-size: 14px;
  white-space: nowrap;
  line-height: 1.5;
}
.search-item .input {
  height: 32px;
  padding: 0 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 14px;
  color: #303133;
  font-family: inherit;
  outline: none;
  transition: border-color 0.2s;
  background: #fff;
  width: 180px;
  line-height: 1.5;
}
.search-item .input::placeholder {
  color: #c0c4cc;
  font-size: 14px;
}
.search-item .input:focus {
  border-color: #409eff;
}
.date-input {
  width: 140px !important;
}
.tilde {
  color: #c0c4cc;
  font-size: 14px;
}
.search-actions {
  margin-left: auto;
  display: flex;
  gap: 8px;
}

.toolbar {
  background: #fff;
  border: 1px solid #ebeef5;
  border-radius: 4px;
  padding: 10px 16px;
  margin-bottom: 12px;
  display: flex;
  align-items: center;
  gap: 8px;
}
.toolbar .spacer {
  flex: 1;
}

.table-wrapper {
  background: #fff;
  border: 1px solid #ebeef5;
  border-radius: 4px;
  overflow: hidden;
}
.data-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
  color: #606266;
}
.data-table thead th {
  background: #f5f7fa;
  color: #303133;
  font-weight: 600;
  font-size: 14px;
  text-align: left;
  padding: 12px 14px;
  border-bottom: 1px solid #ebeef5;
  line-height: 1.5;
}
.data-table tbody td {
  padding: 12px 14px;
  border-bottom: 1px solid #f0f2f5;
  color: #606266;
  font-size: 14px;
  line-height: 1.5;
}
.data-table tbody tr:hover {
  background: #f5f7fa;
}
.data-table tbody tr:last-child td {
  border-bottom: none;
}
.col-check {
  width: 40px;
  text-align: center;
}
.col-check .custom-checkbox {
  cursor: pointer;
}

.custom-checkbox {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
  cursor: pointer;
}
.custom-checkbox input {
  position: absolute;
  opacity: 0;
  width: 0;
  height: 0;
}
.custom-checkbox .checkmark {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 1px solid #dcdfe6;
  border-radius: 3px;
  background: #fff;
  transition: all 0.2s;
  box-sizing: border-box;
}
.custom-checkbox:hover .checkmark {
  border-color: #67c23a;
}
.custom-checkbox input:checked + .checkmark {
  background: #67c23a;
  border-color: #67c23a;
}
.custom-checkbox input:checked + .checkmark::after {
  content: '';
  position: absolute;
  left: 5px;
  top: 2px;
  width: 5px;
  height: 9px;
  border: solid #fff;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}
.custom-checkbox.indeterminate .checkmark {
  background: #67c23a;
  border-color: #67c23a;
}
.custom-checkbox.indeterminate .checkmark::after {
  content: '';
  position: absolute;
  left: 3px;
  top: 7px;
  width: 8px;
  height: 2px;
  background: #fff;
  border: none;
  transform: none;
}

.btn.is-disabled,
.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  pointer-events: none;
}
.col-action {
  width: 160px;
}
.type-code {
  font-family: 'SFMono-Regular', Consolas, 'Liberation Mono', Menlo, monospace;
  font-size: 13px;
  color: #409eff;
  background: #ecf5ff;
  padding: 2px 8px;
  border-radius: 3px;
  border: 1px solid #d9ecff;
  line-height: 1.5;
  cursor: pointer;
  transition: all 0.2s;
}
.type-code:hover {
  background: #d9ecff;
  border-color: #409eff;
}

/* 悬浮预览字典项样式 */
.hover-dict-preview {
  min-width: 380px;
  max-height: 380px;
  overflow-y: auto;
}
.hover-dict-title {
  font-size: 13px;
  font-weight: 600;
  color: #303133;
  margin-bottom: 8px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.hover-count {
  font-size: 12px;
  font-weight: normal;
  color: #909399;
}
.hover-loading,
.hover-empty {
  text-align: center;
  color: #909399;
  padding: 20px;
  font-size: 13px;
}
.hover-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 12px;
}
.hover-table thead th {
  background: #f5f7fa;
  color: #606266;
  font-weight: 600;
  text-align: left;
  padding: 8px 10px;
  border-bottom: 1px solid #ebeef5;
  white-space: nowrap;
}
.hover-table tbody td {
  padding: 6px 10px;
  border-bottom: 1px solid #f0f2f5;
  color: #606266;
  vertical-align: middle;
}
.hover-table tbody tr:last-child td {
  border-bottom: none;
}
.hover-table tbody tr:hover {
  background: #f9fafc;
}
.hover-value {
  color: #606266;
  font-family: 'SFMono-Regular', Consolas, monospace;
  font-size: 12px;
}
.hover-status {
  font-size: 12px;
  padding: 1px 8px;
  border-radius: 10px;
  white-space: nowrap;
}
.hover-status.normal {
  background: #f0f9eb;
  color: #67c23a;
}
.hover-status.disabled {
  background: #fef0f0;
  color: #f56c6c;
}
.status-tag {
  display: inline-block;
  padding: 2px 10px;
  border-radius: 10px;
  font-size: 12px;
  line-height: 1.5;
  font-weight: 500;
}
.status-tag.normal {
  background: #f0f9eb;
  color: #67c23a;
  border: 1px solid #e1f3d8;
}
.status-tag.disabled {
  background: #fef0f0;
  color: #f56c6c;
  border: 1px solid #fde2e2;
}
.owner-tag {
  display: inline-block;
  padding: 2px 10px;
  border-radius: 10px;
  font-size: 12px;
  line-height: 1.5;
  font-weight: 500;
}
.owner-tag.system {
  background: #ecf5ff;
  color: #409eff;
  border: 1px solid #d9ecff;
}
.owner-tag.user {
  background: #fdf6ec;
  color: #e6a23c;
  border: 1px solid #faecd8;
}
.empty {
  text-align: center;
  padding: 48px 0 !important;
  color: #909399;
  font-size: 14px;
}

.link-btn {
  background: none;
  border: none;
  padding: 0 6px;
  cursor: pointer;
  font-size: 13px;
  transition: color 0.2s;
  display: inline-flex;
  align-items: center;
  gap: 3px;
  color: #409eff;
  font-family: inherit;
  line-height: 1.5;
}
.link-btn:hover {
  text-decoration: underline;
}
.link-btn-edit {
  color: #409eff;
}
.link-btn-edit:hover {
  color: #66b1ff;
}
.link-btn-delete {
  color: #f56c6c;
}
.link-btn-delete:hover {
  color: #f78989;
}
.link-btn .icon {
  font-size: 14px;
  line-height: 1;
}
.col-actions {
  display: flex;
  align-items: center;
  gap: 4px;
  white-space: nowrap;
}
.col-actions .link-btn {
  padding: 0 4px;
}
.divider {
  color: #dcdfe6;
  font-size: 12px;
}

.pagination {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  border-top: 1px solid #f0f2f5;
  font-size: 14px;
  color: #606266;
}
.pagination .total {
  color: #606266;
  font-size: 14px;
}
.page-size {
  height: 28px;
  padding: 0 8px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  outline: none;
  background: #fff;
  cursor: pointer;
  font-size: 14px;
  color: #606266;
  font-family: inherit;
}
.page-btns {
  display: flex;
  gap: 4px;
  margin-left: 8px;
}
.page-btn {
  min-width: 28px;
  height: 28px;
  padding: 0 8px;
  border: 1px solid #dcdfe6;
  background: #fff;
  color: #606266;
  border-radius: 3px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
  font-family: inherit;
  line-height: 1;
}
.page-btn:hover:not(:disabled) {
  border-color: #409eff;
  color: #409eff;
}
.page-btn.active {
  background: #409eff;
  border-color: #409eff;
  color: #fff;
}
.page-btn:disabled {
  background: #f5f7fa;
  color: #c0c4cc;
  cursor: not-allowed;
}
.jump .jump-input {
  width: 48px;
  height: 28px;
  text-align: center;
  border: 1px solid #dcdfe6;
  border-radius: 3px;
  outline: none;
  margin: 0 4px;
  font-size: 14px;
  color: #303133;
  font-family: inherit;
}
.jump .jump-input:focus {
  border-color: #409eff;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 6px 14px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-family: inherit;
  transition: all 0.2s;
  background: #fff;
  color: #606266;
  line-height: 1.5;
}
.btn:hover {
  opacity: 0.85;
}
.btn-primary {
  background: #409eff;
  border-color: #409eff;
  color: #fff;
}
.btn-success {
  background: #67c23a;
  border-color: #67c23a;
  color: #fff;
}
.btn-danger {
  background: #f56c6c;
  border-color: #f56c6c;
  color: #fff;
}
.btn-warning {
  background: #e6a23c;
  border-color: #e6a23c;
  color: #fff;
}
.btn-info {
  background: #909399;
  border-color: #909399;
  color: #fff;
}
.btn-default {
  background: #fff;
  color: #606266;
}
.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-icon-only {
  width: 32px;
  height: 32px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  background: #fff;
  color: #606266;
  cursor: pointer;
  font-size: 16px;
  transition: all 0.2s;
}
.btn-icon-only:hover {
  border-color: #409eff;
  color: #409eff;
}

.modal-form {
  padding: 8px 0;
}
.modal-form-scroll {
  display: flex;
  flex-direction: column;
  height: 100%;
  max-height: 75vh;
  overflow: hidden;
}
.form-section {
  flex: 1;
  min-height: 0;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  padding-right: 4px;
}
.form-section::-webkit-scrollbar {
  width: 6px;
}
.form-section::-webkit-scrollbar-thumb {
  background: #c0c4cc;
  border-radius: 3px;
}
.form-section::-webkit-scrollbar-thumb:hover {
  background: #909399;
}
.form-group {
  margin-bottom: 18px;
  display: flex;
  align-items: flex-start;
  gap: 12px;
}
.form-label {
  flex-shrink: 0;
  width: 80px;
  text-align: right;
  font-size: 14px;
  color: #606266;
  line-height: 36px;
  font-family: inherit;
}
.form-label.required::after {
  content: '*';
  color: #f56c6c;
  margin-left: 4px;
}
.form-control {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
}
.form-input {
  width: 100%;
  height: 36px;
  padding: 0 12px;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  font-size: 14px;
  font-family: inherit;
  color: #303133;
  outline: none;
  transition: border-color 0.2s;
  box-sizing: border-box;
  background: #fff;
  line-height: 1.5;
}
.form-input::placeholder {
  color: #c0c4cc;
  font-size: 14px;
}
.form-input:focus {
  border-color: #409eff;
}
.form-input:disabled {
  background: #f5f7fa;
  color: #c0c4cc;
  cursor: not-allowed;
}
.form-validation-error {
  margin-top: 4px;
  font-size: 12px;
  color: #f56c6c;
  line-height: 1.5;
}
.form-textarea {
  height: auto;
  min-height: 60px;
  resize: vertical;
  padding: 8px 12px;
  font-size: 14px;
  line-height: 1.5;
  width: 100%;
  box-sizing: border-box;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  outline: none;
  transition: border-color 0.2s;
  background: #fff;
  font-family: inherit;
  color: #303133;
}
.form-textarea::placeholder {
  color: #c0c4cc;
  font-size: 14px;
}
.form-textarea:focus {
  border-color: #409eff;
}

.radio-group {
  display: flex;
  gap: 24px;
  height: 36px;
  align-items: center;
}
.radio-item {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  font-size: 14px;
  color: #606266;
  user-select: none;
  font-family: inherit;
  line-height: 1.5;
}
.radio-item input {
  display: none;
}
.radio-dot {
  width: 16px;
  height: 16px;
  border: 2px solid #dcdfe6;
  border-radius: 50%;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
  position: relative;
  background: #fff;
}
.radio-item.active .radio-dot {
  border-color: #67c23a;
}
.radio-item.active .radio-dot::after {
  content: '';
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #67c23a;
}
.radio-item:hover .radio-dot {
  border-color: #c0c4cc;
}
.form-actions {
  flex-shrink: 0;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding-top: 16px;
  margin-top: 12px;
  border-top: 1px solid #f0f2f5;
  background: #fff;
}

.btn-sm {
  padding: 4px 10px;
  font-size: 13px;
  line-height: 1.4;
}

.dict-items-section {
  margin-top: 20px;
  border-top: 1px solid #ebeef5;
  padding-top: 16px;
  flex: 1;
  min-height: 0;
  display: flex;
  flex-direction: column;
}
.dict-items-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 12px;
  flex-shrink: 0;
}
.dict-items-title-bar {
  display: flex;
  align-items: center;
  gap: 12px;
}
.dict-items-title {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
}
.selected-count {
  font-size: 13px;
  color: #409eff;
  background: #ecf5ff;
  padding: 2px 8px;
  border-radius: 3px;
}
.dict-items-actions {
  display: flex;
  gap: 8px;
}
.dict-items-table-wrapper {
  border: 1px solid #ebeef5;
  border-radius: 4px;
  overflow: hidden;
  position: relative;
  flex: 1;
  min-height: 0;
  overflow-y: auto;
}
.dict-items-table-wrapper::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}
.dict-items-table-wrapper::-webkit-scrollbar-thumb {
  background: #c0c4cc;
  border-radius: 3px;
}
.dict-items-table-wrapper::-webkit-scrollbar-track {
  background: #f5f7fa;
}
.dict-items-table thead th {
  position: sticky;
  top: 0;
  z-index: 1;
  background: #f5f7fa;
}
.dict-items-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 4px 0;
  margin-top: 4px;
  flex-shrink: 0;
}
.dict-items-footer .total-label {
  font-size: 13px;
  color: #909399;
}
.item-pagination {
  display: flex;
  align-items: center;
  gap: 8px;
}
.item-pagination .page-btn {
  padding: 4px 12px;
  font-size: 13px;
  border: 1px solid #dcdfe6;
  border-radius: 3px;
  background: #fff;
  color: #606266;
  cursor: pointer;
  transition: all 0.2s;
}
.item-pagination .page-btn:hover:not(:disabled) {
  border-color: #409eff;
  color: #409eff;
}
.item-pagination .page-btn.active {
  background: #409eff;
  border-color: #409eff;
  color: #fff;
}
.item-pagination .page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
.item-pagination .jump {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-size: 13px;
  color: #606266;
}
.item-pagination .jump-input {
  width: 45px;
  height: 26px;
  padding: 0 6px;
  border: 1px solid #dcdfe6;
  border-radius: 3px;
  font-size: 13px;
  text-align: center;
  outline: none;
}
.item-pagination .jump-input:focus {
  border-color: #409eff;
}
.dict-items-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 13px;
  color: #606266;
}
.dict-items-table thead th {
  background: #f5f7fa;
  color: #303133;
  font-weight: 600;
  font-size: 13px;
  text-align: left;
  padding: 8px 10px;
  border-bottom: 1px solid #ebeef5;
  white-space: nowrap;
}
.dict-items-table tbody td {
  padding: 6px 8px;
  border-bottom: 1px solid #f0f2f5;
  font-size: 13px;
  vertical-align: middle;
}
.dict-items-table tbody tr:hover {
  background: #f9fafc;
}
.dict-items-table tbody tr:last-child td {
  border-bottom: none;
}
.dict-items-table tbody tr.row-deleted {
  opacity: 0.45;
  background: #fef0f0;
}
.cell-input {
  width: 100%;
  height: 30px;
  padding: 0 8px;
  border: 1px solid #dcdfe6;
  border-radius: 3px;
  font-size: 13px;
  color: #303133;
  outline: none;
  transition: border-color 0.2s;
  box-sizing: border-box;
  background: #fff;
}
.cell-input:focus {
  border-color: #409eff;
}
.cell-input-sm {
  width: 70px;
}
.required-mark {
  color: #f56c6c;
  margin-right: 3px;
  font-weight: bold;
}
.cell-input-error {
  border-color: #f56c6c !important;
  background-color: #fef0f0 !important;
}
.dict-items-table .empty {
  text-align: center;
  padding: 24px 0 !important;
  color: #909399;
  font-size: 13px;
}

@media (max-width: 768px) {
  .search-bar {
    flex-direction: column;
    align-items: stretch;
  }
  .search-item {
    width: 100%;
  }
  .search-item .input {
    flex: 1;
    width: auto;
  }
  .search-actions {
    margin-left: 0;
    justify-content: flex-end;
  }
}
</style>
