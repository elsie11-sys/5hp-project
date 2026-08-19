-- 系统登录日志表
-- 说明：
--   1) 每条记录代表一次"用户尝试登录系统"的行为
--   2) status 约定：1 = 成功，0 = 失败（与前端 LoginLogDto.status 对齐）
--   3) 通常由前端在登录成功/失败时调用 POST /api/system/log/login 写入
--   4) EF Core 的 Migrate() 会自动调用本 DDL，不需要手动执行
CREATE TABLE IF NOT EXISTS sys_login_log (
    id              BIGSERIAL PRIMARY KEY,
    user_name       VARCHAR(50)  NOT NULL,                       -- 用户名
    ip              VARCHAR(50),                                 -- 登录地址（IP）
    location        VARCHAR(100),                                -- 登录地点
    os              VARCHAR(50),                                 -- 操作系统
    browser         VARCHAR(50),                                 -- 浏览器
    status          INTEGER      NOT NULL DEFAULT 1,              -- 1=成功 0=失败
    message         VARCHAR(500),                                -- 描述（登录成功 / 失败原因）
    login_time      TIMESTAMPTZ  DEFAULT NOW()
);

-- 索引：按用户 / IP / 状态 / 时间的高频筛选
CREATE INDEX IF NOT EXISTS IX_sys_login_log_user_name  ON sys_login_log(user_name);
CREATE INDEX IF NOT EXISTS IX_sys_login_log_ip         ON sys_login_log(ip);
CREATE INDEX IF NOT EXISTS IX_sys_login_log_status     ON sys_login_log(status);
CREATE INDEX IF NOT EXISTS IX_sys_login_log_login_time ON sys_login_log(login_time DESC);
