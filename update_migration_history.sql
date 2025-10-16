-- Cập nhật migration history
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20251015153036_InitialMigration', '8.0.0') 
ON CONFLICT DO NOTHING;

