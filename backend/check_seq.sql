SELECT s.relname AS seq_name, s.last_value, s.is_called
FROM pg_class s
JOIN pg_attribute a ON a.attrelid = (SELECT oid FROM pg_class WHERE relname='student_vision_record') AND a.attname='id'
JOIN pg_depend d ON d.refobjid = s.oid AND d.refobjsubid = a.attnum
WHERE s.relkind='S';

SELECT pg_get_serial_sequence('student_vision_record', 'id');
SELECT last_value, is_called FROM student_vision_record_id_seq;
