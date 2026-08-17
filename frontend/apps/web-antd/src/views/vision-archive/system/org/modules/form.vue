<script lang="ts" setup>
import { computed, nextTick, ref } from 'vue';

import { useVbenDrawer } from '@vben/common-ui';

import { message } from 'ant-design-vue';

import { useVbenForm } from '#/adapter/form';
import { orgApi } from '#/api/vision-archive/system';
import type { OrgDto } from '#/api/vision-archive/system';

import { loadOrgLevelOptions, useFormSchema } from '../data';

const emits = defineEmits(['success']);

const [Form, formApi] = useVbenForm({
  schema: useFormSchema(),
  showDefaultActions: false,
});

const id = ref<number | string>();
// 组织树缓存，用于从 parentId 推断子级 level
const orgTree = ref<OrgDto[]>([]);
// 字典项列表（顺序：国家级→学校级），用于推断默认值
const levelOptions = ref<Array<{ label: string; value: string }>>([]);

/**
 * 根据父级 level 推算子级 level。
 * 顺序以字典项为准（不再是硬编码 LEVEL_ORDER）：
 * - 无父级 → 顶级（options[0]）
 * - 父级在 options 中 → 下一项；已是最后一级则保持原级
 * - 父级不在 options 中（脏数据/字典改了） → 兜底 options[0]
 */
function getNextLevel(
  parentLevel: string | undefined,
  options: Array<{ value: string }>,
): string | undefined {
  const first = options[0]?.value;
  if (!options.length || first === undefined) return undefined;
  if (!parentLevel) return first;
  const idx = options.findIndex((o) => o.value === parentLevel);
  if (idx === -1) return first;
  return options[Math.min(idx + 1, options.length - 1)]?.value ?? first;
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
      // values.level 已经是字典项标签本身（ApiSelect 的 value=itemLabel），
      // 直接落到后端 sys_org.level 列即可。
      if (id.value) {
        await orgApi.update(id.value, { ...values, id: id.value } as any);
        message.success('保存成功');
      } else {
        await orgApi.create(values as any);
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
      // 拉组织树 + 字典项。
      // ApiTreeSelect / ApiSelect 自己也会拉，但 form.vue 这里要：
      // 1) 用 orgTree 根据 parentId 推断默认 level
      // 2) 用 levelOptions 校验父级 level 在不在选项里
      // 所以提前拉一次，并行起来更省时。
      await Promise.all([
        loadOrgTree(),
        loadOrgLevelOptions().then((opts) => {
          levelOptions.value = opts;
        }),
      ]);

      if (data) {
        if (data.id !== undefined) {
          id.value = data.id;
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

      // 编辑：用原 level（itemValue，如 "1"）；新增：根据父级 itemValue 推断下一级
      // options 里的 value 也是 itemValue，所以比对时是数字字符串与数字字符串比对
      let defaultLevel: string | undefined;
      if (id.value) {
        defaultLevel = data?.level;
      } else {
        // 新增场景：onAppend 会把 parentId 透传到 drawer（index.vue 的 onAppend），
        // 新建顶级时则没 parentId。直接用 data.parentId，不再去 form 里兜底取。
        const parentId = data?.parentId as number | string | undefined;
        if (
          parentId !== undefined &&
          parentId !== null &&
          parentId !== ''
        ) {
          const parent = findOrgById(orgTree.value, parentId);
          // parent.level 来自后端 sys_org.level，是字典 itemValue（如 "1"）
          defaultLevel = getNextLevel(parent?.level, levelOptions.value);
        } else {
          // 无父级 → 顶级（options[0]，即 "1"）
          defaultLevel = levelOptions.value[0]?.value;
        }
      }
      if (defaultLevel !== undefined) {
        formApi.setFieldValue('level', defaultLevel);
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
