-- 删除旧表（如果存在）
DROP TABLE IF EXISTS sys_org CASCADE;

-- 使用引号标识符创建表（与 EF Core 生成的格式一致）
CREATE TABLE sys_org (
    "Id" BIGSERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Code" VARCHAR(50),
    "Level" VARCHAR(20) NOT NULL,
    "ParentId" BIGINT REFERENCES sys_org("Id") ON DELETE RESTRICT,
    "Sort" INTEGER NOT NULL DEFAULT 0,
    "Status" INTEGER NOT NULL DEFAULT 1,
    "CreatedAt" TIMESTAMPTZ DEFAULT NOW(),
    "UpdatedAt" TIMESTAMPTZ,
    "Remark" VARCHAR(500)
);

-- 索引
CREATE INDEX IX_sys_org_ParentId ON sys_org("ParentId");
CREATE INDEX IX_sys_org_Status ON sys_org("Status");

-- 种子数据
INSERT INTO sys_org ("Id", "Name", "Code", "Level", "ParentId", "Sort", "Status", "CreatedAt") VALUES
(1, '国家教育部', 'GJ-001', '国家级', NULL, 0, 1, '2024-01-01 00:00:00+00'),
(2, '江苏省教育厅', 'SJ-001', '省级', 1, 1, 1, '2024-01-01 00:00:00+00'),
(3, '浙江省教育厅', 'SJ-002', '省级', 1, 2, 1, '2024-01-01 00:00:00+00'),
(4, '南京市教育局', 'SHI-001', '市级', 2, 1, 1, '2024-01-01 00:00:00+00'),
(5, '苏州市教育局', 'SHI-002', '市级', 2, 2, 1, '2024-01-01 00:00:00+00'),
(6, '鼓楼区教育局', 'QX-001', '区县级', 4, 1, 1, '2024-01-01 00:00:00+00'),
(7, '南京市第一中学', 'XX-001', '学校级', 6, 1, 1, '2024-01-01 00:00:00+00'),
(8, '南京市金陵中学', 'XX-002', '学校级', 6, 2, 1, '2024-01-01 00:00:00+00');
