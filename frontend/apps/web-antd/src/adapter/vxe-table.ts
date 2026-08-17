import type { VxeTableGridOptions } from '@vben/plugins/vxe-table';

import type { ComponentPropsMap, ComponentType } from './component';

import {
  defineComponent,
  h,
  resolveComponent,
} from 'vue';

import { VbenTableAction as VbenTableActionCore } from '@vben/common-ui';
import {
  setupVbenVxeTable,
  useVbenVxeGrid as useGrid,
} from '@vben/plugins/vxe-table';

import { Button, Image, Switch, Tag } from 'ant-design-vue';

import { useVbenForm } from './form';

setupVbenVxeTable({
  configVxeTable: (vxeUI) => {
    vxeUI.setConfig({
      grid: {
        align: 'center',
        border: false,
        columnConfig: {
          resizable: true,
        },
        minHeight: 180,
        formConfig: {
          // 全局禁用vxe-table的表单配置，使用formOptions
          enabled: false,
        },
        proxyConfig: {
          autoLoad: true,
          response: {
            result: 'items',
            total: 'total',
            list: 'items',
          },
          showActiveMsg: true,
          showResponseMsg: false,
        },
        round: true,
        showOverflow: true,
        size: 'small',
      } as VxeTableGridOptions,
    });

    // 表格配置项可以用 cellRender: { name: 'CellImage' },
    vxeUI.renderer.add('CellImage', {
      renderTableDefault({ props }, { column, row }) {
        return h(Image, { src: row[column.field], ...props });
      },
    });

    // 表格配置项可以用 cellRender: { name: 'CellLink' },
    vxeUI.renderer.add('CellLink', {
      renderTableDefault({ attrs }) {
        return h(
          Button,
          { size: 'small', type: 'link' },
          { default: () => attrs?.text },
        );
      },
    });

    // 表格配置项可以用 cellRender: { name: 'CellSwitch', attrs: { beforeChange } },
    vxeUI.renderer.add('CellSwitch', {
      renderTableDefault({ attrs, props }, { column, row }) {
        const valueField = attrs?.valueField || props?.valueField || 'status';
        const loadingKey = `__loading_${column.field}`;
        async function onChange(newVal: any) {
          row[loadingKey] = true;
          try {
            const result = await attrs?.beforeChange?.(newVal, row);
            if (result !== false) {
              row[valueField] = newVal;
            }
          } finally {
            row[loadingKey] = false;
          }
        }
        // 关键修复：valueField 是数字 0/1 时，原来的 `!!row.status` 会把 0 正确判定为关
        // 但会把 row.status === undefined/NaN/null 等都判为关（视觉上跟真关一样）
        // 用 `String(x) === String(y)` 同时兼容 number 和 string 后端
        const checkedValue = props?.checkedValue ?? 1;
        const unCheckedValue = props?.unCheckedValue ?? 0;
        return h(Switch, {
          checked: String(row[valueField]) === String(checkedValue),
          checkedValue,
          unCheckedValue,
          checkedChildren: attrs?.checkedChildren || props?.checkedChildren,
          unCheckedChildren: attrs?.unCheckedChildren || props?.unCheckedChildren,
          loading: row[loadingKey] ?? false,
          'onUpdate:checked': onChange,
        });
      },
    });

    // 表格配置项可以用 cellRender: { name: 'CellTag', attrs: { colorMap, textMap } },
    vxeUI.renderer.add('CellTag', {
      renderTableDefault({ attrs, props }, { column, row }) {
        const value = row[column.field];
        const tagProps: Record<string, any> = {};
        if (attrs?.colorField || props?.colorField) {
          tagProps.color = row[attrs?.colorField || props?.colorField];
        } else if (attrs?.colorMap || props?.colorMap) {
          tagProps.color = (attrs?.colorMap || props?.colorMap)[value];
        }
        const textMap = attrs?.textMap || props?.textMap;
        return h(Tag, tagProps, {
          default: () => (textMap ? textMap[value] ?? String(value) : String(value)),
        });
      },
    });

    // 表格配置项可以用 cellRender: { name: 'CellOperation', options: [...], attrs: { onClick } },
    vxeUI.renderer.add('CellOperation', {
      renderTableDefault({ attrs, options, props }, { row }) {
        const onClick = attrs?.onClick || props?.onClick;
        const buttons = (options || []).map((opt: any) => {
          return h(
            Button,
            {
              danger: opt.danger,
              size: 'small',
              type: 'link',
              onClick: () => onClick?.({ code: opt.code, row }),
            },
            { default: () => opt.text },
          );
        });
        return h('div', { class: 'flex items-center justify-center gap-1' }, buttons);
      },
    });

    // 这里可以自行扩展 vxe-table 的全局配置，比如自定义格式化
    // vxeUI.formats.add
  },
  useVbenForm,
});

export const VbenTableAction = defineComponent({
  name: 'VbenTableAction',
  props: {
    actions: { type: Array, default: () => [] },
    dropdownActions: { type: Array, default: () => [] },
    align: { type: String, default: 'center' },
    outside: { type: Boolean, default: false },
  },
  setup(props, { slots, attrs }) {
    return () =>
      h(VbenTableActionCore, { hasPermission: true, ...props, ...attrs }, slots);
  },
});

export const useVbenVxeGrid = <T extends Record<string, any>>(
  ...rest: Parameters<typeof useGrid<T, ComponentType, ComponentPropsMap>>
) => useGrid<T, ComponentType, ComponentPropsMap>(...rest);

export type * from '@vben/plugins/vxe-table';
