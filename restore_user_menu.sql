-- 重建被测试删掉的"用户管理"目录 + 三个按钮
INSERT INTO sys_menu (id, name, code, icon, type, parent_id, sort, status, path, component, permission, is_external, is_keep_alive, is_visible, created_at, updated_at, remark)
VALUES (2, '用户管理', 'USER', 'ant-design:user', 2, 1, 1, 1, '/vision-system/user', 'vision-archive/system/user/index', 'system:user:list', 0, 1, 1, '2026-08-15 11:12:27+00', NULL, NULL);

INSERT INTO sys_menu (id, name, code, icon, type, parent_id, sort, status, path, component, permission, is_external, is_keep_alive, is_visible, created_at, updated_at, remark)
VALUES
  (100, '用户新增', NULL, NULL, 3, 2, 1, 1, NULL, NULL, 'system:user:add', 0, 1, 1, '2026-08-15 11:12:27+00', NULL, NULL),
  (101, '用户修改', NULL, NULL, 3, 2, 2, 1, NULL, NULL, 'system:user:edit', 0, 1, 1, '2026-08-15 11:12:27+00', NULL, NULL),
  (102, '用户删除', NULL, NULL, 3, 2, 3, 1, NULL, NULL, 'system:user:delete', 0, 1, 1, '2026-08-15 11:12:27+00', NULL, NULL);

SELECT setval('sys_menu_id_seq', (SELECT MAX(id) FROM sys_menu));
