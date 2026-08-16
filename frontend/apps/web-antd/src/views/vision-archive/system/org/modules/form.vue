<script lang="ts" setup>
import { computed, nextTick, ref } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';

import { message } from 'ant-design-vue';

import { useVbenForm } from '#/adapter/form';
import { orgApi } from '#/api/vision-archive/system';
import type { OrgDto } from '#/api/vision-archive/system';

import { useFormSchema } from '../data';

const emits = defineEmits(['success']);

const [Form, formApi] = useVbenForm({
  schema: useFormSchema(),
  showDefaultActions: false,
});

const id = ref<number | string>();
// 编辑场景：保留原始 level，避免被父级推断逻辑覆盖
const originalLevel = ref<string | undefined>();
// 组织树缓存，用于从 parentId 推断子级 level
const orgTree = ref<OrgDto[]>([]);

/** 组织级别从高到低 */
const LEVEL_ORDER = ['国家级', '省级', '市级', '区县级', '学校级'];

/** 根据父级 level 推算子级 level；父级为空则为顶级"国家级" */
function getNextLevel(parentLevel: string | undefined): string {
  if (!parentLevel) return LEVEL_ORDER[0];
  const idx = LEVEL_ORDER.indexOf(parentLevel);
  if (idx === -1) return LEVEL_ORDER[0];
  // 已经是最末级（学校级）就保持同级别（按需可改为禁止继续添加）
  return LEVEL_ORDER[Math.min(idx + 1, LEVEL_ORDER.length - 1)];
}

/** 在组织树里按 id 查找节点 */
function findOrgById(
  tree: OrgDto[],
  target: number | string,
): OrgDto | undefined {
  for (const node of tree) {
    if (node.id === target) return node;
    if (node.children?.length) {
      const found = findOrgById(node.children, target);
      if (found) return found;
    }
  }
  return undefined;
}

async function loadOrgTree() {
  try {
    orgTree.value = await orgApi.getTree();
  } catch {
    orgTree.value = [];
  }
}

const [Drawer, drawerApi] = useVbenDrawer({
  async onConfirm() {
    const { valid } = await formApi.validate();
    if (!valid) return;
    const values = await formApi.getValues();
    drawerApi.lock();
    try {
      if (id.value) {
        // 编辑：保留原 level（如果原数据没 level 才用推断）
        const level = originalLevel.value ?? getNextLevel(undefined);
        await orgApi.update(id.value, {
          ...values,
          id: id.value,
          level,
        } as any);
        message.success('保存成功');
      } else {
        // 新增：根据 parentId 推断 level
        const parentId = values.parentId as number | string | undefined;
        let level = LEVEL_ORDER[0];
        if (parentId !== undefined && parentId !== null && parentId !== '') {
          const parent = findOrgById(orgTree.value, parentId);
          level = getNextLevel(parent?.level);
        }
        await orgApi.create({ ...values, level } as any);
        message.success('创建成功');
      }
      emits('success');
      drawerApi.close();
    } catch {
      drawerApi.unlock();
    }
  },
  async onOpenChange(isOpen) {
    if (isOpen) {
      const data = drawerApi.getData<any>();
      formApi.resetForm();
      id.value = undefined;
      originalLevel.value = undefined;
      // 拉组织树（ApiTreeSelect 自己也会拉，但我们要用于 level 推断，提前拉一次）
      await loadOrgTree();
      if (data) {
        if (data.id !== undefined) {
          id.value = data.id;
        }
        if (data.level) {
          originalLevel.value = data.level;
        }
        await nextTick();
        formApi.setValues({
          parentId: data.parentId,
          name: data.name,
          sort: data.sort,
          leader: data.leader,
          phone: data.phone,
          email: data.email,
          status: data.status,
        });
      }
    }
  },
});

const getTitle = computed(() => (id.value ? '编辑部门' : '添加部门'));
</script>
<template>
  <Drawer :title="getTitle">
    <Form />
  </Drawer>
</template>

<style scoped>
/*
 * 让 ant-design-vue InputNumber 的上/下箭头（.ant-input-number-handler-wrap）
 * 在 idle 状态下也常驻显示——默认是 opacity:0，只在 :hover / :focus 时才显示。
 * 用 :deep() 穿透 scoped 边界，影响 schema 里挂了 .always-show-controls 类的 InputNumber。
 */
:deep(.always-show-controls .ant-input-number-handler-wrap) {
  opacity: 1 !important;
}

/* 顺带让箭头本身在 idle 时颜色也清晰可见（默认 hover 状态颜色） */
:deep(.always-show-controls .ant-input-number-handler) {
  opacity: 1;
}
</style>
