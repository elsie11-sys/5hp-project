// Repro: 复现前端删除 204 / 404 的 interceptor 行为
import axios from 'axios';

function defaultResponseInterceptor({ codeField = 'code', dataField = 'data', successCode = 0 } = {}) {
  return {
    fulfilled: (response) => {
      const { config, data: responseData, status } = response;
      if (config.responseReturn === 'raw') return response;
      if (status >= 200 && status < 400) {
        if (config.responseReturn === 'body') return responseData;
        else if (typeof successCode === 'function'
          ? successCode(responseData[codeField])
          : responseData[codeField] === successCode) {
          return typeof dataField === 'function' ? dataField(responseData) : responseData[dataField];
        }
      }
      throw Object.assign({}, response, { response });
    },
  };
}

function errorMessageInterceptor(makeErrorMessage) {
  return {
    rejected: (error) => {
      const status = error?.response?.status;
      let errorMessage = '系统内部错误';
      switch (status) {
        case 400: errorMessage = 'Bad Request'; break;
        case 401: errorMessage = 'Unauthorized'; break;
        case 403: errorMessage = 'Forbidden'; break;
        case 404: errorMessage = 'Not Found'; break;
        case 408: errorMessage = 'Request Timeout'; break;
        default: errorMessage = '系统内部错误';
      }
      makeErrorMessage?.(errorMessage, error);
      return Promise.reject(error);
    },
  };
}

const client = axios.create();
client.interceptors.response.use(defaultResponseInterceptor({ codeField: 'code', dataField: 'data', successCode: 0 }));
client.interceptors.response.use(undefined, errorMessageInterceptor((msg, err) => {
  console.log('  [弹窗]', msg, '| status:', err?.response?.status, '| data:', JSON.stringify(err?.response?.data));
}));

async function tryDelete(id) {
  try {
    const r = await client.delete(`http://localhost:5224/api/health-archive/vision/${id}`);
    console.log(`  ok returned:`, JSON.stringify(r));
  } catch (e) {
    console.log(`  catch e.message =`, String(e.message).substring(0, 100));
    console.log(`  e.response?.status =`, e?.response?.status);
    console.log(`  e.response?.data =`, JSON.stringify(e?.response?.data));
  }
}

console.log('--- 场景1: DELETE /vision/1004 (后端 204 NoContent) ---');
await tryDelete(1004);

console.log('\n--- 场景2: DELETE /vision/999999 (后端 404 视力记录不存在) ---');
await tryDelete(999999);
