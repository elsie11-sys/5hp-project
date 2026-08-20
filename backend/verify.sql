SELECT table_name FROM information_schema.tables WHERE table_schema='public' ORDER BY table_name;
SELECT 'vision:' AS dim, COUNT(*) AS rows FROM student_vision_record
UNION ALL SELECT 'oral:', COUNT(*) FROM student_oral_record
UNION ALL SELECT 'mental:', COUNT(*) FROM student_mental_record
UNION ALL SELECT 'weight:', COUNT(*) FROM weight_record
UNION ALL SELECT 'bone:', COUNT(*) FROM student_bone_record;
SELECT "MigrationId" FROM "__EFMigrationsHistory" ORDER BY "MigrationId";
