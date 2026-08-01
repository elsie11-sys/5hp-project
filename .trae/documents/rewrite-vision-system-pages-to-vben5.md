# 重写 vision-archive/system 系统管理页面为 Vben 5.x 写法

## Context（背景与目标）

`apps/web-antd` 是 Vben Admin **5.x** 项目，但 `src/views/vision-archive/system/` 下的系统管理页面（user、role、org、menu）是从 **Vben 2.x demo** 复制来的，使用了项目里根本不存在的导入路径：

- `@/components/Table`、`@/components/Drawer`、`@/components/Modal`、`@/components/Page`、`@/components/VxeTable`
- `@/api/demo/system`
- `@/hooks/web/useMessage`、`@/hooks/web/usePage`

经核实 `src/` 下没有 `components`、`hooks`、`api/demo` 目录，导致 Vite 报 `Failed to resolve import`（当前报错：user/index.vue 的 `@/api/demo/system`）。role、org、menu 同样是坏代码，修好 user 后会接连报错。

目标：把路由内的 4 个坏页面（user、role、org、menu）重写为 Vben 5.x 标准写法，对接已有的真实 API（`#/api/vision-archive/system`），让系统管理模块可用且构建通过。dict、log 已是自包含写法，保持不动。

## 参考写法（已确认）

playground 里的 `playground/src/views/system/{user,role,menu,dept}/` 是 Vben 5.x 标准实现，作为重写模板。核心模式：

- 列表页 `index.vue`：`useVbenVxeGrid`（来自 `#/adapter/vxe-table`）+ `Page`/`useVbenDrawer`（来自 `@vben/common-ui`）
- 列/表单定义 `data.ts`：导出 `useColumns`、`useGridFormSchema`、`useFormSchema`（函数式，类型用 `VxeTableGridColumns`、`VbenFormSchema`）
- 抽屉表单 `modules/form.vue`：`useVbenForm`（来自 `#/adapter/form`）+ `useVbenDrawer`，`onConfirm` 里 `validate`→`getValues`→create/update
- 表格查询 `proxyConfig.ajax.query: async ({ page }, formValues) => api.getPagedList({ page: page.currentPage, pageSize: page.pageSize, ...formValues })`
- 状态列用 `CellSwitch`（带 `beforeChange` 确认），操作列用 `CellOperation` + `#action` 插槽里的 `VbenTableAction`

本项目与 playground 的差异（重写时需适配）：
- 组件库是 `ant-design-vue`（不是 playground 的 `antdv-next`）→ message/Modal/Tag/Button 从 `ant-design-vue` 导入
- API 是对象式（`userApi.getUserList`、`roleApi.getPagedList`、`menuApi.getTree`、`orgApi.getPagedList`），不是 playground 的扁平函数
- adapter 缺少 `CellSwitch`/`CellOperation`/`CellTag` 渲染器和 `VbenTableAction`（见 Step 1）

## 实施步骤

### Step 1：补全 adapter 基础（一次性，所有模块共用）

修改 `apps/web-antd/src/adapter/vxe-table.ts`：从 `playground/src/adapter/vxe-table.ts` 移植
- 注册 `CellSwitch`、`CellOperation`、`CellTag` 渲染器（`vxeUI.renderer.add(...)`）
- 定义并导出 `VbenTableAction` 组件（包一层 `@vben/common-ui` 的 `VbenTableAction`）
- 保留现有 `CellImage`/`CellLink`、`setupVbenVxeTable`、`useVbenVxeGrid` 不变

> 这是 playground 表格模式能跑的前提；不补这步，状态列/操作列无法渲染。

### Step 2：重写 user 模块

目录 `src/views/vision-archive/system/user/`，对接 `userApi`（`getUserList`/`createUser`/`updateUser`/`deleteUser`，返回 `{items,total}`；表单 DTO 为 `FrontendUserForm`，列表 DTO 为 `BackendUserDto`）。

- 删除旧文件：`account.data.ts`、`AccountModal.vue`、`AccountDetail.vue`、`DeptTree.vue`
- 新建 `data.ts`：`useColumns(onStatusChange)`（列：username、realName、gender、role/org 可用 roleId/orgId、phone、email、status[CellSwitch]、createdAt、operation）、`useGridFormSchema`（搜索：keyword、status、createTime）、`useFormSchema`（表单字段对齐 FrontendUserForm：username、name、gender、role[Select ROLE_OPTIONS]、org[Select ORG_OPTIONS]、phone、email、password、status）
- 新建 `modules/form.vue`：`useVbenForm` + `useVbenDrawer`，`onConfirm` 调 `userApi.createUser`/`updateUser`；`onOpenChange` 用 `drawerApi.getData()` 回填
- 重写 `index.vue`：左侧组织树用 `orgApi.getTree()` + `@vben/common-ui` 的 `Tree`（替代旧 DeptTree），右侧 `useVbenVxeGrid`，`proxyConfig.ajax.query` 调 `userApi.getUserList`；工具栏新增按钮、`#action` 插槽用 `VbenTableAction`（详情/编辑/删除）

### Step 3：重写 role 模块

目录 `src/views/vision-archive/system/role/`，对接 `roleApi`（`getPaged`/`create`/`update`/`delete`；`RoleDto`：id、name、code、level、status、remark、createdAt；`RoleForm`）。

