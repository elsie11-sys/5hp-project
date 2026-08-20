-- 5 张表：移除 IDENTITY（让 EF 8 知道这是普通 long），用 SEQUENCE + DEFAULT
-- 这样 EF Core 8 会发送显式 id=0，但 PG DEFAULT nextval 覆盖
ALTER TABLE student_vision_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET DEFAULT nextval('student_vision_record_id_seq');

ALTER TABLE student_oral_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET DEFAULT nextval('student_oral_record_id_seq');

ALTER TABLE student_mental_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET DEFAULT nextval('student_mental_record_id_seq');

ALTER TABLE weight_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET DEFAULT nextval('weight_record_id_seq');

ALTER TABLE student_bone_record
  ALTER COLUMN id DROP IDENTITY IF EXISTS,
  ALTER COLUMN id SET DEFAULT nextval('student_bone_record_id_seq');

-- 重置 sequence
SELECT setval('student_vision_record_id_seq', GREATEST((SELECT MAX(id) FROM student_vision_record), 1000));
SELECT setval('student_oral_record_id_seq', GREATEST((SELECT MAX(id) FROM student_oral_record), 1000));
SELECT setval('student_mental_record_id_seq', GREATEST((SELECT MAX(id) FROM student_mental_record), 1000));
SELECT setval('weight_record_id_seq', GREATEST((SELECT MAX(id) FROM weight_record), 1000));
SELECT setval('student_bone_record_id_seq', GREATEST((SELECT MAX(id) FROM student_bone_record), 1000));
