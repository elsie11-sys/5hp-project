-- 5 张 collect 表的 id 列改为 GENERATED ALWAYS AS IDENTITY
-- 这会让 EF Core / Npgsql 在 INSERT 时省略 id 列，让 PG 用 nextval
ALTER TABLE student_vision_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET GENERATED ALWAYS AS IDENTITY;

ALTER TABLE student_oral_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET GENERATED ALWAYS AS IDENTITY;

ALTER TABLE student_mental_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET GENERATED ALWAYS AS IDENTITY;

ALTER TABLE weight_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET GENERATED ALWAYS AS IDENTITY;

ALTER TABLE student_bone_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET GENERATED ALWAYS AS IDENTITY;

-- 把 sequence 起始值设为 max(id) + 1
SELECT setval(pg_get_serial_sequence('student_vision_record', 'id'), GREATEST(1000, COALESCE((SELECT MAX(id) FROM student_vision_record), 0) + 1));
SELECT setval(pg_get_serial_sequence('student_oral_record', 'id'), GREATEST(1000, COALESCE((SELECT MAX(id) FROM student_oral_record), 0) + 1));
SELECT setval(pg_get_serial_sequence('student_mental_record', 'id'), GREATEST(1000, COALESCE((SELECT MAX(id) FROM student_mental_record), 0) + 1));
SELECT setval(pg_get_serial_sequence('weight_record', 'id'), GREATEST(1000, COALESCE((SELECT MAX(id) FROM weight_record), 0) + 1));
SELECT setval(pg_get_serial_sequence('student_bone_record', 'id'), GREATEST(1000, COALESCE((SELECT MAX(id) FROM student_bone_record), 0) + 1));
