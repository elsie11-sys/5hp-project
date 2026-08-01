import { initPreferences, updatePreferences } from '@vben/preferences';
import { unmountGlobalLoading } from '@vben/utils';

import {
  DEFAULT_HOME_PATH,
  overridesPreferences,
  preferencesExtension,
} from './preferences';

/**
 * 应用初始化完成之后再进行页面加载渲染
 */
async function initApplication() {
  // name用于指定项目唯一标识
  // 用于区分不同项目的偏好设置以及存储数据的key前缀以及其他一些需要隔离的数据
  const env = import.meta.env.PROD ? 'prod' : 'dev';
  const appVersion = import.meta.env.VITE_APP_VERSION;
  const namespace = `${import.meta.env.VITE_APP_NAMESPACE}-${appVersion}-${env}`;

  // app偏好设置初始化
  await initPreferences({
    extension: preferencesExtension,
    namespace,
    overrides: overridesPreferences,
  });

  // 强制以代码配置为准，覆盖 localStorage 旧缓存中的 defaultHomePath。
  // preferences 的缓存合并策略为「缓存优先」（defu），旧版本缓存的 /analytics
  // 会覆盖 preferences.ts 中的 /vision/national。此处每次启动强制写回正确值，
  // 同时 updatePreferences 会把新值写回缓存，老用户下次读取到的也是新值。
  updatePreferences({ app: { defaultHomePath: DEFAULT_HOME_PATH } });

  // 启动应用并挂载
  // vue应用主要逻辑及视图
  const { bootstrap } = await import('./bootstrap');
  await bootstrap(namespace);

  // 移除并销毁loading
  unmountGlobalLoading();
}

initApplication();
