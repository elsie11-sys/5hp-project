/**
 * 设备自动采集 - 设备端 SDK 调用 API
 *
 * 后端路由前缀：/api/device/v1
 * 鉴权：请求头 X-Device-Key
 * 5 维度端点：/vision/ingest, /oral/ingest, /mental/ingest, /weight/ingest, /bone/ingest
 *
 * 前端也可调用此模块触发"模拟设备推流"按钮，方便演示
 */
import { requestClient } from '#/api/request';

// 直接走 fetch，绕开业务请求拦截器（鉴权方式不同）
async function deviceRequest<T>(path: string, body: unknown): Promise<T> {
  const baseURL = (requestClient as any).options?.baseURL ?? '';
  const resp = await fetch(`${baseURL}${path}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'X-Device-Key': '5hp-device-default-key',
    },
    body: JSON.stringify(body),
  });
  const data = await resp.json();
  if (data?.code !== 0) {
    throw new Error(data?.message || `设备采集失败 (${resp.status})`);
  }
  return data.data as T;
}

export interface DeviceIngestRequest<T> {
  deviceSn: string;
  collectedAt?: string;
  recorderId?: number;
  recorderName?: string;
  items: T[];
}

export interface DeviceIngestResponse {
  total: number;
  success: number;
  failed: number;
  errors: string[];
}

// 5 维度的设备采集端点
export const collectDeviceApi = {
  vision: (req: DeviceIngestRequest<unknown>) => deviceRequest<DeviceIngestResponse>('/api/device/v1/vision/ingest', req),
  oral: (req: DeviceIngestRequest<unknown>) => deviceRequest<DeviceIngestResponse>('/api/device/v1/oral/ingest', req),
  mental: (req: DeviceIngestRequest<unknown>) => deviceRequest<DeviceIngestResponse>('/api/device/v1/mental/ingest', req),
  weight: (req: DeviceIngestRequest<unknown>) => deviceRequest<DeviceIngestResponse>('/api/device/v1/weight/ingest', req),
  bone: (req: DeviceIngestRequest<unknown>) => deviceRequest<DeviceIngestResponse>('/api/device/v1/bone/ingest', req),

  /** 设备探活（无需鉴权） */
  async ping(): Promise<{ code: number; message: string; serverTime: string; dimensions: string[] }> {
    const baseURL = (requestClient as any).options?.baseURL ?? '';
    const resp = await fetch(`${baseURL}/api/device/v1/ping`);
    return resp.json();
  },
};