- 删除旧文件：`role.data.ts`、`RoleDrawer.vue`
- 新建 `data.ts`：`useColumns`（name、code、level、status[CellSwitch]、remark、createdAt、operation）、`useGridFormSchema`（name、code、status、createTime）、`useFormSchema`（name、code、level[Select ROLE_LEVEL_OPTIONS]、status[RadioGroup]、remark）
- 新建 `modules/form.vue`：`onConfirm` 调 `roleApi.create`/`update`
- 重写 `index.vue`：`useVbenVxeGrid`，query 调 `roleApi.getPaged`；新增/编辑走 `FormDrawer`
- 保留记忆中的"角色权限管理界面绿色主题"偏好：在 `index.vue` 局部样式里给状态/操作区以绿色调点缀（不破坏整体）

### Step 4：重写 org 模块

目录 `src/views/vision-archive/system/org/`，对接 `orgApi`（`getPagedList`/`getTree`/`create`/`update`/`delete`；`OrgDto`：id、name、code、level、parentId、sort、status、children；`OrgForm`）。

- 删除旧文件：`useOrgManage.ts`、`DeptModal.vue`（org 目录下的）
- 新建 `data.ts`：`useColumns`（name、code、level、sort、status、operation）、`useGridFormSchema`、`useFormSchema`（name、code、level[Select ORG_LEVEL_OPTIONS]、parentId[ApiTreeSelect→orgApi.getTree]、sort、status）
- 新建 `modules/form.vue`：`onConfirm` 调 `orgApi.create`/`update`
- 重写 `index.vue`：`useVbenVxeGrid`，query 调 `orgApi.getPagedList`（或树形展示用 `getTree` + `treeConfig`）

### Step 5：重写 menu 模块

目录 `src/views/vision-archive/system/menu/`，对接 `menuApi`（`getTree`/`getAll`/`create`/`update`/`delete`；`MenuDto`：id、name、code、icon、type、parentId、sort、status、path、component、permission、children；`MenuForm`）。

- 删除旧文件：`menu.data.ts`、`MenuDrawer.vue`
- 新建 `data.ts`：`useColumns`（name、icon、type、path、permission、status、operation）、`useGridFormSchema`（name、status、type）、`useFormSchema`（name、code、icon[IconPicker]、type[Select 1目录/2菜单/3按钮]、parentId[ApiTreeSelect→menuApi.getTree]、sort、status、path、component、permission）
- 新建 `modules/form.vue`：`onConfirm` 调 `menuApi.create`/`update`
- 重写 `index.vue`：树形表格，query 调 `menuApi.getTree()`，`treeConfig: { transform: false, rowField: 'id', parentField: 'parentId' }`（MenuDto 已自带 children，也可直接用 `data`）

### Step 6：路由核对

`src/router/routes/modules/vision-system.ts` 已在上一步把 permission→role 改好（name `VisionRole`、path `/vision-system/role`、组件指向 `role/index.vue`）。本轮无需再改路由，仅确认 user/org/menu 三条路由组件路径仍指向各自 `index.vue`（已正确）。

## 不处理的部分（说明）

- `dict/`、`log/`：自包含写法，正常工作，不动
- `dept/`、`vxe-account/`、`password/`：未在路由中引用的孤儿 demo 残留；本轮**不删除**（删除属不可逆操作，未经明确确认），但它们含坏导入。由于未被路由引用、不会被 Vite 入口扫描到，理论上不阻塞构建；若实际仍报错，再单独处理
- `system - 副本/`：备份目录，同上不动

## 关键复用资源

- API：`#/api/vision-archive/system`（`userApi`、`roleApi`、`menuApi`、`orgApi`，及 `ROLE_OPTIONS`、`ORG_OPTIONS`、`ROLE_LEVEL_OPTIONS`、`ORG_LEVEL_OPTIONS`、`DATA_SCOPE_OPTIONS`）
- adapter：`#/adapter/vxe-table`（`useVbenVxeGrid`、`VxeTableGridOptions`、`VxeTableGridColumns`、新增 `VbenTableAction`）、`#/adapter/form`（`useVbenForm`、`VbenFormSchema`、`z`）
- common-ui：`Page`、`Tree`、`useVbenDrawer`、`Descriptions`、`VbenTableAction`(core)
- 模板：`playground/src/views/system/{user,role,menu,dept}/` 与 `playground/src/adapter/vxe-table.ts`
- adapter proxyConfig 已配 `response:{result:'items',total:'total'}`，与所有 API 返回的 `{items,total}` 一致，无需改

## 验证

1. 启动 dev：`pnpm dev:antd`（或项目既有命令），确认无 `Failed to resolve import` 报错
2. 浏览器进入 `/vision-system/user`、`/vision-system/role`、`/vision-system/org`、`/vision-system/menu`，确认列表加载、搜索、分页正常
3. 每页点"新增"→填表→保存→列表刷新；点"编辑"→回填→保存；点"删除"→确认→刷新
4. user 页左侧组织树点击节点能筛选右侧列表
5. role 页状态开关切换有二次确认
6. menu 页树形展开/折叠正常
7. `pnpm build`（或既有构建命令）通过
