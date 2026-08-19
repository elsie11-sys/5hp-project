-- 系统操作日志表
-- 说明：
--   1) 每条记录代表一次"前端 → 后端"的 HTTP 调用，包含操作人、模块、类型、状态、耗时、请求/响应内容、客户端 IP 等
--   2) status 约定：1 = 成功，0 = 失败（与前端 OperationLogDto.status 对齐）
--   3) 通常由后端中间件/Filter 在响应阶段自动写入；本 SQL 只负责建表和索引
--   4) EF Core 的 Migrate() 会自动调用本 DDL，不需要手动执行
CREATE TABLE IF NOT EXISTS sys_operation_log (
    id              BIGSERIAL PRIMARY KEY,
    operator        VARCHAR(50)  NOT NULL,                       -- 操作人
    type            VARCHAR(20)  NOT NULL,                       -- login/create/update/delete/export/import/query/other
    module          VARCHAR(50),                                 -- 操作模块（学生档案 / 系统管理 ...）
    content         VARCHAR(500),                                -- 操作内容
    ip              VARCHAR(50),                                 -- 客户端 IP
    status          INTEGER      NOT NULL DEFAULT 1,              -- 1=成功 0=失败
    cost_ms         INTEGER,                                     -- 耗时（毫秒）
    request_url     VARCHAR(200),                                -- 请求地址
    request_method  VARCHAR(10),                                 -- GET / POST / PUT / DELETE
    method          VARCHAR(200),                                -- Controller 方法签名
    request_params  TEXT,                                        -- 请求参数（JSON）
    response_params TEXT,                                        -- 返回参数（JSON）
    error_msg       TEXT,                                        -- 失败时的错误信息
    user_agent      VARCHAR(500),                                -- 浏览器 UA
    location        VARCHAR(100),                                -- IP 归属地
    biz_data        TEXT,                                        -- 业务上下文 JSON（Controller 通过 HttpContext.Items["BizData"] 注入；未注入时由 Filter 填默认摘要）
    created_at      TIMESTAMPTZ  DEFAULT NOW()
);

-- 索引：按操作人 / 类型 / 状态 / 时间的高频筛选
CREATE INDEX IF NOT EXISTS IX_sys_operation_log_operator    ON sys_operation_log(operator);
CREATE INDEX IF NOT EXISTS IX_sys_operation_log_type        ON sys_operation_log(type);
CREATE INDEX IF NOT EXISTS IX_sys_operation_log_status      ON sys_operation_log(status);
CREATE INDEX IF NOT EXISTS IX_sys_operation_log_created_at  ON sys_operation_log(created_at DESC);
