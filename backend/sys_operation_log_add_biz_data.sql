-- 增量：给 sys_operation_log 加 biz_data 列
-- 适用场景：之前已经跑过 sys_operation_log_init.sql 建好表，现在要追加新列
-- 如果是新装，直接跑 sys_operation_log_init.sql 即可（已经包含本列）
ALTER TABLE sys_operation_log
    ADD COLUMN IF NOT EXISTS biz_data TEXT;
