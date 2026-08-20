-- 重置 5 个 sequence 到 max(id) + 1
SELECT setval('student_vision_record_id_seq', GREATEST(1000, (SELECT MAX(id) FROM student_vision_record)) + 1);
SELECT setval('student_oral_record_id_seq', GREATEST(1000, (SELECT MAX(id) FROM student_oral_record)) + 1);
SELECT setval('student_mental_record_id_seq', GREATEST(1000, (SELECT MAX(id) FROM student_mental_record)) + 1);
SELECT setval('weight_record_id_seq', GREATEST(1000, (SELECT MAX(id) FROM weight_record)) + 1);
SELECT setval('student_bone_record_id_seq', GREATEST(1000, (SELECT MAX(id) FROM student_bone_record)) + 1);
SELECT currval('student_vision_record_id_seq');
SELECT nextval('student_vision_record_id_seq');
