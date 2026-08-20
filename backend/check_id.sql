SELECT table_name, column_name, is_identity, identity_generation, identity_start
FROM information_schema.columns
WHERE table_schema='public' AND column_name='id' AND table_name LIKE '%vision%' OR table_name LIKE '%oral%' OR table_name LIKE '%mental%' OR table_name LIKE '%weight%' OR table_name LIKE '%bone%';
