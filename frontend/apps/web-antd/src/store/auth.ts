import type { Recordable, UserInfo } from '@vben/types';

import { ref } from 'vue';
import { useRouter } from 'vue-router';

import { LOGIN_PATH } from '@vben/constants';
import { preferences } from '@vben/preferences';
import { resetAllStores, useAccessStore, useUserStore } from '@vben/stores';

import { notification } from 'ant-design-vue';
import { defineStore } from 'pinia';

import { getAccessCodesApi, getUserInfoApi, loginApi, logoutApi } from '#/api';
import { loginLogApi } from '#/api/vision-archive/system';
import { $t } from '#/locales';

export const useAuthStore = defineStore('auth', () => {
  const accessStore = useAccessStore();
  const userStore = useUserStore();
  const router = useRouter();

  const loginLoading = ref(false);

  /**
   * 异步处理登录操作
   * Asynchronously handle the login process
   * @param params 登录表单数据
   */
  async function authLogin(
    params: Recordable<any>,
    onSuccess?: () => Promise<void> | void,
  ) {
    // 异步处理用户登录操作并获取 accessToken
    let userInfo: null | UserInfo = null;
    let loginSuccess = false;
    let failMessage = '';
    try {
      loginLoading.value = true;
      const { accessToken } = await loginApi(params);

      // 如果成功获取到 accessToken
      if (accessToken) {
        loginSuccess = true;
        accessStore.setAccessToken(accessToken);

        // 获取用户信息并存储到 accessStore 中
        const [fetchUserInfoResult, accessCodes] = await Promise.all([
          fetchUserInfo(),
          getAccessCodesApi(),
        ]);

        userInfo = fetchUserInfoResult;

        userStore.setUserInfo(userInfo);
        accessStore.setAccessCodes(accessCodes);

        if (accessStore.loginExpired) {
          accessStore.setLoginExpired(false);
        } else {
          onSuccess
            ? await onSuccess?.()
            : await router.push(preferences.app.defaultHomePath);
        }

        if (userInfo?.realName) {
          notification.success({
            description: `${$t('authentication.loginSuccessDesc')}:${userInfo?.realName}`,
            duration: 3,
            message: $t('authentication.loginSuccess'),
          });
        }
      }
    } catch (err: any) {
      // 记录失败原因
      failMessage =
        err?.response?.data?.message || err?.message || '登录失败';
      throw err;
    } finally {
      loginLoading.value = false;

      // ============== 写登录日志（fire-and-forget，不影响登录流程） ==============
      // 成功/失败都要记录，保证不同用户登录的痕迹都能追溯
      recordLoginLog({
        userName: (params?.username as string) || (params?.account as string) || 'unknown',
        status: loginSuccess ? 1 : 0,
        message: loginSuccess ? '登录成功' : failMessage,
      });
    }

    return {
      userInfo,
    };
  }

  /**
   * 写登录日志到后端 /api/system/log/login
   * <para>
   * 这里只负责调接口，不 await、不抛出异常——
   * 写日志失败不能让登录流程出问题，也不应该影响用户体验。
   * </para>
   */
  function recordLoginLog(payload: {
    userName: string;
    status: 0 | 1;
    message: string;
  }) {
    const { os, browser } = parseUserAgent(navigator.userAgent || '');
    loginLogApi
      .create({
        userName: payload.userName,
        status: payload.status,
        message: payload.message,
        os,
        browser,
      })
      .catch(() => {
        // 静默吞掉：日志写失败不应影响登录
      });
  }

  async function logout(redirect: boolean = true) {
    try {
      await logoutApi();
    } catch {
      // 不做任何处理
    }
    resetAllStores();
    accessStore.setLoginExpired(false);

    // 回登录页带上当前路由地址
    await router.replace({
      path: LOGIN_PATH,
      query: redirect
        ? {
            redirect: encodeURIComponent(router.currentRoute.value.fullPath),
          }
        : {},
    });
  }

  async function fetchUserInfo() {
    const userInfo = await getUserInfoApi();
    userStore.setUserInfo(userInfo);
    return userInfo;
  }

  function $reset() {
    loginLoading.value = false;
  }

  return {
    $reset,
    authLogin,
    fetchUserInfo,
    loginLoading,
    logout,
  };
});

// ============== 客户端 UA 解析（os / browser） ==============

/**
 * 从 navigator.userAgent 解析出 OS 和 Browser 的友好名称。
 * <para>
 * 不追求识别所有组合，覆盖主流即可（Windows / macOS / Linux / Android / iOS + Chrome / Edge / Safari / Firefox）。
 * </para>
 */
function parseUserAgent(ua: string): { os: string; browser: string } {
  const lower = ua.toLowerCase();
  const os = detectOs(lower);
  const browser = detectBrowser(lower, ua);
  return { os, browser };
}

function detectOs(lower: string): string {
  if (lower.includes('windows nt 10.0')) return 'Windows 10';
  if (lower.includes('windows nt 11.0') || lower.includes('windows nt 12.0'))
    return 'Windows 11';
  if (lower.includes('windows nt 6.3')) return 'Windows 8.1';
  if (lower.includes('windows nt 6.2')) return 'Windows 8';
  if (lower.includes('windows nt 6.1')) return 'Windows 7';
  if (lower.includes('windows')) return 'Windows';
  if (lower.includes('mac os x')) return 'macOS';
  if (lower.includes('android')) {
    const m = lower.match(/android\s([\d.]+)/);
    return m && m[1] ? `Android ${m[1]}` : 'Android';
  }
  if (lower.includes('iphone') || lower.includes('ipad') || lower.includes('ipod')) {
    const m = lower.match(/os\s([\d_]+)/);
    return m && m[1] ? `iOS ${m[1].replace(/_/g, '.')}` : 'iOS';
  }
  if (lower.includes('linux')) return 'Linux';
  if (lower.includes('ubuntu')) return 'Ubuntu';
  return 'Unknown';
}

function detectBrowser(lower: string, raw: string): string {
  // 顺序：Edge > Chrome > Safari > Firefox > IE
  if (lower.includes('edg/') || lower.includes('edge/')) {
    const m = raw.match(/Edg\/([\d.]+)/);
    const v = m && m[1] ? m[1].split('.')[0] : '';
    return v ? `Edge ${v}` : 'Edge';
  }
  if (lower.includes('chrome') || lower.includes('chromium')) {
    const m = raw.match(/Chrome\/([\d.]+)/);
    const v = m && m[1] ? m[1].split('.')[0] : '';
    return v ? `Chrome ${v}` : 'Chrome';
  }
  if (lower.includes('safari') && !lower.includes('chrome')) {
    const m = raw.match(/Version\/([\d.]+)/);
    const v = m && m[1] ? m[1].split('.')[0] : '';
    return v ? `Safari ${v}` : 'Safari';
  }
  if (lower.includes('firefox')) {
    const m = raw.match(/Firefox\/([\d.]+)/);
    const v = m && m[1] ? m[1].split('.')[0] : '';
    return v ? `Firefox ${v}` : 'Firefox';
  }
  if (lower.includes('msie') || lower.includes('trident')) {
    const m = raw.match(/(?:MSIE |rv:)([\d.]+)/);
    const v = m && m[1] ? m[1].split('.')[0] : '';
    return v ? `IE ${v}` : 'IE';
  }
  return 'Unknown';
}
